using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyHPBarPrefab;
    [SerializeField] private StatUI bossUI;
    // UI 요소 게임오브젝트 참조
    [SerializeField] protected GameObject uiGameObject;
    private Dictionary<CharacterStatComponent, EnemyStatUI> enemyUIDic = new Dictionary<CharacterStatComponent, EnemyStatUI>();

    private void Start()
    {
        bossUI.gameObject.SetActive(false);
    }
    public void CreateEnemyHPBar(CharacterStatComponent enemyStat, float y = 2f)
    {
        GameObject go = Instantiate(enemyHPBarPrefab, uiGameObject.transform);
        EnemyStatUI enemyUI = go.GetComponent<EnemyStatUI>();
        enemyUI.SetUpUi(enemyStat.transform, y);
        enemyStat.OnHpChanged += enemyUI.UpdateHP;
        enemyUI.UpdateHP(enemyStat.CurrentHP, enemyStat.MaxHP);
        enemyUI.gameObject.SetActive(true);
        enemyUIDic[enemyStat] = enemyUI;
    }
    public void SetBossHP(bool isShow)
    {
        bossUI.gameObject.SetActive(isShow);
    }
    public void ShowBossHP(CharacterStatComponent enemyStat)
    {
        enemyStat.OnHpChanged += bossUI.UpdateHP;
        bossUI.UpdateHP(enemyStat.CurrentHP, enemyStat.MaxHP);
        bossUI.gameObject.SetActive(false);
    }
    public void UnBindBoss(CharacterStatComponent enemyStat)
    {
        enemyStat.OnHpChanged -= bossUI.UpdateHP;
    }
    public void UnBindEnemyUI(CharacterStatComponent enemyStat)
    {
        if (enemyUIDic.TryGetValue(enemyStat, out EnemyStatUI ui))
        {
            if (ui != null)
            {
                enemyStat.OnHpChanged -= ui.UpdateHP;
                Destroy(ui.gameObject);
            }
            enemyUIDic.Remove(enemyStat);
        }
    }
}
