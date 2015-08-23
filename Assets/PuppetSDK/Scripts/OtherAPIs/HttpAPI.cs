using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Puppet.Core;
using UnityEngine;

namespace Puppet
{
    public class HttpAPI
    {
        #region Send Tracking for Campaign Ads
        public static void TrackCampaign(MonoBehaviour behaviour, DelegateAPICallback callback)
        {
            behaviour.StartCoroutine(_TrackCampaign(callback));
        }

        static IEnumerator _TrackCampaign(DelegateAPICallback callback)
        {
            string uniqueId = DeviceInfoService.Instance.DeviceUniqueId;
            string device_imei = DeviceInfoService.Instance.IMEI;

            Debug.Log("UniqueID : " + uniqueId);
            Debug.Log("DeviceIMEI : " + device_imei);

            WWWForm postForm = new WWWForm();
            postForm.AddField("imei", device_imei);
            postForm.AddField("device_id", uniqueId);
            postForm.AddField("utm_campaign", string.Empty);
            postForm.AddField("utm_source", string.Empty);
            postForm.AddField("utm_medium", string.Empty);
            postForm.AddField("utm_term", string.Empty);
            postForm.AddField("utm_content", string.Empty);
            postForm.AddField("gclid", string.Empty);

            string trackId = Utility.ReadData.GetTrackId();
            if(!string.IsNullOrEmpty(trackId))
                postForm.AddField("track_id", trackId);

            WWW www = new WWW("http://mobileasia.vn/api/receiver.php", postForm);
            yield return www;

            bool status = string.IsNullOrEmpty(www.error);
            string message = string.IsNullOrEmpty(www.error) ? www.text : www.error;

            if (callback != null)
                callback(status, message);
        }
        #endregion


        #region ChangeUseAvatar
        public static void ChangeUseAvatar(MonoBehaviour behaviour, byte[] avatar, DelegateAPICallback callback)
        {
            if (avatar == null)
            {
                if (callback != null) callback(false, "ERROR: Không có dữ liệu về avatar mới.");
                return;
            }

            behaviour.StartCoroutine(UploadFile(avatar, callback));
        }

        static IEnumerator UploadFile(byte[] avatar, DelegateAPICallback callback)
        {
            WWWForm postForm = new WWWForm();
            postForm.AddField("accessToken", API.Client.APILogin.AccessToken);
            postForm.AddBinaryData("avatar", avatar);
            WWW upload = new WWW(string.Format("{0}/static/api/uploadAvatar", AppConfig.HttpUrl), postForm);
            yield return upload;

            bool status = string.IsNullOrEmpty(upload.error);
            string message = string.IsNullOrEmpty(upload.error) ? upload.text : upload.error;
            
            if(status)
            {
                Dictionary<string, object> dict = Puppet.Utils.JsonUtil.Deserialize(message);
                if (dict != null && dict.ContainsKey("code"))
                    status = int.Parse(dict["code"].ToString()) == 0;

                if (dict != null && dict.ContainsKey("message"))
                    message = dict["message"].ToString();
            }

            if (callback != null)
                callback(status, message);
        }
        #endregion
    }
}
