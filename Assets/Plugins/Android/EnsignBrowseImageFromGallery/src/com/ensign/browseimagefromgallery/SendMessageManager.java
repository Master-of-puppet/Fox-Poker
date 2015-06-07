package com.ensign.browseimagefromgallery;

import android.util.Log;

import com.unity3d.player.UnityPlayer;

/**
 * @author vietdungvn88@gmail.com
 *
 */
public class SendMessageManager {
	public SendMessageManager() {
	}

	public SendMessageManager(String gameObjectName, String methodName,
			String param) {
		this.unitySendMessage(gameObjectName, methodName, param);
	}

	/**
	 * Send message to unity
	 * 
	 * @param gameObjectName
	 * @param methodName
	 * @param param
	 */
	public void unitySendMessage(String gameObjectName, String methodName,
			String param) {
		UnityPlayer.UnitySendMessage(gameObjectName, methodName, param);
		Log.i("SendMessageManager", "Sent to gameObject='" + gameObjectName
				+ "' - method='" + methodName + "' - param: " + param);
	}
}