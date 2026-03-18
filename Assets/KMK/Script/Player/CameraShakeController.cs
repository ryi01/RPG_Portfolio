using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    private CinemachineCamera cam;
    private CinemachineBasicMultiChannelPerlin perNoise;
    private Coroutine zoomCoroutine;
    private Coroutine shakeCoroutine;
    private float defaultOrthoSize;


    public void ResetCam()
    {
        perNoise.AmplitudeGain = 0;
    }
    private void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        perNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        defaultOrthoSize = cam.Lens.FieldOfView;
        Debug.Log($"{defaultOrthoSize}");
        ResetCam();
    }

    public void ShakeCam(float intensity, float shakeTime)
    {
        if (perNoise == null) return;
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        perNoise.AmplitudeGain = intensity;
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
    public void ShakeCamDirectional(Vector3 hitDir, float intesity, float time)
    {
        perNoise.AmplitudeGain = intesity;
        transform.position += hitDir * 0.2f;
        StartCoroutine(WaitTime(time));
    }
}
