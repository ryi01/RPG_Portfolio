using UnityEngine;

public class RotationSelf : MonoBehaviour
{
    [SerializeField] private float rotSpeed = 600;
    private void Update()
    {
        transform.Rotate(Vector3.right * rotSpeed * Time.deltaTime, Space.Self);
    }
}
