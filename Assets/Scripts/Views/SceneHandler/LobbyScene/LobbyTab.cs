using UnityEngine;
using System.Collections;
using Puppet.Core.Model;

public delegate void OnTabClickListener(DataChannel data);
public class LobbyTab : MonoBehaviour {
	#region UnityEditor
	public UILabel title;
	#endregion
	private event OnTabClickListener OnChoiceTab;
	public DataChannel data {
		get;
		set;
	}
	public void SetEventChoiceTab(OnTabClickListener onChoiceTabEvent){
		OnChoiceTab = onChoiceTabEvent;
	}
	public static LobbyTab Create(DataChannel data,UITable parent,int index){
		GameObject gobj = null;
		if (index != 0)
			gobj = GameObject.Instantiate (Resources.Load ("Prefabs/Lobby/LobbyTabCenter")) as GameObject;
		else {
			gobj = GameObject.Instantiate (Resources.Load ("Prefabs/Lobby/LobbyTabLeft")) as GameObject;
		}
        gobj.name = data.zoneId + "-" + data.name;
		gobj.transform.parent = parent.transform;
		gobj.transform.localPosition = Vector3.zero;
		gobj.transform.localScale = Vector3.one;
		LobbyTab item = gobj.GetComponent<LobbyTab> ();
		item.SetData (data,index);
		return item;

	}

    void Awake()
    {
        if (GetComponent<SoundButtonClick>() == null)
            gameObject.AddComponent<SoundButtonClick>();
    }

	void Start(){
		UIEventListener.Get (gameObject).onClick += onChoiceTab;
	}
	void onDestroy(){
		UIEventListener.Get (gameObject).onClick -= onChoiceTab;
	}
	void Update(){

	}
	void SetData (DataChannel data,int index)
	{
		this.data = data;
		title.text = data.displayName;
	}

	void onChoiceTab (GameObject go)
	{
		if (OnChoiceTab != null)
			OnChoiceTab (data);
	}
}
