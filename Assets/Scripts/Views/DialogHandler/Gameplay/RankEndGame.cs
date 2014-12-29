using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet;
[PrefabAttribute(Name = "Prefabs/Gameplay/RankEndGame", Depth = 2, IsAttachedToCamera = true, IsUIPanel = true)]
public class RankEndGame : BaseDialog<RankEndGameModel, RankEndGame>
{
	public UILabel lbTitle;
    public override void ShowDialog(RankEndGameModel data)
    {
        base.ShowDialog(data);
		lbTitle.text = data.Title;
    }

    public void DestroyMe()
    {
        OnClickButton(null);
    }
}
public class RankEndGameModel : AbstractDialogData
{
    public override bool IsMessageDialog
    {
        get { return false; }
    }

    public RankEndGameModel(string title)
    {
        this.Title = title;
        this.Content = string.Empty;
    }

    public override void ShowDialog()
    {
		RankEndGame.Instance.ShowDialog(this);
    }

    public void DestroyUI()
    {
        RankEndGame.Instance.DestroyMe();
    }
}