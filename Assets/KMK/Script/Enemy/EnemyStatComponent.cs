using UnityEngine;

// state 마다 이미 속도가 다름 => EnemyStatInfo를 통해 변경 가능
public class EnemyStatComponent : CharacterStatComponent
{
    private EnemyStatInfo enemyStatInfo;

    private float currentGroggy;
    private Transform[] wanderPoints;
    private float currentStunTime;

    public bool IsStun { get => currentStunTime > 0; }
    public float NextPoint { get => enemyStatInfo.nextPointSelectDistance; }
    public float DetectRange { get => enemyStatInfo.detectRange; }
    public float WanderRange { get => enemyStatInfo.wanderRange; }
    public float AttackRange { get => enemyStatInfo.attackRange; }
    public float ReturnRange { get => enemyStatInfo.returnRange; }
    public float MaxGroogy { get => enemyStatInfo.maxGroggy; }
    public float DeathDelayTime { get => enemyStatInfo.deathDelayTime; }
    public bool IsBoss { get => enemyStatInfo.isBoss; }

    public float Exp { get => enemyStatInfo.exp; }
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
        if(enemyStatInfo.isBoss) GameManager.Instance.OnBindEnemy(this, 4);
        else GameManager.Instance.OnBindEnemy(this);
    }
    private void ApplyStun(float duration)
    {
        currentStunTime = duration;
    }
    public void UpdateStunStatus()
    {
        if(IsStun)
        {
            currentStunTime -= Time.deltaTime;
            if(currentStunTime <= 0)
            {
                currentStunTime = 0;
            }
        }
    }
    public bool AddGroogy(float amount, float duration = 3.0f)
    {
        if (IsStun) return false;
        currentGroggy += amount;
        if (currentGroggy >= MaxGroogy)
        {
            currentGroggy = 0;
            ApplyStun(duration);
            return true;
        }
        return false;
    }

    public void ReganGroogy(float deltaTime)
    {
        currentGroggy = Mathf.Clamp(currentGroggy + enemyStatInfo.reganGroggy * Time.deltaTime, 0, enemyStatInfo.maxGroggy);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnUnBindEnemy(this);        
    }
}
