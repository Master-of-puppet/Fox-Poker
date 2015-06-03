using UnityEngine;
using System.Collections;
using Puppet;
using System;
using System.Collections.Generic;
[PrefabAttribute(Name = "Prefabs/Dialog/SearchView/SearchView", Depth = 9, IsAttachedToCamera = true, IsUIPanel = true)]
public class SearchView : SingletonPrefab<SearchView>
{
    public static int TYPE_5_PEOPLE = 0;
    public static int TYPE_9_PEOPLE = 1;
    #region UnityEditor
    public UIInput txtInput;
    public GameObject btnSearch, btnExits;
    public UIToggle cbFivePeople, cbNinePeople;
    public UITable tableBetValue;
    #endregion
    Action<string, Dictionary<int, bool>, double[]> onSearchSubmit;

    void Start()
    {
        btnExits.GetComponent<UISprite>().SetAnchor(NGUITools.GetRoot(gameObject).transform);
        btnExits.GetComponent<UISprite>().topAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().leftAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().rightAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().bottomAnchor.absolute = 0;
        LobbyScene lobby = GameObject.Find("Lobby Scene").GetComponent<LobbyScene>();
        valuesBetting = lobby.presenter.SelectedChannel.configuration.betting;
        for (int i = 0; i < valuesBetting.Length; i++)
        {
            double currentBetting = valuesBetting[i];
            string value = Utility.Convert.ConvertShortcutMoney(currentBetting / 2) + "/" + Utility.Convert.ConvertShortcutMoney(currentBetting);
            SearchViewCheckbox item = SearchViewCheckbox.Create(value, i);
            item.name = "" + i;
            item.transform.parent = tableBetValue.transform;
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;
            if (lobby.isFiltered())
                item.GetComponent<UIToggle>().value = false;
        }
        tableBetValue.Reposition();
        if (lobby.isFiltered())
        {
            txtInput.value = lobby.TxtSearch();
            if (lobby.options() != null)
            {
                foreach (int key in lobby.options().Keys)
                {
                    if (key == TYPE_5_PEOPLE)
                    {
                        cbFivePeople.value = lobby.options()[key];
                    } if (key == TYPE_9_PEOPLE)
                    {
                        cbNinePeople.value = lobby.options()[key];
                    }
                }
            }

            foreach (double value in lobby.presenter.betValueSearch)
            {
                int index = Array.FindIndex<double>(valuesBetting, a => a == value);
                tableBetValue.GetChildList()[index].GetComponent<UIToggle>().value = true;    
            }
        }

    }
    public void SetActionSubmit(Action<string, Dictionary<int, bool>, double[]> onSearchSubmit)
    {
        this.onSearchSubmit = onSearchSubmit;
    }
    void OnEnable()
    {
        UIEventListener.Get(btnSearch).onClick += OnSearchClick;
        UIEventListener.Get(btnExits).onClick += OnCloseSearchView;
    }
    void OnDisable()
    {
        UIEventListener.Get(btnSearch).onClick -= OnSearchClick;
        UIEventListener.Get(btnExits).onClick -= OnCloseSearchView;
    }
    private void OnCloseSearchView(GameObject go)
    {
        GameObject.Destroy(gameObject);
    }

    private void OnSearchClick(GameObject go)
    {
        text = txtInput.value;
        option = new Dictionary<int, bool>();
        option.Add(TYPE_5_PEOPLE, cbFivePeople.value);
        option.Add(TYPE_9_PEOPLE, cbNinePeople.value);
        List<double> bettingChoosen = new List<double>();
        for (int i = 0; i < tableBetValue.transform.childCount; i++)
        {
            Transform gobj = tableBetValue.transform.GetChild(i);
            if (gobj.GetComponent<UIToggle>().value)
            {
                int index = gobj.GetComponent<SearchViewCheckbox>().Index;
                bettingChoosen.Add(valuesBetting[index]);
            }
        }

        if (onSearchSubmit != null)
            onSearchSubmit(text, option,bettingChoosen.ToArray());
        GameObject.Destroy(gameObject);
    }
    void Update()
    {

    }

    private string text;

    private Dictionary<int, bool> option;

    public double[] valuesBetting;
}