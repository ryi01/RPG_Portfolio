
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]private List<QuestInstance> activeQuest = new List<QuestInstance>();
    public static Action OnQuestUpdate;
    public EnumTypes.QUEST GetQueestState(QuestData data)
    {
        var quest = activeQuest.Find(p => p.Data == data);
        return (quest != null) ? quest.State : EnumTypes.QUEST.NOT_START;
    }

    public bool StartQuest(QuestData data)
    {
        if (activeQuest.Exists(q => (q.Data == data) || (q.State == EnumTypes.QUEST.IN_PROGRESS))) return false;
        QuestInstance newQuest = new QuestInstance(data);
        newQuest.SetState(EnumTypes.QUEST.IN_PROGRESS);
        activeQuest.Add(newQuest);
        OnQuestUpdate?.Invoke();
        Debug.Log($"{data.QuestTitle} ¼ö¶ôµÊ!");
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
            activeQuest.Remove(quest);
            OnQuestUpdate?.Invoke();
        }
    }

    public QuestInstance GetActiveInstance()
    {
        if (activeQuest.Count > 0) return activeQuest[0];
        return null;
    }
}
