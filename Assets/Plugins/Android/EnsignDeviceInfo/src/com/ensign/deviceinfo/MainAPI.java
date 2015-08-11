package com.ensign.deviceinfo;

import android.app.Activity;
import android.content.Context;
import android.telephony.TelephonyManager;

/**
 * 
 * @author Nguyen Viet Dung
 *
 */
public class MainAPI {
	public static final String TAG = "SmartLog - Plugins DeviceInfo";
	private Activity activity;
	
	public MainAPI(Activity currentActivity) {
		this.activity = currentActivity;
	}
	
	public static MainAPI getInstance(Activity currentActivity)
	{
		return new MainAPI(currentActivity);
	}
	
	/**
	 * Get IMEI for GSM
	 * @return
	 */
	public String getDeviceId()
	{
		TelephonyManager telephonyManager = (TelephonyManager)activity.getSystemService(Context.TELEPHONY_SERVICE);
		return telephonyManager.getDeviceId();
	}
	
	public String getSimSerialNumber()
	{
		TelephonyManager telephonyManager = (TelephonyManager)activity.getSystemService(Context.TELEPHONY_SERVICE);
		return telephonyManager.getSimSerialNumber();
	}
	
	/**
	 * Get Current Your Phone Numbers
	 * @return
	 */
	public String getPhoneNumber()
	{
		TelephonyManager telephonyManager = (TelephonyManager)activity.getSystemService(Context.TELEPHONY_SERVICE);
		return telephonyManager.getLine1Number();
	}
	
	public boolean isSimSupport()
    {
        TelephonyManager tm = (TelephonyManager) activity.getSystemService(Context.TELEPHONY_SERVICE);
        if (tm.getSimState() == TelephonyManager.SIM_STATE_ABSENT){
            return false;
        } else {
            return true;
        }
    }
}
