﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Puppet;
using Puppet.Core;
using Puppet.Utils;
using Puppet.Utils.Threading;
using Puppet.Service;
using Puppet.Core.Model;

public class PuApp : Singleton<PuApp>
{
	public bool changingScene;
    PuSetting setting;

    List<KeyValuePair<EMessage, string>> listMessage = new List<KeyValuePair<EMessage, string>>();
    protected override void Init()
    {
		setting = new PuSetting("puppet.esimo.vn", "puppet.esimo.vn");
		gameObject.AddComponent<LogViewer> ();
    }

    public void StartApplication()
    {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            PuMain.Dispatcher.onWarningUpgrade += Dispatcher_onWarningUpgrade;
			PuMain.Dispatcher.onDailyGift +=Dispatcher_onDailyGift;
        });

		SocialService.SocialStart ();
    }

	void Dispatcher_onDailyGift(DataDailyGift obj)
	{
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            DialogService.Instance.ShowDialog(new DialogPromotion(obj));
        });
	}

    void Dispatcher_onWarningUpgrade(EUpgrade type, string message, string market)
    {
        if (type == EUpgrade.ForceUpdate || type == EUpgrade.MaybeUpdate)
        {
            PuMain.Setting.Threading.QueueOnMainThread(() =>
            {
                DialogService.Instance.ShowDialog(new DialogConfirm("Kiểm tra phiên bản", message, delegate(bool? click)
                {
                    if (click == true || type == EUpgrade.ForceUpdate)
                        Application.OpenURL(market);
                }));
            });
        }
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

	public void OnApplicationQuit()
	{
		PuMain.Socket.Disconnect();
	}
}
