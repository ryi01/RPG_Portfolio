using System.Collections;
using UnityEngine;

public class InputSkill : MonoBehaviour
{
    private PlayerController pc;
    private int[] hashSkillAttacks;

    public enum SKILLS { SKILL1, SKILL2, SKILL3, SKILL4, SKILL5, SKILL6};

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

    public void ActiveSkill(SKILLS skillTypes = SKILLS.SKILL3)
    {
        skillAttacks[(int)skillTypes].StartSkill();
        pc.Animator.SetBool(hashSkillAttacks[(int)skillTypes], true);
    }

    public void OnSkill3End()
    {
        DeActiveSkill();
    }
    public void DeActiveSkill(SKILLS skillTypes = SKILLS.SKILL3)
    {
        pc.Animator.SetBool(hashSkillAttacks[(int)skillTypes], false);
    }
    public bool CurrentSkillActive(SKILLS types)
    {
        return skillAttacks[(int)types].IsSkill;
    }
    public bool IsSkillAnimation(SKILLS skillTypes)
    {
        string tag = skillAttacks[(int)skillTypes].skillHashName;
        AnimatorStateInfo layer0 = pc.Animator.GetCurrentAnimatorStateInfo(0);
        return layer0.IsTag(tag);
    }

    public IEnumerator WaitSkill(SKILLS currentSkill)
    {
        yield return new WaitForSeconds(skillAttacks[(int)currentSkill].AttackTime);
        DeActiveSkill(currentSkill);
    }
    public void ExcuteSkill(SKILLS type)
    {
        skillAttacks[(int)type].Attack();
    }
    private void OnEnable()
    {
        for (int i = 0; i < skillAttacks.Length; i++)
        {
            // 아이콘 세팅
            skillAttacks[i].SetSkillIcon();
        }
    }
}
