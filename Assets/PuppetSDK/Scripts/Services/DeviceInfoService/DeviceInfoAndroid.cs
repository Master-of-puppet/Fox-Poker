using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class DeviceInfoAndroid : AbstractAndroidAPI, IDeviceInfo
{
    protected override string ClassAndroidAPI
    {
        get { return "com.ensign.deviceinfo.MainAPI"; }
    }

    public bool IsSupportSimCard
    {
        get {
#if UNITY_ANDROID && !UNITY_EDITOR
            return Call<bool>("isSimSupport", new object[] { });
#endif
            return false;
        }
    }

    public string IMEI
    {
        get { 
#if UNITY_ANDROID && !UNITY_EDITOR
            return Call<string>("getDeviceId", new object[] { });
#endif
            return string.Empty;
        }
    }

    public string SimSerialNumber
    {
        get {
#if UNITY_ANDROID && !UNITY_EDITOR
            return Call<string>("getSimSerialNumber", new object[] { });
#endif
            return string.Empty;
        }
    }

    public string PhoneNumber
    {
        get {
#if UNITY_ANDROID && !UNITY_EDITOR
            return Call<string>("getPhoneNumber", new object[] { });
#endif
            return string.Empty;
        }
    }
}
