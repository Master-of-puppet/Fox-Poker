using UnityEngine;
using System.Collections;
using Puppet;
using Puppet.API.Client;
using Puppet.Service;
using System.Collections.Generic;
using System;
using Puppet.Core.Model;

public class PokerPlazaView : MonoBehaviour ,IPlazaView{
	#region UnityEditor
	public GameObject[] btnFreeGold;
	public List<GameObject> btnEvents;
	public GameObject btnPlayNow,btnLeague,btnLobby,btnEvent,btnHelp,btnReceiverEvent,btnReplay;
	public UITable tableEvent,tablePromotion;
    public UISprite indicatorEvent, indicatorPromotion;
	#endregion

	void Start () {
		HeaderMenuView.Instance.ShowInWorldGame();
        presenter = new PokerPlazaPresenter(this);
        UIEventListener.Get(btnPlayNow).onClick += this.OnBtnPlayNowClick;
        UIEventListener.Get(btnLobby).onClick += this.OnBtnLobbyClick;
        UIEventListener.Get(btnLeague).onClick += this.OnBtnLeagueClick;
        UIEventListener.Get(btnEvent).onClick += this.OnBtnEventClick;
		UIEventListener.Get(btnHelp).onClick += this.OnBtnHelpClick;
        UIEventListener.Get(btnReplay).onClick += this.OnBtnHelpClick;
        foreach (GameObject item in btnFreeGold)
        {
            UIEventListener.Get(item).onClick += this.OnClickPromotion;
        }
        tablePromotion.GetComponent<UICenterOnChild>().onCenter += onTablePromotionGoToCenter;
        tableEvent.GetComponent<UICenterOnChild>().onCenter += onTableEventGoToCenter;
        initIndicator(btnFreeGold.Length, indicatorPromotion);
        fetchEvent();
	}

 
    private void fetchEvent()
    {
        Puppet.API.Client.APIGeneric.GetInfoEvents((status, message, data) =>
        {
            if (status)
            {
                foreach (DataEvent e in data.items)
                {
                    PlazaEventItem itemEvent = PlazaEventItem.Create(e);
                    itemEvent.transform.parent = tableEvent.transform;
                    itemEvent.transform.localPosition = Vector3.zero;
                    itemEvent.transform.localScale= Vector3.one;
                    UIEventListener.Get(itemEvent.gameObject).onClick += OnClickToEvent;
                    btnEvents.Add(itemEvent.gameObject);
                }
                tableEvent.Reposition();
                initIndicator(btnEvents.Count, indicatorEvent);
            }
            tableEvent.GetComponent<UICenterOnChild>().CenterOn(btnEvents[0].transform);
        });
    }

    private void OnClickToEvent(GameObject go)
    {
        PlazaEventItem itemEvent = go.GetComponent<PlazaEventItem>();
        string url = itemEvent.evt.url;
        if (itemEvent.evt.GetEventType() == Puppet.EventType.WebView)
        {
            DialogService.Instance.ShowDialog(new DialogEvent(url));
        }
        else
        {
            url = itemEvent.evt.extra_url;
            Application.OpenURL(url);
        }
    }
	void OnClickPromotion (GameObject go)
	{
        switch (go.name)
        {
            case "mission":
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Tính năng đang được xây dựng, vui lòng quay lại sau", null));
                //DialogService.Instance.ShowDialog(new DialogMission());
                break;
            case "daily":
                if (PuApp.Instance.dailyGift == null)
                    DialogService.Instance.ShowDialog(new DialogMessage("Thông báo","Bạn đã nhận quà rồi",null));
                else
                    DialogService.Instance.ShowDialog(new DialogPromotion(PuApp.Instance.dailyGift));
                break;
            case "friend":
                DialogService.Instance.ShowDialog(new DialogListFriend(new List<UserInfo>()));
                break;
            default:
                break;
        }
	}


    private void onTableEventGoToCenter(GameObject centeredObject)
    {
        int indexIndicator = btnEvents.FindIndex(item => item == centeredObject);
        Vector3 currentPosition = indicatorEvent.transform.localPosition;
        UISprite foreground = indicatorEvent.transform.FindChild("Foreground").GetComponent<UISprite>();
        int positionX = foreground.width / 2 + indexIndicator * foreground.width;
        Vector3 translateTo = new Vector3(positionX, 0, 0);
        foreground.transform.localPosition = translateTo;
    }
    void onTablePromotionGoToCenter(GameObject  go)
	{
        int indexIndicator = Array.FindIndex(btnFreeGold, item => item == go);
        Vector3 currentPosition  = indicatorPromotion.transform.localPosition;
        UISprite foreground = indicatorPromotion.transform.FindChild("Foreground").GetComponent<UISprite>();
        int positionX = foreground.width/2 + indexIndicator*foreground.width;
        Vector3 translateTo = new Vector3(positionX, 0, 0);
        foreground.transform.localPosition = translateTo;

	}
    private void initIndicator(int size,UISprite sprite)
    {
        sprite.width = size * 16;
        sprite.transform.localPosition = new Vector3(-sprite.width/2,sprite.transform.localPosition.y,sprite.transform.localPosition.z);
    }
	void OnBtnHelpClick (GameObject go)
	{
		DialogService.Instance.ShowDialog (new DialogHelp ());
	}

    private void OnBtnEventClick(GameObject go)
    {
		DialogService.Instance.ShowDialog (new DialogEvent (null));
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
        UIEventListener.Get(btnReplay).onClick -= this.OnBtnHelpClick;
		UIEventListener.Get(btnLeague).onClick -= this.OnBtnLeagueClick;
        foreach (GameObject item in btnFreeGold)
        {
            UIEventListener.Get(item).onClick += this.OnClickPromotion;
        }
        foreach (GameObject item in btnEvents)
        {
            if(item.GetComponent<PlazaEventItem>() !=null){
                UIEventListener.Get(item).onClick -= this.OnClickToEvent;
            }
        }
        tablePromotion.GetComponent<UICenterOnChild>().onCenter += onTablePromotionGoToCenter;
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
