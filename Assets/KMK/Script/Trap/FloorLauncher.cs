using System.Collections;
using UnityEngine;

public class FloorLauncher : ArrowLauncher
{
    [Header("Floor Àü¿ë")]
    [SerializeField] protected float speed = 3;
    [SerializeField] protected Vector3 tragetOffset = new Vector3(0, 0.5f, 0);

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isActive = false;

    private void Start()
    {
        startPos = transform.position;
        targetPos = tragetOffset + startPos;
    }
    public override void LaunchTrap()
    {
        if (isActive) return;
        StartCoroutine(TrapCycle());
    }

    IEnumerator TrapCycle()
    {
        isActive = true;
        Vector3 preview = startPos + (tragetOffset * 0.3f);
        yield return StartCoroutine(MoveTo(preview, launchTime * 0.15f));
        yield return new WaitForSeconds(launchTime * 0.1f);
        if (impactClip != null) GameManager.Instance.SoundManager.PlayImpactSFX(impactClip, impactClipVolume);
        yield return StartCoroutine(MoveTo(targetPos, launchTime * 0.15f));
        yield return new WaitForSeconds(launchTime * 0.2f);
        yield return StartCoroutine(MoveTo(startPos, launchTime * 0.4f));
        isActive = false;
    }

    IEnumerator MoveTo(Vector3 dest, float duration)
    {
        float timer = 0;
        Vector3 initPos = transform.position;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.position = Vector3.Lerp(startPos, dest, t);
            yield return null;
        }
        transform.position = dest;
    }
}
