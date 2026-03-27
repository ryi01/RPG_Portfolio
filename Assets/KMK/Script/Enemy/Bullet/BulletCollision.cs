using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletCollision : MonoBehaviour
{
    // 피격 대상 태그
    [SerializeField] protected LayerMask hitLayer;
    protected MeshRenderer mesh;
    [SerializeField] protected string sfxString;

    // 피격 제한 대상 태그 (복수 대상일 경우 레이어로 제어할 것)
    [SerializeField] protected LayerMask noHitLayer;

    [SerializeField] protected float destroyTime = 0.5f;

    [SerializeField] protected GameObject bulletParticle;

    [SerializeField] protected bool isArrow = true;

    protected HashSet<CharacterStatComponent> hitTargets = new HashSet<CharacterStatComponent>();

    public BaseController Owner { get; set; }

    protected virtual void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>(true);
        if(mesh != null) mesh.enabled = true;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        HandleEnter(collision.collider, collision.contacts[0].point);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = other.bounds.ClosestPoint(transform.position);
        HandleEnter(other, hitPoint);
    }
    protected virtual void HandleEnter(Collider other, Vector3 hitPoint)
    {
        if (other.gameObject == Owner)
        {
            return;
        }
        if ((hitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnHitTarget(other, hitPoint);
        }
        else if ((noHitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnHitObstacle(other, hitPoint);
            return;
        }
    }
    protected virtual void OnHitTarget(Collider other, Vector3 hitPoint)
    {
        PlayEffect(hitPoint);
        if(mesh != null) mesh.enabled = false;
        DamageTarget(other);
        FinishBullet();
    }
    protected virtual void OnHitObstacle(Collider other, Vector3 hitPoint)
    {
        PlayEffect(hitPoint);
        FinishBullet();
    }

    protected virtual void DamageTarget(Collider other)
    {
        if (!other.TryGetComponent(out BaseController target)) return;
        if (Owner == null) return;
        if (!Owner.TryGetComponent(out BaseController ownerController)) return;

        target.Damage(ownerController.GetStat.Attack, 0, Owner.transform);
    }

    protected virtual void FinishBullet()
    {
        if (mesh != null) mesh.enabled = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        foreach(var ps in particles)
        {
            var emission = ps.emission;
            Destroy(ps.gameObject);
        }

        TargetMovement movement = GetComponentInParent<TargetMovement>();
        if (movement != null) movement.enabled = false;

        Destroy(gameObject, destroyTime);
    }

    protected virtual void PlayEffect(Vector3 pos)
    {
        if(bulletParticle != null)
            Instantiate(bulletParticle, pos, Quaternion.identity);
    }
}
