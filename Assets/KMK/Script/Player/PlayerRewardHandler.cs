using UnityEngine;

public class PlayerRewardHandler : MonoBehaviour
{
    PlayerController pc;
    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        QuestManager.OnQuestCompleted += HandleQuestCompleted;
    }
    private void OnDisable()
    {
        QuestManager.OnQuestCompleted -= HandleQuestCompleted;    
    }

    private void HandleQuestCompleted(QuestData data)
    {
        if (data == null) return;

        if (data.RewardSkill != InputSkill.SKILLS.NONE)
        {
            pc.SkillComp.UnlockByReward(data.RewardSkill);
        }
        if(data.RewardItem.ItemType == EnumTypes.ITEM_TYPE.CB)
        {
            GameManager.Instance.InventroySystem.AddItem(data.RewardItem);
        }
    }
}
