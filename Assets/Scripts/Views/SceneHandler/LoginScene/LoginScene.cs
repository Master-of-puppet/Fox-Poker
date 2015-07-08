using UnityEngine;
using System.Collections;
using Puppet.Core.Network.Http;
using Puppet;
using Puppet.API.Client;
using Puppet.Utils;
using System.Collections.Generic;
using Puppet.Service;
using System;
using HoldemHand;
using Puppet.Poker.Models;
using System.Linq;

public class LoginScene : MonoBehaviour, ILoginView
{
    public GameObject btnLogin, btnForgot, btnFacebook, btnGuest, btnRegister,btnHelp;

    public UIInput txtUsername, txtPassword;

    LoginPresenter presenter;
//	public UI2DSpriteAnimation animation;
   
    void Awake()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
            Screen.SetResolution(960, 640, false);

        //PuSetting.UniqueDeviceId = SystemInfo.deviceUniqueIdentifier;
    }

    void Start()
    {
        presenter = new LoginPresenter(this);
        presenter.ViewStart();
	}
    void OnEnable()
    {
        UIEventListener.Get(btnLogin).onClick += this.onBtnLoginClick;
        UIEventListener.Get(btnForgot).onClick += this.onBtnForgotClick;
        UIEventListener.Get(btnFacebook).onClick += this.onBtnFacebookClick;
        UIEventListener.Get(btnGuest).onClick += this.onBtnGuestClick;
        UIEventListener.Get(btnRegister).onClick += this.onBtnRegisterClick;
        UIEventListener.Get(btnHelp).onClick += this.onBtnHelpClick;
    }


    void OnDisable()
    {
		UIEventListener.Get(btnLogin).onClick -= this.onBtnLoginClick;
		UIEventListener.Get(btnForgot).onClick -= this.onBtnForgotClick;
		UIEventListener.Get(btnFacebook).onClick -= this.onBtnFacebookClick;
		UIEventListener.Get(btnGuest).onClick -= this.onBtnGuestClick;
		UIEventListener.Get(btnRegister).onClick -= this.onBtnRegisterClick;
        UIEventListener.Get(btnHelp).onClick += this.onBtnHelpClick;
        presenter.ViewEnd();
    }

    /*
    void OnGUI()
    {
        PuSetting.UniqueDeviceId = GUI.TextField(new Rect(0, 0, Screen.width / 4, Screen.height / 15), PuSetting.UniqueDeviceId);
        //if (GUI.Button(new Rect(0, 150, Screen.width - Screen.width * 0.9f, 35f), "TEST MODE"))
        //{
        //    Logger.Log("========> " + APIPokerGame.GetPokerGameplay().ListPlayer.Count);
        //    TestModeGUI.Create(ActionRequestOrderHand);
        //}
    }
    */

    public void ActionRequestOrderHand(Dictionary<string, int[]> obj) {
        List<PokerCard> idsCard = new List<PokerCard>();
        foreach (var item in obj.Keys)
        {

            foreach (var items in obj[item])
            {
                idsCard.Add(new PokerCard(items));
            }

        }        
    }

    void onBtnLoginClick(GameObject gobj)
    {
        string userName = txtUsername.value;
        string password = txtPassword.value;
        //if (string.IsNullOrEmpty(userName))
        //    DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Vui lòng nhập thông tin về tài khoản của bạn"));
        //    userName = Application.platform == RuntimePlatform.WindowsPlayer ? "dungnv2" : "dungnv";
        //if (string.IsNullOrEmpty(password))
        //    DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Vui lòng nhập mật khẩu truy cập."));
        //    password = Application.platform == RuntimePlatform.WindowsPlayer ? "1234" : "puppet#89";
        presenter.LoginWithUserName(userName, password);
    }

    void onBtnForgotClick(GameObject gobj)
    {
        presenter.ShowDialogForgot();
     
    }
    private void onBtnHelpClick(GameObject go)
    {
        DialogService.Instance.ShowDialog(new DialogHelp());
    }
    private void onBtnRegisterClick(GameObject go)
    {
        presenter.ShowRegister();
    }
    void onBtnFacebookClick(GameObject gobj)
    {
        presenter.LoginFacebook();
    }
    void onBtnGuestClick(GameObject gobj)
    {
    	presenter.LoginTrail();

    }


    #region ILoginView implementation

    public void ShowError(string message)
    {
        DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", message, null));
    }

    #endregion


    public void ShowConfirm(string message, Action<bool?> action)
    {

    }


  
}
