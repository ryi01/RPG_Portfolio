using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private float shakeIntensityMultiplier = 1.5f;
    [SerializeField] private float shakeFrequencyMultiplier = 1.2f;
    [SerializeField] private float motionBlurMultiplier = 1.4f;
    private CinemachineCamera cam;
    private CinemachineBasicMultiChannelPerlin perNoise;
    private CinemachineImpulseSource impulseSource;
    private Coroutine zoomCoroutine;
    private Coroutine shakeCoroutine;
    private Coroutine motionBlurCoroutine;
    private float defaultOrthoSize;

    private MotionBlur motionBlur;
    private float defaultMotionBlurIntensity = 0;
    public void ResetCam()
    {
        perNoise.AmplitudeGain = 0;
        perNoise.FrequencyGain = 0;
    }
    private void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        perNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if(impulseSource == null) impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        defaultOrthoSize = cam.Lens.FieldOfView;
        if(volume != null && volume.profile != null)
        {
            volume.profile.TryGet(out motionBlur);
            if(motionBlur != null)
            {
                defaultMotionBlurIntensity = motionBlur.intensity.value;
                ResetMotionblur();
            }
        }
        ResetCam();
        ResetMotionblur();
    }

    public void ShakeCam(float intensity, float shakeTime, float frequency = 2.0f)
    {
        if (perNoise == null) return;
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        perNoise.AmplitudeGain = intensity * shakeIntensityMultiplier;
        perNoise.FrequencyGain = frequency * shakeFrequencyMultiplier;
        shakeCoroutine = StartCoroutine(WaitTime(shakeTime));
    }
    IEnumerator WaitTime(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        ResetCam();
        shakeCoroutine = null;
    }

    public void Zoom(float zoomSize, float duration, float holdTime = 0.05f)
    {
        if (zoomCoroutine != null) StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomRoutine(zoomSize, duration, holdTime));
    }    

    IEnumerator ZoomRoutine(float zoomSize, float duration, float holdTime)
    {
        float start = cam.Lens.FieldOfView;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            cam.Lens.FieldOfView = Mathf.Lerp(start, zoomSize, t);
            yield return null;
        }

        cam.Lens.FieldOfView = zoomSize;
        yield return new WaitForSecondsRealtime(holdTime);

        elapsed = 0;
        while(elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            cam.Lens.FieldOfView = Mathf.Lerp(zoomSize, defaultOrthoSize, t);
            yield return null;
        }

        cam.Lens.FieldOfView = defaultOrthoSize;
        zoomCoroutine = null;
    }
    public void ShakeCamDirectional(Vector3 hitDir, float intesity, float time, float frequency = 2.5f, bool alsoShake = true, float impulseStrength = 1.0f)
    {
        hitDir.y = 0f;
        if (hitDir.sqrMagnitude > 0.001f) hitDir.Normalize();
        else hitDir = Vector3.forward;
        GenerateImpulseDirection(hitDir, impulseStrength);
        if (alsoShake) ShakeCam(intesity, time, frequency);
    }

    public void GenerateImpulseDirection(Vector3 dir, float strength = 1.0f)
    {
        if (impulseSource == null) return;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f) dir.Normalize();
        else dir = Vector3.zero;

        impulseSource.GenerateImpulse(dir * strength);
    }
    public void GenerateImpulse(float strength)
    {
        if (impulseSource == null) return;
        impulseSource.GenerateImpulse(strength);
    }


    public void SetMotionBlur(float intensity)
    {
        if (motionBlur == null) return;
        motionBlur.active = true;
        motionBlur.intensity.value = intensity;
        Debug.Log(motionBlur.intensity.value);
    }

    public void ResetMotionblur()
    {
        if (motionBlur == null) return;
        motionBlur.intensity.value = defaultMotionBlurIntensity;
    }
    public void PlayMotionBlur(float targetIntensity, float duration)
    {
        if (motionBlur == null) return;
        if (motionBlurCoroutine != null) StopCoroutine(motionBlurCoroutine);

        motionBlurCoroutine = StartCoroutine(MotionBlurRoutine(targetIntensity * motionBlurMultiplier, duration));
    }

    private IEnumerator MotionBlurRoutine(float targetIntensity, float duration)
    {
        SetMotionBlur(targetIntensity);
        yield return new WaitForSecondsRealtime(duration);
        ResetMotionblur();
        motionBlurCoroutine = null;
    }
}
