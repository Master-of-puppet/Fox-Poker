using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Core.Model;
using Puppet.API.Client;
using Puppet;
using System.Text.RegularExpressions;


[PrefabAttribute(Name = "Prefabs/Dialog/UserInfo/DialogChangeInfo", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogChangeInfoView : BaseDialog<DialogChangeInfo,DialogChangeInfoView> 
{
    #region UnityEditor
    public UIInput userName, fullName, email, phoneNumber, address;
    public GameObject btnSubmit, btnOpenGalery;
    public UIToggle toggleMale, toggleFemale; 
    public UITexture avatar;

    public GameObject prefabAvatarChangeInfo;
    public UITable avatarTableView;
    public UIScrollView avatarScrollView;
    #endregion

    private static string EMAIL_REGEX = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
    bool isChangedAvatar = false;
    string defaultAvatar = string.Empty;

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
	}

	protected override void OnDisable ()
	{
		base.OnDisable ();
        UIEventListener.Get(btnSubmit).onClick -= onClickSubmit;
        UIEventListener.Get(btnOpenGalery).onClick -= onClickOpenGallery;
	}

    private void onClickOpenGallery(GameObject go)
    {
        BrowseImageService.Instance.BrowseImageFromGallary((status, texture) =>
        {
            if (status) 
            {
                defaultAvatar = string.Empty;
                isChangedAvatar = true;
                avatar.mainTexture = texture;
            }
        });
    }
    void onClickToDefaultAvatar(GameObject go)
    {
        isChangedAvatar = true;
        defaultAvatar = go.GetComponent<ComponentData>().dataString;
        avatar.mainTexture = go.GetComponentInChildren<UITexture>().mainTexture;
    }

    int totalChange;
	void onClickSubmit (GameObject go)
	{
        totalChange = 0;
        if (ValidateField())
        {
		    int gender = toggleMale.value == true ? 0 : 1;
            DataUser userInfo = data.info.info;

            string emailChange = email.value.Trim();
            string phoneChange = phoneNumber.value.Trim();
            string fullNameChange = fullName.value.Trim();
            string addressChange = address.value.Trim();

            if (!fullNameChange.Equals(userInfo.firstName) || userInfo.gender != gender || !address.value.Trim().Equals(userInfo.address))
            {
                totalChange++;
                APIUser.ChangeUseInformation(fullNameChange, "", "", gender, addressChange, "", OnSubmitChangeInfoCallBack);
            }

            if ((!string.IsNullOrEmpty(emailChange) && !emailChange.Equals(userInfo.email))
                || (!string.IsNullOrEmpty(phoneChange) && !phoneChange.Equals(userInfo.mobile)))
            {
                totalChange++;
                APIUser.ChangeUseInformationSpecial(emailChange, phoneChange, OnSubmitChangeInfoCallBack);
            }

            if (isChangedAvatar)
            {
                totalChange++;

                if (string.IsNullOrEmpty(defaultAvatar))
                {
                    HttpAPI.ChangeUseAvatar(this, ((Texture2D)avatar.mainTexture).EncodeToPNG(), OnSubmitChangeInfoCallBack);
                }
                else
                    APIGeneric.SaveDefaultAvatar(defaultAvatar, OnSubmitChangeInfoCallBack);
            }

            if (totalChange == 0)
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Không có thông tin gì thay đổi."));
		}
	}

	bool ValidateField()
    {
        if (!string.IsNullOrEmpty(email.value))
        {
            bool isEmail = Regex.IsMatch(email.value, EMAIL_REGEX, RegexOptions.IgnoreCase);
            if (!isEmail)
            {
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Email không đúng định dạng"));
                return false;
            }
        }

        if (!string.IsNullOrEmpty(phoneNumber.value))
        {
            bool isPhoneNumber = (phoneNumber.value.Length == 10 || phoneNumber.value.Length == 11);
            if (!isPhoneNumber)
            {
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Phone không đúng định dạng"));
                return false;
            }
        }

   		return true;
	}

	void OnSubmitChangeInfoCallBack (bool status, string message)
    {
        if (!status) 
        {
			DialogService.Instance.ShowDialog(new DialogMessage("Thay đổi thông tin không thành công.",message));
		}
        else
        {
            totalChange--;
            if (totalChange == 0)
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Thay đổi thông tin cá nhân thành công."));
        }
	}

	public void initView()
    {
		userName.value = data.info.info.userName.Trim();
        fullName.value = string.Format("{0} {1} {2}", 
            data.info.info.firstName != null ? data.info.info.firstName.Trim() : string.Empty,
            data.info.info.middleName != null ? data.info.info.middleName.Trim() : string.Empty,
            data.info.info.lastName != null ? data.info.info.lastName.Trim() : string.Empty
        ).Trim();
        email.value = data.info.info.email != null ? data.info.info.email.Trim() : string.Empty;
        phoneNumber.value = data.info.info.mobile != null ? data.info.info.mobile.Trim() : string.Empty;
        toggleMale.value = data.info.info.gender != null ? data.info.info.gender == 0 : true;
        toggleFemale.value = data.info.info.gender != null ? data.info.info.gender == 1 : false;
        address.value = data.info.info.address != null ? data.info.info.address.Trim() : string.Empty;
        PuApp.Instance.GetImage(data.info.info.avatar, (texture) => avatar.mainTexture = texture);

        APIGeneric.GetDefaultAvatar((status, message, listAvatar) =>
        {
            if(status && listAvatar.Count > 0)
            {
                foreach(string avatarPath in listAvatar)
                {
                    GameObject obj = (GameObject)GameObject.Instantiate(prefabAvatarChangeInfo);
                    obj.transform.parent = avatarTableView.transform;
                    obj.transform.localScale = Vector3.one;
                    obj.transform.localPosition = Vector3.zero;
                    avatarTableView.Reposition();
                    obj.GetComponent<UIDragScrollView>().scrollView = avatarScrollView;
                    obj.AddComponent<ComponentData>().dataString = avatarPath;

                    PuApp.Instance.GetImage(AppConfig.HttpUrl + "/static/" + avatarPath, (img) =>
                    {
                        obj.GetComponentInChildren<UITexture>().mainTexture = img;
                    });

                    UIEventListener.Get(obj).onClick += onClickToDefaultAvatar;
                }
            }
            else if(!status)
            {
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", "Không thể lấy thông tin các biểu tượng mặc định."));
            }
        });
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

public class DialogChangeInfo : AbstractDialogData
{
	public UserInfo info;
	public DialogChangeInfo(UserInfo info) : base(){
		this.info = info;

	}
	public override void ShowDialog ()
	{
		DialogChangeInfoView.Instance.ShowDialog (this);
	}
}
