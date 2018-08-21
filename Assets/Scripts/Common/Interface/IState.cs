public interface IState
{
    void OnInit(IFSM fsm);
    void OnStateEnter(object[] datas=null);
    void OnStateExit();
    void OnUpdate();
    int GetStateId();
}