using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet.API.Client;
using Puppet;

[PrefabAttribute(Name = "Prefabs/Dialog/Gameplay/DialogGamePlayBuyChip", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
public class DialogBuyChipView : BaseDialog<DialogBuyChip,DialogBuyChipView>
{
    #region UnityEditor
    public UILabel minChip,maxChip,money;
    public UISlider slider;
    public UIToggle autoBuy;
    public GameObject btnMinChip, btnMaxChip;
    #endregion

    double minValue = 0;
    double maxValue = 0;
    double defaultValue = 0;
    double currentValue = 0;

    // Use this for initialization
    public override void ShowDialog(DialogBuyChip data)
    {
        base.ShowDialog(data);
        initData();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        EventDelegate.Add(slider.onChange, onSliderChange);
        UIEventListener.Get(btnMinChip).onClick += onBtnMinClick;
        UIEventListener.Get(btnMaxChip).onClick += onBtnMaxClick;
    }

    private void onSliderChange()
    {
        if (slider.value == 0)
            currentValue = minValue;
        else if (slider.value == 1)
            currentValue = maxValue;
        else
        {
            int index = Mathf.FloorToInt(Mathf.Lerp(1, slider.numberOfSteps, slider.value));
            currentValue = minValue * index;
        }

        string[] moneyAndShortcut = Utility.Convert.ConvertMoneyAndShortCut(currentValue);
        money.text = "$" + moneyAndShortcut [0].Trim() + moneyAndShortcut [1].Trim();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventDelegate.Remove(slider.onChange, onSliderChange);
        UIEventListener.Get(btnMinChip).onClick -= onBtnMinClick;
        UIEventListener.Get(btnMaxChip).onClick -= onBtnMaxClick;
    }
    private void initData()
    {
        minValue = data.smallBind * 20;
        maxValue = data.smallBind * 400;
        defaultValue = data.smallBind * 200;
        double playerMoney = PokerObserver.Game.mUserInfo.assets.GetAsset(EAssets.Chip).value;

        if (playerMoney < maxValue)
            maxValue = playerMoney;

        if (playerMoney < minValue)
            minValue = defaultValue = playerMoney;
        else if (playerMoney < defaultValue)
            defaultValue = playerMoney;

        minChip.text = minValue.ToString();
        maxChip.text = maxValue.ToString();
        slider.numberOfSteps = (int)(maxValue / minValue) + (maxValue % minValue > 0 ? 1 : 0);
        slider.value = (float)(defaultValue / maxValue) - 0.02f;

        string[] moneyAndShortcut = Utility.Convert.ConvertMoneyAndShortCut(PokerObserver.Game.mUserInfo.assets.content[0].value);
        labelTitle.text = "Số Gold hiện tại của bạn: $" + moneyAndShortcut[0] + moneyAndShortcut[1];
    }
    protected override void OnPressButton(bool? pressValue, DialogBuyChip data)
    {
        base.OnPressButton(pressValue, data);
        if (pressValue == true)
        {
            if(data.onChooise != null)
                data.onChooise(currentValue, autoBuy.value);
        }
        else if (data.onChooise != null)
            data.onChooise(0, false);
    }
    private void onBtnMaxClick(GameObject go)
    {
        slider.value = 1;
    }

    private void onBtnMinClick(GameObject go)
    {
        slider.value = 0;
    }
}
public class DialogBuyChip : AbstractDialogData
{
    public DialogBuyChip(double smallBind, System.Action<double, bool> onChooise)
    {
        this.smallBind = smallBind;
        this.onChooise = onChooise;
    }
    public System.Action<double, bool> onChooise;
    public int slot;
    public double smallBind;
    public double currentChip;
    public override void ShowDialog()
    {
        DialogBuyChipView.Instance.ShowDialog(this);
    }
}