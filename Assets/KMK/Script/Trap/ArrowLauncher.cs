using System.Collections;
using UnityEngine;

public class ArrowLauncher : MonoBehaviour
{
    [Header("Arrow Àü¿ë")]
    [SerializeField] private Transform[] launchTrans;
    [SerializeField] protected float launchTime = 0.3f;
    [SerializeField] private GameObject trapPrefab;
    
    public virtual void LaunchTrap()
    {
        StartCoroutine(LaunchCor());
    }
    
    IEnumerator LaunchCor()
    {
        yield return new WaitForSeconds(launchTime);
        for(int i = 0; i < launchTrans.Length;i++)
        {
            Instantiate(trapPrefab, launchTrans[i].position, launchTrans[i].rotation);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
