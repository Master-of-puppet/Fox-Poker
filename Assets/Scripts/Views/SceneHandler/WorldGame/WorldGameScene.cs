using UnityEngine;
using System.Collections;
using Puppet.Core.Model;
using Puppet.API.Client;
using Puppet.Service;

public class WorldGameScene : MonoBehaviour,IWorldGameView
{

    #region Unity Editor
    public UITable tableGame;
	public GameObject btnHelp;
    #endregion

    private WorldGamePresenter presenter { get; set; }
    void Start () {
        presenter = new WorldGamePresenter(this);
		HeaderMenuView.Instance.ShowInWorldGame();
	}
	void OnEnable(){
		UIEventListener.Get (btnHelp).onClick += OnClickButtonHelp;
	}
	void OnDestroy(){
		UIEventListener.Get (btnHelp).onClick -= OnClickButtonHelp;
	}

	void OnClickButtonHelp (GameObject go)
	{
		DialogService.Instance.ShowDialog (new DialogHelp ());
	}
	
	void Update () {
	
	}

    public void OnLoadGame(System.Collections.Generic.List<DataGame> datas)
    {
        foreach (DataGame item in datas)
        {
            GameItem.Create(item, tableGame.transform);
        }
        tableGame.Reposition();
    }

    public void ShowLevel(string level)
    {
        throw new System.NotImplementedException();
    }

    public void ShowExp(float currentExp, float nextExp, float currentMaxExp)
    {
        throw new System.NotImplementedException();
    }

    public void ShowError(string message)
    {
        DialogMessageView.Instance.ShowDialog(new DialogMessage("Lỗi", message, null));
    }

    public void ShowConfirm(string message, System.Action<bool?> action)
    {
        throw new System.NotImplementedException();
    }


}
