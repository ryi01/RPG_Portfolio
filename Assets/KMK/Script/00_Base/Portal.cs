using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "GameScene";
    [SerializeField] private Vector3 spawnPos = new Vector3(0, 0, 0);
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            string currentSceneName = gameObject.scene.name;
            FindFirstObjectByType<SceneLoadManager>().ChangeScene(currentSceneName, targetSceneName, spawnPos);
        }
    }
}
