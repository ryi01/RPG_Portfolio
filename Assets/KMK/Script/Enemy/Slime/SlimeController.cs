using UnityEngine;

public class SlimeController : EnemyController
{
    [SerializeField] private int slimeLevel = 3;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private int splitCnt = 2;

    public override void Damage(float damage, float force)
    {
        base.Damage(damage, force);
        if (StatComp.CurrentHP <= 0)
        {
            SplitSlime();
            GetComponent<Collider>().enabled = false;
            
        }
    }

    private void SplitSlime()
    {
        if (slimeLevel > 1)
        {
            for (int i = 0; i < splitCnt; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                GameObject slime = Instantiate(slimePrefab, spawnPos, Quaternion.identity);
                SlimeController sc;
                if (slime.TryGetComponent<SlimeController>(out sc))
                {
                    sc.slimeLevel = slimeLevel - 1;
                    sc.transform.localScale = transform.localScale * 0.6f;
                }
            }
        }
        Destroy(gameObject, 0.5f);
    }
}
