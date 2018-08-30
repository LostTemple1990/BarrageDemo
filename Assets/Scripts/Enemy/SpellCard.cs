using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellCard
{
    public int type;
    public string name;
    public float curHp;
    public float maxHp;
    /// <summary>
    /// 符卡时间，单位：秒
    /// </summary>
    public float spellDuration;
    public float spellTime;
    private int _frameTotal;
    /// <summary>
    /// 符卡剩余帧数
    /// </summary>
    private int _frameLeft;
    public int taskRef;
    public float invincibleDuration;
    public bool isFinish;
    public int finishFuncRef;
    public Task scTask;
    public List<Boss> bossList;
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
    private bool _isStarted;

    public SpellCard()
    {
        finishFuncRef = 0;
    }

    /// <summary>
    /// 设置符卡的基本属性
    /// </summary>
    /// <param name="name"></param>
    /// <param name="funcRef"></param>
    /// <param name="bossList"></param>
    public void SetProperties(string name,float duration, eSpellCardCondition condition)
    {
        this.name = name;
        // 时间
        spellDuration = duration;
        _frameTotal = (int)(duration * 60);
        _frameLeft = _frameTotal;
        // 显示符卡时间
        object[] data = { _frameLeft };
        CommandManager.GetInstance().RunCommand(CommandConsts.NewSpellCardTime, data);
        // 完成条件
        _condition = condition;
    }

    public void SetFinishFuncRef(int funcRef)
    {
        finishFuncRef = funcRef;
    }

    /// <summary>
    /// 设置符卡的task
    /// </summary>
    /// <param name="funcRef"></param>
    /// <param name="bossList"></param>
    public void SetTask(int funcRef, List<Boss> bossList)
    {
        this.bossList = bossList;
        _bossCount = bossList.Count;
        scTask = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        scTask.funcRef = funcRef;
        // 初始化一些属性
        _isStarted = false;
        finishFuncRef = 0;
    }

    /// <summary>
    /// 判断符卡是否已经完成
    /// </summary>
    /// <returns></returns>
    public bool IsComplete()
    {
        if (!_isStarted) return false;
        if ( _condition == eSpellCardCondition.EliminateAll )
        {
            Boss boss;
            for (int i=0;i<_bossCount;i++)
            {
                boss = bossList[i];
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
                boss = bossList[i];
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
            Boss boss = bossList[0];
            if ( boss.GetCurHp() > 0 )
            {
                return false;
            }
            Logger.Log("SpellCard EliminateSpecificOne");
            return true;
        }
        else if ( _condition == eSpellCardCondition.TimeOver )
        {
            if ( _frameLeft <= 0 )
            {
                Logger.Log("SpellCard TimeOver");
                return true;
            }
            return false;
        }
        return false;
    }

    public void Update()
    {
        // 符卡尚未开始
        if ( !_isStarted )
        {
            for (int i=0;i<_bossCount;i++)
            {
                InterpreterManager.GetInstance().AddPara(bossList[i], LuaParaType.LightUserData);
            }
            InterpreterManager.GetInstance().CallTaskCoroutine(scTask, _bossCount);
            _isStarted = true;
        }
        // update
        else
        {
            _frameLeft--;
            object[] data = { _frameLeft };
            CommandManager.GetInstance().RunCommand(CommandConsts.UpdateSpellCardTime, data);
            if (scTask != null && !scTask.isFinish)
            {
                InterpreterManager.GetInstance().CallTaskCoroutine(scTask);
                if (scTask.isFinish)
                {
                    Logger.Log("SpellCard " + name + " task finish!");
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(scTask);
                    scTask = null;
                }
            }
        }
    }

    public void OnFinish()
    {
        if ( finishFuncRef != 0 )
        {
            InterpreterManager.GetInstance().CallLuaFunction(finishFuncRef, 0);
        }
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
        return bossList[index];
    }

    public void Clear()
    {
        bossList = null;
        if ( scTask != null && !scTask.isFinish )
        {
            InterpreterManager.GetInstance().StopTaskThread(scTask);
        }
        scTask = null;
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
