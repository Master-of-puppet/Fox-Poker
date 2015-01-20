using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Puppet;
using UnityEngine;

public enum SoundType
{
    Background,
    ClickButton,
    Reward,
    Daily_Gift,
    Win,
    Lost,
    Draw,
}

[PrefabAttribute(Name = "Prefabs/Sounds/SoundMapping")]
public class PuSound : SingletonPrefab<PuSound>
{
    [Serializable]
    public class SoundClip
    {
        public SoundType type;
        public AudioClip clip;
    }

    #region UNITY EDITOR
    public List<SoundClip> DataMapping;
    #endregion

    public void Play(SoundType type, bool isLoop = false)
    {
        Play(null, type, isLoop);
    }

    public void Play(AudioSource source, SoundType type, bool isLoop)
    {
        SoundClip sound = DataMapping.Find(s => s.type == type && s.clip != null);
        if (sound != null)
            SoundManager.StartSound(source, sound.clip, isLoop);
    }

    public void Stop(AudioSource source)
    {
        SoundManager.StopSound(source);
    }
}
