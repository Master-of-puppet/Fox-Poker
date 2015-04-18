using UnityEngine;
using System.Collections;
using Puppet.Core.Model;
using Puppet.API.Client;

public class LobbySlotUserInfo : MonoBehaviour {

	#region Unity Editor
	public UITexture avatar;
	public UILabel userName;
	#endregion
	public void showUserInfo(DataPlayerController user){
		userName.text = user.userName;
        PuApp.Instance.GetImage(user.avatar, (texture) => avatar.mainTexture = texture);
	}

}
