using UnityEngine;

public class StateSTGMain : IState
{
    private int _stateId;
    private GameStateMachine _fsm;

    private STGMain _stgMain;

    public int GetStateId()
    {
        return _stateId;
    }

    public void OnInit(IFSM fsm)
    {
        _fsm = fsm as GameStateMachine;
    }

    public void OnStateEnter()
    {
        _stgMain = new STGMain();
        _stgMain.Init();
    }

    public void OnStateExit()
    {
        
    }

    public void OnUpdate()
    {
        _stgMain.Update();
    }
}