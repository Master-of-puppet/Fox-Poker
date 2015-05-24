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
        for (int i = 254; i >= 0;i-=2 )
        {
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, i/255f);
            yield return new WaitForFixedUpdate();

            if(i == 100)
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

        if(progress == 1)
        {
            string version;
            PuGameOption option = Puppet.API.Client.APIGeneric.GetOptionInfo(out version);
            SoundManager.MuteMusic(!option.isEnableSoundBG);
            SoundManager.MuteSFX(!option.isEnableSoundEffect);
            Screen.sleepTimeout = option.isAutoLockScreen ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

            PuSound.Instance.Play(SoundType.Background);
        }
    }
}
