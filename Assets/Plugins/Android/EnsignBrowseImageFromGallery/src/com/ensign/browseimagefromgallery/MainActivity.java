package com.ensign.browseimagefromgallery;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.InputStream;

import android.app.Activity;
import android.content.Context;
import android.content.ContextWrapper;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;

/**
 * @author vietdungvn88@gmail.com
 *
 */
public class MainActivity extends Activity {

	private final int BROWSE_IMAGE_CODE = 1306;
	public static final String TAG = "SmartLog - MainActivity";

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		Intent intent = new Intent();
		intent.setType("image/*");
		intent.setAction(Intent.ACTION_GET_CONTENT);
		startActivityForResult(Intent.createChooser(intent, "Browse Image"),
				BROWSE_IMAGE_CODE);
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode,
			Intent imageReturnedIntent) {

		super.onActivityResult(requestCode, resultCode, imageReturnedIntent);

		String imagePath = "";
		if (resultCode == RESULT_OK) {

			if (requestCode == BROWSE_IMAGE_CODE) {

				Uri selectedImage = imageReturnedIntent.getData();

				InputStream imageStream = null;
				try {
					imageStream = getContentResolver().openInputStream(
							selectedImage);
				} catch (FileNotFoundException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
				Bitmap yourSelectedImage = BitmapFactory
						.decodeStream(imageStream);

				imagePath = saveToInternalSorage(yourSelectedImage);
				Log.i(TAG, "Save to: "
						+ saveToInternalSorage(yourSelectedImage));
			}
		}

		SendMessageManager sendMessage = new SendMessageManager();
		sendMessage.unitySendMessage(ImageAPI.objectName, ImageAPI.methodName,
				imagePath);

		this.finish();
	}

	private String saveToInternalSorage(Bitmap bitmapImage) {
		ContextWrapper cw = new ContextWrapper(getApplicationContext());
		// path to /data/data/yourapp/app_data/imageDir
		File directory = cw.getDir("ensign", Context.MODE_WORLD_WRITEABLE);
		// Create imageDir
		File mypath = new File(directory, ImageAPI.fileName);

		FileOutputStream fos = null;
		try {

			fos = new FileOutputStream(mypath);

			// Use the compress method on the BitMap object to write image to
			// the OutputStream
			bitmapImage.compress(Bitmap.CompressFormat.PNG, 100, fos);
			fos.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
		return mypath.getAbsolutePath();
	}
}
