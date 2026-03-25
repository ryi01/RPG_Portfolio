using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InputSkill : MonoBehaviour
{
    private PlayerController pc;
    private int[] hashSkillAttacks;

    public enum SKILLS { NONE = -1, SKILL1, SKILL2, SKILL3, SKILL4, SKILL5, SKILL6 };
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
            skillAttacks[i].SetSkillIcon();
        }
        
    }

    public void ActiveSkill(SKILLS skillTypes = SKILLS.SKILL3)
    {
        PlayerSkillAttack skill = skillAttacks[(int)skillTypes];
        if (!skill.IsUnlocked || skill.IsSkill) return;
        skill.StartSkill();
        pc.Animator.SetBool(hashSkillAttacks[(int)skillTypes], true);
    }

    public void OnSkill3End()
    {
        DeActiveSkill();
    }

    public void UnlockSkill(int level)
    {
        for(int i = 0; i < skillAttacks.Length;i++)
        {
            if (skillAttacks[i].UnLockLevel == level)
            {
                skillAttacks[i].UnLockSkill();
                skillAttacks[i].SetSkillIcon();
            }
        }
    }
    public void UnlockByReward(SKILLS skillType)
    {
        if (skillType == SKILLS.NONE) return;
        int index = (int)skillType;
        if (index < 0 || index >= skillAttacks.Length) return;
        if (skillAttacks[index].IsUnlocked) return;
        skillAttacks[index].UnLockSkill();
        skillAttacks[index].SetSkillIcon();
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
    public void ExecuteSkill(SKILLS type)
    {
        skillAttacks[(int)type].Attack();
    }
}
