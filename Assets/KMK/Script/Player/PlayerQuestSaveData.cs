using System;
[Serializable]
public class PlayerQuestSaveData
{
    public int Id;
    public int PlayerId;
    public int QuestId;
    public int CurrentCount;
    public int IsAccepted;
    public int IsCompleted;
    public int IsReward;
}
