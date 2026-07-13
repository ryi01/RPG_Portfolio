using UnityEngine;

public class Portal : InteractionObject
{
    [SerializeField] private string targetSceneName = "GameScene";
    private SceneCoordinator sceneCoordinator;
    public Vector3 SpawnPlayerPos { get; set; }
    private bool isChangeScene = false;
    public void InitSceneCorrdinator(SceneCoordinator sceneCoordinator)
    {
        this.sceneCoordinator = sceneCoordinator;
    }
    public void ChangeTargetSceneName(string name)
    {
        targetSceneName = name;
    }

    public override void Interact(PlayerController player)
    {
        if (isChangeScene) return;

        string currentSceneName = gameObject.scene.name;

        sceneCoordinator.ChangeScene(targetSceneName);

        isChangeScene = true;

        Destroy(gameObject, 2f);
    }
}
