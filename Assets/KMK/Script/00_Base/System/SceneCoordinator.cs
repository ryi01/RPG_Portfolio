using System.Collections;
using UnityEngine;

public class SceneCoordinator : MonoBehaviour
{
    private const string PersistentSceneName = "PersistentScene";
    private string currentContentScene = "VillageScene";

    [SerializeField] private SceneLoadManager sceneLoadManager;
    [SerializeField] private CameraEnviroment cameraEnviroment;

    public bool IsPathFindingEnable { get; private set; } = false;
    private void Awake()
    {
        cameraEnviroment.ChangeToTown();
        currentContentScene = FindCurrentContentScene();
        IsPathFindingEnable = currentContentScene.Contains("Game");
    }
    public void ChangeScene(string loadSceneName)
    {
        if (string.IsNullOrEmpty(loadSceneName) || loadSceneName == PersistentSceneName || currentContentScene == loadSceneName) return;

        bool isDungeon = loadSceneName.Contains("Game");
        GameManager.Instance.ChangeState(isDungeon ? GameState.Dungeon : GameState.Town);
        if (isDungeon) cameraEnviroment.ChangeToDungeon();
        else cameraEnviroment.ChangeToTown();
        IsPathFindingEnable = isDungeon;
        StartCoroutine(ChangeSceneRoutine(loadSceneName));
    }

    private IEnumerator ChangeSceneRoutine(string loadSceneName)
    {
        string perviousScene = currentContentScene;
        yield return StartCoroutine(sceneLoadManager.ChangeSceneCor(perviousScene, loadSceneName));

        currentContentScene = loadSceneName;
    }
    public string GetCurrentContentScene()
    {
        return currentContentScene;
    }

    private string FindCurrentContentScene()
    {
        for(int i = 0; i <UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

            if (!scene.isLoaded) continue;
            if (scene.name == PersistentSceneName) continue;
            return scene.name;
        }
        return "VillageScene";
    }
}
