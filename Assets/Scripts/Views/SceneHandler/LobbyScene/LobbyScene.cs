using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet;
using Puppet.Core.Model;
using Puppet.API.Client;
using Puppet.Service;

public class LobbyScene : MonoBehaviour, ILobbyView
{
    public GameObject btnCreateGame, btnHelp, btnPlayNow;
    public UITable tableType1, tableType2, tableTab;
    bool isShowType1 = true;

    List<LobbyRowType1> types1 = new List<LobbyRowType1>();
    List<LobbyRowType2> types2 = new List<LobbyRowType2>();
    List<LobbyTab> tabs = new List<LobbyTab>();
    void Start()
    {
        HeaderMenuView.Instance.ShowInLobby();
        HeaderMenuView.Instance.SetSearchSubmitCallBack(OnSearchLobbyHandler);
        presenter = new PokerLobbyPresenter(this);
        HeaderMenuView.Instance.SetChangeTypeLobbyCallBack(delegate()
        {
            if (isShowType1)
            {
                isShowType1 = false;
                //          go.transform.GetChild(0).GetComponent<UISprite>().spriteName = "icon_menu_type_2";
                tableType1.transform.parent.parent.gameObject.SetActive(false);
                tableType2.transform.parent.parent.gameObject.SetActive(true);
                StartCoroutine(initShowRowType2(presenter.Lobbies));
            }
            else
            {
                isShowType1 = true;
                //            go.transform.GetChild(0).GetComponent<UISprite>().spriteName = "icon_menu";
                tableType1.transform.parent.parent.gameObject.SetActive(true);
                tableType2.transform.parent.parent.gameObject.SetActive(false);
                StartCoroutine(initShowRowType1(presenter.Lobbies));
            }
        });
    }

    private void OnSearchLobbyHandler(string arg1, bool[] arg2)
    {
        presenter.SearchLobby(arg1, arg2);
    }
    void FixedUpdate()
    {
        if (types1.Count == 0)
        {
            btnCreateGame.SetActive(true);
            return;
        }
        btnCreateGame.SetActive((tableType1.GetComponent<UICenterOnChild>().centeredObject != null && tableType1.transform.GetChild(0).gameObject.GetComponent<LobbyRowType1>().data.roomId == tableType1.GetComponent<UICenterOnChild>().centeredObject.GetComponent<LobbyRowType1>().data.roomId));

    }
    void OnEnable()
    {
        UIEventListener.Get(btnPlayNow).onClick += OnClickPlayNow;
        UIEventListener.Get(btnCreateGame).onClick += OnClickCreateGame;
        if (btnHelp != null)
            UIEventListener.Get(btnHelp).onClick += OnClickHelp;
        tableType1.GetComponent<UICenterOnChild>().onFinished += OnDragFinish;
    }

    void OnClickHelp(GameObject go)
    {
        DialogService.Instance.ShowDialog(new DialogHelp());
    }

    private void OnClickCreateGame(GameObject go)
    {
        presenter.CreateLobby();
    }

    private void OnClickPlayNow(GameObject go)
    {
        APILobby.QuickJoinLobby((status, message, data) =>
        {
            int roomId = System.Convert.ToInt32(data);
            if (roomId == -1)
                APILobby.CreateLobby(presenter.SelectedChannel.configuration.betting[0], 9, null);
            else if (roomId == -2)
                DialogService.Instance.ShowDialog(new DialogMessage("Message", message, null));
        });
    }

    private void OnCreateLobbyHandler(bool status, string message)
    {
        if (!status)
            Logger.LogError(message);
    }


    void OnDisable()
    {
        UIEventListener.Get(btnPlayNow).onClick -= OnClickPlayNow;
        UIEventListener.Get(btnCreateGame).onClick -= OnClickCreateGame;
        if (btnHelp != null)
            UIEventListener.Get(btnHelp).onClick -= OnClickHelp;
        presenter.ViewEnd();
    }

    private void ClearAllRow()
    {
        while (types1.Count > 0)
        {
            GameObject.Destroy(types1[0].gameObject);
            types1.RemoveAt(0);
        }
        while (types2.Count > 0)
        {
            GameObject.Destroy(types2[0].gameObject);
            types2.RemoveAt(0);
        }
    }
    public static Vector3 VectorItemCenter = Vector3.one;

    private void OnDragFinish()
    {
        if (tableType1.GetComponent<UICenterOnChild>().centeredObject == null)
        {
            tableType1.GetComponent<UICenterOnChild>().Recenter();
        }
        else
        {
            VectorItemCenter = tableType1.GetComponent<UICenterOnChild>().centeredObject.transform.position;
        }
    }
    private IEnumerator _AddRowType1(DataLobby lobby)
    {
        yield return new WaitForEndOfFrame();
        types1.Add(LobbyRowType1.Create(lobby, tableType1, JoinGame));
        yield return new WaitForSeconds(0.1f);
        tableType1.repositionNow = true;
        yield return new WaitForSeconds(0.2f);
        tableType1.GetComponent<UICenterOnChild>().Recenter();
        //tableType1.GetComponent<UICenterOnChild>().onFinished = OnDragFinish;
        //OnDragFinish();
    }
    private IEnumerator _AddRowType2(DataLobby lobby)
    {
        yield return new WaitForEndOfFrame();
        types2.Add(LobbyRowType2.Create(lobby, tableType2));
        tableType2.repositionNow = true;
    }
    IEnumerator initShowRowType1(List<DataLobby> lobbies)
    {
        ClearAllRow();
        yield return new WaitForEndOfFrame();
        foreach (DataLobby item in lobbies)
        {
            types1.Add(LobbyRowType1.Create(item, tableType1, JoinGame));
        }
        tableType1.repositionNow = true;
        yield return new WaitForSeconds(0.05f);
        tableType1.GetComponent<UICenterOnChild>().Recenter();
        yield return new WaitForSeconds(0.1f);

    }

    IEnumerator initShowRowType2(List<DataLobby> lobbies)
    {
        ClearAllRow();
        yield return new WaitForEndOfFrame();
        foreach (DataLobby item in lobbies)
        {
            types2.Add(LobbyRowType2.Create(item, tableType2));
        }
        tableType2.repositionNow = true;
    }

    public void ShowLobbyByType()
    {


    }

    public void DrawChannels(List<DataChannel> channels)
    {
        for (int i = 0; i < channels.Count; i++)
        {
            LobbyTab tab = LobbyTab.Create(channels[i], tableTab, i);
            tab.SetEventChoiceTab(delegate(DataChannel channel)
            {
                presenter.LoadLobbiesByChannel(channel);
            });
            tabs.Add(tab);
        }
        tableTab.Reposition();
        Vector3 currentPosition = tableTab.transform.localPosition;
        tableTab.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y - 2, currentPosition.z);
        tableTab.transform.parent.GetComponent<UIScrollView>().ResetPosition();
    }
    public void DrawLobbies(List<DataLobby> lobbies)
    {
        tabs.Find(s => s.data.name == presenter.SelectedChannel.name).GetComponent<UIToggle>().value = true;
        if (isShowType1)
        {
            StartCoroutine(initShowRowType1(lobbies));
        }
        else
        {
            StartCoroutine(initShowRowType2(lobbies));
        }
    }

    public void RemoveLobby(DataLobby lobby)
    {

        if (isShowType1)
        {
            LobbyRowType1 lobbyRow = types1.Find(lobbiesView => lobbiesView.data.roomId == lobby.roomId);
            if (lobbyRow != null && lobbyRow.gameObject != null)
            {
                GameObject.Destroy(lobbyRow.gameObject);
                types1.Remove(lobbyRow);
            }
        }
        else
        {
            LobbyRowType2 lobbyRow = types2.Find(lobbiesView => lobbiesView.data.roomId == lobby.roomId);
            GameObject.Destroy(lobbyRow.gameObject);
            types2.Remove(lobbyRow);
        }
    }

    public void UpdateLobby(DataLobby lobby)
    {
        _UpdateLobby(lobby);
    }

    private void _UpdateLobby(DataLobby lobby)
    {
        if (isShowType1)
        {
            _UpdateLobbyType1(lobby);
        }
        else
        {
            _UpdateLobbyType2(lobby);
        }
    }
    private void _UpdateLobbyType1(DataLobby lobby)
    {
        LobbyRowType1 lobbyRow = types1.Find(lb => lb.data.roomId == lobby.roomId);
        lobbyRow.setData(lobby);
    }
    private void _UpdateLobbyType2(DataLobby lobby)
    {
        LobbyRowType2 lobbyRow = types2.Find(lb => lb.data.roomId == lobby.roomId);
        lobbyRow.setData(lobby);
    }
    public void AddLobby(DataLobby lobby)
    {
        if (isShowType1)
        {
            StartCoroutine(_AddRowType1(lobby));
        }
        else
        {
            StartCoroutine(_AddRowType2(lobby));
        }
    }

    public void ShowError(string message)
    {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            DialogService.Instance.ShowDialog(new DialogMessage("Lỗi", message, null));
        });
    }

    public void ShowConfirm(string message, System.Action<bool?> action)
    {
        throw new System.NotImplementedException();
    }

    public PokerLobbyPresenter presenter { get; set; }


    public void JoinGame(DataLobby lobby)
    {
        presenter.JoinToGame(lobby);
    }
}
