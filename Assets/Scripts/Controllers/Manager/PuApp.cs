using UnityEngine;
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
    public PuSetting setting;

    List<KeyValuePair<EMessage, string>> listMessage = new List<KeyValuePair<EMessage, string>>();
    protected override void Init()
    {
        setting = new PuSetting("foxpokers.com", "foxpokers.com");
		gameObject.AddComponent<LogViewer> ();
    }

    public void StartApplication()
    {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            PuMain.Dispatcher.onWarningUpgrade += Dispatcher_onWarningUpgrade;
			PuMain.Dispatcher.onDailyGift +=Dispatcher_onDailyGift;
            PuMain.Dispatcher.onNoticeMessage += Dispatcher_onNoticeMessage;
        });

		SocialService.SocialStart ();
    }

    void Dispatcher_onNoticeMessage(EMessage type, string message)
    {
        PuMain.Setting.Threading.QueueOnMainThread(() =>
        {
            DialogService.Instance.ShowDialog(new DialogMessage("Warning", message, null));
        });
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

    void OnApplicationPause(bool pauseStatus)
    {
        if (setting != null)
            setting.OnApplicationPause(pauseStatus);
    }

    void OnApplicationQuit()
    {
        if (setting != null)
            setting.OnApplicationQuit();
    }

    public void BackScene()
    {
        Puppet.API.Client.APIGeneric.BackScene((bool status, string message) => {
            if (!status)
                Logger.Log(message);
        });
    }

    public void ExecuteFuntion(float delayTime, System.Action callback)
    {
        StartCoroutine(_DelayFunction(delayTime, callback));
    }

    IEnumerator _DelayFunction(float delayTime, System.Action callback)
    {
        yield return new WaitForSeconds(delayTime);
        if (callback != null)
            callback();
    }

    public void RequestInviteApp(string message)
    {
        Puppet.Service.SocialService.AppRequest(SocialType.Facebook, "", null, null, (status, requestIds) => 
        {
           if(status)
           {
               Puppet.API.Client.APIGeneric.SaveRequestFB(SocialService.GetSocialNetwork(SocialType.Facebook).UserId, requestIds, (saveStatus, saveMessage) =>
               {
                   string responseMessage = saveStatus ? "Bạn đã gửi lời mới thành công." : message;
                   DialogService.Instance.ShowDialog(new DialogMessage("Gửi lời mời.", responseMessage, null));
               });
           }
           else
           {
               DialogService.Instance.ShowDialog(new DialogMessage("Gửi lời mời.", "Không thể gửi lời mới cho bạn bè", null));
           }
        });
    }
}
