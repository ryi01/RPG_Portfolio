using UnityEngine;

public class EnemyShotAttack : MeleeAttack
{
    [SerializeField] protected GameObject bulletPrefab;
    public override void Attack()
    {
        GameObject obj = CreateSomething(bulletPrefab, attackTransform);
        if (obj)
        {
            obj.GetComponentInChildren<BulletCollision>().Owner = this.gameObject;
        }
    }
    public void OnShotAttack()
    {
        Attack();
    }
    
    protected GameObject CreateSomething(GameObject prefab, Transform pos)
    {
        return Instantiate(prefab, pos.position, transform.rotation);
    }
}
