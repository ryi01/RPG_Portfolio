using UnityEngine;

public class EnemyController : BaseController
{
    protected override void Awake()
    {
        base.Awake();
        StatComp = GetComponent<EnemyStatComponent>();
    }
    public override void Damage(float damage, float force)
    {

    }
}
