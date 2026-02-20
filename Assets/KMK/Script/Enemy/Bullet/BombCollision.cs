using System.Collections;
using UnityEngine;

public class BombCollision : BulletCollision
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] protected Material flashMat;
    [SerializeField] protected int flashCount = 4;
    private Material originMat;
    private MeshRenderer meshRenderer;
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        originMat = meshRenderer.material;
    }

    protected override void OnHitTarget(Collider other)
    {
        
    }
    protected override void HandleEnter(Collider other)
    {
        if (other.gameObject == Owner) return;
        if ((hitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            return;
        }
        else if ((noHitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnHitObstacle(other);
        }
    }
    protected override void OnHitObstacle(Collider other)
    {
        if (rb.isKinematic) return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 물리 연산 중단
        rb.isKinematic = true;

        StartCoroutine(ExplosionRoutine(other)); 
    }
    private IEnumerator ExplosionRoutine(Collider other)
    {
        float interval = destroyTime / (flashCount * 2);
        for(int i = 0; i < flashCount; i++)
        {
            if (meshRenderer != null) meshRenderer.material = flashMat;
            yield return new WaitForSeconds(interval);
            if (meshRenderer != null) meshRenderer.material = originMat;
            yield return new WaitForSeconds(interval);

        }

        if(bulletParticle != null) Instantiate(bulletParticle, transform.position, bulletParticle.transform.rotation);
        DamageTarget(other);
        Destroy(gameObject);
    }
    protected override void DamageTarget(Collider other)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitLayer);
        foreach(Collider hit in hits)
        {
            base.DamageTarget(hit);
        }
    }

}
