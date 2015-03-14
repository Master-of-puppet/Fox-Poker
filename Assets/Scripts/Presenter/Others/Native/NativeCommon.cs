using UnityEngine;
using System.Collections;
using System;

public class NativeCommon {
	
	public static void OpenGallery(Action<Texture> takeImageCompleteCallback){
		if (Application.platform == RuntimePlatform.Android) {
			AndroidCamera.instance.OnImagePicked = delegate(AndroidImagePickResult result) {
				if(result.IsSucceeded){
					takeImageCompleteCallback(result.image);
				}else{
					takeImageCompleteCallback(result.image);
				}
			};
			AndroidCamera.instance.GetImageFromGallery();
		}

	}
}
