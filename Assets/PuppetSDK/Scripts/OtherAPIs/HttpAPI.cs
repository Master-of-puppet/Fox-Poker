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
    }
}
