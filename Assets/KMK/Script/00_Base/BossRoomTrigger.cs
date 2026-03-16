using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.Instance.SoundManager.PlayBGM(EBGMType.BOSS_BATTLE);
            Debug.Log("보스방 진입!");
            GameManager.Instance.EnemyUIManager.SetBossHP(true);
        }
    }
}
