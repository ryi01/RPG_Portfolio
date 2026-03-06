using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private QuestData myQuestData;
    [SerializeField] private int myStageIndex;
    [SerializeField] private int startDialogueIndex = 0;
    [SerializeField] private int inProgressDialogueIndex = 1;
    [SerializeField] private int objectiveDoneDialogueIndex = 2;
    [SerializeField] private int completeDialogueIndex = 3;

    [SerializeField] private GameObject questIcon; // 머리 위 아이콘 오브젝트
    [SerializeField] private Image iconImage;       // 실제 아이콘이 들어갈 이미지
    [SerializeField] private Sprite notStartSprite; // 수락 가능 아이콘
    [SerializeField] private Sprite doneSprite;     // 완료 보고 아이콘
    [SerializeField] private Sprite inProgressSprite;    

    public static Action<EnumTypes.QUEST> OnQuestDialogueFinish;

    private EnumTypes.QUEST currentState;

    private void OnEnable()
    {
        QuestManager.OnQuestUpdate += UpdateIconUI;
    }
    private void OnDisable()
    {
        QuestManager.OnQuestUpdate -= UpdateIconUI;
    }
    private void UpdateIconUI()
    {
        var state = GameManager.Instance.QuestManager.GetQueestState(myQuestData);
        if (state == EnumTypes.QUEST.NOT_START)
        {
            questIcon.SetActive(true);
            iconImage.sprite = notStartSprite; // 느낌표
        }
        else if (state == EnumTypes.QUEST.OBJECTIVE_DONE)
        {
            questIcon.SetActive(true);
            iconImage.sprite = doneSprite;     // 물음표
        }
        else if(state == EnumTypes.QUEST.IN_PROGRESS)
        {
            questIcon.SetActive(true);
            iconImage.sprite = inProgressSprite;
        }
        else
        {
            questIcon.SetActive(false);        // 진행 중이거나 완료 상태면 숨김
        }
    }
    public void Interact()
    {
        var questManager = GameManager.Instance.QuestManager;
        currentState = questManager.GetQueestState(myQuestData);
        int targetDialogueIndex = GetDialgueIndex(currentState);

        DialogueUI.OnDialogueFinish += HandleQuestLogic;

        GameManager.Instance.DialogueSystem.LoadCurrentDialogueDatas(myStageIndex, targetDialogueIndex);
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
        currentState = questManager.GetQueestState(myQuestData);
        switch (currentState)
        {
            case EnumTypes.QUEST.NOT_START:
                questManager.StartQuest(myQuestData);
                break;
            case EnumTypes.QUEST.OBJECTIVE_DONE:
                questManager.CompletedQuest(myQuestData);
                break;
        }
    }
}
