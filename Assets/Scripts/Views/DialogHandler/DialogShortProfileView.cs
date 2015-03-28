using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Core.Model;
using Puppet;
using Puppet.Core.Network.Http;
using Puppet.Poker.Models;

[PrefabAttribute(Name = "Prefabs/Dialog/UserInfo/DialogShortProfile", Depth = 10, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogShortProfileView : BaseDialog<DialogShortProfile, DialogShortProfileView> {
	#region UnityEditor
	public UILabel lbUserName,lbChip,lbLevel;
	public UITexture avatar;
	#endregion
	protected override void OnEnable ()
	{
		base.OnEnable ();
	}
	protected override void OnDisable ()
	{
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
