using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class QuestNPCInteraction : BaseNPCInteraction
{
    [SerializeField] private QuestData myQuestData;

    [SerializeField] private int myStageIndex;
    [SerializeField] private int startDialogueIndex = 0;
    [SerializeField] private int inProgressDialogueIndex = 1;
    [SerializeField] private int objectiveDoneDialogueIndex = 2;
    [SerializeField] private int completeDialogueIndex = 3;
    [SerializeField] private Sprite notStartSprite; // 수락 가능 아이콘
    [SerializeField] private Sprite doneSprite;     // 완료 보고 아이콘
    [SerializeField] private Sprite inProgressSprite;    

    public static Action<EnumTypes.QUEST> OnQuestDialogueFinish;

    private EnumTypes.QUEST currentState;
    private bool isPortalSpawned = false;

    private void OnEnable()
    {
        QuestManager.OnQuestUpdate += RefreshNPCUI;
        QuestManager.OnQuestCompleted += HandleQuestComplete;
    }
    private void OnDisable()
    {
        QuestManager.OnQuestUpdate -= RefreshNPCUI;
        QuestManager.OnQuestCompleted -= HandleQuestComplete;
    }
    protected override void RefreshNPCUI()
    {
        if (npcUI == null || myQuestData == null) return;
        var qm = GameManager.Instance.QuestManager;
        if (qm == null) return;
        EnumTypes.QUEST state = qm.GetQuestState(myQuestData);
        bool isAvailable = qm.IsQuestAvailable(myQuestData);
        HideIcon();
        if (state == EnumTypes.QUEST.NOT_START && isAvailable)
        {
            SetIcon(notStartSprite);
        }
        else if (state == EnumTypes.QUEST.IN_PROGRESS)
        {
            SetIcon(inProgressSprite);
        }
        else if (state == EnumTypes.QUEST.OBJECTIVE_DONE)
        {
            SetIcon(doneSprite);
        }
        if (state == EnumTypes.QUEST.IN_PROGRESS || state == EnumTypes.QUEST.OBJECTIVE_DONE)
        {
            EnsurePortal();
        }
    }
    private void HandleQuestComplete(QuestData completeData)
    {
        if (completeData == null || myQuestData == null) return;
        if(completeData.QuestID == myQuestData.QuestID || completeData.NextQuestID == myQuestData.QuestID)
        {
            RefreshNPCUI();
        }
    }

    private int GetDialgueIndex(EnumTypes.QUEST state)
    {
        switch(state)
        {
            case EnumTypes.QUEST.NOT_START: return startDialogueIndex;
            case EnumTypes.QUEST.IN_PROGRESS: return inProgressDialogueIndex;
            case EnumTypes.QUEST.OBJECTIVE_DONE: return objectiveDoneDialogueIndex;
            case EnumTypes.QUEST.COMPLETED: return completeDialogueIndex;
            default: return startDialogueIndex;
        }
    }
    private void HandleQuestLogic()
    {
        DialogueUI.OnDialogueFinish -= HandleQuestLogic;
        var questManager = GameManager.Instance.QuestManager;
        switch (currentState)
        {
            case EnumTypes.QUEST.NOT_START:
                bool started = questManager.StartQuest(myQuestData);
                if(started)
                {
                    QuestData currentQuest = questManager.GetCurrentQuestData();
                    if(currentQuest != null && currentQuest.QuestID == myQuestData.QuestID)
                    {
                        EnsurePortal();
                    }
                }
                break;
            case EnumTypes.QUEST.OBJECTIVE_DONE:
                questManager.CompletedQuest(myQuestData);
                break;
        }
        RefreshNPCUI();
    }
    private void EnsurePortal()
    {
        if (isPortalSpawned) return;
        if (myQuestData == null || myQuestData.BossPrefab == null) return;
        GameManager.Instance.SpawnPortal("GameScene", transform.position + Vector3.right * 2);
        isPortalSpawned = true;
    }
    private bool IsInteract(QuestManager questManager)
    {
        if (questManager == null || myQuestData == null) return false;
        EnumTypes.QUEST state = questManager.GetQuestState(myQuestData);
        if (state == EnumTypes.QUEST.NOT_START && !questManager.IsQuestAvailable(myQuestData)) return false;
        return true;
    }
    public override void Interact(PlayerController player)
    {
        QuestManager questManager = GameManager.Instance.QuestManager;
        if (!IsInteract(questManager)) return;

        currentState = questManager.GetQuestState(myQuestData);
        int targetDialogueIndex = GetDialgueIndex(currentState);

        GameManager.Instance.ChangeState(GameState.Dialogue);

        DialogueUI.OnDialogueFinish -= HandleQuestLogic;
        DialogueUI.OnDialogueFinish += HandleQuestLogic;

        GameManager.Instance.DialogueSystem.LoadCurrentDialogueDatas(myStageIndex, targetDialogueIndex);
    }
}
