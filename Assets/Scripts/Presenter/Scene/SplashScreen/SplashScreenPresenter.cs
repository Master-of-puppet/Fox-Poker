using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SplashScreenPresenter
{
    public event Action<float, string> onLoadingChange;

    public void OnUIReady()
    {
        PuApp.Instance.StartApplication(OnLoadConfig);
        SoundManager.Instance.LoadOnStartApp();
    }

    void OnLoadConfig(float progres, string message)
    {
        if (onLoadingChange != null)
            onLoadingChange(progres, message);
    }
}
