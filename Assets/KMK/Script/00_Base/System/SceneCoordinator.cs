using UnityEngine;

public class SceneCoordinator : MonoBehaviour
{
    [SerializeField] private SceneLoadManager sceneLoadManager;
    [SerializeField] private CameraEnviroment cameraEnviroment;

    public bool IsPathFindingEnable { get; private set; } = false;

    public void ChangeScene(string unloadSceneName, string loadSceneName)
    {
        bool isDungeon = loadSceneName.Contains("Game");
        GameManager.Instance.ChangeState(isDungeon ? GameState.Dungeon : GameState.Town);
        if (isDungeon) cameraEnviroment.ChangeToDungeon();
        else cameraEnviroment.ChangeToTown();
        IsPathFindingEnable = isDungeon;
        StartCoroutine(sceneLoadManager.ChangeSceneCor(unloadSceneName, loadSceneName));
    }


}
