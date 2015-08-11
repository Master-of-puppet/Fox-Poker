using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using Puppet;
using System.IO;
using Puppet.Core.Network.Http;

public class DeviceInfoService : Singleton<DeviceInfoService>
{
    IDeviceInfo info;

    protected override void Init()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        info = new DeviceInfoIOS();
#elif UNITY_ANDROID && !UNITY_EDITOR
        info = new DeviceInfoAndroid();
#else
        info = new DeviceInfoEditor();
#endif
    }

    public string DeviceUniqueId
    {
        get { return SystemInfo.deviceUniqueIdentifier; }
    }

    public bool IsSupportSimCard
    {
        get { return info.IsSupportSimCard; }
    }

    public string IMEI
    {
        get { return info.IMEI; }
    }

    public string PhoneNumber
    {
        get { return info.PhoneNumber; }
    }

    public string SimSerialNumber
    {
        get { return info.SimSerialNumber; }
    }
}
