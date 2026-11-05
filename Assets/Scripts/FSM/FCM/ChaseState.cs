using UnityEngine.AI;
using static FSM;

namespace Root
{
    public class ChaseState : IState
    {
        private FSM _fsm;
        private ZombieStateMachine _zombieStateMachine;
        private NavMeshAgent _agent;

        private Player _player;
        public ChaseState(FSM fsm, NavMeshAgent agent, ZombieStateMachine state)
        {
            _fsm = fsm;
            _agent = agent;
            _zombieStateMachine = state;
            _player = GameManager.Player;
        }
        public void OnEnter()
        {
            _zombieStateMachine.animator.Play("scream");
            _zombieStateMachine.animator.SetBool("running", true);
            _agent.speed = _zombieStateMachine.chaseSpeed;
        }

        public void OnExit()
        {
            
        }

        public void OnUpdate()
        {
            if (!_zombieStateMachine.fov.DetectPlayer(_player))
            {
                
                _fsm.ChangeState(AgentStates.Patrol);
                _zombieStateMachine.animator.SetBool("running", false);
                return;
            }
            else
            {
                _zombieStateMachine.animator.SetBool("running", true);
                _agent.SetDestination(GameManager.Player.gameObject.transform.position);
            }

            if (_zombieStateMachine.IsPlayerInAttackRange())
            {
                _fsm.ChangeState(AgentStates.MeleeAttack);
            }
        }


    }
}
