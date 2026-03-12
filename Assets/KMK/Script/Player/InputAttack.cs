using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class InputAttack : MonoBehaviour
{
    private PlayerController pc;
    // 공격 애니메이션 해시값
    private int hashAttack = Animator.StringToHash("Attack");
    private bool isBufferActive = false;
    private float bufferTimer = 0;
    private const float BUFFER_DURATION = 0.25f;
    [SerializeField] private float autoTargetRadius = 3.0f;
    [SerializeField] private LayerMask enemyLayer;
    private Vector3 mousePos;

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
                if (normalizedTime > 0.18f)
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
        ApplyAutoTargeting();
        // 트리거 리셋후 다시 실행
        pc.Animator.ResetTrigger(hashAttack);
        pc.Animator.SetTrigger(hashAttack);
        isBufferActive = false;
    }
    // 입력이 들어오면 버퍼를 활성화
    public void TriggerAttack(Vector3 mousePos)
    {
        // 클릭 예약
        isBufferActive = true;
        bufferTimer = BUFFER_DURATION;
        this.mousePos = mousePos;
    }

    // animation을 태그로 확인하고 실행중이면 true
    public bool IsAttackAnimation()
    {
        return pc.Animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");
    }

    private void ApplyAutoTargeting()
    {
        Collider[] closeEnemies = Physics.OverlapSphere(mousePos, autoTargetRadius, enemyLayer);
        Transform bestTarget = null;
        float closeDist = float.MaxValue;

        foreach (var enemy in closeEnemies)
        {
            float dis = Vector3.Distance(new Vector3(mousePos.x, 0, mousePos.z), new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
            if (dis < closeDist)
            {
                closeDist = dis;
                bestTarget = enemy.transform;
            }
        }
        Vector3 targetLookDir;
        if (bestTarget != null)
        {
            targetLookDir = (bestTarget.position - transform.position).normalized;
        }
        else
        {
            targetLookDir = (mousePos - transform.position).normalized;
        }
        targetLookDir.y = 0;
        if (targetLookDir != Vector3.zero)
        {
            pc.MovementComp.LookAtInstant(targetLookDir);
        }
    }
}
