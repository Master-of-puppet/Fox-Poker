using UnityEngine;
using System.Collections;

namespace Puppet.Service{
	[PrefabAttribute(Name = "Prefabs/Dialog/DialogSettings", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
	public class DialogSettingsView : BaseDialog<DialogSetting,DialogSettingsView> {
		#region Unity Editor
		public UILabel lbUserName, lbVersion;
		public GameObject btnLogout;
        public UIProgressBar toggleAutoSit,toggleSoundBackground,toggleSoundEffect,toggleLockScreen;
		#endregion
		public override void ShowDialog (DialogSetting data)
		{
			base.ShowDialog (data);
			InitData (data);
		}
		protected override void OnEnable(){
			base.OnEnable();
			UIEventListener.Get (btnLogout).onClick += onBtnLogoutClick;
			UIEventListener.Get (toggleAutoSit.gameObject).onClick += onToggleAutoSit;
            UIEventListener.Get(toggleSoundBackground.gameObject).onClick += onToggleSoundBackground;
            UIEventListener.Get(toggleSoundEffect.gameObject).onClick += onToggleSoundEffect;
            UIEventListener.Get(toggleLockScreen.gameObject).onClick += onToggleLockScreen;
			
		}
		protected override void  OnDisable(){
			base.OnDisable();
			UIEventListener.Get (btnLogout).onClick -= onBtnLogoutClick;
            UIEventListener.Get(toggleAutoSit.gameObject).onClick -= onToggleAutoSit;
            UIEventListener.Get(toggleSoundBackground.gameObject).onClick -= onToggleSoundBackground;
            UIEventListener.Get(toggleSoundEffect.gameObject).onClick -= onToggleSoundEffect;
            UIEventListener.Get(toggleLockScreen.gameObject).onClick -= onToggleLockScreen;
		}
		void InitData (DialogSetting data)
		{
            string currentVertion = string.Empty;
            option = Puppet.API.Client.APIGeneric.GetOptionInfo(out currentVertion);
			this.lbUserName.text = data.userName;
            this.lbVersion.text = currentVertion;
            this.toggleAutoSit.value = option.isAutoSitdown ? 1 : 0;
            this.toggleSoundBackground.value = option.isEnableSoundBG? 1 : 0;
            this.toggleSoundEffect.value = option.isEnableSoundEffect ? 1 : 0;
            this.toggleLockScreen.value = option.isAutoLockScreen? 1 : 0;
		}

		void onBtnLogoutClick (GameObject go)
		{
            API.Client.APIGeneric.LoginOut(null);
            SocialService.SocialLogout();
		}

		void onToggleAutoSit (GameObject go)
		{
			if (toggleAutoSit.value == 0)
                toggleAutoSit.value = 1;
			else
                toggleAutoSit.value = 0;
            toggleAutoSit.ForceUpdate();
            option.isAutoSitdown = toggleAutoSit.value == 1 ? true : false;
            Puppet.API.Client.APIGeneric.ChangeOptionInfo(option);
		}

		void onToggleSoundBackground (GameObject go)
		{
            if (toggleSoundBackground.value == 0)
                toggleSoundBackground.value = 1;
			else
                toggleSoundBackground.value = 0;
            toggleSoundBackground.ForceUpdate();
            option.isEnableSoundBG = toggleSoundBackground.value == 1 ? true : false;
            Puppet.API.Client.APIGeneric.ChangeOptionInfo(option);

            SoundManager.MuteMusic(!option.isEnableSoundBG);
		}

		void onToggleSoundEffect (GameObject go)
		{
            if (toggleSoundEffect.value == 0)
                toggleSoundEffect.value = 1;
			else
                toggleSoundEffect.value = 0;
            toggleSoundEffect.ForceUpdate();
            option.isEnableSoundEffect = toggleSoundEffect.value == 1 ? true : false;
            Puppet.API.Client.APIGeneric.ChangeOptionInfo(option);

            SoundManager.MuteSFX(!option.isEnableSoundEffect);
		}

		void onToggleLockScreen (GameObject go)
		{
            if (toggleLockScreen.value == 0)
                toggleLockScreen.value = 1;
			else
                toggleLockScreen.value = 0;
            
            toggleLockScreen.ForceUpdate();
            option.isAutoLockScreen = toggleLockScreen.value == 1 ? true : false;
            Puppet.API.Client.APIGeneric.ChangeOptionInfo(option);

            Screen.sleepTimeout = option.isAutoLockScreen ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
		}

        public PuGameOption option { get; set; }
    }
	public class DialogSetting : AbstractDialogData {
		public string userName;
		public string version;
		public DialogSetting(string userName,string version) : base(){
			this.userName = userName;
			this.version = version;
		}
		public override void ShowDialog ()
		{
			DialogSettingsView.Instance.ShowDialog (this);
		}
	}
}
