using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Text questTitle;
    [SerializeField] private Text questDescription;
    private void OnEnable()
    {
        QuestManager.OnQuestUpdate += UpdateQuestUI;
        UpdateQuestUI();
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

            // 상태값별 텍스트 출력 로직
            if (quest.State == EnumTypes.QUEST.OBJECTIVE_DONE)
            {
                questDescription.text = "목표 달성! 의뢰인에게 돌아가세요.";
            }
            else if (quest.State == EnumTypes.QUEST.IN_PROGRESS)
            {
                questDescription.text = quest.Data.Description;
            }
            else
            {
                // 의도치 않은 상태일 때 확인용
                questDescription.text = $"상태 확인 필요: {quest.State}";
            }
        }
    }
}
