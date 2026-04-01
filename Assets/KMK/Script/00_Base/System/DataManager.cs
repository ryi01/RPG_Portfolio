using UnityEngine;

// 플레이어 경험치, 레벨, 골드 시스템
public class DataManager : MonoBehaviour
{
    public PlayerSaveData PlayerData { get; private set; } = new PlayerSaveData();
    public int Id => PlayerData.Id;
    public string Name => PlayerData.Name;
    public int Gold => PlayerData.Gold;
    public int Level => PlayerData.Level;
    public int CurrentExp => PlayerData.CurrentExp;
    public float CurrentHP => PlayerData.CurrentHP;

    public void SetId(int id) => PlayerData.Id = Mathf.Max(1, id);
    public void SetName(string name) => PlayerData.Name = string.IsNullOrEmpty(name) ? "Player" : name;
    public void ChangeGold(int amount) => PlayerData.Gold = Mathf.Max(0, PlayerData.Gold + amount);
    public void SetCurrentExp(int exp) => PlayerData.CurrentExp = Mathf.Max(0, exp);
    public void SetLevel(int level) => PlayerData.Level = Mathf.Max(0, level);
    public void SetCurrentHP(float hp) => PlayerData.CurrentHP = Mathf.Max(0, hp);

    private void Awake()
    {
        LoadFromDB();
    }

    public void LoadFromDB()
    {
        if (GameManager.Instance.SQLiteManager == null) return;
        PlayerData = GameManager.Instance.SQLiteManager.LoadPlayer(1);

        if(PlayerData == null)
        {
            int newId = GameManager.Instance.SQLiteManager.InsertDefaultPlayer("Player");
            PlayerData = GameManager.Instance.SQLiteManager.LoadPlayer(newId);
        }
    }

    public void SaveData()
    {
        if (GameManager.Instance.SQLiteManager == null || PlayerData == null) return;
        GameManager.Instance.SQLiteManager.UpdatePlayer(PlayerData);
    }
    public void SyncPlayerStat(CharacterStatComponent statComp)
    {
        if (statComp == null) return;
        PlayerData.CurrentHP = statComp.CurrentHP;
    }

}
