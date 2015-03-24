using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Core.Model;
using Puppet.API.Client;
using Puppet;


[PrefabAttribute(Name = "Prefabs/Dialog/UserInfo/DialogChangeInfo", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogChangeInfoView : BaseDialog<DialogChangeInfo,DialogChangeInfoView> {
	#region UnityEditor
	public UIInput userName,fullName,email,phoneNumber,address;
	public GameObject btnSubmit;
	public UIToggle toggleMale, toggleFemale;
	#endregion
	public override void ShowDialog (DialogChangeInfo data)
	{
		base.ShowDialog (data);
		initView ();
	}
	protected override void OnEnable ()
	{
		base.OnEnable ();
		UIEventListener.Get (fullName.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite>().gameObject).onClick = onClickEditFullName;
		UIEventListener.Get (email.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick = onClickEditEmail;
		UIEventListener.Get (phoneNumber.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick = onClickEditPhone;
		UIEventListener.Get (address.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick = onClickEditAddress;
		UIEventListener.Get (btnSubmit).onClick += onClickSubmit;
	
	}
	protected override void OnDisable ()
	{
		base.OnDisable ();
//		UIEventListener.Get (fullName.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite>().gameObject).onClick -= onClickEditFullName;
//		UIEventListener.Get (email.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick -= onClickEditEmail;
//		UIEventListener.Get (phoneNumber.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick -= onClickEditPhone;
//		UIEventListener.Get (address.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick -= onClickEditAddress;
		UIEventListener.Get (btnSubmit).onClick -= onClickSubmit;
	}

	void onClickSubmit (GameObject go)
	{
		APIUser.ChangeUseInformationSpecial (email.value, phoneNumber.value, OnSubmitChangeInfoCallBack);
	}

	void OnSubmitChangeInfoCallBack (bool status, string message)
	{
		if (status) {
			DialogService.Instance.ShowDialog(new DialogMessage("Thông báo",message,null));
		}

	}

	public void initView(){
		userName.value = data.info.info.userName;
        fullName.value = data.info.info.lastName + data.info.info.firstName;
		//email.value = data.info.info.;
		//fullName.value = data.info.info.lastName + data.info.info.firstName;
	}

	void onClickEditFullName (GameObject go)
	{

		fullName.collider.enabled = true;
		fullName.isSelected = true;
		fullName.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject.SetActive (false);
	}

	void onClickEditEmail (GameObject go)
	{
		email.collider.enabled = true;
		email.isSelected = true;
		email.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject.SetActive (false);
	}

	void onClickEditPhone (GameObject go)
	{
		phoneNumber.collider.enabled = true;
		phoneNumber.isSelected = true;
		phoneNumber.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject.SetActive (false);
	}

	void onClickEditAddress (GameObject go)
	{
		address.collider.enabled = true;
		address.isSelected = true;
		address.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject.SetActive (false);
	}
}
public class DialogChangeInfo : AbstractDialogData{
	public UserInfo info;
	public DialogChangeInfo(UserInfo info) : base(){
		this.info = info;

	}
	public override void ShowDialog ()
	{
		DialogChangeInfoView.Instance.ShowDialog (this);
	}
}
