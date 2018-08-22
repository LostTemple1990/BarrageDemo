﻿using UnityEngine;
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
    /// update STG
    /// </summary>
    private const int StateUpdateSTG = 4;

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
        // 实例化MainSTG,初始化参数
        CommandManager.GetInstance().Register(CommandConsts.STGInitComplete, this);
        CommandManager.GetInstance().Register(CommandConsts.STGLoadStageLuaComplete, this);
        CommandManager.GetInstance().Register(CommandConsts.EnterStage, this);
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
        CommandManager.GetInstance().Remove(CommandConsts.STGInitComplete, this);
        CommandManager.GetInstance().Remove(CommandConsts.STGLoadStageLuaComplete, this);
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
    }

    private void OnWaitForInitUpdate()
    {

    }

    /// <summary>
    /// 初始化STGMain
    /// </summary>
    private void OnStateInitSTGMainUpdate()
    {
        _curState = StateWait;
        _stgMain.Init();
        // 打开界面
        UIManager.GetInstance().ShowView(WindowName.GameInfoView, null);
        UIManager.GetInstance().ShowView(WindowName.STGBottomView, null);
        UIManager.GetInstance().ShowView(WindowName.GameMainView);
    }

    /// <summary>
    /// 加载Stage.lua
    /// </summary>
    private void OnStateLoadStageLuaUpdate()
    {
        _curState = StateWait;
        _curStageId = _nextStageId;
        _stgMain.EnterStage(_curStageId);
    }
}