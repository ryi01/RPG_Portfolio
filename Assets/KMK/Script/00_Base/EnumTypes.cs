using UnityEngine;

public class EnumTypes
{
    public enum ITEM_TYPE { WP, CB }
    public enum WP_TYPE { ARMOR, MELEE }
    public enum CB_TYPE { GEM_UP, HP_UP, MP_UP }
    public enum STATE 
    { 
        IDLE, WANDER, DETECT, ATTACK, RETURN, DAMAGE, STUN, DEATH, PATTERN_PHASE
    }

}
