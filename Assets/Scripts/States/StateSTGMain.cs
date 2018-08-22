using UnityEngine;
using System.Collections.Generic;

public class StateSTGMain : IState,ICommand
{
    /// <summary>
    /// 等待
    /// </summary>
    private const int StateWait = 1;
    /// <summary>
    /// 初始化STGMain
    /// </summary>
    private const int StateInitSTGMain = 2;
    /// <summary>
    /// 加载对应的stage.lua
    /// </summary>
    private const int StateLoadStageLua = 3;
    /// <summary>
    /// 执行clear
    /// </summary>
    private const int StateClear = 4;
    /// <summary>
    /// STG开始前的初始化
    /// </summary>
    private const int StateInitSTG = 5;
    /// <summary>
    /// update STG
    /// </summary>
    private const int StateUpdateSTG = 6;

    private int _curStageId;
    private int _nextStageId;
    /// <summary>
    /// 装嵌状态
    /// </summary>
    private int _curState;

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
                //OnEnterStageHandler((int)datas[0]);
                break;
            case CommandConsts.STGInitComplete:
                OnSTGInitComplete();
                break;
            case CommandConsts.STGLoadStageLuaComplete:
                OnLoadStageLuaComplete();
                break;
            case CommandConsts.RetryGame:
                OnRetryGame();
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
        // 初始化STGMain
        _curState = StateInitSTGMain;
        // 打开loadingView
        List<object> commandList = new List<object>();
        commandList.Add(CommandConsts.STGInitComplete);
        commandList.Add(CommandConsts.STGLoadStageLuaComplete);
        object[] commandArr = commandList.ToArray();
        UIManager.GetInstance().ShowView(WindowName.GameLoadingView, commandArr);
        // 添加监听
        CommandManager.GetInstance().Register(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryStage, this);
        // 设置需要载入的stageId
        _nextStageId = (int)datas[0];
        // 实例化STGMain
        if ( Global.STGMain == null )
        {
            _stgMain = new STGMain();
            Global.STGMain = _stgMain;
        }
    }

    public void OnStateExit()
    {

    }

    public void OnUpdate()
    {
        if ( _curState == StateInitSTGMain )
        {
            OnStateInitSTGMainUpdate();
        }
        else if ( _curState == StateLoadStageLua )
        {
            OnStateLoadStageLuaUpdate();
        }
        else if ( _curState == StateClear )
        {
            OnStateClearUpdate();
        }
        else if ( _curState == StateInitSTG )
        {
            OnStateInitSTGUpdate();
        }
        else if ( _curState == StateUpdateSTG )
        {
            if (!Global.IsPause)
            {
                _stgMain.Update();
            }
        }
    }

    private void OnEnterStageHandler(int nextStageId)
    {
        _nextStageId = nextStageId;
    }

    /// <summary>
    /// STG初始化完成回调
    /// <para>执行加载stage.lua</para>
    /// </summary>
    private void OnSTGInitComplete()
    {
        _curState = StateLoadStageLua;
    }

    /// <summary>
    /// 加载stage.lua完成回调
    /// <para>执行下一个状态-->updateSTG</para>
    /// </summary>
    private void OnLoadStageLuaComplete()
    {
        _curState = StateUpdateSTG;
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    private void OnRetryGame()
    {
        _curState = StateClear;
        _nextStageId = 1;
        // 打开loadingView
        List<object> commandList = new List<object>();
        commandList.Add(CommandConsts.STGLoadStageLuaComplete);
        object[] commandArr = commandList.ToArray();
        UIManager.GetInstance().ShowView(WindowName.GameLoadingView, commandArr);
    }

    /// <summary>
    /// 初始化STGMain
    /// </summary>
    private void OnStateInitSTGMainUpdate()
    {
        _stgMain.Init();
        // 打开界面
        UIManager.GetInstance().ShowView(WindowName.GameInfoView, null);
        UIManager.GetInstance().ShowView(WindowName.STGBottomView, null);
        UIManager.GetInstance().ShowView(WindowName.GameMainView);
        _curState = StateLoadStageLua;
    }

    /// <summary>
    /// 加载Stage.lua
    /// </summary>
    private void OnStateLoadStageLuaUpdate()
    {
        _curState = StateWait;
        _curStageId = _nextStageId;
        _stgMain.EnterStage(_curStageId);
        _curState = StateUpdateSTG;
    }

    /// <summary>
    /// 执行clear
    /// </summary>
    private void OnStateClearUpdate()
    {
        _curState = StateWait;
        _stgMain.Clear();
        _curState = StateInitSTG;
    }


    private void OnStateInitSTGUpdate()
    {
        _stgMain.InitSTG();
        _curState = StateLoadStageLua;
    }
}