using EnemyFSM;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityHFSM;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
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

    private StateMachine<EnemyState, StateEvent> EnemyFSM;
    private Animator Animator;
    private NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        EnemyFSM = new StateMachine<EnemyState, StateEvent>();

        //Add States
        EnemyFSM.AddState(EnemyState.Idle, new EIdleState(false, this));
        EnemyFSM.AddState(EnemyState.Chase, new EChaseState(true, this, Player.transform));
        EnemyFSM.AddState(EnemyState.Attack, new EAttackState(true, this, OnAttack));
        EnemyFSM.AddState(EnemyState.Hit, new EHitState(true, this));

        //Add Transitions
        EnemyFSM.AddTriggerTransition(StateEvent.DetectPlayer, new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase));
        EnemyFSM.AddTriggerTransition(StateEvent.LostPlayer, new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Chase,
              (transition) => IsInChasingRange
                              && Vector3.Distance(Player.transform.position, transform.position) > Agent.stoppingDistance)
        );
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Idle,
            (transition) => !IsInChasingRange
                            || Vector3.Distance(Player.transform.position, transform.position) <= Agent.stoppingDistance)
        );

        //Attack Transitions
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Attack, ShouldMelee)
        {
            forceInstantly = true
        });
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Attack, ShouldMelee)
        {
            forceInstantly = true
        });
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Attack, EnemyState.Chase, IsNotWithinIdleRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Attack, EnemyState.Chase, transition => IsInChasingRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Attack, EnemyState.Idle, transition => !IsInChasingRange));

        //Hit Reaction Transitions
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Idle, EnemyState.Hit, transition => gotHit)
        {
            forceInstantly = true
        });

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Chase, EnemyState.Hit, transition => gotHit)
        {
            forceInstantly = true
        });

        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Attack, EnemyState.Hit, transition => gotHit)
        {
            forceInstantly = true
        });

        // Hit to Chase Transition
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Hit, EnemyState.Chase,transition => !gotHit && IsInChasingRange));
        EnemyFSM.AddTransition(new Transition<EnemyState>(EnemyState.Hit, EnemyState.Idle, transition => !gotHit));



        EnemyFSM.Init();
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
        EnemyFSM.Trigger(StateEvent.LostPlayer);
        IsInChasingRange = false;
    }

    private void FollowPlayerSensor_OnPlayerEnter(Transform Player) 
    {
        EnemyFSM.Trigger(StateEvent.DetectPlayer);
        IsInChasingRange = true;
    }

    private bool ShouldMelee(Transition<EnemyState> Transition) => LastAttackTime + AttackCooldown <= Time.time && IsInMeleeRange;

    private bool IsWithinIdleRange(Transition<EnemyState> Transition)
    {
        float distance = Vector3.Distance(Player.transform.position, transform.position);

        return distance <= AttackRange;
    }

    private bool IsNotWithinIdleRange(Transition<EnemyState> Transition)
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

    private void OnAttack(State<EnemyState, StateEvent> State) 
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
        EnemyFSM.OnLogic();
    }
}
