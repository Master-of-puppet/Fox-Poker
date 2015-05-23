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
        if (Lobbies != null)
        {
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
        bool canAdded = false;
        if (!isFiltered)
        {
            if (Lobbies != null && Lobbies.Find(item => item.roomId == obj.roomId) == null)
            {
                canAdded = true;
            }
        }
        else
        {
            List<DataLobby> listLobbyFilter = getListLobbyFilter();
            if (listLobbyFilter.Contains(obj))
            {
                canAdded = true;
            }
        }
        if (canAdded)
        {
            view.AddLobby(obj);
            Lobbies.Add(obj);
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
        isFiltered = false;
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


    public void SearchLobby(string id, Dictionary<int, bool> cbArr)
    {
        searchId = id;
        searchDictionary = cbArr;
        FilterLobbies();
    }

    private void FilterLobbies()
    {
        isFiltered = true;
      
        view.DrawLobbies(lobbies);
    }
    private List<DataLobby> getListLobbyFilter()
    {
        List<DataLobby> lobbiesData;
        int? totalPlayer = null;
        int? roomId = null;
        if (searchDictionary != null)
        {
            if (!searchDictionary[SearchView.TYPE_5_PEOPLE] && searchDictionary[SearchView.TYPE_9_PEOPLE])
            {
                totalPlayer = 9;
            }
            else if (searchDictionary[SearchView.TYPE_5_PEOPLE] && !searchDictionary[SearchView.TYPE_9_PEOPLE])
            {
                totalPlayer = 5;
            }
        }
        if (!string.IsNullOrEmpty(searchId))
        {
            roomId = Convert.ToInt16(searchId);
        }
        lobbiesData = Puppet.API.Client.APILobby.FillterCurrentChannel(null, null, roomId, totalPlayer, null, null);
        return lobbiesData;
    }
    public Dictionary<int, bool> searchDictionary;

    public string searchId;

    public bool isFiltered { get; set; }
}

