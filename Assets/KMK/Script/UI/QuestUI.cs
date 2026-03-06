using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Text questTitle;
    [SerializeField] private Text questDescription;
    private void OnEnable()
    {
        QuestManager.OnQuestUpdate += UpdateQuestUI;
        questTitle.text = "퀘스트 없음";
        questDescription.text = "";
    }
    private void OnDisable()
    {
        QuestManager.OnQuestUpdate -= UpdateQuestUI;
    }
    private void UpdateQuestUI()
    {
        var quest = GameManager.Instance.QuestManager.GetActiveInstance();
        if (quest == null)
        {
            questTitle.text = "퀘스트 없음";
            questDescription.text = "";
        }
        else
        {
            questTitle.text = quest.Data.QuestTitle;
            if (quest.State == EnumTypes.QUEST.OBJECTIVE_DONE) questDescription.text = "목표 달성! 의뢰인에게 돌아가세요.";
            else questDescription.text = quest.Data.Description;
        }
    }
}
