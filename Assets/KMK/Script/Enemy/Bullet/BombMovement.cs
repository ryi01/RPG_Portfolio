using UnityEngine;

public class BombMovement : MonoBehaviour
{
    [SerializeField] private float throwPower = 15f;
    [SerializeField] private float bias = 0.5f;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        Vector3 targetDir = (player.transform.position - transform.position).normalized;
        targetDir.y += bias;

        Vector3 velocity = targetDir.normalized * throwPower;

        rb.AddForce(velocity, ForceMode.VelocityChange);
    }

}
