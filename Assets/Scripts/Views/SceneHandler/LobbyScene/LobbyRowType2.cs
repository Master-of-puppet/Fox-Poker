using UnityEngine;
using System.Collections;
using Puppet.Core.Model;
using Puppet;

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
        go.name = data.roomId + " - " + data.roomName;
        LobbyRowType2 item = go.GetComponent<LobbyRowType2>();
        item.StartCoroutine(item.setData(data));
        return item;
    }


    void Start () {
	
	}
    public IEnumerator setData(DataLobby data)
    {
        
        yield return new WaitForEndOfFrame();
        this.data = data;
        lbRoomNumber.text = data.roomId.ToString();
        double smallBind = data.gameDetails.betting / 2;
        double minBind = smallBind * 20;
        double maxBind = smallBind * 400;
        lbMoneyStep.text = "$" + smallBind + "/" + data.gameDetails.betting;
        lbMoneyMinMax.text = "$" + Utility.Convert.ConvertShortcutMoney(minBind) + "/" + Utility.Convert.ConvertShortcutMoney(maxBind); 
		lbPeopleNumber.text = data.users.Length + "/" + data.gameDetails.numPlayers;
    }

    void OnClick()
    {
        GameObject.FindObjectOfType<LobbyScene>().JoinGame(this.data);
    }
}
