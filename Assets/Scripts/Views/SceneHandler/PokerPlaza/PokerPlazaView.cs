﻿using UnityEngine;
using System.Collections;
using Puppet;
using Puppet.API.Client;

public class PokerPlazaView : MonoBehaviour ,IPlazaView{
	public GameObject btnPlayNow,btnLeague,btnLobby,btnEvent;
	void Start () {
		HeaderMenuView.Instance.ShowInPlaza ();
        presenter = new PokerPlazaPresenter(this);
        UIEventListener.Get(btnPlayNow).onClick += this.OnBtnPlayNowClick;
        UIEventListener.Get(btnLobby).onClick += this.OnBtnLobbyClick;
        UIEventListener.Get(btnLeague).onClick += this.OnBtnLeagueClick;
        UIEventListener.Get(btnEvent).onClick += this.OnBtnEventClick;

	}

    private void OnBtnEventClick(GameObject go)
    {
        throw new System.NotImplementedException();
    }

    private void OnBtnLeagueClick(GameObject go)
    {
        throw new System.NotImplementedException();
    }
	void OnDestroy(){
        UIEventListener.Get(btnPlayNow).onClick -= this.OnBtnPlayNowClick;
        UIEventListener.Get(btnLobby).onClick -= this.OnBtnLobbyClick;
	}
	
	void OnBtnPlayNowClick(GameObject obj){
        presenter.PlayNow();
	}
	void OnBtnLobbyClick(GameObject obj){
        presenter.JoinLobby();
	}
	
    public void ShowEvent()
    {
        throw new System.NotImplementedException();
    }

    public void ShowQuestionToReceiverGold()
    {
        throw new System.NotImplementedException();
    }

    public PokerPlazaPresenter presenter { get; set; }
}
