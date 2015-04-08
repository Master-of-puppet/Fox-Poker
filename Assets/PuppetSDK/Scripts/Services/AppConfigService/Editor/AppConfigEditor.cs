using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace Puppet
{
	[CustomEditor (typeof (AppConfig))]
	internal class AppConfigEditor : Editor
	{
		private AppConfig config;

        GUIContent distributeNameLabel = new GUIContent("Distributor Name [?]:", "For your own use, example:'dev', 'qa', 'prod'");
        GUIContent httpUrlLabel = new GUIContent("Web Server URL [?]:", "Server Url for Http Web Request");
        GUIContent httpPortLabel = new GUIContent("Port Web [?]:", "Port own use for Web Server, example: '80', '8888', '8080'");
		GUIContent socketUrlLabel = new GUIContent("Socket Server URL [?]:", "Server Url for Socket");
        GUIContent socketPortLabel = new GUIContent("Port Socket [?]:", "Port own use for Socket Server, example: '80', '8888', '8080'");
		GUIContent appVersionLabel = new GUIContent("App Version [?]:", "Application Version, example: 1.0.0");
		GUIContent platformTypwLabel = new GUIContent("Platform Type [?]:", "Platform Type, reference for Platform specific functions");
		GUIContent bundleIdLabel = new GUIContent("App Bundle Identifier [?]:", "Application Bundle Id, example: com.abc.helloworld");
		
		public void OnEnable ()
		{
			config = (AppConfig) target;
		}
		
		public override void OnInspectorGUI ()
		{
			EditorGUIUtility.LookLikeControls ();

			EnvironmentGUI ();
			AppVersionGUI ();
			PlatformGUI ();
			BundleGUI ();
		}

		private void EnvironmentGUI()
		{
            EditorGUILayout.HelpBox("1) Add the Distributor(s) associated with the development", MessageType.None);
			if (config.HttpUrls.Length == 0 || config.HttpUrls[config.SelectedDistributeIndex] == "0")
			{
                EditorGUILayout.HelpBox("Invalid Distributor", MessageType.Error);
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(distributeNameLabel, GUILayout.Width(Screen.width / 3f));
            EditorGUILayout.LabelField(httpUrlLabel, GUILayout.Width(Screen.width / 3f));
            EditorGUILayout.LabelField(httpPortLabel, GUILayout.Width(Screen.width / 3f));
			EditorGUILayout.EndHorizontal();
			for (int i = 0; i < config.HttpUrls.Length; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				config.SetDistributeLabel(i, EditorGUILayout.TextField(config.DistributeLabels[i], GUILayout.Width(Screen.width / 3f)));
				GUI.changed = false;
                config.SetDistributtor(i, EditorGUILayout.TextField(config.HttpUrls[i], GUILayout.Width(Screen.width / 3f)));
                GUI.changed = false;
                config.SetHttpPort(i, EditorGUILayout.TextField(config.HttpPorts[i], GUILayout.Width(Screen.width / 3f)));
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Add Another Distributor"))
			{
				var envLabels = new List<string>(config.DistributeLabels);
				envLabels.Add("New Distributor");
				config.DistributeLabels = envLabels.ToArray();
				
				var envURLs = new List<string>(config.HttpUrls);
				envURLs.Add("");
				config.HttpUrls = envURLs.ToArray();

                var httpPorts = new List<string>(config.HttpPorts);
                httpPorts.Add("80");
                config.HttpPorts = httpPorts.ToArray();

				var skUrls = new List<string>(config.SocketUrls);
				skUrls.Add("");
                config.SocketUrls = skUrls.ToArray();

                var socketPorts = new List<string>(config.SocketPorts);
                socketPorts.Add("8888");
                config.SocketPorts = socketPorts.ToArray();
			}

			if (config.DistributeLabels.Length > 1)
			{
                if (GUILayout.Button("Remove Last Distributor"))
				{
					var envLabels = new List<string>(config.DistributeLabels);
					envLabels.RemoveAt(envLabels.Count - 1);
					config.DistributeLabels = envLabels.ToArray();
					
					var envURLs = new List<string>(config.HttpUrls);
					envURLs.RemoveAt(envURLs.Count - 1);
					config.HttpUrls = envURLs.ToArray();

                    var httpPorts = new List<string>(config.HttpPorts);
                    httpPorts.RemoveAt(httpPorts.Count - 1);
                    config.HttpPorts = httpPorts.ToArray();

					var skUrls = new List<string>(config.SocketUrls);
					skUrls.RemoveAt(skUrls.Count - 1);
					config.SocketUrls = skUrls.ToArray();

                    var socketPorts = new List<string>(config.SocketPorts);
                    socketPorts.RemoveAt(socketPorts.Count - 1);
                    config.SocketPorts = socketPorts.ToArray();
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();

			EditorGUILayout.HelpBox("2) Add the Socket config associated with the Environment if needed", MessageType.None);
			EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(distributeNameLabel, GUILayout.Width(Screen.width / 3f));
            EditorGUILayout.LabelField(socketUrlLabel, GUILayout.Width(Screen.width / 3f));
            EditorGUILayout.LabelField(socketPortLabel, GUILayout.Width(Screen.width / 3f));
			EditorGUILayout.EndHorizontal();
			for (int i = 0; i < config.SocketUrls.Length; ++i)
			{
				EditorGUILayout.BeginHorizontal();
                config.SetDistributeLabel(i, EditorGUILayout.TextField(config.DistributeLabels[i], GUILayout.Width(Screen.width / 3f)));
				GUI.changed = false;
                config.SetSocketUrl(i, EditorGUILayout.TextField(config.SocketUrls[i], GUILayout.Width(Screen.width / 3f)));
                GUI.changed = false;
                config.SetSocketPort(i, EditorGUILayout.TextField(config.SocketPorts[i], GUILayout.Width(Screen.width / 3f)));
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.Space();

			if (config.HttpUrls.Length > 1)
			{
                EditorGUILayout.HelpBox("3) Select Distributor to be compiled with this game", MessageType.None);
				GUI.changed = false;
                config.SetDistributeIndex(EditorGUILayout.Popup("Selected Distributor", config.SelectedDistributeIndex, config.DistributeLabels));
	//			if (GUI.changed)
	//				ManifestMod.GenerateManifest();
				EditorGUILayout.Space();
			}
			else
			{
				config.SetDistributeIndex (0);
			}
		}

		private void AppVersionGUI ()
		{
			EditorGUILayout.HelpBox("4) Application Version", MessageType.None);
			config.AppVersion = EditableField(appVersionLabel, config.AppVersion);
			EditorGUILayout.Space();
		}

		private void PlatformGUI ()
		{
			EditorGUILayout.HelpBox("6) Add Platform(s) associated with the development", MessageType.None);
			if (config.PlatformLabels.Length == 0 || config.PlatformLabels[config.SelectedPlatformIndex] == "0")
			{
				EditorGUILayout.HelpBox("Invalid Platform Type", MessageType.Error);
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(platformTypwLabel);
			EditorGUILayout.EndHorizontal();
			for (int i = 0; i < config.PlatformLabels.Length; ++i)
			{
				EditorGUILayout.BeginHorizontal();
				config.SetPlatformLabel(i, EditorGUILayout.TextField(config.PlatformLabels[i]));
				GUI.changed = false;
				//			if (GUI.changed)
				//				ManifestMod.GenerateManifest();
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Add Another Platform Type"))
			{
				var platformLabels = new List<string>(config.PlatformLabels);
				platformLabels.Add("New Platform");
				config.PlatformLabels = platformLabels.ToArray();
			}
			
			if (config.PlatformLabels.Length > 1)
			{
				if (GUILayout.Button("Remove Last Platform Type"))
				{
					var platformLabels = new List<string>(config.PlatformLabels);
					platformLabels.RemoveAt(platformLabels.Count - 1);
					config.PlatformLabels = platformLabels.ToArray();
				}
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
			if (config.PlatformLabels.Length > 1)
			{
				EditorGUILayout.HelpBox("7) Select a Platform to be compiled with this game", MessageType.None);
				GUI.changed = false;
				config.SetPlatformIndex(EditorGUILayout.Popup("Selected Platform Type: ", config.SelectedPlatformIndex, config.PlatformLabels));
				//			if (GUI.changed)
				//				ManifestMod.GenerateManifest();
				EditorGUILayout.Space();
			}
			else
			{
				config.SetPlatformIndex (0);
			}
		}

		private void BundleGUI ()
		{
			EditorGUILayout.HelpBox("8) Add iOS bundle id or Android package name", MessageType.None);
			config.BundleId = EditableField(bundleIdLabel, config.BundleId);
			EditorGUILayout.Space();
		}
		
		private string EditableField (GUIContent label, string value)
		{
			string ret = "";
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField (label, GUILayout.Width (120), GUILayout.Height (16));
			ret = EditorGUILayout.TextField (value, GUILayout.Height (16));
	//		if (GUI.changed && (label.Equals(pushKeyLabel) || label.Equals(gcmKeyLabel) || label.Equals(appIdLabel))){
	//			ManifestMod.GenerateManifest();
	//		}
			EditorGUILayout.EndHorizontal ();
			return ret;
		}

		[MenuItem ("Puppet Editor/Edit Application Configuration", priority=0)]
		static void EditSettings ()
		{
			Selection.activeObject = AppConfig.Instance;
		}

        [MenuItem("Puppet Editor/PlayerPrefs Delete All", priority = 99)]
        static void PlayerPrefsDeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
	}
}
