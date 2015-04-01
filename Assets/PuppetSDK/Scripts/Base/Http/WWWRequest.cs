using Puppet.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Puppet.Core.Network.Http
{
    public class WWWRequest : IHttpRequest
    {
        Action<IHttpRequest, float> _onProgress;
        Action<IHttpRequest, IHttpResponse> _onResponse;
        IServerMode serverMode;
        WWWResponse dataResponse;

        float timeOut;
        HttpMethod method = HttpMethod.Get;

        float originTimeOut;

        public string Path;
        public Hashtable Header;
        public Dictionary<string, object> PostData;
        public int RetryCount;
        public float WaitForProgress = 0.01f;
        public bool EnableTimeOut;
        public bool isFullUrl;
        MonoBehaviour monoBehaviour;

        public WWWRequest(MonoBehaviour monoBehaviour, string _path, float _timeOut, int _retryCount)
        {
            this.monoBehaviour = monoBehaviour;
            this.Path = _path;
            this.TimeOut = _timeOut;
            this.originTimeOut = _timeOut;
            this.RetryCount = _retryCount;
            this.EnableTimeOut = timeOut > 0;
        }

        public void Start(IServerMode server)
        {
            dataResponse = new WWWResponse();
            this.serverMode = server;
            monoBehaviour.StartCoroutine(DownloadData());
        }

        IEnumerator DownloadData()
        {
            string url = isFullUrl ? Path : serverMode.GetPath(Path);
            Logger.Log("WWW Download: " + url);
            WWW www;
            if (Method == HttpMethod.Get)
                www = new WWW(url);
            else
            {
                WWWForm form = new WWWForm();
                foreach(string key in PostData.Keys)
                {
                    object obj = PostData[key];
                    if (obj is int)
                        form.AddField(key, (int)obj);
                    else if (obj is string)
                        form.AddField(key, (string)obj);
                    else if (obj is byte[])
                        form.AddBinaryData(key, (byte[])obj);
                    else
                        Logger.Log("Type invalid {0}", key);
                }
                
                www = new WWW(url, form);
            }

            float lastProgress = -1;
            while (www != null && www.progress < 1f && string.IsNullOrEmpty(www.error) && !IsTimeOut)
            {
                if (lastProgress < www.progress)
                {
                    //Only dispatch event onDownloading when have a changing progress
                    lastProgress = www.progress;
                    if (_onProgress != null)
                        _onProgress(this, www.progress);
                }

                yield return new WaitForSeconds(WaitForProgress);

                TimeOut -= WaitForProgress;
            }

            if (IsTimeOut)
            {
                //DOWNLOAD TIMEOUT!!!
                dataResponse.State = HttpStatusCode.RequestTimeout;
            }
            else
            {
                if (string.IsNullOrEmpty(www.error))
                {
                    //Just yield www when successful
                    yield return www;
                    //Set RetryCount to less than or equal 0 for not continue to download more 
                    RetryCount = -1;
                    dataResponse.State = HttpStatusCode.OK;
                }
                else
                    dataResponse.State = HttpStatusCode.BadRequest;
            }
            Dispose(www);
            yield return null;
        }

        public HttpMethod Method
        {
            get { return method; } set { method = value; }
        }

        public float TimeOut
        {
            get { return timeOut; } set { timeOut = value; }
        }

        public Action<IHttpRequest, float> onProgress
        {
            get { return _onProgress; }
            set { _onProgress = value; }
        }

        public Action<IHttpRequest, IHttpResponse> onResponse
        {
            get { return _onResponse; }
            set { _onResponse = value; }
        }

        public void Dispose(WWW www)
        {
            if (--RetryCount > 0)
            {
                //If you still need to download again
                ResetDownload();
                monoBehaviour.StartCoroutine(DownloadData());
            }
            else
            {
                dataResponse.www = www;
                dataResponse.Error = www.error;
                dataResponse.Data = www.text;
                dataResponse.Bytes = www.bytes;

                //When complete download, Whether success or failure
                if (_onResponse != null)
                {
                    _onResponse(this, dataResponse);
                    _onResponse = null;
                }
            }
            www.Dispose();
            www = null;
        }

        void ResetDownload()
        {
            TimeOut = originTimeOut;
        }

        bool IsTimeOut
        {
            get { return EnableTimeOut && TimeOut <= 0; }
        }
    }
}

