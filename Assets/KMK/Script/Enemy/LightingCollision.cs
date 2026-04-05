using UnityEngine;

public class LightingCollision : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 1f)] protected float clipVolume = 1;
    [SerializeField] protected AudioClip clip;
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterStatComponent statComp))
        {
            if (statComp.CurrentHP <= 0) return;
            statComp.TakeDamage(10);
            GameManager.Instance.SoundManager.PlayImpactSFX(clip, clipVolume);
            Destroy(gameObject, 0.5f);
        }
    }
}
