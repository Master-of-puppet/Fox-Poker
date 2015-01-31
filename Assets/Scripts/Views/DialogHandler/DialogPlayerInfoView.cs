using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.Poker.Models;
using Puppet;
[PrefabAttribute(Name = "Prefabs/Dialog/Friend/DialogPlayerProfile", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogPlayerInfoView : BaseDialog<DialogPlayerInfo, DialogPlayerInfoView>
{
    #region Unity Editor
    public UILabel lbUserName, lbChip, lbLevel, lbPercenWin, lbTotalPlay, lbChipGift;
    public UITexture avatar;
    public UISlider sliderChipGift;
    public UIGrid gridIcon;
    #endregion
    public override void ShowDialog(DialogPlayerInfo data)
    {
        base.ShowDialog(data);
        initView();
    }
    public void initView() {
        lbUserName.text = data.player.userName;
        lbChip.text = data.player.asset.content[0].value.ToString();
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
