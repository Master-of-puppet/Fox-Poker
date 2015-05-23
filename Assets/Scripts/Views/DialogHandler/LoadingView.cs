// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using Puppet.Service;
using Puppet;
using UnityEngine;

[PrefabAttribute(Name = "Prefabs/Dialog/DialogLoading", Depth = 100, IsUIPanel = true)]
public class LoadingView : SingletonPrefab<LoadingView>
{
	#region UnityEditor
	public UISprite transparentLeft, transparentRight;
	public UI2DSprite loadingIcon;
	#endregion
	private bool isWillClose = false;

    public UICamera camera;

    void OnLevelWasLoaded(int level)
    {
        HandleInput();
    }

    void HandleInput()
    {
        if(camera == null)
            camera = GameObject.FindObjectOfType<UICamera>();

        if (camera != null)
        {
            camera.useTouch = !loadingIcon.gameObject.activeSelf;
            camera.useMouse = !loadingIcon.gameObject.activeSelf;
            camera.useKeyboard = !loadingIcon.gameObject.activeSelf;
            camera.useController = !loadingIcon.gameObject.activeSelf;
        }
    }

	void CalculatorTwoBackground()
    {
        iTween.Stop(transparentLeft.gameObject);
        iTween.Stop(transparentRight.gameObject);

        loadingIcon.gameObject.SetActive(true);
        transparentLeft.gameObject.SetActive(true);
        transparentRight.gameObject.SetActive(true);
        transparentLeft.transform.localPosition = new Vector3(-20f, 0f, 0f);
        transparentRight.transform.localPosition = new Vector3(20f, 0f, 0f);
		StartTranslate ();

        HandleInput();
	}

	public void Show(bool isWillShow = true)
    {
        this.isWillClose = !isWillShow;

        if (isWillShow)
        {
            Invoke("CheckTimeOut", 15f);
            CalculatorTwoBackground();
        }
        else
        {
            CancelInvoke("CheckTimeOut");
            Close();
        }
	}

	private void StartTranslate()
    {
        iTween.MoveTo(transparentLeft.gameObject, iTween.Hash("islocal", true, "time", .8, "position", Vector3.zero, "easetype", iTween.EaseType.linear, "oncomplete", "CompleteSlideInto", "oncompletetarget", gameObject));
        iTween.MoveTo(transparentRight.gameObject, iTween.Hash("islocal", true, "time", .8, "position", Vector3.zero, "easetype", iTween.EaseType.linear, "oncomplete", "CompleteSlideInto", "oncompletetarget", gameObject));
	}
	private void CompleteSlideInto()
    {
	}

	void Close()
    {
        iTween.MoveTo(transparentLeft.transform.parent.gameObject, iTween.Hash("islocal", true, "time", .8, "position", new Vector3(-100f, 0f, 0f), "easetype", iTween.EaseType.linear, "oncomplete", "OnClose", "oncompletetarget", gameObject));
        iTween.MoveTo(transparentRight.transform.parent.gameObject, iTween.Hash("islocal", true, "time", .8, "position", new Vector3(100f, 0f, 0f), "easetype", iTween.EaseType.linear, "oncomplete", "OnClose", "oncompletetarget", gameObject)); 
	}

	private void OnClose()
    {
        loadingIcon.gameObject.SetActive(false);
        transparentLeft.gameObject.SetActive(false);
        transparentRight.gameObject.SetActive(false);

        HandleInput();
	}

    void CheckTimeOut()
    {
        Puppet.API.Client.APIGeneric.RequestTimeOut();
        Show(false);
    }
}



