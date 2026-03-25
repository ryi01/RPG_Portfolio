
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private List<QuestData> questDatabase = new List<QuestData>();
    [SerializeField] private int firstQuestID = 1001;


    private List<QuestInstance> activeQuest = new List<QuestInstance>();
    private List<QuestInstance> completeQuest = new List<QuestInstance>();

    private Dictionary<int, QuestData> questDict = new Dictionary<int, QuestData>();
    private HashSet<int> availableQuestIDs = new HashSet<int>();

    public static Action OnQuestUpdate;

    public static Action<QuestData> OnQuestCompleted;

    private void Awake()
    {
        BuildDictionary();
        availableQuestIDs.Add(firstQuestID);
    }
    private void BuildDictionary()
    {
        questDict.Clear();

        foreach(var quest in questDatabase)
        {
            if (questDict.ContainsKey(quest.QuestID)) continue;
            questDict.Add(quest.QuestID, quest);
        }
    }
    public bool IsQuestAvailable(QuestData data)
    {
        if (data == null) return false;
        return availableQuestIDs.Contains(data.QuestID);
    }
    public EnumTypes.QUEST GetQuestState(QuestData data)
    {
        if (data == null) return EnumTypes.QUEST.NOT_START;
        var active = activeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        if (active != null) return active.State;
        var complete = completeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        if (complete != null) return complete.State;
        return EnumTypes.QUEST.NOT_START;
    }

    public bool StartQuest(QuestData data)
    {
        if (activeQuest.Exists(q => q.Data != null && q.Data.QuestID == data.QuestID)) return false;
        if (completeQuest.Exists(q => q.Data != null && q.Data.QuestID == data.QuestID)) return false;

        activeQuest.Add(new QuestInstance(data));

        OnQuestUpdate?.Invoke();
        return true;
    }
    public void AddProgress(QuestData data, int amount = 1)
    {
        var quest = activeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        if (quest == null) return;

        quest.AddProgress(amount);

        OnQuestUpdate?.Invoke();
    }

    public void CompletedQuest(QuestData data)
    {
        var quest = activeQuest.Find(q => q.Data != null && q.Data.QuestID == data.QuestID);
        if (quest == null) return;
        if (quest.State != EnumTypes.QUEST.OBJECTIVE_DONE) return;
        quest.SetState(EnumTypes.QUEST.COMPLETED);
        activeQuest.Remove(quest);
        completeQuest.Add(quest);

        UnlockNextQuest(data);

        OnQuestUpdate?.Invoke();
        OnQuestCompleted?.Invoke(data);
    }
    private void UnlockNextQuest(QuestData data)
    {
        if (data == null) return;
        if (data.NextQuestID == 0) return;
        if(questDict.ContainsKey(data.NextQuestID))
        {
            availableQuestIDs.Add(data.NextQuestID);
        }
    }

    public bool IsFirstQuest(QuestData data)
    {
        if (data == null) return false;
        return data.QuestID == firstQuestID;
    }
    public QuestData GetCurrentQuestData()
    {
        QuestInstance active = GetActiveInstance();
        return active != null ? active.Data : null;
    }
    public QuestInstance GetActiveInstance()
    {
        return activeQuest.Find(q=>q.State == EnumTypes.QUEST.IN_PROGRESS ||
                                q.State == EnumTypes.QUEST.OBJECTIVE_DONE);
    }
}
