using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossSummonComponent : MonoBehaviour
{
    [SerializeField] private GameObject[] summonPrefabs;
    [SerializeField] private int summonCount = 3;
    [SerializeField] private int maxAliveSummon = 6;
    [SerializeField] private LayerMask groundLayer;

    private List<GameObject> aliveSummons = new List<GameObject>();

    private EnemyController controller;
    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    public int AliveCount
    {
        get
        {
            RemoveDeadEntries();
            return aliveSummons.Count;
        }
    }
    public bool CanSummon()
    {
        RemoveDeadEntries();
        return summonPrefabs != null && summonPrefabs.Length > 0 && aliveSummons.Count < maxAliveSummon;
    }
    public void SummonAround(Vector3 center)
    {
        if (!CanSummon()) return;
        RemoveDeadEntries();
        int remain = maxAliveSummon - aliveSummons.Count;
        int spawnAmount = Mathf.Min(summonCount, remain);

        for(int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPos = GetSpawnPos(center);
            GameObject prefab = summonPrefabs[Random.Range(0, summonPrefabs.Length)];
            GameObject summonObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            aliveSummons.Add(summonObj);
        }
    }

    public void ClearAll()
    {
        for(int i = 0; i < aliveSummons.Count; i++)
        {
            if (aliveSummons[i] != null) Destroy(aliveSummons[i]);
        }
        aliveSummons.Clear();
    }
    private Vector3 GetSpawnPos(Vector3 center)
    {
        
        Vector2 randomCircle = Random.insideUnitCircle * controller.StatComp.AttackRadius;
        Vector3 pos = center + new Vector3(randomCircle.x, 0, randomCircle.y);
        if(Physics.Raycast(pos + Vector3.up *5f, Vector3.down, out RaycastHit hit))
        {
            pos = hit.point;
        }
        return pos;
    }    
    private void RemoveDeadEntries()
    {
        aliveSummons.RemoveAll(x => x == null);
    }
}
