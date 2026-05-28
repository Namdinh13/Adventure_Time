using BossFSM;
using EnemyFSM;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class BossEnemy : MonoBehaviour
{
    [Header("References")]
    [field: SerializeField] public PlayerController Player { get; private set; }

    [Header("AI")]
    [SerializeField] private float DecisionDelay = 1.5f;
    private float nextDecisionTime;

    [Header("Combo")]
    [SerializeField] private int CurrentCombo;
    [SerializeField] private int MaxCombo = 3;
    [HideInInspector]
    public bool ContinueCombo;


    [Header("Attack Config")]
    [SerializeField][Range(0.1f, 5f)] private float AttackCooldown = 3.0f;
    [SerializeField] private float AttackRange = 2f;

    [Header("Sensors")]
    [SerializeField] private PlayerSensor FollowPlayerSensor;
    [SerializeField] private PlayerSensor AttackPlayerSensor;

    [Space]
    [Header("Debug Info")]
    [SerializeField] private bool IsInAttackRange;
    [SerializeField] private bool IsInChasingRange;
    [SerializeField] private float LastAttackTime;

    [Header("Hitbox")]
    [SerializeField] private Collider WeaponCollider;
    [SerializeField] private EnemyWeaponHitbox EnemyWeaponHitbox;

    private bool gotHit;
    private StateMachine<BossState, StateEvent> BossFSM;

    public Animator Animator;
    public NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        BossFSM = new StateMachine<BossState, StateEvent>();

        // Add States
        BossFSM.AddState(BossState.Idle, new BIdleState(false, this));
        BossFSM.AddState(BossState.CombatIdle, new BCombatIdleState(true, this, Player.transform));
        BossFSM.AddState(BossState.Chase, new BChaseState(true, this, Player.transform));
        BossFSM.AddState(BossState.CombatMove, new BCombatMoveState(true, this, Player.transform));
        BossFSM.AddState(BossState.PreAttack, new BPreAttackState(true, this, Player.transform));
        BossFSM.AddState(BossState.Attack, new BAttackState(true, this, OnAttack));
        BossFSM.AddState(BossState.Recover, new BRecoverState(true, this, Player.transform));
        BossFSM.AddState(BossState.Hit, new BHitState(true, this));

        // Add Transitions
        BossFSM.AddTriggerTransition(
            StateEvent.DetectPlayer,
            new Transition<BossState>(BossState.Idle, BossState.Chase)
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Chase,
                BossState.CombatMove,
                transition => Vector3.Distance(Player.transform.position, transform.position) <= 6f
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatMove,
                BossState.CombatIdle,
                transition => IsInAttackRange
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatIdle,
                BossState.PreAttack,
                transition => CanMakeDecision() && ShouldAttack(transition)
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.PreAttack,
                BossState.Attack,
                transition => true
            )
        );



        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatMove,
                BossState.Chase,
                transition => Vector3.Distance(Player.transform.position, transform.position) > 7f
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Attack,
                BossState.Attack,
                transition =>
                    ContinueCombo &&
                    CurrentCombo < MaxCombo
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Attack,
                BossState.Recover,
                transition =>
                    !ContinueCombo ||
                    CurrentCombo >= MaxCombo
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Recover,
                BossState.CombatMove,
                transition => true
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatMove,
                BossState.CombatIdle,
                transition => IsInAttackRange
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatIdle,
                BossState.CombatMove,
                transition => !IsInAttackRange
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Chase,
                BossState.Idle,
                transition => !IsInChasingRange
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Idle,
                BossState.Hit,
                transition => gotHit)
            {
                forceInstantly = true
            }
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Chase,
                BossState.Hit,
                transition => gotHit)
            {
                forceInstantly = true
            }
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatMove,
                BossState.Hit,
                transition => gotHit)
            {
                forceInstantly = true
            }
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Attack,
                BossState.Hit,
                transition => gotHit)
            {
                forceInstantly = true
            }
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Hit,
                BossState.CombatMove,
                transition => !gotHit
            )
        );

        BossFSM.Init();
    }

    private void Start()
    {
        FollowPlayerSensor.OnPlayerEnter += FollowPlayerSensor_OnPlayerEnter;
        FollowPlayerSensor.OnPlayerExit += FollowPlayerSensor_OnPlayerExit;
        AttackPlayerSensor.OnPlayerEnter += AttackPlayerSensor_OnPlayerEnter;
        AttackPlayerSensor.OnPlayerExit += AttackPlayerSensor_OnPlayerExit;
    }

    private void FollowPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition)
    {
        BossFSM.Trigger(StateEvent.LostPlayer);
        IsInChasingRange = false;
    }

    private void FollowPlayerSensor_OnPlayerEnter(Transform Player)
    {
        BossFSM.Trigger(StateEvent.DetectPlayer);
        IsInChasingRange = true;
    }

    private bool ShouldAttack(Transition<BossState> Transition)
        => LastAttackTime + AttackCooldown <= Time.time && IsInAttackRange;

    private bool IsWithinIdleRange(Transition<BossState> Transition)
    {
        float distance = Vector3.Distance(Player.transform.position, transform.position);
        return distance <= AttackRange;
    }

    private bool IsNotWithinIdleRange(Transition<BossState> Transition)
    {
        return !IsWithinIdleRange(Transition);
    }

    private void AttackPlayerSensor_OnPlayerExit(Vector3 LastKnownPosition)
    {
        IsInAttackRange = false;
    }

    private void AttackPlayerSensor_OnPlayerEnter(Transform Player)
    {
        IsInAttackRange = true;
    }

    public void EnableWeapon()
    {
        EnemyWeaponHitbox.ResetHitTargets();
        WeaponCollider.enabled = true;
    }

    public void DisableWeapon()
    {
        WeaponCollider.enabled = false;
    }

    private void OnAttack(State<BossState, StateEvent> State)
    {
        transform.LookAt(Player.transform.position);
        LastAttackTime = Time.time;
    }

    public void OnHit()
    {
        gotHit = true;
    }

    public void EndHit()
    {
        gotHit = false;
    }

    public int GetNextComboAttack()
    {
        CurrentCombo++;

        if (CurrentCombo > MaxCombo)
        {
            CurrentCombo = 1;
        }

        return CurrentCombo;
    }

    public void ResetCombo()
    {
        CurrentCombo = 0;
    }
    public bool CanMakeDecision()
    {
        return Time.time >= nextDecisionTime;
    }

    public void ResetDecisionTimer()
    {
        nextDecisionTime = Time.time + DecisionDelay;
    }

    private void Update()
    {
        BossFSM.OnLogic();
    }
}
