using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet;
using Puppet.API.Client;

[PrefabAttribute(Name = "Prefabs/Dialog/Gameplay/DialogGameplayShare", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]

public class DialogGameplayShareView : BaseDialog<DialogGameplayShare, DialogGameplayShareView>
{

    #region UnityEditor
    public UILabel lbTitle,lbDescription;
    public UIInput txtMessage;
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

    }
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnPressButton(bool? pressValue, DialogGameplayShare data)
    {
        if (pressValue == true)
        {

        }
        base.OnPressButton(pressValue, data);
       
    }
}
public class DialogGameplayShare : AbstractDialogData
{
    public string title;
    public string description;
    public string expandText;
    public DialogGameplayShare(string title,string description,string expandText) {
        this.title = title;
        this.description = description;
        this.expandText = expandText;
    }
    public override void ShowDialog()
    {
        DialogGameplayShareView.Instance.ShowDialog(this);
    }
}
