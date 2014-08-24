﻿using UnityEngine;
using System.Collections;

public class LobbyScene : MonoBehaviour {

	public UITable tableType1;
	public UIGrid gridType2;
	public static Vector3 centerObject = Vector3.one;

	void Start () {
		tableType1.GetComponent<UIScaleCenterChild> ().onFinished = OnDragFinishGift;
		tableType1.GetComponent<UIScaleCenterChild> ().CenterOn (tableType1.transform.GetChild (0));
	}
	


	void OnDragFinishGift ()
	{
		centerObject = tableType1.GetComponent<UIScaleCenterChild> ().centeredObject.transform.position;
	}
}
