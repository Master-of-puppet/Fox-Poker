﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet.Poker;
using Puppet.Poker.Datagram;
using System;
using Puppet.API.Client;
using Puppet;
using Puppet.Core.Model;
using Puppet.Poker.Models;

public class PokerObserver
{
    public event Action<ResponseUpdateGame> dataUpdateGameChange;
    public event Action<ResponsePlayerListChanged> onPlayerListChanged;
    public event Action<ResponseUpdateHand> onEventUpdateHand;
    public event Action<ResponseUpdateTurnChange> onTurnChange;
    public event Action<ResponseFinishGame> onFinishGame;
    public event Action<ResponseWaitingDealCard> onNewRound;
    public event Action<ResponseUpdatePot> onUpdatePot;
    public event Action<ResponseUpdateUserInfo> onUpdateUserInfo;
    public event Action<ResponseError> onEncounterError;
    public event Action<ResponseUpdateRoomMaster> onUpdateRoomMaster;

    public bool isWaitingFinishGame = false;
    
    static PokerObserver _instance;
    public static PokerObserver Instance
    {
        get 
        {
            if (_instance == null)
                _instance = new PokerObserver();
            return _instance; 
        }
    }

    public static PokerGameplay Game
    {
        get { return APIPokerGame.GetPokerGameplay(); }
    }

    public void StartGame()
    {
        isWaitingFinishGame = false;
        Puppet.Poker.EventDispatcher.onGameEvent = EventDispatcher_onGameEvent;
        Game.IsClientListening = true;
    }

    void EventDispatcher_onGameEvent(string command, object data)
    {
        //Logger.Log(ELogColor.YELLOW, "**** Client handled: " + command + " - " + DateTime.Now.ToString("hh:mm:ss"));

        if (data is ResponseUpdateGame)
        {
            ResponseUpdateGame dataGame = (ResponseUpdateGame)data;
            if (dataUpdateGameChange != null)
                dataUpdateGameChange(dataGame);
        }
        else if (data is ResponsePlayerListChanged)
        {
            ResponsePlayerListChanged dataPlayerChange = (ResponsePlayerListChanged)data;
            if (onPlayerListChanged != null)
                onPlayerListChanged(dataPlayerChange);
        }
        else if (data is ResponseUpdateHand && onEventUpdateHand != null)
            onEventUpdateHand((ResponseUpdateHand)data);
        else if (data is ResponseUpdateTurnChange)
        {
            ResponseUpdateTurnChange dataTurn = (ResponseUpdateTurnChange)data;

            if (onTurnChange != null)
                onTurnChange(dataTurn);
        }
        else if (data is ResponseFinishGame && onFinishGame != null)
            onFinishGame((ResponseFinishGame)data);
        else if (data is ResponseWaitingDealCard && onNewRound != null)
            onNewRound((ResponseWaitingDealCard)data);
        else if (data is ResponseUpdateRoomMaster && onUpdateRoomMaster != null)
            onUpdateRoomMaster((ResponseUpdateRoomMaster)data);
        else if (data is ResponseUpdatePot)
        {
            if (onUpdatePot != null)
                onUpdatePot((ResponseUpdatePot)data);
        }
        else if (data is ResponseUpdateUserInfo)
        {
            ResponseUpdateUserInfo dataUserInfo = (ResponseUpdateUserInfo)data;

            if (onUpdateUserInfo != null)
                onUpdateUserInfo(dataUserInfo);
        }
        else if (data is ResponseError && onEncounterError != null)
            onEncounterError((ResponseError)data);
    }

    #region HANDLE BUTTON
    public void QuitGame()
    {
        APIGeneric.BackScene((status, message) =>
        {
            if (status)
                _instance = null;
        });
    }

    public void SitDown(int slotServer, double betting, bool isAutoBuy)
    {
        APIPokerGame.SitDown(slotServer, betting, isAutoBuy);
    }

    public void Request(PokerRequestPlay type, double value)
    {
        APIPokerGame.PlayRequest(type, value);
    }

    public void AutoSitDown()
    {
        APIPokerGame.AutoSitDown();
    }

    public void StandUp()
    {
        APIPokerGame.StandUp();
    }
    #endregion
}
