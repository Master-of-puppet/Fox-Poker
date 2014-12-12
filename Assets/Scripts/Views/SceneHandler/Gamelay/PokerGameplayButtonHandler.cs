using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet.Poker;
using System;
using Puppet.Poker.Models;
using Puppet.Poker.Datagram;
using Puppet;
using Puppet.Service;
using System.Linq;
using Puppet.Core.Model;

public class PokerGameplayButtonHandler : MonoBehaviour 
{
    public ButtonStepData [] dataButtons;
    public ButtonItem[] itemButtons;

    public enum EButtonSlot
    {
        First = 0,
        Second = 1,
        Third = 2,
    }

    public enum EButtonType
    {
        InTurn = 0,
        OutTurn = 1,
        OutGame = 2,
        InGame = 3,
    }

    [Serializable()]
    public class ButtonStepData
    {
        public string text;
        public EButtonSlot slot;
        public EButtonType type;
        public Vector3 labelPosition;
        public int labelFontSize;
        public bool enableCheckBox;
        public Vector3 checkBoxPosition;
    }

    [Serializable()]
    public class ButtonItem
    {
        public EButtonSlot slot;
        public GameObject button;
        public UILabel label;
        public UIToggle toggle;
        public GameObject checkboxObject;
    }

    EButtonType currentType;

    DialogBetting bettingDialog;

    void Start()
    {
        SetEnableButtonType(EButtonType.OutGame);
    }

    void OnEnable()
    {
        PokerObserver.Instance.onTurnChange += Instance_dataTurnGame;
        PokerObserver.Instance.onUpdatePot += Instance_onUpdatePot;
        PokerObserver.Instance.onNewRound += Instance_onNewRound;
        PokerObserver.Instance.onFinishGame += Instance_onFinishGame;
        PokerObserver.Instance.onPlayerListChanged += Instance_onPlayerListChanged;

        foreach(ButtonItem item in itemButtons)
        {
            if (item.slot == EButtonSlot.First)
                UIEventListener.Get(item.button).onClick += OnClickButton1;
            else if(item.slot == EButtonSlot.Second)
                UIEventListener.Get(item.button).onClick += OnClickButton2;
            else if (item.slot == EButtonSlot.Third)
                UIEventListener.Get(item.button).onClick += OnClickButton3;
        }
    }

    void OnDisable()
    {
        PokerObserver.Instance.onTurnChange -= Instance_dataTurnGame;
        PokerObserver.Instance.onUpdatePot -= Instance_onUpdatePot;
        PokerObserver.Instance.onNewRound -= Instance_onNewRound;
        PokerObserver.Instance.onFinishGame -= Instance_onFinishGame;
        PokerObserver.Instance.onPlayerListChanged -= Instance_onPlayerListChanged;

        foreach (ButtonItem item in itemButtons)
        {
            if (item.slot == EButtonSlot.First)
                UIEventListener.Get(item.button).onClick -= OnClickButton1;
            else if (item.slot == EButtonSlot.Second)
                UIEventListener.Get(item.button).onClick -= OnClickButton2;
            else if (item.slot == EButtonSlot.Third)
                UIEventListener.Get(item.button).onClick -= OnClickButton3;
        }
    }

    void OnClickButton1(GameObject go)
    {
        if(currentType == EButtonType.InTurn)
        {
            if(PokerObserver.Game.MaxCurrentBetting == 0)
                Puppet.API.Client.APIPokerGame.PlayRequest(PokerRequestPlay.CHECK, 0);
            else
            {
                double diff = PokerObserver.Instance.CurrentBettingDiff;
                if(diff > 0)
                    Puppet.API.Client.APIPokerGame.PlayRequest(PokerRequestPlay.CALL, diff);
                else if(diff == 0)
                    Puppet.API.Client.APIPokerGame.PlayRequest(PokerRequestPlay.CHECK, 0);
                else
                    Logger.LogError("Call Betting INVALID");
            }
        }
    }
    void OnClickButton2(GameObject go)
    {
        if (currentType == EButtonType.InTurn)
        {
            Puppet.API.Client.APIPokerGame.PlayRequest(PokerRequestPlay.FOLD, 0);
        }
    }

    void OnClickButton3(GameObject go)
    {
        if (currentType == EButtonType.InTurn)
        {
			bettingDialog = new DialogBetting(GetMaxBinded(), GetMaxRaise(),(money) =>
            {
                Puppet.API.Client.APIPokerGame.PlayRequest(PokerRequestPlay.RAISE, money);
            }, Array.Find<ButtonItem>(itemButtons, button => button.slot == EButtonSlot.Third).button.transform);

            if(DialogService.Instance.IsShowing(bettingDialog) == false)
                DialogService.Instance.ShowDialog(bettingDialog);
        }
        else if(currentType == EButtonType.OutGame)
        {
            Puppet.API.Client.APIPokerGame.AutoSitDown(PokerObserver.Instance.gameDetails.customConfiguration.SmallBlind * 20);
        }
    }
	double GetMaxBinded(){
		double maxBinded = PokerObserver.Game.ListPlayer
			.Max<PokerPlayerController>(p => p.currentBet);
		if (PokerObserver.Game.MainPlayer.currentBet != 0)
			maxBinded = maxBinded - PokerObserver.Game.MainPlayer.currentBet;
		return maxBinded;
	}
	double GetMaxRaise()
    {
        if (PokerObserver.Game.MainPlayer.GetMoney() <= 0)
            return 0;
        int countPlayerUnAllIn = PokerObserver.Game.ListPlayer.FindAll(p => PokerObserver.Game.IsPlayerInGame(p.userName) && p.userName != PokerObserver.Game.MainPlayer.userName && p.GetPlayerState() != PokerPlayerState.fold && p.GetPlayerState() != PokerPlayerState.allIn).Count;
        if (countPlayerUnAllIn == 0) //Nếu tất cả các người chơi khác đã All-In thì mình chỉ được theo cược
            return 0;
        
        double maxOtherMoney = PokerObserver.Game.ListPlayer
			.Where<PokerPlayerController>(p => p.userName != PokerObserver.Game.MainPlayer.userName && p.GetPlayerState() != PokerPlayerState.fold )
                .Max<PokerPlayerController>(p => p.GetMoney() + p.currentBet);
        double maxBinded = PokerObserver.Game.ListPlayer.Where<PokerPlayerController>(p => p.GetPlayerState() != PokerPlayerState.fold).Max<PokerPlayerController>(p => p.currentBet);
		double myMoney = PokerObserver.Game.MainPlayer.GetMoney() +  PokerObserver.Game.MainPlayer.currentBet;
		double maxRaise = myMoney;
		if(myMoney > maxOtherMoney)
			maxRaise = maxOtherMoney;
		if (PokerObserver.Game.MainPlayer.currentBet != 0)
			maxRaise = maxRaise - PokerObserver.Game.MainPlayer.currentBet;
        if (maxRaise <= PokerObserver.Game.MaxCurrentBetting)
            return 0;

		return maxRaise;
	}
    void SetEnableButtonType(EButtonType type)
    {
        this.currentType = type;
        ButtonStepData[] buttonData = Array.FindAll<ButtonStepData>(dataButtons, b => b.type == type);
        foreach(ButtonItem item in itemButtons)
        {
            ButtonStepData data = Array.Find<ButtonStepData>(buttonData, b => b.slot == item.slot);
            NGUITools.SetActive(item.button, data != null);
            if (data != null)
            {
                bool enableButton = EnableButton(type, item.slot);
                item.button.collider.enabled = enableButton;
                item.button.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, enableButton ? 1f : 0.45f);
                string moreText = AddMoreTextButton(type, item.slot);
                string overrideName = OverrideName(type, item.slot);
                item.label.text = (overrideName ?? data.text) + (string.IsNullOrEmpty(moreText) ? string.Empty : string.Format("\n({0})", moreText));
                item.label.fontSize = data.labelFontSize;
                item.label.transform.localPosition = data.labelPosition;
                NGUITools.SetActive(item.checkboxObject.gameObject, data.enableCheckBox);
                item.checkboxObject.transform.localPosition = data.checkBoxPosition;
                item.button.GetComponent<UIToggle>().enabled = data.enableCheckBox;
            }
        }
    }

    #region CUSTOM BUTTON
    string AddMoreTextButton(EButtonType type, EButtonSlot slot)
    {
        if (PokerObserver.Instance.gameDetails != null)
        {
            if (slot == EButtonSlot.Third && type == EButtonType.OutGame)
                return (PokerObserver.Instance.gameDetails.customConfiguration.SmallBlind * 20).ToString("#,##");

            if ((type == EButtonType.InTurn || type == EButtonType.OutTurn) && slot == EButtonSlot.First)
            {
                double diff = PokerObserver.Instance.CurrentBettingDiff;
                if(diff > 0)
                    return diff.ToString("#,##");
            }
        }
        return null;
    }
    string OverrideName(EButtonType type, EButtonSlot slot)
    {
        if (slot == EButtonSlot.First && type == EButtonType.InTurn)
        {
            if(PokerObserver.Game.MaxCurrentBetting == 0 || PokerObserver.Instance.CurrentBettingDiff == 0)
                return "XEM BÀI";
        }
        return null;
    }

    bool EnableButton(EButtonType type, EButtonSlot slot)
    {
        if (slot == EButtonSlot.Third && type == EButtonType.InTurn) {
			try 
            {
                if (GetMaxRaise() <= 0)
					return false;				
				else
                    return PokerObserver.Game.MainPlayer.GetMoney() + PokerObserver.Game.MainPlayer.currentBet >= PokerObserver.Game.MaxCurrentBetting;
			} catch (Exception ex) {
                return PokerObserver.Game.MainPlayer.GetMoney() + PokerObserver.Game.MainPlayer.currentBet >= PokerObserver.Game.MaxCurrentBetting;
			}
        }
        else if (type == EButtonType.OutTurn && PokerObserver.Game.MainPlayer.GetMoney() == 0)
			return false;
        return true;
    }
    #endregion

    void Instance_onPlayerListChanged(ResponsePlayerListChanged data)
    {
        if (PokerObserver.Instance.IsMainPlayer(data.player.userName))
        {
            switch (data.GetActionState())
            {
                case PokerPlayerChangeAction.playerAdded:
                    SetEnableButtonType(EButtonType.InGame);
                    break;
                case PokerPlayerChangeAction.waitingPlayerAdded:
                    SetEnableButtonType(EButtonType.OutGame);
                    break;
            }
        }
    }

    void Instance_onUpdatePot(ResponseUpdatePot data)
    {
        SetEnableButtonType(currentType);
    }

    void Instance_dataTurnGame(ResponseUpdateTurnChange data)
    {
        if (PokerObserver.Instance.IsMainPlayerSatDown())
        {
            if(PokerObserver.Instance.isWaitingFinishGame || !PokerObserver.Game.IsMainPlayerInGame)
                SetEnableButtonType(EButtonType.InGame);
            else if (data.toPlayer != null)
            {
                ButtonItem selectedButton = Array.Find<ButtonItem>(itemButtons, button => button.toggle.value);

                SetEnableButtonType(PokerObserver.Instance.IsMainTurn ? EButtonType.InTurn : EButtonType.OutTurn);

                if (selectedButton != null)
                {
                    if (selectedButton.slot == EButtonSlot.First && data.action == "Call")
                        OnClickButton1(selectedButton.button);
                    else if (selectedButton.slot == EButtonSlot.Second)
                        OnClickButton2(selectedButton.button);
                    else if (selectedButton.slot == EButtonSlot.Third)
                        OnClickButton1(selectedButton.button);

                    selectedButton.toggle.value = false;
                }
            }
            else
                SetEnableButtonType(EButtonType.InGame);
        }
    }

    void Instance_onNewRound(ResponseWaitingDealCard data)
    {
        if (PokerObserver.Instance.IsMainPlayerSatDown())
            SetEnableButtonType(EButtonType.InGame);
    }

    void Instance_onFinishGame(ResponseFinishGame obj)
    {
        if (PokerObserver.Instance.IsMainPlayerSatDown())
            SetEnableButtonType(EButtonType.InGame);
    }
}