using UnityEngine;
using System.Collections;
using Puppet;
using Puppet.API.Client;
using Puppet.Service;
using System.Collections.Generic;

public class PokerPlazaView : MonoBehaviour ,IPlazaView{
	#region UnityEditor
	public GameObject btnPromotionMission,btnPromotionDaily,btnPromotionFriend;
	List<GameObject> btnEvents;
	public GameObject btnPlayNow,btnLeague,btnLobby,btnEvent,btnHelp,btnReceiverEvent;
	public UITable tableEvent,tablePromotion;
	public UISprite indicatorEvent,indicatorPromotion;
	#endregion

	void Start () {
		//HeaderMenuView.Instance.ShowInWorldGame();
        presenter = new PokerPlazaPresenter(this);
		btnEvents = new List <GameObject>();
        UIEventListener.Get(btnPlayNow).onClick += this.OnBtnPlayNowClick;
        UIEventListener.Get(btnLobby).onClick += this.OnBtnLobbyClick;
        UIEventListener.Get(btnLeague).onClick += this.OnBtnLeagueClick;
        UIEventListener.Get(btnEvent).onClick += this.OnBtnEventClick;
		UIEventListener.Get(btnHelp).onClick += this.OnBtnHelpClick;
		UIEventListener.Get (btnPromotionMission).onClick += this.OnClickPromotionMission;
		UIEventListener.Get (btnPromotionDaily).onClick += this.OnClickPromotionDaily;
		UIEventListener.Get (btnPromotionFriend).onClick += this.OnClickPromotionFriend;
		tablePromotion.GetComponent<UICenterOnChild> ().onCenter += onTableCentered;
	}

	void OnClickPromotionMission (GameObject go)
	{

	}

	void OnClickPromotionDaily (GameObject go)
	{

	}

	void OnClickPromotionFriend (GameObject go)
	{
		DialogService.Instance.ShowDialog (new DialogListFriend(new List<Puppet.Core.Model.UserInfo>()));
	}

	void onTableCentered (GameObject go)
	{
		Debug.Log("==============> " + go.name + go.gameObject.transform.localPosition);
	}

	void OnBtnHelpClick (GameObject go)
	{
		DialogService.Instance.ShowDialog (new DialogHelp ());
	}

    private void OnBtnEventClick(GameObject go)
    {
		DialogService.Instance.ShowDialog (new DialogEvent ());
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
