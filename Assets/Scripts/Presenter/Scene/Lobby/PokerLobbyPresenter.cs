using Puppet.API.Client;
using Puppet.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Puppet.Service;
using Puppet;


public class PokerLobbyPresenter : ILobbyPresenter
{
    private List<Puppet.Core.Model.DataChannel> channels;
    private List<Puppet.Core.Model.DataLobby> lobbies;
    public List<Puppet.Core.Model.DataLobby> Lobbies
    {
        get { return lobbies; }
        set { lobbies = value; }
    }
    private DataChannel selectedChannel;

    public DataChannel SelectedChannel
    {
        get { return selectedChannel; }
        set { selectedChannel = value; }
    }
    public List<Puppet.Core.Model.DataChannel> Channels
    {
        get { return channels; }
        set { channels = value; }
    }
    public PokerLobbyPresenter(ILobbyView view)
    {
        HeaderMenuView.Instance.ShowInLobby();

        this.view = view;
        ViewStart();
    }



    public void LoadChannels()
    {
        APILobby.AddListener(onCreateCallback, onUpdateCallback, onDeleteCallback);
        APILobby.GetGroupsLobby(OnGetGroupNameCallback);
    }

    private void onDeleteCallback(DataLobby obj)
    {
        if (Lobbies != null) { 
            DataLobby lob = Lobbies.Find(i => i.roomId == obj.roomId);
            if (lob != null)
            {
                view.RemoveLobby(obj);
                Lobbies.Remove(lob);
            }
        }
    }

    private void onUpdateCallback(DataLobby obj)
    {
        view.UpdateLobby(obj);
        DataLobby lob = Lobbies.Find(i => i.roomId == obj.roomId);
        lob = obj;
    }

    private void onCreateCallback(DataLobby obj)
    {
        if (Lobbies != null && Lobbies.Find(item=>item.roomId == obj.roomId) == null)
        {
            view.AddLobby(obj);
        }
    }

    private void OnGetGroupNameCallback(bool status, string message, List<Puppet.Core.Model.DataChannel> data)
    {
        if (status)
        {
            this.channels = data;
            PuApp.Instance.StartCoroutine(DrawChannelsAndLoadDefaultsLobbyInChannel(channels));
        }
        else
            view.ShowError(message);
    }
    IEnumerator DrawChannelsAndLoadDefaultsLobbyInChannel(List<DataChannel> data)
    {
        view.DrawChannels(data);

        yield return new WaitForEndOfFrame();
        LoadLobbiesByChannel(data[0]);
    }

    public void LoadAllLobbies()
    {
        APILobby.GetAllLobby(OnGetAllLobbyInChannel);
    }

    public void LoadLobbiesByChannel(Puppet.Core.Model.DataChannel channel)
    {
        if (lobbies != null)
            lobbies = null;
        selectedChannel = channel;
        APILobby.SetSelectChannel(channel, OnGetAllLobbyInChannel);
    }

    private void OnGetAllLobbyInChannel(bool status, string message, List<Puppet.Core.Model.DataLobby> data)
    {
        if (status)
        {
            if (lobbies == null)
            {
                this.lobbies = data;
                view.DrawLobbies(data);
            }
        }
        else
            view.ShowError(message);
    }
    IEnumerator DrawLobbies(List<DataLobby> data)
    {
        yield return new WaitForSeconds(0.5f);
        view.DrawLobbies(data);
    }
    public void JoinToGame(Puppet.Core.Model.DataLobby lobby)
    {
        APILobby.JoinLobby(lobby, (bool status, string message) =>
        {
            if (!status)
                view.ShowError(message);
        });
    }

    public void ViewStart()
    {
        LoadChannels();
    }

    public void ViewEnd()
    {
    }


    public void CreateLobby()
    {
        DialogService.Instance.ShowDialog(new DialogCreateGame(new List<double>(selectedChannel.configuration.betting)));

    }

    private void OnCreateLobbyCallBack(bool status, string message)
    {
        if (!status)
            view.ShowError(message);
    }
    public ILobbyView view { get; set; }


    public void SearchLobby(string id, bool[] cbArr)
    {
        List<DataLobby> lobbyFilters = new List<DataLobby>();
        if (!string.IsNullOrEmpty(id))
        {
            lobbyFilters.AddRange(lobbies.FindAll(s => s.roomId == Int16.Parse(id)));
            view.DrawLobbies(lobbyFilters);
        }
        else
        {
            view.DrawLobbies(lobbies);
        }
    }
}

