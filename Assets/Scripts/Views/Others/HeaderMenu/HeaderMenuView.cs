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
using Puppet;
using UnityEngine;
using System.Collections.Generic;
using Puppet.Service;

[PrefabAttribute(Name = "Prefabs/GameManager/HeaderMenu", Depth = 3, IsAttachedToCamera = true, IsUIPanel = true)]
public class HeaderMenuView : SingletonPrefab<HeaderMenuView>,IHeaderMenuView
{

	#region Unity Editor
	public UITable tableButton,tableLeft;
	public GameObject btnBack, btnCommon, btnRecharge, btnMessage,btnLeague , btnSettings, btnLobbyChangeTypeShow, btnSearch,btnUp,btnShowInfo;
	public GameObject formInfo;
	public GameObject logoInLobby;
	public UITexture avatar;
	public UILabel lbUserName,lbChip,lbLevel;
	public UISlider slideLevel;
	#endregion

	Action lobbyChangeTypeCallBack;
    Action onGameplayQuitCallback;
	public  HeaderMenuPresenter presenter;
	public void ShowInWorldGame(){
		btnCommon.SetActive (true);
		btnRecharge.SetActive (true);
		btnMessage.SetActive (true);
		btnSettings.SetActive (true);
		avatar.transform.parent.gameObject.SetActive (true);

		btnBack.SetActive (false);
		btnLobbyChangeTypeShow.SetActive (false);
		btnLeague.SetActive (false);
		btnSearch.SetActive (false);
		tableButton.Reposition ();
		slideLevel.transform.parent.parent.gameObject.SetActive (true);
		avatar.transform.parent.gameObject.SetActive (true);
		tableLeft.Reposition ();
	}

	public void ShowInPlaza(){
		btnCommon.SetActive (true);
		btnBack.SetActive (true);
		btnRecharge.SetActive (true);
		btnMessage.SetActive (true);
		btnSettings.SetActive (true);
		btnLeague.SetActive (true);
		btnLobbyChangeTypeShow.SetActive (false);
		btnSearch.SetActive (false);
		tableButton.Reposition ();
		tableLeft.Reposition ();
	}
	public void ShowInLobby(){
		btnLobbyChangeTypeShow.SetActive (true);
		btnSearch.SetActive (true);
		btnBack.SetActive (true);
		logoInLobby.SetActive (true);

		btnCommon.SetActive (false);
		btnRecharge.SetActive (false);
		btnMessage.SetActive (false);
		btnSettings.SetActive (false);
		btnLeague.SetActive (false);
		tableLeft.Reposition ();
		tableButton.Reposition ();
	}
    public void ShowInGameplay(Action onClickQuit, UIEventListener.VoidDelegate handleStandUp)
    {
		btnSearch.SetActive (true);
		btnRecharge.SetActive (true);
		btnBack.SetActive (true);
		btnUp.SetActive (true);

		formInfo.SetActive (false);
		btnLobbyChangeTypeShow.SetActive (false);
		btnCommon.SetActive (false);
		btnMessage.SetActive (false);
		btnSettings.SetActive (false);
		btnLeague.SetActive (false);
		tableLeft.Reposition ();
		tableButton.Reposition ();

        UIEventListener.Get(btnUp).onClick = handleStandUp;
        onGameplayQuitCallback = onClickQuit;
	}
	void Start(){
		presenter = new HeaderMenuPresenter (this);
		presenter.ViewStart ();
		UIPanel panel = gameObject.GetComponent<UIPanel> ();
		panel.SetAnchor( NGUITools.GetRoot (gameObject));
		presenter.OnShowLobbyRowTypeCallBack = OnLobbyShowTypeHandler;
        presenter.onHandleQuit = onGameplayQuitCallback;
	}
    
	public void SetChangeTypeLobbyCallBack(Action callback){
		lobbyChangeTypeCallBack = callback;
	}

    public void SetSearchSubmitCallBack(Action<string,bool[]> callback) {
        onSearchSubmitCallBack = callback;
    }
	void OnLobbyShowTypeHandler ()
	{
		if (lobbyChangeTypeCallBack != null)
			lobbyChangeTypeCallBack ();
	}

	void OnEnable(){
		UIEventListener.Get (btnBack).onClick += OnBackClickCallBack;
		UIEventListener.Get (btnCommon).onClick += OnCommonClickCallBack;
		UIEventListener.Get (btnRecharge).onClick += OnRechargeClickCallBack;
		UIEventListener.Get (btnMessage).onClick += OnMessageClickCallBack;
		UIEventListener.Get (btnSettings).onClick += OnSettingsClickCallBack;
		UIEventListener.Get (btnLobbyChangeTypeShow).onClick += OnLobbyChangeTypeClickCallBack;
		UIEventListener.Get (btnSearch).onClick += OnSearchClickCallBack;
		UIEventListener.Get (btnLeague).onClick += OnLeagueClickCallBack;
		UIEventListener.Get (avatar.gameObject).onClick += OnClickProfileCallBack;
		UIEventListener.Get (btnShowInfo).onClick += OnClickProfileCallBack;
	}
	void OnDisable(){
		UIEventListener.Get (btnBack).onClick -= OnBackClickCallBack;
		UIEventListener.Get (btnCommon).onClick -= OnCommonClickCallBack;
		UIEventListener.Get (btnRecharge).onClick -= OnRechargeClickCallBack;
		UIEventListener.Get (btnMessage).onClick -= OnMessageClickCallBack;
		UIEventListener.Get (btnSettings).onClick -= OnSettingsClickCallBack;
		UIEventListener.Get (btnLobbyChangeTypeShow).onClick -= OnLobbyChangeTypeClickCallBack;
		UIEventListener.Get (btnSearch).onClick -= OnSearchClickCallBack;
		UIEventListener.Get (btnLeague).onClick -= OnLeagueClickCallBack;
		UIEventListener.Get (avatar.gameObject).onClick += OnClickProfileCallBack;
		UIEventListener.Get (btnShowInfo).onClick -= OnClickProfileCallBack;
        presenter.ViewEnd();

	}
	#region IHeaderMenuView implementation
	public void ShowUserName (string username)
	{
		lbUserName.text = username;
	}

	public void ShowChip (string chip)
	{
		lbChip.text = chip;
	}

	public void ShowLevel (string level)
	{
		if (lbLevel.gameObject.activeSelf)
			lbLevel.text = level;
	}

	public void ShowExp (float currentExp)
	{
		if (slideLevel.gameObject.activeSelf)
			slideLevel.value = currentExp;
	}
	
	
	public void ShowError (string message)
	{
		throw new NotImplementedException ();
	}
	
	
	public void ShowConfirm (string message, Action<bool?> action)
	{
		throw new NotImplementedException ();
	}
	#endregion


	void OnBackClickCallBack (GameObject go)
	{
		presenter.OnBackPressed ();	
	}

	void OnCommonClickCallBack (GameObject go)
	{
		presenter.ShowDialogCommon ();
	}

	void OnRechargeClickCallBack (GameObject go)
	{
		DialogRecharge recharge = new DialogRecharge ();
		DialogService.Instance.ShowDialog (recharge);
	}

	void OnMessageClickCallBack (GameObject go)
	{
		presenter.ShowDialogMessage ();
	}

	void OnSettingsClickCallBack (GameObject go)
	{
		presenter.ShowDialogSettings ();
	}

	void OnLobbyChangeTypeClickCallBack (GameObject go)
	{
		if (go.transform.GetChild (0).GetComponent<UISprite> ().spriteName == "icon_menu") 
		{
			go.transform.GetChild (0).GetComponent<UISprite> ().spriteName = "icon_menu_type_2";
		}
		else {
			go.transform.GetChild (0).GetComponent<UISprite> ().spriteName = "icon_menu";
		}
		presenter.ShowLobbyType ();
	}

	void OnSearchClickCallBack (GameObject go)
	{
        SearchView.Instance.SetActionSubmit(onSearchSubmitCallBack);
	}

	void OnLeagueClickCallBack (GameObject go)
	{
	}

	void OnClickProfileCallBack (GameObject go)
	{
		presenter.ShowDialogProfile ();
	}

    public Action<string, bool[]> onSearchSubmitCallBack { get; set; }

}


