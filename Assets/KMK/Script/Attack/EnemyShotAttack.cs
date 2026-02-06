using UnityEngine;

public class EnemyShotAttack : MeleeAttack
{
    [SerializeField] protected GameObject bulletPrefab;
    public override void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab, attackTransform.position, transform.rotation);
        if(bullet)
        {
            bullet.GetComponentInChildren<BulletCollision>().Owner = this.gameObject;
        }
    }
    public void OnShotAttack()
    {
        Attack();
    }
}
