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

public class PokerGameplayPlaymat : MonoBehaviour
{
    #region UNITY EDITOR
    public GameObject prefabBetObject;
    public Transform []positionDealCards;
    public PokerPotManager potContainer;
    public GameObject objectDealer;
    public UILabel lbMyRanking;
    public Transform Dealer;
    #endregion
    PokerGPSide[] arrayPokerSide;
    Dictionary<string, GameObject> dictPlayerObject = new Dictionary<string, GameObject>();
    bool isWaitingFinishGame = false;

    #region Effect deal cards to hand and table
    //hand
    int indexDealer = -1;
    List<GameObject> ListCardsDeal = new List<GameObject>();//List chứa cards được chia theo đúng thứ tự
    Dictionary<int, GameObject[]> dCardsDeal = new Dictionary<int, GameObject[]>();

    //table
    List<GameObject> cardsDealToTable = new List<GameObject>();
    int NumberCardsInTable = 0;
    #endregion
    void Awake()
    {
        objectDealer.SetActive(false);

        arrayPokerSide = GameObject.FindObjectsOfType<PokerGPSide>();

        PokerObserver.Instance.onPlayerListChanged += Instance_onPlayerListChanged;
        PokerObserver.Instance.dataUpdateGameChange += Instance_dataUpdateGame;
        PokerObserver.Instance.onEventUpdateHand += Instance_onEventUpdateHand;
        PokerObserver.Instance.onTurnChange += Instance_dataTurnGame;
        PokerObserver.Instance.onNewRound += Instance_onNewRound;
        PokerObserver.Instance.onUpdatePot += Instance_onUpdatePot;
        PokerObserver.Instance.onFinishGame += Instance_onFinishGame;
        PokerObserver.Instance.onUpdateRoomMaster += Instance_onUpdateRoomMaster;
    }

    void OnDestroy()
    {
        PokerObserver.Instance.onPlayerListChanged -= Instance_onPlayerListChanged;
        PokerObserver.Instance.dataUpdateGameChange -= Instance_dataUpdateGame;
        PokerObserver.Instance.onEventUpdateHand -= Instance_onEventUpdateHand;
        PokerObserver.Instance.onTurnChange -= Instance_dataTurnGame;
        PokerObserver.Instance.onNewRound -= Instance_onNewRound;
        PokerObserver.Instance.onUpdatePot -= Instance_onUpdatePot;
        PokerObserver.Instance.onFinishGame -= Instance_onFinishGame;
        PokerObserver.Instance.onUpdateRoomMaster -= Instance_onUpdateRoomMaster;
    }
    ResponseUpdatePot currentUpdatePot = null;
    void Instance_onUpdatePot(ResponseUpdatePot obj)
    {
        if (!PokerObserver.Instance.isWaitingFinishGame && obj.pot != null && obj.pot.Length > 0 && obj.pot[0].value > 0)
        {
            currentUpdatePot = obj;
            //PokerPlayerUI[] players = GameObject.FindObjectsOfType<PokerPlayerUI>();
            //foreach (PokerPlayerUI item in players)
            //{
            //    if (item != null && item.gameObject != null && item.currentBet.CurrentBet !=0) {
            //        item.addMoneyToMainPot();
            //    }   
            //}
            potContainer.UpdatePot(new List<ResponseUpdatePot.DataPot>(obj.pot));
        }
    }
    bool PotIsUpdate(ResponseUpdatePot obj) {
        if (currentUpdatePot == null)
        {
            return true;
        }
        else {
            var same = obj.pot.Except(currentUpdatePot.pot).Count() == 0 && currentUpdatePot.pot.Except(obj.pot).Count() == 0;
            return same;
        }
    }
    void Instance_onNewRound(ResponseWaitingDealCard data)
    {
        ResetNewRound();
    }

    void ResetNewRound()
    {
        countGenericCard = 0;
        for (int i = cardsDeal.Count - 1; i >= 0; i--)
            GameObject.Destroy(cardsDeal[i]);
        cardsDeal.Clear();

        potContainer.DestroyAllPot();
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
        if (data.dealComminityCards != null && data.dealComminityCards.Length > 0)
        {
            CreateCardDeal(data.dealComminityCards,false);
            ShowRank();
        }
    }

    List<GameObject> cardsDeal = new List<GameObject>();
    int countGenericCard = 0;

    void CreateCardDeal(int[] cards, bool isAllIn)
    {
        if (!isAllIn)
        {
            for (int i = 0; i < cards.Length; i++)
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
                cardsDealToTable.Add(card);
                NumberCardsInTable++;
            }
            StartCoroutine(OnDealCardToTable());
        }
        else
        {
            StartCoroutine(OnDealCardToTable_AllIn(cards));
        }
    }
    public List<PokerCard> pocket;
    void Instance_onEventUpdateHand(ResponseUpdateHand data)
    {
        if (pocket == null)
            pocket = new List<PokerCard>();
        pocket.Clear();
        for (int i = 0; i < data.hand.Length; i++)
        {
            pocket.Add(new PokerCard(data.hand[i]));
        }
        ShowRank();
        //StartCoroutine(_Instance_onEventUpdateHand(data));
        CreateHand(data.players, data.hand, data.timeForAnimation);
    }

    void ShowRank() {
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
    void CreateHand(PokerPlayerController[] players, int[] hands, float timeForAnimation)
    {
        //for deal carl to hand
        List<int> ListSlotIndex = new List<int>();
        indexDealer = -1;
        ListCardsDeal.Clear();
        dCardsDeal.Clear();//

        foreach(PokerPlayerController p in players)
        {
            int handSize = p.handSize;
            GameObject[] cardObjects = dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().cardOnHands.Length > 0 
                ? dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().cardOnHands 
                : new GameObject[handSize];
            for (int i = 0; i < handSize;i++)
                if(cardObjects[i] == null)
                    cardObjects[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CardUI"));

            if (PokerObserver.Instance.mUserInfo.info.userName == p.userName)
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

            //for deal card to hand
            if (PokerObserver.Game.Dealer == p.userName)
            {
                indexDealer = p.slotIndex;
                ListCardsDeal.AddRange(cardObjects);
            }
            else
                dCardsDeal.Add(p.slotIndex, cardObjects);
            ListSlotIndex.Add(p.slotIndex);
        }
        if (timeForAnimation == -1) timeForAnimation = 2000;
        float time = (timeForAnimation / 1000) / 2;
        time = time / players.Count();
        DealCardToHand(ListSlotIndex, time);/////////
    }
    #region EFFECT DEAL CARD TO HAND
    void SortSlotIndex(List<int> ListSlotIndex)
    {
        List<int> largerNumber = new List<int>();
        List<int> smallerNumber = new List<int>();
        for (int i = 0; i < ListSlotIndex.Count; i++)
        {
            if (ListSlotIndex[i] > indexDealer)
                largerNumber.Add(ListSlotIndex[i]);
            else if (ListSlotIndex[i] < indexDealer)
                smallerNumber.Add(ListSlotIndex[i]);
        }
        if (largerNumber.Count > 0)
        {
            largerNumber.Sort();
            for (int i = 0; i < largerNumber.Count; i++)
                ListCardsDeal.AddRange(dCardsDeal[largerNumber[i]]);
        }

        if (smallerNumber.Count > 0)
        {
            smallerNumber.Sort();
            for (int i = 0; i < smallerNumber.Count; i++)
                ListCardsDeal.AddRange(dCardsDeal[smallerNumber[i]]);
        }

    }
    IEnumerator OnDealCardToHand(float timeForAnimation)
    {
        for (int i = 0; i <= 1; i++)
        {
            for (int j = i; j < ListCardsDeal.Count(); j += 2)
            {
                yield return new WaitForSeconds(timeForAnimation);
                if (ListCardsDeal[j].GetComponent<PokerCardObject>().gameObject != null)
                {
                    iTween.MoveFrom(ListCardsDeal[j].GetComponent<PokerCardObject>().gameObject, Dealer.position, timeForAnimation);
                    ListCardsDeal[j].GetComponent<PokerCardObject>().gameObject.SetActive(true);
                }
            }
        }
    }
    void DealCardToHand(List<int> ListSlotIndex, float timeForAnimation)
    {
        foreach (GameObject item in cardsDeal)
            item.GetComponent<PokerCardObject>().gameObject.SetActive(false);
        SortSlotIndex(ListSlotIndex);
        StartCoroutine(OnDealCardToHand(timeForAnimation));
    }
    #endregion

    #region EFFECT DEAL CARD TO Table
    float timeRotate = 0.5f;
    float timeShowFace = 0.1f;
    float timeDealCard = 0.3f;
    IEnumerator OnDealCardToTable()
    {
        foreach (GameObject item in cardsDealToTable)
            item.GetComponent<PokerCardObject>().gameObject.SetActive(false);

        //Chia bài
        for (int i = 0; i < cardsDealToTable.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            if (cardsDealToTable[i].GetComponent<PokerCardObject>().gameObject != null)
            {
                iTween.MoveFrom(cardsDealToTable[i].GetComponent<PokerCardObject>().gameObject, Dealer.position, timeDealCard);
                cardsDealToTable[i].GetComponent<PokerCardObject>().gameObject.SetActive(true);
                cardsDealToTable[i].GetComponent<PokerCardObject>().spriteBackground.depth = 14;
                cardsDealToTable[i].GetComponent<PokerCardObject>().spriteBackground.transform.localRotation = new Quaternion(0, -180, 0, 0);
                cardsDealToTable[i].GetComponent<PokerCardObject>().spriteBackground.spriteName = "card_up";
            }
        }
        yield return new WaitForSeconds(0.5f);
        //Lật bài
        for (int i = 0; i < cardsDealToTable.Count; i++)
        {
            if (cardsDealToTable[i].GetComponent<PokerCardObject>().gameObject != null)
            {
                iTween.RotateTo(cardsDealToTable[i].GetComponent<PokerCardObject>().spriteBackground.gameObject, new Vector3(0, 0, 0), timeRotate);//quay object
                yield return new WaitForSeconds(timeShowFace); //hiện mặt card_empty 
                cardsDealToTable[i].GetComponent<PokerCardObject>().spriteBackground.spriteName = "card_empty";
                cardsDealToTable[i].GetComponent<PokerCardObject>().spriteBackground.depth = 10;
            }
        }

        cardsDealToTable.Clear();
    }

    IEnumerator OnDealCardToTable_AllIn(int[] cards)
    {
        if (NumberCardsInTable == 0 || NumberCardsInTable > 3)
        {
            for (int i = NumberCardsInTable; i < cards.Count(); i++)
            {
                if (i == 3 || i == 4) yield return new WaitForSeconds(0.6f);
                if (cardsDeal.Find(o => o.GetComponent<PokerCardObject>().card.cardId == cards[i]) != null)
                    continue;

                GameObject card = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CardUI"));
                card.GetComponent<PokerCardObject>().SetDataCard(new PokerCard(cards[i]));
                card.transform.parent = positionDealCards[countGenericCard++].transform;
                card.transform.localRotation = Quaternion.identity;
                card.transform.localPosition = Vector3.zero;
                card.transform.localScale = Vector3.one * 0.9f;
                cardsDeal.Add(card);
                cardsDealToTable.Add(card);

                if (i > 1)
                {
                    foreach (GameObject item in cardsDealToTable)
                        item.GetComponent<PokerCardObject>().gameObject.SetActive(false);

                    //Chia bài
                    for (int j = 0; j < cardsDealToTable.Count; j++)
                    {

                        yield return new WaitForSeconds(0.3f);
                        if (cardsDealToTable[j].GetComponent<PokerCardObject>().gameObject != null)
                        {
                            iTween.MoveFrom(cardsDealToTable[j].GetComponent<PokerCardObject>().gameObject, Dealer.position, timeDealCard);
                            cardsDealToTable[j].GetComponent<PokerCardObject>().gameObject.SetActive(true);
                            cardsDealToTable[j].GetComponent<PokerCardObject>().spriteBackground.depth = 14;
                            cardsDealToTable[j].GetComponent<PokerCardObject>().spriteBackground.transform.localRotation = new Quaternion(0, -180, 0, 0);
                            cardsDealToTable[j].GetComponent<PokerCardObject>().spriteBackground.spriteName = "card_up";
                        }
                    }
                    yield return new WaitForSeconds(0.5f);
                    //Lật bài
                    for (int j = 0; j < cardsDealToTable.Count; j++)
                    {
                        if (cardsDealToTable[j].GetComponent<PokerCardObject>().gameObject != null)
                        {
                            iTween.RotateTo(cardsDealToTable[j].GetComponent<PokerCardObject>().spriteBackground.gameObject, new Vector3(0, 0, 0), timeRotate);//quay object
                            yield return new WaitForSeconds(timeShowFace); //hiện mặt card_empty 
                            cardsDealToTable[j].GetComponent<PokerCardObject>().spriteBackground.spriteName = "card_empty";
                            cardsDealToTable[j].GetComponent<PokerCardObject>().spriteBackground.depth = 10;
                        }
                    }

                    cardsDealToTable.Clear();
                }
            }
        }


    }
    #endregion 

    void Instance_onFinishGame(ResponseFinishGame responseData)
    {
        StartCoroutine(_onFinishGame(responseData));
    }

    IEnumerator _onFinishGame(ResponseFinishGame responseData)
    {
        PokerObserver.Game.StartFinishGame();
        PokerObserver.Instance.isWaitingFinishGame = true;

        float time = responseData.time/1000f;
        float waitTimeViewCard = time > 1 ? 1f : 0f;
        float timeEffectPot = responseData.pots.Length > 0 ? time - (waitTimeViewCard / responseData.pots.Length) : time - waitTimeViewCard;

        int numberPlayerNotFold = PokerObserver.Game.ListPlayer.FindAll(p => p.GetPlayerState() != PokerPlayerState.fold && p.GetPlayerState() != PokerPlayerState.none).Count;
        bool isFaceUp = numberPlayerNotFold > 1 && PokerObserver.Game.IsMainPlayerInGame && PokerObserver.Game.MainPlayer.GetPlayerState() != PokerPlayerState.fold;

        if (isFaceUp)
        {
            CreateCardDeal(responseData.dealComminityCards, isFaceUp);
            float waitingTimeSetResult = 6f;
            if (NumberCardsInTable == 3) waitingTimeSetResult = waitingTimeSetResult / 2;
            if (NumberCardsInTable == 4) waitingTimeSetResult = waitingTimeSetResult / 3;
            
            yield return new WaitForSeconds(waitingTimeSetResult);
        }
        NumberCardsInTable = 0;
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
        foreach(ResponseResultSummary summary in responseData.pots)
        {
            ResponseUpdatePot.DataPot pot = new ResponseUpdatePot.DataPot();
            pot.id = summary.potId;
            ResponseMoneyExchange playerWin = Array.Find<ResponseMoneyExchange>(summary.players, p => p.winner);
            if(playerWin != null)
                pot.value = playerWin.moneyExchange;
        }
        potContainer.UpdatePot(potFinishGame);
        #endregion

        yield return new WaitForSeconds(waitTimeViewCard /2f);

        #region UPDATE CARD 
        foreach (ResponseResultSummary summary in responseData.pots)
        {
            ResponseMoneyExchange playerWin = Array.Find<ResponseMoneyExchange>(summary.players, p => p.winner);

            if (potContainer != null && playerWin != null)
            {
                string rankWin = Array.Find<ResponseFinishCardPlayer>(responseData.players, rdp => rdp.userName == playerWin.userName).ranking;

                RankEndGameModel playerWinRank = new RankEndGameModel(UTF8Encoder.DecodeEncodedNonAsciiCharacters(rankWin));

                if(dictPlayerObject.ContainsKey(playerWin.userName))
                    dictPlayerObject[playerWin.userName].GetComponent<PokerPlayerUI>().SetResult(true);

                if (isFaceUp)
                {
                    DialogService.Instance.ShowDialog(playerWinRank);
                    List<int> list = new List<int>(playerWin.cards);
                    List<GameObject> listCardObject = cardsDeal.FindAll(o => list.Contains(o.GetComponent<PokerCardObject>().card.cardId));
                    for (int i = 0; i < 20; i++)
                    {
                        listCardObject.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(i % 2 == 0));
                        yield return new WaitForSeconds(timeEffectPot / 20f);
                    }
                    listCardObject.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(false));
                    playerWinRank.DestroyUI();
                }
                else
                    yield return new WaitForSeconds(timeEffectPot);

                if (dictPlayerObject.ContainsKey(playerWin.userName))
                    dictPlayerObject[playerWin.userName].GetComponent<PokerPlayerUI>().SetResult(false);
            }
        }
        yield return new WaitForSeconds(waitTimeViewCard / 2);
        
        #endregion

        // Reset Result title
        if (isFaceUp)
            Array.ForEach<PokerPlayerUI>(playerUI, p => { if (p != null) p.SetTitle(null); });

        ResetNewRound();
        PokerObserver.Instance.isWaitingFinishGame = false;
        PokerObserver.Game.EndFinishGame();
    }

    void Instance_onJoinGamePlaying(ResponseUpdateGame data)
    {
        if (data.players != null && data.players.Length > 0 && Array.FindAll<PokerPlayerController>(data.players, p => p.GetPlayerState() != PokerPlayerState.none).Length > 0)
        {
            int[] hands = null;
            foreach (PokerPlayerController player in data.players)
            {
                if (PokerObserver.Instance.IsMainPlayer(player.userName))
                    hands = player.hand;

                SetPositionAvatarPlayer(player.userName);
            }
            CreateHand(data.players, hands,-1);
            CreateCardDeal(data.dealComminityCards,false);
        }
    }

    void Instance_dataUpdateGame(ResponseUpdateGame data)
    {
        ResetNewRound();
    }

    void Instance_onUpdateRoomMaster(ResponseUpdateRoomMaster data)
    {
    }

    void Instance_onPlayerListChanged(ResponsePlayerListChanged dataPlayer)
    {
        PokerPlayerChangeAction state = dataPlayer.GetActionState();
        if(state == PokerPlayerChangeAction.playerAdded)
        {
            SetPositionAvatarPlayer(dataPlayer.player.userName);
        }
        else if ((state == PokerPlayerChangeAction.playerRemoved || state == PokerPlayerChangeAction.playerQuitGame)
            && dictPlayerObject.ContainsKey(dataPlayer.player.userName))
        {
            if(PokerObserver.Game.Dealer == dataPlayer.player.userName)
                objectDealer.SetActive(false);

            DestroyCardObject(dictPlayerObject[dataPlayer.player.userName].GetComponent<PokerPlayerUI>().cardOnHands);
            GameObject.Destroy(dictPlayerObject[dataPlayer.player.userName]);
            dictPlayerObject.Remove(dataPlayer.player.userName);
        }

        UpdatePositionPlayers(dataPlayer.player.userName);
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

}
