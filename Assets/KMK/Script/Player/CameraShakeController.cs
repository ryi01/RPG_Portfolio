using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    private CinemachineCamera cam;
    private CinemachineBasicMultiChannelPerlin perNoise;

    public void ResetCam()
    {
        perNoise.AmplitudeGain = 0;
    }
    private void Start()
    {
        cam = GetComponent<CinemachineCamera>();
        perNoise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        ResetCam();
    }

    public void ShakeCam(float intensity, float shakeTime)
    {
        perNoise.AmplitudeGain = intensity;
        StartCoroutine(WaitTime(shakeTime));
    }
    IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
        ResetCam();
    }
}
