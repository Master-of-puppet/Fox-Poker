using UnityEngine;
using System.Collections;
using Puppet.Service;
using System.Collections.Generic;
using Puppet;
using Puppet.API.Client;
using Puppet.Core.Model;

[PrefabAttribute(Name = "Prefabs/Dialog/DialogRechargeChip", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogRechargeView : BaseDialog<DialogRecharge, DialogRechargeView>
{

    #region UnityEditor
	public UILabel lbTitleCard,lbSupportPhone;
	public UIInput txtSerial,txtPassword;
	public GameObject panelInputCard,panelScrollCardAndSMS,btnClosePanelInputCard,btnSubmitCard;
	#endregion
    #region Tab Recharge
    public UITable tableTypeRecharge;
    public UIGrid tableRechargeItem;


    #endregion
	List<GameObject> lstRechargeGobj = new List<GameObject> ();



    public override void ShowDialog(DialogRecharge data)
    {
        base.ShowDialog(data);
        initTab();

    }
	protected override void OnEnable ()
	{
		base.OnEnable ();
		UIEventListener.Get (btnClosePanelInputCard).onClick = OnClosePanelInputCard;
		UIEventListener.Get (btnSubmitCard).onClick = onButtonSubmitCardClickListener;
	}
	void onButtonSubmitCardClickListener (GameObject go)
	{
		if(txtSerial.value.Length > 0 && txtPassword.value.Length >0){
		APIGeneric.RechargeCard(txtPassword.value, txtSerial.value, currentCard.provider, (status, message) =>
   		{
			Logger.Log("Send RechargeCard - Status: " + status);
			Logger.Log("Send RechargeCard - Message: " + message);
			panelInputCard.SetActive(false);
			panelScrollCardAndSMS.SetActive(true);
            if (status)
                DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", message, null));
		});
		}
	}

	void OnClosePanelInputCard (GameObject go)
	{
		panelInputCard.SetActive(false);
		panelScrollCardAndSMS.SetActive(true);
	}

    private void initTab()
    {
		lbSupportPhone.text = data.dataRecharge.support_phone;
        foreach (RechargeTab item in data.tabs)
        {
            GameObject gobj = GameObject.Instantiate(Resources.Load("Prefabs/Dialog/Recharge/RechargeChipTab")) as GameObject;
            gobj.transform.parent = tableTypeRecharge.transform;
            gobj.transform.localScale = Vector3.one;
            gobj.transform.localPosition = Vector3.zero;
            gobj.GetComponent<RechargeTabView>().SetData(item, OnTabRechargeSelected);
        }
        tableTypeRecharge.Reposition();
		tableTypeRecharge.transform.GetChild(0).GetComponent<UIToggle>().value = true;
    }
    private void OnTabRechargeSelected(bool isSelected, RechargeTab tab)
    {
        if (isSelected)
        {
            StartCoroutine(ShowTabRecharge(tab));
        }
    }
    IEnumerator ShowTabRecharge(RechargeTab tab)
    {

        while (lstRechargeGobj.Count > 0)
        {
            GameObject.Destroy(lstRechargeGobj[0]);
            lstRechargeGobj.RemoveAt(0);
        }
        lstRechargeGobj.Clear();
        yield return new WaitForSeconds(0.1f);
        if (tab.typeRecharge == 0)
        {
			foreach (DataRecharge item in data.dataRecharge.sms)
            {
                GameObject gobj = GameObject.Instantiate(Resources.Load("Prefabs/Dialog/Recharge/SMSItem")) as GameObject;
                gobj.transform.parent = tableRechargeItem.transform;
                gobj.transform.localScale = Vector3.one;
                gobj.transform.localPosition = Vector3.zero;
                gobj.GetComponent<SMSRechargeView>().SetData(item, OnSMSClickListener);
                lstRechargeGobj.Add(gobj);
            }
        }
        else if (tab.typeRecharge == 1)
        {
            foreach (DataRecharge item in data.dataRecharge.card)
            {
                GameObject gobj = GameObject.Instantiate(Resources.Load("Prefabs/Dialog/Recharge/CardItem")) as GameObject;
                gobj.transform.parent = tableRechargeItem.transform;
                gobj.transform.localScale = Vector3.one;
                gobj.transform.localPosition = Vector3.zero;
                gobj.GetComponent<CardRechargeView>().SetData(item, OnCardClickListener);
                lstRechargeGobj.Add(gobj);
            }
        }
        tableRechargeItem.Reposition();
    }
	private void OnSMSClickListener(DataRecharge model)
    {

    }

	DataRecharge currentCard;

	private void OnCardClickListener(DataRecharge model)
    {
		panelInputCard.SetActive (true);
		lbTitleCard.text = "Nạp thẻ cào " + model.provider;
		currentCard = model;
		panelScrollCardAndSMS.SetActive (false);
    }
}
public class DialogRecharge : AbstractDialogData
{
    public List<RechargeTab> tabs;
	public DataResponseRecharge dataRecharge = null;
    
	public DialogRecharge(DataResponseRecharge dataRecharge) : base()
    {
        tabs = new List<RechargeTab>();
		this.dataRecharge = dataRecharge;
        RechargeTab tabSMS = new RechargeTab();
        tabSMS.title = "Nạp chip qua SMS";
        tabSMS.typeRecharge = 0;
        RechargeTab tabCard = new RechargeTab();
        tabCard.title = "Nạp chip qua thẻ cào";
        tabCard.typeRecharge = 1;
        tabs.Add(tabSMS);
        tabs.Add(tabCard);


    }
    public override void ShowDialog()
    {
        DialogRechargeView.Instance.ShowDialog(this);
    }
}
