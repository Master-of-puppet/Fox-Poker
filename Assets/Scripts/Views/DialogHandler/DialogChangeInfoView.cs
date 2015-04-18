using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Core.Model;
using Puppet.API.Client;
using Puppet;
using System.Text.RegularExpressions;


[PrefabAttribute(Name = "Prefabs/Dialog/UserInfo/DialogChangeInfo", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogChangeInfoView : BaseDialog<DialogChangeInfo,DialogChangeInfoView> {
    #region UnityEditor
    public UIInput userName, fullName, email, phoneNumber, address;
    public GameObject btnSubmit, btnOpenGalery;
    public UIToggle toggleMale, toggleFemale;
    public GameObject[] btnDefaultAvatars;
    public UITexture avatar;
    #endregion
    private static string EMAIL_REGEX = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    bool isChangedAvatar = false;
	public override void ShowDialog (DialogChangeInfo data)
	{
		base.ShowDialog (data);
		initView ();
	}
	protected override void OnEnable ()
	{
		base.OnEnable ();
        UIEventListener.Get(fullName.GetComponentInChildren<UIAnchor>().GetComponentInChildren<UISprite>().gameObject).onClick = onClickEditFullName;
        UIEventListener.Get(email.GetComponentInChildren<UIAnchor>().GetComponentInChildren<UISprite>().gameObject).onClick = onClickEditEmail;
        UIEventListener.Get(phoneNumber.GetComponentInChildren<UIAnchor>().GetComponentInChildren<UISprite>().gameObject).onClick = onClickEditPhone;
        UIEventListener.Get(address.GetComponentInChildren<UIAnchor>().GetComponentInChildren<UISprite>().gameObject).onClick = onClickEditAddress;
        UIEventListener.Get(btnSubmit).onClick += onClickSubmit;
        UIEventListener.Get(btnOpenGalery).onClick += onClickOpenGallery;
        foreach (GameObject defaultAvatar in btnDefaultAvatars)
        {
            UIEventListener.Get(defaultAvatar).onClick += onClickToDefaultAvatar;
        } 
	
	}
	protected override void OnDisable ()
	{
		base.OnDisable ();
//		UIEventListener.Get (fullName.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite>().gameObject).onClick -= onClickEditFullName;
//		UIEventListener.Get (email.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick -= onClickEditEmail;
//		UIEventListener.Get (phoneNumber.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick -= onClickEditPhone;
//		UIEventListener.Get (address.GetComponentInChildren<UIAnchor> ().GetComponentInChildren<UISprite> ().gameObject).onClick -= onClickEditAddress;
        UIEventListener.Get(btnSubmit).onClick -= onClickSubmit;
        UIEventListener.Get(btnOpenGalery).onClick -= onClickOpenGallery;
        foreach (GameObject defaultAvatar in btnDefaultAvatars)
        {
            UIEventListener.Get(defaultAvatar).onClick -= onClickToDefaultAvatar;
        } 
	}

    private void onClickOpenGallery(GameObject go)
    {
        BrowseImageService.Instance.BrowseImageFromGallary((status, texture) =>
        {
            if (status) { 
                isChangedAvatar = true;
                avatar.mainTexture = texture;
            }
        });
    }
    void onClickToDefaultAvatar(GameObject go)
    {
        isChangedAvatar = true;
        avatar.mainTexture = go.GetComponentInChildren<UITexture>().mainTexture;
    }

	void onClickSubmit (GameObject go)
	{
		int gender = toggleMale.value == true ? 0 : 1;
		APIUser.ChangeUseInformation (fullName.value,"","",gender,address.value,"",OnSubmitChangeInfoCallBack);
		if (ValidateField ()) {
            APIUser.ChangeUseInformationSpecial(email.value, phoneNumber.value, null);

            if (isChangedAvatar)
                APIUser.ChangeUseInformation(((Texture2D)avatar.mainTexture).EncodeToPNG(), OnChangeAvatarCallBack);
		}
	}

    private void OnChangeAvatarCallBack(bool status, string message)
    {
        Logger.Log("========> " + status);
    }
	bool ValidateField(){

        if (!string.IsNullOrEmpty(email.value))
        {
            bool isEmail = Regex.IsMatch(email.value, EMAIL_REGEX, RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Email không đúng định dạng", null));
                return false;
            }
        }

        if (!string.IsNullOrEmpty(phoneNumber.value))
        {
            bool isPhoneNumber = (phoneNumber.value.Length == 10 || phoneNumber.value.Length == 11);
            if (!isPhoneNumber)
            {
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Phone không đúng định dạng", null));
                return false;
            }
        }
   		return true;

	}

	void OnSubmitChangeInfoCallBack (bool status, string message)
	{
		if (status) {
            initView();
			DialogService.Instance.ShowDialog(new DialogMessage("Thông báo",message,null));
		}

	}

	public void initView(){
		userName.value = data.info.info.userName;
		fullName.value = data.info.info.lastName + data.info.info.firstName;
        email.value = data.info.info.email;
        phoneNumber.value = data.info.info.mobile;
        if (data.info.info.gender == 0)
        {
            toggleMale.value = true;
        }
        else
        {
            toggleFemale.value = true;
        }
        address.value = data.info.info.address;
        PuApp.Instance.GetImage(data.info.info.avatar, (texture) => avatar.mainTexture = texture);
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
