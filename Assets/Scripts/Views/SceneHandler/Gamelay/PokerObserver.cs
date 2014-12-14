using UnityEngine;
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
    /// <summary>
    /// Info Setting Poker Game
    /// </summary>
    public PokerGameDetails gameDetails;

    public event Action<ResponseUpdateGame> onFirstJoinGame;
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

    public UserInfo mUserInfo;
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
        get { return Puppet.API.Client.APIPokerGame.GetPokerGameplay(); }
    }

    public void StartGame()
    {
        mUserInfo = Puppet.API.Client.APIUser.GetUserInformation();
        Puppet.Poker.EventDispatcher.onGameEvent += EventDispatcher_onGameEvent;
        Game.StartListenerEvent();
    }

    void EventDispatcher_onGameEvent(string command, object data)
    {
        //Logger.Log(ELogColor.YELLOW, "Client handled: " + command + " - " + DateTime.Now.ToString("hh:mm:ss"));
        if (data is ResponseUpdateGame)
        {
            ResponseUpdateGame dataGame = (ResponseUpdateGame)data;
            if (gameDetails == null && dataGame.gameDetails != null)
                gameDetails = dataGame.gameDetails;
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
        APIGeneric.BackScene(null);
        _instance = null;
    }

    public void SitDown(int slotServer, double betting)
    {
        APIPokerGame.SitDown(slotServer, betting);
    }
    #endregion

    #region PLAYER
    public bool IsMainPlayer(string userName)
    {
        return mUserInfo.info.userName == userName;
    }

    public bool IsMainPlayerSatDown()
    {
        return Game.ListPlayer.Find(p => p.userName == mUserInfo.info.userName) != null;
    }

    public bool IsMainTurn { get { try { return Game.CurrentPlayer.userName == Game.MainPlayer.userName; } catch { return false; } } }
    #endregion

    #region BETTING VALUE
    public double CurrentBettingDiff
    {
        get
        {
            if (Game.CurrentPlayer == null)
                return 0;
            double leftMoney = Game.CurrentPlayer.GetMoney();
            double diff = Game.MaxCurrentBetting - Game.MainPlayer.currentBet;
            return leftMoney > diff ? diff : leftMoney;
        }
    }
    #endregion
}
