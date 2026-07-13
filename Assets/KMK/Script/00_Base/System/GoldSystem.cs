using System;
using UnityEngine;

public class GoldSystem : MonoBehaviour
{
    private int gold;
    public int CurrentGold => gold;

    public Action<int> OnGoldChanged;

    public void InitializeGold(int startGold)
    {
        gold = Mathf.Max(0, startGold);
        OnGoldChanged?.Invoke(gold);
    }
    public void AddGold(int getGold)
    {
        if (getGold <= 0) return;
        gold += getGold;
        if(GameManager.Instance != null && GameManager.Instance.DataManager != null)
        {
            GameManager.Instance.DataManager.SetGold(gold);
            GameManager.Instance.DataManager.SaveData();
        }
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
        if (GameManager.Instance != null && GameManager.Instance.DataManager != null)
        {
            GameManager.Instance.DataManager.SetGold(gold);
            GameManager.Instance.DataManager.SaveData();
        }
        OnGoldChanged?.Invoke(gold);
        return true;
    }    
}
