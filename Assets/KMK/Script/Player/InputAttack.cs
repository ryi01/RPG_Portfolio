using UnityEngine;

public class InputAttack : MonoBehaviour
{
    private PlayerController pc;
    // 공격 애니메이션 해시값
    private int hashAttack = Animator.StringToHash("Attack");
    private bool isBufferActive = false;
    private float bufferTimer = 0;
    private const float BUFFER_DURATION = 0.5f;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    public void UpdateAttackProgress()
    {
        // 버퍼가 들어왔을떄
        if (isBufferActive)
        {
            // 타이머활성화
            bufferTimer -= Time.deltaTime;
            AnimatorStateInfo stateInfo = pc.Animator.GetCurrentAnimatorStateInfo(0);
            // 애니메이션 시간을 0~1 사이의 비율
            float normalizedTime = stateInfo.normalizedTime % 1f;
            // 공격중이라면, 일정 시간이 지났을때 트리거 투입
            if (stateInfo.IsTag("Attack"))
            {
                if (normalizedTime > 0.4f)
                {
                    ExecuteAttak();
                }
            }
            else
            {
                // 공격중이 아니라면 바로 실행
                ExecuteAttak();
            }
            if (bufferTimer <= 0) isBufferActive = false;
        }
        
    }
    private void ExecuteAttak()
    {
        // 트리거 리셋후 다시 실행
        pc.Animator.ResetTrigger(hashAttack);
        pc.Animator.SetTrigger(hashAttack);
        isBufferActive = false;
    }
    // 입력이 들어오면 버퍼를 활성화
    public void TriggerAttack()
    {
        // 클릭 예약
        isBufferActive = true;
        bufferTimer = BUFFER_DURATION;
    }

    // animation을 태그로 확인하고 실행중이면 true
    public bool IsAttackAnimation()
    {
        return pc.Animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack");
    }
}
