using UnityEngine;
using System.Collections;
using System;
using Puppet.Core.Model;
using Puppet.Core.Network.Http;

public class CardRechargeView : MonoBehaviour {
	#region UnityEditor
	public UITexture texture;
	#endregion	
	DataRecharge model;
	Action<DataRecharge> action;
	void OnEnable(){
		UIEventListener.Get (gameObject).onClick += OnClicked;
	}
	void OnDisable(){
		UIEventListener.Get (gameObject).onClick -= OnClicked;
	}
	public void SetData(DataRecharge model,Action<DataRecharge> action){
		this.model = model;
		this.action = action;
        PuApp.Instance.GetImage(model.image, (txture) => texture.mainTexture = txture);
		NGUITools.AddWidgetCollider (gameObject);
	}

	void OnClicked (GameObject go)
	{
		if (action != null)
			action (model);
	}
}
