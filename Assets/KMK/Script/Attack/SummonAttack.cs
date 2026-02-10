using UnityEngine;

public class SummonAttack : EnemyShotAttack
{
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Transform slimeTrans;
    public override void Attack()
    {
        int index = Random.Range(0, 2);
        switch(index)
        {
            case 0:
                base.Attack();
                break;
            case 1:
                CreateSomething(slimePrefab, slimeTrans);
                break;
        }
    }
}
