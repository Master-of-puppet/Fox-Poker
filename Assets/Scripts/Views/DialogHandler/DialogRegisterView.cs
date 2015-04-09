using UnityEngine;
using System.Collections;
using Puppet.API.Client;
using Puppet;
using System;
using Puppet.Service;
[PrefabAttribute(Name = "Prefabs/Dialog/DialogRegister", Depth = 9, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogRegisterView : BaseDialog<DialogRegister,DialogRegisterView>
{

    #region UnityEditor
    public UIInput userName, password, rePassword;
	public UISprite backgroundTransparent;
    Action<bool?, string, string> OnRegisterComplete;
    #endregion

    void Start () 
    {
		UIPanel root = NGUITools.GetRoot(gameObject).GetComponent<UIPanel>();
		backgroundTransparent.SetAnchor(root.gameObject, 0, 0, 0, 0);
	}
	public override void ShowDialog (DialogRegister data)
	{
		base.ShowDialog (data);
		this.OnRegisterComplete = data.OnRegisterComplete;
		if (!string.IsNullOrEmpty (data.suggestUser)) {
			userName.value = data.suggestUser;
		}
	}
	protected override void OnPressButton (bool? pressValue, DialogRegister data)
	{
		if (pressValue == true) {
			string name = userName.value;
			string pass = password.value;
			string rePass = rePassword.value;
			if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(rePass)){
				PuMain.Setting.Threading.QueueOnMainThread(() =>
				                                           {
					DialogService.Instance.ShowDialog(new DialogMessage("Lỗi", "Không được để trống các trường", null));
				});
				return;
			}
				if (pass.Equals(rePass))
				{
                    LoadingView.Instance.Show(false);
					APILogin.QuickRegister(name, pass, QuickRegisterCallBack);
				}
				else
				{
				DialogService.Instance.ShowDialog(new DialogMessage("Lỗi", "Mật khẩu không giống nhau", null));
				}
		}
		base.OnPressButton (pressValue, data);
	}

    private void QuickRegisterCallBack(bool status, string message)
    {
        if (status)
        {
            PuMain.Setting.Threading.QueueOnMainThread(() =>
            {
                if (OnRegisterComplete != null)
                    OnRegisterComplete(status, userName.value, password.value);
            });
        }
        else
        {
            PuMain.Setting.Threading.QueueOnMainThread(() =>
            {
                DialogService.Instance.ShowDialog(new DialogMessage("Lỗi", message, null));
            });
        }
    }



    private void OnClickClose(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }

}

public class DialogRegister : AbstractDialogData
{
    public string suggestUser;
    public Action<bool?, string, string> OnRegisterComplete;
    public DialogRegister(string suggestUser, Action<bool?, string, string> OnRegisterComplete)
        : base()
    {
        this.suggestUser = suggestUser;
        this.OnRegisterComplete = OnRegisterComplete;
    }
    public DialogRegister(Action<bool?, string, string> OnRegisterComplete)
        : base()
    {
        this.OnRegisterComplete = OnRegisterComplete;
    }

    public override void ShowDialog()
    {
        DialogRegisterView.Instance.ShowDialog(this);
    }
}