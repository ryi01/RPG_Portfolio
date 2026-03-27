using UnityEngine;

// state 마다 이미 속도가 다름 => EnemyStatInfo를 통해 변경 가능
public class EnemyStatComponent : CharacterStatComponent
{
    private EnemyStatInfo enemyStatInfo;

    private float currentGroggy;
    [SerializeField] private float yOffset = 2.5f;

    public float NextPoint { get => enemyStatInfo.nextPointSelectDistance; }
    public float DetectRange { get => enemyStatInfo.detectRange; }
    public float WanderRange { get => enemyStatInfo.wanderRange; }
    public float AttackRange { get => enemyStatInfo.attackRange; }
    public float ReturnRange { get => enemyStatInfo.returnRange; }
    public float MaxGroogy { get => enemyStatInfo.maxGroggy; }
    public float DeathDelayTime { get => enemyStatInfo.deathDelayTime; }
    public bool IsBoss { get => enemyStatInfo.isBoss; }

    public Vector3 RoamCenter { get; set; }

    public float Exp { get => enemyStatInfo.exp; }
    public Transform WayPoint { get; set; }
    public float WanderNavCheckRadius { get => enemyStatInfo.wanderNavCheckRadius; }
    protected override void Awake()
    {
        base.Awake();
        enemyStatInfo = statinfo as EnemyStatInfo;
        if (enemyStatInfo == null) Debug.Log($"적 스테이트인포 없음");
    }
    private void Start()
    {
        if (enemyStatInfo.isBoss) GameManager.Instance.BindBoss(this);
        else GameManager.Instance.OnBindEnemy(this, yOffset);
    }

    public bool AddGroogy(float amount)
    {
        currentGroggy += amount;
        if (currentGroggy >= MaxGroogy)
        {
            currentGroggy = 0;
            return true;
        }
        return false;
    }
    public void ResetStateForSpawn()
    {
        currentHP = MaxHP;
        currentGroggy = 0;
        IsHit = false;
    }
    private void OnDestroy()
    {
        if (!enemyStatInfo.isBoss)
            GameManager.Instance.OnUnBindEnemy(this);
        else GameManager.Instance.OnUnBindBoss(this);
    }
}
