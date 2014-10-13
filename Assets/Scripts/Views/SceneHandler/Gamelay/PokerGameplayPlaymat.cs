﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet.Poker;
using System;
using Puppet.Poker.Models;
using Puppet.Poker.Datagram;
using Puppet;

public class PokerGameplayPlaymat : MonoBehaviour
{
    #region UNITY EDITOR
    public Transform []positionDealCards;
    #endregion
    PokerGPSide[] arrayPokerSide;
    Dictionary<string, GameObject> dictPlayerObject = new Dictionary<string, GameObject>();

    void Awake()
    {
        arrayPokerSide = GameObject.FindObjectsOfType<PokerGPSide>();

        PokerGameModel.Instance.dataFirstJoinGame += Instance_dataFirstJoinGame;
        PokerGameModel.Instance.dataPlayerListChanged += Instance_dataPlayerListChanged;
        PokerGameModel.Instance.dataUpdateGameChange += Instance_dataUpdateGame;
        PokerGameModel.Instance.onEventUpdateHand += Instance_onEventUpdateHand;
        PokerGameModel.Instance.dataTurnGame += Instance_dataTurnGame;
        PokerGameModel.Instance.onNewRound += Instance_onNewRound;
    }

    void Instance_onNewRound(ResponseWaitingDealCard data)
    {
        countGenericCard = 0;
        for(int i = cardsDeal.Count-1;i>=0;i--)
            GameObject.Destroy(cardsDeal[i]);

        cardsDeal.Clear();
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
        CreateHand(data);
    }


    void CreateHand(ResponseUpdateHand data)
    {
        foreach(PokerPlayerController p in data.players)
        {
            int handSize = p.handSize;
            GameObject[] cardObjects = new GameObject[handSize];
            for (int i = 0; i < handSize;i++)
                cardObjects[i] = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/CardUI"));

            if (PokerGameModel.Instance.mUserInfo.info.userName == p.userName)
                for(int i=0;i<handSize;i++)
                    cardObjects[i].GetComponent<PokerCardObject>().SetDataCard(new PokerCard(data.hand[i]), i);
            else
                for (int i = 0; i < handSize; i++)
                    cardObjects[i].GetComponent<PokerCardObject>().SetDataCard(new PokerCard(), i);

            dictPlayerObject[p.userName].GetComponent<PokerPlayerUI>().UpdateSetCardObject(cardObjects);

            cardsDeal.AddRange(cardObjects);
        }
    }

    void Instance_dataFirstJoinGame(ResponseUpdateGame data)
    {
        foreach (PokerPlayerController player in data.players)
        {
            SetPositionAvatarPlayer(player);
        }
    }

    void Instance_dataUpdateGame(ResponseUpdateGame data)
    {
        foreach (PokerPlayerController player in data.players)
        {
            SetPositionAvatarPlayer(player);
        }
    }

    void Instance_dataPlayerListChanged(ResponsePlayerListChanged dataPlayer)
    {
        PokerPlayerChangeAction state = dataPlayer.GetActionState();
        if(state == PokerPlayerChangeAction.playerAdded)
        {
            SetPositionAvatarPlayer(dataPlayer.player);
        }
        else if(state == PokerPlayerChangeAction.playerRemoved)
        {
            GameObject.Destroy(dictPlayerObject[dataPlayer.player.userName]);
            dictPlayerObject.Remove(dataPlayer.player.userName);
        }
    }

    public PokerGPSide GetPokerSide(PokerSide side)
    {
        return Array.Find<PokerGPSide>(arrayPokerSide, s => s.CurrentSide == side);
    }

    void SetPositionAvatarPlayer(PokerPlayerController player)
    {
        GameObject obj;
        if (dictPlayerObject.ContainsKey(player.userName))
            obj = dictPlayerObject[player.userName];
        else
        {
            obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PlayerUI"));
            obj.GetComponent<PokerPlayerUI>().SetData(player);
            dictPlayerObject.Add(player.userName, obj);
        }

        PokerGPSide playerSide = GetPokerSide(player.GetSide());
        obj.GetComponent<PokerPlayerUI>().side = playerSide;

        obj.transform.parent = playerSide.transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }


}
