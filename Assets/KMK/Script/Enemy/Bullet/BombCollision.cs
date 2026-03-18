using System.Collections;
using UnityEngine;

public class BombCollision : BulletCollision
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] protected Material flashMat;
    [SerializeField] protected int flashCount = 4;
    private Material originMat;

    private bool isExploding = false;
    protected override void Awake()
    {
        base.Awake();
        originMat = mesh.material;
    }

    protected override void OnHitTarget(Collider other, Vector3 hitPoint)
    {

    }
    protected override void OnHitObstacle(Collider other, Vector3 hitPoint)
    {
        if (rb.isKinematic || isExploding) return;
        isExploding = true;
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
            if (mesh != null) mesh.material = flashMat;
            yield return new WaitForSeconds(interval);
            if (mesh != null) mesh.material = originMat;
            yield return new WaitForSeconds(interval);
        }

        if(bulletParticle != null) Instantiate(bulletParticle, transform.position, bulletParticle.transform.rotation);
        ExplodeDamage();
        GameManager.Instance.SoundManager.PlaySFX(sfxString);
        Destroy(gameObject);
    }
    private void ExplodeDamage()
    {
        hitTargets.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, hitLayer);
        foreach(Collider hit in hits)
        {
            base.DamageTarget(hit);
        }
    }

}
