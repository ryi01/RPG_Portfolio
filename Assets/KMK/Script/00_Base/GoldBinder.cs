using UnityEngine;

public class GoldBinder : MonoBehaviour
{
    [SerializeField] private GoldSystem goldSystem;
    [SerializeField] private DataManager dataManager;
    [SerializeField] private UIManager uiManager;

    private void OnEnable()
    {
        if (goldSystem == null) return;
        goldSystem.OnGoldChanged += HandleGoldChanged;
    }
    private void OnDisable()
    {
        if (goldSystem == null) return;
        goldSystem.OnGoldChanged -= HandleGoldChanged;
    }
    private void HandleGoldChanged(int amount)
    {
        dataManager?.ChangeGold(amount); 
        uiManager?.ChangeGold(amount);
    }

}
