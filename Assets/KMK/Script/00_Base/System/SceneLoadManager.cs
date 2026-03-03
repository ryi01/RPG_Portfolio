using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public void ChangeScene(string unloadSceneName, string loadSceneName, Vector3 spawnPos)
    {
        StartCoroutine(ChangeSceneCor(unloadSceneName, loadSceneName, spawnPos));
    }

    IEnumerator ChangeSceneCor(string unloadSceneName, string loadSceneName, Vector3 spawnPos)
    {
        yield return SceneManager.UnloadSceneAsync(unloadSceneName);
        // ·Îµù
        yield return SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);

        yield return new WaitForSeconds(1);

        AsyncOperation op = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
        while(!op.isDone)
        {
            yield return null;
        }

        GameObject.FindWithTag("Player").transform.position = spawnPos;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadSceneName));
        yield return SceneManager.UnloadSceneAsync("LoadingScene");

    }
}
