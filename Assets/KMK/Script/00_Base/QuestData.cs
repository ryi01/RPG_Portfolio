using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [SerializeField] private int questID;
    [SerializeField] private string questTitle;
    [SerializeField][TextArea] private string description;
    [SerializeField] private int targetCount;
    [SerializeField] private GameObject rewardItem;
    [SerializeField] private InputSkill.SKILLS rewardSkill;

    public int QuestID => questID;
    public string QuestTitle => questTitle;
    public string Description => description;
    public int TargetCount => targetCount;
    public GameObject RewardItem => rewardItem;

    public InputSkill.SKILLS RewardSkill => rewardSkill;
}

public class QuestInstance
{
    public QuestData Data { get; private set; }
    public EnumTypes.QUEST State { get; private set; }
    public QuestInstance(QuestData data)
    {
        this.Data = data;
        this.State = EnumTypes.QUEST.IN_PROGRESS;
    }

    public void SetState(EnumTypes.QUEST newState)
    {
        this.State = newState;
    }
}
