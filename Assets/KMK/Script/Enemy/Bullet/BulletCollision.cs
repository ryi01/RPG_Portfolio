using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    // 피격 대상 태그
    [SerializeField] private LayerMask hitLayer;
    private MeshRenderer mesh;

    // 피격 제한 대상 태그 (복수 대상일 경우 레이어로 제어할 것)
    [SerializeField] private LayerMask noHitLayer;

    [SerializeField] private float attack = 2;

    public GameObject Owner { get; set; }

    // 생성 오프셋
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Owner)
        {
            return;
        }
        mesh.enabled = false;
        if ((hitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            if (other.TryGetComponent(out CharacterStatComponent statComp))
            {
                statComp.TakeDamage(attack);
            }
        }
        else if ((noHitLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            return;
        }
        Destroy(gameObject, 0.5f);
    }
}
