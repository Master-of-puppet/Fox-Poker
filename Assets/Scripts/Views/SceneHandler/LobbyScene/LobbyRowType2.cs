using UnityEngine;
using System.Collections;
using Puppet.Core.Model;

public class LobbyRowType2 : MonoBehaviour
{
    #region Unity Editor
    public UILabel lbRoomNumber, lbMoneyStep, lbMoneyMinMax, lbPeopleNumber;
    #endregion
    public DataLobby data;
    public static LobbyRowType2 Create(DataLobby data, UITable parent)
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Lobby/LobbyRowType2")) as GameObject;
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.GetComponent<UIDragScrollView>().scrollView = parent.GetComponentInParent<UIScrollView>();
        go.name = data.roomId + " - " + data.roomName;
        LobbyRowType2 item = go.GetComponent<LobbyRowType2>();
        item.setData(data);
        return item;
    }


    void Start () {
	
	}
    private void setData(DataLobby data)
    {
        this.data = data;
        lbRoomNumber.text = data.roomId.ToString();
		lbPeopleNumber.text = data.users.Length + "/" + data.gameDetails.numPlayers;
    }

    void OnClick()
    {
        GameObject.FindObjectOfType<LobbyScene>().JoinGame(this.data);
    }
}
