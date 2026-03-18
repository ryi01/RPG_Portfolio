using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
[System.Serializable]
public struct SoundData
{
    public string name;
    public AudioClip clip;
}
public enum EBGMType { MAIN_MENU, BOSS_BATTLE, FIELD_THEME}
[System.Serializable]
public struct BGMData
{
    public EBGMType type;
    public AudioClip clip;
}
public enum EAudioMixerType { Master, BGM, SFX}
public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider s_BGM;
    [SerializeField] private Slider s_SFX;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource loopSFXSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<SoundData> soundList;
    [SerializeField] private List<BGMData> bgmList;
    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();
    private Dictionary<EBGMType, AudioClip> bgmDict = new Dictionary<EBGMType, AudioClip>();

    private void Awake()
    {
        s_BGM.onValueChanged.AddListener(ChangeBGM);
        s_SFX.onValueChanged.AddListener(ChangeSFX);

        foreach(var data in soundList)
        {
            soundDict[data.name] = data.clip;
        }
        foreach(var data in bgmList)
        {
            bgmDict[data.type] = data.clip;
        }
    }
    private void Start()
    {
        s_BGM.value = 100;
        s_SFX.value = 100;
        PlayBGM(EBGMType.MAIN_MENU);
    }
    public void PlayBGM(EBGMType type)
    {
        if(bgmDict.TryGetValue(type, out AudioClip clip))
        {
            if (bgmSource.clip == clip) return;
            bgmSource.Stop();
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }
    public void PlaySFX(string name)
    {
        if(soundDict.TryGetValue(name, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    private void ChangeBGM(float volume)
    {
        float safeVolume = Mathf.Clamp(volume, 0.0001f, 100f);
        audioMixer.SetFloat("BGMAudio", Mathf.Log10(safeVolume/100) * 20);
    }
    private void ChangeSFX(float volume)
    {
        float safeVolume = Mathf.Clamp(volume, 0.0001f, 100f);
        audioMixer.SetFloat("SFXAudio", Mathf.Log10(safeVolume / 100) * 20);
    }

    public void PlayLoopSFX(string name)
    {
        if(soundDict.TryGetValue(name, out AudioClip clip))
        {
            if (loopSFXSource.clip == clip && loopSFXSource.isPlaying) return;
            loopSFXSource.clip = clip;
            loopSFXSource.loop = true;
            loopSFXSource.Play();
        }
    }
    public void StopLoopSFX()
    {
        loopSFXSource.Stop();
        loopSFXSource.clip = null;
    }
}
