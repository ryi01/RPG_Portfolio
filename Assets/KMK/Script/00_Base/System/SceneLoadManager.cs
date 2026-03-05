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
        yield return SceneManager.UnloadSceneAsync(unloadSceneName);

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
        op.allowSceneActivation = false;
        float timer = 0;
        float minLoadingTime = 2;
        while(timer < minLoadingTime || op.progress < 0.9f)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / minLoadingTime);

            loadingImage.fillAmount = progress;
            if (op.progress >= 0.9f && timer >= minLoadingTime)
            {
                break;
            }
            yield return null;
        }
        op.allowSceneActivation = true;
        while(!op.isDone) yield return null;

        yield return null;

        Vector3 finalPos = Vector3.zero;

        if (loadSceneName == "GameScene")
        {
            GameManager.Instance.DungeonGenerator.GenerateDungeon();
        }
        finalPos = (loadSceneName == "GameScene") ? GameManager.Instance.DungeonGenerator.WorldStartPoint : new Vector3(0, 1.5f, 0);
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
