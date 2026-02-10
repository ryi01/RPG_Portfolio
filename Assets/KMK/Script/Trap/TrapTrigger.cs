using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private ArrowLauncher launcher;
    private bool isTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger) return;
        if(!other.CompareTag("Player"))
        {
            return;
        }
        launcher.LaunchTrap();
        isTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTrigger = false;
    }
}
