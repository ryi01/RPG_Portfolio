using UnityEngine;
// 최상위 부모 : 모든 컨트롤러의 공통 스탯 관리용
// 인터페이스 역할
public abstract class BaseController : MonoBehaviour
{
    public Animator Animator { get; protected set; }
    // 공통 Stat을 CharacterStatComponent 타입으로 접근 가능하게 함
    public abstract CharacterStatComponent GetStat();
    public abstract void Damage(float damage, float force);
}
// 제너릭을 통해 자식들마다 가진 StatComp를 변환
// 자식 쪽에서 타입을 변환하기 위함
public abstract class BaseController<T> : BaseController where T : CharacterStatComponent
{
    public T StatComp { get; protected set; }

    public override CharacterStatComponent GetStat()
    {
        return StatComp;
    }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        StatComp = GetComponent<T>();
    }
    public override void Damage(float damage, float force)
    {
        StatComp.TakeDamage(damage);
    }
}
