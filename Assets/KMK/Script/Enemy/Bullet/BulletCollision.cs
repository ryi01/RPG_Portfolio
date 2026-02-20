using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    // 피격 대상 태그
    [SerializeField] protected LayerMask hitLayer;
    protected MeshRenderer mesh;

    // 피격 제한 대상 태그 (복수 대상일 경우 레이어로 제어할 것)
    [SerializeField] protected LayerMask noHitLayer;

    [SerializeField] protected float attack = 2;
    [SerializeField] protected float destroyTime = 0.5f;

    [SerializeField] protected GameObject bulletParticle;

    protected HashSet<CharacterStatComponent> hitTargets = new HashSet<CharacterStatComponent>();

    public GameObject Owner { get; set; }

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = true;
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        HandleEnter(collision.collider);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        HandleEnter(other);
    }
    protected virtual void HandleEnter(Collider other)
    {
        if (other.gameObject == Owner)
        {
            return;
        }
        if ((hitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnHitTarget(other);
        }
        else if ((noHitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            OnHitObstacle(other);
            return;
        }
    }
    protected virtual void OnHitTarget(Collider other)
    {
        mesh.enabled = false;
        DamageTarget(other);
        Destroy(gameObject, destroyTime);
    }
    protected virtual void OnHitObstacle(Collider other)
    {
        Destroy(gameObject, destroyTime);
    }

    protected virtual void DamageTarget(Collider other)
    {
        if (other.TryGetComponent(out CharacterStatComponent statComp))
        {
            if (statComp.CurrentHP <= 0) return;
            if (hitTargets.Add(statComp))
            {
                statComp.TakeDamage(attack);
            }
        }
    }
}
