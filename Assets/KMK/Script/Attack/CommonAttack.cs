using UnityEngine;

// 제너릭을 사용한 이유
// CommonAttack에서 공통적으로 Controller에 다가가고 싶기 대문
public abstract class CommonAttack : MonoBehaviour
{
    [SerializeField] protected BaseController bc;

    [SerializeField] protected Transform attackTransform;
    protected virtual void Awake()
    {
        bc = GetComponent<BaseController>();
    }
    public abstract void Attack();
}
