package com.ensign.browseimagefromgallery;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

/**
 * @author vietdungvn88@gmail.com
 *
 */
public class ImageAPI {
	public static String objectName;
	public static String methodName;
	public static String fileName;

	public static final String TAG = "SmartLog - ImageAPI";

	private Activity activity;

	public ImageAPI(Activity currentActivity) {
		Log.i(TAG, "ImageAPI - Constructor called with currentActivity = "
				+ currentActivity);
		this.activity = currentActivity;
	}

	public static ImageAPI getInstance(Activity currentActivity) {
		Log.i(TAG, "ImageAPI - getInstance");
		return new ImageAPI(currentActivity);
	}

	public void dungnv_GetImageFromGallery(String objectName,
			String methodName, String fileName) {
		Log.d(TAG, "ImageAPI - getImageFromGallery ********************: "
				+ objectName + " - " + objectName);
		ImageAPI.objectName = objectName;
		ImageAPI.methodName = methodName;
		ImageAPI.fileName = fileName;

		Intent intent = new Intent(activity, MainActivity.class);
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK
				| Intent.FLAG_ACTIVITY_MULTIPLE_TASK);
		activity.getApplicationContext().startActivity(intent);
	}
}
