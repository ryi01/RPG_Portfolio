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
        Vector3 targetPos = player.transform.position + Vector3.up * 1.5f;
        Vector3 targetDir = (targetPos - transform.position).normalized;
        if (Vector3.Distance(targetPos, transform.position) < 0.1f) return;
        if (targetDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 600f);
        }
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.Self);
    }

}
