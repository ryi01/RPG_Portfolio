using UnityEngine;

public class DirectionMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private void Update()
    {
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }
}
