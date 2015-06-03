using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using Puppet;
using System.IO;
using Puppet.Core.Network.Http;

public class BrowseImageService : Singleton<BrowseImageService>
{
    IBrowseImage image;
    Action<bool, Texture2D> callback;

    protected override void Init()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        image = new BrowseImageEditor();
#elif UNITY_ANDROID && !UNITY_EDITOR
        image = new BrowseImageAndroid();
#else
        image = new BrowseImageEditor();
#endif
    }

    public void BrowseImageFromGallary(Action<bool, Texture2D> callback)
    {
        this.callback = callback;
        image.GetImageFromGallery(this.gameObject.name, "GetImage", "avatar.png");
    }

    public void GetImage(string imagePath)
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
#if UNITY_WEBPLAYER
        bool status = false;
#else
        bool status = texture.LoadImage(File.ReadAllBytes(imagePath));
#endif
        if (callback != null)
            callback(!string.IsNullOrEmpty(imagePath) && status, texture);
    }
}
