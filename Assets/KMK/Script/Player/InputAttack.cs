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
    private Vector3 mouseWorldPos;

    private int comboStep = 0;


    private void Awake()
    {
        pc = GetComponent<PlayerController>();
    }

    // pc에서 공격 입력이 들어온 경우
    public void RequestAttack(Vector3 mousePos, Vector3 lookDir)
    {
        pc.SetAttackLookDir(lookDir);
        if(IsAttackAnimation())
        {
            QueueNextAttack(mousePos);
            return;
        }
        StartAttack(mousePos);
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
    // 첫 공격 시작
    private void StartAttack(Vector3 mousePos)
    {
        mouseWorldPos = mousePos;

        comboStep = 0;
        isBuffered = false;
        bufferTimer = 0;

        ExecuteAttak();
    }
    // 다음 공격 예약
    public void QueueNextAttack(Vector3 mousePos)
    {
        mouseWorldPos = mousePos;
        isBuffered = true;
        bufferTimer = comboBufferDuration;
    }
    // 실제 공격
    private void ExecuteAttak()
    {
        ApplyAutoTargeting();
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
    private void ApplyAutoTargeting()
    {
        Collider[] closeEnemies = Physics.OverlapSphere(mouseWorldPos, autoTargetRadius, enemyLayer);
        Transform bestTarget = null;
        float closeDist = float.MaxValue;

        foreach (var enemy in closeEnemies)
        {
            float dis = Vector3.Distance(new Vector3(mouseWorldPos.x, 0, mouseWorldPos.z), new Vector3(enemy.transform.position.x, 0, enemy.transform.position.z));
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
            targetLookDir = (mouseWorldPos - transform.position).normalized;
        }
        targetLookDir.y = 0;
        if (targetLookDir != Vector3.zero)
        {
            pc.MovementComp.LookAtInstant(targetLookDir);
        }
    }
    public bool IsBufferActive()
    {
        return isBuffered;
    }
}
