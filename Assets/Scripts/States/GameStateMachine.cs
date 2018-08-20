using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateMachine : IFSM
{
    private Dictionary<int, IState> _states;
    private IState _curState;

    private int _curStateId;
    private int _nextStateId;

    public void Init()
    {
        _states = new Dictionary<int, IState>();
        _curState = null;
        _nextStateId = -1;
        _curStateId = -2;
    }

    public void AddState(int stateId, IState newState)
    {
        IState state;
        if ( !_states.TryGetValue(stateId,out state) )
        {
            _states.Add(stateId, newState);
            newState.OnInit(this);
        }
        else
        {
            Logger.LogError("State " + stateId + " is already exist!");
        }
    }

    public IState GetCurState()
    {
        return _curState;
    }

    public int GetCurStateId()
    {
        return _curStateId;
    }

    public void SetNextStateId(int stateId)
    {
        _nextStateId = stateId;
    }

    public void Update()
    {
        if (_curState != null)
        {
            _curState.OnUpdate();
        }
        if ( _nextStateId != -1 )
        {
            if ( _nextStateId == _curStateId )
            {
                _nextStateId = -1;
                return;
            }
            IState nextState;
            if ( !_states.TryGetValue(_nextStateId,out nextState) )
            {
                _nextStateId = -1;
                Logger.Log("Change to next state fail! Reason : Next state is not exist!");
                return;
            }
            if ( _curState != null )
            {
                _curState.OnStateExit();
            }
            _curStateId = _nextStateId;
            _nextStateId = -1;
            _curState = nextState;
            nextState.OnStateEnter();
        }
    }
}
