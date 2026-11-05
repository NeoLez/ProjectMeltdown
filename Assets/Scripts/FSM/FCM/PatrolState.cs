using Root;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static FSM;

public class PatrolState : IState
{
    private FSM _fsm;
    private ZombieStateMachine _zombieStateMachine;
    private NavMeshAgent _agent;
    private Transform[] Waypoints;
    private EnemyFov _fov;

    private int _waypointIndex;
    private bool _isChasing;
    public PatrolState(FSM fsm, NavMeshAgent agent, Transform[] waypoints, EnemyFov fov, ZombieStateMachine state)
    {
        _fsm = fsm;
        _agent = agent;
        Waypoints = waypoints;
        _fov = fov;
        _zombieStateMachine = state;
    }

    public void OnEnter()
    {
        _isChasing = false;
        _agent.speed = _zombieStateMachine.patrolSpeed;
        _agent.SetDestination(Waypoints[_waypointIndex].transform.position);
    }

    public void OnExit()
    {
        _zombieStateMachine.StopAllCoroutines();
    }


    private IEnumerator PatrolRoutine()
    {
        if (Waypoints.Length < 0)
        {
            Debug.LogError("Assign waypoints!!");
            yield break;
        }

        if (_isChasing)
        {
            yield break;
        }

        if (_agent.enabled && _agent.isOnNavMesh && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _waypointIndex++;

            if (_waypointIndex >= Waypoints.Length)
            {
                _waypointIndex = 0;
            }

            _agent.SetDestination(Waypoints[_waypointIndex].transform.position);
        }
    }

    public void OnUpdate()
    {
        if (!_fov.DetectPlayer(GameManager.Player))
        {
            _agent.enabled = true;
            _zombieStateMachine.animator.SetBool("walking", true);
            _zombieStateMachine.StartCoroutine(PatrolRoutine());
        }
        else
        {
            if(CanActiveAbility())
            {
                _fsm.ChangeState(AgentStates.JumpAttack);
            }
            else
            {
                _isChasing = true;
                _fsm.ChangeState(AgentStates.Chase);
            }
        }
    }

    private bool CanActiveAbility() 
    {
        return _zombieStateMachine.CanPerformJump(_zombieStateMachine.jumpChance);
    }
}
