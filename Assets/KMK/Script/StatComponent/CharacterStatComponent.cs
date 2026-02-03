using UnityEngine;

public class CharacterStatComponent : MonoBehaviour
{
    [SerializeField]protected StatInfo statinfo;

    protected float currentHP;
    protected float speedMutlfile = 1;

    public float CurrentHP { get => currentHP;}
    public float MaxHP { get => statinfo.maxHP; }
    public float MoveSpeed { get => statinfo.moveSpeed * speedMutlfile;}
    public float RotSpeed { get => statinfo.rotSpeed; }
    public float Attack { get => statinfo.attack; }
    public float NockbackForce { get => statinfo.nockbackForce; }

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
