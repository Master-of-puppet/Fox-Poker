﻿// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using Puppet;
using Puppet.API.Client;
using Puppet.Core.Network.Http;
using Puppet.Service;
using System.Text.RegularExpressions;
using Puppet.Utils.Threading;

public class LoginPresenter : ILoginPresenter
{
	ILoginView view;
    public static string REGEX_EMAIL = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
	public LoginPresenter(ILoginView view){
		this.view = view;

	}
    public void ShowRegister()
    {
		DialogService.Instance.ShowDialog (new DialogRegister (RegisterComplete));
	}

	public void ShowDialogForgot ()
	{
		DialogService.Instance.ShowDialog(new DialogForgot(OnClickForgotPass));
	}

	void OnClickForgotPass (bool? arg1, string arg2)
	{
		if (arg1 == true) {
            if (string.IsNullOrEmpty(arg2))
            {
                view.ShowError("Không được để trống email");
                return;
            }
            else {
                bool isEmail = Regex.IsMatch(arg2,REGEX_EMAIL);
                if(isEmail ){
                    APILogin.RequestChangePassword(arg2, ForgotPassWordCallBack);
                }
                else{
                    view.ShowError("Email không đúng định dạng");
                }
            }
		}
	}
    public void ForgotPassWordCallBack(bool status, string message) {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            view.ShowError(message);
        });
       
    }
    void RegisterComplete(bool? status,string userName,string password){
        if (status == true)
            LoginWithUserName(userName, password);
        else
            view.ShowError("Không đăng ký được tài khoản");
    }
	public void ViewEnd(){
        if (SocialService.Instance != null)
		    SocialService.Instance.onLoginComplete -= onLoginComplete;
	}

    public void ViewStart()
    {
        PuApp.Instance.StartApplication();
        SocialService.Instance.onLoginComplete += onLoginComplete;
    }
	#region ILoginPresenter implementation
    public void LoginTrail()
    {
        APILogin.LoginTrial((bool status, string message) =>
        {
            if (status == false)
                ShowDialogErrorInMainThread(message);
        });
    }

    public void LoginFacebook()
    {
        SocialService.SocialLogin(SocialType.Facebook);
    }
	public void LoginWithUserName (string username, string password)
	{
		APILogin.GetAccessToken (username, password,GetAccessTokenCallBack);
	}

	public void GetAccessTokenWithSocial (string accessToken)
	{
		APILogin.GetAccessTokenFacebook (accessToken, OnGetAccessTokenWithFacebookCallBack);
	}

	public void LoginWithAccessToken (string accessToken)
	{
		APILogin.Login (accessToken, LoginCallBack);
	}

	void OnGetAccessTokenWithFacebookCallBack (bool status, string message, System.Collections.Generic.Dictionary<string, object> data)
	{
		foreach (string key in data.Keys) {
		
		}
		if (data.ContainsKey ("suggestUser")) {
			DialogService.Instance.ShowDialog (new DialogRegister (data ["suggestUser"].ToString(), RegisterComplete));
		} else if (data.ContainsKey ("accessToken")) {
			LoginWithAccessToken(data["accessToken"].ToString());
		}


	}

	#endregion

	void GetAccessTokenCallBack (bool status, string message, IHttpResponse data)
	{
		if (status) {
			LoginWithAccessToken (message);
		} else {
            Logger.Log("============> " + message);
            ShowDialogErrorInMainThread(message);
		}
	}
	
	void LoginCallBack (bool status, string message)
	{
        if (!status)
        {
            Logger.Log("============> " + message);
            ShowDialogErrorInMainThread(message);
        }
            
	}

	void onLoginComplete (SocialType arg1, bool arg2)
	{
		if(arg2){
			GetAccessTokenWithSocial(SocialService.GetSocialNetwork(arg1).AccessToken);
        }
	}
    void ShowDialogErrorInMainThread(string message)
    {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            view.ShowError(message);
        });

    }



}


