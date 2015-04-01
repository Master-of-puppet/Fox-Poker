using Puppet;
using Puppet.API.Client;
using Puppet.Core.Model;
using Puppet.Core.Network.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class GameItemPresenter : IGameItemPresenter
{
    private DataGame data;
    public DataGame Data {
        get {
            return data;
        }
        set {
            this.data = value;
            LoadImage();
        }
    }
    public GameItemPresenter(IGameItemView view)
    {
        this.view = view;
    }

    public void JoinToGame()
    {
        APIWorldGame.JoinRoom(data, OnJoinRoomCallBack);
    }

    private void OnJoinRoomCallBack(bool status, string message)
    {
        if (!status)
        {
            PuMain.Setting.Threading.QueueOnMainThread(() =>
            {
                view.ShowError(message);
            });
        }
    }

    public void LoadImage()
    {
        PuApp.Instance.GetImage(data.icon, (texture) => view.ShowImage(texture));
    }

    private IGameItemView view { get; set; }

}

