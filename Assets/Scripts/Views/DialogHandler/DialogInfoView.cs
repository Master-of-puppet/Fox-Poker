
// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using Puppet.Service;
using Puppet;
using Puppet.Core.Model;
using UnityEngine;
using Puppet.Core.Network.Http;


[PrefabAttribute(Name = "Prefabs/Dialog/UserInfo/DialogInfo", Depth = 6, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogInfoView : BaseDialog<DialogInfo,DialogInfoView>
{
	#region UnityEditor
	public UILabel lbUserName,lbChip,lbLevel,lbExp,lbNumberWin,lbNumberLost,lbPrecentsWin, lbTotalPlay,lbStatictis;
	public UISlider sliderExp;
	public UITexture avatar;
	public GameObject btnViewStatictis,btnEditAvatar;
    #endregion
    UserInfo userInfo;
	protected override void OnEnable ()
	{
		base.OnEnable ();
		UIEventListener.Get (btnViewStatictis).onClick += onClickViewStatictis;
		UIEventListener.Get (btnEditAvatar).onClick += onClickBtnEdit;

        userInfo = Puppet.API.Client.APIUser.GetUserInformation();
        userInfo.onDataChanged += OnDataUserChange;
	}
	protected override void OnDisable ()
	{
		base.OnDisable ();
		UIEventListener.Get (btnViewStatictis).onClick -= onClickViewStatictis;
		UIEventListener.Get (btnEditAvatar).onClick -= onClickBtnEdit;

        userInfo.onDataChanged -= OnDataUserChange;
	}

	void onClickViewStatictis (GameObject go)
	{
		DialogService.Instance.ShowDialog (new DialogStatistic());
	}


	void onClickBtnEdit (GameObject go)
	{
		DialogService.Instance.ShowDialog (new DialogChangeInfo (data.info)); 
	}

	public override void ShowDialog (DialogInfo data)
	{
		base.ShowDialog (data);
		initData (data);
	}

	void initData (DialogInfo data)
	{
		lbUserName.text = data.info.info.userName;
		lbChip.text = data.info.assets.content [0].value.ToString ();
        PuApp.Instance.GetImage(data.info.info.avatar, (texture) => avatar.mainTexture = texture);
        if (data.info.info.numberTotalGames != 0)
            lbPrecentsWin.text = (data.info.info.numberWinGames / (float)data.info.info.numberTotalGames) + "%";
        else
            lbPrecentsWin.text = "0%";
        lbNumberWin.text = data.info.info.numberWinGames.ToString();
        lbNumberLost.text = data.info.info.numberLossGames.ToString();
        lbTotalPlay.text = data.info.info.numberTotalGames.ToString();
        //lbWinMax.text = data.info.info.numberWinGames.ToString() ;
	}

    void OnDataUserChange(IDataModel info)
    {
        UserInfo user = (UserInfo)info;
        lbChip.text = user.assets.GetAsset(EAssets.Chip).value.ToString();
        PuApp.Instance.GetImage(user.info.avatar, (texture) => avatar.mainTexture = texture);
    }
}

public class DialogInfo : AbstractDialogData{
	public UserInfo info;
	public DialogInfo(UserInfo info) : base(){
		this.info = info;
	}
	public override void ShowDialog ()
	{
		DialogInfoView.Instance.ShowDialog (this);
	}
}
