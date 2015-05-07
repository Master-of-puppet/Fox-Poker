using UnityEngine;
using System.Collections;
using Puppet.Service;
using System.Collections.Generic;
using Puppet;
using Puppet.API.Client;
using Puppet.Core.Model;
using System.Text.RegularExpressions;

[PrefabAttribute(Name = "Prefabs/Dialog/DialogRechargeChip", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogRechargeView : BaseDialog<DialogRecharge, DialogRechargeView>
{

    #region UnityEditor
    public UILabel lbTitleCard, lbSupportPhone;
    public UIInput txtSerial, txtPassword;
    public GameObject panelInputCard, panelScrollCardAndSMS, btnClosePanelInputCard, btnSubmitCard;
    #endregion
    #region Tab Recharge
    public UITable tableTypeRecharge;
    public UIGrid tableRechargeItem;


    #endregion
    List<GameObject> lstRechargeGobj = new List<GameObject>();



    public override void ShowDialog(DialogRecharge data)
    {
        base.ShowDialog(data);
        initTab();
        lbSupportPhone.text = data.dataRecharge.support_phone;

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        UIEventListener.Get(btnClosePanelInputCard).onClick = OnClosePanelInputCard;
        UIEventListener.Get(btnSubmitCard).onClick = onButtonSubmitCardClickListener;
    }
    void onButtonSubmitCardClickListener(GameObject go)
    {
        if (txtSerial.value.Length > 0 && txtPassword.value.Length > 0)
        {
            APIGeneric.RechargeCard(txtPassword.value, txtSerial.value, Utility.ReadData.GetTrackId(), currentCard.provider, (status, message) =>
        {

            panelInputCard.SetActive(false);
            panelScrollCardAndSMS.SetActive(true);
            DialogService.Instance.ShowDialog(new DialogMessage("Thông báo", message, null));
        });
        }
    }

    void OnClosePanelInputCard(GameObject go)
    {
        panelInputCard.SetActive(false);
        panelScrollCardAndSMS.SetActive(true);
    }

    private void initTab()
    {
        fetchGroupCard();
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


            foreach (string item in dictCard.Keys)
            {
                GameObject gobj = GameObject.Instantiate(Resources.Load("Prefabs/Dialog/Recharge/CardItem")) as GameObject;
                gobj.transform.parent = tableRechargeItem.transform;
                gobj.transform.localScale = Vector3.one;
                gobj.transform.localPosition = Vector3.zero;
                gobj.GetComponent<CardRechargeView>().SetData(dictCard[item][0], OnCardClickListener);
                lstRechargeGobj.Add(gobj);
            }
        }
        tableRechargeItem.Reposition();
    }
    private void fetchGroupCard()
    {
        dictCard = new Dictionary<string, List<DataRecharge>>();
        foreach (DataRecharge card in data.dataRecharge.card)
        {
            if (!dictCard.ContainsKey(card.provider))
            {
                List<DataRecharge> list = new List<DataRecharge>();
                list.Add(card);
                dictCard.Add(card.provider, list);
            }
            else
            {
                dictCard[card.provider].Add(card);
            }
        }
    }
    private void OnSMSClickListener(DataRecharge model)
    {
        string[] templates = Regex.Split(model.template, @"{{");
        UserInfo userInfo = Puppet.API.Client.APIUser.GetUserInformation(); ;
        string template = "";
        if (templates[1].Contains("username"))
        {
            template = templates[0] + userInfo.info.userName;
        }else{
            template = templates[0] + userInfo.info.id;
        }
        SendSMSService.Instance.SendSMSMessage(new List<string>(new string[] { model.code}), template);
    }

    DataRecharge currentCard;

    private void OnCardClickListener(DataRecharge model)
    {
        panelInputCard.SetActive(true);
        lbTitleCard.text = "Nạp thẻ cào " + model.provider;
        currentCard = model;
        panelScrollCardAndSMS.SetActive(false);
    }

    public Dictionary<string, List<DataRecharge>> dictCard { get; set; }
}
public class DialogRecharge : AbstractDialogData
{
    public List<RechargeTab> tabs;
    public DataResponseRecharge dataRecharge = null;

    public DialogRecharge(DataResponseRecharge dataRecharge)
        : base()
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
