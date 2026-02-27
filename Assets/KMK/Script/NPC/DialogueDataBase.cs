using UnityEngine;
[CreateAssetMenu(fileName = "NewDialogueDB", menuName = "DialogueSystem/DialogueDB")]
public class DialogueDataBase : ScriptableObject
{
    [SerializeField] private DialogueDatas[] dialgogueDB;
    public DialogueDatas[] DB => dialgogueDB;
}
