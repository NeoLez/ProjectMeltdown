using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    FSM _fsm;

    public IdleState(FSM fsm)
    {
        _fsm = fsm;
    }
    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
    }

}
