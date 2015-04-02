using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using Puppet;
using System.IO;
using Puppet.Core.Network.Http;

public class SendSMSService : Singleton<SendSMSService>
{
    ISendSMS send;

    protected override void Init()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        send = new SendSMSIOS();
#elif UNITY_ANDROID && !UNITY_EDITOR
        send = new SendSMSAndroid();
#else
        send = new SendSMSEditor();
#endif
    }

    public bool IsSupportSimCard
    {
        get { return send.IsSupportSimCard; }
    }

    public bool SendSMSMessage(List<string> smsNumbers, string message)
    {
        if (smsNumbers.Count == 0) return false;
        string numbers = "";
        for (int i = 0; i < smsNumbers.Count; i++) numbers += smsNumbers[i] + ",";
        if (numbers.Length > 0) numbers = numbers.Substring(0, numbers.Length - 1);
        
        if (!IsSupportSimCard)
            return false;

        return send.SendSMS(numbers, message);
    }

    public bool SendEmailMessage(List<string> emailAddresses, string subject, string message)
    {
        if (emailAddresses.Count == 0) return false;
        string email = string.Empty;
        for (int i = 0; i < emailAddresses.Count; i++) email += emailAddresses[i] + ",";
        if (!string.IsNullOrEmpty(email)) email = email.Substring(0, email.Length - 1);

        return send.SendMail(email, subject, message);
    }
}
