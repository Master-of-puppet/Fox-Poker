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
using Puppet.Core.Model;

public class PokerGameplayPlaymat : MonoBehaviour
{
    #region UNITY EDITOR
    public PokerGameplayView gpView;
    public GameObject prefabBetObject, prefabCard, prefabPlayer;
    public GameObject positionEffectDealCard;
    public Transform []positionDealCards;
    public PokerPotManager potContainer;
    public GameObject objectDealer;
    #endregion
    PokerGPSide[] arrayPokerSide;
    Dictionary<string, GameObject> dictPlayerObject = new Dictionary<string, GameObject>();
    List<PokerPotItem> _listPotMarkers = new List<PokerPotItem>();
    List<GameObject> cardsDeal = new List<GameObject>();
    int countGenericCard = 0;

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
    }
    
    void Instance_onUpdatePot(ResponseUpdatePot obj)
    {
        UnMarkPot();
        if (!PokerObserver.Instance.isWaitingFinishGame && obj.pot != null && obj.pot.Length > 0 && obj.pot[0].value > 0)
            potContainer.UpdatePot(new List<ResponseUpdatePot.DataPot>(obj.pot));
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
        UnMarkPot();
    }
    
    private void Instance_dataTurnGame(ResponseUpdateTurnChange data)
    {
        if (data.toPlayer != null && PokerObserver.Game.IsMainPlayer(data.toPlayer.userName))
            PuSound.Instance.Play(SoundType.YourTurn);

        if (data.dealComminityCards != null && data.dealComminityCards.Length > 0)
        {
            CreateCardDeal(data.dealComminityCards);
            gpView.ShowRank();
        }
    }

    void CreateCardDeal(int [] cards)
    {
        for(int i=0;i<cards.Length;i++)
        {
            if (cardsDeal.Find(o => o.GetComponent<PokerCardObject>().card.cardId == cards[i]) != null)
                continue;

            GameObject card = (GameObject)GameObject.Instantiate(prefabCard);
            card.GetComponent<PokerCardObject>().SetDataCard(new PokerCard(cards[i]));
            card.transform.parent = positionDealCards[countGenericCard++].transform;
            card.transform.localRotation = Quaternion.identity;
            card.transform.localPosition = Vector3.zero;
            card.transform.localScale = Vector3.one * 0.9f;
            cardsDeal.Add(card);
            
            PuSound.Instance.Play(SoundType.DealComminity);
        }
    }

    void Instance_onEventUpdateHand(ResponseUpdateHand data)
    {
        StartCoroutine(CreateEffectDealCard(data.players, data.hand,data.timeForAnimation));
    }

    IEnumerator CreateEffectDealCard(PokerPlayerController[] players, int[] hands,int time)
    {
        PokerObserver.Game.IsClientListening = false;
        PokerPlayerController dealer =  Array.Find<PokerPlayerController>(players, p => p.userName == PokerObserver.Game.Dealer);
        int indexDealer = Array.IndexOf(players, dealer);
        List<PokerPlayerController> playerDeal = new List<PokerPlayerController>();
        for (int i = indexDealer; i < players.Length; i++)
            playerDeal.Add(players[i]);    
        for (int i = 0; i < indexDealer; i++ )
            playerDeal.Add(players[i]);
        
        float timeEffect = (time/1000) ;
        float timeMove = 1.0f;
        float timeWaitForStart = (timeEffect - timeMove) / (playerDeal.Count * 2 - 1);
        for (int i = 0; i < 2; i++)
        {
            foreach (PokerPlayerController p in playerDeal)
            {
                GameObject cardObjects = (GameObject)GameObject.Instantiate(prefabCard);
                cardObjects.transform.parent = positionEffectDealCard.transform;
                cardObjects.transform.localRotation = Quaternion.identity;
                cardObjects.transform.localPosition = Vector3.zero;
                cardObjects.transform.localScale = Vector3.one /3;

                Vector3 cardMoveTo = Vector3.zero;
                if (PokerObserver.Game.IsMainPlayer(p.userName))
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
        PokerObserver.Game.IsClientListening = true;
    }
    void onMoveCardComplete(object vals) {
        Hashtable table = (Hashtable)vals;
        string userName = (string)table["userName"];
        GameObject cardObjects = (GameObject)table["cardObject"];
        int index = (int)table["index"];
        int cardId = (int)table["cardId"];
        cardObjects.GetComponent<PokerCardObject>().SetDataCard( PokerObserver.Game.IsMainPlayer(userName) ? new PokerCard(cardId) : new PokerCard(), index);
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

            for (int i = 0; i < handSize; i++)
                if (cardObjects[i] == null)
                    cardObjects[i] = (GameObject)GameObject.Instantiate(prefabCard);

            if (PokerObserver.Game.IsMainPlayer(p.userName))
            {
                if (hands != null && hands.Length == handSize)
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
        PokerObserver.Game.IsClientListening = false;
        PokerObserver.Instance.isWaitingFinishGame = true;

        float totalTimeFinishGame = responseData.time / 1000f;
        float waitTimeViewResultCard = totalTimeFinishGame > 2f ? 1f : 0f;
        float timeEffectPot = totalTimeFinishGame - waitTimeViewResultCard;
        if (responseData.pots.Length > 0)
            timeEffectPot /= responseData.pots.Length;

        bool isFaceUp = PokerObserver.Game.GetTotalPlayerNotFold > 1;
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

        yield return new WaitForSeconds(waitTimeViewResultCard * 3 / 4f);
        #region UPDATE CARD PER POTS SUMMARY
        foreach (ResponseResultSummary summary in responseData.pots)
        {
            ResponseMoneyExchange playerWin = Array.Find<ResponseMoneyExchange>(summary.players, p => p.winner);
            List<string> lstWinner = new List<string>();
            foreach (ResponseMoneyExchange item in summary.players)
            {
                if (item.winner)
                {
                    lstWinner.Add(item.userName);

                    if (PokerObserver.Game.IsMainPlayer(item.userName))
                    {
                        PuSound.Instance.Play(SoundType.PlayerWin);
                        string rankWin = Array.Find<ResponseFinishCardPlayer>(responseData.players, rdp => rdp.userName == item.userName).ranking;
                        
                        if (!string.IsNullOrEmpty(PokerObserver.Game.mUserInfo.info.facebookId))
                            StartCoroutine(gpView.ShowBtnShareFacebook(UTF8Encoder.DecodeEncodedNonAsciiCharacters(rankWin), 3f));
                    }
                }
            }

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

                potContainer.SummaryPot(summary, timeEffectPot);

                if (isFaceUp)
                {
                    List<GameObject> listHightlight = new List<GameObject>();
                    List<GameObject> listMask = new List<GameObject>();
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
                            listHightlight.AddRange(cardsDeal.FindAll(o => list.Contains(o.GetComponent<PokerCardObject>().card.cardId)));
                        }
                    }

                    listMask = cardsDeal.FindAll(o => listHightlight.Contains(o) == false);
                    foreach (GameObject card in listMask)
                        card.GetComponent<PokerCardObject>().SetMask(true);

                    for (int i = 0; i < 20; i++)
                    {
                        listHightlight.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(i % 2 == 0));
                        yield return new WaitForSeconds(timeEffectPot / 20f);
                    }
                    listHightlight.ForEach(o => o.GetComponent<PokerCardObject>().SetHighlight(false));
                    listMask.ForEach(o => o.GetComponent<PokerCardObject>().SetMask(false));
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
        PokerObserver.Game.IsClientListening = true;
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
                if (PokerObserver.Game.IsMainPlayer(player.userName))
                    hands = player.hand;

                SetPositionAvatarPlayer(player.userName);
                if(player.inTurn)
                    GetPlayerUI(player.userName).StartTimer(data.totalTime / 1000f, data.remainingTime / 1000f);
            }
            CreateHand(data.players, hands);
            CreateCardDeal(data.dealComminityCards);

            if(data.pot != null && data.pot.Length > 0)
                potContainer.UpdatePot(new List<ResponseUpdatePot.DataPot>(data.pot));
        }
    }

    public PokerPlayerUI GetPlayerUI(string userName)
    {
        if(dictPlayerObject.ContainsKey(userName))
            return dictPlayerObject[userName].GetComponent<PokerPlayerUI>();
        return null;
    }

    void Instance_onPlayerListChanged(ResponsePlayerListChanged dataPlayer)
    {
        PokerPlayerChangeAction state = dataPlayer.GetActionState();
        if(state == PokerPlayerChangeAction.playerAdded)
        {
            if (PokerObserver.Game.IsMainPlayer(dataPlayer.player.userName))
                PuSound.Instance.Play(SoundType.Sit_Down);

            SetPositionAvatarPlayer(dataPlayer.player.userName);
        }
        else if ((state == PokerPlayerChangeAction.playerRemoved || state == PokerPlayerChangeAction.playerQuitGame)
            && dictPlayerObject.ContainsKey(dataPlayer.player.userName))
        {
            if (PokerObserver.Game.IsMainPlayer(dataPlayer.player.userName))
                PuSound.Instance.Play(SoundType.StandUp);

            if(PokerObserver.Game.Dealer == dataPlayer.player.userName)
                objectDealer.SetActive(false);

            DestroyCardObject(dictPlayerObject[dataPlayer.player.userName].GetComponent<PokerPlayerUI>().cardOnHands);
            GameObject.Destroy(dictPlayerObject[dataPlayer.player.userName]);
            dictPlayerObject.Remove(dataPlayer.player.userName);
        }

        UpdatePositionPlayers(dataPlayer.player.userName);
    }

    void DestroyCardObject(GameObject[] cards)
    {
        for (int i = cards.Length - 1; i >= 0; i--)
        {
            GameObject card = cards[i];
            cardsDeal.Remove(card);
            GameObject.Destroy(card);
        }
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
            obj = (GameObject)GameObject.Instantiate(prefabPlayer);
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
        if (!_listPotMarkers.Contains(pot))
            _listPotMarkers.Add(pot);
    }

    void UnMarkPot()
    {
        for (int i = _listPotMarkers.Count - 1; i >= 0; i--)
            GameObject.Destroy(_listPotMarkers[i].gameObject);
        _listPotMarkers.Clear();
    }
    #endregion
}
