using System;
using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    [SerializeField] private SceneCoordinator sceneCoordinator;
    [SerializeField] private GameObject protalPrefab;
    [SerializeField] private GridAStar gridAStar;
    [SerializeField]
    [Range(0f, 1f)] protected float impactClipVolume = 0.7f;
    [SerializeField] protected AudioClip impactClip;
    public void SpawnPortal(string sceneName, Vector3 pos)
    {
        GameObject existingPortal = GameObject.FindGameObjectWithTag("Portal");
        if (existingPortal != null)
        {
            Destroy(existingPortal);
        }
        if (protalPrefab != null)
        {
            if(GameManager.Instance.CurrentState != GameState.Start) GameManager.Instance.SoundManager.PlayImpactSFX(impactClip, impactClipVolume);
            GameObject go = Instantiate(protalPrefab, pos, Quaternion.identity);
            
            go.tag = "Portal";
            if (go.TryGetComponent<Portal>(out Portal portal))
            {
                portal.InitSceneCorrdinator(sceneCoordinator);
                portal.ChangeTargetSceneName(sceneName);
            }
            if(gridAStar != null)
            {
                gridAStar.RegisterObjectNode(pos, true);
            }
        }
    }
}
