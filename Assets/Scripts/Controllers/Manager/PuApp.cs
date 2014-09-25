﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet;
using Puppet.Core;
using Puppet.Utils;
using Puppet.Utils.Threading;

public class PuApp : Singleton<PuApp>
{
    PuSetting setting;

    List<KeyValuePair<EMessage, string>> listMessage = new List<KeyValuePair<EMessage, string>>();
    protected override void Init()
    {
        setting = new PuSetting("test.esimo.vn");
    }

    public void StartApplication()
    {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            PuMain.Instance.Dispatcher.onWarningUpgrade += Dispatcher_onWarningUpgrade;
        });
    }

    void Dispatcher_onWarningUpgrade(EUpgrade type, string message, string market)
    {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            DialogConfirmModel model = new DialogConfirmModel("Kiểm tra phiên bản", message, null);
            DialogFactory.QueueOrShowDialog(model);
        });
    }

    void FixedUpdate()
    {
        if (setting != null)
            setting.Update();
    }

    public void BackScene()
    {
        Puppet.API.Client.APIGeneric.BackScene((bool status, string message) => {
            if (!status)
                Logger.Log(message);
        });
    }
    void OnDestroy() {
        DialogFactory.ApplicationDestroy();
    }
}
