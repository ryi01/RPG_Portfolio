using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    private bool isTrigger = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger) return;

        if(other.CompareTag("Player"))
        {
            isTrigger = true;
            GameManager.Instance.SoundManager.PlayBGM(EBGMType.BOSS_BATTLE);

            GameManager.Instance.EnemyUIManager.SetBossHP(true);

            GameManager.Instance.CameraEnviroment.ChangeToBossRoom();
        }
    }
}
