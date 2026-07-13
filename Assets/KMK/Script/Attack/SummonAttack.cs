using UnityEngine;

public class SummonAttack : EnemyShotAttack
{
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private Transform slimeTrans;
    public override void Attack()
    {
        PlaySwingSFX();
        int index = Random.Range(0, 10);
        if (index < 2) CreateSomething(slimePrefab, slimeTrans);
        else base.Attack();
    }
}
