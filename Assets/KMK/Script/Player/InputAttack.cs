using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class InputAttack : MonoBehaviour
{
    private PlayerController pc;
    // 공격 애니메이션 해시값
    private int hashAttack = Animator.StringToHash("Attack");
    [SerializeField] private float comboBufferDuration = 0.25f;
    [SerializeField] private int maxComboCount = 4;
    [SerializeField] private float autoTargetRadius = 3.0f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isBuffered;
    private float bufferTimer;

    private int comboStep = 0;

    private Vector3 mouseWorldPos;
    private Vector3 lookDir;


    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    // pc에서 공격 입력이 들어온 경우
    public void RequestAttack(Vector3 mousePos, Vector3 lookDir)
    {
        this.mouseWorldPos = mousePos;
        lookDir.y = 0;
        this.lookDir = lookDir.sqrMagnitude > 0.001f ? lookDir.normalized : Vector3.zero;
        if (this.lookDir == Vector3.zero) return;
        // 바라보는 방향 저장
        pc.SetAttackLookDir(this.lookDir);
        // 이미 공격 중이라면 다음 공격 예약
        if(IsAttackAnimation())
        {
            QueueNextAttack(mousePos, this.lookDir);
            return;
        }
        // 공격중이 아니라면 바로 시작
        StartAttack(mousePos, this.lookDir);
    }
    // 첫 공격 시작
    private void StartAttack(Vector3 mousePos, Vector3 lookDir)
    {
        mouseWorldPos = mousePos;
        this.lookDir = lookDir;

        comboStep = 0;
        isBuffered = false;
        bufferTimer = 0;

        ExecuteAttak();
    }

    // 다음 공격 예약
    public void QueueNextAttack(Vector3 mousePos, Vector3 lookDir)
    {
        mouseWorldPos = mousePos;
        this.lookDir = lookDir;
        isBuffered = true;
        bufferTimer = comboBufferDuration;
    }
    // 매프레임 공격 버퍼 갱신
    // 공격중 + 버퍼 있는 경우 호출
    public void UpdateAttackProgress()
    {
        // 버퍼가 들어왔을떄
        if (!isBuffered || GameManager.Instance.CurrentState == GameState.Town) return;

        // 타이머활성화
        bufferTimer -= Time.deltaTime;
        AnimatorStateInfo stateInfo = pc.Animator.GetCurrentAnimatorStateInfo(1);

        // 애니메이션 시간을 0~1 사이의 비율
        float normalizedTime = stateInfo.normalizedTime % 1f;

        // 공격중이라면, 일정 시간이 지났을때 트리거 투입
        if (stateInfo.IsTag("Attack") && normalizedTime > 0.2f)
        {
            comboStep = Mathf.Min(comboStep + 1, maxComboCount - 1);
            ExecuteAttak();
            return;
        }
        if (bufferTimer <= 0) isBuffered = false;
    }
 

    // 실제 공격
    private void ExecuteAttak()
    {
        // 입력방향에 적이 있는 쪽으로 보정
        Vector3 finalLookDir = ApplyAutoTargeting(this.lookDir);
        if (finalLookDir == Vector3.zero) finalLookDir = lookDir;
        // 최종 회전
        pc.SetAttackLookDir(finalLookDir);
        pc.MovementComp.LookAtInstant(finalLookDir);

        // 트리거 리셋후 다시 실행
        pc.Animator.SetInteger("Combo", comboStep);
        pc.Animator.ResetTrigger(hashAttack);
        pc.Animator.SetTrigger(hashAttack);

        isBuffered = false;
    }

    // 콤보를 처음으로 초기화
    // 공격이 완전히 끝난경우
    public void ResetCombo()
    {
        pc.TrailOff();
        comboStep = 0;
        isBuffered = false;
        bufferTimer = 0;
    }
    // animation을 태그로 확인하고 실행중이면 true
    public bool IsAttackAnimation()
    {
        return pc.Animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack");
    }
    // 공격 방향 보정
    private Vector3 ApplyAutoTargeting(Vector3 inputLookDir)
    {
        Collider[] closeEnemies = Physics.OverlapSphere(transform.position, autoTargetRadius, enemyLayer);
        Transform bestTarget = null;
        float closeDist = float.MaxValue;

        foreach (var enemy in closeEnemies)
        {
            Vector3 dirToEnemy = enemy.transform.position - transform.position;
            dirToEnemy.y = 0;
            if (dirToEnemy.sqrMagnitude < 0.001f) continue;
            dirToEnemy.Normalize();

            // 입력 방향과 근처 적 방향확인
            float dot = Vector3.Dot(inputLookDir, dirToEnemy);
            // 옆이나 뒤면 보정 대상 제외
            if (dot < 0.4f) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            // 방향 유사도와 거리 가중치에 따라
            float score = dot * 2f - dist * 0.2f;
            // 에임 보정
            if(score > closeDist)
            {
                closeDist = score;
                bestTarget = enemy.transform;
            }
        }
        if (bestTarget == null) return inputLookDir;
        Vector3 result = bestTarget.position - pc.transform.position;
        result.y = 0;
        if (result.sqrMagnitude < 0.001f) return inputLookDir;
        return result.normalized;
    }
    public bool IsBufferActive()
    {
        return isBuffered;
    }
}
