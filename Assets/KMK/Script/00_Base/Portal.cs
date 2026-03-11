using UnityEngine;

public class Portal : InteractionObject
{
    [SerializeField] private string targetSceneName = "GameScene";
    public Vector3 SpawnPlayerPos { get; set; }
    private bool isChangeScene = false;
    public void ChangeTargetSceneName(string name)
    {
        targetSceneName = name;
    }

    public override void Interact(PlayerController player)
    {
        if (isChangeScene) return;

        string currentSceneName = gameObject.scene.name;

        GameManager.Instance.ChangeScene(currentSceneName, targetSceneName);

        isChangeScene = true;
    }
}
