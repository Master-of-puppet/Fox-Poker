using UnityEngine;
using System.Collections;
using Puppet;
using Puppet.Service;

[PrefabAttribute(Name = "Prefabs/Dialog/UserInfo/DialogListGift", Depth = 9, IsAttachedToCamera = true, IsUIPanel = true)]

public class DialogListGiftView : BaseDialog<DialogListGift, DialogListGiftView>
{

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
public class DialogListGift : AbstractDialogData
{

    public DialogListGift()
    {
       
    }
    public override void ShowDialog()
    {
        DialogListGiftView.Instance.ShowDialog(this);
    }
}
