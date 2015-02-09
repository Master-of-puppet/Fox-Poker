using UnityEngine;
using System.Collections;

namespace Puppet.Service{
	[PrefabAttribute(Name = "Prefabs/Dialog/DialogSettings", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
	public class DialogSettingsView : BaseDialog<DialogSetting,DialogSettingsView> {
		#region Unity Editor
		public UILabel lbUserName, lbVersion;
		public GameObject btnLogout;
        public UIProgressBar toggleAutoSit,toggleAutoBuy,togglePlayNotification,toggleGameNotification;
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
            UIEventListener.Get(toggleAutoBuy.gameObject).onClick += onToggleAutoBuy;
            UIEventListener.Get(togglePlayNotification.gameObject).onClick += onTogglePlayNotification;
            UIEventListener.Get(toggleGameNotification.gameObject).onClick += onToggleGameNotification;
			
		}
		protected override void  OnDisable(){
			base.OnDisable();
			UIEventListener.Get (btnLogout).onClick -= onBtnLogoutClick;
            UIEventListener.Get(toggleAutoSit.gameObject).onClick -= onToggleAutoSit;
            UIEventListener.Get(toggleAutoBuy.gameObject).onClick -= onToggleAutoBuy;
            UIEventListener.Get(togglePlayNotification.gameObject).onClick -= onTogglePlayNotification;
            UIEventListener.Get(toggleGameNotification.gameObject).onClick -= onToggleGameNotification;
		}
		void InitData (DialogSetting data)
		{
			this.lbUserName.text = data.userName;
			this.lbVersion.text = data.version;

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
		}

		void onToggleAutoBuy (GameObject go)
		{
			if (toggleAutoBuy.value == 0)
                toggleAutoBuy.value = 1;
			else
                toggleAutoBuy.value = 0;
            toggleAutoBuy.ForceUpdate();
		}

		void onTogglePlayNotification (GameObject go)
		{
			if (togglePlayNotification.value == 0)
                togglePlayNotification.value = 1;
			else
                togglePlayNotification.value = 0;
            togglePlayNotification.ForceUpdate();
		}

		void onToggleGameNotification (GameObject go)
		{
			if (toggleGameNotification.value == 0)
                toggleGameNotification.value = 1;
			else
                toggleGameNotification.value = 0;
            toggleGameNotification.ForceUpdate();
		}
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
