using System;
using UnityEngine;

// 카메라 위치 변경 : follow offset (7, 5, -5)
// lookattarget -> dialoguePos
public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private DialogueDataBase dialogueDB;
    [SerializeField] private int stageIndex;

    public static Action<DialogueData, DialogueDatas> OnLoadDialogue;

    private DialogueDatas currentDialogueDatas;

    private int currentDialogueIndex;
    private void OnEnable()
    {
        DialogueUI.OnRequestNext += NextLoadDialogueData;
    }
    private void OnDisable()
    {
        DialogueUI.OnRequestNext -= NextLoadDialogueData;
    }

    public void LoadCurrentDialogueDatas(int stageIndex, int dialogueIndex)
    {
        GameManager.Instance.ChangeState(GameState.Dialogue);
        // 현재 스테이지 위치와 로드할 대화 내용
        this.stageIndex = stageIndex;
        this.currentDialogueIndex = dialogueIndex;
        // 현재 스테이지 번호에 맞는 대화내용 불러오기
        currentDialogueDatas = dialogueDB.DB[stageIndex];
        LoadCurrentDialogue(currentDialogueIndex);
    }

    public void LoadCurrentDialogue(int index)
    {
        DialogueData currentData = currentDialogueDatas.Datas[index];

        Debug.Log($"{currentData.direction} : {currentData.nameId}: {currentData.message}");

        OnLoadDialogue?.Invoke(currentData, currentDialogueDatas);
    }

    public void NextLoadDialogueData()
    {
        LoadCurrentDialogue(++currentDialogueIndex);
    }

}
