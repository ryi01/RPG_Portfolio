using UnityEngine;

public abstract class StatInfo : ScriptableObject
{
    [Header("HitLayer")]
    public LayerMask targetLayer;
    [Header("NoHitLayer")]
    public LayerMask passLayer;
    [Header("HP")]
    public float maxHP;
    [Header("Attack")]
    public float attack;
    public float attackRadius;
    public float hitAngle;
    [Header("Movement")]
    public float moveSpeed;
    public float rotSpeed;
    [Header("KnockBack")]
    public float nockbackForce;
}
