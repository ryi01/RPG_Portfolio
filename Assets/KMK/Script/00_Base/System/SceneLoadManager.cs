using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private GameObject player;
    [SerializeField] private Image loadingImage;
    private void Start()
    {
        loadingCanvas.SetActive(false);
    }
    public IEnumerator ChangeSceneCor(string unloadSceneName, string loadSceneName)
    {
        if (unloadSceneName == "PersistentScene")
        {
            Debug.LogError("PersistentScene은 언로드하면 안 됩니다.");
            yield break;
        }

        loadingCanvas.SetActive(true);
        loadingImage.fillAmount = 0;

        if (dungeonGenerator != null)
        {
            dungeonGenerator.ClearDungeon();
        }

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

        Scene nextScene = SceneManager.GetSceneByName(loadSceneName);
        if(nextScene.IsValid() && nextScene.isLoaded)
        {
            SceneManager.SetActiveScene(nextScene);
        }

        Scene sceneToUnload = SceneManager.GetSceneByName(unloadSceneName);
        if(sceneToUnload.IsValid()&& sceneToUnload.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(sceneToUnload);
        }
        else
        {
            Debug.LogWarning($"언로드하려는 씬({unloadSceneName})이 이미 없거나 유효하지 않습니다.");
        }
        Vector3 finalPos = new Vector3(0, 0.3f, 0);
        if (loadSceneName.Contains("GameScene") && dungeonGenerator != null)
        {
            dungeonGenerator.GenerateDungeon();
            // [중요] 던전 생성이 끝난 후, 물리 엔진이 한 프레임 업데이트되도록 대기
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            finalPos = dungeonGenerator.WorldStartPoint + Vector3.up * 5f;

            RaycastHit hit;

            if (Physics.Raycast(finalPos, Vector3.down, out hit, 20f))
            {
                finalPos = hit.point + Vector3.up * 0.1f;
            }
            else finalPos = dungeonGenerator.WorldStartPoint;
        }
        else
        {
            Vector3 rayStart = new Vector3(0f, 5f, 0f);

            RaycastHit hit;

            if (Physics.Raycast(rayStart, Vector3.down, out hit, 20f))
            {
                finalPos = hit.point + Vector3.up * 0.1f;
            }
            else finalPos = new Vector3(0, 0.3f, 0);
        }

        yield return new WaitForFixedUpdate();

        var controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = finalPos;
            controller.enabled = true;
        }
        else
        {
            player.transform.position = finalPos;
        }
        var vcam = GameObject.FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null)
        {
            vcam.PreviousStateIsValid = false;
            vcam.transform.position = finalPos + new Vector3(0, 10, -10);
        }
        yield return new WaitForSeconds(0.2f);
        loadingCanvas.SetActive(false);
        GameManager.Instance.SoundManager.PlayBGM(EBGMType.DUNGEON);
    }
}
