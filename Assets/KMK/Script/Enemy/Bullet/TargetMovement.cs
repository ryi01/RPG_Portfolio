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
        Vector3 targetPos = ((player.transform.position + Vector3.up * 1.5f)- transform.position);
        Vector3 diff = targetPos - transform.position;
        if (diff.sqrMagnitude < 0.01f) return;
        targetDir = diff.normalized;
        transform.forward = targetDir;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 600f);
        transform.Translate(transform.forward * Time.deltaTime * moveSpeed, Space.World);
    }

}
