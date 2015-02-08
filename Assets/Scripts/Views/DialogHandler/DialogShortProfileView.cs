using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Core.Model;
using Puppet;
using Puppet.Core.Network.Http;

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
		lbUserName.text = data.info.info.userName;
		lbChip.text = data.info.assets.content [0].value.ToString() ;
		WWWRequest request = new WWWRequest (this, data.info.info.avatar, 5, 3);
		request.isFullUrl = true;
		request.onResponse = delegate(IHttpRequest arg1, IHttpResponse arg2) {
			WWWResponse response = (WWWResponse)arg2;
			if(response.www.texture !=null){
				avatar.mainTexture = response.www.texture;
			}
		};
		request.Start (null);
	}
}
public class DialogShortProfile : AbstractDialogData{
	#region implemented abstract members of AbstractDialogData
	public UserInfo info;
	public DialogShortProfile(UserInfo info){
		this.info = info;
	}
	public override void ShowDialog ()
	{
		DialogShortProfileView.Instance.ShowDialog (this);
	}

	#endregion


}
