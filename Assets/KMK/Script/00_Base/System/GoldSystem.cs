using System;
using UnityEngine;

public class GoldSystem : MonoBehaviour
{
    private int gold;
    public int CurrentGold => gold;

    public Action<int> OnChangedGold;

    public void AddGold(int getGold)
    {
        gold += getGold;
        OnChangedGold?.Invoke(gold);
    }

    public bool IsEnoughGold(int amount)
    {
        if(gold < amount)
        {   
            return false;
        }
        gold -= amount;
        OnChangedGold?.Invoke(gold);
        return true;
    }
}
