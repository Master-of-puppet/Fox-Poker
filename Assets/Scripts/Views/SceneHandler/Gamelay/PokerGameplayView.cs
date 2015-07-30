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
    public PokerGameplayPlaymat playmat;
    public GameObject btnShareFacebook, btnCloseLayoutShareFacebook;
    public GameObject btnGameMini, btnRule, btnSendMessage;
	public GameObject btnViewCheckBox, btnFollowBetCheckBox, btnFollowAllBetCheckbox;
    public UILabel lbMessage;
	public UILabel lbTime,lbTitle;
    public UILabel lbCountdown;
    public UILabel lbMyRanking;
    #endregion

    private const string ITEM_INTERACTION_PREFIX = "PII";
    private List<PokerCard> listMyPokerCard = new List<PokerCard>();
    private List<DataChat> dataChat;
    private float timeStartGame;
    private string winWithRank;

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
        LoadingView.Instance.Show(false);
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
        PokerObserver.Game.onFirstTimeJoinGame += Game_onFirstTimeJoinGame;
        PokerObserver.Instance.onNewRound += Instance_onNewRound;
        PokerObserver.Instance.onEventUpdateHand += Instance_onEventUpdateHand;
        PokerObserver.Instance.onPlayerListChanged += Instance_onPlayerListChanged;
        
        UIEventListener.Get(btnRule).onClick += OnButtonRuleClickCallBack;
        UIEventListener.Get(btnSendMessage).onClick += OnButtonSendMessageClickCallBack;
        UIEventListener.Get(btnShareFacebook).onClick += OnClickButtnShowDialogShare;
        UIEventListener.Get(btnCloseLayoutShareFacebook).onClick += OnClickButtonCloseLayoutShareFacebook;

        PuMain.Dispatcher.onChatMessage += ShowMessage;
        PuMain.Dispatcher.onChatMessage += onShowMessage;
    }

    void OnDisable()
    {
        PokerObserver.Instance.onEncounterError -= Instance_onEncounterError;
        PokerObserver.Game.onFirstTimeJoinGame -= Game_onFirstTimeJoinGame;
        PokerObserver.Instance.onNewRound -= Instance_onNewRound;
        PokerObserver.Instance.onEventUpdateHand -= Instance_onEventUpdateHand;
        PokerObserver.Instance.onPlayerListChanged -= Instance_onPlayerListChanged;
        
        UIEventListener.Get(btnRule).onClick -= OnButtonRuleClickCallBack;
        UIEventListener.Get(btnSendMessage).onClick -= OnButtonSendMessageClickCallBack;
        UIEventListener.Get(btnShareFacebook).onClick -= OnClickButtnShowDialogShare;
        UIEventListener.Get(btnCloseLayoutShareFacebook).onClick -= OnClickButtonCloseLayoutShareFacebook;
      
        PuMain.Dispatcher.onChatMessage -= ShowMessage;
        PuMain.Dispatcher.onChatMessage -= onShowMessage;
    }
    
    private void ShowMessage(DataChat message) 
    {
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
    }

    private void onShowMessage(DataChat message)
    {
        if (message.GetChatType() == DataChat.ChatType.Private)
        {
            string itemInteraction = message.Content;
            if (itemInteraction.IndexOf(ITEM_INTERACTION_PREFIX) == 0)
            {
                PokerPlayerUI sender = playmat.GetPlayerUI(message.Sender.userName);
                PokerPlayerUI receiver = playmat.GetPlayerUI(message.ReceiverName);
                GameObject pointTo = new GameObject();
                pointTo.name = "Point To";
                pointTo.transform.parent = receiver.transform;
                pointTo.transform.localScale = Vector3.one;
                pointTo.transform.localPosition = Vector3.zero;
                pointTo.transform.parent = gameObject.transform;
                Vector3 pointMoveTo = pointTo.transform.localPosition;
                GameObject.Destroy(pointTo);

                string nameSprite2D = message.Content.Split('_')[1];
                Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/ItemInteractions/" + nameSprite2D);
                GameObject pointFrom = new GameObject();
                pointTo.name = "Point From";
                pointFrom.transform.parent = sender.transform;
                pointFrom.transform.localScale = Vector3.one;
                pointFrom.transform.localPosition = Vector3.zero;
                pointFrom.transform.parent = gameObject.transform;

                pointFrom.AddComponent<UI2DSprite>().sprite2D = sprites[0];
                pointFrom.GetComponent<UI2DSprite>().depth = 20;
                pointFrom.GetComponent<UI2DSprite>().MakePixelPerfect();

                pointFrom.AddComponent<UI2DSpriteAnimation>().frames = Array.FindAll<Sprite>(sprites, sp => sp.name.Contains(nameSprite2D));
                pointFrom.GetComponent<UI2DSpriteAnimation>().ignoreTimeScale = false;
                pointFrom.GetComponent<UI2DSpriteAnimation>().framesPerSecond = 5;
                pointFrom.GetComponent<UI2DSpriteAnimation>().loop = true;
                pointFrom.GetComponent<UI2DSpriteAnimation>().Play();
                pointFrom.name = nameSprite2D;
                Hashtable tweenValue = new Hashtable();
                tweenValue.Add("item", pointFrom);
                tweenValue.Add("spriteArray", Array.FindAll<Sprite>(sprites, sp => sp.name.Contains("finish")));
                iTween.MoveTo(pointFrom, iTween.Hash("islocal", true, "position", pointMoveTo, "time", 1.5f, "oncomplete", "onMoveItemInteractionComplete", "oncompletetarget", gameObject, "oncompleteparams", tweenValue));
            }
        }
    }
    void onMoveItemInteractionComplete(object vals)
    {
        Hashtable table = (Hashtable)vals;
        GameObject animationObject = (GameObject)table["item"];
        Sprite[] sprites = (Sprite[])table["spriteArray"];
        animationObject.GetComponent<UI2DSpriteAnimation>().Pause();
        animationObject.GetComponent<UI2DSpriteAnimation>().frames = sprites;
        animationObject.GetComponent<UI2DSpriteAnimation>().loop = false;
        animationObject.GetComponent<UI2DSpriteAnimation>().Play();
        SoundType type = (SoundType)Enum.Parse(typeof(SoundType), animationObject.name);
        PuSound.Instance.Play(type);
        StartCoroutine(destroyItemInteractive(animationObject, 2f));
    }
    IEnumerator destroyItemInteractive(GameObject gobj, float time)
    {
        yield return new WaitForSeconds(time);
        GameObject.Destroy(gobj);
    }

    public void HideHandType()
    {
        lbMyRanking.text = string.Empty;
    }
    public void ShowRank()
    {
        if (listMyPokerCard.Count == 0)
            return;
        string pocketHand = HandEvaluatorConvert.ConvertPokerCardsToString(listMyPokerCard);
        string boards = HandEvaluatorConvert.ConvertPokerCardsToString(PokerObserver.Game.DealComminityCards);
        int count = 0;
        double[] player = new double[9];
        double[] opponent = new double[9];
        if (!Hand.ValidateHand(pocketHand + " " + boards))
        {
            lbMyRanking.text = string.Empty;
            return;
        }
        Hand.ParseHand(pocketHand + " " + boards, ref count);

        // Don't allow these configurations because of calculation time.
        if (count == 0 || count == 1 || count == 3 || count == 4 || count > 7)
        {
            lbMyRanking.text = string.Empty;
            return;
        }
        Hand.HandPlayerOpponentOdds(pocketHand, boards, ref player, ref opponent);
        var indexAtMax = player.ToList().IndexOf(player.Max());
        string myRank = "";
        switch ((Hand.HandTypes)indexAtMax)
        {
            case Hand.HandTypes.HighCard:
            case Hand.HandTypes.Pair:
            case Hand.HandTypes.TwoPair:
                myRank = "Hai đôi : " + FormatPercent(player[2]);
                break;
            case Hand.HandTypes.Trips:
                myRank = "Ba lá : " + FormatPercent(player[indexAtMax]);
                break;
            case Hand.HandTypes.Straight:
                myRank = "Sảnh  : " + FormatPercent(player[indexAtMax]);
                break;
            case Hand.HandTypes.Flush:
                myRank = "Đồng hoa  : " + FormatPercent(player[indexAtMax]);
                break;
            case Hand.HandTypes.FullHouse:
                myRank = "Cù lũ  : " + FormatPercent(player[indexAtMax]);
                break;
            case Hand.HandTypes.FourOfAKind:
                myRank = "Tứ quý  : " + FormatPercent(player[indexAtMax]);
                break;
            case Hand.HandTypes.StraightFlush:
                myRank = "Sảnh thông  : " + FormatPercent(player[indexAtMax]);
                break;
        }
        lbMyRanking.text = myRank;
    }

    void Instance_onEncounterError(ResponseError data)
    {
        if(data.showPopup)
            DialogService.Instance.ShowDialog(new DialogMessage("Error: " + data.errorCode, data.errorMessage, null));
    }

    void Instance_onPlayerListChanged(ResponsePlayerListChanged data)
    {
        if (PokerObserver.Game.ListPlayer.Count <= 1)
            ResetCountdown();
    }

    void Game_onFirstTimeJoinGame(ResponseUpdateGame data)
    {
        if(data.players.Length == 0)
            SetCountDown(data.remainingTime, data.totalTime);
    }

    void Instance_onEventUpdateHand(ResponseUpdateHand data)
    {
        ResetCountdown();

        listMyPokerCard.Clear();
        for (int i = 0; i < data.hand.Length; i++)
            listMyPokerCard.Add(new PokerCard(data.hand[i]));
        ShowRank();
    }

    void Instance_onNewRound(ResponseWaitingDealCard data)
    {
        SetCountDown(data.time, data.time);
    }

    private void OnClickButtnShowDialogShare(GameObject go)
    {
        DialogService.Instance.ShowDialog(new DialogGameplayShare(string.Format("Bạn đã đạt {0} hãy chia sẻ để mọi người cùng biết", winWithRank), string.Empty, string.Empty));
    }

    public IEnumerator ShowBtnShareFacebook(string rank, float timeAutoClose)
    {
        this.winWithRank = rank;
        btnShareFacebook.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeAutoClose);
        OnClickButtonCloseLayoutShareFacebook(null);
    }

    private void OnClickButtonCloseLayoutShareFacebook(GameObject go)
    {
        btnShareFacebook.transform.parent.gameObject.SetActive(false);
    }

    private void OnButtonSendMessageClickCallBack(GameObject go)
    {
        DialogService.Instance.ShowDialog(new DialogGameplayChat(dataChat));
    }

    private void OnButtonClickStandUp(GameObject go)
    {
        HideDialogBetting();
        if(PokerObserver.Game.IsMainPlayerInGame)
        {
            string text = "Bạn có chắc muốn đứng dậy ?";
            if(PokerObserver.Game.MainPlayer.GetPlayerState() != Puppet.Poker.PokerPlayerState.fold && (PokerObserver.Game.MainPlayer.currentBet > 0 || PokerObserver.Game.DealComminityCards.Count > 0))
                text += "\nNếu bạn đứng dậy sẽ mất hết số tiền đã cược";

            DialogService.Instance.ShowDialog(new DialogConfirm("WARNING", text, (action) =>
            {
                if (action == true)
                    PokerObserver.Instance.StandUp();
            }));
        }
        else
            PokerObserver.Instance.StandUp();
    }
    public void HideDialogBetting()
    {
        if (GameObject.Find("DialogBettingView") != null)
        {
            GameObject.Destroy(GameObject.Find("DialogBettingView"));
        }
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
            {
                Puppet.API.Client.APIPokerGame.QuitGame();
                PuApp.Instance.BackScene();
            }
        }));
    
    }

    private void OnButtonRuleClickCallBack(GameObject go)
    {
        HideDialogBetting();
        double[] player = new double[9];
        double[] opponent = new double[9];
        if (listMyPokerCard != null && listMyPokerCard.Count > 0)
        {
            string boards = HandEvaluatorConvert.ConvertPokerCardsToString(PokerObserver.Game.DealComminityCards);
            string pocket = HandEvaluatorConvert.ConvertPokerCardsToString(listMyPokerCard);
            DialogService.Instance.ShowDialog(new DialogGameplayRankModel(pocket,boards));
        }
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

    #region TEST MODE - ORDER CARD
    /*
    void OnGUI()
    {
        if (!PokerObserver.Game.IsMainPlayerInGame && PokerObserver.Game.MainPlayer != null && PokerObserver.Game.MainPlayer.isMaster)
        {
            if (GUI.Button(new Rect(0, 150, Screen.width - Screen.width * 0.9f, 35f), "TEST MODE"))
                TestModeGUI.Create(ActionRequestOrderHand);
        }
    }
    */
    public void ActionRequestOrderHand(Dictionary<string, int[]> obj)
    {
        foreach (var item in obj.Keys)
        {
            Logger.Log("========> keys" + item + " --- value lenght " + obj[item].Length);
            foreach (var items in obj[item])
                Logger.Log("========> id" + items );    
        }
        Dictionary<string, int[]> dictHand = obj;
        if (dictHand.ContainsKey(TestModeGUI.KEY_COMUTITY_CARD))
        {
            Dictionary<string,int[]> comunityCard = new Dictionary<string,int[]>();
            APIPokerGame.GetOrderCommunity(dictHand[TestModeGUI.KEY_COMUTITY_CARD]);
            dictHand.Remove(TestModeGUI.KEY_COMUTITY_CARD);
        }
        APIPokerGame.GetOrderHand(dictHand);        
    }
    #endregion

    #region COUNT DOWN TIMER START GAME
    void Update()
    {
        float countdown = timeStartGame - Time.realtimeSinceStartup;
        if (countdown > 0)
        {
            int second = Mathf.FloorToInt(countdown);
            lbCountdown.text = second.ToString();

            if (second == 0)
            {
                lbCountdown.fontSize = 60;
                lbCountdown.text = "Bắt đầu";
            }
            else if (second < 0)
            {
                ResetCountdown();
            }
        }
    }
    void OnDestroy()
    {
        HideDialogBetting();
    }
    void SetCountDown(int remainingTime, int totalTime)
    {
        timeStartGame = Time.realtimeSinceStartup + (remainingTime / 1000f);
    }

    void ResetCountdown()
    {
        timeStartGame = -1;
        lbCountdown.fontSize = 100;
        lbCountdown.text = string.Empty;
    }
    #endregion
}

