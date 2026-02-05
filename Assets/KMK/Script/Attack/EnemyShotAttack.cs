using UnityEngine;

public class EnemyShotAttack : MeleeAttack
{
    [SerializeField] protected GameObject bulletPrefab;
    public override void Attack()
    {
        Instantiate(bulletPrefab, attackTransform.position, transform.rotation);
    }
    public void OnShotAttack()
    {
        Attack();
    }
}
