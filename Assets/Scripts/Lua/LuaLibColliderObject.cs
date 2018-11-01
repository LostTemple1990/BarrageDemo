﻿using UnityEngine;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 根据类型创建一个collider
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateObjectColliderByType(ILuaState luaState)
    {
        eColliderType type = (eColliderType)luaState.ToInteger(-1);
        luaState.Pop(1);
        ObjectColliderBase collider = ColliderManager.GetInstance().CreateColliderByType(type);
        luaState.PushLightUserData(collider);
        return 1;
    }

    /// <summary>
    /// 设置物体碰撞器的位置
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderToPos(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-3) as ObjectColliderBase;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        collider.SetToPositon(posX, posY);
        return 0;
    }

    /// <summary>
    /// 设置物体碰撞器的尺寸
    /// <para>arg0 矩形碰撞器的宽、圆碰撞器的半径</para>
    /// <para>arg1 矩形碰撞器的高，圆碰撞器不使用</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderSize(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-3) as ObjectColliderBase;
        float arg0 = (float)luaState.ToNumber(-2);
        float arg1 = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        collider.SetSize(arg0, arg1);
        return 0;
    }

    /// <summary>
    /// 设置物体碰撞器的碰撞组
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderColliderGroup(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        int colliderGroup = luaState.ToInteger(-1);
        luaState.Pop(2);
        collider.SetColliderGroup((eColliderGroup)colliderGroup);
        return 0;
    }

    /// <summary>
    /// 设置物体碰撞器的生存时间
    /// <para>duration 生存时间</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderExistDuration(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        int duration = luaState.ToInteger(-1);
        luaState.Pop(2);
        collider.SetExistDuration(duration);
        return 0;
    }

    /// <summary>
    /// 将ObjectCollider缩放至指定尺寸
    /// <para>toWidth 圆的半径、矩形的宽</para>
    /// <para>toHeight</para>
    /// <para>duration 持续时间</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ObjectColliderScaleToSize(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-4) as ObjectColliderBase;
        float toWidth = (float)luaState.ToNumber(-3);
        float toHeight = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        collider.ScaleToSize(toWidth, toHeight, duration);
        return 0;
    }

    /// <summary>
    /// ObjectCollider清除自身
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ObjectColliderClearSelf(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-1) as ObjectColliderBase;
        luaState.Pop(1);
        collider.ClearSelf();
        return 0;
    }

    /// <summary>
    /// 设置ObjectCollider的清除类型
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderEliminateType(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        eEliminateDef eliminateType = (eEliminateDef)luaState.ToInteger(-1);
        luaState.Pop(2);
        collider.SetEliminateType(eliminateType);
        return 0;
    }

    /// <summary>
    /// 设置ObjectCollider击中敌机的伤害
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderHitEnemyDamage(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        int damage = luaState.ToInteger(-1);
        luaState.Pop(2);
        collider.SetHitEnemyDamage(damage);
        return 0;
    }
}
