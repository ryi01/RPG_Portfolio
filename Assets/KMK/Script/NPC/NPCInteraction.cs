using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private int myStageIndex;
    [SerializeField] private int startDialogueIndex = 0;

    public void Interact()
    {
        GameManager.Instance.DialogueSystem.LoadCurrentDialogueDatas(myStageIndex, startDialogueIndex);
    }
}
