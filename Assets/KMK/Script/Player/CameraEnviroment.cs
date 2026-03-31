using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class EnvironmentSetting
{
    public Color backgroundColor = Color.black;

    public bool useFog = false;
    public FogMode fogMode = FogMode.Exponential;
    public Color fogColor = Color.gray;
    [Range(0f, 0.1f)] public float fogDensity = 0.02f;

    public Color ambientColor = Color.white;

    public Color directionalLightColor = Color.white;
    [Range(0f, 2f)] public float directionalLightIntensity = 1f;

    public bool useBloom = true;
    [Range(0f, 2f)] public float bloomIntensity = 0.25f;
    [Range(0f, 5f)] public float bloomThreshold = 1.1f;

    public bool useVignette = true;
    [Range(0f, 1f)] public float vignetteIntensity = 0.2f;
    [Range(0f, 1f)] public float vignetteSmoothness = 0.5f;

    [Range(-5f, 5f)] public float postExposure = -0.05f;
    [Range(-100f, 100f)] public float contrast = 12f;
    [Range(-100f, 100f)] public float saturation = -8f;
    public Color colorFilter = Color.white;
}

public class CameraEnviroment : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Volume globalVolume;

    [Header("Presets")]
    [SerializeField] private EnvironmentSetting townSetting;
    [SerializeField] private EnvironmentSetting dungeonSetting;
    [SerializeField] private EnvironmentSetting bossRoomSetting;

    private Bloom bloom;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private void Awake()
    {
        if(globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out bloom);
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out colorAdjustments);
        }
    }
    public void ChangeToDungeon()
    {
        ApplyEnvironment(dungeonSetting);
    }

    public void ChangeToTown()
    {
        ApplyEnvironment(townSetting);
    }
    public void ChangeToBossRoom()
    {
        ApplyEnvironment(bossRoomSetting);
    }

    public void ApplyEnvironment(EnvironmentSetting setting)
    {
        if (setting == null) return;
        if (cam != null) cam.backgroundColor = setting.backgroundColor;

        RenderSettings.fog = setting.useFog;
        RenderSettings.fogMode = setting.fogMode;
        RenderSettings.fogColor = setting.fogColor;
        RenderSettings.fogDensity = setting.fogDensity;
        RenderSettings.ambientLight = setting.ambientColor;

        if (directionalLight != null)
        {
            directionalLight.color = setting.directionalLightColor;
            directionalLight.intensity = setting.directionalLightIntensity;
        }
        ApplyPostProcessing(setting);
    }

    private void ApplyPostProcessing(EnvironmentSetting setting)
    {
        if(bloom != null)
        {
            bloom.active = setting.useBloom;
            bloom.intensity.value = setting.bloomIntensity;
            bloom.threshold.value = setting.bloomThreshold;
        }

        if(vignette != null)
        {
            vignette.active = setting.useVignette;
            vignette.intensity.value = setting.vignetteIntensity;
            vignette.smoothness.value = setting.vignetteSmoothness;
        }

        if(colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = setting.postExposure;
            colorAdjustments.contrast.value = setting.contrast;
            colorAdjustments.saturation.value = setting.saturation;
            colorAdjustments.colorFilter.value = setting.colorFilter;
        }
    }
}
