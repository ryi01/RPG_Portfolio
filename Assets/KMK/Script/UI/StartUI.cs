using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject skillCanvas;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject I_Back;

    private void Start()
    {
        ClickPlayButt(true);
        settingCanvas.SetActive(false);
    }
    #region ½ºÅ¸Æ® ui
    public void OnClickStart()
    {
        ClickPlayButt(false);
    }
    private void ClickPlayButt(bool isOn)
    {
        startCanvas.SetActive(isOn);
        I_Back.SetActive(isOn);
        skillCanvas.SetActive(!isOn);
    }

    public void OnClickSetting()
    {
        startCanvas.SetActive(false);
        settingCanvas.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnDoneSetting()
    {
        startCanvas.SetActive(true);
        settingCanvas.SetActive(false);
    }

    #endregion
}
