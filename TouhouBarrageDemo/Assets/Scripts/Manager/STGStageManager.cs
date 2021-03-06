﻿using UnityEngine;
using System.Collections.Generic;

public class STGStageManager
{
    private const int StateGetStageTask = 1;
    private const int StateUpdateStageTask = 2;
    private const int StateUpdateSpellCard = 3;

    /// <summary>
    /// ctrl跳过剧情部分的时间间隔
    /// </summary>
    private const int SkipDialogTimeInterval = 10;

    private static STGStageManager _instance = new STGStageManager();

    public static STGStageManager GetInstance()
    {
        return _instance;
    }

    private int _state;

    private Task _curStageTask;
    private SpellCard _curSpellCard;
    /// <summary>
    /// 是否正在符卡阶段
    /// </summary>
    private bool _isCastingSpellCard;
    /// <summary>
    /// 是否等待符卡结束
    /// </summary>
    private bool _isWaitingForSpellCard;
    /// <summary>
    /// 从关卡开始经过的帧数
    /// </summary>
    private int _frameSinceStageStart;
    /// <summary>
    /// 当前是否允许射击
    /// <para>优先级最高</para>
    /// <para>优先判断该值，若为false，则不允许射击</para>
    /// </summary>
    private bool _isEnableToShoot;
    /// <summary>
    /// 是否在剧情模式
    /// </summary>
    private bool _isInDialogMode;
    /// <summary>
    /// 剧情模式的task
    /// </summary>
    private Task _dialogTask;
    /// <summary>
    /// 上次跳过剧情对话的时间间隔
    /// </summary>
    private int _timeSinceLastSkip;
    /// <summary>
    /// 关卡信息
    /// </summary>
    private Dictionary<string, StageInfo> _stageInfos;
    /// <summary>
    /// 当前关卡信息
    /// </summary>
    private StageInfo _curStageInfo;

    public STGStageManager()
    {
        _curSpellCard = new SpellCard();
        _stageInfos = new Dictionary<string, StageInfo>();
    }

    public void Init()
    {
        _state = StateGetStageTask;
    }

    public void Update()
    {
        if ( _state == StateUpdateStageTask)
        {
            _frameSinceStageStart++;
            //Logger.Log("--------------STG Frame " + _frameSinceStageStart + " finish------------");
            if (_isInDialogMode)
            {
                UpdateDialogTask();
                if (_isInDialogMode)
                {
                    return;
                }
            }
            CommandManager.GetInstance().RunCommand(CommandConsts.UpdateDialog);
            if ( !_isWaitingForSpellCard )
            {
                OnStageTaskUpdate();
            }
            if (_isCastingSpellCard)
            {
                if ( !_curSpellCard.IsComplete() )
                {
                    _curSpellCard.Update();
                }
                else
                {
                    _curSpellCard.OnFinish();
                    _isCastingSpellCard = false;
                    _isWaitingForSpellCard = false;
                }
            }
        }
    }

    /// <summary>
    /// 加载关卡
    /// </summary>
    /// <param name="stageId"></param>
    public void LoadStage(string stageName)
    {
        StageInfo info;
        if (_stageInfos.TryGetValue(stageName,out info))
        {
            _curStageInfo = info;
            _curStageTask = InterpreterManager.GetInstance().CreateStageTask(info);
            _state = StateUpdateStageTask;
            _frameSinceStageStart = 0;
            _isEnableToShoot = true;
            _isInDialogMode = false;
            // 预先加载BGM
            if (info.bgm == "")
                info.bgm = "bgm";
            SoundManager.GetInstance().PauseAllSTGSound();
            SoundManager.GetInstance().Play(info.bgm, 0.5f, false, true);
        }
        else
        {
            Logger.LogError("Stage info of " + stageName + " is not exist!");
        }
        CommandManager.GetInstance().RunCommand(CommandConsts.STGLoadStageLuaComplete);
    }

    /// <summary>
    /// 符卡开始
    /// </summary>
    /// <param name="initFuncRef"></param>
    /// <param name="bossList"></param>
    public void StartSpellCard(int initFuncRef, int finishFuncRef, List<Boss> bossList)
    {
        _curSpellCard.SetTask(initFuncRef, bossList);
        _curSpellCard.SetFinishFuncRef(finishFuncRef);
        _isCastingSpellCard = true;
    }

    /// <summary>
    /// 设置符卡结束的清除相关函数
    /// </summary>
    /// <param name="funcRef"></param>
    public void SetSpellCardFinishFunc(int funcRef)
    {
        _curSpellCard.SetFinishFuncRef(funcRef);
    }

    /// <summary>
    /// 获取当前符卡
    /// </summary>
    /// <returns></returns>
    public SpellCard GetSpellCard()
    {
        return _curSpellCard;
    }

    /// <summary>
    /// 设置关卡task等待符卡结束再执行
    /// </summary>
    /// <param name="value"></param>
    public void SetStageTaskWaitingForSpellCardFinish(bool value)
    {
        _isWaitingForSpellCard = value;
    }

    /// <summary>
    /// 执行关卡task
    /// </summary>
    private void OnStageTaskUpdate()
    {
        if (_curStageTask.isFinish != true)
        {
            if (!_curStageTask.isStarted)
            {
                // 设置fixedFPS
                if (_curStageInfo.fixedFps)
                {
                    Application.targetFrameRate = 90;
                }
                else
                {
                    Application.targetFrameRate = 60;
                }
                FPSController.GetInstance().Restart(_curStageInfo.fixedFps);
                // bgm开始播放
                //SoundManager.GetInstance().SetSoundPlayTime(_curStageInfo.bgm, 0);
                SoundManager.GetInstance().ResumeAllSTGSound();
                //SoundManager.GetInstance().PrintSoundTime("DarkRoad");
                InterpreterManager.GetInstance().AddPara(ExtraTaskManager.GetInstance(), LuaParaType.LightUserData);
                InterpreterManager.GetInstance().CallTaskCoroutine(_curStageTask, 1);
            }
            else
            {
                InterpreterManager.GetInstance().CallTaskCoroutine(_curStageTask);
            }
        }
    }

    /// <summary>
    /// 获取关卡开始之后经过的帧数
    /// </summary>
    /// <returns></returns>
    public int GetFrameSinceStageStart()
    {
        return _frameSinceStageStart;
    }

    /// <summary>
    /// 设置当前是否允许射击
    /// </summary>
    /// <param name="vale"></param>
    public void SetIsEnableToShoot(bool value)
    {
        _isEnableToShoot = value;
    }

    /// <summary>
    /// 当前是否允许射击
    /// </summary>
    /// <returns></returns>
    public bool GetIsEnableToShoot()
    {
        if (_isInDialogMode)
            return false;
        return _isEnableToShoot;
    }

    /// <summary>
    /// 创建对话
    /// </summary>
    public void StartDialog(int funcRef)
    {
        _isInDialogMode = true;
        _timeSinceLastSkip = SkipDialogTimeInterval;
        _dialogTask = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        _dialogTask.funcRef = funcRef;
        InterpreterManager.GetInstance().CallTaskCoroutine(_dialogTask);
        CommandManager.GetInstance().RunCommand(CommandConsts.StartDialog);
    }

    public void UpdateDialogTask()
    {
        int dTime = 1;
        _timeSinceLastSkip++;
        if (!_dialogTask.isFinish)
        {
            if (CheckCanSkip())
            {
                dTime = _dialogTask.totalWaitTime - _dialogTask.curWaitTime;
                _dialogTask.curWaitTime = _dialogTask.totalWaitTime;
                _timeSinceLastSkip = 0;
            }
            CommandManager.GetInstance().RunCommand(CommandConsts.UpdateDialog,dTime);
            InterpreterManager.GetInstance().CallTaskCoroutine(_dialogTask);
        }
        if (_dialogTask.isFinish)
        {
            _isInDialogMode = false;
            ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(_dialogTask);
            _dialogTask = null;
        }
    }

    private bool CheckCanSkip()
    {
        if (OperationController.GetInstance().GetKeyDown(eSTGKey.KeyZ))
            return true;
        if (OperationController.GetInstance().GetKey(eSTGKey.KeyCtrl))
        {
            if (_timeSinceLastSkip >= SkipDialogTimeInterval)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 结束对话
    /// </summary>
    public void FinishDialog()
    {
        CommandManager.GetInstance().RunCommand(CommandConsts.FinishDialog);
    }

    /// <summary>
    /// 创建对话CG
    /// </summary>
    /// <param name="name"></param>
    /// <param name="spName"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    public void CreateDialogCG(string name,string spName,float posX,float posY)
    {
        List<object> datas = new List<object>();
        datas.Add(name);
        datas.Add(spName);
        datas.Add(posX);
        datas.Add(posY);
        CommandManager.GetInstance().RunCommand(CommandConsts.CreateDialogCG, datas);
    }

    /// <summary>
    /// 高亮角色CG
    /// </summary>
    /// <param name="name"></param>
    /// <param name="highlight"></param>
    public void HighlightDialogCG(string name,bool highlight)
    {
        List<object> datas = new List<object>();
        datas.Add(name);
        datas.Add(highlight);
        CommandManager.GetInstance().RunCommand(CommandConsts.HighlightDialogCG, datas);
    }

    public void DelDialogCG(string name)
    {
        CommandManager.GetInstance().RunCommand(CommandConsts.FadeOutDialogCG, name);
    }

    /// <summary>
    /// 创建对话框
    /// </summary>
    /// <param name="style">对话框样式</param>
    /// <param name="text">文本</param>
    /// <param name="posX">对话框左上角位置(scale为负数时是右上角的位置)</param>
    /// <param name="posY">对话框左上角位置(scale为负数时是右上角的位置)</param>
    /// <param name="duration">持续时间</param>
    /// <param name="scale">缩放</param>
    public void CreateDialogBox(int style,string text,float posX,float posY,int duration,float scale)
    {
        List<object> datas = new List<object>();
        datas.Add(style);
        datas.Add(text);
        datas.Add(posX);
        datas.Add(posY);
        datas.Add(duration);
        datas.Add(scale);
        CommandManager.GetInstance().RunCommand(CommandConsts.CreateDialogBox, datas);
    }

    /// <summary>
    /// 添加stage信息
    /// </summary>
    /// <param name="info"></param>
    public void AddStageInfo(StageInfo info)
    {
        if (!_stageInfos.ContainsKey(info.stageName))
        {
            _stageInfos.Add(info.stageName, info);
        }
    }

    /// <summary>
    /// 获取当前关卡信息
    /// </summary>
    /// <returns></returns>
    public StageInfo GetCurStageInfo()
    {
        return _curStageInfo;
    }

    public void Clear()
    {
        _curSpellCard.Clear();
        _isCastingSpellCard = false;
        _isWaitingForSpellCard = false;
        if (_isInDialogMode)
        {
            InterpreterManager.GetInstance().StopTaskThread(_dialogTask);
            ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(_dialogTask);
            _dialogTask = null;
            _isInDialogMode = false;
        }
        InterpreterManager.GetInstance().StopTaskThread(_curStageTask);
        Application.targetFrameRate = 60;
    }
}