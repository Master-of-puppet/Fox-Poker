using UnityEngine;
using System.Collections;
using Puppet;
using System;
using System.Collections.Generic;
[PrefabAttribute(Name = "Prefabs/Dialog/SearchView/SearchView", Depth = 9,IsAttachedToCamera=true,IsUIPanel=true)]
public class SearchView : SingletonPrefab<SearchView>
{
    public static int TYPE_5_PEOPLE = 0;
    public static int TYPE_9_PEOPLE = 1;
    #region UnityEditor
    public UIInput txtInput;
    public GameObject btnSearch, btnExits;
    public UIToggle cbFivePeople, cbNinePeople;
    #endregion
    Action<string, Dictionary<int,bool>> onSearchSubmit;
    void Start() {
        btnExits.GetComponent<UISprite>().SetAnchor(NGUITools.GetRoot(gameObject).transform);
        btnExits.GetComponent<UISprite>().topAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().leftAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().rightAnchor.absolute = 0;
        btnExits.GetComponent<UISprite>().bottomAnchor.absolute = 0;
    }
    public void SetActionSubmit(Action<string, Dictionary<int,bool>> onSearchSubmit)
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
        bool[] arrayCheckbox = new bool[3];
        string text = txtInput.value;
        Dictionary<int, bool> option = new Dictionary<int, bool>();
        option.Add(TYPE_5_PEOPLE, cbFivePeople.value);
        option.Add(TYPE_9_PEOPLE, cbNinePeople.value);
        if (onSearchSubmit != null)
            onSearchSubmit(text, option);
        GameObject.Destroy(gameObject);
    }
    void Update()
    {

    }
}
