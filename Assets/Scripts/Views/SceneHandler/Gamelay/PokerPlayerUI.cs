using Puppet;
using Puppet.Poker.Datagram;
using Puppet.Poker.Models;
using System;
using System.Collections.Generic;
using UnityEngine;
using Puppet.Poker;
using System.Collections;
using Puppet.Core.Model;
using Puppet.Service;
using Puppet.Core.Network.Http;

public class PokerPlayerUI : MonoBehaviour
{
    #region UNITY EDITOR
    public UITexture texture;
    public UILabel labelCurrentGold;
    public UILabel labelUsername;
    public UISlider timerSlider;
    public GameObject btnGift;
    public UISprite spriteResultIcon;
    #endregion

    [HideInInspector]
    public GameObject[] cardOnHands;
    PokerPlayerController data;
    PokerGameplayPlaymat playmat;
    [HideInInspector]
    public PokerGPSide side;
    public PokerPotItem currentBet;
    public string UserName
    {
        get { return data.userName; }
    }

    void Awake()
    {
        this.SetResult(false);
        playmat = GameObject.FindObjectOfType<PokerGameplayPlaymat>();
    }

    void OnEnable()
    {
        PokerObserver.Instance.onTurnChange += Instance_dataTurnGame;
        PokerObserver.Instance.onFinishGame += Instance_onFinishGame;
        PokerObserver.Instance.onUpdatePot += Instance_onUpdatePot;
        UIEventListener.Get(gameObject).onClick += OnOpenProfile;
        PuMain.Dispatcher.onChatMessage += ShowMessage;
    }

    void OnDisable()
    {
        PokerObserver.Instance.onTurnChange -= Instance_dataTurnGame;
        PokerObserver.Instance.onFinishGame -= Instance_onFinishGame;
        PokerObserver.Instance.onUpdatePot -= Instance_onUpdatePot;
        PuMain.Dispatcher.onChatMessage -= ShowMessage;
        UIEventListener.Get(gameObject).onClick -= OnOpenProfile;
    }
    private void OnOpenProfile(GameObject gobj) {
        if (!PokerObserver.Instance.IsMainPlayer (UserName)) {
			DialogService.Instance.ShowDialog (new DialogPlayerInfo (data));
		} else {
			UserInfo userInfo = Puppet.API.Client.APIUser.GetUserInformation ();

			DialogService.Instance.ShowDialog(new DialogShortProfile(userInfo));
		}
    }
    private void ShowMessage(DataChat message)
    {
        if (message.GetChatType() == DataChat.ChatType.Public)
        {
            if (message.Sender.userName == UserName)
            {
                if (message.Content.IndexOf(DialogGameplayChatView.EMOTICON_CODE) != 0)
                {
                    PokerGameplayPlayerChat.Create(message.Content, this);
                }
                else
                {
                    if (message.Content.Contains(DialogGameplayChatView.EMOTICON_STICKER_CODE))
                    {
                        string name = "S_" + message.Content.Split('_')[1];
                        EmoticonSticker.Create(name, gameObject.transform);
                    }
                    else if (message.Content.Contains(DialogGameplayChatView.EMOTICON_ANIMATION_CODE))
                    {
                        string name = message.Content.Split('-')[1];
                        EmoticonAnimation.Create(name, gameObject.transform);
                    }
                }
            }
        }
    }
    void UpdateUI(PokerPlayerController player)
    {
        if (player != null && player.userName == data.userName)
        {
            double money = player.GetMoney();
            labelCurrentGold.text = money > 0 ? money.ToString("#,###") : "All In";

            string customTitle = string.Empty;
            if (PokerObserver.Instance.isWaitingFinishGame || (PokerObserver.Game.CurrentPlayer != null && PokerObserver.Game.CurrentPlayer.userName == player.userName))
                labelUsername.text = data.userName;
            else if (PokerObserver.Game.ListPlayerWaitNextGame.Contains(player.userName) || (PokerObserver.Game.ListPlayerInGame.Count > 0 && player.GetPlayerState() == PokerPlayerState.none))
                customTitle = "Chờ ván mới";
            else if (PokerObserver.Game.IsPlayerInGame(player.userName))
            {
                if (player.GetPlayerState() == Puppet.Poker.PokerPlayerState.check)
                    customTitle = PokerObserver.Game.GetTimesInteractiveInRound(player.userName) > 0 ? "Xem bài" : "Chờ đặt cược";
                else if (player.GetPlayerState() == Puppet.Poker.PokerPlayerState.fold)
                    customTitle = "Bỏ bài";
                else if (player.currentBet == 0)
                    customTitle = "Chờ đặt cược";
                else if (player.GetPlayerState() == Puppet.Poker.PokerPlayerState.bigBlind)
                    customTitle = "Big Blind";
                else if (player.GetPlayerState() == Puppet.Poker.PokerPlayerState.smallBlind)
                    customTitle = "Small Blind";
                else if (player.GetPlayerState() == Puppet.Poker.PokerPlayerState.call)
                    customTitle = "Theo cược";
                else if (player.GetPlayerState() == Puppet.Poker.PokerPlayerState.allIn)
                    customTitle = "All-in";
                else if (player.GetPlayerState() == Puppet.Poker.PokerPlayerState.raise)
                    customTitle = "Thêm cược";
                else
                    labelUsername.text = data.userName;
            }
            else
                labelUsername.text = data.userName;

            if (!string.IsNullOrEmpty(customTitle))
                labelUsername.text = string.Format("[FFFF50]{0}[-]", customTitle);

            if(player.currentBet > 0)
                LoadCurrentBet(player.currentBet);

            if (PokerObserver.Game.Dealer == player.userName && PokerObserver.Game.IsPlayerInGame(player.userName))
                playmat.SetDealerObjectToPlayer(player);
        }
    }

    public void SetTitle(string title)
    {
        labelUsername.text = string.IsNullOrEmpty(title) ? data.userName : title;
    }

    void Instance_onFinishGame(ResponseFinishGame data)
    {
        StopTimer();

        if (currentBet != null)
            NGUITools.SetActive(currentBet.gameObject, false);

        ResponseFinishCardPlayer cardPlayer = Array.Find<ResponseFinishCardPlayer>(data.players, p => p.userName == this.data.userName);
        if (cardPlayer != null && cardPlayer.cards != null)
        {
            if (PokerObserver.Instance.IsMainPlayer(cardPlayer.userName))
                return;

            //FP-128 Người chơi bỏ bài, chờ ván mới, đứng xem không nhìn thấy bài tay của những người chơi so bài
            bool isFaceUp = PokerObserver.Instance.GetTotalPlayerNotFold() > 1;// PokerObserver.Game.IsMainPlayerInGame && PokerObserver.Game.MainPlayer.GetPlayerState() != PokerPlayerState.fold;
            if (isFaceUp)
            {
                for (int i = 0; i < cardPlayer.cards.Length; i++)
                {
                    cardOnHands[i].GetComponent<PokerCardObject>().SetDataCard(new PokerCard(cardPlayer.cards[i]));
                    cardOnHands[i].transform.parent = side.positionCardGameEnd[i].transform;
                    cardOnHands[i].transform.localRotation = Quaternion.identity;
                    cardOnHands[i].transform.localPosition = Vector3.zero;
                    cardOnHands[i].transform.localScale = Vector3.one;
                }
            }
        }
    }

    private void Instance_dataTurnGame(ResponseUpdateTurnChange responseData)
    {
        NGUITools.SetActive(timerSlider.gameObject, responseData.toPlayer != null && responseData.toPlayer.userName == this.data.userName);
        if (responseData.toPlayer != null && responseData.toPlayer.userName == this.data.userName)
            StartTimer(responseData.time > 1000 ? responseData.time / 1000f : responseData.time);
        else
            StopTimer();
    }

    void Instance_onUpdatePot(ResponseUpdatePot obj)
    {
        NGUITools.SetActive(currentBet.gameObject, false);
    }

    void LoadCurrentBet(double value)
    {
        if (side != null)
        {
            if (currentBet == null)
            {
                currentBet = NGUITools.AddChild(side.positionMoney, playmat.prefabBetObject).GetComponent<PokerPotItem>();
                NGUITools.SetActive(currentBet.gameObject, false);
            }
            else
            {
                currentBet.transform.parent = side.positionMoney.transform;
                currentBet.transform.localPosition = Vector3.zero;
            }
        }

        if (currentBet != null)
            addBetAnim();
    }
    public void addMoneyToMainPot() {
        currentBet.labelCurrentbet.transform.parent.gameObject.SetActive(false);
        currentBet.gameObject.transform.parent = playmat.potContainer.tablePot.transform.parent.transform;
        iTween.MoveTo(currentBet.gameObject, iTween.Hash("islocal",true,"position", playmat.potContainer.tablePot.transform.localPosition, "time", 3.0f, "oncomplete", "onMoneyToMainPotComplete", "oncompletetarget", gameObject));
    }
    void onMoneyToMainPotComplete()
    {
        currentBet.transform.localPosition = Vector3.zero;
        currentBet.labelCurrentbet.transform.parent.gameObject.SetActive(true);
        StartCoroutine(SetCurrentBet());
    }

    IEnumerator SetCurrentBet()
    {
        yield return new WaitForSeconds(data.currentBet > 0 ? 1.0f : 0f);
        if (data.currentBet > 0)
            NGUITools.SetActive(currentBet.gameObject, true);
        currentBet.SetBet(data.currentBet);
    }
    PokerPotItem betAnim;
    void addBetAnim()
    {
        //Logger.Log("Betting Player: " + data.userName + " - " + data.currentBet);
        tweenComplete();
        if (currentBet.CurrentBet < data.currentBet)
        {
            if (betAnim == null)
                betAnim = NGUITools.AddChild(gameObject, playmat.prefabBetObject).GetComponent<PokerPotItem>();
            NGUITools.SetActive(betAnim.gameObject, true);
            betAnim.labelCurrentbet.transform.parent.gameObject.SetActive(false);
            iTween.MoveTo(betAnim.gameObject, iTween.Hash("position", side.positionMoney.transform.localPosition, "islocal", true, "time", 1.0f, "oncomplete", "tweenComplete", "oncompletetarget", gameObject));
        }
        StartCoroutine(SetCurrentBet());
    }
    void tweenComplete()
    {
        if (betAnim != null)
        {
            betAnim.gameObject.transform.localPosition = transform.localPosition;
            NGUITools.SetActive(betAnim.gameObject, false);
        }
    }
    public void SetResult(bool isWinner)
    {
        NGUITools.SetActive(spriteResultIcon.gameObject, isWinner);
    }

    void OnDestroy()
    {
        if (currentBet != null)
        {
            currentBet.transform.parent = playmat.transform;
            playmat.MarkerPot(currentBet);
        }

        data.onDataChanged -= playerModel_onDataChanged;
    }

    public void SetData(PokerPlayerController player)
    {
        bool addEvent = data == null;
        this.data = player;
        if (addEvent)
            data.onDataChanged += playerModel_onDataChanged;
        UpdateUI(player);
		WWWRequest request = new WWWRequest (this, data.avatar, 5, 3);
		request.isFullUrl = true;
		request.onResponse = delegate(IHttpRequest arg1, IHttpResponse arg2) {
			WWWResponse response = (WWWResponse)arg2;
			if(response.www.texture !=null)
				texture.mainTexture = response.www.texture;
		};
		request.Start (null);
        Vector3 giftPosition = btnGift.transform.localPosition;
        if ((int)player.GetSide() > (int)Puppet.Poker.PokerSide.Slot_5)
        {
            float x = Math.Abs(giftPosition.x);
            giftPosition.x = x;
        }
        btnGift.transform.localPosition = giftPosition;
    }

    void playerModel_onDataChanged()
    {
        UpdateUI(data);
    }
    public void UpdateSetCardObject(GameObject cardOnHands,int index)
    {

        try
        {
            this.cardOnHands[index] = cardOnHands;
        }
        catch (Exception)
        {
            this.cardOnHands = new GameObject[2];
        }
        finally
        {
            this.cardOnHands[index] = cardOnHands;
        }
        

        cardOnHands.transform.parent = PokerObserver.Instance.IsMainPlayer(data.userName) ? side.positionCardMainPlayer[index].transform : side.positionCardFaceCards[index].transform;
        cardOnHands.transform.localRotation = Quaternion.identity;
        cardOnHands.transform.localPosition = Vector3.zero;
        cardOnHands.transform.localScale = PokerObserver.Instance.IsMainPlayer(data.userName) ? Vector3.one : Vector3.one / 3;
    }
    public void UpdateSetCardObject(GameObject[] cardOnHands)
    {
        this.cardOnHands = cardOnHands;

        for (int i = 0; i < cardOnHands.Length; i++)
        {
            cardOnHands[i].transform.parent = PokerObserver.Instance.IsMainPlayer(data.userName) ? side.positionCardMainPlayer[i].transform : side.positionCardFaceCards[i].transform;
            cardOnHands[i].transform.localRotation = Quaternion.identity;
            cardOnHands[i].transform.localPosition = Vector3.zero;
            cardOnHands[i].transform.localScale = PokerObserver.Instance.IsMainPlayer(data.userName) ? Vector3.one : Vector3.one / 3;
        }
    }

    #region TIMER
    float totalCountDown = 0f;
    float timeCountDown = 0f;
    float realtime = 0f;
    public void StartTimer(float time, float remainingTime = 0f)
    {
        if (time > 0)
        {
            totalCountDown = timeCountDown = time;
            if (remainingTime > 0)
                timeCountDown = remainingTime;
            realtime = Time.realtimeSinceStartup;
        }
    }
    void StopTimer()
    {
        timeCountDown = -1f;
        realtime = 0f;
        timerSlider.value = 0;
    }

    const float START_COUNTDOWN_SOUND_FROM = 6f;
    float COUNTDOWN_ONE_SECOND = 0f;
    #endregion

    void Update()
    {
        if (timeCountDown >= 0 && realtime >= 0)
        {
            timeCountDown -= (Time.realtimeSinceStartup - realtime);
            realtime = Time.realtimeSinceStartup;
            timerSlider.value = timeCountDown / totalCountDown;

            if(timeCountDown <= START_COUNTDOWN_SOUND_FROM && PokerObserver.Instance.IsMainTurn)
            {
                if (COUNTDOWN_ONE_SECOND <= 0f)
                {
                    COUNTDOWN_ONE_SECOND = 1f;
                    PuSound.Instance.Play(SoundType.Countdown);
                }
                else
                    COUNTDOWN_ONE_SECOND -= Time.deltaTime;
            }
        }
    }

}
