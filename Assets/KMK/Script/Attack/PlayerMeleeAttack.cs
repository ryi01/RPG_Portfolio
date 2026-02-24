using UnityEngine;

public class PlayerMeleeAttack : MeleeAttack
{
    protected PlayerController pc => bc as PlayerController;
    private CameraShakeController camController;
    [SerializeField] private Vector2 cameraGain;
    private void Start()
    {
        camController = FindFirstObjectByType<CameraShakeController>();
    }
    protected override void AttackReady()
    {
        base.AttackReady();
        camController.ShakeCam(cameraGain.x, cameraGain.y);
    }
    protected override void AttackHit(Collider hit)
    {
        base.AttackHit(hit);
        
        hit.GetComponent<BaseController>()?.Damage(CS.FinalAttack, CS.NockbackForce, transform);
    }
}
