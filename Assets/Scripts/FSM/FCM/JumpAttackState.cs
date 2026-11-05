using System;
using System.Collections;
using Timers;
using UnityEngine;
using UnityEngine.AI;
using static FSM;

namespace Root
{
    public class JumpAttackState : IState
    {
        private FSM _fsm;
        private ZombieStateMachine _zombieStateMachine;
        private NavMeshAgent _agent;
        private bool _isJumping;
        private Transform _transform;

        private HealthControl _playerHealth;

        private Action _behaviour;
        public JumpAttackState(FSM fsm, NavMeshAgent agent, Transform transform,  ZombieStateMachine state)
        {
            _fsm = fsm;
            _agent = agent;
            _zombieStateMachine = state;
            _transform = transform;
            _playerHealth = GameManager.Player.gameObject.GetComponent<HealthControl>();
        }

        public void OnEnter()
        {
            _isJumping = true;

            _zombieStateMachine.StartCoroutine(PreparingJumpRoutine());
        }

        public void OnExit()
        {
            _zombieStateMachine.StopAllCoroutines();
        }

        public void OnUpdate()
        {
            if (_isJumping) return;

            _behaviour?.Invoke(); 
        }

        private void JumpAttackBehaviour()
        {
            if(_agent.enabled)
            {
                _zombieStateMachine.animator.SetBool("running", true);
                _agent.SetDestination(GameManager.Player.gameObject.transform.position);

                if (_zombieStateMachine.CanJumpAttack())
                {
                    //Debug.Log("entre");
                    _zombieStateMachine.StartCoroutine(JumpAttackRoutine());
                    return;
                }
                
            }
        }

        private void AttackBehaviour()
        {
            if (_zombieStateMachine.IsPlayerInJumpAttackRange())
            {
                Attack();
            }
            _zombieStateMachine.StopCoroutine(JumpAttackRoutine());
            _zombieStateMachine.StartCoroutine(RecoverFromJumpRoutine());                    
        }


        private IEnumerator PreparingJumpRoutine()
        {
            _agent.enabled = false;
            yield return new WaitForSeconds(_zombieStateMachine.jumpPrepareDelay);
            _agent.enabled = true;

            _agent.speed = _zombieStateMachine.chaseSpeed;
            _isJumping = false;

            _zombieStateMachine.animator.Play("scream");
            _behaviour = JumpAttackBehaviour;
        }


        private IEnumerator RecoverFromJumpRoutine()
        {
            //Debug.Log("me recupere");
            _zombieStateMachine.animator.Play("getup");
            _agent.enabled = false;
            yield return new WaitForSeconds(_zombieStateMachine.failedJumpAttackCooldown);
            _agent.enabled = true;

            _behaviour = null;
            _fsm.ChangeState(AgentStates.Idle);
        }

        private void Attack()
        {
            if (_agent.enabled)
            {
                _playerHealth.TakeDamage(_zombieStateMachine.jumpDamage);
            }

        }

        private IEnumerator JumpAttackRoutine()
        {
            _agent.enabled = false;

            _zombieStateMachine.animator.Play("jump");
            Vector3 startingPosition = _zombieStateMachine.gameObject.transform.position;
            Vector3 lastPlayerPosition = GameManager.Player.transform.position;
            for (float time = 0; time < 1; time += Time.deltaTime * _zombieStateMachine.jumpSpeed)
            {
                _zombieStateMachine.gameObject.transform.position = Vector3.Lerp(startingPosition, lastPlayerPosition, time)
                    + Vector3.up * _zombieStateMachine.heightCurve.Evaluate(time);
                _zombieStateMachine.gameObject.transform.rotation = Quaternion.Slerp(_zombieStateMachine.gameObject.transform.rotation,
                    Quaternion.LookRotation(lastPlayerPosition - _zombieStateMachine.gameObject.transform.position), time);

                yield return null;
            }

            _zombieStateMachine.usedTime = Time.time;
            _agent.enabled = true;
            _zombieStateMachine.animator.SetBool("running", false);
            _isJumping = false;
            _behaviour = AttackBehaviour;
        }
    }
}
