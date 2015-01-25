using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SoundButtonClick : MonoBehaviour
{
    void OnClick()
    {
        PuSound.Instance.Play(SoundType.ClickButton);
    }
}
