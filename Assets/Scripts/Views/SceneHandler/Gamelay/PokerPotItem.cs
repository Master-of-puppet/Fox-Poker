using Puppet.Poker.Datagram;
using Puppet.Poker.Models;
using System.Collections.Generic;
using UnityEngine;

public class PokerPotItem : MonoBehaviour
{
    #region UNITY EDITOR
    public UISprite spriteIcon;
    public UIWidget[] allWidget;
    public UIWidget[] hiddensOnMove;
    public UILabel labelCurrentbet;
    #endregion
    double _currentBet = 0;
    private string[] nameChipArr = { "icon_chip_1", "icon_chip_2", "icon_chip_3", "icon_chip_4" };
    Puppet.Poker.Datagram.ResponseUpdatePot.DataPot _pot;

    public Puppet.Poker.Datagram.ResponseUpdatePot.DataPot Pot
    {
        get { return _pot; }
    }
    public static PokerPotItem Create(Puppet.Poker.Datagram.ResponseUpdatePot.DataPot pot)
    {
        GameObject gobj = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gameplay/PotItem"));
        gobj.name = "" + pot.id;
        PokerPotItem item = gobj.GetComponent<PokerPotItem>();
        item.SetValue(pot);
        return item;
    }
    public void SetValue(Puppet.Poker.Datagram.ResponseUpdatePot.DataPot pot)
    {
        this._pot = pot;
        SetBet(pot.value);
    }
    public void SetBet(double value)
    {
        string[] money = Utility.Convert.ConvertMoneyAndShortCut(value);
        _currentBet = value;
        labelCurrentbet.text = string.Format("{0:f2}{1}", money[0], money[1]);
    }
    
    public double CurrentBet { get { return _currentBet; } }

    public void SetAlpha(int alpha)
    {
        foreach(UIWidget widget in allWidget)
            widget.color = new Color(widget.color.r, widget.color.g, widget.color.b, alpha);
    }

    public void OnMove()
    {
        foreach (UIWidget widget in hiddensOnMove)
            NGUITools.SetActive(widget.gameObject, false);
    }
}
