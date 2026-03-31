using UnityEngine;
// 베이스 컨트롤러 생성 이유 
// TakeDamage()함수를 호출할때 EnemyContorller/PlayerController가 아닌 BaseController만 호출하고 싶기에 생성
// Base StatComponent - 플레이어와 적이 공통되는 Status를 가짐
// ㄴ PlayerStatComp / EnemyStatComp = 플레이어와 적이 각각의 Status를 가짐

// 최상위 부모 : 모든 컨트롤러의 공통 스탯 관리용
// 인터페이스 역할
[RequireComponent(typeof(CharacterStatComponent))]
public abstract class BaseController : MonoBehaviour
{
    public Animator Animator { get; protected set; }
    // 공통 Stat을 CharacterStatComponent 타입으로 접근 가능하게 함
    public abstract CharacterStatComponent GetStat { get; }
    public abstract void Damage(float damage, float force, Transform attacker = null );
}
// 제너릭을 통해 자식들마다 가진 StatComp를 변환
// 자식 쪽에서 타입을 변환하기 위함
public abstract class BaseController<T> : BaseController where T : CharacterStatComponent
{
    public T StatComp { get; protected set; }

    public override CharacterStatComponent GetStat => StatComp;

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        StatComp = GetComponent<T>();
    }
    public override void Damage(float damage, float force, Transform attacker = null)
    {
        StatComp.TakeDamage(damage);
    }
}
