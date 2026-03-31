using NUnit.Framework;
using System.Collections.Generic;
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
    public List<float> requiredExpByLevel;
}
