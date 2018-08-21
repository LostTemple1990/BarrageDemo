public interface IFSM 
{
    void Init();
    void AddState(int stateId,IState state);
    int GetCurStateId();
    IState GetCurState();
    void SetNextStateId(int stateId,object[] datas=null);
    void Update();
}
