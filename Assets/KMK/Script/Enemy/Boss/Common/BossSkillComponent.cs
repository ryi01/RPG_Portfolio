using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossSkillComponent : MonoBehaviour
{
    [SerializeField] private EnemySkillAttack[] skillList;
    [SerializeField] private float attackCooldown = 2f;

    private EnemyController controller;
    private Coroutine cooldownCoroutine;

    public EnemySkillAttack[] SkillList => skillList;
    public int LastSkillIndex { get; set; } = -1;
    public bool CoolTimeAttack { get; private set; }

    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }
    public bool CanAttack()
    {
        return !CoolTimeAttack && skillList != null && skillList.Length > 0f;
    }
    public EnemySkillAttack GetNextSkill()
    {
        if (skillList == null || skillList.Length == 0) return null;
        int nextIndex = Random.Range(0, skillList.Length);

        if(skillList.Length > 1)
        {
            while(nextIndex == LastSkillIndex)
            {
                nextIndex = Random.Range(0, skillList.Length);
            }
        }
        LastSkillIndex = nextIndex;
        return skillList[nextIndex];
    }

    public void ExccuteAttack(EnemySkillAttack skill)
    {
        if (CoolTimeAttack || skill == null) return;

        controller.NavigationStop();
        controller.TransitionToState(EnumTypes.STATE.ATTACK, skill);

        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);

        cooldownCoroutine = StartCoroutine(AttackCoolTimeRoutine(attackCooldown));
    }
    private IEnumerator AttackCoolTimeRoutine(float delay)
    {
        CoolTimeAttack = true;
        yield return new WaitForSeconds(delay);
        CoolTimeAttack = false;
    }

}
