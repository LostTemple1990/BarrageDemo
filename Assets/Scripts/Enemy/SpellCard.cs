using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCard
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
        if ( _finishFuncRef != 0 )
        {
            InterpreterManager.GetInstance().CallLuaFunction(_finishFuncRef, 0);
        }
        // 清除BossTask
        for (int i=0;i<_bossCount;i++)
        {
            _bossList[i].OnSpellCardFinish();
        }
        ColliderManager.GetInstance().ClearAllObjectCollider(new List<eEliminateDef>{ eEliminateDef.PlayerSpellCard });
        CreateEliminateEnemyCollider();
        //BulletsManager.GetInstance().ClearAllEnemyBullets();
        //EnemyManager.GetInstance().RawEliminateAllEnemyByCode(false);
    }

    /// <summary>
    /// 创建一个销毁敌机子弹以及敌机的ObjectCollider
    /// </summary>
    private void CreateEliminateEnemyCollider()
    {
        // 清除子弹用的collider
        ColliderCircle collider = ColliderManager.GetInstance().CreateColliderByType(eColliderType.Circle) as ColliderCircle;
        collider.SetToPosition(0, 120);
        collider.SetSize(0, 0);
        collider.SetEliminateType(eEliminateDef.CodeEliminate);
        collider.SetColliderGroup(eColliderGroup.Enemy | eColliderGroup.EnemyBullet);
        collider.SetHitEnemyDamage(9999);
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

    public void Clear()
    {
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
