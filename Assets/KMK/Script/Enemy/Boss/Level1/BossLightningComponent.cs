using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLightningComponent : MonoBehaviour
{
    [SerializeField] private GameObject wariningPrefab;
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private float lightningInterval = 2.0f;
    [SerializeField] private float strikeDelay = 1.0f;

    private EnemyController controller;
    private Coroutine lightCoroutine;
    private List<GameObject> lightList = new List<GameObject>();

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
            if (controller.Player == null) yield break;
            if (controller.CurrentState != null && controller.CurrentState.StateType == EnumTypes.STATE.DEATH) yield break;
            Vector3 strikePos = controller.Player.transform.position;
            strikePos.y = 0.5f;

            yield return StartCoroutine(ExecuteLightning(strikePos));
        }
    }
    private IEnumerator ExecuteLightning(Vector3 pos)
    {
        GameObject warning = Instantiate(wariningPrefab, pos, Quaternion.identity);

        lightList.Add(warning);

        warning.transform.localScale = Vector3.zero;
        float elapsed = 0;

        while (elapsed < strikeDelay)
        {
            if (warning == null) yield break;
            elapsed += Time.deltaTime;
            float ratio = elapsed / strikeDelay;

            warning.transform.localScale = Vector3.one * ratio;
            yield return null;
        }

        if (warning != null)
        {
            lightList.Remove(warning);
            Destroy(warning);
        }
        
        GameObject bolt = Instantiate(lightningPrefab, pos, Quaternion.identity);
        Destroy(bolt, 2);
    }

    private void ClearAll()
    {
        if(lightCoroutine != null)
        {
            StopCoroutine(lightCoroutine);
            lightCoroutine = null;
        }
        foreach(var obj in lightList)
        {
            if (obj != null) Destroy(obj);
        }
        lightList.Clear();
    }
}
