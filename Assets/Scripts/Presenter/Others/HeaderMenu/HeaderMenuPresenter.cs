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
using Puppet.Core.Model;
using Puppet;
using UnityEngine;
using Puppet.Service;
using Puppet.API.Client;

public class HeaderMenuPresenter : IHeaderMenuPresenter
{
	IHeaderMenuView view;
	public Action OnShowLobbyRowTypeCallBack;
    public Action onHandleQuit;
    UserInfo userInfo;

	public HeaderMenuPresenter(IHeaderMenuView view){
		this.view = view;
	}
	#region IPresenter implementation

	public void ViewStart ()
	{
		userInfo = Puppet.API.Client.APIUser.GetUserInformation ();
        userInfo.onDataChanged += OnDataUserChange;

		view.ShowUserName(userInfo.info.userName);
		if(userInfo.assets !=null && userInfo.assets.content.Length > 0)
			view.ShowChip(userInfo.assets.content[0].value.ToString());
        PuApp.Instance.GetImage(userInfo.info.avatar, (texture) => view.ShowAvatar(texture));
	}

    void OnDataUserChange(IDataModel info)
    {
        UserInfo user = (UserInfo)info;
        view.ShowChip(user.assets.GetAsset(EAssets.Chip).value.ToString());
        PuApp.Instance.GetImage(user.info.avatar, (texture) => view.ShowAvatar(texture));
    }

	public void OnBackPressed ()
	{
		if (PuApp.Instance.setting.sceneName == Scene.LobbyScene.ToString ()) 
        {
			Application.LoadLevel (Scene.Poker_Plaza.ToString ());
			PuApp.Instance.setting.sceneName = Scene.Poker_Plaza.ToString ();
			return;
		}
        else if(PuApp.Instance.setting.sceneName == Scene.GameplayScene.ToString ())
        {
            if(onHandleQuit != null)
                onHandleQuit();
		}
        else
			PuApp.Instance.BackScene();
	}
	

	public void ViewEnd ()
	{
        userInfo.onDataChanged -= OnDataUserChange;
	}



	public void ShowDialogProfile ()
	{
		DialogService.Instance.ShowDialog (new DialogInfo (APIUser.GetUserInformation()));
	}

	public void ShowDialogCommon ()
	{
		DialogService.Instance.ShowDialog(new DialogListFriend(null));
	}

	public void ShowDialogRecharge ()
	{
		APIGeneric.GetInfoRecharge((status, message, dataRecharge) =>
		                           {
			if(status){
				DialogRecharge recharge = new DialogRecharge(dataRecharge);
				DialogService.Instance.ShowDialog(recharge);
			}
		});

	}

	public void ShowDialogSettings ()
	{
		UserInfo userInfo = Puppet.API.Client.APIUser.GetUserInformation ();
		DialogService.Instance.ShowDialog(new DialogSetting(userInfo.info.userName,"1.0"));
	}

	public void ShowDialogMessage ()
	{
		DialogService.Instance.ShowDialog (new DialogInbox ());
	}

	public void ShowDialogSearch ()
	{

	}

	public void ShowLobbyType ()
	{
		if (OnShowLobbyRowTypeCallBack != null) {
			OnShowLobbyRowTypeCallBack();
		}
	}

	#endregion



}

