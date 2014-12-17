﻿using UnityEngine;
using System.Collections;
using Puppet.API.Client;
using Puppet;
using System;
using Puppet.Service;
[PrefabAttribute(Name = "Prefabs/Dialog/Gameplay/DialogGameplayBetting", Depth = 9)]
public class DialogBettingView : BaseDialog<DialogBetting, DialogBettingView>
{
    #region UnityEditor
    public UISlider sliderBar;
    public UILabel labelMoney;
    #endregion


    double smallBlind;
    EventDelegate del;
    void Awake()
    {

        del = new EventDelegate(this, "OnSliderChange");
        smallBlind = PokerObserver.Instance.gameDetails.customConfiguration.SmallBlind;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        sliderBar.onChange.Add(del);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        sliderBar.onChange.Remove(del);
    }

    public double GetCurrentMoney
    {
        get
        {
            double money = (smallBlind * GetSliderIndex) + data.MaxBinded;
            return money;
        }
    }
    int GetSliderIndex
    {
        get
        {
            int index = (int)Mathf.Lerp(1, sliderBar.numberOfSteps, sliderBar.value);
            return index;
        }
    }
    double GetMinBind
    {
        get
        {
            return data.MaxBinded + smallBlind;
        }
    }
    void OnSliderChange()
    {
        Logger.Log("==========" + GetCurrentMoney);
        labelMoney.text = GetCurrentMoney >= data.MaxBetting ? "All In" : GetCurrentMoney.ToString("#,###");
        if (sliderBar.value == 1) labelMoney.text = "All In";
        if (GetCurrentMoney >= data.MaxBetting && GetCurrentMoney < PokerObserver.Game.MainPlayer.GetMoney())
            labelMoney.text = GetCurrentMoney.ToString("#,###");

    }

    public override void ShowDialog(DialogBetting data)
    {
        base.ShowDialog(data);
        sliderBar.numberOfSteps = (int)((data.MaxBetting - data.MaxBinded) / smallBlind);
        gameObject.transform.parent = data.parent;
        gameObject.transform.localPosition = new Vector3(0f, 280f, 0f);
        gameObject.transform.localScale = Vector3.one;
        gameObject.transform.parent = null;
    }

    protected override void OnPressButton(bool? pressValue, DialogBetting data)
    {
        if (pressValue == true && data.onBetting != null)
        {
            if (labelMoney.text == "All In" && data.MaxBetting - GetCurrentMoney < smallBlind)
            {
                data.onBetting(data.MaxBetting);
            }
            else
                data.onBetting(GetCurrentMoney >= data.MaxBetting ? data.MaxBetting : GetCurrentMoney);
        }

    }
}

public class DialogBetting : AbstractDialogData
{
    public double MaxBinded, MaxBetting;
    public Action<double> onBetting;
    public Transform parent;
    public double currentMoney;
    public DialogBetting(double maxBinded, double max, Action<double> onBetting, Transform parent)
    {
        this.MaxBinded = maxBinded;
        this.MaxBetting = max;
        this.onBetting = onBetting;
        this.parent = parent;
    }

    public override void ShowDialog()
    {
        DialogBettingView.Instance.ShowDialog(this);
    }

    protected override string GetButtonName(bool? button)
    {
        return "Tố";
    }
}