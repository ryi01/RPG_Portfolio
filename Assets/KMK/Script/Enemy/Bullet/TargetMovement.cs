using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    protected GameObject player;
    protected Vector3 targetDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        if (player == null || gameObject == null) return;
        targetDir = ((player.transform.position + Vector3.up * 1.5f)- transform.position).normalized;
        transform.Translate(targetDir * Time.deltaTime * moveSpeed, Space.World);
        transform.forward = targetDir;
    }

}
