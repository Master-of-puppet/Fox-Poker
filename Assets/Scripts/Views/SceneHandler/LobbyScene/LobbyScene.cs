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
    void Update()
    {
        if (tableType1.transform.parent.GetComponent<UIScrollView>().isDragging)
        {
            btnCreateGame.SetActive(false); 
            return;
        }
        if (tableType1.GetComponent<UICenterOnChild>().centeredObject != null && tableType1.transform.childCount > 0) { 
            btnCreateGame.SetActive(tableType1.GetComponent<UICenterOnChild>().centeredObject == tableType1.transform.GetChild(0).gameObject);
            return;
        }
        btnCreateGame.SetActive(true);

    }
    private void OnSearchLobbyHandler(string arg1, Dictionary<int, bool> arg2, double[] arrayBetValue)
    {
        presenter.SearchLobby(arg1, arg2, arrayBetValue);
    }
    void OnEnable()
    {
        UIEventListener.Get(btnPlayNow).onClick += OnClickPlayNow;
        UIEventListener.Get(btnCreateGame).onClick += OnClickCreateGame;
        if (btnHelp != null)
            UIEventListener.Get(btnHelp).onClick += OnClickHelp;
        tableType1.GetComponentInParent<UIScrollView>().onDragFinished += onDragScrollViewFinished ;
        tableType1.GetComponentInParent<UIScrollView>().onStoppedMoving += onScrollViewStopMoving ;
        tableType1.GetComponent<UICenterOnChild>().onFinished += onFinishCenter;
    }

    private void onFinishCenter()
    {
    }


    private void onScrollViewStopMoving()
    {
        tableType1.GetComponent<UICenterOnChild>().Recenter();
    }

    private void onDragScrollViewFinished()
    {
        tableType1.GetComponent<UICenterOnChild>().Recenter();
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
    int count = 0;
    IEnumerator initShowRowType1(List<DataLobby> lobbies)
    {
        ClearAllRow();
        yield return new WaitForEndOfFrame();

        if (lobbies.Count > 0)
        {
        for (int i = 0; i < lobbies.Count; i++)
        {
                if (types1.Find(lb => lb.data.roomId == lobbies[i].roomId) != null)
                    continue;
            LobbyRowType1 lobby = LobbyRowType1.Create(lobbies[i], tableType1, JoinGame);
            types1.Add(lobby);

        }

        tableType1.repositionNow = true;
            yield return new WaitForEndOfFrame();
            tableType1.GetComponent<UICenterOnChild>().CenterOn(tableType1.transform.GetChild(0));
        }
    }

    IEnumerator initShowRowType2(List<DataLobby> lobbies)
    {
        ClearAllRow();
        yield return new WaitForEndOfFrame();
        foreach (DataLobby item in lobbies)
        {
			if (types2.Find(lb => lb.data.roomId == item.roomId) != null)
				continue;
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
        if (lobbyRow != null)
            lobbyRow.setData(lobby);
    }
    private void _UpdateLobbyType2(DataLobby lobby)
    {
        LobbyRowType2 lobbyRow = types2.Find(lb => lb.data.roomId == lobby.roomId);
        if (lobbyRow != null)
            lobbyRow.SetData(lobby);
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
    public string TxtSearch()
    {
        return presenter.searchId;
    }
    public Dictionary<int, bool> options()
    {
        return presenter.searchDictionary;
    }


    public bool isFiltered()
    {
        return presenter.IsFiltered;
    }


    public void ShowLoading()
    {
        LoadingView.Instance.Show(true);
    }

    public void HideLoading()
    {
        LoadingView.Instance.Show(false);
    }

    public bool isDrawing { get; set; }
}
