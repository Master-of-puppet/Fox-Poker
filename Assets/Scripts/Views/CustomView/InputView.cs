using UnityEngine;
using System.Collections;

public class InputView: MonoBehaviour {
	public void onBlankInput(){	
		UILabel label = gameObject.GetComponentInChildren<UILabel> ();
		if (label !=null && !string.IsNullOrEmpty(label.text)) {
			gameObject.GetComponent<UIInput>().value = "";
		}
	}
	public void onValueChange(){
		UISprite btnClose = gameObject.GetComponentInChildren<UIAnchor>().transform.GetChild(0).GetComponent<UISprite>();
		if (btnClose != null) {
			btnClose.gameObject.SetActive(!string.IsNullOrEmpty(gameObject.GetComponent<UIInput>().value));
		}
	}
}
