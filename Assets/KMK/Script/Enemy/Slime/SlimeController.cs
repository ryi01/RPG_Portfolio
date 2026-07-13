using UnityEngine;
using UnityEngine.AI;

public class SlimeController : EnemyController
{
    [SerializeField] private int slimeLevel = 3;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private int splitCnt = 2;

    private bool isHasSplit;
    public void InitalizeSplitSlime(int newLevel, Vector3 newScale)
    {
        slimeLevel = newLevel;
        transform.localScale = newScale;
        isHasSplit = false;

        if (TryGetComponent(out Collider col)) col.enabled = true;
        if(TryGetComponent(out NavMeshAgent agent))
        {
            if (!agent.enabled) agent.enabled = transform;
            agent.isStopped = false;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }
        if(Animator != null)
        {
            Animator.Rebind();
            Animator.Update(0);
            Animator.SetBool("Death", false);
            Animator.SetInteger("State", 0);
        }
        if (StatComp != null)
        {
            StatComp.ResetMat();
            StatComp.ResetStateForSpawn();
        }
        TransitionToState(EnumTypes.STATE.IDLE);
    }

    /// <summary>
    /// 단순하게 자신의 객체를 복사하기에 currentState도 복사함 => 초기화 하는 함수필요
    /// </summary>
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
                    sc.InitalizeSplitSlime(slimeLevel - 1, transform.localScale * 0.8f);
                }
            }
        }
        Destroy(gameObject, 0.2f);
    }

    public override void OnDeathEntered(object data = null)
    {
        base.OnDeathEntered(data);
        if (isHasSplit) return;
        isHasSplit = true;
        NavigationStop();

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        SplitSlime();
    }
}
