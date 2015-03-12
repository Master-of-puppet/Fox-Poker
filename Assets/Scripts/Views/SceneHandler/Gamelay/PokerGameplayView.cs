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
    public GameObject btnGameMini, btnRule, btnSendMessage;
	public GameObject btnViewCheckBox, btnFollowBetCheckBox, btnFollowAllBetCheckbox;
    public UILabel lbMessage;
	public UILabel lbTime,lbTitle;
    public PokerGameplayPlaymat playmat;


    public List<DataChat> dataChat;
    #endregion

    void Awake()
    {
        HeaderMenuView.Instance.ShowInGameplay(OnClickQuitGame, OnButtonClickStandUp);
        dataChat = new List<DataChat>();
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
        PokerGameDetails gameDetails = PokerObserver.Game.gameDetails;
        RoomInfo roomInfo = APIGeneric.SelectedRoomJoin();
        if (gameDetails != null)
        {
			double smallBind = gameDetails.customConfiguration.SmallBlind;
            lbTitle.text = "Phòng : " + roomInfo.roomId + " - $" + smallBind + "/" + gameDetails.customConfiguration.MaxBlind;
		}
	}
    void OnEnable() 
    {
        PokerObserver.Instance.onEncounterError += Instance_onEncounterError;
        //UIEventListener.Get(btnGameMini).onClick += OnButtonGameMiniClickCallBack;
        UIEventListener.Get(btnRule).onClick += OnButtonRuleClickCallBack;
        UIEventListener.Get(btnSendMessage).onClick += OnButtonSendMessageClickCallBack;
        PuMain.Dispatcher.onChatMessage += ShowMessage;
    }

    void OnDisable()
    {
        PokerObserver.Instance.onEncounterError -= Instance_onEncounterError;
        //UIEventListener.Get(btnGameMini).onClick -= OnButtonGameMiniClickCallBack;
        UIEventListener.Get(btnRule).onClick -= OnButtonRuleClickCallBack;
        UIEventListener.Get(btnSendMessage).onClick -= OnButtonSendMessageClickCallBack;
        PuMain.Dispatcher.onChatMessage -= ShowMessage;
    }
    private void ShowMessage(DataChat message) {
        if (message.GetChatType() == DataChat.ChatType.Public)
        {
            if (message.Content.IndexOf(DialogGameplayChatView.EMOTICON_CODE) != 0)
            {
                dataChat.Add(message);
                if (message.GetChatType() == DataChat.ChatType.Public)
                {
                    lbMessage.text = message.Sender.userName + " : " + message.Content;
                }
            }
        }
        else { 
            
        }
    }
    void Instance_onEncounterError(ResponseError data)
    {
        if(data.showPopup)
        {
            DialogService.Instance.ShowDialog(new DialogMessage("Error: " + data.errorCode, data.errorMessage, null));
        }
    }

    private void OnButtonSendMessageClickCallBack(GameObject go)
    {
        DialogService.Instance.ShowDialog(new DialogGameplayChat(dataChat));
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

    private void OnClickQuitGame()
    {
        string text = "Bạn có chắc chắn muốn thoát khỏi bàn chơi?";
        if (PokerObserver.Game.MainPlayer != null && PokerObserver.Game.MainPlayer.GetPlayerState() != Puppet.Poker.PokerPlayerState.fold
            && (PokerObserver.Game.MainPlayer.currentBet > 0 || PokerObserver.Game.DealComminityCards.Count > 0))
            text += "\nNếu bạn thoát, sẽ mất hết số tiền đã cược";

        DialogService.Instance.ShowDialog(new DialogConfirm("XÁC NHẬN THOÁT", text, delegate(bool? confirm)
        {
            if (confirm == true)
                PuApp.Instance.BackScene();
        }));
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

    void OnGUI()
    {
        if (!PokerObserver.Game.IsMainPlayerInGame && PokerObserver.Game.MainPlayer != null && PokerObserver.Game.MainPlayer.isMaster)
        {
            if (GUI.Button(new Rect(0, 150, Screen.width - Screen.width * 0.9f, 35f), "TEST MODE"))
            {
                Logger.Log("========> " + APIPokerGame.GetPokerGameplay().ListPlayer.Count);
                TestModeGUI.Create(ActionRequestOrderHand);
            }
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

