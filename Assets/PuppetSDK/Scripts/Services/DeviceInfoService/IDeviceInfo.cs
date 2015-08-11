using System;
using System.Collections.Generic;

public interface IDeviceInfo
{
    bool IsSupportSimCard { get; }
    string IMEI { get; }
    string SimSerialNumber { get; }
    string PhoneNumber { get; }
}
