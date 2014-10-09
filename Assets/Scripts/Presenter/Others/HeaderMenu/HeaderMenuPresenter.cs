// ------------------------------------------------------------------------------
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
	public HeaderMenuPresenter(IHeaderMenuView view){
		this.view = view;
	}
	#region IPresenter implementation

	public void ViewStart ()
	{
		UserInfo userInfo = Puppet.API.Client.APIUser.GetUserInformation ();
		view.ShowUserName(userInfo.info.userName);
		if(userInfo.assets !=null && userInfo.assets.content.Length > 0)
			view.ShowChip(userInfo.assets.content[0].value.ToString());
	}

	public void OnBackPressed ()
	{

		if (PuApp.Instance.setting.sceneName == Scene.LobbyScene.ToString ()) {
			Application.LoadLevel (Scene.Poker_Plaza.ToString ());
			PuApp.Instance.setting.sceneName = Scene.Poker_Plaza.ToString ();
			return;
		}else if(PuApp.Instance.setting.sceneName == Scene.GameplayScene.ToString ()){
			DialogService.Instance.ShowDialog(new DialogConfirm("XÁC NHẬN THOÁT","Bạn có chắc chắn muốn thoát khỏi bàn chơi",delegate(bool? confirm){
				if(confirm == true){
					PuApp.Instance.BackScene();
				}
			}));
		}else
			PuApp.Instance.BackScene();
	}
	

	public void ViewEnd ()
	{

	}



	public void ShowDialogProfile ()
	{
		
	}

	public void ShowDialogCommon ()
	{

	}

	public void ShowDialogRecharge ()
	{

	}

	public void ShowDialogSettings ()
	{

	}

	public void ShowDialogMessage ()
	{

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

