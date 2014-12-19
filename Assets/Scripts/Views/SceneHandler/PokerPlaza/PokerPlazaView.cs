using UnityEngine;
using System.Collections;
using Puppet;
using Puppet.API.Client;
using Puppet.Service;

public class PokerPlazaView : MonoBehaviour ,IPlazaView{
	public GameObject btnPlayNow,btnLeague,btnLobby,btnEvent,btnHelp;
	void Start () {
		HeaderMenuView.Instance.ShowInPlaza ();
        presenter = new PokerPlazaPresenter(this);
        UIEventListener.Get(btnPlayNow).onClick += this.OnBtnPlayNowClick;
        UIEventListener.Get(btnLobby).onClick += this.OnBtnLobbyClick;
        UIEventListener.Get(btnLeague).onClick += this.OnBtnLeagueClick;
        UIEventListener.Get(btnEvent).onClick += this.OnBtnEventClick;
		UIEventListener.Get(btnHelp).onClick += this.OnBtnHelpClick;

	}

	void OnBtnHelpClick (GameObject go)
	{
		DialogService.Instance.ShowDialog (new DialogHelp ());
	}

    private void OnBtnEventClick(GameObject go)
    {
        throw new System.NotImplementedException();
    }

    private void OnBtnLeagueClick(GameObject go)
    {
		#if UNITY_ANDROID
		AndroidLocalPushNotification.sendPushNotification (10, "Fox Poker", "Ban vua bam tham gia giai dau");
		#endif
	}
	void OnDestroy(){
        UIEventListener.Get(btnPlayNow).onClick -= this.OnBtnPlayNowClick;
        UIEventListener.Get(btnLobby).onClick -= this.OnBtnLobbyClick;
		UIEventListener.Get(btnHelp).onClick -= this.OnBtnHelpClick;
		UIEventListener.Get(btnLeague).onClick -= this.OnBtnLeagueClick;
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
