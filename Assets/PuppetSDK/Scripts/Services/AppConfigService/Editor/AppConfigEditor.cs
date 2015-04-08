using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using System.Diagnostics;

namespace Puppet
{
	[CustomEditor (typeof (AppConfig))]
	internal class AppConfigEditor : Editor
	{
		private AppConfig config;

        GUIContent distributeNameLabel = new GUIContent("Distributor Name [?]", "For your own use, example:'dev', 'qa', 'prod'");
        GUIContent httpUrlLabel = new GUIContent("Web Server URL [?]", "Server Url for Http Web Request");
        GUIContent httpPortLabel = new GUIContent("Port [?]", "Port own use for Web Server, example: '80', '8888', '8080'");
		GUIContent socketUrlLabel = new GUIContent("Socket Server URL [?]", "Server Url for Socket");
        GUIContent socketPortLabel = new GUIContent("Port [?]", "Port own use for Socket Server, example: '80', '8888', '8080'");
		GUIContent appVersionLabel = new GUIContent("App Version [?]", "Application Version, example: 1.0.0");
		GUIContent platformTypwLabel = new GUIContent("Platform Type [?]", "Platform Type, reference for Platform specific functions");
		GUIContent bundleIdLabel = new GUIContent("App Bundle Identifier [?]", "Application Bundle Id, example: com.abc.helloworld");
		
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

            BuildUI();
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
                config.SetHttpPort(i, EditorGUILayout.IntField(config.HttpPorts[i], GUILayout.Width(Screen.width / 3f)));
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

                var httpPorts = new List<int>(config.HttpPorts);
                httpPorts.Add(80);
                config.HttpPorts = httpPorts.ToArray();

				var skUrls = new List<string>(config.SocketUrls);
				skUrls.Add("");
                config.SocketUrls = skUrls.ToArray();

                var socketPorts = new List<int>(config.SocketPorts);
                socketPorts.Add(8888);
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

                    var httpPorts = new List<int>(config.HttpPorts);
                    httpPorts.RemoveAt(httpPorts.Count - 1);
                    config.HttpPorts = httpPorts.ToArray();

					var skUrls = new List<string>(config.SocketUrls);
					skUrls.RemoveAt(skUrls.Count - 1);
					config.SocketUrls = skUrls.ToArray();

                    var socketPorts = new List<int>(config.SocketPorts);
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
                config.SetSocketPort(i, EditorGUILayout.IntField(config.SocketPorts[i], GUILayout.Width(Screen.width / 3f)));
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
			
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Major", GUILayout.Width(Screen.width / 4f));
            EditorGUILayout.LabelField("Minor", GUILayout.Width(Screen.width / 4f));
            EditorGUILayout.LabelField("Patch", GUILayout.Width(Screen.width / 4f));
            EditorGUILayout.LabelField("Build", GUILayout.Width(Screen.width / 4f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < config.AppVersionValues.Length; i++)
            {
                config.SetAppVersionValues(i, EditorGUILayout.IntField(config.AppVersionValues[i], GUILayout.Width(Screen.width / 4f)));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditableField(new GUIContent("Current version: ", ""), config.AppVersion);
            EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();
		}

		private void PlatformGUI ()
		{
			EditorGUILayout.HelpBox("5) Update Platform(s) value associated with the development", MessageType.None);
			
			EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(platformTypwLabel);
			EditorGUILayout.EndHorizontal();
			for (int i = 0; i < config.PlatformLabels.Length; ++i)
			{
				EditorGUILayout.BeginHorizontal();
                string title = i == 0 ? "Standalone: " : i == 1 ? "Web Player: " : i == 2 ? "Android: " : i == 3 ? "iOS: " : "Other platforms: ";
                GUIContent content = new GUIContent(title, "");
                config.SetPlatformLabel(i, EditableField(content, config.PlatformLabels[i]));
				GUI.changed = false;
				EditorGUILayout.EndHorizontal();
			}
			
			EditorGUILayout.Space();
		}

		private void BundleGUI ()
		{
			EditorGUILayout.HelpBox("6) Add iOS bundle id or Android package name", MessageType.None);
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

        [MenuItem("Puppet SDK/Edit Application Configuration", priority = 0)]
		static void EditSettings ()
		{
			Selection.activeObject = AppConfig.Instance;
		}

        [MenuItem("Puppet SDK/PlayerPrefs Delete All", priority = 99)]
        static void PlayerPrefsDeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Puppet SDK/Build/Android")]
        static void BuildAndroid()
        {
            PerformBuild(BuildTarget.Android, AppConfig.Instance.IsDevelopmentBuild);
        }
        [MenuItem("Puppet SDK/Build/iOS")]
        static void BuildiOS()
        {
            PerformBuild(BuildTarget.iPhone, AppConfig.Instance.IsDevelopmentBuild);
        }
        [MenuItem("Puppet SDK/Build/PC")]
        static void BuildPC()
        {
            PerformBuild(BuildTarget.StandaloneWindows, AppConfig.Instance.IsDevelopmentBuild);
        }
        [MenuItem("Puppet SDK/Support - Report Bug")]
        static void Support()
        {
            Application.OpenURL("http://jira.chieuvuong.com/browse/FP/");
            Application.OpenURL("mailto:vietdungvn88@gmail.com?subject=Need Support in PuppetSDK&body=Hi Dung,I need support for issue");
        }
        
        #region BUILD
        private void BuildUI()
        {
            GUILayout.FlexibleSpace();

            config.IsDevelopmentBuild = EditorGUILayout.Toggle("Is Development Build: ", config.IsDevelopmentBuild);
            EditorGUILayout.Space();
            config.LastBuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Platform Build Target: ", config.LastBuildTarget);
            EditorGUILayout.Space();

            if (GUILayout.Button("Perform Build - " + config.LastBuildTarget.ToString(), GUILayout.Height(Screen.height / 12f)))
                PerformBuild(config.LastBuildTarget, config.IsDevelopmentBuild);
        }
        static void PerformBuild(BuildTarget targetBuild, bool isDevelopment)
        {
            UnityEngine.Debug.Log("*****PerformBuild - Build target: " + targetBuild);

            //EditorUserBuildSettings.SwitchActiveBuildTarget(targetBuild);
            EditorUserBuildSettings.appendProject = true;
            EditorUserBuildSettings.allowDebugging = false;
            EditorUserBuildSettings.development = isDevelopment;

            if (targetBuild == BuildTarget.Android)
            {
                PlayerSettings.Android.keystorePass = "puppet#2014";
                PlayerSettings.Android.keyaliasName = "foxpoker";
                PlayerSettings.Android.keyaliasPass = "puppet#2014";
            }

            UnityEngine.Debug.Log("*****PerformBuild - Using the bundle id: " + PlayerSettings.bundleIdentifier);

            string buildTargetDirect = Application.dataPath.Replace("/Assets", string.Format("/Build/{0}", targetBuild.ToString().ToLower()));
            if (Directory.Exists(buildTargetDirect) == false)
                Directory.CreateDirectory(buildTargetDirect);

            string locationPath = string.Format("{0}/{1}", buildTargetDirect, "FoxPoker");
            if (targetBuild == BuildTarget.Android)
                locationPath += string.Format("_{0:yyyyMMddHHmm}.apk", DateTime.Now);
            else if (targetBuild == BuildTarget.StandaloneWindows || targetBuild == BuildTarget.StandaloneWindows64)
                locationPath += ".exe";
            else if (targetBuild == BuildTarget.StandaloneOSXIntel || targetBuild == BuildTarget.StandaloneOSXIntel64 || targetBuild == BuildTarget.StandaloneOSXUniversal)
                locationPath += ".app";

            UnityEngine.Debug.Log("*****PerformBuild - UNITY_BUILD_TARGET Use path: " + locationPath);

            string[] buildScenes = (from scene in EditorBuildSettings.scenes where scene.enabled select scene.path).ToArray();
            var result = BuildPipeline.BuildPlayer(buildScenes, locationPath, targetBuild,
                targetBuild == BuildTarget.iPhone ? BuildOptions.SymlinkLibraries | BuildOptions.Development | BuildOptions.None : BuildOptions.None
            );

            if (result.Length > 0)
                UnityEngine.Debug.LogError("*****PerformBuild - Result: " + result);

            OpenExplorer(buildTargetDirect);
        }

        static bool OpenExplorer(string path)
        {
            bool isError = false;
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.UseShellExecute = false;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
#if UNITY_EDITOR_OSX
                startInfo.FileName = "sh";
			    startInfo.Arguments = string.Format("open -a Terminal {0}", path);
#elif UNITY_EDITOR_WIN
                startInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
                startInfo.Arguments = string.Format("/c explorer {0}", path.Replace("/", "\\"));
#endif
                Process p = Process.Start(startInfo);
                p.OutputDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                        UnityEngine.Debug.Log("OpenExplorer: " + e.Data);
                };
                p.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (e.Data != null)
                    {
                        UnityEngine.Debug.LogError("OpenExplorer: " + e.Data);
                        isError = true;
                    }
                };
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();
                p.WaitForExit();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("OpenExplorer: " + e.Message);
                return false;
            }
            return !isError;
        }
        #endregion
    }
}
