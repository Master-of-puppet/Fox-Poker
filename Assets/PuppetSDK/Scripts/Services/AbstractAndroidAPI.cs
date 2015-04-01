using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

internal abstract class AbstractAndroidAPI
{
    private const string CurrentActivity = "currentActivity";
    private const string ClassUnityPlayer = "com.unity3d.player.UnityPlayer";
    private const string ClassInstanceMethod = "getInstance";

    protected abstract string ClassAndroidAPI { get; }

#if UNITY_ANDROID
    AndroidJavaObject _androidActivity;
    protected virtual AndroidJavaObject GetActivity
    {
        get
        {
            if (_androidActivity == null)
            {
                using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass(ClassUnityPlayer))
                {
                    _androidActivity = cls_UnityPlayer.GetStatic<AndroidJavaObject>(CurrentActivity);
                }
            }
            return _androidActivity;
        }
    }

    AndroidJavaObject _androidobject;
    protected virtual AndroidJavaObject GetAndroidJavaObject
    {
        get
        {
            if (_androidobject == null)
            {
                AndroidJavaClass cls_AndroidAPI = new AndroidJavaClass(ClassAndroidAPI);
                _androidobject = cls_AndroidAPI.CallStatic<AndroidJavaObject>(ClassInstanceMethod, GetActivity);
            }
            return _androidobject;
        }
    }

    protected T Call<T>(string method, params object[] param)
    {
        return GetAndroidJavaObject.Call<T>(method, param);
    }
    protected void Call(string method, params object[] param)
    {
        GetAndroidJavaObject.Call(method, param);
    }
#endif
}
