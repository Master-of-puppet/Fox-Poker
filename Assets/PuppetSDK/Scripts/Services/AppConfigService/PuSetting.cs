using Puppet;
using Puppet.Utils.Loggers;
using Puppet.Utils.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using Puppet.Core.Model;

public class PuSetting
{
	public static string UniqueDeviceId;
    public static string persistentDataPath;

	public string sceneName;


    public PuSetting(Action<float, string> onLoadConfig)
    {
        persistentDataPath = Application.persistentDataPath;
        CurrentSetting setting = new CurrentSetting();
        PuMain.Setting = setting;
		PuMain.Setting.Init();
        setting.CustomServer();

        PuMain.Dispatcher.onChangeScene += ChangeScene;
        PuMain.Instance.Load(onLoadConfig);

        Puppet.Poker.PokerMain.Instance.StartListen();
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
        PuMain.Setting.OnUpdate();
    }

    public void OnApplicationPause(bool pauseStatus)
    {
        PuMain.Setting.OnApplicationPause(pauseStatus);
    }

    public void OnApplicationQuit()
    {
        PuMain.Setting.OnApplicationQuit();
    }

    class CurrentSetting : DefaultSetting
    {
        DataClientDetails _clientDetail;

        public override bool UseUnity
        {
            get { return true; }
        }

        public void CustomServer()
        {
            server = new ServerMode(AppConfig.SocketUrl, AppConfig.SocketPort);
            serverBundle = new WebServerMode(AppConfig.HttpUrl, AppConfig.HttPort);
            serverWebHttp = new WebServerMode(AppConfig.HttpUrl, AppConfig.HttPort);
        }

        protected override void AfterInit()
        {
            _clientDetail = new DataClientDetails();
            _clientDetail.bundleId = AppConfig.Instance.BundleId;
            _clientDetail.uniqueId = UniqueDeviceIdentification;
            _clientDetail.version = new Puppet.Core.Model.Version(AppConfig.Instance.AppVersionValues[0], AppConfig.Instance.AppVersionValues[1], AppConfig.Instance.AppVersionValues[2], AppConfig.Instance.AppVersionValues[3]);
            _clientDetail.distributor = AppConfig.DistributorName;
            _clientDetail.platform = AppConfig.Platform;

            IsDebug = UnityEngine.Debug.isDebugBuild;
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
            get { return PuSetting.persistentDataPath; }
        }

        public override DataClientDetails ClientDetails
        {
            get
            {
                return _clientDetail;
            }
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
            int port;
            public ServerMode(string domain, int port)
            {
                this.port = port;
                if (!string.IsNullOrEmpty(domain))
                    this.domain = domain;
                else
                    this.domain = "127.0.0.1";
            }
            public string GetBaseUrl() { return string.Format("{0}:{1}", domain, Port); }

            public int Port { get { return port; } }

            public string Domain { get { return domain; } }

            public string GetPath(string path) { return string.Format("{0}/{1}", GetBaseUrl(), path); }
        }

        class WebServerMode : IServerMode
        {
            string domain;
            int port;
            public WebServerMode(string domain, int port)
            {
                this.port = port;
                if (!string.IsNullOrEmpty(domain))
                    this.domain = domain;
                else
                    this.domain = "127.0.0.1";
            }
            public string GetBaseUrl() { return string.Format("{0}:{1}", domain, Port); }

            public int Port { get { return port; } }

            public string Domain { get { return domain; } }

            public string GetPath(string path) { return string.Format("{0}{1}", GetBaseUrl(), path); }
        }
    }
}
