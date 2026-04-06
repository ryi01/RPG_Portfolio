using UnityEngine;

public class BossHomingMissileSkillAttack : EnemySkillAttack
{
    [SerializeField] private GameObject homingProjectilePrefab;

    public override void Attack()
    {
        if (homingProjectilePrefab == null) return;
        GameObject missile = Instantiate(homingProjectilePrefab, attackTransform.position, attackTransform.rotation);
        if(missile != null)
        {
            missile.GetComponentInChildren<BulletCollision>().InitSet(this.GetComponent<BaseController>(), cameraEffect, this);

            cameraEffect.PlaySpawn();
        }
    }
    public void OnMissileEffect()
    {
        PlayEffect();
    }
    public void OnMissileEffectOff()
    {
        StopPlayEffect();
    }
    public void OnFireMissile()
    {
        PlaySwingSFX();
        Attack();
    }
}
