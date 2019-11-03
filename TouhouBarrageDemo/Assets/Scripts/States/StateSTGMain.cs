﻿using System;
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

    private string _curStageName;
    private string _nextStageName;
    /// <summary>
    /// 当前状态
    /// </summary>
    private int _curState;
    /// <summary>
    /// 录像是否已经结束
    /// </summary>
    private bool _isReplayFinish;
    /// <summary>
    /// 当前STG的基本数据
    /// </summary>
    private STGData _stgData;

    public StateSTGMain()
    {
        _curStageName = "";
        _nextStageName = "";
    }

    private GameStateMachine _fsm;

    private STGMain _stgMain;

    public void Execute(int cmd, object data)
    {
        switch (cmd)
        {
            case CommandConsts.STGInitComplete:
                OnSTGInitComplete();
                break;
            case CommandConsts.STGLoadStageLuaComplete:
                OnLoadStageLuaComplete();
                break;
            case CommandConsts.RetryGame:
                OnRetryGame();
                break;
            case CommandConsts.ContinueGame:
                OnContinueGame();
                break;
            case CommandConsts.StageClear:
                OnStageClear();
                break;
            case CommandConsts.SaveReplay:
                OnSaveReplay();
                break;
        }
    }

    public int GetStateId()
    {
        return (int)eGameState.STG;
    }

    public void OnInit(IFSM fsm)
    {
        _fsm = fsm as GameStateMachine;
    }

    public void OnStateEnter(object data=null)
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
        CommandManager.GetInstance().Register(CommandConsts.ContinueGame, this);
        CommandManager.GetInstance().Register(CommandConsts.StageClear, this);
        CommandManager.GetInstance().Register(CommandConsts.SaveReplay, this);
        _stgData = (STGData)data;
        // 设置需要载入的stage
        _nextStageName = _stgData.stageName;
        //_isInReplayMode = (bool)datas[1];
        //Global.IsInReplayMode = _isInReplayMode;
        // 实例化STGMain
        if (_stgMain == null )
        {
            _stgMain = new STGMain();
        }
    }

    public void OnStateExit()
    {
        CommandManager.GetInstance().Remove(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Remove(CommandConsts.ContinueGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.StageClear, this);
        CommandManager.GetInstance().Remove(CommandConsts.SaveReplay, this);
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
            OnSTGMainUpdate();
        }
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
        Logger.Log("Retry Game");
        _curState = StateClear;
        _nextStageName = _curStageName;
        // 打开loadingView
        List<object> commandList = new List<object>();
        commandList.Add(CommandConsts.STGLoadStageLuaComplete);
        object[] commandArr = commandList.ToArray();
        UIManager.GetInstance().ShowView(WindowName.GameLoadingView, commandArr);
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    private void OnContinueGame()
    {
        Global.IsPause = false;
    }

    /// <summary>
    /// 初始化STGMain
    /// </summary>
    private void OnStateInitSTGMainUpdate()
    {
        _stgMain.Init();
        _stgData.seed = InitSeed();
        MTRandom.Init(_stgData.seed);
        _stgMain.InitSTG(_stgData.characterIndex);
        // 加载各个stage.lua文件
        List<string> stageLuaList = new List<string> { "stage1", "stage1sc" };
        //List<string> stageLuaList = new List<string> { "TestEditorStage" };
        for (int i = 0; i < stageLuaList.Count; i++)
        {
            InterpreterManager.GetInstance().LoadLuaFile(stageLuaList[i]);
        }
        // 设置初始残机数和符卡数目
        PlayerService.GetInstance().SetLifeCounter(Consts.STGInitLifeCount, 0);
        PlayerService.GetInstance().SetSpellCardCounter(Consts.STGInitSpellCardCount, 0);
        // 打开界面
        UIManager.GetInstance().ShowView(WindowName.GameInfoView, null);
        UIManager.GetInstance().ShowView(WindowName.STGBottomView, null);
        UIManager.GetInstance().ShowView(WindowName.GameMainView);

        UIManager.GetInstance().ShowView(WindowName.STGDialogView);

        _curState = StateLoadStageLua;
    }

    /// <summary>
    /// 初始化随机数种子
    /// </summary>
    /// <returns></returns>
    private long InitSeed()
    {
        long seed = System.DateTime.Now.Ticks % 0xffffffff;
        return seed;
    }

    /// <summary>
    /// 加载Stage.lua
    /// </summary>
    private void OnStateLoadStageLuaUpdate()
    {
        _curState = StateWait;
        if (Global.DebugStageName != "")
        {
            _curStageName = Global.DebugStageName;
        }
        else
        {
            _curStageName = _nextStageName;
        }
        _stgMain.EnterStage(_curStageName);
        Global.IsPause = false;
        if (Global.IsInReplayMode)
        {
            _isReplayFinish = false;
        }
        _curState = StateUpdateSTG;
    }

    /// <summary>
    /// 执行clear
    /// </summary>
    private void OnStateClearUpdate()
    {
        _curState = StateWait;
        _stgMain.Clear(eSTGClearType.RetryCurStage);
        _curState = StateInitSTG;
    }

    /// <summary>
    /// 重新开始游戏的初始化
    /// </summary>
    private void OnStateInitSTGUpdate()
    {
        if (!Global.IsInReplayMode)
        {
            _stgData.seed = InitSeed();
        }
        else
        {
            _stgData = ReplayManager.GetInstance().GetReplaySTGData();
        }
        MTRandom.Init(_stgData.seed);
        _stgMain.InitSTG(_stgData.characterIndex);
        // 设置初始残机数和符卡数目
        PlayerService.GetInstance().SetLifeCounter(Consts.STGInitLifeCount, 0);
        PlayerService.GetInstance().SetSpellCardCounter(Consts.STGInitSpellCardCount, 0);
        _curState = StateLoadStageLua;
    }

    /// <summary>
    /// 更新STG主线程
    /// </summary>
    private void OnSTGMainUpdate()
    {
        // 录像状态中，检测是否进行到了录像的最后一帧
        if (Global.IsInReplayMode)
        {
            if (_isReplayFinish)
                return;
            int curFrame = STGStageManager.GetInstance().GetFrameSinceStageStart();
            if (curFrame >= ReplayManager.GetInstance().GetReplayLastFrame())
            {
                _isReplayFinish = true;
                UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseAfterReplayFinished);
                Global.IsPause = true;
                CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
                return;
            }
        }
        // 检测是否在游戏进行中按下Esc进行暂停
        if (!Global.IsPause && Input.GetKeyDown(KeyCode.Escape))
        {
            if (Global.IsInReplayMode)
            {
                UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseInReplay);
            }
            else
            {
                UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseInGame);
            }
            Global.IsPause = true;
            CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
        }
        if ( !Global.IsPause )
        {
            _stgMain.Update();
        }
    }

    /// <summary>
    /// 通关当前关卡
    /// </summary>
    private void OnStageClear()
    {
        UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseAfterGameClear);
        Global.IsPause = true;
        CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
    }

    #region replay

    /// <summary>
    /// 保存并播放replay
    /// </summary>
    private void OnSaveReplay()
    {
        Logger.Log("Save Replay");
        ReplayManager.GetInstance().SaveReplay(_stgData);
        // 以replay模式重新播放
        Global.IsInReplayMode = true;

        _curState = StateClear;
        _nextStageName = _curStageName;
        // 打开loadingView
        List<object> commandList = new List<object>();
        commandList.Add(CommandConsts.STGLoadStageLuaComplete);
        object[] commandArr = commandList.ToArray();
        UIManager.GetInstance().ShowView(WindowName.GameLoadingView, commandArr);
    }
    #endregion
}