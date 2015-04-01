using System;

namespace Puppet.Service
{
	public abstract class SocialNetwork : ISocialNetwork
	{
	    public abstract string AccessToken { get; }
        public abstract string AppId { get; }
	    public virtual string UserId { get; set; }
	    public abstract bool IsLoggedIn { get; }

	    public abstract void SocialInit();
	    public abstract void SocialLogin();
	    public abstract void SocialLogout();
	    public abstract void checkPublishPermission(Action<bool> onGetFinishPermission);
	    public abstract void Publish(string content, string url, Action<bool> onShareComplete);
        public abstract void AppRequest(string message, string[] to, string title, Action<bool, string[]> onRequestComplete);
	}
}