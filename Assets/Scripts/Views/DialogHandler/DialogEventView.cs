using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet;
using System;
namespace Puppet.Service
{
	[PrefabAttribute(Name = "Prefabs/Dialog/DialogEvent", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
	public class DialogEventView : BaseDialog<DialogEvent, DialogEventView>
    {
        #region UnityEditor
        public UISprite foreground;
        public GameObject contentwebView;
        public GameObject TopLeft, BottomRight;
        #endregion
        int aTop, aLeft, aBottom, aRight;

#if UNITY_ANDROID || UNITY_IOS
       
        protected override void OnEnable()
        {
            base.OnEnable();
            initEventWebView();
			//SetWebView ();
        }
        private void initEventWebView()
        {
            contentwebView.GetComponent<UniWebView>().OnReceivedMessage += OnReceivedMessage;
            contentwebView.GetComponent<UniWebView>().OnLoadComplete += OnLoadComplete;
            contentwebView.GetComponent<UniWebView>().OnWebViewShouldClose += OnWebViewShouldClose;
            contentwebView.GetComponent<UniWebView>().OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
        }
        private void removeEventWebView() {
            if (contentwebView.GetComponent<UniWebView>() != null)
            {
                contentwebView.GetComponent<UniWebView>().OnReceivedMessage -= OnReceivedMessage;
                contentwebView.GetComponent<UniWebView>().OnLoadComplete -= OnLoadComplete;
                contentwebView.GetComponent<UniWebView>().OnWebViewShouldClose -= OnWebViewShouldClose;
                contentwebView.GetComponent<UniWebView>().OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
            }
        }



        protected override void OnDisable()
        {
            base.OnDisable();
            removeEventWebView();
        }
        

		private void SetWebView()
        {
            if (!String.IsNullOrEmpty(data.url)) { 
                contentwebView.GetComponent<UniWebView>().url = data.url;
                contentwebView.GetComponent<UniWebView>().Load();
            }
            contentwebView.GetComponent<UniWebView>().insets = new UniWebViewEdgeInsets(aTop, aLeft, aBottom, aRight);
        }

        private void OnEvalJavaScriptFinished(UniWebView webView, string result)
        {
            throw new System.NotImplementedException();
        }

        bool OnWebViewShouldClose(UniWebView webView)
        {
            if (this.contentwebView.GetComponent<UniWebView>() == webView)
            {
                GameObject.Destroy(gameObject);
                return true;
            }
            return false;
        }

        private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
        {
            if (success)
            {
                webView.Show();
            }
            else
            {
                Debug.Log("Something wrong in webview loading: " + errorMessage);
                //_errorMessage = errorMessage;
            }
        }

        private void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
        {
            throw new System.NotImplementedException();
        }
#endif

        public override void ShowDialog(DialogEvent data)
        {
            base.ShowDialog(data);
            StartCoroutine(StartShowDialog());
        }

        IEnumerator StartShowDialog()
        {
            yield return new WaitForEndOfFrame();
            Vector3 topLeft = UICamera.mainCamera.WorldToScreenPoint(TopLeft.transform.position);
            Vector3 bottomRight = UICamera.mainCamera.WorldToScreenPoint(BottomRight.transform.position);
            aTop = Screen.height - Mathf.FloorToInt(topLeft.y);
            aLeft = Mathf.FloorToInt(topLeft.x);
            aBottom = Mathf.FloorToInt(bottomRight.y);
            aRight = Screen.width - Mathf.FloorToInt(bottomRight.x);
#if UNITY_ANDROID || UNITY_IOS
            SetWebView();
#endif
        }
    }
    public class DialogEvent : AbstractDialogData
    {
        public string url;
        public DialogEvent(string url) {
            this.url = url;
        }
        public override void ShowDialog()
        {
            DialogEventView.Instance.ShowDialog(this);
        }
    }
}