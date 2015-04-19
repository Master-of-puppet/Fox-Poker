using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet;
using Puppet.API.Client;

[PrefabAttribute(Name = "Prefabs/Dialog/Gameplay/DialogGameplayShare", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]

public class DialogGameplayShareView : BaseDialog<DialogGameplayShare, DialogGameplayShareView>
{

    #region UnityEditor
    public UILabel lbTitle, lbDescription;
    public UIInput txtMessage;
    public GameObject btnShare;
    #endregion
    public override void ShowDialog(DialogGameplayShare data)
    {
        base.ShowDialog(data);
        initView();
    }

    private void initView()
    {
        lbTitle.text = data.title;
        lbDescription.text = data.description;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnShare).onClick -= OnClickShareFacebook;

    }

    private void OnClickShareFacebook(GameObject go)
    {
        SocialService.GetAccessToken(SocialType.Facebook, (status, token) =>
        {
            if (status)
            {
                APIGeneric.PostFacebook(token, data.title, null, delegate(bool status1, string message)
                {
                    if (status == false)
                        DialogService.Instance.ShowDialog(new DialogMessage("Lỗi", "Gặp lỗi khi post facebook", null));
                    if (gameObject != null)
                        GameObject.Destroy(gameObject);

                });
            }
            else
            {
                DialogService.Instance.ShowDialog(new DialogMessage("Lỗi", "Không thể đăng nhập Facebook.", null));
                if (gameObject != null)
                    GameObject.Destroy(gameObject);
            }
        });
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnShare).onClick += OnClickShareFacebook;
    }
    protected override void OnPressButton(bool? pressValue, DialogGameplayShare data)
    {
      
        base.OnPressButton(pressValue, data);

       
    }
}
public class DialogGameplayShare : AbstractDialogData
{
    public string title;
    public string description;
    public string expandText;
    public DialogGameplayShare(string title, string description, string expandText)
    {
        this.title = title;
        this.description = description;
        this.expandText = expandText;
    }
    public override void ShowDialog()
    {
        DialogGameplayShareView.Instance.ShowDialog(this);
    }
}
