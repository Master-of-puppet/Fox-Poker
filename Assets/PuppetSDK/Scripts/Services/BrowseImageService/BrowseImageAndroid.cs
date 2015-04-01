using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class BrowseImageAndroid : AbstractAndroidAPI, IBrowseImage
{
    protected override string ClassAndroidAPI
    {
        get { return "com.ensign.browseimagefromgallery.ImageAPI"; }
    }

    public void GetImageFromGallery(string objectName, string method, string cacheFileName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
         Call("dungnv_GetImageFromGallery", new object[] { objectName, method, cacheFileName });
#endif
    }
}
