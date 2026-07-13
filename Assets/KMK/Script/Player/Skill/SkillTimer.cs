using UnityEngine;
using UnityEngine.UI;

public class SkillTimer : MonoBehaviour
{
    [SerializeField] private Image filledImage;
    [SerializeField] private Image skillIconImage;
    [SerializeField] private GameObject skillMaskImage;
    private float timer;
    private bool isTimerRun = false;
    private float timeDuration;
    private PlayerSkillAttack skillAttack;
    private void Awake()
    {
        filledImage.fillAmount = 0;
    }
    public void DeleteMaskImage()
    {
        if (skillMaskImage == null) return;
        skillMaskImage.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(isTimerRun)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                if (filledImage != null)
                {
                    filledImage.fillAmount = timer / timeDuration;
                }
            }
            else
            {
                EndTimer();
            }
        }
    }

    public void StartTimer(PlayerSkillAttack sa, float td)
    {
        this.skillAttack = sa;
        this.timeDuration = td;
        timer = td; 
        isTimerRun = true;
    }

    public void EndTimer()
    {
        isTimerRun = false;
        if (filledImage != null)
        {
            filledImage.fillAmount = 0f;
        }

        skillAttack.EndSkill();
    }

    public void SetSkillIcon(Sprite sprite)
    {
        skillIconImage.sprite = sprite;
    }
}
