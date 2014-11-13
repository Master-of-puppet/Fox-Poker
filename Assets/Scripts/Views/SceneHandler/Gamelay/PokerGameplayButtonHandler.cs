﻿using UnityEngine;
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
            Puppet.API.Client.APIPokerGame.PlayRequest(PokerRequestPlay.CALL, PokerObserver.Instance.MaxCurrentBetting);
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
            bettingDialog = new DialogBetting(PokerObserver.Instance.MaxCurrentBetting, Convert.ToInt32(PokerObserver.Instance.mainPlayer.asset.GetAsset(EAssets.Chip).value), (money) =>
            {
                Puppet.API.Client.APIPokerGame.PlayRequest(PokerRequestPlay.RAISE, money);
            }, Array.Find<ButtonItem>(itemButtons, button => button.slot == EButtonSlot.Third).button.transform);

            if(DialogService.Instance.IsShowing(bettingDialog) == false)
                DialogService.Instance.ShowDialog(bettingDialog);
        }
        else if(currentType == EButtonType.OutGame)
        {
            UserInfo userInfo = Puppet.API.Client.APIUser.GetUserInformation ();
            DialogService.Instance.ShowDialog(new DialogBuyChip(PokerObserver.Instance.gameDetails.customConfiguration.SmallBlind, userInfo.assets.content[0].value));
            //Puppet.API.Client.APIPokerGame.AutoSitDown(PokerObserver.Instance.gameDetails.customConfiguration.SmallBlind * 20);
        }
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

    string AddMoreTextButton(EButtonType type, EButtonSlot slot)
    {
        if (PokerObserver.Instance.gameDetails != null)
        {
            if (slot == EButtonSlot.Third && type == EButtonType.OutGame)
                return (PokerObserver.Instance.gameDetails.customConfiguration.SmallBlind * 20).ToString("#,##");

            if ((type == EButtonType.InTurn || type == EButtonType.OutTurn) && slot == EButtonSlot.First)
            {
                double difference = PokerObserver.Instance.Difference;
                if (difference > 0)
                    return difference.ToString("#,##");
            }
        }
        return null;
    }
    string OverrideName(EButtonType type, EButtonSlot slot)
    {
        if (slot == EButtonSlot.First && type == EButtonType.InTurn && PokerObserver.Instance.Difference == 0)
            return "Xem Bài";
        return null;
    }

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

    void Instance_dataTurnGame(ResponseUpdateTurnChange data)
    {
        if (PokerObserver.Instance.IsMainPlayerInGame())
        {
            if (data.toPlayer != null)
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
        if (PokerObserver.Instance.IsMainPlayerInGame())
            SetEnableButtonType(EButtonType.InGame);
    }

    void Instance_onFinishGame(ResponseFinishGame obj)
    {
        if (PokerObserver.Instance.IsMainPlayerInGame())
            SetEnableButtonType(EButtonType.InGame);
    }
}