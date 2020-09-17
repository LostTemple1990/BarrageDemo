#define DEBUG_GAMESTATE
#define LOG_RANDOMSEED

using System;
using UnityEngine;
using System.Collections.Generic;

public class StateSTGMain : IState,ICommand
{
    enum eSTGMainState : byte
    {
        /// <summary>
        /// 等待
        /// </summary>
        StateWait = 1,
        /// <summary>
        /// 初始化STGMain
        /// </summary>
        StateInitSTGMain = 2,
        /// <summary>
        /// 创建对应Stage的Task
        /// </summary>
        StateCreateStageTask = 3,
        /// <summary>
        /// 执行clear
        /// </summary>
        StateClear = 4,
        /// <summary>
        ///  根据STGData来初始化一些STG基本数据，包括随机数种子
        /// </summary>
        StateInitSTGData = 5,
        /// <summary>
        /// update STG
        /// </summary>
        StateUpdateSTG = 6,
        /// <summary>
        /// 加载关卡初始背景的场景
        /// </summary>
        StateLoadStageDefaultBg = 7,
        /// <summary>
        /// 初始化STG，包括创建人物、初始化人物控制器，初始残机等等信息
        /// </summary>
        StateInitSTG = 8,
    }

    private string _curStageName;
    private string _nextStageName;
    /// <summary>
    /// 当前状态
    /// </summary>
    private eSTGMainState _curState;
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
    /// <summary>
    /// 是否恢复STG音效
    /// </summary>
    private bool _resumeSTGSE;

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
            case CommandConsts.STGLoadStageDefaultBgComplete:
                OnLoadStageDefaultBgComplete();
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
        // 打开loadingView
        List<object> commandList = new List<object>();
        commandList.Add(CommandConsts.STGInitComplete);
        commandList.Add(CommandConsts.STGLoadStageLuaComplete);
        commandList.Add(CommandConsts.STGLoadStageDefaultBgComplete);
        object[] commandArr = commandList.ToArray();
        UIManager.GetInstance().ShowView(WindowName.GameLoadingView, commandArr);
        // STGCamera启用
        UIManager.GetInstance().GetSTGCamera().cullingMask = (1 << Consts.STGLayerIndex);
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
            _curState = eSTGMainState.StateInitSTGMain;
        }
        else
        {
            _curState = eSTGMainState.StateInitSTGData;
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

        CommandManager.GetInstance().Remove(CommandConsts.STGLoadStageLuaComplete, this);
        CommandManager.GetInstance().Remove(CommandConsts.STGLoadStageDefaultBgComplete, this);

        UIManager.GetInstance().HideView(WindowName.GameInfoView);
        UIManager.GetInstance().HideView(WindowName.STGBottomView);
        UIManager.GetInstance().HideView(WindowName.GameMainView);
        UIManager.GetInstance().HideView(WindowName.STGDialogView);

        UIManager.GetInstance().GetSTGCamera().cullingMask = 0;
    }

    public void OnUpdate()
    {
        if (_curState == eSTGMainState.StateInitSTGMain)
        {
            OnStateInitSTGMainUpdate();
        }
        else if (_curState == eSTGMainState.StateInitSTGData)
        {
            OnStateInitSTGDataUpdate();
        }
        else if (_curState == eSTGMainState.StateCreateStageTask)
        {
            OnStateLoadStageLuaUpdate();
        }
        else if (_curState == eSTGMainState.StateLoadStageDefaultBg)
        {
            OnStateLoadStageDefaultBgUpdate();
        }
        else if (_curState == eSTGMainState.StateUpdateSTG)
        {
            OnSTGMainUpdate();
        }
        else if (_curState == eSTGMainState.StateClear)
        {
            OnStateClearUpdate();
        }
        else if (_curState == eSTGMainState.StateInitSTG)
        {
            OnStateInitSTGUpdate();
        }
    }

    /// <summary>
    /// STG初始化完成回调
    /// <para>执行加载stage.lua</para>
    /// </summary>
    private void OnSTGInitComplete()
    {
#if DEBUG_GAMESTATE
        Logger.Log("Starting STG...");
#endif
        _curState = eSTGMainState.StateUpdateSTG;
    }

    /// <summary>
    /// 加载stage.lua完成回调
    /// <para>执行下一个状态-->updateSTG</para>
    /// </summary>
    private void OnLoadStageLuaComplete()
    {
        CommandManager.GetInstance().Remove(CommandConsts.STGLoadStageLuaComplete, this);
        _curState = eSTGMainState.StateLoadStageDefaultBg;
    }

    private void OnLoadStageDefaultBgComplete()
    {
        CommandManager.GetInstance().Remove(CommandConsts.STGLoadStageDefaultBgComplete, this);
        _curState = eSTGMainState.StateInitSTG;
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    private void OnRetryGame()
    {
        Logger.Log("Retry Game");
        _curState = eSTGMainState.StateClear;
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
        _resumeSTGSE = true;
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
        //List<string> stageLuaList = new List<string> { "stage1", "stage1sc" };
        List<string> stageLuaList = new List<string> { "TestEditorStage" };
#if DEBUG_GAMESTATE
        string fileListStr = "";
        for (int k = 0; k < stageLuaList.Count; k++)
        {
            fileListStr = k == 0 ? stageLuaList[k] + ".lua" : "\n" + stageLuaList[k] + ".lua";
        }
        Logger.Log(string.Format("Init STGMain,Start to load lua files\n{0}", fileListStr));
        TimeUtil.BeginSample("InitSTGMain");
#endif
        for (int i = 0; i < stageLuaList.Count; i++)
        {
            InterpreterManager.GetInstance().LoadLuaFile(stageLuaList[i]);
        }
#if DEBUG_GAMESTATE
        TimeUtil.EndSample("InitSTGMain");
        TimeUtil.LogSampleTick("InitSTGMain", "Load luafile(s) complete.Cost time {0}");
#endif
        _curState = eSTGMainState.StateInitSTGData;
    }

    /// <summary>
    /// 初始化随机数种子
    /// </summary>
    /// <returns></returns>
    private long InitSeed()
    {
        //long seed = System.DateTime.Now.Ticks % 0xffffffff;
        long seed = 1281328940L;
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
#if DEBUG_GAMESTATE
        Logger.Log(string.Format("Create task of stage \"{0}\"", _stgData.stageName));
#endif
        _curState = eSTGMainState.StateWait;
        CommandManager.GetInstance().Register(CommandConsts.STGLoadStageLuaComplete, this);

        STGStageManager.GetInstance().LoadStage(_stgData.stageName);

        //_stgMain.EnterStage(_curStageName);
    }

    /// <summary>
    /// 执行clear
    /// </summary>
    private void OnStateClearUpdate()
    {
#if DEBUG_GAMESTATE
        Logger.Log("Clear STG");
#endif
        _stgMain.Clear();
        CommandManager.GetInstance().Remove(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Remove(CommandConsts.ContinueGameAfterGameOver, this);

        _curState = eSTGMainState.StateInitSTGData;
    }

    /// <summary>
    /// 重新开始游戏的初始化
    /// </summary>
    private void OnStateInitSTGDataUpdate()
    {
        Global.IsInReplayMode = _stgData.isReplay;
        if (!Global.IsInReplayMode)
        {
            _stgData.seed = InitSeed();
        }
        MTRandom.Init(_stgData.seed);
#if DEBUG_GAMESTATE
        string modeStr = Global.IsInReplayMode ? "Replay" : "Play";
#if LOG_RANDOMSEED
        Logger.Log(string.Format("Init STGData,seed = {0}\nGameMode = {1}", _stgData.seed, modeStr));
#else
        Logger.Log(string.Format("Init STGData\nGameMode = {1}", _stgData.seed, modeStr));
#endif
#endif
        _curState = eSTGMainState.StateCreateStageTask;
    }

    /// <summary>
    /// 加载关卡的默认背景场景
    /// </summary>
    private void OnStateLoadStageDefaultBgUpdate()
    {
#if DEBUG_GAMESTATE
        Logger.Log("Start loading bg of stage " + _curStageName);
#endif
        _curState = eSTGMainState.StateWait;
        CommandManager.GetInstance().Register(CommandConsts.STGLoadStageDefaultBgComplete, this);
        BackgroundManager.GetInstance().LoadStageDefaultBg(_curStageName);
    }

    private void OnStateInitSTGUpdate()
    {
        _curState = eSTGMainState.StateWait;
#if DEBUG_GAMESTATE
        Logger.Log("Init STG ");
#endif
        CommandManager.GetInstance().Register(CommandConsts.STGInitComplete, this);
        // 添加事件监听
        CommandManager.GetInstance().Register(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Register(CommandConsts.ContinueGameAfterGameOver, this);
        // 各种参数初始化
        _isGameOver = false;
        Global.IsPause = false;
        _resumeSTGSE = false;
        ReplayManager.GetInstance().SetReplayEnable(true);

        _stgMain.InitSTG(_stgData.characterIndex);
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
                PauseGame(ePauseViewState.PauseAfterReplayFinished);
                return;
            }
            // 检测是否在游戏进行中按下Esc进行暂停
            if (!Global.IsPause && Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame(ePauseViewState.PauseInReplay);
            }
            else if (!Global.IsPause && !Global.IsApplicationFocus)
            {
                PauseGame(ePauseViewState.PauseInReplay);
            }
        }
        else
        {
            if (_isGameOver)
            {
                if (!Global.IsPause)
                {
                    _gameOverTimeCounter++;
                    if (_gameOverTimeCounter >= 30)
                    {
                        PauseGame(ePauseViewState.PauseAfterGameOver);
                        return;
                    }
                }
            }
            // 检测是否在游戏进行中按下Esc进行暂停
            if (!Global.IsPause && Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame(ePauseViewState.PauseInGame);
            }
            else if (!Global.IsPause && !Global.IsApplicationFocus)
            {
                PauseGame(ePauseViewState.PauseInGame);
            }
        }
        if (_resumeSTGSE)
        {
            _resumeSTGSE = false;
            SoundManager.GetInstance().ResumeAllSTGSound();
        }
        if (!Global.IsPause)
        {
            _stgMain.Update();
        }
    }

    /// <summary>
    /// 暂停游戏
    /// <para>pauseState 暂停界面的状态</para>
    /// </summary>
    /// <param name="pauseState"></param>
    private void PauseGame(ePauseViewState pauseState)
    {
        UIManager.GetInstance().ShowView(WindowName.STGPauseView, pauseState);
        Global.IsPause = true;
        SoundManager.GetInstance().PauseAllSTGSound();
        CommandManager.GetInstance().RunCommand(CommandConsts.PauseGame);
    }

    /// <summary>
    /// 通关当前关卡
    /// </summary>
    private void OnStageFinished()
    {
        UIManager.GetInstance().ShowView(WindowName.STGPauseView, ePauseViewState.PauseAfterGameClear);
        Global.IsPause = true;
        SoundManager.GetInstance().PauseAllSTGSound();
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
        _resumeSTGSE = true;
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