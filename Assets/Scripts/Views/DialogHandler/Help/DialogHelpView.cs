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
        public GameObject TopLeft, BottomRight;
        #endregion
        int aTop, aLeft, aBottom, aRight;
#if UNITY_ANDROID || UNITY_IOS
       
        protected override void OnEnable()
        {
            base.OnEnable();
            EventDelegate.Add(btnFAQ.onChange, OnBtnFAQChanged);
            EventDelegate.Add(btnRule.onChange, OnBtnRuleChanged);
            EventDelegate.Add(btnExp.onChange, OnBtnEXPChanged);
            EventDelegate.Add(btnFeedBack.onChange, OnBtnFeedBackChanged);
            initEventWebView();
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

            contentwebView.GetComponent<UniWebView>().insets = new UniWebViewEdgeInsets(aTop, aLeft, aBottom, aRight);

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
			StartCoroutine(StartShowDialog());
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