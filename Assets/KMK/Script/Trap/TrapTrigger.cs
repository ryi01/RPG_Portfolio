using System.Collections;
using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private ArrowLauncher[] launcher;
    private bool isTrigger = false;


    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger) return;
        if(!other.CompareTag("Player"))
        {
            return;
        }
        LaunchAll();

        StartCoroutine(ReloadTimeRoutine());
    }
    private void LaunchAll()
    {
        for (int i = 0; i < launcher.Length; i++)
        {
            launcher[i].LaunchTrap();
        }
    }
    IEnumerator ReloadTimeRoutine()
    {
        isTrigger = true;
        float waitTime = Random.Range(1, 3);
        yield return new WaitForSeconds(waitTime);

        isTrigger = false;
    }
}
