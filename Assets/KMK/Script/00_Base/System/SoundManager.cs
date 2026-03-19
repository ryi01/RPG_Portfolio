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
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource combatSource;

    [SerializeField] private List<SoundData> soundList;
    [SerializeField] private List<BGMData> bgmList;
    private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();
    private Dictionary<EBGMType, AudioClip> bgmDict = new Dictionary<EBGMType, AudioClip>();
    private Dictionary<AudioClip, float> lastPlayTimeDict = new Dictionary<AudioClip, float>();
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
        ChangeBGM(100);
        ChangeSFX(100);
        PlayBGM(EBGMType.MAIN_MENU);
    }
    public void PlayBGM(EBGMType type)
    {
        if (!bgmDict.TryGetValue(type, out AudioClip clip) || clip == null) return;
        if (bgmSource.clip == clip) return;
        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }
    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale);
    }
    public void PlayCombatSFX(AudioClip clip, float volumeScale = 1f, bool randomPitch = true)
    {
        if (clip == null) return;
        if (randomPitch) combatSource.pitch = Random.Range(0.97f, 1.03f);
        else combatSource.pitch = 1f;

        combatSource.PlayOneShot(clip, volumeScale);
    }
    public void PlayImpactSFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;

        combatSource.pitch = Random.Range(0.98f, 1.02f);
        combatSource.PlayOneShot(clip, volumeScale);
    }

    public void PlaySFXWithCooldown(AudioClip clip, float cooldown, float volumeScale = 1f)
    {
        float now = Time.time;
        if(lastPlayTimeDict.TryGetValue(clip, out float lastTime))
        {
            if(now - lastTime < cooldown)
            {
                return;
            }
        }

        lastPlayTimeDict[clip] = now;
        PlaySFX(clip, volumeScale);
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
            if (sfxSource.clip == clip && sfxSource.isPlaying) return;
            sfxSource.clip = clip;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }
    public void StopLoopSFX()
    {
        sfxSource.Stop();
        sfxSource.clip = null;
    }
}
