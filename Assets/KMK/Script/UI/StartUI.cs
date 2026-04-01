using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject skillCanvas;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject I_Back;

    private void Start()
    {
        ShowTitleMenu(true);
        settingCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
    }

    #region 스타트 ui
    public void OnClickStart()
    {
        ShowTitleMenu(false);
        GameManager.Instance.StartGameFromTitle();
    }
    private void ShowTitleMenu(bool isOn)
    {
        startCanvas.SetActive(isOn);
        skillCanvas.SetActive(!isOn);
        I_Back.SetActive(isOn);
    }

    public void OnClickSetting()
    {
        startCanvas.SetActive(false);
        settingCanvas.SetActive(true);
    }

    public void OnClickExit()
    {
        GameManager.Instance.SaveAndQuitGame();
    }

    public void OnDoneSetting()
    {
        startCanvas.SetActive(true);
        settingCanvas.SetActive(false);
    }

    #endregion

    #region 일시정지
    public void SetPauseCanvas(bool isOn)
    {
        pauseCanvas.SetActive(isOn);
    }
    public void OnClickMain()
    {
        GameManager.Instance.SaveCurrentPlayerData();
        GameManager.Instance.EnterTitleMenu();
        ShowTitleMenu(true);
        pauseCanvas.SetActive(false);
    }
    public void OnClickResume()
    {
        ShowTitleMenu(false);
        GameManager.Instance.ResumeGame();
    }
    #endregion

}
