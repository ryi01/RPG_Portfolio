using System;
using UnityEngine;

public class BossQuestComponent : MonoBehaviour
{
    private EnemyController controller;
    public static Action OnBossDeath;
    private QuestData currentQuestData;
    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    public void HandleBossDeath()
    {
        if (controller.BossLightning != null) controller.BossLightning.StopPattern();

        OnBossDeath?.Invoke();
        if (GameManager.Instance.QuestManager == null || currentQuestData == null) return;
        GameManager.Instance.QuestManager.AddProgress(currentQuestData, 1);
    }

    public void SetQuestData(QuestData data)
    {
        currentQuestData = data;
    }
}
