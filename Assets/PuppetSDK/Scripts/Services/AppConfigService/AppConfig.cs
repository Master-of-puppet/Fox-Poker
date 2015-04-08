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
        private string[] distributorLabels = new[] { "Distributor Name" };
		[SerializeField]
		private int selectedDistributeIndex = 0;
		[SerializeField]
        private string[] httpUrls = new[] { "Http Server Url" };
        [SerializeField]
        private string[] httpPorts = new[] { "80" };

		[SerializeField]
        private string[] socketUrls = new[] { "Socket Server Url" };
        [SerializeField]
        private string[] socketPorts = new[] { "8888" };

		[SerializeField]
		private string appVersion;

		[SerializeField]
		private int selectedPlatformIndex = 0;
		[SerializeField]
		private string[] platformLabels = new[] { "Platform Name" };

		[SerializeField]
		private string bundleId;

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

        public void SetHttpPort(int index, string value)
        {
            if (httpPorts[index] != value)
            {
                httpPorts[index] = value;
                DirtyEditor();
            }
        }

        public string[] HttpPorts
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

        public void SetSocketPort(int index, string value)
        {
            if (socketPorts[index] != value)
            {
                socketPorts[index] = value;
                DirtyEditor();
            }
        }

        public string[] SocketPorts
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

		public static string SocketUrl
		{
			get
			{
				return Instance.socketUrls[Instance.SelectedDistributeIndex];
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
		
		public string AppVersion
		{
			get
			{
				#if UNITY_EDITOR
				appVersion = PlayerSettings.bundleVersion;
				#endif
				return appVersion;
			}
			set
			{
				if (!string.Equals (appVersion, value))
				{
					appVersion = value;
					#if UNITY_EDITOR
					PlayerSettings.bundleVersion = value;
					#endif
					DirtyEditor ();
				}
			}
		}

		#region Platform
		public void SetPlatformIndex(int index)
		{
			if (selectedPlatformIndex != index)
			{
				selectedPlatformIndex = index;
				DirtyEditor();
			}
		}
		
		public int SelectedPlatformIndex
		{
			get { return selectedPlatformIndex; }
		}
		
		public void SetPlatform(int index, string value)
		{
			if (platformLabels[index] != value)
			{
				platformLabels[index] = value;
				DirtyEditor();
			}
		}
		
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
				return Instance.platformLabels[Instance.SelectedPlatformIndex];
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

		public void SelectPlatformByPlatformLabel (string platformLabel)
		{
			int index = GetIndexByPlatformLabel (platformLabel);
			if (index >= 0)
			{
				SetPlatformIndex (index);
			}
		}
		
		private int GetIndexByPlatformLabel (string platformLabel)
		{
			int result = -1;
			for (int i = 0; i < platformLabels.Length; i++)
			{
				if (string.Equals (platformLabel, platformLabels[i]))
				{
					result = i;
					break;
				}
			}
			
			return result;
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

		
		private static void DirtyEditor()
		{
			#if UNITY_EDITOR
			EditorUtility.SetDirty(Instance);
			#endif
		}
	}
}



