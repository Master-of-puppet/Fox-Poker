using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet.Poker;
using System;
using System.Linq; 
using Puppet.Poker.Models;
using Puppet.Poker.Datagram;
using Puppet;
using Puppet.Service;
using Puppet.Utils;
using HoldemHand;
using Puppet.Core.Model;

public class PokerGameplayPlaymat : MonoBehaviour
{
    #region UNITY EDITOR
    public GameObject prefabBetObject;
    public GameObject positionEffectDealCard;
    public Transform []positionDealCards;
    public PokerPotManager potContainer;
    public GameObject objectDealer;
    public UILabel lbMyRanking;
    public UILabel lbCountdown;
    #endregion
    PokerGPSide[] arrayPokerSide;
    Dictionary<string, GameObject> dictPlayerObject = new Dictionary<string, GameObject>();
    List<PokerPotItem> _listPotMarkers = new List<PokerPotItem>();
    float timeStartGame;
    private static string ITEM_INTERACTION_PREFIX = "PII";
    void Awake()
    {
        objectDealer.SetActive(false);

        arrayPokerSide = GameObject.FindObjectsOfType<PokerGPSide>();

        PokerObserver.Game.onFirstTimeJoinGame += Game_onFirstTimeJoinGame;
        PokerObserver.Instance.onPlayerListChanged += Instance_onPlayerListChanged;
        PokerObserver.Instance.dataUpdateGameChange += Instance_dataUpdateGame;
        PokerObserver.Instance.onEventUpdateHand += Instance_onEventUpdateHand;
        PokerObserver.Instance.onTurnChange += Instance_dataTurnGame;
        PokerObserver.Instance.onNewRound += Instance_onNewRound;
        PokerObserver.Instance.onUpdatePot += Instance_onUpdatePot;
        PokerObserver.Instance.onFinishGame += Instance_onFinishGame;
        PokerObserver.Instance.onUpdateRoomMaster += Instance_onUpdateRoomMaster;
        PuMain.Dispatcher.onChatMessage += onShowMessage;
    }

    void OnDestroy()
    {
        PokerObserver.Game.onFirstTimeJoinGame -= Game_onFirstTimeJoinGame;
        PokerObserver.Instance.onPlayerListChanged -= Instance_onPlayerListChanged;
        PokerObserver.Instance.dataUpdateGameChange -= Instance_dataUpdateGame;
        PokerObserver.Instance.onEventUpdateHand -= Instance_onEventUpdateHand;
        PokerObserver.Instance.onTurnChange -= Instance_dataTurnGame;
        PokerObserver.Instance.onNewRound -= Instance_onNewRound;
        PokerObserver.Instance.onUpdatePot -= Instance_onUpdatePot;
        PokerObserver.Instance.onFinishGame -= Instance_onFinishGame;
        PokerObserver.Instance.onUpdateRoomMaster -= Instance_onUpdateRoomMaster;
        PuMain.Dispatcher.onChatMessage -= onShowMessage;
    }
    private void onShowMessage(DataChat message)
    {
        if (message.GetChatType() == DataChat.ChatType.Private) {
            string itemInteraction = message.Content;
            if (itemInteraction.IndexOf(ITEM_INTERACTION_PREFIX) == 0)
            {
                PokerPlayerUI sender = dictPlayerObject[message.Sender.userName].GetComponent<PokerPlayerUI>();
                PokerPlayerUI receiver = dictPlayerObject[message.ReceiverName].GetComponent<PokerPlayerUI>();
                GameObject pointTo = new GameObject();
                pointTo.name = "Point To";
                pointTo.transform.parent = receiver.transform;
                pointTo.transform.localScale  = Vector3.one;
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

                pointFrom.AddComponent<UI2DSpriteAnimation>().frames = Array.FindAll<Sprite>(sprites,sp=>sp.name.Contains(nameSprite2D));
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
    public void onMoveItemInteractionComplete(object vals)
    {
        Hashtable table = (Hashtable)vals;
        GameObject animationObject =(GameObject) table["item"];
        Sprite[] sprites = (Sprite[] )table["spriteArray"];
        animationObject.GetComponent<UI2DSpriteAnimation>().Pause();
        animationObject.GetComponent<UI2DSpriteAnimation>().frames = sprites;
        animationObject.GetComponent<UI2DSpriteAnimation>().loop = false;
        animationObject.GetComponent<UI2DSpriteAnimation>().Play();
		SoundType type = (SoundType)Enum.Parse(typeof(SoundType),animationObject.name);
		PuSound.Instance.Play (type);
        StartCoroutine(destroyItemInteractive(animationObject, 2f));

    }
    IEnumerator destroyItemInteractive(GameObject gobj, float time) {
        yield return new WaitForSeconds(time);
        GameObject.Destroy(gobj);
    }
    void Instance_onUpdatePot(ResponseUpdatePot obj)
    {
        UnMarkPot();
        if (!PokerObserver.Instance.isWaitingFinishGame && obj.pot != null && obj.pot.Length > 0 && obj.pot[0].value > 0)
        {
            // foreach (PokerPlayerController controller in PokerObserver.Game.ListPlayer)
            // {
            //      dictPlayerObject[controller.userName].GetComponent<PokerPlayerUI>().addMoneyToMainPot();
            // }
            //StartCoroutine(updatePotView(obj));
            potContainer.UpdatePot(new List<ResponseUpdatePot.DataPot>(obj.pot));
        }
    }
    IEnumerator updatePotView(ResponseUpdatePot obj) {
        yield return new WaitForSeconds(1.0f);
        potContainer.UpdatePot(new List<ResponseUpdatePot.DataPot>(obj.pot));
    }
    void Instance_onNewRound(ResponseWaitingDealCard data)
    {
        timeStartGame = Time.realtimeSinceStartup + (data.time / 1000f);
        ResetNewRound();
    }

    void ResetNewRound()
    {
        countGenericCard = 0;
        for (int i = cardsDeal.Count - 1; i >= 0; i--)
            GameObject.Destroy(cardsDeal[i]);
        cardsDeal.Clear();

        potContainer.DestroyAllPot();
        UnMarkPot();
    }
    void DestroyCardObject(GameObject [] cards)
    {
        for (int i = cards.Length - 1; i >= 0;i-- )
        {
            GameObject card = cards[i];
            cardsDeal.Remove(card);
            GameObject.Destroy(card);
        }
    }

    private void Instance_dataTurnGame(ResponseUpdateTurnChange data)
    {
        if (data.toPlayer != null && PokerObserver.Instance.IsMainPlayer(data.toPlayer.userName))
            PuSound.Instance.Play(SoundType.YourTurn);

        if (data.dealComminityCards != null && data.dealComminityCards.Length > 0)
        {
            CreateCardDeal(data.dealComminityCards);
            ShowRank();
        }
    }

    List<GameObject> cardsDeal = new List<GameObject>();
    int countGenericCard = 0;
    void CreateCardDeal(int [] cards)
    {
        for(int i=0;i<cards.Length;i++)
        {
            if (cardsDeal.Find(o => o.GetComponent<PokerCardObject>().card.cardId == cards[i]) != null)
                continue;

            GameObject card = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CardUI"));
            card.GetComponent<PokerCardObject>().SetDataCard(new PokerCard(cards[i]));
            card.transform.parent = positionDealCards[countGenericCard++].transform;
            card.transform.localRotation = Quaternion.identity;
            card.transform.localPosition = Vector3.zero;
            card.transform.localScale = Vector3.one * 0.9f;
            cardsDeal.Add(card);
            
            PuSound.Instance.Play(SoundType.DealComminity);
        }
    }
    public List<PokerCard> pocket = new List<PokerCard>();
    void Instance_onEventUpdateHand(ResponseUpdateHand data)
    {
        ResetCountdown();
        pocket.Clear();
        for (int i = 0; i < data.hand.Length; i++)
        {
            pocket.Add(new PokerCard(data.hand[i]));
        }
        ShowRank();
        //CreateHand(data.players, data.hand);
        StartCoroutine(CreateEffectDealCard(data.players, data.hand,data.timeForAnimation));
    }

    void ShowRank() {
        if (pocket.Count == 0) return;
        
        string pocketHand = HandEvaluatorConvert.ConvertPokerCardsToString(pocket);
        string boards = HandEvaluatorConvert.ConvertPokerCardsToString(PokerObserver.Game.DealComminityCards);
        int count = 0;
        double[] player = new double[9];
        double[] opponent = new double[9];
        if (!Hand.ValidateHand(pocketHand + " " + boards))
        {
            lbMyRanking.text = "";
            return;
        }
        Hand.ParseHand(pocketHand + " " + boards, ref count);

        // Don't allow these configurations because of calculation time.
        if (count == 0 || count == 1 || count == 3 || count == 4 || count > 7)
        {
            lbMyRanking.text = "";
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
    IEnumerator CreateEffectDealCard(PokerPlayerController[] players, int[] hands,int time)
    {
        PokerPlayerController dealer =  Array.Find<PokerPlayerController>(players, p => p.userName == PokerObserver.Game.Dealer);
        int indexDealer = Array.IndexOf(players, dealer);
        List<PokerPlayerController> playerDeal = new List<PokerPlayerController>();
        for (int i = indexDealer; i < players.Length; i++)
        {
            playerDeal.Add(players[i]);    
        }
        for (int i = 0; i < indexDealer; i++ )
        {
            playerDeal.Add(players[i]);
        }
        
        float timeEffect = (time/1000) ;
        float timeMove = 1.0f;
        float timeWaitForStart = (timeEffect - timeMove) / (playerDeal.Count * 2 - 1);
        for (int i = 0; i < 2; i++)
        {
            foreach (PokerPlayerController p in playerDeal)
            {
                GameObject cardObjects = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CardUI"));
                cardObjects.transform.parent = positionEffectDealCard.transform;
                cardObjects.transform.localRotation = Quaternion.identity;
                cardObjects.transform.localPosition = Vector3.zero;
                cardObjects.transform.localScale = Vector3.one /3;
                //cardObjects.transform.parent = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().transform.parent.transform;

                Vector3 cardMoveTo = Vector3.zero;
                //cardMoveTo = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().transform.localPosition;
                if (PokerObserver.Instance.IsMainPlayer(p.userName))
                {
                    cardMoveTo = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().side.positionCardMainPlayer[i].transform.localPosition;
                    cardObjects.transform.parent = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().side.positionCardMainPlayer[i].transform.parent;
                }
                else
                {
                    cardMoveTo = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().side.positionCardFaceCards[i].transform.localPosition;
                    cardObjects.transform.parent = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().side.positionCardFaceCards[i].transform.parent;
                }
                Hashtable tweenValue = new Hashtable();
                tweenValue.Add("cardObject", cardObjects);
                tweenValue.Add("index", i);
                tweenValue.Add("cardId", hands[i]);
                tweenValue.Add("userName", p.userName);
                iTween.MoveTo(cardObjects, iTween.Hash("islocal", true, "time", timeMove, "position", cardMoveTo, "oncomplete", "onMoveCardComplete", "oncompletetarget", gameObject, "oncompleteparams", tweenValue));

                PuSound.Instance.Play(SoundType.DealCard);
                yield return new WaitForSeconds(timeWaitForStart);
              
            }
        }
    }
    void onMoveCardComplete(object vals) {
        Hashtable table = (Hashtable)vals;
        string userName = (string)table["userName"];
        GameObject cardObjects = (GameObject)table["cardObject"];
        int index = (int)table["index"];
        int cardId = (int)table["cardId"];
        if (PokerObserver.Instance.IsMainPlayer(userName))
        {
            cardObjects.GetComponent<PokerCardObject>().SetDataCard(new PokerCard(cardId), index);
        }
        else
            cardObjects.GetComponent<PokerCardObject>().SetDataCard(new PokerCard(), index);
        dictPlayerObject[userName].GetComponent<PokerPlayerUI>().UpdateSetCardObject(cardObjects, index);
        cardsDeal.Add(cardObjects);
    }

    void CreateHand(PokerPlayerController[] players, int [] hands)
    {
        foreach(PokerPlayerController p in players)
        {
            int handSize = p.handSize;
            GameObject[] cardObjects = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().cardOnHands.Length > 0 
                ? dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().cardOnHands 
                : new GameObject[handSize];
            for (int i = 0; i < handSize;i++)
                if(cardObjects[i] == null)
                    cardObjects[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CardUI"));

            if (PokerObserver.Instance.IsMainPlayer(p.userName))
            {
                if (hands.Length == handSize)
                    for (int i = 0; i < handSize; i++)
                        cardObjects[i].GetComponent<PokerCardObject>().SetDataCard(new PokerCard(hands[i]), i);
                else
                    Logger.LogError("Hand Size & Card On Hand: is not fit");
            }
            else
                for (int i = 0; i < handSize; i++)
                    cardObjects[i].GetComponent<PokerCardObject>().SetDataCard(new PokerCard(), i);

            dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().UpdateSetCardObject(cardObjects);

            cardsDeal.AddRange(cardObjects);
        }
    }

    void Instance_onFinishGame(ResponseFinishGame responseData)
    {
        StartCoroutine(_onFinishGame(responseData));
    }
    IEnumerator _onFinishGame(ResponseFinishGame responseData)
    {
        UnMarkPot();
        PokerObserver.Game.StartFinishGame();
        PokerObserver.Instance.isWaitingFinishGame = true;

        float totalTimeFinishGame = responseData.time / 1000f;
        float waitTimeViewResultCard = totalTimeFinishGame > 2f ? 1f : 0f;
        float timeEffectPot = totalTimeFinishGame - waitTimeViewResultCard;
        if (responseData.pots.Length > 0)
            timeEffectPot /= responseData.pots.Length;

        bool isFaceUp = PokerObserver.Instance.GetTotalPlayerNotFold() > 1;
        if (isFaceUp)
        {
            if (responseData.dealComminityCards.Length == 5)
            {
                int[] threeCard = new int[3];
                Array.Copy(responseData.dealComminityCards, 0, threeCard, 0, 3);
                CreateCardDeal(threeCard);
                yield return new WaitForSeconds(1.0f);
                int[] cardFouth = new int[1];
                Array.Copy(responseData.dealComminityCards, 3, cardFouth, 0, 1);
                CreateCardDeal(cardFouth);
                yield return new WaitForSeconds(1.0f);
                int[] cardFive = new int[1];
                Array.Copy(responseData.dealComminityCards, 4, cardFive, 0, 1);
                CreateCardDeal(cardFive);
            }
            else
            {
                CreateCardDeal(responseData.dealComminityCards);
            }
        }

        #region SET RESULT TITLE
        PokerPlayerUI[] playerUI = GameObject.FindObjectsOfType<PokerPlayerUI>();
        for (int i = 0; i < playerUI.Length; i++)
        {
            for (int j = 0; j < responseData.players.Length; j++)
            {
                if (playerUI[i].UserName == responseData.players[j].userName)
                    playerUI[i].SetTitle(isFaceUp ? UTF8Encoder.DecodeEncodedNonAsciiCharacters(responseData.players[j].ranking) : null);
            }
        }
        
        #endregion

        #region UPDATE POTS WHEN FINISH GAME
        List<ResponseUpdatePot.DataPot> potFinishGame = new List<ResponseUpdatePot.DataPot>();
        List<ResponseResultSummary> potSummaries = responseData.pots.ToList().OrderByDescending(p => p.players.Length).ToList();

        foreach (ResponseResultSummary summary in potSummaries)
        {
            ResponseUpdatePot.DataPot pot = new ResponseUpdatePot.DataPot();
            pot.id = summary.potId;
            ResponseMoneyExchange playerWin = Array.Find<ResponseMoneyExchange>(summary.players, p => p.winner);
            if (playerWin != null)
            {
                if (PokerObserver.Instance.IsMainPlayer(playerWin.userName))
                    PuSound.Instance.Play(SoundType.PlayerWin);

                pot.value = playerWin.moneyExchange;
                potFinishGame.Add(pot);
            }
        }
        potContainer.DestroyAllPot();
        yield return new WaitForEndOfFrame();
        potContainer.UpdatePot(potFinishGame);
        #endregion

        yield return new WaitForSeconds(waitTimeViewResultCard * 3 / 4f);
        #region UPDATE CARD
        foreach (ResponseResultSummary summary in responseData.pots)
        {
            ResponseMoneyExchange playerWin = Array.Find<ResponseMoneyExchange>(summary.players, p => p.winner);

            List<string> lstWinner = new List<string>();
            foreach (ResponseMoneyExchange item in summary.players)
                if (item.winner)
                    lstWinner.Add(item.userName);

            if (potContainer != null && playerWin != null)
            {
                string rankWin = Array.Find<ResponseFinishCardPlayer>(responseData.players, rdp => rdp.userName == playerWin.userName).ranking;
                RankEndGameModel playerWinRank = new RankEndGameModel(UTF8Encoder.DecodeEncodedNonAsciiCharacters(rankWin));

                if (lstWinner.Count > 0)
                {
                    for (int i = 0; i < lstWinner.Count(); i++)
                        if (dictPlayerObject.ContainsKey(lstWinner[i]))
                            dictPlayerObject[lstWinner[i]].GetComponent<PokerPlayerUI>().SetResult(true);
                }

                if (isFaceUp)
                {
                    List<GameObject> listCardObject = new List<GameObject>();
                    List<int> list = new List<int>();

                    if (playerWinRank != null)
                        DialogService.Instance.ShowDialog(playerWinRank);
                    else
                        Logger.LogError("Can't found player Win");

                    foreach (ResponseMoneyExchange item in summary.players)
                    {
                        if (item.winner)
                        {
                            list.AddRange(item.cards);
                            listCardObject.AddRange(cardsDeal.FindAll(o => list.Contains(o.GetComponent<PokerCardObject>().card.cardId)));
                        }
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        listCardObject.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(i % 2 == 0));
                        yield return new WaitForSeconds(timeEffectPot / 20f);
                    }
                    listCardObject.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(false));
                    playerWinRank.DestroyUI();
                    yield return new WaitForEndOfFrame();
                }
                else
                    yield return new WaitForSeconds(timeEffectPot);

                if (lstWinner.Count > 0)
                {
                    for (int i = 0; i < lstWinner.Count(); i++)
                        if (dictPlayerObject.ContainsKey(lstWinner[i]))
                            dictPlayerObject[lstWinner[i]].GetComponent<PokerPlayerUI>().SetResult(false);
                }
            }
        }
        #endregion
        yield return new WaitForSeconds(waitTimeViewResultCard / 4f);

        // Reset Result title
        if (isFaceUp)
            Array.ForEach<PokerPlayerUI>(playerUI, p => { if (p != null) p.SetTitle(null); });

        ResetNewRound();
        PokerObserver.Instance.isWaitingFinishGame = false;
        PokerObserver.Game.EndFinishGame();
    }

    void Instance_dataUpdateGame(ResponseUpdateGame data)
    {
        ResetNewRound();
    }

    void Game_onFirstTimeJoinGame(ResponseUpdateGame data)
    {
        if (data.players != null && data.players.Length > 0 && Array.FindAll<PokerPlayerController>(data.players, p => p.GetPlayerState() != PokerPlayerState.none).Length > 0)
        {
            int[] hands = null;
            foreach (PokerPlayerController player in data.players)
            {
                if (PokerObserver.Instance.IsMainPlayer(player.userName))
                    hands = player.hand;

                SetPositionAvatarPlayer(player.userName);
                if(player.inTurn)
                    dictPlayerObject[player.userName].GetComponent<PokerPlayerUI>().StartTimer(data.totalTime / 1000f, data.remainingTime / 1000f);
            }
            CreateHand(data.players, hands);
            CreateCardDeal(data.dealComminityCards);

            if(data.pot != null && data.pot.Length > 0)
                potContainer.UpdatePot(new List<ResponseUpdatePot.DataPot>(data.pot));
        }
    }

    void Instance_onUpdateRoomMaster(ResponseUpdateRoomMaster data)
    {
    }

    void Instance_onPlayerListChanged(ResponsePlayerListChanged dataPlayer)
    {
        PokerPlayerChangeAction state = dataPlayer.GetActionState();
        if(state == PokerPlayerChangeAction.playerAdded)
        {
            if (PokerObserver.Instance.IsMainPlayer(dataPlayer.player.userName))
                PuSound.Instance.Play(SoundType.Sit_Down);

            SetPositionAvatarPlayer(dataPlayer.player.userName);
        }
        else if ((state == PokerPlayerChangeAction.playerRemoved || state == PokerPlayerChangeAction.playerQuitGame)
            && dictPlayerObject.ContainsKey(dataPlayer.player.userName))
        {
            if (PokerObserver.Instance.IsMainPlayer(dataPlayer.player.userName))
                PuSound.Instance.Play(SoundType.StandUp);

            if(PokerObserver.Game.Dealer == dataPlayer.player.userName)
                objectDealer.SetActive(false);

            DestroyCardObject(dictPlayerObject[dataPlayer.player.userName].GetComponent<PokerPlayerUI>().cardOnHands);
            GameObject.Destroy(dictPlayerObject[dataPlayer.player.userName]);
            dictPlayerObject.Remove(dataPlayer.player.userName);
        }

        UpdatePositionPlayers(dataPlayer.player.userName);

        if (PokerObserver.Game.ListPlayer.Count <= 1)
            ResetCountdown();
    }

    void UpdatePositionPlayers(string ignorePlayer)
    {
        PokerObserver.Game.ListPlayer.ForEach(p =>
        {
            if (!string.IsNullOrEmpty(ignorePlayer) && p.userName != ignorePlayer)
                SetPositionAvatarPlayer(p.userName);
        });
    }

    void SetPositionAvatarPlayer(string userName)
    {
        PokerPlayerController player = PokerObserver.Game.GetPlayer(userName);

        GameObject obj;
        if (dictPlayerObject.ContainsKey(userName))
            obj = dictPlayerObject[userName];
        else
        {
            obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerUI"));
            dictPlayerObject.Add(player.userName, obj);
        }

        PokerGPSide playerSide = Array.Find<PokerGPSide>(arrayPokerSide, s => s.CurrentSide == player.GetSide());
        obj.GetComponent<PokerPlayerUI>().side = playerSide;
        obj.GetComponent<PokerPlayerUI>().SetData(player);
        obj.transform.parent = playerSide.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }

    public void SetDealerObjectToPlayer(PokerPlayerController player)
    {
        objectDealer.SetActive(true);
        PokerGPSide playerSide = Array.Find<PokerGPSide>(arrayPokerSide, s => s.CurrentSide == player.GetSide());
        objectDealer.transform.parent = playerSide.positionDealer.transform;
        objectDealer.transform.localPosition = Vector3.zero;
        objectDealer.transform.localScale = Vector3.one;
    }

    public PokerGPSide GetPokerSide(PokerSide side)
    {
        return Array.Find<PokerGPSide>(arrayPokerSide, s => s.CurrentSide == side);
    }

    #region NHỮNG NGƯỜI CHƠI ĐÃ THOÁT SẼ LƯU LẠI CHỜ ĐẾN KHI UPDATEPOT HOẶC FINISGGAME MỚI XÓA POT
    public void MarkerPot(PokerPotItem pot)
    {
        if (_listPotMarkers.Contains(pot))
            _listPotMarkers.Add(pot);
    }

    void UnMarkPot()
    {
        for (int i = _listPotMarkers.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(_listPotMarkers[i].gameObject);
        }
        _listPotMarkers.Clear();
    }
    #endregion

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
            else if(second < 0)
            {
                ResetCountdown();
            }
        }
    }

    void ResetCountdown()
    {
        timeStartGame = -1;
        lbCountdown.fontSize = 100;
        lbCountdown.text = "";
    }

    public object spriteArray { get; set; }
}
