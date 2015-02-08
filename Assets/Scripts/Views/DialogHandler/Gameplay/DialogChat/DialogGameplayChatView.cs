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
    public GameObject[] btnChatTemplates;
    #endregion
    string placeHolder = "Nhập nội dung";
	public static string EMOTICON_CODE = "FPE";
	public static string EMOTICON_STICKER_CODE = "FPES";
	public static string EMOTICON_ANIMATION_CODE = "FPEA";
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnSend).onClick += OnClickButtonSend;
        foreach (GameObject btn in btnChatTemplates) {
            UIEventListener.Get(btn).onClick += OnClickChatTemplate ;
        }
        PuMain.Dispatcher.onChatMessage += ShowMessage;

    }
    protected override void OnDisable()
    {
        base.OnDisable();
        UIEventListener.Get(btnSend).onClick -= OnClickButtonSend;
        foreach (GameObject btn in btnChatTemplates)
        {
            UIEventListener.Get(btn).onClick -= OnClickChatTemplate;
        }
        PuMain.Dispatcher.onChatMessage -= ShowMessage;
    }
	
    private void OnClickChatTemplate(GameObject go)
    {
        string textTemplate = go.GetComponentInChildren<UILabel>().text;
        Puppet.API.Client.APIGeneric.SendChat(new DataChat(textTemplate, DataChat.ChatType.Public));
		//Call to hide me
		OnClickButton(null);
    }
    public override void ShowDialog(DialogGameplayChat data)
    {
        base.ShowDialog(data);
        initData();
    }
    private void ShowMessage(DataChat message)
    {
		if (message.Content.IndexOf (EMOTICON_CODE) != 0) {
			data.datas.Add (message);
			string msg = "[00ff00]" + message.Sender.userName + " : [-]" + message.Content;
			chatArea.Add (msg);
		}
    }
    private void initData()
    {
        txtMessage.value = placeHolder;
        for (int i = 0; i < data.datas.Count; i++)
        {
            string message = "[00ff00]" + data.datas[i].Sender.userName + " : [-]" + data.datas[i].Content;
            chatArea.Add(message);
        }
    }
    public void OnClickButtonSend(GameObject gobj) 
    {
        if (!string.IsNullOrEmpty(txtMessage.value) && txtMessage.value != placeHolder)
            Puppet.API.Client.APIGeneric.SendChat(new DataChat(txtMessage.value, DataChat.ChatType.Public));

        //Call to hide me
        OnClickButton(null);
    }
	public void OnEmoticonClick(GameObject gobj){

		string emoticonCode = gobj.name;
		Puppet.API.Client.APIGeneric.SendChat(new DataChat(emoticonCode, DataChat.ChatType.Public));
		//Call to hide me
		OnClickButton(null);		
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
