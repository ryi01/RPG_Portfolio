using UnityEngine;

public class EnemyStatUI : StatUI
{
    [SerializeField] protected Vector3 offset;
    private Transform targetTras;
    public void SetUpUi(Transform target, float yOffset)
    {
        targetTras = target;
        offset = new Vector3(0, yOffset, 0);
    }
    protected void LateUpdate()
    {
        if (targetTras == null) return;
        UpdateUIPos();
    }
    public void UpdateUIPos()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetTras.position + offset);
        transform.position = screenPos;
    }
}
