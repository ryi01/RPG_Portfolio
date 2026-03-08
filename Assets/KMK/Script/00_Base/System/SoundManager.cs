using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum EAudioMixerType { Master, BGM, SFX}
public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider s_BGM;
    [SerializeField] private Slider s_SFX;
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        s_BGM.onValueChanged.AddListener(ChangeBGM);
        s_SFX.onValueChanged.AddListener(ChangeSFX);
    }
    private void Start()
    {
        s_BGM.value = 100;
        s_SFX.value = 100;
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


}
