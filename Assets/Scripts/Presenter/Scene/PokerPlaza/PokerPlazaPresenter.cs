using Puppet;
using Puppet.API.Client;
using Puppet.Core.Model;
using Puppet.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PokerPlazaPresenter : IPlazaPresenter
{
    public PokerPlazaPresenter(IPlazaView view)
    {
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
        Application.LoadLevelAsync(Scene.LobbyScene.ToString());
    }

    public void PlayNow()
    {
        LoadingView.Instance.Show();
        //APIPlaza.Play();
        APILobby.QuickJoinLobby((status, message, data) =>
        {
            LoadingView.Instance.Show(false);
            int roomId = System.Convert.ToInt32(data);
            if (roomId < 0)
                DialogService.Instance.ShowDialog(new DialogMessage("Message", message, null));
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

