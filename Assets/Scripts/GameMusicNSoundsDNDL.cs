using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Util;

public class GameMusicNSoundsDNDL : Singletonref<GameMusicNSoundsDNDL>
{
    
    public AudioSource MusicAudioChannel;
    public AudioSource SfxAudioChannel;

    public List<AudioClip> GameMusic;

    public AudioClip UIClick;
    Coroutine MusicPlayer;
    int currentidx = -1;
    public void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        MusicAudioChannel = transform.AddComponent<AudioSource>();
        SfxAudioChannel = transform.AddComponent<AudioSource>();
    }
    
    public void StartMusic()
    {
        currentidx++;
        currentidx=currentidx<GameMusic.Count?currentidx:0;
        currentidx=Mathf.Clamp(currentidx, 0, GameMusic.Count-1);
        MusicAudioChannel.clip=GameMusic[currentidx];
        MusicAudioChannel.volume = SettingDNDL.Instance.MusicVolume;
        MusicAudioChannel.Play();
        if (MusicPlayer != null) {
            StopCoroutine(MusicPlayer);
        }
            MusicPlayer = StartCoroutine(QueuetheNextClips(GameMusic[currentidx].length));
        
    }
    public void PlaySfx(AudioClip clip)
    {
        SfxAudioChannel.clip =clip;
        SfxAudioChannel.volume = SettingDNDL.Instance.SFXVolume;
        SfxAudioChannel.pitch = 1;
        SfxAudioChannel.Play();
    }
    public void PlaySfx(AudioClip clip,float PitchMin,float PitchMax)
    {
        SfxAudioChannel.clip = clip;
        SfxAudioChannel.volume = SettingDNDL.Instance.SFXVolume;
        SfxAudioChannel.pitch=Random.Range(PitchMin,PitchMax);
        SfxAudioChannel.Play();
    }
    IEnumerator QueuetheNextClips(float sec)
    {
        yield return new WaitForSeconds(sec);
        StartMusic();
    }
    public void playClickSFX()
    {
        GameMusicNSoundsDNDL.Instance.PlaySfx(GameMusicNSoundsDNDL.Instance.UIClick, 1f, 1.2f);
    }
}
