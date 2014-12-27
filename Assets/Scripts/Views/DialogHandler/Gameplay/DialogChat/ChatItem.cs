using UnityEngine;
using System.Collections;
using Puppet.Core.Model;

public class ChatItem : MonoBehaviour {

    public UILabel lbMessage;
    public static ChatItem Create(DataChat data) {
        GameObject gobj = GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/ChatItem")) as GameObject;
        ChatItem item = gobj.GetComponent<ChatItem>();
        item.initData(data);
        return item;
    }

    private void initData(DataChat data)
    {
        string message = "[00ff00]" +data.Sender.userName + " : [-]" + data.Content;
        lbMessage.text = message;
    }
}
