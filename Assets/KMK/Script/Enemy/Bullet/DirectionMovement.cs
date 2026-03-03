using UnityEngine;

public class DirectionMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.Self);
    }
}
