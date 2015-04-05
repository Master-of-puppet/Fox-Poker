using UnityEngine;
using System.Collections;
using Puppet;
using Puppet.Service;

[PrefabAttribute(Name = "Prefabs/Dialog/DialogMission", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogMissionView : BaseDialog<DialogMission, DialogMissionView>
{

    
}
public class DialogMission : AbstractDialogData
{
 
    public DialogMission() : base()
    {

    }
    public override void ShowDialog()
    {
        DialogMissionView.Instance.ShowDialog(this);
    }
}
