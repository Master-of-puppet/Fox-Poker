using UnityEngine;
using System.Collections;
using System;
using Puppet.Core.Model;
using Puppet.Core.Network.Http;

public class SMSRechargeView : MonoBehaviour {

	#region UnityEditor
	public UITexture texture;
	public UILabel lbMoney;
	#endregion

	Action<DataRecharge> actionClick;

	DataRecharge model;
	void OnEnable(){
		UIEventListener.Get (gameObject).onClick += OnClicked;
	}
	void OnDisable(){
		UIEventListener.Get (gameObject).onClick -= OnClicked;
	}
	
	void OnClicked (GameObject go)
	{
		if (actionClick != null)
			actionClick (model);
	}
	

	public void SetData(DataRecharge model,Action<DataRecharge> action){
		this.model = model;
        PuApp.Instance.GetImage(model.image, (txture) => texture.mainTexture = txture);
		
		lbMoney.text = model.code_value;;
		NGUITools.AddWidgetCollider (gameObject);
		this.actionClick = action;
	}

}