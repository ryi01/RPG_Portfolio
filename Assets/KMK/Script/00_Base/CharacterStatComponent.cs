using System;
using System.Collections;
using UnityEngine;

public class CharacterStatComponent : MonoBehaviour
{
    [SerializeField]protected StatInfo statinfo;
    [SerializeField] protected Material flashMat;
    [SerializeField] private Material origin;
    private Material[] originMat;
    private SkinnedMeshRenderer[] renderers;
    private bool isMat = false;
    private bool isInvincible = false;

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

    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }

    public LayerMask TargetLayer { get => statinfo.targetLayer; }
    public LayerMask PassLayer { get => statinfo.passLayer; }

    public Action<float, float> OnHpChanged;

    protected virtual void Awake()
    {
        InitStat();
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        originMat = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originMat[i] = renderers[i].material;
        }
    }
    protected virtual void InitStat()
    {
        currentHP = statinfo.maxHP;
        OnHpChanged?.Invoke(currentHP, statinfo.maxHP);
    }
    public void SetCurrentHP(float hp)
    {
        currentHP = Mathf.Clamp(hp, 0, statinfo.maxHP);
        OnHpChanged?.Invoke(currentHP, statinfo.maxHP);
    }
    public void ResetMat()
    {
        StopAllCoroutines();
        isMat = false;
        originMat[0] = origin;
        renderers[0].material = originMat[0];
    }

    public float SetSpeedMultifle(float value)
    {
        speedMutlfile = value;
        return MoveSpeed;
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsInvincible) return;
        currentHP -= damage;
        
        if(currentHP <= 0.01f)
        {
            currentHP = 0;
        }
        else
        {
            currentHP = Mathf.Clamp(currentHP, 0, statinfo.maxHP);
        }
        OnHpChanged?.Invoke(currentHP, statinfo.maxHP);
        StartCoroutine(FlashMaterial());
    }

    protected IEnumerator FlashMaterial()
    {
        // material º¯°æ
        if (isMat || flashMat == null) yield break;
        isMat = true;

        foreach(var r in renderers)
        {
            r.material = flashMat;
        }
        yield return new WaitForSeconds(0.05f);
        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = originMat[i];
        }
        isMat = false;
    }

}
