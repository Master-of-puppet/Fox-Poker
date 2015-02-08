using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Poker.Models;
using Puppet;
using Puppet.Core.Model;
using Puppet.Core.Network.Http;


[PrefabAttribute(Name = "Prefabs/Dialog/Friend/DialogPlayerProfile", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogPlayerInfoView : BaseDialog<DialogPlayerInfo, DialogPlayerInfoView>
{
    #region Unity Editor
    public UILabel lbUserName, lbChip, lbLevel, lbPercenWin, lbTotalPlay, lbChipGift;
    public UITexture avatar;
    public UISlider sliderChipGift;
    public UIGrid gridIcon;
    public GameObject[] buttonInteractions;
    #endregion
    protected override void OnEnable()
    {
         base.OnEnable();
        foreach(GameObject btn in buttonInteractions)
        {
            UIEventListener.Get(btn).onClick += OnButtonInteractionClick;
        }
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        foreach (GameObject btn in buttonInteractions)
        {
            UIEventListener.Get(btn).onClick -= OnButtonInteractionClick;
        }
    }
    private void OnButtonInteractionClick(GameObject obj) {
        string nameInteraction = obj.name;
        Puppet.API.Client.APIGeneric.SendChat(new DataChat(nameInteraction, data.player.userName));
        OnClickButton(null);
    }
    public override void ShowDialog(DialogPlayerInfo data)
    {
        base.ShowDialog(data);
        initView();
    }
    public void initView() {
        lbUserName.text = data.player.userName;
        lbChip.text = data.player.asset.content[0].value.ToString();
		WWWRequest request = new WWWRequest (this, data.player.avatar, 5, 3);
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
public class DialogPlayerInfo : AbstractDialogData
{
    public PokerPlayerController player;
    public DialogPlayerInfo(PokerPlayerController player) {
        this.player = player;
    }
    public override void ShowDialog()
    {
        DialogPlayerInfoView.Instance.ShowDialog(this);
    }
}
