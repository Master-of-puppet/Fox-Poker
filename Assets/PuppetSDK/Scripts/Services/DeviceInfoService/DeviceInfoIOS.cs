using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

class DeviceInfoIOS : IDeviceInfo
{
#if UNITY_IPHONE && !UNITY_EDITOR
    //[DllImport("__Internal")]
    //public static extern bool dungnv_IsRunningInSimulator();

    //[DllImport("__Internal")]
    //public static extern bool dungnv_ShowMessageComposer(string phone, string message);

    //[DllImport("__Internal")]
    //public static extern bool dungnv_ShowMailComposer(string email, string subject, string message);
#endif

    public bool SendSMS(string phoneNumber, string message)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        return dungnv_ShowMessageComposer(phoneNumber, message);
#endif
        return false;
    }

    public bool SendMail(string email, string subject, string message)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        return dungnv_ShowMailComposer(email, subject, message);
#endif
        return false;
    }

    public bool IsSupportSimCard
    {
        get { 
#if UNITY_IPHONE && !UNITY_EDITOR
            return !dungnv_IsRunningInSimulator();
#endif
            return false;      
        
        }
    }

    public string IMEI
    {
        get {
#if UNITY_IPHONE && !UNITY_EDITOR
            return string.Empty;
#endif
            return string.Empty;
        }
    }

    public string SimSerialNumber
    {
        get {
#if UNITY_IPHONE && !UNITY_EDITOR
            return string.Empty;
#endif
            return string.Empty;
        }
    }

    public string PhoneNumber
    {
        get {
#if UNITY_IPHONE && !UNITY_EDITOR
            return string.Empty;
#endif
            return string.Empty;
        }
    }
}
