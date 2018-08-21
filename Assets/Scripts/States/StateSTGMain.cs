using UnityEngine;

public class StateSTGMain : IState,ICommand
{
    /// <summary>
    /// 等待初始化
    /// </summary>
    private const int StateWaitForInit = 1;
    /// <summary>
    /// update STG
    /// </summary>
    private const int StateUpdateSTG = 2;

    private int _curStageId;
    private int _nextStageId;
    private int _curState;
    private int _time;

    public StateSTGMain()
    {
        _curStageId = -1;
        _nextStageId = -1;
    }

    private GameStateMachine _fsm;

    private STGMain _stgMain;

    public void Execute(int cmd, object[] datas)
    {
        switch (cmd)
        {
            case CommandConsts.EnterStage:
                OnEnterStageHandler((int)datas[0]);
                break;
        }
    }

    public int GetStateId()
    {
        return _curStageId;
    }

    public void OnInit(IFSM fsm)
    {
        _fsm = fsm as GameStateMachine;
    }

    public void OnStateEnter(object[] datas=null)
    {
        CommandManager.GetInstance().Register(CommandConsts.EnterStage, this);
        _nextStageId = (int)datas[0];
        if ( Global.STGMain == null )
        {
            _stgMain = new STGMain();
            Global.STGMain = _stgMain;
        }
        _curState = StateWaitForInit;
    }

    public void OnStateExit()
    {
        
    }

    public void OnUpdate()
    {
        if ( _nextStageId != -1 )
        {
            EnterNextStage();
        }
        else if ( _stgMain.IsStart )
        {
            if ( !Global.IsPause )
            {
                _stgMain.Update();
            }
        }
    }

    private void OnEnterStageHandler(int nextStageId)
    {
        _nextStageId = nextStageId;
    }

    private void EnterNextStage()
    {
        _curStageId = _nextStageId;
        _nextStageId = -1;
        if ( _stgMain.IsStart )
        {
            _stgMain.Clear();
        }
        else
        {
            _stgMain.Init();
            // 打开界面
            UIManager.GetInstance().ShowView(WindowName.GameInfoView, null);
            UIManager.GetInstance().ShowView(WindowName.STGBottomView, null);
            UIManager.GetInstance().ShowView(WindowName.GameMainView);
        }
        _stgMain.EnterStage(_curStageId);
    }
}