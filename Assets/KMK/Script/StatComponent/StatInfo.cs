using UnityEngine;

public abstract class StatInfo : ScriptableObject
{
    [Header("HP")]
    public float maxHP;
    [Header("Attack")]
    public float attack;
    [Header("Movement")]
    public float moveSpeed;
    public float rotSpeed;
    [Header("KnockBack")]
    public float nockbackForce;
}
