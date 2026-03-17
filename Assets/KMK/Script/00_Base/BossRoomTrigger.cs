using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.SoundManager.PlayBGM(EBGMType.BOSS_BATTLE);

            GameManager.Instance.EnemyUIManager.SetBossHP(true);
        }
    }
}
