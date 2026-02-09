using UnityEngine;

public class BombCollision : BulletCollision
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float explosionRadius = 3;

    protected override void OnHitTarget(Collider other)
    {
        
    }
    protected override void OnHitObstacle(Collider other)
    {
        if (rb.isKinematic) return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 물리 연산 중단
        rb.isKinematic = true;
        CheckRadius(other);
        Destroy(gameObject, destroyTime);
    }
    protected override void CheckRadius(Collider other)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitLayer);
        foreach(Collider hit in hits)
        {
            base.CheckRadius(hit);
        }
    }
}
