using UnityEngine;

// 플레이어 경험치, 레벨, 골드 시스템
public class DataManager : MonoBehaviour
{
    public int Gold { get; set; }
    public int Level { get; set; }
    public int CurrentExp { get; set; }

    public void ChangeGold(int amount)
    {
        Gold += amount;
    }
    public void SaveData()
    {
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.SetInt("Level", Level);
        PlayerPrefs.SetInt("Exp", CurrentExp);
        PlayerPrefs.Save();
    }

    public void GetData()
    {
        Gold = PlayerPrefs.GetInt("Gold", 0);
        Level = PlayerPrefs.GetInt("Level", 1);
        CurrentExp = PlayerPrefs.GetInt("Exp", 0);
    }
}
