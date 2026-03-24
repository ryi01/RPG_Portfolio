using System;
using UnityEngine;

public class BossQuestComponent : MonoBehaviour
{
    private EnemyController controller;
    public static Action OnBossDeath;
    public QuestData QuestData { get; private set; }
    private void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    public void HandleBossDeath()
    {
        if (controller.BossLightning != null) controller.BossLightning.StopPattern();

        OnBossDeath?.Invoke();

        if(GameManager.Instance.QuestManager != null && QuestData != null)
        {
            GameManager.Instance.QuestManager.CheckObjectiveComplete(QuestData);
        }
    }

    public void SetQuestData(QuestData data)
    {
        QuestData = data;
    }
}
