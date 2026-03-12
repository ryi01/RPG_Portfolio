using UnityEngine;

public class LightingCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterStatComponent statComp))
        {
            if (statComp.CurrentHP <= 0) return;
            statComp.TakeDamage(10);
            Destroy(gameObject, 0.5f);
        }
    }
}
