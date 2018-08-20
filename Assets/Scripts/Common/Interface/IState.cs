public interface IState
{
    void OnInit(IFSM fsm);
    void OnStateEnter();
    void OnStateExit();
    void OnUpdate();
    int GetStateId();
}