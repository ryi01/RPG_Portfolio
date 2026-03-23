using System.Collections;
using UnityEngine;

public class CombatFeedback : MonoBehaviour
{
    public enum HitStrength { Light, Medium, Heavy, Boss }
    private Coroutine timeEffectCoroutine;
    private const float BASE_FIXED_DELTA = 0.02f;
    [SerializeField] private float lightDuration = 0.035f;
    [SerializeField] private float lightScale = 0.04f;

    [SerializeField] private float mediumDuration = 0.05f;
    [SerializeField] private float mediumScale = 0.03f;

    [SerializeField] private float heavyDuration = 0.07f;
    [SerializeField] private float heavyScale = 0.02f;

    [SerializeField] private float bossDuration = 0.08f;
    [SerializeField] private float bossScale = 0.02f;
    public void HitStop(float duration, float stopScale = 0.03f)
    {
        PlayTimeEffect(stopScale, duration);
    }
    public void ImpactSlow(float scale, float duration)
    {
        PlayTimeEffect(scale, duration);
    }
    public void HitStopThenSlow(float stopDuration, float stopScale, float slowScale, float slowDuration)
    {
        if (timeEffectCoroutine != null) StopCoroutine(timeEffectCoroutine);
        timeEffectCoroutine = StartCoroutine(HitStopThenRoutine(stopDuration, stopScale, slowScale, slowDuration));
    }

    private void PlayTimeEffect(float slowScale, float duration)
    {
        if (timeEffectCoroutine != null) StopCoroutine(timeEffectCoroutine);

        timeEffectCoroutine = StartCoroutine(TimeEffectRoutine(slowScale, duration));
    }
    IEnumerator TimeEffectRoutine(float slowScale, float duration)
    {
        ApplyTimeScale(slowScale);
        yield return new WaitForSecondsRealtime(duration);
        RestoreTimeScale();
        timeEffectCoroutine = null;
    }

    IEnumerator HitStopThenRoutine(float stopDuration, float stopScale, float slowScale, float slowDuration)
    {
        ApplyTimeScale(stopScale);
        yield return new WaitForSecondsRealtime(stopDuration);
        ApplyTimeScale(slowScale);
        yield return new WaitForSecondsRealtime(slowDuration);
        RestoreTimeScale();
        timeEffectCoroutine = null;
    }
    private void ApplyTimeScale(float scale)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = BASE_FIXED_DELTA * Time.timeScale;
    }
    private void RestoreTimeScale()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = BASE_FIXED_DELTA;
    }
    public void HitStopByStrength(HitStrength strenght)
    {
        switch (strenght)
        {
            case HitStrength.Light:
                HitStop(lightDuration, lightScale);
                break;
            case HitStrength.Medium:
                HitStop(mediumDuration, mediumScale);
                break;
            case HitStrength.Heavy:
                HitStop(heavyDuration, heavyScale);
                break;
            case HitStrength.Boss:
                HitStop(bossDuration, bossScale);
                break;
        }
    }
    private void OnDisable()
    {
        RestoreTimeScale();
    }
}
