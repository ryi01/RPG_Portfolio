
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]private List<QuestInstance> activeQuest = new List<QuestInstance>();
    public static Action OnQuestUpdate;
    public static Action<QuestData> OnQuestCompleted;
    public EnumTypes.QUEST GetQueestState(QuestData data)
    {
        var quest = activeQuest.Find(p => p.Data == data);
        return (quest != null) ? quest.State : EnumTypes.QUEST.NOT_START;
    }

    public bool StartQuest(QuestData data)
    {
        if (activeQuest.Exists(q => (q.Data == data))) return false;
        QuestInstance newQuest = new QuestInstance(data);
        newQuest.SetState(EnumTypes.QUEST.IN_PROGRESS);
        activeQuest.Add(newQuest);
        OnQuestUpdate?.Invoke();
        return true;
    }
    public void CheckObjectiveComplete(QuestData data)
    {
        var quest = activeQuest.Find(p => p.Data == data);
        if(quest != null && quest.State == EnumTypes.QUEST.IN_PROGRESS)
        {
            quest.SetState(EnumTypes.QUEST.OBJECTIVE_DONE);
            OnQuestUpdate?.Invoke();
        }
    }

    public void CompletedQuest(QuestData data)
    {
        var quest = activeQuest.Find(p => p.Data == data);
        if(quest != null)
        {
            quest.SetState(EnumTypes.QUEST.COMPLETED);
            OnQuestUpdate?.Invoke();
            OnQuestCompleted?.Invoke(data);
        }
    }

    public QuestInstance GetActiveInstance()
    {
        return activeQuest.Find(q=>q.State == EnumTypes.QUEST.IN_PROGRESS ||
                                q.State == EnumTypes.QUEST.OBJECTIVE_DONE);
    }
}
