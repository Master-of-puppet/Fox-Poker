using UnityEngine;
using System.Collections;

public class SearchViewCheckbox : MonoBehaviour {

    public UILabel lbValue;
    private int index;
    public int Index
    {
        get
        {
            return index;
        }
    }
    public static SearchViewCheckbox Create(string text, int index)
    {
        GameObject go = GameObject.Instantiate(Resources.Load("Prefabs/Dialog/SearchView/SearchViewCheckbox")) as GameObject;
        SearchViewCheckbox cbSearchView = go.GetComponent<SearchViewCheckbox>();
        cbSearchView.SetData(text, index);
        return cbSearchView;
    }
    
	void Start () {
	
	}
    private void SetData(string title,int index)
    {
        lbValue.text = title;
        this.index = index;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
