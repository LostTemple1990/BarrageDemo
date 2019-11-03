public interface IState
{
    void OnInit(IFSM fsm);
    void OnStateEnter(object data=null);
    void OnStateExit();
    void OnUpdate();
    int GetStateId();
}