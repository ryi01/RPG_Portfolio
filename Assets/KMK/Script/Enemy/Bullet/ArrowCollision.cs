using UnityEngine;

public class ArrowCollision : BulletCollision
{
    [SerializeField] private GameObject brokenArrowPrefab;

    protected override void OnHitTarget(Collider other, Vector3 hitPoint)
    {
        SpawnBrokenArrow(other.transform, hitPoint);
        base.OnHitTarget(other, hitPoint);
    }

    protected override void OnHitObstacle(Collider other, Vector3 hitPoint)
    {
        base.OnHitObstacle(other, hitPoint);
    }


    private void SpawnBrokenArrow(Transform parent, Vector3 hitPoint)
    {
        if (brokenArrowPrefab == null) return;

        Vector3 finalPos = hitPoint + transform.forward * 0.15f;

        GameObject broken = Instantiate(brokenArrowPrefab, finalPos, transform.rotation);
        broken.transform.SetParent(parent);
        transform.localScale = Vector3.one;

        Destroy(broken, 10);
    }
}
