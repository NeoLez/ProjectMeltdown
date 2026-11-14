using System.Collections;
using Timers;
using UnityEngine;
using UnityEngine.AI;
using static FSM;

namespace Root
{
    public class MeleeAttackState : IState
    {
        private FSM _fsm;
        private ZombieStateMachine _zombieStateMachine;
        private NavMeshAgent _agent;

        private readonly Timer _attackTimer = new();
        private bool _isAttaking;


        public MeleeAttackState(FSM fsm, NavMeshAgent agent, ZombieStateMachine state)
        {
            _fsm = fsm;
            _agent = agent;
            _zombieStateMachine = state;
        }

        public void OnEnter()
        {
            _agent.isStopped = true;
            _isAttaking = false;
        }

        public void OnExit()
        {
            _agent.isStopped = false;
            _zombieStateMachine.StopAllCoroutines();
        }

        public void OnUpdate()
        {
            if(_zombieStateMachine.IsPlayerInChaseDistance())
            {
                _fsm.ChangeState(AgentStates.Chase);
            }

            if (_isAttaking)
            {
                return;
            }

            FollowPlayerRotation();

            if (_attackTimer.IsCompleted())
            {
                if (_zombieStateMachine.AreEntitiesFacing())
                {
                    _zombieStateMachine.StartCoroutine(AttackRoutine());
                }
                else
                {
                    _zombieStateMachine.StopCoroutine(AttackRoutine());
                }
            }
        }

        private void FollowPlayerRotation()
        {
            Quaternion startingRot = _zombieStateMachine.transform.rotation;
            Vector3 direction = GameManager.Player.transform.position - _zombieStateMachine.transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            _zombieStateMachine.transform.rotation = lookRotation;
        }

        private IEnumerator AttackRoutine()
        {  
            while (true)
            {
                if (_agent.enabled)
                {
                    _isAttaking = true;
                    _agent.isStopped = true;

                    _zombieStateMachine.animator.Play("Attack");

                    if (!_zombieStateMachine.AreEntitiesFacing()) break;
                }

                yield return null;
            }
            ResetTimer();
        }

        private void ResetTimer()
        {
            _attackTimer.Reset(_zombieStateMachine.attackCooldown);
            _isAttaking = false;
        }
    }
}
