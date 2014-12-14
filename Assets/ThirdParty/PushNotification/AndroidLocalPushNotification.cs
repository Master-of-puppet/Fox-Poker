using UnityEngine;
using System.Collections;

public class AndroidLocalPushNotification  {
	private static AndroidJavaObject _localPush;
	
	
	static AndroidLocalPushNotification()
	{
		if( Application.platform != RuntimePlatform.Android )
			return;
		
		// find the plugin instance
		using( var pluginClass = new AndroidJavaClass( "com.puppet.congnt.localpushnotification.PuppetPlugin" ) )
			_localPush = pluginClass.CallStatic<AndroidJavaObject>( "getInstance" );
		
	}

	public static void sendPushNotification(int second,string title,string message){
		if( Application.platform != RuntimePlatform.Android )
			return;
		_localPush.Call( "sendPushNotification",title,message,second);
	}
    public static void clearLocalPushNotification()
    {
        if (Application.platform != RuntimePlatform.Android)
            return;
        _localPush.Call("clearPushNotification");
    }
}
