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
        if ( type == BulletComponentType.MoveParasChange )
        {
            bullet.AddComponent<BCParasChange>();
        }
        else if ( type == BulletComponentType.Rebound )
        {
            bullet.AddComponent<BCRebound>();
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
        MovePara para = (MovePara)luaState.ToInteger(-6);
        ParaChangeMode changeMode  = (ParaChangeMode)luaState.ToInteger(-5);
        float changeValue = (float)luaState.ToNumber(-4);
        int delay = luaState.ToInteger(-3);
        InterpolationMode intMode = (InterpolationMode)luaState.ToInteger(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(7);
        BCParasChange bc = bullet.GetComponent<BCParasChange>();
        bc.AddParaChangeEvent(para, changeMode, changeValue, delay, intMode, duration);
        return 0;
    }
}
