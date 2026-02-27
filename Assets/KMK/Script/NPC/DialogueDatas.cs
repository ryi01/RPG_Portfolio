using System;
using UnityEngine;

public enum DIR { LEFT, RIGHT }
[Serializable]
// 대화 데이터
public struct DialogueData
{
    // 방향, 이미지 아이디, 캐릭터아이디, 대화내용을 담음
    public DIR direction;
    public int imageId;
    public int nameId;
    public string message;
}
[CreateAssetMenu(fileName = "NewDialogueData", menuName = "DialogueSystem/DialogueData")]
public class DialogueDatas : ScriptableObject
{
    [SerializeField] private Sprite[] npcSprites;
    [SerializeField] private string[] npcNames;
    [SerializeField] private DialogueData[] dialogueDatas;

    public Sprite[] NPCSprites => npcSprites;
    public string[] NCPName => npcNames;
    public DialogueData[] Datas => dialogueDatas;
}
