using UnityEngine;
using System.Collections;
using Puppet.Service;
using Puppet;
namespace Puppet.Service
{
    [PrefabAttribute(Name = "Prefabs/Dialog/DialogHelp", Depth = 7, IsAttachedToCamera = true, IsUIPanel = true)]
    public class DialogHelpView : BaseDialog<DialogHelp, DialogHelpView>
    {
        #region UnityEditor
        public UIToggle btnFAQ, btnRule, btnExp, btnFeedBack;
        public UISprite foreground;
        public GameObject contentFeedBack, contentwebView;
        #endregion
#if UNITY_ANDROID || UNITY_IOS
       
        protected override void OnEnable()
        {
            base.OnEnable();
            EventDelegate.Add(btnFAQ.onChange, OnBtnFAQChanged);
            EventDelegate.Add(btnRule.onChange, OnBtnRuleChanged);
            EventDelegate.Add(btnExp.onChange, OnBtnEXPChanged);
            EventDelegate.Add(btnFeedBack.onChange, OnBtnFeedBackChanged);
            initEventWebView();
//			SetWebView ();
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
            EventDelegate.Remove(btnFAQ.onChange, OnBtnFAQChanged);
            EventDelegate.Remove(btnRule.onChange, OnBtnRuleChanged);
            EventDelegate.Remove(btnExp.onChange, OnBtnEXPChanged);
            EventDelegate.Remove(btnFeedBack.onChange, OnBtnFeedBackChanged);
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
        private void OnBtnFeedBackChanged()
        {
            if (btnFeedBack.value)
            {
                if (!contentFeedBack.activeSelf)
                {
                    contentFeedBack.SetActive(true);
                    removeEventWebView();
                    GameObject.Destroy(contentwebView.GetComponent<UniWebView>());
                    
                }
            }
        }
        private void OnBtnFAQChanged()
        {
            if (btnFAQ.value)
            {
                if (contentwebView.GetComponent<UniWebView>() == null)
                {
                    contentFeedBack.SetActive(false);
                    contentwebView.AddComponent<UniWebView>();
                    initEventWebView();
                    
                }
                contentwebView.GetComponent<UniWebView>().url = "http://vnexpress.net/";
                contentwebView.GetComponent<UniWebView>().Load();
            }
        }
        private void OnBtnRuleChanged()
        {
            if (btnRule.value)
            {
                if (contentwebView.GetComponent<UniWebView>() == null)
                {
                    contentFeedBack.SetActive(false);
                    contentwebView.AddComponent<UniWebView>();
                    initEventWebView();

                }
                contentwebView.GetComponent<UniWebView>().url = "http://www.24h.com.vn/";
                contentwebView.GetComponent<UniWebView>().Load();
            }
        }
        private void OnBtnEXPChanged()
        {
            if (btnExp.value)
            {
                if (contentwebView.GetComponent<UniWebView>() == null)
                {
                    contentFeedBack.SetActive(false);
                    contentwebView.AddComponent<UniWebView>();
                    initEventWebView();

                }
                contentwebView.GetComponent<UniWebView>().url = "http://www.baomoi.com/";
                contentwebView.GetComponent<UniWebView>().Load();
            }
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
        public override void ShowDialog(DialogHelp data)
        {
            base.ShowDialog(data);
#if UNITY_ANDROID || UNITY_IOS
            SetWebView();
            btnFAQ.value = true;
#endif
        }
    }
    public class DialogHelp : AbstractDialogData
    {

        public override void ShowDialog()
        {
            DialogHelpView.Instance.ShowDialog(this);
        }
    }
}