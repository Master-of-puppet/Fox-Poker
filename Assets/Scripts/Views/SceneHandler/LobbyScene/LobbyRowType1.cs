using UnityEngine;
using System.Collections;
using Puppet.Core.Model;
using Puppet;
using System;

public class LobbyRowType1 : MonoBehaviour
{
    #region Unity Editor
    public GameObject[] slots;
    public UILabel title;
    #endregion
    public DataLobby data;

    private Action<DataLobby> action;
    int[] arrayIndex5Player = { 0, 2, 4, 5, 7 };
    public static LobbyRowType1 Create(DataLobby data, UITable parent, Action<DataLobby> callBack)
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Lobby/LobbyRowType1")) as GameObject;
        go.transform.parent = parent.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.GetComponent<UIDragScrollView>().scrollView = parent.GetComponentInParent<UIScrollView>();
        go.name = data.roomId + " - " + data.roomName;
        LobbyRowType1 item = go.GetComponent<LobbyRowType1>();
        item.setData(data);
        item.action = callBack;
        return item;
    }
    public void setData(DataLobby lobby)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].SetActive(false);
        this.data = lobby;
        double smallBind = lobby.gameDetails.betting / 2;
        title.text = "Phòng : " + lobby.roomId + " - $" + smallBind + "/" + lobby.gameDetails.betting;

        if (data.users != null && data.users.Length > 0)
        {
            foreach (DataPlayerController item in data.users)
            {
                int index = item.slotIndex;
                if (data.gameDetails.numPlayers == 5)
                    index = arrayIndex5Player[item.slotIndex];

                slots[index].SetActive(true);
                slots[index].GetComponent<LobbySlot>().setData(item);
            }
        }
    }
    void Start()
    {
        gameObject.GetComponent<UIEventListener>().onClick += onTableClick;
    }

    void OnDestroy()
    {
        gameObject.GetComponent<UIEventListener>().onClick -= onTableClick;
    }
    private void onTableClick(GameObject go)
    {
        if (action != null)
            action(data);
    }

    void Update()
    {
        if (transform.parent.GetComponent<UICenterOnChild>().centeredObject == null)
            return;
        if (gameObject.transform.GetComponentInParent<UIScrollView>().isDragging || gameObject != gameObject.GetComponentInParent<UICenterOnChild>().centeredObject)
        {
            gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
            gameObject.GetComponent<UISprite>().color = new Color(69f / 255f, 69f / 255f, 69f / 255f);
            gameObject.collider.enabled = false;
            return;
        }
        if (gameObject == gameObject.GetComponentInParent<UICenterOnChild>().centeredObject)
        {
            gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            gameObject.GetComponent<UISprite>().color = new Color(1f, 1f, 1f);
            gameObject.collider.enabled = true;
        }

       
    }


}

