using UnityEngine;

public class EnumTypes
{
    public enum STATE 
    { 
        IDLE, WANDER, DETECT, ATTACK, RETURN, DAMAGE, STUN, DEATH, 
        // 보스 스킬
        SKILL_01, SKILL_02, SKILL_03, SKILL_04, PATTERN_PHASE
    }

    public enum PlayerActionState { IDLE, MOVE, ATTACK, SKILL, STUN }
}
