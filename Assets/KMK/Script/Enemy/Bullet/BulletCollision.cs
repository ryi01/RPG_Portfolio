using System.Collections.Generic;
using System.Linq;
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
        if (collision.contactCount == 0) return;
        HandleEnter(collision.collider, collision.contacts[0].point);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = other.bounds.ClosestPoint(transform.position);
        HandleEnter(other, hitPoint);
    }
    protected virtual void HandleEnter(Collider other, Vector3 hitPoint)
    {
        if (Owner != null && other.gameObject == Owner.gameObject)
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
        float final = Owner != null ? Owner.GetStat.Attack : 1;
        Transform finalPos = Owner != null ? Owner.transform : null;
        target.Damage(final, 0, finalPos);
    }

    protected virtual void FinishBullet()
    {
        if (mesh != null) mesh.enabled = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        
        foreach(var ps in particles)
        {
            ps.transform.SetParent(null);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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
