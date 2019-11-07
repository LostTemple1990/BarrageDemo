using System;
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
    /// <summary>
    /// 是否已经疮痍
    /// </summary>
    private bool _isGameOver;
    /// <summary>
    /// 损失所有残机之后的计时
    /// </summary>
    private int _gameOverTimeCounter;

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
                OnStageFinished();
                break;
            case CommandConsts.SaveReplay:
                OnSaveReplay((ReplayInfo)data);
                break;
            case CommandConsts.BackToTitle:
                OnBackToTitle();
                break;
            case CommandConsts.PlayerMiss:
                OnPlayerMiss();
                break;
            case CommandConsts.ContinueGameAfterGameOver:
                OnContinueAfterGameOver();
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
        CommandManager.GetInstance().Register(CommandConsts.BackToTitle, this);
        _stgData = (STGData)data;
        // 设置需要载入的stage
        _nextStageName = _stgData.stageName;
        // 打开界面
        UIManager.GetInstance().ShowView(WindowName.GameInfoView, null);
        UIManager.GetInstance().ShowView(WindowName.STGBottomView, null);
        UIManager.GetInstance().ShowView(WindowName.GameMainView);

        UIManager.GetInstance().ShowView(WindowName.STGDialogView);
        // 实例化STGMain
        if (_stgMain == null )
        {
            _curState = StateInitSTGMain;
        }
        else
        {
            _curState = StateInitSTG;
        }
    }

    public void OnStateExit()
    {
        CommandManager.GetInstance().Remove(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Remove(CommandConsts.ContinueGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.StageClear, this);
        CommandManager.GetInstance().Remove(CommandConsts.SaveReplay, this);
        CommandManager.GetInstance().Remove(CommandConsts.BackToTitle, this);

        CommandManager.GetInstance().Remove(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Remove(CommandConsts.ContinueGameAfterGameOver, this);

        UIManager.GetInstance().HideView(WindowName.GameInfoView);
        UIManager.GetInstance().HideView(WindowName.STGBottomView);
        UIManager.GetInstance().HideView(WindowName.GameMainView);
        UIManager.GetInstance().HideView(WindowName.STGDialogView);
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
        _stgMain = new STGMain();
        _stgMain.Init();
        // 加载各个stage.lua文件
        List<string> stageLuaList = new List<string> { "stage1", "stage1sc" };
        //List<string> stageLuaList = new List<string> { "TestEditorStage" };
        for (int i = 0; i < stageLuaList.Count; i++)
        {
            InterpreterManager.GetInstance().LoadLuaFile(stageLuaList[i]);
        }

        _curState = StateInitSTG;
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
        if (Global.IsInReplayMode)
        {
            _isReplayFinish = false;
            _curStageName = _stgData.stageName;
        }
        else
        {
            if (Global.DebugStageName != "")
            {
                _curStageName = Global.DebugStageName;
            }
            else
            {
                _curStageName = _nextStageName;
            }
        }
        _stgData.stageName = _curStageName;
        _stgMain.EnterStage(_curStageName);
        // 添加事件监听
        CommandManager.GetInstance().Register(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Register(CommandConsts.ContinueGameAfterGameOver, this);
        _isGameOver = false;
        Global.IsPause = false;
        ReplayManager.GetInstance().SetReplayEnable(true);

        _curState = StateUpdateSTG;
    }

    /// <summary>
    /// 执行clear
    /// </summary>
    private void OnStateClearUpdate()
    {
        _stgMain.Clear();
        CommandManager.GetInstance().Remove(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Remove(CommandConsts.ContinueGameAfterGameOver, this);

        _curState = StateInitSTG;
    }

    /// <summary>
    /// 重新开始游戏的初始化
    /// </summary>
    private void OnStateInitSTGUpdate()
    {
        Global.IsInReplayMode = _stgData.isReplay;
        if (!Global.IsInReplayMode)
        {
            _stgData.seed = InitSeed();
        }
        MTRandom.Init(_stgData.seed);
        _stgMain.InitSTG(_stgData.characterIndex);
        // 设置初始残机数和符卡数目
        PlayerInterface.GetInstance().SetLifeCounter(Consts.STGInitLifeCount, 0);
        PlayerInterface.GetInstance().SetSpellCardCounter(Consts.STGInitSpellCardCount, 0);
        _curState = StateLoadStageLua;
    }

    #region STG主线程相关
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
            // 检测是否在游戏进行中按下Esc进行暂停
            if (!Global.IsPause && Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseInReplay);
                Global.IsPause = true;
                CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
            }
        }
        else
        {
            if (_isGameOver)
            {
                if (!Global.IsPause)
                {
                    _gameOverTimeCounter++;
                    if (_gameOverTimeCounter >= 60)
                    {
                        UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseAfterGameOver);
                        Global.IsPause = true;
                        CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
                        return;
                    }
                }
            }
            // 检测是否在游戏进行中按下Esc进行暂停
            if (!Global.IsPause && Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseInGame);
                Global.IsPause = true;
                CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
            }
        }
        if (!Global.IsPause)
        {
            _stgMain.Update();
        }
    }

    /// <summary>
    /// 通关当前关卡
    /// </summary>
    private void OnStageFinished()
    {
        UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseAfterGameClear);
        Global.IsPause = true;
        CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
    }

    /// <summary>
    /// 玩家死亡之后消失
    /// </summary>
    private void OnPlayerMiss()
    {
        _isGameOver = !PlayerInterface.GetInstance().Miss();
        if (_isGameOver)
            _gameOverTimeCounter = 0;
    }

    /// <summary>
    /// 疮痍之后继续游戏
    /// </summary>
    private void OnContinueAfterGameOver()
    {
        ReplayManager.GetInstance().SetReplayEnable(false);
        Global.IsPause = false;
        _isGameOver = false;
        // 设置初始残机数和符卡数目
        PlayerInterface.GetInstance().SetLifeCounter(Consts.STGInitLifeCount, 0);
        PlayerInterface.GetInstance().SetSpellCardCounter(Consts.STGInitSpellCardCount, 0);
    }
    #endregion

    private void OnBackToTitle()
    {
        // 打开loadingView
        List<object> commandList = new List<object>();
        commandList.Add(CommandConsts.OnEnterTitle);
        object[] commandArr = commandList.ToArray();
        UIManager.GetInstance().ShowView(WindowName.GameLoadingView, commandArr);
        _stgMain.Clear();
        _fsm.SetNextStateId((int)eGameState.Title);
    }

    #region replay

    /// <summary>
    /// 保存并播放replay
    /// </summary>
    private void OnSaveReplay(ReplayInfo info)
    {
        Logger.Log("Save Replay");
        ReplayManager.GetInstance().SaveReplay(info.replayIndex, info.name, _stgData);
        CommandManager.GetInstance().RunCommand(CommandConsts.SaveReplaySuccess);
        // 以replay模式重新播放
        //Global.IsInReplayMode = true;

        //_curState = StateClear;
        //_nextStageName = _curStageName;
        //// 打开loadingView
        //List<object> commandList = new List<object>();
        //commandList.Add(CommandConsts.STGLoadStageLuaComplete);
        //object[] commandArr = commandList.ToArray();
        //UIManager.GetInstance().ShowView(WindowName.GameLoadingView, commandArr);
    }
    #endregion
}