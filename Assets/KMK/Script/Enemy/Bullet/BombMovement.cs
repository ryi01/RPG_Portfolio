using UnityEngine;

public class BombMovement : MonoBehaviour
{
    [SerializeField] private float throwPower = 15f;
    [SerializeField] private float upwardForce = 4f;

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

        Vector3 force = targetDir * throwPower + Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.VelocityChange);
    }

}
