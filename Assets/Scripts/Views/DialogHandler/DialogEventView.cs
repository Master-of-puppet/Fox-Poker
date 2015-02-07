using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet;
namespace Puppet.Service
{
	[PrefabAttribute(Name = "Prefabs/Dialog/DialogEvent", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
	public class DialogEventView : BaseDialog<DialogEvent, DialogEventView>
    {
        #region UnityEditor
        public UISprite foreground;
        public GameObject contentwebView;
        #endregion
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

            //int uiFactor = UniWebViewHelper.RunningOnRetinaIOS() ? 2 : 1;
            int uiFactor = 1;
            UIRoot mRoot = NGUITools.FindInParents<UIRoot>(gameObject);
            float ratioHeight = ((float)mRoot.activeHeight / UniWebViewHelper.screenHeight) * uiFactor;
            float ratioWidth = ((float)mRoot.manualWidth / UniWebViewHelper.screenWidth) * uiFactor;
            int width = Mathf.FloorToInt(UniWebViewHelper.screenWidth * ratioWidth / uiFactor);
            int height = Mathf.FloorToInt(UniWebViewHelper.screenHeight * ratioHeight / uiFactor);

            int webMarginWidth = Mathf.FloorToInt(width - (foreground.width));
            int webMarginHeight = Mathf.FloorToInt(height - (foreground.height));

            int leftRight = Mathf.FloorToInt(webMarginWidth / (2 * ratioWidth));

            int topbottom = Mathf.RoundToInt((webMarginHeight / (2 * ratioHeight)));
            contentwebView.GetComponent<UniWebView>().insets = new UniWebViewEdgeInsets(Mathf.RoundToInt((topbottom +130) * ratioHeight), leftRight, Mathf.RoundToInt((topbottom - 20 ) * ratioHeight), leftRight);

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
#if UNITY_ANDROID || UNITY_IOS
            SetWebView();
#endif
        }
    }
    public class DialogEvent : AbstractDialogData
    {

        public override void ShowDialog()
        {
            DialogEventView.Instance.ShowDialog(this);
        }
    }
}