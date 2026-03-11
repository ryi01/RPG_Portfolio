using UnityEngine;

public class EnemyStatUI : StatUI
{
    [SerializeField] protected Vector3 offset;
    private Transform targetTras;
    private Camera cam;
    public void SetUpUi(Transform target, float yOffset)
    {
        targetTras = target;
        offset = new Vector3(0, yOffset, 0);
        cam = Camera.main;
    }
    protected void LateUpdate()
    {
        if(targetTras ==  null)
        {
            Destroy(gameObject);
            return;
        }
        UpdateUIPos();
    }
    public void UpdateUIPos()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(targetTras.position + offset);
        transform.position = screenPos;
    }
}
