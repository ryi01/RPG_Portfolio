using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static InputSkill;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject skillUI;
    [SerializeField] private GameObject dialogueUI;
    [SerializeField] private Image[] npcImages;
    [SerializeField] private GameObject[] npcNames;
    [SerializeField] private GameObject[] nameBlock;
    [SerializeField] private GameObject nextButt;
    [SerializeField] private GameObject finishButt;
    [SerializeField] private float typingDelayTime;
    [SerializeField] private Text messageText;
    private Coroutine typingCor;
    [SerializeField] private bool isSkip;

    public static Action OnRequestNext;
    public static Action OnDialogueFinish;

    private void OnEnable()
    {
        DialogueSystem.OnLoadDialogue += ShowDialogueUI;
    }
    private void OnDisable()
    {
        DialogueSystem.OnLoadDialogue -= ShowDialogueUI;
    }
    public void ClearDilogueUI(bool isLast)
    {
        if (!isSkip || isLast) nextButt.SetActive(false);
        if (isSkip && isLast) finishButt.SetActive(true);
        for (int i = 0; i < npcNames.Length; i++)
        {
            npcImages[i].enabled = false;
            npcNames[i].SetActive(false);
            nameBlock[i].SetActive(false);
        }

        messageText.text = string.Empty;
    }
    public void SpearateData(DialogueData currentData, DialogueDatas currentDialogueDatas, bool isLast)
    {
        DIR dir = currentData.direction;
        int imageId = currentData.imageId;
        Sprite npcSprite = currentDialogueDatas.NPCSprites[imageId];
        int nameId = currentData.nameId;
        string npcName = currentDialogueDatas.NCPName[nameId];

        string message = currentData.message.Replace("\\n", "\n");

    }
    public void ShowDialogueUI(DialogueData data, DialogueDatas so)
    {
        dialogueUI.SetActive(true);
        skillUI.SetActive(false);
        ClearDilogueUI(false);
        bool isLast = data.isEnd;
        int dirIndex = (int)data.direction;
        npcImages[dirIndex].enabled = true;
        npcImages[dirIndex].sprite = so.NPCSprites[data.imageId];
        npcNames[dirIndex].SetActive(true);
        nameBlock[dirIndex].SetActive(true);
        npcNames[dirIndex].GetComponentInChildren<Text>().text = so.NCPName[data.nameId];

        if(typingCor != null)
        {
            StopCoroutine(typingCor);
        }

        typingCor = StartCoroutine(TypingDialogueCoroutine(data.message, isLast));
    }

    public IEnumerator TypingDialogueCoroutine(string message, bool isLast)
    {
        char[] messageCharArray = message.ToCharArray();
        for(int i = 0; i < messageCharArray.Length; i++)
        {
            messageText.text += messageCharArray[i];
            yield return new WaitForSeconds(typingDelayTime);
        }

        nextButt.SetActive(!isLast);
        finishButt.SetActive(isLast);
    }

    public void OnNextButt()
    {
        OnRequestNext?.Invoke();
    }

    public void OnFinishButt()
    {
        skillUI.SetActive(true);
        dialogueUI.SetActive(false);
        GameManager.Instance.ChangeState(GameState.Town);
        OnDialogueFinish?.Invoke();
    }
}
