using System.Collections;
using UnityEngine;

public class BossChangePhaseState : EnemyState
{
    [SerializeField] private ParticleSystem phaseAuraEffect;
    [SerializeField] private GameObject phaseShockwavePrefab;
    [SerializeField] private float shockwaveYOffset = 0.1f;
    [SerializeField] private AudioClip phaseChangeSfx;

    [SerializeField] private float phaseTwoSpeedMultiplier = 1.3f;
    [SerializeField] private bool isUseInvincibleDuringPhaseChange = true;

    [SerializeField] private float phaseTwoScaleMultiplier = 1.2f;
    [SerializeField] private float scaleUpDuration = 0.25f;

    [Range(0f, 1f)]
    [SerializeField] protected float phase2AuraVolume = 1;
    [SerializeField] protected AudioClip auraClip;
    private bool isPhaseEnded;
    private Coroutine scaleRoutine;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
        if (phaseAuraEffect == null) return;
        phaseAuraEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    public override void EnterState(EnumTypes.STATE state, object data = null)
    {
        base.EnterState(state, data);
        isPhaseEnded = false;

        controller.NavigationStop();
        if (isUseInvincibleDuringPhaseChange) statComp.IsInvincible = true;

        PlayPhaseChangeEffect();
        Anim.SetInteger("State", 10);
        Anim.SetTrigger("Phase");
    }
    public override void UpdateState()
    {

    }

    private void PlayPhaseChangeEffect()
    {
        if (phaseAuraEffect != null) phaseAuraEffect.Play();

        if (phaseShockwavePrefab != null)
        {
            StartCoroutine(SpawnShockwaveDelay());
        }
        if(scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleUpRoutine());
        if (controller.Player != null)
        {
            CameraShakeController cam = controller.Player.GetComponent<CameraShakeController>();
            if(cam != null)
            {
                cam.ShakeCam(1.2f, 0.3f, 3.2f);
                cam.PlayMotionBlur(0.2f, 0.1f);
            }
        }
        if (phaseChangeSfx != null)
        {
            GameManager.Instance.SoundManager.PlaySFX(phaseChangeSfx);
        }
    }
    public void OnChangeEndPhase()
    {
        if (isPhaseEnded) return;
        isPhaseEnded = true;
        statComp.SetSpeedMultifle(phaseTwoSpeedMultiplier);
        if (controller.BossPhase != null)
        {
            controller.BossPhase.CompletePhaseTwo();
        }

        if (isUseInvincibleDuringPhaseChange)
        {
            statComp.IsInvincible = false;
        }
        if(auraClip != null)GameManager.Instance.SoundManager.PlayLoopSFX(auraClip, clipVolume);
        controller.TransitionToState(EnumTypes.STATE.DETECT);
    }
    private IEnumerator ScaleUpRoutine()
    {
        Vector3 startScale = originalScale;
        Vector3 endScale = originalScale * phaseTwoScaleMultiplier;

        float time = 0;
        while(time < scaleUpDuration)
        {
            time += Time.deltaTime;
            float t = time / scaleUpDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        transform.localScale = endScale;
    }    
    private IEnumerator SpawnShockwaveDelay()
    {
        if (phaseShockwavePrefab == null) yield break;

        Vector3 pos = transform.position + Vector3.up * shockwaveYOffset;
        yield return new WaitForSeconds(0.2f);

        GameObject large = Instantiate(phaseShockwavePrefab, pos, Quaternion.identity);
        large.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.3f);

        GameObject mid = Instantiate(phaseShockwavePrefab, pos, Quaternion.identity);
        mid.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.3f);

        GameObject small = Instantiate(phaseShockwavePrefab, pos, Quaternion.identity);
        small.transform.localScale = Vector3.one;

        Destroy(large, 2f);
        Destroy(mid, 2f);
        Destroy(small, 2f);
    }

}
