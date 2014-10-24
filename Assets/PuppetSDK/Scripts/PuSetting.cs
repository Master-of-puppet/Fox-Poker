﻿using Puppet;
using Puppet.Utils.Loggers;
using Puppet.Utils.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;

public class PuSetting
{
	public static string UniqueDeviceId;

	public string sceneName;

    public PuSetting(string domain, string socketServer)
    {
        CurrentSetting setting = new CurrentSetting();
        PuMain.Setting = setting;
		PuMain.Setting.Init();
        setting.CustomServer(domain, socketServer);

        PuMain.Instance.Load();
        PuMain.Dispatcher.onChangeScene += ChangeScene;
    }

    void ChangeScene(EScene fromScene, EScene toScene)
    {
		sceneName = string.Empty;
        if (toScene == EScene.LoginScreen)
			sceneName = Scene.LoginScene.ToString ();
        else if (toScene == EScene.World_Game)

			sceneName = Scene.WorldGame.ToString();
        else if (toScene == EScene.Pocker_Plaza)
			sceneName = Scene.Poker_Plaza.ToString();
        else if (toScene == EScene.Pocker_Lobby)
        {
            if (fromScene == EScene.World_Game)
				sceneName = Scene.Poker_Plaza.ToString();
            else 
				sceneName =  Scene.LobbyScene.ToString();
        }
        else if (toScene == EScene.Pocker_Gameplay)
			sceneName = Scene.GameplayScene.ToString();
        else if (toScene == EScene.SplashScreen)
			sceneName =  Scene.SplashScene.ToString();

		PuApp.Instance.StartCoroutine(_ChangeScene(sceneName));
    }


	IEnumerator _ChangeScene(string sceneName)
	{
		PuApp.Instance.changingScene = true;
		AsyncOperation asyncLoadlevel = Application.LoadLevelAsync (sceneName);
		while(!asyncLoadlevel.isDone || asyncLoadlevel.progress < 1f)
			yield return new WaitForFixedUpdate();
		PuApp.Instance.changingScene = false;
	}

    public void Update()
    {
        if (PuMain.Setting.ActionUpdate != null)
            PuMain.Setting.ActionUpdate();
    }

    class CurrentSetting : DefaultSetting
    {
        public override bool UseUnity
        {
            get { return true; }
        }

        public void CustomServer(string domain, string socketServer)
        {
            //DefaultSetting.domain = domain;
            //DefaultSetting.soketServer = socketServer;
            server = new ServerMode(socketServer);
            serverWebService = new WebServiceServerMode(domain);
            serverBundle = new WebServerMode(domain);
            serverWebHttp = new WebServerMode(domain);
        }

        protected override void AfterInit()
        {

        }

        public override void ActionPrintLog(ELogType type, object message)
        {
            if (!IsDebug) return;

            switch (type)
            {
                case ELogType.Info:
                    UnityEngine.Debug.Log(message);
                    break;
                case ELogType.Warning:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case ELogType.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
                case ELogType.Exception:
                    UnityEngine.Debug.LogException((Exception)message);
                    break;
            }
        }

        public override Puppet.Utils.Threading.IThread Threading
        {
            get { return UnityThread.Instance; }
        }

        public override Puppet.Utils.Storage.IStorage PlayerPref
        {
            get { return UnityPlayerPrefab.Instance; }
        }

        public override string PathCache
        {
            get { return Path.Combine(Application.persistentDataPath, "Caching.save"); }
        }

        public override string UniqueDeviceIdentification
        {
            get
            {
				return UniqueDeviceId ?? SystemInfo.deviceUniqueIdentifier;
            }
        }



        class ServerMode : IServerMode
        {
            string domain;
            public ServerMode(string domain)
            {
                if (!string.IsNullOrEmpty(domain))
                    this.domain = domain;
                else
                    this.domain = "127.0.0.1";
            }

            public string GetBaseUrl() { return string.Format("https://{0}:{1}", Domain, Port); }

            public int Port { get { return 9933; } }

            public string Domain { get { return domain; } }

            public string GetPath(string path) { return string.Format("{0}/{1}", GetBaseUrl(), path); }
        }

        class WebServiceServerMode : IServerMode
        {
            string domain;
            public WebServiceServerMode(string domain)
            {
                if (!string.IsNullOrEmpty(domain))
                    this.domain = domain;
                else
                    this.domain = "127.0.0.1";
            }

            public string GetBaseUrl() { return string.Format("http://{0}:{1}", Domain, Port); }

            public int Port { get { return 80; } }

            public string Domain { get { return domain; } }

            public string GetPath(string path) { return string.Format("{0}/realtime/puppet/{1}", GetBaseUrl(), path); }
        }

        class WebServerMode : IServerMode
        {
            string domain;
            public WebServerMode(string domain)
            {
                if (!string.IsNullOrEmpty(domain))
                    this.domain = domain;
                else
                    this.domain = "127.0.0.1";
            }

            public string GetBaseUrl() { return string.Format("http://{0}:{1}", Domain, Port); }

            public int Port { get { return 80; } }

            public string Domain { get { return domain; } }

            public string GetPath(string path) { return string.Format("{0}/static/api/{1}", GetBaseUrl(), path); }
        }
    }
}
