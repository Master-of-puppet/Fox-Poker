using UnityEngine;
using System;

namespace Puppet.Service
{
	public enum SocialType
	{
	    Facebook,
	    GooglePlus
	}

	public class SocialService : Singleton<SocialService>
	{
		protected override void Init ()
		{

		}
	    ISocialNetwork facebook, google;

	    #region SOCIAL EVENT
	    /// <summary>
	    /// Event arises when initializing social networking success
	    /// </summary>
	    public event Action<SocialType> onInitComplete;
	    /// <summary>
	    /// Event arises when successfully logged into the social networking
	    /// </summary>
	    public event Action<SocialType, bool> onLoginComplete;
	    /// <summary>
	    /// Event arises when you log into social but could not get accessToken
	    /// </summary>
	    public event Action<SocialType> onAccessTokenNotFound;
	    /// <summary>
	    /// Event arises when logging out
	    /// </summary>
	    public event Action<SocialType> onLogout;

	    internal void SetOnLoginComplete(SocialType type, bool value)
	    {
	        if (onLoginComplete != null)
	            onLoginComplete(type, value);
	    }
	    internal void DispathAccessTokenNotFound(SocialType type)
	    {
	        if (onAccessTokenNotFound != null)
	            onAccessTokenNotFound(type);
	    }
	    internal void DispathInitComplete(SocialType type)
	    {
	        if (onInitComplete != null)
	            onInitComplete(type);
	    }
	    #endregion

	    void Awake()
	    {
	        facebook = new FacebookHandler();
	    }

	    public static ISocialNetwork GetSocialNetwork(SocialType type)
	    {
	        return type == SocialType.Facebook ? Instance.facebook : Instance.google;
	    }

	    /// <summary>
	    /// Start using social
	    /// </summary>
	    public static void SocialStart()
	    {
	        Instance.facebook.SocialInit();
	    }

	    public static void SocialLogout()
	    {
	        SocialLogout(SocialType.Facebook);
	        SocialLogout(SocialType.GooglePlus);
	    }

	    public static void SocialLogout(SocialType type)
	    {
	        if (Instance.onLogout != null)
	            Instance.onLogout(type);

	        GetSocialNetwork(type).SocialLogout();
	    }

	    public static void SocialLogin(SocialType type)
	    {
	        GetSocialNetwork(type).SocialLogin();
	    }

	    /// <summary>
	    /// Check publish permission. 
	    /// - If yes, will do something
	    /// - If no, will login
	    /// </summary>
	    /// <param name="type">Type of social network will publish permission</param>
	    /// <param name="action">Method Execute when is already have or not permission to publish</param>
	    public static void checkPublishPermission(SocialType type, Action<bool> action)
	    {
	        GetSocialNetwork(type).checkPublishPermission((bool isHasPermission) =>
	        {
	            if (!isHasPermission)
	            {
	                if (!string.IsNullOrEmpty(GetSocialNetwork(type).AccessToken))
	                    GetSocialNetwork(type).SocialLogout();
	                GetSocialNetwork(type).SocialLogin();
	            }
	            action(isHasPermission);
	        });
	    }
	}
}