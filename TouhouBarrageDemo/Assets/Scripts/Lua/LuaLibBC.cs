﻿using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 添加子弹组件
    /// <para>type enum BulletComponentType</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int AddBulletComponent(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        BulletComponentType type = (BulletComponentType)luaState.ToInteger(-1);
        if ( type == BulletComponentType.ParasChange )
        {
            bullet.AddOrGetComponent<BCParasChange>();
        }
        else if ( type == BulletComponentType.Rebound )
        {
            bullet.AddOrGetComponent<BCRebound>();
        }
        else if (type == BulletComponentType.ColliderTrigger)
        {
            bullet.AddOrGetComponent<BCColliderTrigger>();
        }
        return 0;
    }

    /// <summary>
    /// 使用组件进行移动参数变更，
    /// <para>具体参数参考AddParaChangeEvent方法</para>
    /// <para>bullet</para>
    /// <para>para MovePara</para>
    /// <para>changeMode 改变方式 ChangeTo,IncBy,DecBy,ChangeTo</para>
    /// <para>valueType 参数类型</para>
    /// <para>arg0 参数0</para>
    /// <para>arg1 参数1</para>
    /// <para>valueOffset 随机偏移量</para>
    /// <para>delay 起始延迟</para>
    /// <para>duration 变化持续时间</para>
    /// <para>intMode 插值方式</para>
    /// <para>repeatCount 重复次数</para>
    /// <para>repeatInterval 每次重复的间隔</para>
    /// <para>总计12个参数</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int AddBulletParaChangeEvent(ILuaState luaState)
    {
        ISTGObject o = luaState.ToUserData(-12) as ISTGObject;
        STGObjectParaType para = (STGObjectParaType)luaState.ToInteger(-11);
        ParaChangeMode changeMode  = (ParaChangeMode)luaState.ToInteger(-10);
        int valueType = luaState.ToInteger(-9);
        float arg0 = (float)luaState.ToNumber(-8);
        float arg1 = (float)luaState.ToNumber(-7);
        float valueOffset = (float)luaState.ToNumber(-6);
        int delay = luaState.ToInteger(-5);
        int duration = luaState.ToInteger(-4);
        InterpolationMode intMode = (InterpolationMode)luaState.ToInteger(-3);
        int repeatCount = luaState.ToInteger(-2);
        int repeatInterval = luaState.ToInteger(-1);
        BCParasChange bc = o.AddOrGetComponent<BCParasChange>();
        ParaChangeValue changeValue = new ParaChangeValue
        {
            argType = valueType,
            arg0 = arg0,
            arg1 = arg1,
            offset = valueOffset,
        };
        bc.AddParaChangeEvent(para, changeMode, changeValue, delay, duration, intMode, repeatCount, repeatInterval);
        return 0;
    }

    /// <summary>
    /// 添加子弹的自定义碰撞事件
    /// <para>bullet</para>
    /// <para>triggerType 触发碰撞事件的类型，从eEliminateDef.CustomizedType0~eEliminateDef.CustomizedType5</para>
    /// <para>triggerFuncRef 触发的函数</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int AddBulletColliderTriggerEvent(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-3) as EnemyBulletBase;
        int triggerType = luaState.ToInteger(-2);
        int triggerFuncRef = luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        BCColliderTrigger bc = bullet.AddOrGetComponent<BCColliderTrigger>();
        bc.Register(triggerType, triggerFuncRef);
        return 0;
    }

    /// <summary>
    /// 添加反弹
    /// <para>bullet</para>
    /// <para>reboundPara 反弹参数</para>
    /// <para>reboundCount 反弹次数</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int AddBulletRebound(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-3) as EnemyBulletBase;
        int reboundPara = luaState.ToInteger(-2);
        int reboundCount = luaState.ToInteger(-1);
        BCRebound rebound = bullet.AddOrGetComponent<BCRebound>();
        if (rebound != null)
            rebound.AddReboundPara(reboundPara, reboundCount);
        return 0;
    }
}
