using UnityEngine;
using System.Collections;

public class EmoticonSticker : MonoBehaviour {

	public static EmoticonSticker Create(string name,Transform parent){
		GameObject gobj = GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/EmoticonSticker")) as GameObject;
		gobj.transform.parent = parent;
		gobj.transform.localScale = Vector3.one;
		gobj.transform.localPosition = new Vector3 (0f, 50f, 0f);
		gobj.GetComponent<UISprite> ().spriteName = name;
		gobj.GetComponent<UISprite> ().MakePixelPerfect ();
		EmoticonSticker sticker = gobj.GetComponent<EmoticonSticker> ();
		sticker.killMeAfter (2.0f);
		return sticker;
	}
	public void killMeAfter(float second){
		StartCoroutine(_killMeAfter(second));
	}
	IEnumerator _killMeAfter(float second){
		yield return new WaitForSeconds(second);
		GameObject.Destroy (gameObject);
	}

}
