using Root;
using UnityEngine;
using UnityEngine.AI;
using static FSM;

public class ZombieStateMachine : MonoBehaviour
{
    FSM _fsm;
    //TODO- ahcer serializable y protected
    public Transform zombieTransform;
    public NavMeshAgent agent;
    public Transform[] Waypoints;
    public LayerMask layers;
    public EnemyFov fov;
    public float chaseDistance;
    public Animator animator;

    [Header("Damage")]
    public float damage;
    public float jumpDamage;

    [Header("Speed")]
    public float chaseSpeed;
    public float patrolSpeed;

    [Header("MeleeAttack")]
    public float attackRange;
    public float attackDelay = 0.8f;
    public float attackCooldown;

    [Header("JumpAttack")]
    public AnimationCurve heightCurve;
    public float minJumpDistance;
    public float maxJumpDistance;
    public float jumpSpeed = 2f;
    public float jumpCooldown;
    public float jumpPrepareDelay;
    public float failedJumpAttackCooldown;
    public float usedTime;
    [Range(0, 100)]
    public float jumpChance;

    void Awake()
    {
        _fsm = new FSM();

        _fsm.AddState(AgentStates.Patrol, new PatrolState(_fsm, agent, Waypoints, fov, this));
        _fsm.AddState(AgentStates.Chase, new ChaseState(_fsm, agent, this));
        _fsm.AddState(AgentStates.RangeAttack, new MeleeAttackState(_fsm, agent, this));
        _fsm.AddState(AgentStates.JumpAttack, new JumpAttackState(_fsm, agent, zombieTransform, this));

        _fsm.ChangeState(AgentStates.Patrol);
        /*agent.enabled = false;

        GameManager.MapGeneration.OnBeforeRebase += DisableForOneSecond;*/
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

    void Update() => _fsm.ArtficialUpdate();


    public bool IsPlayerInAttackRange()
    {
        return Physics.SphereCast(zombieTransform.position, attackRange, zombieTransform.forward,
            out RaycastHit hit, attackRange, layers);
    }

    public bool IsPlayerInChaseDistance()
    {
        float distance = Vector3.Distance(zombieTransform.position, GameManager.Player.transform.position);

        return distance >= chaseDistance;
    }

    /*public bool IsPlayerInChaseDistance()
    {
        Vector3 dist = (GameManager.Player.transform.position - zombieTransform.position).normalized;

        return (dist.sqrMagnitude >= chaseDistance);
    }*/

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

}
