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
    [SerializeField] private PlayerController Player;

    [Header("Attack Config")]
    [SerializeField]
    [Range(0.1f, 5f)]
    private float AttackCooldown = 2;
    [SerializeField] private float AttackRange = 2f;

    [Header("Sensors")]
    [SerializeField] private PlayerSensor FollowPlayerSensor;
    [SerializeField] private PlayerSensor MeleePlayerSensor;

    [Space]
    [Header("Debug Info")]
    [SerializeField] private bool IsInMeleeRange;
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

        //Add States
        BossFSM.AddState(BossState.Idle, new BIdleState(false, this));

        BossFSM.AddState(BossState.Chase, new BChaseState(true, this, Player.transform));

        BossFSM.AddState(BossState.CombatMove, new BCombatMoveState(true, this, Player.transform));

        BossFSM.AddState(BossState.Attack, new BAttackState(true, this, OnAttack));

        BossFSM.AddState(BossState.Hit, new BHitState(true, this));

        BossFSM.AddTriggerTransition(StateEvent.DetectPlayer, new Transition<BossState>(BossState.Idle, BossState.Chase));

        //Add Transitions
        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Chase,
                BossState.CombatMove,
                transition =>
                    Vector3.Distance(
                        Player.transform.position,
                        transform.position
                    ) <= 6f
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatMove,
                BossState.Attack,
                ShouldMelee
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.CombatMove,
                BossState.Chase,
                transition =>
                    Vector3.Distance(
                        Player.transform.position,
                        transform.position
                    ) > 7f
            )
        );

        BossFSM.AddTransition(
            new Transition<BossState>(
                BossState.Attack,
                BossState.CombatMove,
                transition => true
            )
        );

        // Hit Transitions
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

        // Exit Hit
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
        MeleePlayerSensor.OnPlayerEnter += MeleePlayerSensor_OnPlayerEnter;
        MeleePlayerSensor.OnPlayerExit += MeleePlayerSensor_OnPlayerExit;
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

    private bool ShouldMelee(Transition<BossState> Transition) => LastAttackTime + AttackCooldown <= Time.time && IsInMeleeRange;

    private bool IsWithinIdleRange(Transition<BossState> Transition)
    {
        float distance = Vector3.Distance(Player.transform.position, transform.position);

        return distance <= AttackRange;
    }

    private bool IsNotWithinIdleRange(Transition<BossState> Transition)
    {
        return !IsWithinIdleRange(Transition);
    }

    private void MeleePlayerSensor_OnPlayerExit(Vector3 LastKnownPosition)
    {
        IsInMeleeRange = false;
    }

    private void MeleePlayerSensor_OnPlayerEnter(Transform Player)
    {
        IsInMeleeRange = true;
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

        Invoke(nameof(ResetHit), 0.3f);
    }

    private void ResetHit()
    {
        gotHit = false;
    }

    private void Update()
    {
        BossFSM.OnLogic();
    }
}
