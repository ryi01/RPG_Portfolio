using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject skillCanvas;

    private void Start()
    {
        ClickPlayButt(true);
    }
    #region ½ºÅ¸Æ® ui
    public void OnClickStart()
    {
        ClickPlayButt(false);
    }
    private void ClickPlayButt(bool isOn)
    {
        startCanvas.SetActive(isOn);
        skillCanvas.SetActive(!isOn);
    }

    public void OnClickSetting()
    {

    }

    public void OnClickExit()
    {
        Application.Quit();
    }
    #endregion
}
