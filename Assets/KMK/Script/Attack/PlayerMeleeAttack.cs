using UnityEngine;

public class PlayerMeleeAttack : MeleeAttack
{
    protected PlayerController pc => bc as PlayerController;
    private bool isHitShakeSwing = false;
    private int comboIndex = 0;
    [SerializeField] protected Vector2 hitShake;
    [SerializeField] protected Vector2 attackShake;
    [SerializeField] protected Vector2 impactScaleAndDuration;
    [SerializeField] protected Vector2 zoomSizeAndDuration;
    [SerializeField] protected float stopTime;
    [SerializeField] protected bool isAttackShake = true;
    [SerializeField] protected bool isHitShake = true;
    [SerializeField] protected bool isHitStop = true;
    public override void Attack()
    {
        isHitShakeSwing = false;
        if(isAttackShake)
        {
            float multifle = 1f + comboIndex * 0.2f;
            pc.CameraShakeController.ShakeCam(hitShake.x * multifle, hitShake.y);
        }
        base.Attack();
    }

    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
            
        if (isHitShake && !isHitShakeSwing)
        {
            float multifle = 1f + comboIndex * 0.25f;
            pc.CameraShakeController.ShakeCam(attackShake.x * multifle, attackShake.y);
            pc.CombatFeedback.HitStop(stopTime);
            isHitShakeSwing = true;
        }
        if(isHitStop) pc.CombatFeedback.HitStop(stopTime);
        if (hit.TryGetComponent<BaseController>(out var target))
        {
            target.Damage(CS.FinalAttack, CS.NockbackForce, transform);
        }
    }

    public void SetComboIndex(int n)
    {
        comboIndex = n;
    }
}
