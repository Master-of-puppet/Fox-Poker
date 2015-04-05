using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Core.Model;
using Puppet;
using Puppet.Core.Network.Http;
using Puppet.Poker.Models;

[PrefabAttribute(Name = "Prefabs/Dialog/UserInfo/DialogShortProfile", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogShortProfileView : BaseDialog<DialogShortProfile, DialogShortProfileView> {
	#region UnityEditor
	public UILabel lbUserName,lbChip,lbLevel;
	public UITexture avatar;
    public GameObject btnGift, btnItem;
	#endregion
	protected override void OnEnable ()
	{
        UIEventListener.Get(btnGift).onClick += onClickButtonGift;
		base.OnEnable ();
	}

    private void onClickButtonGift(GameObject go)
    {
        DialogService.Instance.ShowDialog(new DialogListGift());
    }
	protected override void OnDisable ()
	{
        UIEventListener.Get(btnGift).onClick -= onClickButtonGift;
		base.OnDisable ();
	}
	public override void ShowDialog (DialogShortProfile data)
	{
		base.ShowDialog (data);
		lbUserName.text = data.info.userName;
		lbChip.text = data.info.GetAvailableChip ().ToString();
        PuApp.Instance.GetImage(data.info.avatar, (texture) => avatar.mainTexture = texture);
	}
}
public class DialogShortProfile : AbstractDialogData{
	#region implemented abstract members of AbstractDialogData
	public PokerPlayerController info;
	public DialogShortProfile(PokerPlayerController info){
		this.info = info;
	}
	public override void ShowDialog ()
	{
		DialogShortProfileView.Instance.ShowDialog (this);
	}

	#endregion


}
