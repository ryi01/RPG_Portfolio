using UnityEngine;

public class EnemyShotAttack : MeleeAttack
{
    [SerializeField] protected GameObject bulletPrefab;
    public override void Attack()
    {
        GameObject obj = CreateSomething(bulletPrefab, attackTransform);
        if (obj)
        {
            obj.GetComponentInChildren<BulletCollision>().Owner = this.GetComponent<BaseController>();
        }
    }
    public void OnShotAttack()
    {
        Attack();
    }
    
    protected GameObject CreateSomething(GameObject prefab, Transform pos)
    {
        if (prefab == null) return null;
        return Instantiate(prefab, pos.position, transform.rotation);
    }
}
