using UnityEngine;
using System.Collections;
using Puppet.Core.Model;

public class PlazaEventItem : MonoBehaviour
{

    #region UnityEditor
    #endregion
    public static PlazaEventItem Create(DataEvent evt)
    {
        GameObject gobj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/PokerPlaza/PlazaEventItem"));
        gobj.GetComponent<PlazaEventItem>().SetData(evt);
        return gobj.GetComponent<PlazaEventItem>();
    }
    public void SetData(DataEvent evt)
    {
        this.evt = evt;
        PuApp.Instance.GetImage(evt.image,(texture)=>GetComponent<UITexture>().mainTexture = texture);
    }
    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public DataEvent evt { get { return _evt; } set { this._evt = value; } }
    private DataEvent _evt;
}
