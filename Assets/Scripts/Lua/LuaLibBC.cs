using UnityEngine;
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
        luaState.Pop(2);
        if ( type == BulletComponentType.ParasChange )
        {
            bullet.AddComponent<BCParasChange>();
        }
        else if ( type == BulletComponentType.Rebound )
        {
            bullet.AddComponent<BCRebound>();
        }
        else if (type == BulletComponentType.ColliderTrigger)
        {
            bullet.AddComponent<BCColliderTrigger>();
        }
        return 0;
    }

    /// <summary>
    /// 使用组件进行移动参数变更，
    /// <para>具体参数参考AddParaChangeEvent方法</para>
    /// <para>para MovePara</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int AddBulletParaChangeEvent(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-7) as EnemyBulletBase;
        BulletParaType para = (BulletParaType)luaState.ToInteger(-6);
        ParaChangeMode changeMode  = (ParaChangeMode)luaState.ToInteger(-5);
        float changeValue = (float)luaState.ToNumber(-4);
        int delay = luaState.ToInteger(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode intMode = (InterpolationMode)luaState.ToInteger(-1);
        luaState.Pop(7);
        BCParasChange bc = bullet.GetComponent<BCParasChange>();
        bc.AddParaChangeEvent(para, changeMode, changeValue, 0, delay, duration, intMode, 1, 0);
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
        luaState.Pop(2);
        BCColliderTrigger bc = bullet.GetComponent<BCColliderTrigger>();
        bc.Register(triggerType, triggerFuncRef);
        return 0;
    }
}
