using Puppet.API.Client;
using Puppet.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PokerPlazaPresenter : IPlazaPresenter
{
    public PokerPlazaPresenter(IPlazaView view)
    {
        Puppet.Poker.PokerMain.Instance.EnterPoker();
        this.view = view;
        ViewStart();
    }

    public void GetEvent()
    {
        throw new NotImplementedException();
    }

    public void JoinLobby()
    {
		PuApp.Instance.setting.sceneName = Scene.LobbyScene.ToString ();
        Application.LoadLevel(Scene.LobbyScene.ToString());
    }

    public void PlayNow()
    {
        //APIPlaza.Play();
        APILobby.QuickJoinLobby((status, message, data) =>
        {
            Puppet.Logger.Log("Quick Join Game: {0} - Message: {1}", status, message);
        });
    }

    public void GetListQuest()
    {
        throw new NotImplementedException();
    }

    public IPlazaView view { get; set; }




    public void JoinToEvent()
    {
        throw new NotImplementedException();
    }

    public void ViewStart()
    {
    }
    public void ViewEnd()
    {
        throw new NotImplementedException();
    }
}

