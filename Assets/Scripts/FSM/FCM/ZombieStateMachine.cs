using System;
using Root;
using UnityEngine;
using UnityEngine.AI;
using static FSM;
using Random = UnityEngine.Random;

public class ZombieStateMachine : MonoBehaviour
{
    [SerializeField] private bool testOption;
    public Transform zombieTransform;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform[] Waypoints;
    public EnemyFov fov;
    public LayerMask playerRayCastMask;
    public float chaseDistance;

    [Header("Damage")]
    public float damage;
    public float jumpDamage;

    [Header("Speed")]
    public float patrolSpeed;
    public float chaseSpeed;

    [Header("MeleeAttack")]
    public float maxFacingAngle;
    public float attackRange;
    public float attackDelay = 0.8f;
    public float attackCooldown;

    [Header("JumpAttack")]
    public AnimationCurve heightCurve;
    public float jumpAttackRange;
    public float minJumpDistance;
    public float maxJumpDistance;
    public float jumpSpeed = 2f;
    public float jumpCooldown;
    public float jumpPrepareDelay;
    public float failedJumpAttackCooldown;
    [Range(0, 100)]
    public float jumpChance;
    [HideInInspector]
    public float usedTime;

    private FSM _fsm;
    private float dotThreshold;
    public NavMeshAgent Agent => agent;

    public Action OnFinishedAttacking;
    public bool IsRecovering {  get;  set; }

    [Header("Ground Check")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 offset;
    private Vector3 _lastZombiePosition;
    private Vector3 _sampleIntialYPosition;
    public Vector3 LastGroundedPosition
    {
        get => _lastZombiePosition;
        set
        {
            _lastZombiePosition = new Vector3(_lastZombiePosition.x, _sampleIntialYPosition.y, _sampleIntialYPosition.z);
        }
    }


    void Awake()
    {
        _fsm = new FSM();

        _fsm.AddState(AgentStates.Idle, new IdleState(_fsm, agent, this));
        _fsm.AddState(AgentStates.Patrol, new PatrolState(_fsm, agent, Waypoints, fov, this));
        _fsm.AddState(AgentStates.Chase, new ChaseState(_fsm, agent, this));
        _fsm.AddState(AgentStates.MeleeAttack, new MeleeAttackState(_fsm, agent, this));
        _fsm.AddState(AgentStates.JumpAttack, new JumpAttackState(_fsm, agent, zombieTransform, this));

        if(testOption)
        {
            _fsm.ChangeState(AgentStates.Patrol);
        }
        else
        {
            agent.enabled = false;

            GameManager.MapGeneration.OnBeforeRebase += DisableForOneSecond;
        }
    }

    private void Start()
    {
        dotThreshold = Mathf.Cos(maxFacingAngle * Mathf.Deg2Rad);

        _sampleIntialYPosition.y = transform.position.y;
    }

    private void Update()
    {
        _fsm.ArtficialUpdate();

        IsGrounded();
    }


    private void DisableForOneSecond()
    {
        enabled = false;
        agent.enabled = false;
        Invoke("EnableAgain", 2);
    }

    private void EnableAgain()
    {
        agent.enabled = true;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogError($"{gameObject.name} spawned too far away from a valid NavMesh!", this);
        }
        enabled = true;
        _fsm.ChangeState(AgentStates.Patrol);

    }


    public bool IsPlayerInAttackRange()
    {
        return Physics.SphereCast(zombieTransform.position, attackRange, zombieTransform.forward,
            out RaycastHit hit, attackRange, playerRayCastMask) && hit.collider.TryGetComponent(out Player player);
    }

    public bool IsPlayerInJumpAttackRange()
    {
        return Physics.Raycast(zombieTransform.position, zombieTransform.forward, out var hit, jumpAttackRange, playerRayCastMask) 
            && hit.collider.TryGetComponent(out Player player);
    }

    public bool IsPlayerInChaseDistance()
    {
        float distance = Vector3.Distance(zombieTransform.position, GameManager.Player.transform.position);

        if(distance <= chaseDistance)
        {
            return false;
        }
        Vector3 dist = GameManager.Player.transform.position - zombieTransform.position;
        dist.y = 0;

        float dotProduct = Vector3.Dot(transform.forward, dist);
        return dotProduct >= dotThreshold;
    }

    public bool CanJumpAttack()
    {
        float distance = Vector3.Distance(zombieTransform.position, GameManager.Player.transform.position);

        return /*usedTime + jumpCooldown < Time.deltaTime 
            && */distance >= minJumpDistance
            && distance <= maxJumpDistance;
    }

    public bool CanPerformJump(float probability)
    {
        float randomChance = Random.Range(0, 100);
        if (randomChance <= probability)
        {
            return true;
        }
        return false;
    }

    public bool AreEntitiesFacing()
    {
        Vector3 direction = GameManager.Player.transform.position - zombieTransform.position;
        direction.y = 0;

        float angle = Vector3.Angle(zombieTransform.forward, direction);
        return angle <= maxFacingAngle;
    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        if(!Physics.Raycast(transform.position + offset, -transform.up + offset, out hit, maxDistance))
        {
            return false;
        }
        _lastZombiePosition = hit.point;
        return true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + offset, transform.position + offset + Vector3.down * maxDistance);
    }

    private void OnDestroy()
    {
        GameManager.MapGeneration.OnBeforeRebase -= DisableForOneSecond;
    }
}
