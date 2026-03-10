using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : InteractionObject
{
    [SerializeField] private string uiCanvasRootName;
    [SerializeField] private QuestData myQuestData;
    [SerializeField] private int myStageIndex;
    [SerializeField] private int startDialogueIndex = 0;
    [SerializeField] private int inProgressDialogueIndex = 1;
    [SerializeField] private int objectiveDoneDialogueIndex = 2;
    [SerializeField] private int completeDialogueIndex = 3;

    [SerializeField] private GameObject uiPrefab; // 머리 위 아이콘 오브젝트
    [SerializeField] private Sprite notStartSprite; // 수락 가능 아이콘
    [SerializeField] private Sprite doneSprite;     // 완료 보고 아이콘
    [SerializeField] private Sprite inProgressSprite;    

    public static Action<EnumTypes.QUEST> OnQuestDialogueFinish;

    private EnumTypes.QUEST currentState;
    private Transform uiCanvasRoot;
    private NPCUI npcUI;
    private bool isPortalSpawned = false;
    private void Start()
    {
        uiCanvasRoot = GameObject.Find(uiCanvasRootName).transform;
        npcUI = Instantiate(uiPrefab, uiCanvasRoot).GetComponent<NPCUI>();
        npcUI.SetUpUi(transform, 3f);
    }
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
            npcUI.SetIconSprites(notStartSprite);
        }
        else if (state == EnumTypes.QUEST.OBJECTIVE_DONE)
        {
            npcUI.SetIconSprites(doneSprite);
        }
        else if(state == EnumTypes.QUEST.IN_PROGRESS)
        {
            npcUI.SetIconSprites(inProgressSprite);
        }
        else
        {
            npcUI.SetActiveIcon(false);
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
        switch (currentState)
        {
            case EnumTypes.QUEST.NOT_START:
                if (questManager.StartQuest(myQuestData) && !isPortalSpawned) SpawnPortal();
                break;
            case EnumTypes.QUEST.OBJECTIVE_DONE:
                questManager.CompletedQuest(myQuestData);
                break;
        }
        UpdateIconUI();
    }
    private void SpawnPortal()
    {
        GameManager.Instance.SpawnPortal("GameScene", transform.position + Vector3.right * 2);
        isPortalSpawned = true;
    }

    public override void Interact(PlayerController player)
    {
        player.MovementComp.LookAtInstant((transform.position - player.transform.position).normalized);
        player.TryInteract(this);
    }
}
