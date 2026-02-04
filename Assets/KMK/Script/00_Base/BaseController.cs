using UnityEngine;

public abstract class BaseController : MonoBehaviour
{
    public CharacterStatComponent StatComp { get; protected set; }
    public Animator Animator { get; protected set; }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public abstract void Damage(float damage, float force);
}
