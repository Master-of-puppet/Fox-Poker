using System;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Puppet
{
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif

	public class AppConfig : ScriptableObject
	{
		const string settingsAssetName = "AppConfig";
        const string settingsAssetFolder = "PuppetSDK/Config/Resources";
		const string settingsAssetExtension = ".asset";
		
		private static AppConfig _instance;
		
		public static AppConfig Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load(settingsAssetName) as AppConfig;
					
					if (_instance == null)
					{
						// If not found, autocreate the asset object.
						_instance = CreateInstance<AppConfig>();
						#if UNITY_EDITOR
						string fullPath = Path.Combine(Path.Combine("Assets", settingsAssetFolder), settingsAssetName + settingsAssetExtension);
						AssetDatabase.CreateAsset(_instance, fullPath);
						#endif
					}
				}
				return _instance;
			}
		}

        [SerializeField]
        private string[] distributorLabels = new[] { "foxpoker" };
		[SerializeField]
		private int selectedDistributeIndex = 0;
		[SerializeField]
        private string[] httpUrls = new[] { "http://foxpokers.com" };
        [SerializeField]
        private int[] httpPorts = new int[] { 80 };

		[SerializeField]
        private string[] socketUrls = new[] { "foxpokers.com" };
        [SerializeField]
        private int[] socketPorts = new int[] { 9933 };

        [SerializeField]
        private int[] appVersionValues = new int[4] { 1, 0, 0, 100 };

		[SerializeField]
		private string[] platformLabels = new[] { "pc", "web", "android", "ios", "others" };

		[SerializeField]
		private string bundleId;
        [SerializeField]
        private int lastBuild = 13;
        [SerializeField]
        private bool developmentBuild = true;

		public void Load () {}

			
		#region Environment
		public void SetDistributeIndex(int index)
		{
			if (selectedDistributeIndex != index)
			{
				selectedDistributeIndex = index;
				DirtyEditor();
			}
		}

		public int SelectedDistributeIndex
		{
			get { return selectedDistributeIndex; }
		}

		public void SetDistributtor (int index, string value)
		{
			if (httpUrls[index] != value)
			{
				httpUrls[index] = value;
				DirtyEditor();
			}
		}

		public string[] HttpUrls
		{
			get { return httpUrls; }
			set
			{
				if (httpUrls != value)
				{
					httpUrls = value;
					DirtyEditor();
				}
			}
		}

        public void SetHttpPort(int index, int value)
        {
            if (httpPorts[index] != value)
            {
                httpPorts[index] = value;
                DirtyEditor();
            }
        }

        public int[] HttpPorts
        {
            get { return httpPorts; }
            set
            {
                if (httpPorts != value)
                {
                    httpPorts = value;
                    DirtyEditor();
                }
            }
        }

		public void SetSocketUrl (int index, string value)
		{
			if (socketUrls[index] != value)
			{
				socketUrls[index] = value;
				DirtyEditor();
			}
		}

		public string[] SocketUrls
		{
			get { return socketUrls; }
			set
			{
				if (socketUrls != value)
				{
					socketUrls = value;
					DirtyEditor();
				}
			}
		}

        public void SetSocketPort(int index, int value)
        {
            if (socketPorts[index] != value)
            {
                socketPorts[index] = value;
                DirtyEditor();
            }
        }

        public int[] SocketPorts
        {
            get { return socketPorts; }
            set
            {
                if (socketPorts != value)
                {
                    socketPorts = value;
                    DirtyEditor();
                }
            }
        }
		
		public void SetDistributeLabel(int index, string value)
		{
			if (distributorLabels[index] != value)
			{
				distributorLabels[index] = value;
				DirtyEditor();
			}
		}
		
		public string[] DistributeLabels
		{
			get { return distributorLabels; }
			set
			{
				if (distributorLabels != value)
				{
					distributorLabels = value;
					DirtyEditor();
				}
			}
		}
		
		public static string HttpUrl
		{
			get
			{
				return Instance.httpUrls[Instance.SelectedDistributeIndex];
			}
		}

        public static int HttPort
        {
            get
            {
                return Instance.httpPorts[Instance.SelectedDistributeIndex];
            }
        }

		public static string SocketUrl
		{
			get
			{
				return Instance.socketUrls[Instance.SelectedDistributeIndex];
			}
		}

        public static int SocketPort
        {
            get
            {
                return Instance.socketPorts[Instance.SelectedDistributeIndex];
            }
        }

		public static string DistributorName
		{
			get
			{
				return Instance.distributorLabels[Instance.SelectedDistributeIndex];
			}
		}
		
		public static bool IsValidDistributor
		{
			get
			{
				return AppConfig.HttpUrl != null 
					&& AppConfig.HttpUrl.Length > 0 
						&& !AppConfig.HttpUrl.Equals("");
			}
		}
		#endregion

        #region App Version
        public void SetAppVersionValues(int index, int value)
        {
            if (appVersionValues[index] != value)
            {
                appVersionValues[index] = value;
                UpdateAppVersion();
            }
        }

        public int[] AppVersionValues
        {
            get { return appVersionValues; }
            set
            {
                if (appVersionValues != value)
                {
                    appVersionValues = value;
                    UpdateAppVersion();
                }
            }
        }

		public string AppVersion
		{
			get
			{
                return string.Format("{0}.{1}.{2}.{3}", AppVersionValues[0], AppVersionValues[1], AppVersionValues[2], AppVersionValues[3]);
			}
		}

        void UpdateAppVersion()
        {
#if UNITY_EDITOR
            PlayerSettings.bundleVersion = AppVersion;
#endif
            DirtyEditor();
        }
        #endregion

        #region Platform
		public void SetPlatformLabel(int index, string value)
		{
			if (platformLabels[index] != value)
			{
				platformLabels[index] = value;
				DirtyEditor();
			}
		}
		
		public string[] PlatformLabels
		{
			get { return platformLabels; }
			set
			{
				if (platformLabels != value)
				{
					platformLabels = value;
					DirtyEditor();
				}
			}
		}

		public static string Platform
		{
			get
			{
                return 
                    Application.isEditor ? Instance.platformLabels[0] : 
                    Application.isWebPlayer ? Instance.platformLabels[1] :
                    Application.platform == RuntimePlatform.Android ? Instance.platformLabels[2] :
                    Application.platform == RuntimePlatform.IPhonePlayer ? Instance.platformLabels[3] :
                    Application.platform == RuntimePlatform.OSXPlayer ? Instance.platformLabels[0] :
                    Application.platform == RuntimePlatform.LinuxPlayer ? Instance.platformLabels[0] :
                    Application.platform == RuntimePlatform.WindowsPlayer ? Instance.platformLabels[0] : 
                    Application.platform == RuntimePlatform.OSXDashboardPlayer ? Instance.platformLabels[0] :
                    Instance.platformLabels[4];
			}
		}
		
		public static bool IsValidPlatform
		{
			get
			{
				return AppConfig.Platform != null 
					&& AppConfig.Platform.Length > 0 
						&& !AppConfig.Platform.Equals("0");
			}
		}
		#endregion

		public string BundleId
		{
			get
			{
				#if UNITY_EDITOR
				bundleId = PlayerSettings.bundleIdentifier;
				#endif
				return bundleId;
			}
			set
			{
				if (!string.Equals (bundleId, value))
				{
					bundleId = value;
					#if UNITY_EDITOR
					PlayerSettings.bundleIdentifier = bundleId;
					#endif
					DirtyEditor ();
				}
			}
		}

#if UNITY_EDITOR
        public BuildTarget LastBuildTarget
        {
            get
            {
                return (BuildTarget)lastBuild;
            }
            set
            {
                if (lastBuild != (int)value)
                {
                    lastBuild = (int)value;
                    DirtyEditor();
                }
            }
        }
#endif

        public bool IsDevelopmentBuild
        {
            get
            {
                return developmentBuild;
            }
            set
            {
                if (developmentBuild != value)
                {
                    developmentBuild = value;
                    DirtyEditor();
                }
            }
        }

		
		private static void DirtyEditor()
		{
			#if UNITY_EDITOR
			EditorUtility.SetDirty(Instance);
			#endif
		}
	}
}



