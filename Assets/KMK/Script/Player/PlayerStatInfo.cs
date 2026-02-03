using UnityEngine;

[CreateAssetMenu(menuName = "Stat/Player")]
public class PlayerStatInfo : StatInfo
{
    [Header("Critical")]
    public float criticalChance;
    public float criticalMultifle;
    [Header("Stemina")]
    public float maxST;
    public float regenST;
    [Header("Level")]
    public float level;
    public float exp;
}
