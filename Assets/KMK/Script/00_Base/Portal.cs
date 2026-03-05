using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "GameScene";
    public Vector3 SpawnPlayerPos { get; set; }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            string currentSceneName = gameObject.scene.name;
            GameManager.Instance.ChangeScene(currentSceneName, targetSceneName);
        }
    }
}
