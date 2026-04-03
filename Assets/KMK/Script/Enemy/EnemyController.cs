using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public struct HitData
{
    public float Force;
    public Transform Attacker;
    public HitData(float force, Transform attacker)
    {
        Force = force;
        Attacker = attacker;
    }
}
[RequireComponent(typeof(EnemyStatComponent))]
public class EnemyController : BaseController<EnemyStatComponent>
{
    [SerializeField] protected EnemyState[] enemyStates;

    protected Dictionary<EnumTypes.STATE, EnemyState> stateDict = new();
    protected EnemyState currentState;
    protected NavMeshAgent navMeshAgent;
    protected GameObject player;
    public NavMeshAgent NavMeshAgent => navMeshAgent;
    public EnemyState CurrentState => currentState;
    public GameObject Player { get => player; set => player = value; }

    public BossPhaseComponent BossPhase { get; private set; }
    public BossSkillComponent BossSkill { get; private set; }
    public BossQuestComponent BossQuest { get; private set; }

    public BossSummonComponent BossSummon { get; private set; }
    public BossLightningComponent BossLightning { get; private set; }


    protected override void Awake()
    {
        base.Awake();

        navMeshAgent = GetComponent<NavMeshAgent>();

        TryGetComponent(out BossPhaseComponent bossPhase);
        BossPhase = bossPhase;
        TryGetComponent(out BossSkillComponent bossSkill);
        BossSkill = bossSkill;

        TryGetComponent(out BossQuestComponent bossQuest);
        BossQuest = bossQuest;

        TryGetComponent(out BossLightningComponent bossLightning);
        BossLightning = bossLightning;

        TryGetComponent(out BossSummonComponent bossSummon);
        BossSummon = bossSummon;

        foreach (var state in enemyStates)
        {
            if (state != null)
            {
                if (state == null) continue;
                state.Intialize(this);
                stateDict[state.StateType] = state;
            }
        }
    }
    private void Start()
    {
        if(navMeshAgent != null) navMeshAgent.avoidancePriority = UnityEngine.Random.Range(0, 99);

        player = GameObject.FindGameObjectWithTag("Player");
        TransitionToState(EnumTypes.STATE.IDLE);
    }
    protected virtual void Update()
    {
        currentState?.UpdateState();
    }
    public override void Damage(float damage, float force, Transform attacker)
    {
        base.Damage(damage, force, attacker);
        if (currentState == null || currentState.StateType == EnumTypes.STATE.DEATH) return;
        if (currentState.StateType == EnumTypes.STATE.PATTERN_PHASE) return;
        if (StatComp.CurrentHP <= 0)
        {
            GameManager.Instance.SendEnemyKilled(StatComp.Exp);
            TransitionToState(EnumTypes.STATE.DEATH, force);
            return;
        }
        if(currentState != null && currentState.StateType == EnumTypes.STATE.STUN) return;

        if(StatComp.AddGroogy(damage))
        {
            TransitionToState(EnumTypes.STATE.STUN);
        }
        else TransitionToState(EnumTypes.STATE.DAMAGE, new HitData(force, attacker));
    }
    public void ForceStun(float duration)
    {
        StatComp.AddGroogy(StatComp.MaxGroogy);
        TransitionToState(EnumTypes.STATE.STUN);
    }
 
    public virtual void TransitionToState(EnumTypes.STATE state, object data = null)
    {
        if (currentState != null && currentState.StateType == EnumTypes.STATE.DEATH) return;
        if (!stateDict.TryGetValue(state, out EnemyState nextState)) return;
        if (currentState == nextState && data == null) return;
        if (state == EnumTypes.STATE.STUN && currentState == nextState) return;

        currentState?.ExitState();
        currentState = null;
        currentState = nextState;
        currentState?.EnterState(state, data);
    }
    public float GetPlayerDis()
    {
        if (player == null) return float.MaxValue;
        return Vector3.Distance(transform.position, Player.transform.position);
    }

    public virtual void NavigationStop()
    {
        if (navMeshAgent == null) return;
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh) return;
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.nextPosition = transform.position;
        if(Animator != null)
        {
            if(BossLightning != null) Animator.SetBool("Run", false);
            Animator.SetInteger("State", 0);
        }
    }

    public virtual void NavigationResume(float speedMultiplier = 1f)
    {
        if (navMeshAgent == null) return;
        if (!navMeshAgent.enabled || !navMeshAgent.isOnNavMesh) return;
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = StatComp.MoveSpeed * speedMultiplier;
        if (Animator != null)
        {
            if (BossLightning != null) Animator.SetBool("Run", false);
            Animator.SetInteger("State", 2);
        }
    }
    private void OffCollider()
    {
        Collider[] cols = GetComponentsInChildren<Collider>();

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].enabled = false;
        }
    }
    public virtual void OnDeathEntered(object data = null)
    {
        BossQuest?.HandleBossDeath();
        BossSummon?.ClearAll();
        BossLightning?.StopPattern();
        OffCollider();
    }
}
