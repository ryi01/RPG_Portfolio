using UnityEngine;

public class PlayerMeleeAttack : MeleeAttack
{
    private CameraShakeController camController;
    [SerializeField] private Vector2 attackGain;
    private void Start()
    {
        camController = GetComponent<CameraShakeController>();
    }
    protected override void AttackReady()
    {
        base.AttackReady();
        camController.ShakeCam(attackGain.x, attackGain.y);
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
        
        bc.Damage(CS.FinalAttack, CS.NockbackForce);
    }
}
