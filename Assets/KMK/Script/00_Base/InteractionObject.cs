using UnityEngine;

public abstract class InteractionObject : MonoBehaviour
{
    public virtual Transform GetTransform()
    {
        return transform;
    }

    public abstract void Interact(PlayerController player);
}
