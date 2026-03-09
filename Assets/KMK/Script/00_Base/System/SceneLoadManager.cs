using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Image loadingImage;
    private void Start()
    {
        loadingCanvas.SetActive(false);
    }
    public IEnumerator ChangeSceneCor(string unloadSceneName, string loadSceneName)
    {
        loadingCanvas.SetActive(true);
        loadingImage.fillAmount = 0;

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        float timer = 0;
        float minLoadingTime = 2;
        while(timer < minLoadingTime || op.progress < 0.9f)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(Mathf.Min(op.progress / 0.9f, timer / minLoadingTime));

            loadingImage.fillAmount = progress;

            yield return null;
        }
        op.allowSceneActivation = true;
        while(!op.isDone) yield return null;

        yield return SceneManager.UnloadSceneAsync(unloadSceneName);

        Vector3 finalPos = Vector3.zero;

        if (loadSceneName == "GameScene")
        {
            GameManager.Instance.DungeonGenerator.GenerateDungeon();
        }
        finalPos = (loadSceneName == "GameScene") ? GameManager.Instance.DungeonGenerator.WorldStartPoint : new Vector3(0, 0.5f, 0);
        GameObject.FindWithTag("Player").transform.position = finalPos;
        var vcam = GameObject.FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.PreviousStateIsValid = false;
            vcam.transform.position = finalPos + new Vector3(0, 10, -10);
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadSceneName));
        yield return new WaitForSeconds(0.2f);
        loadingCanvas.SetActive(false);
    }
}
