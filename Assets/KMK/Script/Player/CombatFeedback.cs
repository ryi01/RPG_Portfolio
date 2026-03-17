using System.Collections;
using UnityEngine;

public class CombatFeedback : MonoBehaviour
{
    private Coroutine slowCorutine;
    public void HitStop(float duration)
    {
        PlayTimeEffect(0, duration);
    }
    public void ImpactSlow(float scale, float duration)
    {
        PlayTimeEffect(scale, duration);
    }

    private void PlayTimeEffect(float slowScale, float duration)
    {
        if (slowCorutine != null)
            StopCoroutine(slowCorutine);

        slowCorutine = StartCoroutine(TimeEffectRoutine(slowScale, duration));
    }
    IEnumerator TimeEffectRoutine(float slowScale, float duration)
    {

        Time.timeScale = slowScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1;
        slowCorutine = null;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
