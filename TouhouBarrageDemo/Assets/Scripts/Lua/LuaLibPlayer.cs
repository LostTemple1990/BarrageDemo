﻿using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 获取玩家坐标
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetPlayerPos(ILuaState luaState)
    {
        luaState.PushNumber(Global.PlayerPos.x);
        luaState.PushNumber(Global.PlayerPos.y);
        return 2;
    }

    /// <summary>
    /// 设置玩家的位置
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetPlayerPos(ILuaState luaState)
    {
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        PlayerInterface.GetInstance().GetCharacter().SetPosition(posX, posY);
        return 0;
    }

    /// <summary>
    /// 设置玩家当前是否可以移动
    /// <para>isMovable </para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetPlayerIsMovable(ILuaState luaState)
    {
        bool isMovable = luaState.ToBoolean(-1);
        PlayerInterface.GetInstance().GetCharacter().IsMovable = isMovable;
        return 0;
    }

    /// <summary>
    /// 获取玩家当前是否可以移动
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetPlayerIsMovable(ILuaState luaState)
    {
        bool isMovable = PlayerInterface.GetInstance().GetCharacter().IsMovable;
        luaState.PushBoolean(isMovable);
        return 1;
    }

    /// <summary>
    /// 设置关卡当前是否允许射击
    /// <para>isEnable</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetStageIsEnableToShoot(ILuaState luaState)
    {
        bool isEnable = luaState.ToBoolean(-1);
        luaState.Pop(1);
        STGStageManager.GetInstance().SetIsEnableToShoot(isEnable);
        return 0;
    }


    public static int GetKey(ILuaState luaState)
    {
        int key = luaState.ToInteger(-1);
        bool ret = OperationController.GetInstance().GetKey((eSTGKey)key);
        luaState.PushBoolean(ret);
        return 1;
    }

    public static int GetKeyDown(ILuaState luaState)
    {
        int key = luaState.ToInteger(-1);
        bool ret = OperationController.GetInstance().GetKeyDown((eSTGKey)key);
        luaState.PushBoolean(ret);
        return 1;
    }

    public static int GetKeyUp(ILuaState luaState)
    {
        int key = luaState.ToInteger(-1);
        bool ret = OperationController.GetInstance().GetKeyUp((eSTGKey)key);
        luaState.PushBoolean(ret);
        return 1;
    }
}
