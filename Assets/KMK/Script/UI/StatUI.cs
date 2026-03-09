using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private Image expBar;
    [SerializeField] private Text levelText;

    public virtual void UpdateHP(float cur, float max)
    {
        float ratio = cur / max;
        hpBar.fillAmount = ratio;
        if (ratio <= 0)
        {
            hpBar.fillAmount = 0;
            gameObject.SetActive(false);
        }
    }

    public void UpdateExp(float cur, float max)
    {
        float ratio = cur / max;
        expBar.fillAmount = ratio;
    }

    public void UpdateLevel(int level)
    {
        levelText.text = level.ToString();
    }

}
