using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet.Poker.Datagram;
using System;
using Puppet;

public class PokerPotManager : MonoBehaviour
{

    #region UnityEditor
    public UITable tablePot;
    public GameObject topLeftPosition,topRightPosition;
    #endregion
    List<PokerPotItem> pots = new List<PokerPotItem>();

    public void UpdatePot(List<Puppet.Poker.Datagram.ResponseUpdatePot.DataPot> datas) 
    {
        StartCoroutine(_UpdatePot(datas));
    }
    IEnumerator _UpdatePot(List<Puppet.Poker.Datagram.ResponseUpdatePot.DataPot> datas)
    {
        foreach (Puppet.Poker.Datagram.ResponseUpdatePot.DataPot item in datas)
        {
            currPot = null;
            PokerPotItem currentPot = pots.Find(p => item.id == p.Pot.id);
            if (currentPot != null)
            {
                currentPot.SetValue(item);
            }
            else
            {
                PokerPotItem potNew = PokerPotItem.Create(item);
                switch (pots.Count)
                {
                    case 6:
                        potNew.transform.parent = topLeftPosition.transform;
                        break;
                    case 7:
                        potNew.transform.parent = topRightPosition.transform;
                        break;
                    default :
                        potNew.transform.parent = tablePot.transform;
                        break;

                }
                potNew.transform.localScale = Vector3.one;
                potNew.transform.localPosition = Vector3.zero;              
                pots.Add(potNew);
            }
        }
        yield return new WaitForEndOfFrame();
        tablePot.Reposition();
        NGUITools.AddWidgetCollider(tablePot.gameObject);
        tablePot.transform.localPosition = new Vector3(-tablePot.gameObject.collider.bounds.size.x/2, 0, 0);

    }
    public void MovePotToPlayer(ResponseMoneyExchange[] exchangeMoney, int potId, float timeEffect)
    {
        foreach (ResponseMoneyExchange ex in exchangeMoney) {
            if (ex.moneyExchange > 0) {
               PokerPlayerUI[] ui = GameObject.FindObjectsOfType<PokerPlayerUI>();
               PokerPlayerUI pokerUI = Array.Find(ui, pui => pui.UserName == ex.userName);
               if (pokerUI != null)
               {
                   PokerPotItem item = pots.Find(p => p.Pot.id == potId);
                   currPot = item;
                   pots.Remove(item);
                   if (item != null) { 
                        iTween.MoveTo(item.gameObject,iTween.Hash("position",pokerUI.transform.localPosition,"islocal",true,"time",0.5f,"oncomplete","onMovePotComplete","oncompletetarget",gameObject));
                   }
               }
            }
        }
    }
    private void onMovePotComplete() {
        if (currPot != null && currPot.gameObject != null)
        {
            GameObject.Destroy(currPot.gameObject);
        }
        currPot = null;
    }
    public void DestroyAllPot() 
    {
        foreach (PokerPotItem item in pots)
            GameObject.Destroy(item.gameObject);
        pots.Clear();
    }

    public PokerPotItem currPot { get; set; }
}
