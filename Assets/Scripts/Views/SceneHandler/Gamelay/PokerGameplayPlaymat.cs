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

public class PokerGameplayPlaymat : MonoBehaviour
{
    #region UNITY EDITOR
    public GameObject prefabBetObject;
    public Transform []positionDealCards;
    public PokerPotManager potContainer;
    public GameObject objectDealer;
    public UILabel lbMyRanking;
    #endregion
    PokerGPSide[] arrayPokerSide;
    Dictionary<string, GameObject> dictPlayerObject = new Dictionary<string, GameObject>();
    bool isWaitingFinishGame = false;

    void Awake()
    {
        objectDealer.SetActive(false);

        arrayPokerSide = GameObject.FindObjectsOfType<PokerGPSide>();

        PokerObserver.Instance.onFirstJoinGame += Instance_onFirstJoinGame;
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
        PokerObserver.Instance.onFirstJoinGame -= Instance_onFirstJoinGame;
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

        if (!PokerObserver.Instance.isWaitingFinishGame && obj.pot != null && obj.pot.Length > 0)
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
        if(data.dealComminityCards != null && data.dealComminityCards.Length > 0)
            CreateCardDeal(data.dealComminityCards);
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
        }
    }

    void Instance_onEventUpdateHand(ResponseUpdateHand data)
    {
        CreateHand(data.players, data.hand);
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
        }
    }

    void Instance_onFinishGame(ResponseFinishGame responseData)
    {
        StartCoroutine(_onFinishGame(responseData));
    }

    IEnumerator _onFinishGame(ResponseFinishGame responseData)
    {
        PokerObserver.Instance.isWaitingFinishGame = true;
     

        float time = responseData.time/1000f;
        float waitTimeViewCard = time > 1 ? 1f : 0f;
        float timeEffectPot = responseData.pots.Length > 0 ? time - (waitTimeViewCard / responseData.pots.Length) : time - waitTimeViewCard;

        #region SET RESULT TITLE
        PokerPlayerUI[] playerUI =  GameObject.FindObjectsOfType<PokerPlayerUI> ();
		for (int i = 0; i < playerUI.Length ;i++) {
			for(int j= 0 ;j<responseData.players.Length;j++){
				if(playerUI[i].UserName == responseData.players[j].userName){
                    playerUI[i].SetTitle(UTF8Encoder.DecodeEncodedNonAsciiCharacters(responseData.players[j].ranking));
				}
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
            pot.value = playerWin.moneyExchange;
        }
        potContainer.UpdatePot(potFinishGame);
        #endregion

        yield return new WaitForSeconds(waitTimeViewCard /2f);

        #region UPDATE CARD 
        bool isFoldAll = PokerObserver.Game.ListPlayer.FindAll(p => p.GetPlayerState() == PokerPlayerState.fold).Count == 0;
        if (isFoldAll || PokerObserver.Game.ListPlayer.FindAll(p => p.userName != PokerObserver.Game.MainPlayer.userName).Count == 0)
        {

        }
        else
        {
            CreateCardDeal(responseData.dealComminityCards);
            foreach (ResponseResultSummary summary in responseData.pots)
            {
                ResponseMoneyExchange playerWin = Array.Find<ResponseMoneyExchange>(summary.players, p => p.winner);

                if (potContainer != null && playerWin != null)
                {
                   
                        string rankWin = Array.Find<ResponseFinishCardPlayer>(responseData.players, rdp => rdp.userName == playerWin.userName).ranking;
                        RankEndGameModel playerWinRank = new RankEndGameModel(UTF8Encoder.DecodeEncodedNonAsciiCharacters(rankWin));
                        DialogService.Instance.ShowDialog(playerWinRank);
                        dictPlayerObject[playerWin.userName].GetComponent<PokerPlayerUI>().SetResult(true);

                        List<int> list = new List<int>(playerWin.cards);
                        List<GameObject> listCardObject = cardsDeal.FindAll(o => list.Contains(o.GetComponent<PokerCardObject>().card.cardId));
                        for (int i = 0; i < 20; i++)
                        {
                            listCardObject.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(i % 2 == 0));
                            yield return new WaitForSeconds(timeEffectPot / 20f);
                        }
                        listCardObject.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(false));
                        dictPlayerObject[playerWin.userName].GetComponent<PokerPlayerUI>().SetResult(false);

                        playerWinRank.DestroyUI();
                    
                }
            }
        }
            yield return new WaitForSeconds(waitTimeViewCard / 2);
        
        #endregion

        // Reset Result title
        Array.ForEach<PokerPlayerUI>(playerUI, p => { if (p != null) p.SetTitle(null); });

        ResetNewRound();
        PokerObserver.Instance.isWaitingFinishGame = false;
    }

    void Instance_onFirstJoinGame(ResponseUpdateGame data)
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
            CreateHand(data.players, hands);
            CreateCardDeal(data.dealComminityCards);
        }
    }

    void Instance_dataUpdateGame(ResponseUpdateGame data)
    {
        ResetNewRound();
        //Instance_onFirstJoinGame(data);
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
