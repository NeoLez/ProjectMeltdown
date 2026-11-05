using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static FSM;

namespace Root
{
    public class IdleState : IState
    {
        private FSM _fsm;
        private ZombieStateMachine _zombieStateMachine;

        public IdleState(FSM fsm, NavMeshAgent agent, ZombieStateMachine state)
        {
            _fsm = fsm;
            _zombieStateMachine = state;
        }

        public void OnEnter()
        {
            _zombieStateMachine.StartCoroutine(Wait());
        }

        public void OnExit()
        {
            _zombieStateMachine.StopCoroutine(Wait());
        }

        public void OnUpdate()
        {
         
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(2f);

            _fsm.ChangeState(AgentStates.Patrol);
        }
    }
}
