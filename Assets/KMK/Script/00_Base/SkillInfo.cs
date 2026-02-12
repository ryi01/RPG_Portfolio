using UnityEngine;
[CreateAssetMenu]
public class SkillInfo: ScriptableObject
{
    [Header("Attack")]
    public float attackMultifle = 1.0f;
    public float attackMinRange = 2.5f;
    public float attackMaxRange = 15f;
    public float attackRadius = 2.0f;
    public float hitAngle = 60f;
    public float attackTime = 1;
    [Header("KnockBack")]
    public float nockbackForce = 2;
    [Header("CoolTime")]
    public float coolTime = 2;
    [Header("Anim")]
    public string animTrigger;
}
