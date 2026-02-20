using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private StatUI playerHUD;
    [SerializeField] private GameObject enemyHPBarPrefab;
    private StatUI enemyStatUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void BindPlayerUI(PlayerStatComponent player)
    {
        player.OnHpChanged += playerHUD.UpdateHP;
        playerHUD.UpdateHP(player.CurrentHP, player.MaxHP);
        player.OnChangeST += playerHUD.UpdateST;
        playerHUD.UpdateST(player.CurrentST, player.MaxST);
    }

    public void CreateEnemyHPBar(CharacterStatComponent enemyStat)
    {
        GameObject go = Instantiate(enemyHPBarPrefab, enemyStat.transform);
        go.transform.localPosition = new Vector3(0, 2.5f, 0);
        enemyStatUI = go.GetComponent<StatUI>();
        enemyStat.OnHpChanged += enemyStatUI.UpdateHP;
        enemyStatUI.UpdateHP(enemyStat.CurrentHP, enemyStat.MaxHP);
    }
    
    public void UnBindPlayerUI(PlayerStatComponent player)
    {
        player.OnHpChanged -= playerHUD.UpdateHP;
        player.OnChangeST -= playerHUD.UpdateST;
    }

    public void UnBindEnemyUI(CharacterStatComponent enemyStat)
    {
        enemyStat.OnHpChanged -= enemyStatUI.UpdateHP;
    }
}
