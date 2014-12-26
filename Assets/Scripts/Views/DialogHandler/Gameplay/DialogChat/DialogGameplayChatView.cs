using UnityEngine;
using System.Collections;
using Puppet.Service;
using System.Collections.Generic;
using Puppet.Core.Model;
using Puppet;


[PrefabAttribute(Name = "Prefabs/Gameplay/ChatDialog", Depth = 4, IsUIPanel = true , IsAttachedToCamera = true)]
public class DialogGameplayChatView : BaseDialog<DialogGameplayChat,DialogGameplayChatView>
{

    #region UNITY EDITOR
    public UIInput txtMessage;
    public UITextList chatArea;
    public GameObject btnSend;
    #endregion
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnSend).onClick += OnClickButtonSend;
        PuMain.Dispatcher.onChatMessage += ShowMessage;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnSend).onClick -= OnClickButtonSend;
        PuMain.Dispatcher.onChatMessage -= ShowMessage;
    }
    public override void ShowDialog(DialogGameplayChat data)
    {
        base.ShowDialog(data);
        initData();
    }
    private void ShowMessage(DataChat message)
    {
        data.datas.Add(message);
        string msg = "[00ff00]" + message.Sender.userName + " : [-]" + message.Content;
        chatArea.Add(msg);
    }
    private void initData()
    {
        for (int i = 0; i < data.datas.Count; i++)
        {
            string message = "[00ff00]" + data.datas[i].Sender.userName + " : [-]" + data.datas[i].Content;
            chatArea.Add(message);
        }
    }
    public void OnClickButtonSend(GameObject gobj) {
        if (!string.IsNullOrEmpty(txtMessage.value)) {
            Puppet.API.Client.APIGeneric.SendChat(new DataChat(txtMessage.value, DataChat.ChatType.Public));
        }
    }

}
public class DialogGameplayChat : AbstractDialogData {

    public DialogGameplayChat(List<DataChat> datas) : base(){
        this.datas = datas;
    }
    public override void ShowDialog()
    {
        DialogGameplayChatView.Instance.ShowDialog(this);
    }

    public List<DataChat> datas { get; set; }
}
