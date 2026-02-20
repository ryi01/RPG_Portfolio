using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image stBar;

    public void UpdateHP(float cur, float max)
    {
        float ratio = cur / max;
        hpBar.fillAmount = ratio;
        if (ratio <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateST(float cur, float max)
    {
        float ratio = cur / max;
        stBar.fillAmount = ratio;

    }

}
