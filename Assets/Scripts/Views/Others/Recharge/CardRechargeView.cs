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
		WWWRequest request = new WWWRequest (this, model.image, 5, 3);
		request.isFullUrl = true;
		request.onResponse = delegate(IHttpRequest arg1, IHttpResponse arg2) {
			WWWResponse response = (WWWResponse)arg2;
			if(response.www.texture !=null){
				texture.mainTexture = response.www.texture;
			}
		};
		request.Start (null);
		NGUITools.AddWidgetCollider (gameObject);
	}

	void OnClicked (GameObject go)
	{
		if (action != null)
			action (model);
	}
}
