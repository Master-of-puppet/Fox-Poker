﻿using UnityEngine;
using System.Collections;
using Puppet.Core.Network.Http;
using Puppet;
using Puppet.API.Client;
using Puppet.Utils;
using System.Collections.Generic;

public class LoginScene : MonoBehaviour
{
    public UIEventListener btnLogin, btnForgot, btnFacebook, btnGuest;
    public UIInput txtUsername, txtPassword;

    void Start()
    {
        PuApp.Instance.StartApplication();

        btnLogin.onClick += this.onBtnLoginClick;
        btnForgot.onClick += this.onBtnForgotClick;
        btnFacebook.onClick += this.onBtnFacebookClick;
        btnGuest.onClick += this.onBtnGuestClick;
    }
    void OnDestroy()
    {
        btnLogin.onClick -= this.onBtnLoginClick;
        btnForgot.onClick -= this.onBtnForgotClick;
        btnFacebook.onClick -= this.onBtnFacebookClick;
        btnGuest.onClick -= this.onBtnGuestClick;
    }

    void Update()
    {

    }

    void onBtnLoginClick(GameObject gobj)
    {
        string userName = txtUsername.value;
        string password = txtPassword.value;
        if (string.IsNullOrEmpty(userName))
            userName = "dungnv";
        if (string.IsNullOrEmpty(password))
            password = "puppet#89";
        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
        {
            APILogin.GetAccessToken(userName, password, GetAccessTokenResponse);
        }
    }

    void GetAccessTokenResponse(bool status, string message, IHttpResponse response)
    {
        if (status == false)
        {
          
            PuMain.Setting.Threading.QueueOnMainThread(() =>
            {
                DialogConfirmModel model = new DialogConfirmModel("Lỗi", message, null);
                DialogFactory.QueueOrShowDialog(model);
            });
            
        }
        else
            APILogin.Login(message, LoginResponse);
    }

    void LoginResponse(bool status, string message)
    {
        if (status == false)
            Logger.Log(message);
    }


    void onBtnForgotClick(GameObject gobj)
    {
        for (int i = 0; i < 5; i++)
        {
            Logger.Log("========> LoginScene = " + i);
            DialogConfirmModel model = new DialogConfirmModel("5 cái dialog", "Dialog thứ " + i, null);
            PuMain.Setting.Threading.QueueOnMainThread(() =>
            {
                DialogFactory.QueueOrShowDialog(model);
            });    
        }
        
    }
    void onBtnFacebookClick(GameObject gobj)
    {

    }
    void onBtnGuestClick(GameObject gobj)
    {
        string uuid = SystemInfo.deviceUniqueIdentifier;
    }
}
