using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    private EnemyUIManager enemyUIManager;
    private CameraEnviroment cameraEnviroment;
    private SoundManager soundManager;
    private bool isTrigger = false;

    public void InitRoomTrigger(EnemyUIManager enemyUI, CameraEnviroment cameraEnv)
    {
        enemyUIManager = enemyUI;
        cameraEnviroment = cameraEnv;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger) return;

        if(other.CompareTag("Player"))
        {
            isTrigger = true;
            GameManager.Instance.ChangeState(GameState.BossPhase);

            enemyUIManager.SetBossHP(true);

            cameraEnviroment.ChangeToBossRoom();
        }
    }
}
