using System.Collections;
using UnityEngine;

public class InputSkill : MonoBehaviour
{
    private PlayerController pc;
    private int[] hashSkillAttacks;

    public enum SKILLS { SKILL1, SKILL2, SKILL3, SKILL4, SKILL5, NONE };

    [SerializeField] private PlayerSkillAttack[] skillAttacks;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }
    private void Start()
    {
        hashSkillAttacks = new int[skillAttacks.Length];
        for (int i = 0; i < skillAttacks.Length; i++)
        {
            hashSkillAttacks[i] = Animator.StringToHash(skillAttacks[i].skillHashName);
        }
    }

    public void ActiveSkill(SKILLS skillTypes)
    {
        skillAttacks[(int)skillTypes].StartSkill();
        pc.Animator.SetBool(hashSkillAttacks[(int)skillTypes], true);
    }
    public void ActiveSkill3(SKILLS skillTypes)
    {
        skillAttacks[(int)skillTypes].StartSkill();
        pc.Animator.SetTrigger(hashSkillAttacks[(int)skillTypes]);     
    }
    public void DeActiveSkill(SKILLS skillTypes)
    {
        skillAttacks[(int)skillTypes].EndSkill();
        pc.Animator.SetBool(hashSkillAttacks[(int)skillTypes], false);
    }
    public bool CurrentSkillActive(SKILLS types)
    {
        return skillAttacks[(int)types].IsSkill;
    }
    public bool IsSkillAnimation(SKILLS skillTypes)
    {
        AnimatorStateInfo stat = pc.Animator.GetCurrentAnimatorStateInfo(1);
        return stat.IsTag(skillAttacks[(int)skillTypes].skillHashName);
    }

    public IEnumerator WaitSkill(SKILLS currentSkill)
    {
        yield return new WaitForSeconds(skillAttacks[(int)currentSkill].WaitSkillTime);
        DeActiveSkill(currentSkill);
    }
    public void ExcuteSkill(SKILLS type)
    {
        skillAttacks[(int)type].Attack();
    }
    public void OnSkillEnd()
    {
        pc.Animator.SetLayerWeight(1, 0);
    }
}
