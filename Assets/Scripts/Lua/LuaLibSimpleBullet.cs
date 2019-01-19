﻿using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 根据id创建SimpleBullet
    /// <para>id 配置里面的id</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSimpleBulletById(ILuaState luaState)
    {
        string sysId = luaState.ToString(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        EnemyBulletSimple bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_Simple) as EnemyBulletSimple;
        bullet.SetStyleById(sysId);
        bullet.SetToPosition(posX, posY);
        luaState.PushLightUserData(bullet);
        return 1;
    }

    /// <summary>
    /// 设置子弹生成时是否播放生成特效
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletAppearEffectAvailable(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-2) as EnemyBulletSimple;
        bool isAvailable = luaState.ToBoolean(-1);
        luaState.Pop(2);
        bullet.SetAppearEffectAvailable(isAvailable);
        return 0;
    }

    /// <summary>
    /// 设置子弹是否自旋
    /// <para>SimpleBullet bullet</para>
    /// <para>float omega 自旋角度，为0说明不自旋</para>
    /// <para></para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletSelfRotation(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-2) as EnemyBulletSimple;
        float omega = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        bullet.SetSelfRotation(omega);
        return 0;
    }

    /// <summary>
    /// 设置子弹的缩放
    /// <para>scale 缩放值</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletScale(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-2) as EnemyBulletSimple;
        float scale = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        bullet.SetScale(scale);
        return 0;
    }

    /// <summary>
    /// SimpleBullet延迟delay帧后经过duration帧缩放至toScale
    /// <para>SimpleBullet bullet</para>
    /// <para>float toScale</para>
    /// <para>int duration</para>
    /// <para>int delay</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int BulletDoScale(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-4) as EnemyBulletSimple;
        float toScale = (float)luaState.ToNumber(-3);
        int delay = luaState.ToInteger(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        bullet.DoScale(toScale, delay, duration);
        return 0;
    }

    /// <summary>
    /// <para>bullet</para>
    /// <para>velocity</para>
    /// <para>vAngle</para>
    /// <para>isAimToPlayer</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletStraightParas(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-6) as EnemyBulletSimple;
        float velocity = (float)luaState.ToNumber(-5);
        float vAngle = (float)luaState.ToNumber(-4);
        bool isAimToPlayer = luaState.ToBoolean(-3);
        float acce = (float)luaState.ToNumber(-2);
        float accAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(6);
        // 设置子弹线性运动
        if (isAimToPlayer)
        {
            vAngle += MathUtil.GetAngleBetweenXAxis(Global.PlayerPos.x - bullet.PosX, Global.PlayerPos.y - bullet.PosY, false);
        }
        bullet.DoStraightMove(velocity, vAngle);
        bullet.DoAcceleration(acce, accAngle);
        return 0;
    }

    /// <summary> 赋予子弹加速度
    /// <para>bullet</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DoBulletAcceleration(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-3) as EnemyBulletSimple;
        float acce = (float)luaState.ToNumber(-2);
        float accAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        bullet.DoAcceleration(acce, accAngle);
        return 0;
    }

    /// <summary> 赋予子弹加速度
    /// <para>bullet</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// <para>maxVelocity  速度最大值</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DoBulletAccelerationWithLimitation(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-4) as EnemyBulletSimple;
        float acce = (float)luaState.ToNumber(-3);
        float accAngle = (float)luaState.ToNumber(-2);
        float maxVelocity = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        bullet.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
        return 0;
    }

    /// <summary>
    /// <para>bullet</para>
    /// <para>radius</para>
    /// <para>curveAngle</para>
    /// <para>deltaR</para>
    /// <para>omiga</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletCurvePara(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-5) as EnemyBulletSimple;
        float radius = (float)luaState.ToNumber(-4);
        float curveAngle = (float)luaState.ToNumber(-3);
        float deltaR = (float)luaState.ToNumber(-2);
        float omiga = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        bullet.DoMoveCurve(radius, curveAngle, deltaR, omiga);
        return 0;
    }

    /// <summary>
    /// 创建自定义的simpleBullet
    /// <para>customizedName 自定义的子弹名称</para>
    /// <para>id 默认的id</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedBullet(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        string customizedName = luaState.ToString(-5 - numArgs);
        string sysId = luaState.ToString(-4 - numArgs);
        float posX = (float)luaState.ToNumber(-3 - numArgs);
        float posY = (float)luaState.ToNumber(-2 - numArgs);
        luaState.Pop(1);
        EnemyBulletSimple bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_Simple) as EnemyBulletSimple;
        bullet.SetStyleById(sysId);
        bullet.SetToPosition(posX, posY);
        // 设置自定义的数据
        BCCustomizedTask bc = bullet.AddComponent<BCCustomizedTask>();
        // 使用pcall
        //luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, InterpreterManager.GetInstance().GetTracebackIndex());
        //int funcRef = InterpreterManager.GetInstance().GetInitFuncRef(customizedName);
        //luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        //if (!luaState.IsFunction(-1))
        //{
        //    Logger.LogError("InitFuncRef of " + customizedName + " is not point to a function");
        //}
        //luaState.PushLightUserData(bullet);
        //luaState.Replace(-4 - numArgs);
        //luaState.Replace(-4 - numArgs);
        //luaState.Replace(-4 - numArgs);
        //luaState.PCall(numArgs + 1, 0, -numArgs - 3);
        // 使用call
        int funcRef = InterpreterManager.GetInstance().GetInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        if (!luaState.IsFunction(-1))
        {
            Logger.LogError("InitFuncRef of " + customizedName + " is not point to a function");
        }
        luaState.PushLightUserData(bullet);
        luaState.Replace(-3 - numArgs);
        luaState.Replace(-3 - numArgs);
        luaState.Call(numArgs + 1, 0);
        // 弹出剩余两个参数
        luaState.Pop(2);
        luaState.PushLightUserData(bullet);
        return 1;
    }
}
