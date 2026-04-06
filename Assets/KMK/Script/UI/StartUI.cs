using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject skillCanvas;
    [SerializeField] private GameObject settingCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject I_Back;

    [SerializeField] private GameObject goCanvas;
    [SerializeField] private PlayerController playerController;
    private void OnEnable()
    {
        if (playerController != null)
            playerController.OnPlayerDeath += OpenDeathUI;
    }

    private void OnDisable()
    {
        if (playerController != null)
            playerController.OnPlayerDeath -= OpenDeathUI;
    }

    private void Start()
    {
        CloseDeathUI();
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
    #region 게임 오버
    private void OpenDeathUI()
    {
        goCanvas.SetActive(true);
    }

    public void CloseDeathUI()
    {
        goCanvas.SetActive(false);
    }

    public void OnClickRetry()
    {
        CloseDeathUI();
        GameManager.Instance.RetryFromDungeonStart();
    }
    public void OnClickDeathMain()
    {
        CloseDeathUI();
        GameManager.Instance.ReturnToMainMenu();
    }
    #endregion
}
