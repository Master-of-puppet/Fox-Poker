using UnityEngine;
using System.Collections;

public class EmoticonAnimation : MonoBehaviour {

	public static EmoticonAnimation Create(string name,Transform parent){
		GameObject gobj = GameObject.Instantiate(Resources.Load("Prefabs/Emoticon/"+name)) as GameObject;
		gobj.transform.parent = parent;
		gobj.transform.localScale = Vector3.one;
		gobj.transform.localPosition = new Vector3 (0f, 50f, 0f);
		EmoticonAnimation item = gobj.GetComponent<EmoticonAnimation> ();
		item.killMeAfter (2.0f);
		return item;
	}
	public void killMeAfter(float second){
		StartCoroutine(_killMeAfter(second));
	}
	IEnumerator _killMeAfter(float second){
		yield return new WaitForSeconds(second);
		GameObject.Destroy (gameObject);
	}
}
