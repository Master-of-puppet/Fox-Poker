using Puppet.Poker.Datagram;
using Puppet.Poker.Models;
using Puppet.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Puppet;
using Puppet.API.Client;
using Puppet.Core.Model;
using HoldemHand;


public class PokerGameplayView : MonoBehaviour
{
    #region UnityEditor
    public GameObject btnView, btnLeaveTurn, btnAddBet, btnFollowBet, btnConvertMoney, btnGameMini, btnRule, btnSendMessage;
	public GameObject btnViewCheckBox, btnFollowBetCheckBox, btnFollowAllBetCheckbox;
    public UIInput txtMessage;
	public UILabel lbTime,lbTitle;
    public PokerGameplayPlaymat playmat;

    #endregion

    void Awake()
    {
        HeaderMenuView.Instance.ShowInGameplay();
        HeaderMenuView.Instance.handleStandUp = OnButtonClickStandUp;
    }
    void FixedUpdate() {
        if (lbTime != null)
        {
            string theTime = System.DateTime.Now.ToString("hh:mm tt");
            lbTime.text = theTime;
        }
    }
    IEnumerator Start()
    {
        //For Ensure all was init!!!!!
        yield return new WaitForSeconds(0.5f);
        PokerObserver.Instance.StartGame();
		showInfoGame ();
    }
	private void showInfoGame(){
		DataLobby lobby = APIGeneric.SelectedLobby();
		if (lobby != null) {
			double smallBind = lobby.gameDetails.betting / 2;
			lbTitle.text = "Phòng : " + lobby.roomId + " - $" + smallBind+"/"+lobby.gameDetails.betting;
		}
	}
    void OnEnable() 
    {
        PokerObserver.Instance.onEncounterError += Instance_onEncounterError;
        //UIEventListener.Get(btnView).onClick += OnButtonViewClickCallBack;
        //UIEventListener.Get(btnLeaveTurn).onClick += OnButtonLeaveTurnClickCallBack;
        //UIEventListener.Get(btnAddBet).onClick += OnButtonAddBetClickCallBack;
        //UIEventListener.Get(btnFollowBet).onClick += OnButtonFollowBetClickCallBack;
        //UIEventListener.Get(btnConvertMoney).onClick += OnButtonConvertMoneyClickCallBack;
        //UIEventListener.Get(btnGameMini).onClick += OnButtonGameMiniClickCallBack;
        UIEventListener.Get(btnRule).onClick += OnButtonRuleClickCallBack;
        //UIEventListener.Get(btnSendMessage).onClick += OnButtonSendMessageClickCallBack;
        //UIEventListener.Get(headerMenuBtnBack).onClick += OnButtonHeaderBackClickCallBack;
        //UIEventListener.Get(headerMenuBtnUp).onClick += OnButtonHeaderUpClickCallBack;
        //UIEventListener.Get(headerMenuBtnRecharge).onClick += OnButtonHeaderRechargeClickCallBack;
        //UIEventListener.Get(headerMenuBtnSettings).onClick += OnButtonHeaderSettingClickCallBack;

    }

    void OnDisable()
    {
        PokerObserver.Instance.onEncounterError -= Instance_onEncounterError;
        
        //UIEventListener.Get(btnView).onClick -= OnButtonViewClickCallBack;
        //UIEventListener.Get(btnLeaveTurn).onClick -= OnButtonLeaveTurnClickCallBack;
        //UIEventListener.Get(btnAddBet).onClick -= OnButtonAddBetClickCallBack;
        //UIEventListener.Get(btnFollowBet).onClick -= OnButtonFollowBetClickCallBack;
        //UIEventListener.Get(btnConvertMoney).onClick -= OnButtonConvertMoneyClickCallBack;
        //UIEventListener.Get(btnGameMini).onClick -= OnButtonGameMiniClickCallBack;
        UIEventListener.Get(btnRule).onClick -= OnButtonRuleClickCallBack;
        //UIEventListener.Get(btnSendMessage).onClick -= OnButtonSendMessageClickCallBack;
        //UIEventListener.Get(headerMenuBtnBack).onClick -= OnButtonHeaderBackClickCallBack;
        //UIEventListener.Get(headerMenuBtnUp).onClick -= OnButtonHeaderUpClickCallBack;
        //UIEventListener.Get(headerMenuBtnRecharge).onClick -= OnButtonHeaderRechargeClickCallBack;
        //UIEventListener.Get(headerMenuBtnSettings).onClick -= OnButtonHeaderSettingClickCallBack;
    }

    void Instance_onEncounterError(ResponseError data)
    {
        if(data.showPopup)
        {
            DialogService.Instance.ShowDialog(new DialogMessage("Error: " + data.errorCode, data.errorMessage, null));
        }
    }

    void OnButtonQuitClick(GameObject go)
    {
    }

    private void OnButtonHeaderSettingClickCallBack(GameObject go)
    {
    }

    private void OnButtonHeaderRechargeClickCallBack(GameObject go)
    {
    }

    private void OnButtonHeaderUpClickCallBack(GameObject go)
    {
    }

    private void OnButtonHeaderBackClickCallBack(GameObject go)
    {
    }

    private void OnButtonSendMessageClickCallBack(GameObject go)
    {
    }

    private void OnButtonClickStandUp(GameObject go)
    {
        if(PokerObserver.Game.IsMainPlayerInGame)
        {
            string text = "Bạn có chắc muốn đứng dậy ?";
            if(PokerObserver.Game.MainPlayer.GetPlayerState() != Puppet.Poker.PokerPlayerState.fold && (PokerObserver.Game.MainPlayer.currentBet > 0 || PokerObserver.Game.DealComminityCards.Count > 0))
                text += "\nNếu bạn đứng dậy sẽ mất hết số tiền đã cược";

            DialogService.Instance.ShowDialog(new DialogConfirm("WARNING", text, (action) =>
            {
                if(action == true)
                    Puppet.API.Client.APIPokerGame.StandUp();
            }));
        }
        else
            Puppet.API.Client.APIPokerGame.StandUp();
    }

    private void OnButtonRuleClickCallBack(GameObject go)
    {
        double[] player = new double[9];
        double[] opponent = new double[9];
        if (playmat.pocket != null && playmat.pocket.Count > 0)
        {
            string boards = HandEvaluatorConvert.ConvertPokerCardsToString(PokerObserver.Game.DealComminityCards);
            string pocket = HandEvaluatorConvert.ConvertPokerCardsToString(playmat.pocket);
            DialogService.Instance.ShowDialog(new DialogGameplayRankModel(pocket,boards));
        }

    }

    private void OnButtonGameMiniClickCallBack(GameObject go)
    {
    }

    private void OnButtonConvertMoneyClickCallBack(GameObject go)
    {
    }

    private void OnButtonFollowBetClickCallBack(GameObject go)
    {
    }

    private void OnButtonAddBetClickCallBack(GameObject go)
    {
    }

    private void OnButtonLeaveTurnClickCallBack(GameObject go)
    {
    }

    private void OnButtonViewClickCallBack(GameObject go)
    {
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 150, Screen.width - Screen.width * 0.9f, 35f), "TEST MODE"))
        {
            Logger.Log("========> " + APIPokerGame.GetPokerGameplay().ListPlayer.Count);
            TestModeGUI.Create(ActionRequestOrderHand);
        }   
    }
    public void ActionRequestOrderHand(Dictionary<string, int[]> obj)
    {
        foreach (var item in obj.Keys)
        {
            Logger.Log("========> keys" + item + " --- value lenght " + obj[item].Length);
            foreach (var items in obj[item])
            {
                Logger.Log("========> id" + items );    
            }
           
        }
        Dictionary<string, int[]> dictHand = obj;
        if (dictHand.ContainsKey(TestModeGUI.KEY_COMUTITY_CARD))
        {
            Dictionary<string,int[]> comunityCard = new Dictionary<string,int[]>();
            /// Request COMUNITY CARD;
            APIPokerGame.GetOrderCommunity(dictHand[TestModeGUI.KEY_COMUTITY_CARD]);
            dictHand.Remove(TestModeGUI.KEY_COMUTITY_CARD);
        }
        APIPokerGame.GetOrderHand(dictHand);        
    }
    private string FormatPercent(double v)
    {
        if (v != 0.0)
        {
            if (v * 100.0 >= 1.0)
                return string.Format("{0:##0.0}%", v * 100.0);
            else
                return "<1%";
        }
        return "n/a";
    }

    
}

