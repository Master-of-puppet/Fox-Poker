using Puppet;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class SplashScreenView : MonoBehaviour
{
    public UISprite backgroundImage;
    public UISlider progressBar;
    public UILabel loadingText;

    SplashScreenPresenter presenter;

    void Awake()
    {
        presenter = new SplashScreenPresenter();
    }

    IEnumerator Start()
    {
        for (int i = 255; i >= 0;i-- )
        {
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, i/255f);
            yield return new WaitForFixedUpdate();

            if(i == 200)
                presenter.OnUIReady();
        }
    }

    void OnEnable()
    {
        presenter.onLoadingChange += presenter_onLoadingChange;
    }

    void OnDisable()
    {
        presenter.onLoadingChange -= presenter_onLoadingChange;
    }

    void presenter_onLoadingChange(float progress, string loadingText)
    {
        this.progressBar.value = progress;
        this.loadingText.text = loadingText;
    }
}
