using UnityEngine;
#region 카메라 프리셋
[System.Serializable]
public class CameraEffectPreset
{
    [Header("Shake")]
    public float intensity = 0.2f;
    public float time = 0.15f;
    public float frequency = 2.5f;

    [Header("Impulse")]
    public bool isImpulse = false;
    public float impulseStrength = 1f;

    [Header("Directional")]
    public bool isDirectionalShake = false;

    [Header("Zoom")]
    public bool isZoom = false;
    public float zoomFov = 55f;
    public float zoomDuration = 0.08f;
    public float zoomHoldTime = 0.04f;

    [Header("Motion Blur")]
    public bool isMotionBlur = false;
    public float motionBlurIntensity = 0.15f;
    public float motionBlurDuration = 0.08f;
}
#endregion
public class BossCameraEffectController : MonoBehaviour
{
    private CameraShakeController cameraShake;

    [SerializeField] private CameraEffectPreset lightHitPreset = new CameraEffectPreset();
    [SerializeField] private CameraEffectPreset heavyHitPreset = new CameraEffectPreset();
    [SerializeField] private CameraEffectPreset projectileImpactPreset = new CameraEffectPreset();
    [SerializeField] private CameraEffectPreset dashImpactPreset = new CameraEffectPreset();
    [SerializeField] private CameraEffectPreset spawnPreset = new CameraEffectPreset();
    [SerializeField] private CameraEffectPreset dashMovePreset = new CameraEffectPreset();
    [SerializeField] private CameraEffectPreset pushHitPreset = new CameraEffectPreset();


    public void SetCameraShakeController(CameraShakeController csc)
    {
        Debug.Log($"{csc}");
        cameraShake = csc;
    }
    public enum BossCameraEffectType
    {
        LightHit,          // 휘두르기, 약한 근접 타격
        HeavyHit,          // 내려찍기, 밀치기 같은 강한 타격
        ProjectileImpact,  // 돌 착탄, 유도탄 폭발, 총알 히트
        DashMove,        // 대쉬 충돌
        PushHit,
        Spawn              // 소환, 생성 계열
    }

    public void PlayEffect(BossCameraEffectType type)
    {
        PlayWithDir(type, Vector3.zero);
    }
    public void PlayWithDir(BossCameraEffectType type, Vector3 dir)
    {
        if (cameraShake == null) return;
        CameraEffectPreset preset = GetPreset(type);
        if(preset == null) return;

        ApplyPreset(preset, dir);
    }
    public void PlayDashMove()
    {
        PlayEffect(BossCameraEffectType.DashMove);
    }
    public void PlayLightHit()
    {
        PlayEffect(BossCameraEffectType.LightHit);
    }
    public void PlayHeavyHit()
    {
        PlayEffect(BossCameraEffectType.HeavyHit);
    }
    public void PlayProjectileImpact(Vector3 dir)
    {
        PlayWithDir(BossCameraEffectType.ProjectileImpact, dir);
    }
    public void PlayPushHit(Vector3 dir)
    {
        PlayWithDir(BossCameraEffectType.PushHit, dir);
    }
    public void PlaySpawn()
    {
        PlayEffect(BossCameraEffectType.Spawn);
    }

    private CameraEffectPreset GetPreset(BossCameraEffectType type)
    {
        switch (type)
        {
            case BossCameraEffectType.LightHit:
                return lightHitPreset;

            case BossCameraEffectType.HeavyHit:
                return heavyHitPreset;

            case BossCameraEffectType.ProjectileImpact:
                return projectileImpactPreset;

            case BossCameraEffectType.DashMove:
                return dashImpactPreset;
            case BossCameraEffectType.PushHit:
                return pushHitPreset;
            case BossCameraEffectType.Spawn:
                return spawnPreset;
        }

        return null;
    }

    private void ApplyPreset(CameraEffectPreset preset, Vector3 dir)
    {
        if (preset.isDirectionalShake)
        {
            cameraShake.ShakeCamDirectional(dir, preset.intensity, preset.time, preset.frequency, true, preset.impulseStrength);
        }
        else
        {
            cameraShake.ShakeCam(preset.intensity, preset.time, preset.frequency);
            if(preset.isImpulse)
            {
                cameraShake.GenerateImpulse(preset.impulseStrength);
            }
        }
        if(preset.isZoom)
        {
            cameraShake.Zoom(preset.zoomFov, preset.zoomDuration, preset.zoomHoldTime);
        }
        if(preset.isMotionBlur)
        {
            cameraShake.PlayMotionBlur(preset.motionBlurIntensity, preset.motionBlurDuration);
        }
    }

}
