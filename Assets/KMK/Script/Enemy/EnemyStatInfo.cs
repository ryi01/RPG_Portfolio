using UnityEngine;

[CreateAssetMenu(menuName = "Stat/Enemy")]
public class EnemyStatInfo : StatInfo
{
    [Header("Range")]
    public float detectRange;
    public float attackRange;
    public float wanderRange;
    public float returnRange;
    [Header("Groggy")]
    public float maxGroggy;
    public float reganGroggy;
    [Header("Wander")]
    public float wanderNavCheckRadius;
    public float nextPointSelectDistance = 1;
    [Header("Death")]
    public float deathDelayTime = 2;
    [Header("Boss")]
    public bool isBoss = false;
}
