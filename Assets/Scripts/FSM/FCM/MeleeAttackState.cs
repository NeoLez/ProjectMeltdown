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

        private HealthControl _playerHealth;

        public MeleeAttackState(FSM fsm, NavMeshAgent agent, ZombieStateMachine state)
        {
            _fsm = fsm;
            _agent = agent;
            _zombieStateMachine = state;
            _playerHealth = GameManager.Player.gameObject.GetComponent<HealthControl>();
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

        
            if (_isAttaking) return;        

            if (_attackTimer.IsCompleted())
            {
                _zombieStateMachine.StartCoroutine(AttackRoutine());
            }
           
        }

        private IEnumerator AttackRoutine()
        {
            //WaitForSeconds wait = new WaitForSeconds(_zombieStateMachine.attackDelay);
            while (true)
            {
                if (_agent.enabled)
                {
                    _isAttaking = true;
                    _agent.isStopped = true;

                    //_playerHealth.TakeDamage(_zombieStateMachine.damage);
                    _zombieStateMachine.animator.Play("Attack");
                    _attackTimer.Reset(_zombieStateMachine.attackCooldown);
                    _isAttaking = false;
                }

                yield return null;
            }

        }

    }
}
