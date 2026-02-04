using UnityEngine;

public abstract class CommonAttack : MonoBehaviour
{
    [SerializeField] protected BaseController bc;

    [SerializeField] protected Transform attackTransform;

    public abstract void Attack();
}
