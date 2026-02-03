using UnityEngine;

public class EnemyStatComponent : CharacterStatComponent
{
    private EnemyStatInfo enemyStatInfo;

    private float currrentGroggy;
    private Transform[] wanderPoints;

    public float DetectRange { get => enemyStatInfo.detectRange; }
    public float WanderRange { get => enemyStatInfo.wanderRange; }
    public float AttackRange { get => enemyStatInfo.attackRange; }
    public float ReturnRange { get => enemyStatInfo.returnRange; }
    public float MaxGroogy { get => enemyStatInfo.maxGroggy; }
    public Transform[] WanderPoints { get => wanderPoints; set => wanderPoints = value; }
    public float WanderNavCheckRadius { get => enemyStatInfo.wanderNavCheckRadius; }
    protected override void Awake()
    {
        base.Awake();
        enemyStatInfo = statinfo as EnemyStatInfo;
        if (enemyStatInfo == null) Debug.Log($"적 스테이트인포 없음");
    }

    private void Start()
    {
        GameObject[] wayPointGameObjects = GameObject.FindGameObjectsWithTag("GenPoint");
        wanderPoints = new Transform[wayPointGameObjects.Length];
        for (int i = 0; i < wayPointGameObjects.Length; i++)
        {
            wanderPoints[i] = wayPointGameObjects[i].transform;
        }
    }

    public bool AddGroogy(float amount)
    {
        if (currrentGroggy < amount) return false;
        currrentGroggy -= amount;
        return true;
    }

    public void ReganGroogy(float deltaTime)
    {
        currrentGroggy = Mathf.Clamp(currrentGroggy + enemyStatInfo.reganGroggy * Time.deltaTime, 0, enemyStatInfo.maxGroggy);
    }
}
