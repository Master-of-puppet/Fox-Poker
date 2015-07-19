using UnityEngine;
using System.Collections;
using Puppet.Core.Model;
using Puppet;

public class LobbyRowType2 : MonoBehaviour
{
    #region Unity Editor
    public GameObject lbRoomNumber, lbMoneyStep, lbMoneyMinMax, lbPeopleNumber;
    public UISprite divider;
    #endregion
    public DataLobby data;
    public static LobbyRowType2 Create(DataLobby data, UITable parent)
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Lobby/LobbyRowType2")) as GameObject;
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.name = data.roomId + " - " + data.roomName;
        LobbyRowType2 item = go.GetComponent<LobbyRowType2>();
        item.SetData(data);
        return item;
    }


    public void SetData(DataLobby data)
    {
        divider.leftAnchor.Set(gameObject.transform.parent.parent, 0, 0);
        divider.rightAnchor.Set(gameObject.transform.parent.parent, 1, 0);
        this.data = data;
        lbRoomNumber.GetComponent<UILabel>().text = data.roomId.ToString();
        double smallBind = data.gameDetails.betting / 2;
        double minBind = smallBind * 20;
        double maxBind = smallBind * 400;
        lbMoneyStep.GetComponent<UILabel>().text = "$" + smallBind + "/" + data.gameDetails.betting;
        lbMoneyMinMax.GetComponent<UILabel>().text = "$" + Utility.Convert.ConvertShortcutMoney(minBind) + "/" + Utility.Convert.ConvertShortcutMoney(maxBind);
		string numberUser = "0/" +  data.gameDetails.numPlayers;
		if(data.users != null){
			numberUser = data.users.Length + "/" + data.gameDetails.numPlayers;
		}
        lbPeopleNumber.GetComponent<UILabel>().text = numberUser;
    }

    void OnClick()
    {
        GameObject.FindObjectOfType<LobbyScene>().JoinGame(this.data);
    }
}
