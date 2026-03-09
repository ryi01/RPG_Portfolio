using System.Collections.Generic;
using UnityEngine;

public class EnemyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyHPBarPrefab;
    // Overlap UI 캔바스의 게임오브젝트 이름
    [SerializeField] protected string uiCanvasRootName;
    // UI 요소 게임오브젝트 참조
    protected GameObject uiGameObject;

    // Overlap UI 캔바스의 Trasform 컴포넌트
    protected Transform uiCanvasRoot;
    private Dictionary<CharacterStatComponent, EnemyStatUI> enemyUIDic = new Dictionary<CharacterStatComponent, EnemyStatUI>();
    private void Start()
    {
        uiCanvasRoot = GameObject.Find(uiCanvasRootName).transform;
    }
    public void CreateEnemyHPBar(CharacterStatComponent enemyStat, float y = 1.8f)
    {
        GameObject go = Instantiate(enemyHPBarPrefab, uiCanvasRoot);
        EnemyStatUI enemyUI = go.GetComponent<EnemyStatUI>();
        enemyUI.SetUpUi(enemyStat.transform, y);
        enemyStat.OnHpChanged += enemyUI.UpdateHP;
        enemyUI.UpdateHP(enemyStat.CurrentHP, enemyStat.MaxHP);
        enemyUI.gameObject.SetActive(true);
        enemyUIDic[enemyStat] = enemyUI;
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
