using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SendSMSAndroid : SendSMSAbstract
{
    protected override string ClassAndroidAPI
    {
        get { return "com.ensign.sendsms.SendSMSAPI"; }
    }

    public override bool IsSupportSimCard
    {
        get {
#if UNITY_ANDROID && !UNITY_EDITOR
            return Call<bool>("dungnv_IsSupportSimCard", new object[] { });
#endif
            return false;
        }
    }

    public override bool SendSMS(string phoneNumber, string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Call<bool>("dungnv_SendSMS", new object[] { phoneNumber, message });
#endif
        return false;
    }
}
