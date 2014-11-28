﻿using Puppet.Poker.Datagram;
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
//		DataLobby lobby = APIGeneric.SelectedLobby();
//		Logger.Log
//		double smallBind = lobby.gameDetails.betting / 2;
//		lbTitle.text = "Phòng : " + lobby.roomId + " - $" + smallBind+"/"+lobby.gameDetails.betting;
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
        //UIEventListener.Get(btnRule).onClick += OnButtonRuleClickCallBack;
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
        //UIEventListener.Get(btnRule).onClick -= OnButtonRuleClickCallBack;
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

    private void OnButtonRuleClickCallBack(GameObject go)
    {
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
}

