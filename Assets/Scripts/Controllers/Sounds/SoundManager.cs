using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Puppet;

/// <summary>
/// Author: vietdungvn88@gmail.com
/// Class mapping AudioClip and type of sound in Foxpoker
/// </summary>
[PrefabAttribute(Name = "Prefabs/Sounds/SoundManager")]
public class SoundManager : Singleton<SoundManager>
{
    public List<AudioSource> SFXSources;
    public List<AudioSource> MusicSources;

    public float MusicVolume = 1f;
    public float SFXVolume = 1f;
    public float Pitch = 1f;
    public bool Loop = false;
    public float MinDistance = 1f;
    public float MaxDistance = 500f;
    public float pan = 1f;

    public bool SoundEnabled = true;
    public bool IsMuteMusic = false;
    public bool IsMuteSFX = false;

    protected override void Init()
    {
        SFXSources = new List<AudioSource>();
        MusicSources = new List<AudioSource>();
    }

    public void LoadOnStartApp()
    {
    }

    void Awake()
    { 
        if(gameObject.GetComponent<AudioListener>() == null)
            gameObject.AddComponent<AudioListener>();
        DontDestroyOnLoad(gameObject);
    }

    public static void StartSound(AudioSource source, AudioClip clip, int loopTime)
    {
        if (clip == null || !Application.isPlaying)
            return;

        if (source != null)
            Instance._StartSound(source, clip, loopTime < 0 ? true : false, false);
        else
            Instance.StartCoroutine(Instance._StartOnOtherSource(null, clip, loopTime, false));
    }

    public static void StartMusic(AudioSource source, AudioClip clip, int loopTime)
    {
        if (clip == null || !Application.isPlaying)
            return;

        if (source != null)
            Instance._StartSound(source, clip, loopTime < 0 ? true : false, true);
        else
            Instance.StartCoroutine(Instance._StartOnOtherSource(null, clip, loopTime, true));
    }

    public static void StartOneShotSound(GameObject obj, AudioClip clip)
    {
        if (clip == null || !Application.isPlaying)
            return;

        Instance.StartCoroutine(Instance._StartOnOtherSource(obj, clip, 1, false));
    }

    public static void StopSound(AudioSource source)
    {
        if (!Application.isPlaying)
            return;

        if(Instance != null && source != null)
        {
            source.Stop();
            Instance.UnRegisterSource(source);
        }
    }

    public static void PauseSound(AudioSource source, bool isPause)
    {
        if (!Application.isPlaying)
            return;

        if (isPause)
            source.Stop();
        else
            source.Play();
    }

    public static void SetEnableSound(bool value)
    {
        if (Application.isPlaying) Instance.SoundEnabled = value;
    }

    public static void MuteMusic(bool mute)
    {
        if (!Application.isPlaying)
            return;

        HandleSource(ref Instance.MusicSources, mute);
        Instance.IsMuteMusic = mute;
    }

    public static void MuteSFX(bool mute)
    {
        if (!Application.isPlaying)
            return;
        
        HandleSource(ref Instance.SFXSources, mute);
        Instance.IsMuteSFX = mute;
    }

    public static void StopAllSource(bool isMusic)
    {
        if (!Application.isPlaying || Instance == null)
            return;

        if(isMusic)
            StopSource(ref Instance.MusicSources);
        else
            StopSource(ref Instance.SFXSources);
    }

    static void StopSource(ref List<AudioSource> listSource)
    {
        //Remove all AudioSource destroyed when change scene
        listSource.RemoveAll(i => i == null);
        for (int i = 0; i < listSource.Count; i++)
            listSource[i].Stop();
    }

    static void HandleSource(ref List<AudioSource> listSource, bool isMute)
    {
        //Remove all AudioSource destroyed when change scene
        listSource.RemoveAll(i => i == null);
        for (int i = 0; i < listSource.Count; i++)
        {
            if (isMute) listSource[i].Stop();
            else listSource[i].Play();

            listSource[i].mute = isMute;
        }
    }

    void _StartSound(AudioSource source, AudioClip clip, bool isLoop, bool isMusic)
    {
        RegisterSource(source, isMusic);
        if (source.isPlaying)
            source.Stop();
        PlaySound(source, clip, isLoop, isMusic);
    }

    IEnumerator _StartOnOtherSource(GameObject obj, AudioClip clip, int loopTime, bool isMusic)
    {
        bool isLoop = loopTime < 0;
        AudioSource newSource = CreateNewSource(obj, isLoop, isMusic);

        RegisterSource(newSource, isMusic);

        if (loopTime > 0)
        {
            for (int i = 0; i < loopTime; i++)
            {
                PlaySound(newSource, clip, isLoop, isMusic);
                while (newSource != null && newSource.isPlaying && !isLoop)
                    yield return null;
            }
        }
        else
        {
            PlaySound(newSource, clip, isLoop, isMusic);
        }

        if (newSource != null && !isLoop)
        {
            GameObject.Destroy(newSource);
            UnRegisterSource(newSource);
        }
    }

    private void RegisterSource(AudioSource source, bool isMusic)
    {
        if (source && isMusic && !MusicSources.Contains(source))
        {
            MusicSources.Add(source);
        }
        else if (source && !isMusic && !SFXSources.Contains(source))
        {
            SFXSources.Add(source);
        }
    }

    private void UnRegisterSource(AudioSource source)
    {
        if (source && MusicSources.Contains(source))
        {
            MusicSources.Remove(source);
        }
        if (source && SFXSources.Contains(source))
        {
            SFXSources.Remove(source);
        }
    }

    void PlaySound(AudioSource source, AudioClip clip, bool isLoop, bool isMusic)
    {
        source.loop = isLoop;
        source.clip = clip;
        if (SoundEnabled && ((!isMusic && !IsMuteSFX) || ((isMusic && !IsMuteMusic))))
        {
            source.Play();
        }
    }

    AudioSource CreateNewSource(GameObject obj, bool isLoop, bool isMusic)
    {
        if (obj == null) obj = gameObject;

        AudioSource source = obj.AddComponent<AudioSource>();
        source.volume = isMusic ? MusicVolume : SFXVolume;
        source.pitch = Pitch;
        source.loop = isLoop;
        source.minDistance = MinDistance;
        source.maxDistance = MaxDistance;
        source.pan = pan;

        return source;
    }
}
