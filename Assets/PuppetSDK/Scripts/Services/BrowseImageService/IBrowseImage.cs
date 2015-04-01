using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IBrowseImage
{
    void GetImageFromGallery(string gameObject, string method, string cacheFileName);
}
