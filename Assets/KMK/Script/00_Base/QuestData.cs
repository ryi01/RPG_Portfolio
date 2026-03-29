using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest Data")]
public class QuestData : ScriptableObject
{
    [SerializeField] private int questID;
    [SerializeField] private int nextQuestID;
    [SerializeField] private string questTitle;
    [SerializeField][TextArea] private string description;
    [SerializeField] private int targetCount;
    [SerializeField] private ItemInfo rewardItem;
    [SerializeField] private InputSkill.SKILLS rewardSkill;
    [SerializeField] private GameObject bossPrefab;

    public int QuestID => questID;
    public int NextQuestID => nextQuestID;
    public string QuestTitle => questTitle;
    public string Description => description;
    public int TargetCount => targetCount;
    public GameObject BossPrefab => bossPrefab;
    public ItemInfo RewardItem => rewardItem;

    public InputSkill.SKILLS RewardSkill => rewardSkill;
}

public class QuestInstance
{
    public QuestData Data { get; private set; }
    public EnumTypes.QUEST State { get; private set; }

    public int CurrentCount { get; private set; }
    public QuestInstance(QuestData data)
    {
        this.Data = data;
        this.State = EnumTypes.QUEST.IN_PROGRESS;

        CurrentCount = 0;
    }
    public void AddProgress(int amount = 1)
    {
        if (State != EnumTypes.QUEST.IN_PROGRESS) return;
        CurrentCount += amount;
        if(CurrentCount >= Data.TargetCount)
        {
            CurrentCount = Data.TargetCount;
            State = EnumTypes.QUEST.OBJECTIVE_DONE;
        }
    }
    public void SetState(EnumTypes.QUEST newState)
    {
        this.State = newState;
    }
}
