using UnityEngine;

public class BombMovement : MonoBehaviour
{
    [SerializeField] private float throwPower = 15f;
    [SerializeField] private float upwardForce = 4f;

    public Transform TargetPos { get; set; }

    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        Vector3 dir = (TargetPos.position - transform.position).normalized;
        Vector3 force = dir.normalized * throwPower + Vector3.up * upwardForce;

        rb.AddForce(force, ForceMode.VelocityChange);
    }

}
