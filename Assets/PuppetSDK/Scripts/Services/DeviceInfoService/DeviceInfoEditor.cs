using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class DeviceInfoEditor : IDeviceInfo
{
    public bool IsSupportSimCard
    {
        get { return false; }
    }

    public string IMEI
    {
        get { return "IMEI_UNITY_EDITOR"; }
    }

    public string SimSerialNumber
    {
        get { return "SIM_SERIAL_UNITY_EDITOR"; }
    }

    public string PhoneNumber
    {
        get { return "PHONE_NUMBER_SERIAL_UNITY_EDITOR"; }
    }
}
