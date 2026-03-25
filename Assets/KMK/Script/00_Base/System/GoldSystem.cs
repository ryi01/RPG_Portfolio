using System;
using UnityEngine;

public class GoldSystem : MonoBehaviour
{
    private int gold;
    public int CurrentGold => gold;

    public Action<int> OnGoldChanged;

    public void AddGold(int getGold)
    {
        if (getGold <= 0) return;
        gold += getGold;
        OnGoldChanged?.Invoke(gold);
    }

    public bool IsEnoughGold(int amount)
    {
        if (amount < 0) return false;   
        return gold >= amount;
    }

    public bool SpendGold(int amount)
    {
        if (amount <= 0) return false;
        if (!IsEnoughGold(amount)) return false;
        gold -= amount;
        OnGoldChanged?.Invoke(gold);
        return true;
    }    
}
