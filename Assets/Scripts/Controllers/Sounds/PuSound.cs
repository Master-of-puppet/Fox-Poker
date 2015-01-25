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
    Sit_Down,
    StandUp,
    YourTurn,
    DealCard,
    DealComminity,
    FoldCard,
    RaiseCost,
    CheckCard,
    UpdatePot,
    PlayerWin,
    GetMoney,
}

/// <summary>
/// Author: vietdungvn88@gmail.com
/// Class mapping AudioClip and type of sound in Foxpoker
/// </summary>
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

    /// <summary>
    /// Method play AudioClip with Type of sound.
    /// </summary>
    /// <param name="type">Type of sound will play</param>
    /// <param name="loopTime">more than zero/-1 is forever/0 is no playing</param>
    public void Play(SoundType type, int loopTime = 1)
    {
        Play(null, type, loopTime);
    }

    public void Play(AudioSource source, SoundType type, int loopTime)
    {
        SoundClip sound = DataMapping.Find(s => s.type == type && s.clip != null);
        if (sound != null)
            SoundManager.StartSound(source, sound.clip, loopTime);
    }

    public void Stop(AudioSource source)
    {
        SoundManager.StopSound(source);
    }
}
