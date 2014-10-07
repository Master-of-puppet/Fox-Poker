﻿using Puppet.API.Client;
using Puppet.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


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
		HeaderMenuView.Instance.ShowInLobby ();
	
        this.view = view;
        ViewStart();
    }



    public void LoadChannels()
    {
        APILobby.GetGroupsLobby(OnGetGroupNameCallback);
    }

    private void OnGetGroupNameCallback(bool status, string message, List<Puppet.Core.Model.DataChannel> data)
    {
        if (status){
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
        selectedChannel = channel;
        APILobby.SetSelectChannel(channel, OnGetAllLobbyInChannel);
    }

    private void OnGetAllLobbyInChannel(bool status, string message, List<Puppet.Core.Model.DataLobby> data)
    {
        if (status)
        {
            this.lobbies = data;
			view.DrawLobbies(data);
			
//			PuApp.Instance.StartCoroutine(DrawLobbies(this.lobbies));
        
        }
        else
            view.ShowError(message);
    }
	IEnumerator DrawLobbies(List<DataLobby> data){
		yield return new WaitForSeconds (0.5f);
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
        APILobby.CreateLobby(OnCreateLobbyCallBack);
    }

    private void OnCreateLobbyCallBack(bool status, string message)
    {
        if (!status)
            view.ShowError(message);
    }

    public ILobbyView view { get; set; }
}
