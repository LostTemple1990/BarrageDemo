using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCard : ICommand
{
    /// <summary>
    /// 符卡名称
    /// </summary>
    private string _scName;
    /// <summary>
    /// 符卡时间，单位：秒
    /// </summary>
    private float _scDuration;
    /// <summary>
    /// 符卡总帧数
    /// </summary>
    private int _frameTotal;
    /// <summary>
    /// 符卡剩余帧数
    /// </summary>
    private int _frameLeft;
    private int _finishFuncRef;
    private Task _scTask;
    private List<Boss> _bossList;
    /// <summary>
    /// 符卡对应的boss数量
    /// </summary>
    private int _bossCount;
    /// <summary>
    /// 符卡完成条件
    /// </summary>
    private eSpellCardCondition _condition;
    /// <summary>
    /// 符卡是否已经开始
    /// </summary>
    private bool _isSCStarted;
    /// <summary>
    /// 符卡的task是否已经开始
    /// </summary>
    private bool _isSCTaskStarted;
    /// <summary>
    /// 是否符卡，false表示非符，不显示符卡信息
    /// </summary>
    private bool _isSpellCard;
    /// <summary>
    /// 符卡期间自机miss次数
    /// </summary>
    private int _missCount;
    /// <summary>
    /// 符卡期间自机放B的次数
    /// </summary>
    private int _castSCCount;
    /// <summary>
    /// 符卡期间自机进入决死状态的次数
    /// </summary>
    private int _dyingCount;
    /// <summary>
    /// 是否击破符卡
    /// </summary>
    private bool _isFinishSpellCard;
    /// <summary>
    /// 符卡起始时间戳
    /// </summary>
    private long _scStartTimeTick;

    public SpellCard()
    {
        _finishFuncRef = 0;
    }

    /// <summary>
    /// 设置符卡的基本属性
    /// </summary>
    /// <param name="name"></param>
    /// <param name="duration"></param>
    /// <param name="condition"></param>
    /// <param name="isSpellCard">是否符卡，符卡的话需要显示信息</param>
    public void SetProperties(string name,float duration, eSpellCardCondition condition,bool isSpellCard)
    {
        _scName = name;
        // 时间
        _scDuration = duration;
        _frameTotal = (int)(duration * 60);
        _frameLeft = _frameTotal;
        // 显示符卡时间
        object[] scTimeDatas = { _frameLeft };
        CommandManager.GetInstance().RunCommand(CommandConsts.NewSpellCardTime, scTimeDatas);
        // 完成条件
        _condition = condition;
        // 是否显示符卡信息
        _isSpellCard = isSpellCard;
        if ( _isSpellCard )
        {
            object[] scDatas = { _scName };
            CommandManager.GetInstance().RunCommand(CommandConsts.ShowSpellCardInfo, scDatas);
        }
        _isSCStarted = true;
        // 初始化记录符卡状态的一些变量
        _missCount = 0;
        _castSCCount = 0;
        _dyingCount = 0;
        _isFinishSpellCard = false;
        _scStartTimeTick = TimeUtil.GetTimestamp();
        CommandManager.GetInstance().Register(CommandConsts.PlayerDying, this);
        CommandManager.GetInstance().Register(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Register(CommandConsts.PlayerCastSC, this);
        Logger.Log("Start SpellCard " + _scName);
    }

    public void SetFinishFuncRef(int funcRef)
    {
        _finishFuncRef = funcRef;
    }

    /// <summary>
    /// 设置符卡的task
    /// </summary>
    /// <param name="funcRef"></param>
    /// <param name="bossList"></param>
    public void SetTask(int funcRef, List<Boss> bossList)
    {
        this._bossList = bossList;
        _bossCount = bossList.Count;
        _scTask = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        _scTask.funcRef = funcRef;
        // 初始化一些属性
        _isSCStarted = false;
        _isSCTaskStarted = false;
        _finishFuncRef = 0;
    }

    /// <summary>
    /// 判断符卡是否已经完成
    /// </summary>
    /// <returns></returns>
    public bool IsComplete()
    {
        if (!_isSCStarted) return false;
        if (_frameLeft <= 0)
        {
            // 优先判断时符
            if (_condition == eSpellCardCondition.TimeOver)
            {
                Logger.Log("TimeSpellCard TimeOver");
                _isFinishSpellCard = true;
                return true;
            }
            Logger.Log("SpellCard Over Time!");
            return true;
        }
        if ( _condition == eSpellCardCondition.EliminateAll )
        {
            Boss boss;
            for (int i=0;i<_bossCount;i++)
            {
                boss = _bossList[i];
                if ( boss.GetCurHp() > 0 )
                {
                    return false;
                }
            }
            Logger.Log("SpellCard EliminateAll");
            _isFinishSpellCard = true;
            return true;
        }
        else if ( _condition == eSpellCardCondition.EliminateOne )
        {
            Boss boss;
            for (int i = 0; i < _bossCount; i++)
            {
                boss = _bossList[i];
                if (boss.GetCurHp() <= 0 )
                {
                    Logger.Log("SpellCard EliminateOne");
                    _isFinishSpellCard = true;
                    return true;
                }
            }
            return false;
        }
        else if ( _condition == eSpellCardCondition.EliminateSpecificOne )
        {
            Boss boss = _bossList[0];
            if ( boss.GetCurHp() > 0 )
            {
                return false;
            }
            Logger.Log("SpellCard EliminateSpecificOne");
            _isFinishSpellCard = true;
            return true;
        }
        return false;
    }

    public void Update()
    {
        // 符卡尚未开始
        if ( !_isSCTaskStarted )
        {
            for (int i=0;i<_bossCount;i++)
            {
                InterpreterManager.GetInstance().AddPara(_bossList[i], LuaParaType.LightUserData);
            }
            InterpreterManager.GetInstance().CallTaskCoroutine(_scTask, _bossCount);
            _isSCTaskStarted = true;
            Logger.Log("Call SpellCard Task");
        }
        // update
        else
        {
            _frameLeft--;
            object[] data = { _frameLeft };
            CommandManager.GetInstance().RunCommand(CommandConsts.UpdateSpellCardTime, data);
            if (_scTask != null && !_scTask.isFinish)
            {
                InterpreterManager.GetInstance().CallTaskCoroutine(_scTask);
                if (_scTask.isFinish)
                {
                    Logger.Log("SpellCard " + _scName + " task finish!");
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(_scTask);
                    _scTask = null;
                }
            }
        }
    }

    public void OnFinish()
    {
        // 移除符卡事件监听
        CommandManager.GetInstance().Remove(CommandConsts.PlayerDying, this);
        CommandManager.GetInstance().Remove(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Remove(CommandConsts.PlayerCastSC, this);

        // 通过符卡经过的时间
        int framePassed = _frameTotal - _frameLeft;
        int timePassed = framePassed * 1000 / Consts.TargetFrameRate;
        // 是否获得符卡奖励
        bool getBonus = true;
        if (!_isFinishSpellCard)
            getBonus = false;
        else if (_dyingCount != 0 || _castSCCount != 0)
            getBonus = false;
        // 计算通过符卡实际经过的时间
        long scFinishTimeTick = TimeUtil.GetTimestamp();
        int realTimePassed = (int)((scFinishTimeTick - _scStartTimeTick) / 10000);
        object[] datas = { _isSpellCard, _isFinishSpellCard, getBonus, timePassed, realTimePassed };
        CommandManager.GetInstance().RunCommand(CommandConsts.SpellCardFinish, datas);

        if (_finishFuncRef != 0)
        {
            InterpreterManager.GetInstance().AddPara(_bossList[0], LuaParaType.LightUserData);
            InterpreterManager.GetInstance().AddPara(_isFinishSpellCard, LuaParaType.Bool);
            InterpreterManager.GetInstance().AddPara(getBonus, LuaParaType.Bool);
            InterpreterManager.GetInstance().CallLuaFunction(_finishFuncRef, 3);
        }
        // 清除BossTask
        for (int i = 0; i < _bossCount; i++)
        {
            _bossList[i].OnSpellCardFinish();
        }
        ColliderManager.GetInstance().ClearAllObjectCollider(new List<eEliminateDef> { eEliminateDef.PlayerSpellCard });
        CreateEliminateEnemyCollider();
    }

    /// <summary>
    /// 创建一个销毁敌机子弹以及敌机的ObjectCollider
    /// </summary>
    private void CreateEliminateEnemyCollider()
    {
        // 清除子弹用的collider
        ColliderCircle collider = ColliderManager.GetInstance().CreateColliderByType(eColliderType.Circle) as ColliderCircle;
        collider.SetPosition(0, 120);
        collider.SetSize(0, 0);
        collider.SetEliminateType(eEliminateDef.CodeEliminate);
        collider.SetColliderGroup(eColliderGroup.EnemyBullet | eColliderGroup.Enemy);
        collider.ScaleToSize(400, 400, 30);
        collider.SetExistDuration(30);
    }

    /// <summary>
    /// 获取符卡剩余时间的比例
    /// </summary>
    /// <returns></returns>
    public float GetTimeRate()
    {
        return (float)_frameLeft / _frameTotal;
    }

    /// <summary>
    /// 根据索引获得对应的boss
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Boss GetBossByIndex(int index)
    {
        index = Mathf.Clamp(index, 0, _bossCount - 1);
        return _bossList[index];
    }

    public void Execute(int cmd, object data)
    {
        switch ( cmd )
        {
            case CommandConsts.PlayerDying:
                _dyingCount++;
                break;
            case CommandConsts.PlayerMiss:
                _missCount++;
                break;
            case CommandConsts.PlayerCastSC:
                _castSCCount++;
                break;
        }
    }

    public void Clear()
    {
        CommandManager.GetInstance().Remove(CommandConsts.PlayerDying, this);
        CommandManager.GetInstance().Remove(CommandConsts.PlayerMiss, this);
        CommandManager.GetInstance().Remove(CommandConsts.PlayerCastSC, this);
        _bossList = null;
        if ( _scTask != null && !_scTask.isFinish )
        {
            InterpreterManager.GetInstance().StopTaskThread(_scTask);
        }
        _scTask = null;
    }
}

public enum eSpellCardCondition : int
{
    /// <summary>
    /// 消灭全部
    /// </summary>
    EliminateAll = 1,
    /// <summary>
    /// 消灭其中一个
    /// </summary>
    EliminateOne = 2,
    /// <summary>
    /// 消灭指定的一个，暂时指第一个
    /// </summary>
    EliminateSpecificOne = 3,
    /// <summary>
    /// 时符，时间结束
    /// </summary>
    TimeOver = 5,
}
