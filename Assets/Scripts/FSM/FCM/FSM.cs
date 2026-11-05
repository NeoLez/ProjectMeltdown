using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM /*<TKey>*/  //no es monobehavoir porque lo creo en mi Agente
{
    //guardo estado actual
    IState _currentState;

    //guardo, en IState, AgentStates
    Dictionary<AgentStates,IState> _allstates = new Dictionary<AgentStates,IState>();
    public enum AgentStates
    {
        Idle, Patrol, Chase, MeleeAttack, JumpAttack
    }
    
    //se encarga de decidir cual es el estado actual y correrlo
    public void ArtficialUpdate()
    {
        if(_currentState != null )
        {
            _currentState.OnUpdate();
        }
    }
    
    public void ChangeState(AgentStates newState)
    {
        //por las duda                salgo del ultimo estado
        if(_currentState != null)  _currentState.OnExit();

        //si ese nuevo estado esta, lo establecemos como el _currentState
        if (_allstates.ContainsKey(newState)) _currentState = _allstates[newState];

        //entro al nuevo estado
        _currentState?.OnEnter();
    }
    public void AddState(AgentStates newState, IState state)
    {
        //si ya tiene algo, retorna
        if (_allstates.ContainsKey(newState)) return;

        //sino agregalo a la lista
        _allstates.Add(newState, state);
    }
}
