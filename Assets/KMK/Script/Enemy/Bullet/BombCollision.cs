using System.Collections;
using UnityEngine;

public class BombCollision : BulletCollision
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float explosionRadius = 3;

    [SerializeField] protected Material flashMat;
    [SerializeField] protected int flashCount = 4;
    [SerializeField] private float explodeDelay = 1.2f;

    private Material originMat;

    private bool isArmed = false;
    private bool isExploding = false;
    protected override void Awake()
    {
        base.Awake();
        originMat = mesh.material;
    }

    protected override void OnTriggerEnter(Collider other)
    {
       
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (isExploding) return;
        if (collision.contactCount == 0) return;

        Collider other = collision.collider;

        Vector3 hitPoint = collision.contacts[0].point;

        if (Owner != null && other.gameObject == Owner.gameObject) return;

        if (collision.relativeVelocity.magnitude < 1.5f) return;
        TryArmBomb();
    }
    private void TryArmBomb()
    {
        if (isArmed) return;
        isArmed = true;
        StartCoroutine(ArmAndExplodeRoutine());
    }

    private IEnumerator ArmAndExplodeRoutine()
    {
        yield return StartCoroutine(FlashRoutine());
        Explode();
    }

    private IEnumerator FlashRoutine()
    {
        if(mesh == null || flashMat == null || originMat == null)
        {
            yield return new WaitForSeconds(explodeDelay);
            yield break;
        }

        float elapsed = 0f;
        float interval = 0.2f;
        while(elapsed < explodeDelay)
        {
            mesh.material = flashMat;
            yield return new WaitForSeconds(interval);
            elapsed += interval;

            mesh.material = originMat;
            yield return new WaitForSeconds(interval);
            elapsed += interval;

            interval = Mathf.Max(0.05f, interval * 0.8f);
        }
        mesh.material = originMat;
    }

    private void Explode()
    {
        if (isExploding) return;
        isExploding = true;
        if(rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
        if (bulletParticle != null) Instantiate(bulletParticle, transform.position, bulletParticle.transform.rotation);

        ExplodeDamage();
        Destroy(gameObject);
    }
    private void ExplodeDamage()
    {
        hitTargets.Clear(); 
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitLayer);
        foreach(Collider hit in hits)
        {
            DamageTarget(hit);
        }
    }

}
