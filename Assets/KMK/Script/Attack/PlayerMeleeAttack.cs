using UnityEngine;

public class PlayerMeleeAttack : MeleeAttack
{
    protected PlayerController pc => bc as PlayerController;
    
    [SerializeField] private Vector2 cameraGain;
    private void Start()
    {
        
    }
    protected override void AttackReady()
    {
        base.AttackReady();
        pc.CameraShakeController.ShakeCam(cameraGain.x, cameraGain.y);
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
        
        hit.GetComponent<BaseController>()?.Damage(CS.FinalAttack, CS.NockbackForce, transform);
    }
}
