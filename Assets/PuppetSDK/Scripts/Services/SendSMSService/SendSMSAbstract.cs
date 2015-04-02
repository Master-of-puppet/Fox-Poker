using System;
using System.Collections.Generic;
using UnityEngine;

class SendSMSAbstract : AbstractAndroidAPI, ISendSMS
{
    public virtual bool IsSupportSimCard
    {
        get { return false; }
    }
    public virtual bool SendSMS(string phoneNumber, string message)
    {
        return false;
    }
    public virtual bool SendMail(string email, string subject, string message)
    {
        string url = string.Format("mailto:{0}?subject={1}&body={2}", email, WWW.EscapeURL(subject), WWW.EscapeURL(message));
        url = url.Replace("+", "%20");
        Application.OpenURL(url);
        return true;
    }

    protected override string ClassAndroidAPI
    {
        get { return string.Empty; }
    }
}
