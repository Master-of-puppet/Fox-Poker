using System;
using System.Collections.Generic;
using UnityEngine;
using Puppet;
using Puppet.Core;

[Prefab(Name = "Prefabs/Others/Debug/PingUI")]
public class PingManager : SingletonPrefab<PingManager>
{

    [SerializeField]
    UnityEngine.UI.Text pingValue;

    void Awake()
    {
        PingHandler.Instance.StartPing();
    }

    void OnDestroy()
    {
        PingHandler.Instance.StopPing();
    }
    
    void Start()
    {
        InvokeRepeating("InvokeValue", 0f, 1f);
    }

    void InvokeValue()
    {
        if(PingHandler.Instance.IsRunning)
            pingValue.text = string.Format("{0} ms", PingHandler.Instance.Value);
        else
            pingValue.text = string.Empty;
    }
}
