using UnityEngine;

public class CharacterStatComponent : MonoBehaviour
{
    [SerializeField]protected StatInfo statinfo;

    protected float currentHP;
    public float speedMutlfile = 1;
    public float attackBuffMultifle = 1;

    public float CurrentHP { get => currentHP;}
    public float MaxHP { get => statinfo.maxHP; }
    private bool isHit = false;
    public bool IsHit { get => isHit; set => isHit = value; }
    public float MoveSpeed { get => statinfo.moveSpeed * speedMutlfile;}
    public float RotSpeed { get => statinfo.rotSpeed; }
    public float Attack { get => statinfo.attack; }
    public float FinalAttack { get => Attack * attackBuffMultifle; }
    public float AttackRadius { get => statinfo.attackRadius; }
    public float HitAngle { get => statinfo.hitAngle; }
    public float NockbackForce { get => statinfo.nockbackForce; }
    public float KnckBackTime { get => statinfo.nockbackTime; }

    public LayerMask TargetLayer { get => statinfo.targetLayer; }
    public LayerMask PassLayer { get => statinfo.passLayer; }

    protected virtual void Awake()
    {
        InitStat();
    }
    protected virtual void InitStat()
    {
        currentHP = statinfo.maxHP;
    }

    public void SetSpeedMultifle(float value)
    {
        speedMutlfile = value;
    }
    public void RecoveryHP(float recovery)
    {
        currentHP = Mathf.Clamp(currentHP + recovery, 0, statinfo.maxHP);
    }
    public void TakeDamage(float damage)
    {
        currentHP = Mathf.Clamp(currentHP - damage, 0, statinfo.maxHP);
    }
}
