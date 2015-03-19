using System;
using System.Collections.Generic;
using UnityEngine;
using Puppet.Utils;

namespace Puppet.Service
{
	public sealed class FacebookHandler : SocialNetwork, ISocialNetwork
	{
		public Dictionary<string, object> friendsDictionary;
	    SocialType type = SocialType.Facebook;

	    public override string AccessToken
	    {
	        get { return FB.AccessToken; }
	    }

	    public override bool IsLoggedIn
	    {
	        get { return FB.IsLoggedIn; }
	    }

	    public override void SocialInit()
	    {
	        FB.Init(() => OnInit(), (bool isGameShown) => { if (!isGameShown) Time.timeScale = 0; else Time.timeScale = 1; });
	    }

	    public override void SocialLogin()
	    {
	        FB.Login("email,publish_actions", (FBResult response) => {
	            OnLoginCompleted(IsLoggedIn); 
	        });
	    }

	    public override void SocialLogout()
	    {
	        if (IsLoggedIn)
	            FB.Logout();
	    }

	    public override void checkPublishPermission(Action<bool> onGetFinishPermission)
	    {
	        if (UserId != null)
	        {
	            Logger.Log("Social id " + UserId);
	            FB.API(UserId + "/permissions", Facebook.HttpMethod.GET, getPermissionFacebookCallback);
	            this.callbackGetPermission = onGetFinishPermission;
	        }
	        else
	            onGetFinishPermission(false);
	    }
	    Action<bool> callbackGetPermission;
	    void getPermissionFacebookCallback(FBResult response)
	    {
	        bool hasPermission = false;
	        if (response != null)
	        {
	            if (!string.IsNullOrEmpty(response.Text))
	            {
	                Dictionary<string, object> permission = (Dictionary<string, object>)JsonUtil.Deserialize(response.Text);
	                List<object> resultSet = permission["data"] as List<object>;
	                Dictionary<string, object> detail = resultSet[0] as Dictionary<string, object>;

	                if (detail.ContainsKey("publish_actions"))
	                    hasPermission = Convert.ToBoolean(detail["publish_actions"]);
	            }
	        }
	        else
	            Logger.Log("Fb response is null");

	        if (callbackGetPermission != null)
	            callbackGetPermission(hasPermission);
	    }

	    public override void Publish(string content, string url, Action<bool> onShareComplete)
	    { 
	        FB.Feed(link: url, linkDescription: content, callback: (FBResult result) => {
	            if (onShareComplete != null)
	                onShareComplete(string.IsNullOrEmpty(result.Error));
	        });
	    }

        public override void AppRequest(string message, string[] to, string title, Action<bool, string[]> onRequestComplete)
        {
            FB.AppRequest(message, to, null, null, null, null, title, (FBResult result) => {

                bool status = false;
                List<string> requestIds = new List<string>();

                if (result != null && !string.IsNullOrEmpty(result.Text))
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)JsonUtil.Deserialize(result.Text);
                    if(dict.ContainsKey("to"))
                    {
                        status = true;
                        List<object> list = dict["to"] as List<object>;
                        foreach(object o in list)
                            requestIds.Add(o.ToString());
                    }
                }

                if (onRequestComplete != null)
                    onRequestComplete(status, requestIds.ToArray());
            });
        }

	    private void OnInit()
	    {
	        Logger.Log("FB Init completed");
	        SocialService.Instance.DispathInitComplete(type);
	        if (!string.IsNullOrEmpty(AccessToken))
	        {
	            getCurrentUser();
	        }
	    }

	    void OnLoginCompleted(bool isSuccess)
	    {
            Logger.Log("FB Login: " + isSuccess);
	        if (isSuccess)
	        {
	            if (!string.IsNullOrEmpty(AccessToken))
	                getCurrentUser();
	            else
	                SocialService.Instance.DispathAccessTokenNotFound(type);
	        }
	        else
	            SocialService.Instance.SetOnLoginComplete(type, false);

	    }

	    void getCurrentUser()
	    {
	        FB.API("me", Facebook.HttpMethod.GET, (FBResult response) =>
	        {
	            if (response != null && !string.IsNullOrEmpty(response.Text))
	            {
	                Dictionary<string, object> profile = (Dictionary<string, object>)JsonUtil.Deserialize(response.Text);
	                UserId = profile["id"].ToString();
	            }
	            SocialService.Instance.SetOnLoginComplete(type, true);
	        });
	    }
	}
}