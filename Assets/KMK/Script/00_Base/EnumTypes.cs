using UnityEngine;

public class EnumTypes
{
    public enum ITEM_TYPE { CB }
    public enum WP_TYPE { ARMOR, MELEE }
    public enum CB_TYPE { GEM_UP, HP_UP, MP_UP }
    public enum STATE 
    { 
        IDLE, WANDER, DETECT, ATTACK, RETURN, DAMAGE, STUN, DEATH, PATTERN_PHASE
    }
    public enum QUEST
    {
        NOT_START, IN_PROGRESS, OBJECTIVE_DONE, COMPLETED
    }

    public enum ItemUIMode
    {
        Use,
        ShopSell,
        StoreBuy,
        BoxLoot,
        BoxStorage
    }

}
