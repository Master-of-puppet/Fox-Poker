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
    #endregion
    Action<string, Dictionary<int, bool>> onSearchSubmit;

    void Start()
    {
        btnExits.GetComponent<UISprite>().SetAnchor(NGUITools.GetRoot(gameObject).transform);
        btnExits.GetComponent<UISprite>().topAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().leftAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().rightAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().bottomAnchor.absolute = 0;
        LobbyScene lobby = GameObject.Find("Lobby Scene").GetComponent<LobbyScene>();
        txtInput.value = lobby.roomId();
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

    }
    public void SetActionSubmit(Action<string, Dictionary<int, bool>> onSearchSubmit)
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
        if (onSearchSubmit != null)
            onSearchSubmit(text, option);
        GameObject.Destroy(gameObject);
    }
    void Update()
    {

    }

    public string text;

    public Dictionary<int, bool> option;
}