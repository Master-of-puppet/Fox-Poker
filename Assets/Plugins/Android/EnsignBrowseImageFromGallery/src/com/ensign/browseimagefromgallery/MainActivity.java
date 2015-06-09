package com.ensign.browseimagefromgallery;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.InputStream;

import android.app.Activity;
import android.content.ActivityNotFoundException;
import android.content.Context;
import android.content.ContextWrapper;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.util.Log;
import android.widget.Toast;

/**
 * @author vietdungvn88@gmail.com
 *
 */
public class MainActivity extends Activity {

	private final int BROWSE_IMAGE_CODE = 1306;
	public static final String TAG = "SmartLog - MainActivity";
	private static final int RESULT_CROP = 1602;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		Intent intent = new Intent();
		intent.setType("image/*");

		intent.setAction(Intent.ACTION_GET_CONTENT);
		intent.putExtra("crop", "true");
		intent.putExtra("aspectX", 0);
		intent.putExtra("aspectY", 0);
		intent.putExtra("outputX", 300);
		intent.putExtra("outputY", 300);
		intent.putExtra("return-data", true);
		startActivityForResult(Intent.createChooser(intent, "Browse Image"),
				BROWSE_IMAGE_CODE);
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		super.onActivityResult(requestCode, resultCode, data);
		String imagePath = "";

		if (requestCode == BROWSE_IMAGE_CODE) {
			if (resultCode == Activity.RESULT_OK) {
				Bundle extras2 = data.getExtras();
				if (extras2 != null) {
					Bitmap photo = extras2.getParcelable("data");
					//Log.i(TAG, "==============> " + photo);
					imagePath = saveToInternalSorage(photo);
				}
				// String choosenImagePath= data.getStringExtra("picturePath");
				// // perform Crop on the Image Selected from Gallery
				// performCrop(choosenImagePath);
			}
			// Bundle extras = imageReturnedIntent.getExtras();
			// Uri selectedImage = imageReturnedIntent.getData();
			// InputStream imageStream = null;
			// try {
			// imageStream =
			// getContentResolver().openInputStream(selectedImage);
			// } catch (FileNotFoundException e) {
			// // TODO Auto-generated catch block
			// e.printStackTrace();
			// }
			// Bitmap yourSelectedImage = BitmapFactory
			// .decodeStream(imageStream);
			//
			// imagePath = saveToInternalSorage(yourSelectedImage);
			// Log.i(TAG, "Save to: "
			// + saveToInternalSorage(yourSelectedImage));
		}
		// if (requestCode == RESULT_CROP ) {
		// if(resultCode == Activity.RESULT_OK){
		// Bundle extras = data.getExtras();
		// Bitmap selectedBitmap = extras.getParcelable("data");
		// // Set The Bitmap Data To ImageView
		// imagePath = saveToInternalSorage(selectedBitmap);
		// }
	
		//
		// }
		 SendMessageManager sendMessage = new SendMessageManager();
		 sendMessage.unitySendMessage(ImageAPI.objectName,
		 ImageAPI.methodName,
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
//			bitmapImage.compress(Bitmap.CompressFormat.JPEG, 75, fos);
			bitmapImage.compress(Bitmap.CompressFormat.PNG, 100, fos);
			fos.flush();
			fos.close();
		} catch (Exception e) {
			e.printStackTrace();
		}
		return mypath.getAbsolutePath();
	}

	private void performCrop(String picUri) {
		try {
			// Start Crop Activity

			Intent cropIntent = new Intent("com.android.camera.action.CROP");
			// indicate image type and Uri
			File f = new File(picUri);
			Uri contentUri = Uri.fromFile(f);

			cropIntent.setDataAndType(contentUri, "image/*");
			// set crop properties
			cropIntent.putExtra("crop", "true");
			// indicate aspect of desired crop
			cropIntent.putExtra("aspectX", 1);
			cropIntent.putExtra("aspectY", 1);
			// indicate output X and Y
			cropIntent.putExtra("outputX", 280);
			cropIntent.putExtra("outputY", 280);

			// retrieve data on return
			cropIntent.putExtra("return-data", true);
			// start the activity - we handle returning in onActivityResult
			startActivityForResult(cropIntent, RESULT_CROP);
		}
		// respond to users whose devices do not support the crop action
		catch (ActivityNotFoundException anfe) {
			// display an error message
			String errorMessage = "your device doesn't support the crop action!";
			Toast toast = Toast
					.makeText(this, errorMessage, Toast.LENGTH_SHORT);
			toast.show();
		}
	}
}
