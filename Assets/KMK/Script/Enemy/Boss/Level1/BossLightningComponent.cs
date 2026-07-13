using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossLightningComponent : MonoBehaviour
{
    [SerializeField] private GameObject wariningPrefab;
    [SerializeField] private GameObject lightningPrefab;

    [SerializeField] private float lightningInterval = 1.8f;
    [SerializeField] private float strikeDelay = 0.55f;
    [SerializeField] private float multiStrikeGap = 0.12f;

    [SerializeField] private int strikeCount = 3;
    [SerializeField] private float strikeRadius = 2.8f;
    [SerializeField] private float groundY = 0.5f;
    [SerializeField] private float boltLifeTime = 1.5f;

    private EnemyController controller;
    private Coroutine lightCoroutine;

    private List<GameObject> activeWarnings = new List<GameObject>();
    private List<GameObject> activeBolts = new List<GameObject>();

    [SerializeField]
    [Range(0f, 1f)] protected float clipVolume = 1;
    [SerializeField] protected AudioClip clip;

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }
    public void StartPattern()
    {
        if (lightCoroutine != null) return;
        lightCoroutine = StartCoroutine(LightningRoutine());
    }
    public void StopPattern()
    {
        if(lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
            lightCoroutine = null;
        }

        ClearAll();
    }
    private IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(lightningInterval);
            if(!CanRunPattern())
            {
                StopPattern();
                yield break;
            }

            List<Vector3> strikePos = BuildStrikePositions();

            for (int i = 0; i < strikePos.Count; i++)
            {
                StartCoroutine(ExecuteLightning(strikePos[i]));
                if (i < strikePos.Count - 1) yield return new WaitForSeconds(multiStrikeGap);
            }
        }
    }
    private bool CanRunPattern()
    {
        if (controller == null) return false;
        if (controller.Player == null) return false;
        if (controller.CurrentState != null &&
            controller.CurrentState.StateType == EnumTypes.STATE.DEATH) return false;

        return true;
    }
    private List<Vector3> BuildStrikePositions()
    {
        List<Vector3> positions = new List<Vector3>();

        Vector3 center = controller.Player.transform.position;
        center.y = groundY;

        float startAngle = UnityEngine.Random.Range(0f, 360f);
        float angleStep = 360f / strikeCount;

        for (int i = 0; i< strikeCount; i++)
        {
            float angle = startAngle + angleStep * i + UnityEngine.Random.Range(-20f, 20f);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad));
            Vector3 pos = center + dir * strikeRadius;
            pos.y = groundY;

            positions.Add(pos);
        }

        return positions;
    }

    private IEnumerator ExecuteLightning(Vector3 pos)
    {
        GameObject warning = null;

        if(wariningPrefab != null)
        {
            warning = Instantiate(wariningPrefab, pos, Quaternion.identity);
            activeWarnings.Add(warning);
            warning.transform.localScale = Vector3.zero;
        }
        float elapsed = 0;

        while (elapsed < strikeDelay)
        {
            if(!CanRunPattern())
            {
                RemoveWarning(warning);
                yield break;
            }

            elapsed += Time.deltaTime;
            float ratio = Mathf.Clamp01(elapsed / strikeDelay);
            if (warning != null)
            {
                // 마지막으로 갈수록 더 빠르게 커지는 느낌
                float scale = Mathf.Lerp(0f, 1f, ratio * ratio * 1.15f);
                warning.transform.localScale = Vector3.one * Mathf.Clamp01(scale);
            }

            yield return null;
        }

        RemoveWarning(warning);

        if (!CanRunPattern()) yield break;

        GameObject bolt = Instantiate(lightningPrefab, pos, Quaternion.identity);
        GameManager.Instance.SoundManager.PlayImpactSFX(clip, clipVolume);
        activeBolts.Add(bolt);
        StartCoroutine(DestroyBoltAfterTime(bolt, boltLifeTime));
    }
    private IEnumerator DestroyBoltAfterTime(GameObject bolt, float delay)
    {
        yield return new WaitForSeconds(delay);

        if(bolt != null)
        {
            activeBolts.Remove(bolt);
            Destroy(bolt);
        }
    }    

    private void RemoveWarning(GameObject warning)
    {
        if(warning != null)
        {
            activeWarnings.Remove(warning);
            Destroy(warning);
        }
    }
    private void ClearAll()
    {
        for(int i = 0; i < activeWarnings.Count; i++)
        {
            if (activeWarnings[i] != null) Destroy(activeWarnings[i]);
        }
        activeWarnings.Clear();

        for(int i = 0; i < activeBolts.Count;i++)
        {
            if (activeBolts[i] != null) Destroy(activeBolts[i]);
        }
        activeBolts.Clear();
    }
}
