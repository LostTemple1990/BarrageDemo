﻿using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    public static int EnemyMoveTowards(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-4) as EnemyBase;
        float v = (float)luaState.ToNumber(-3);
        float angle = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        enemy.MoveTowards(v, angle, duration);
        return 0;
    }

    public static int EnemyMoveToPos(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-5) as EnemyBase;
        float endX = (float)luaState.ToNumber(-4);
        float endY = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        int moveMode = luaState.ToInteger(-1);
        luaState.Pop(5);
        enemy.MoveToPos(endX, endY, duration, (InterpolationMode)moveMode);
        return 0;
    }

    public static int GetEnemyPos(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-1) as EnemyBase;
        luaState.Pop(1);
        luaState.PushNumber(enemy.CurPos.x);
        luaState.PushNumber(enemy.CurPos.y);
        return 2;
    }

    public static int HitEnemy(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int damage = luaState.ToInteger(-1);
        luaState.Pop(2);
        enemy.GetHit(damage);
        return 0;
    }

    public static int EliminateEnemy(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-1) as EnemyBase;
        enemy.Eliminate(eEnemyEliminateDef.CodeEliminate);
        return 0;
    }

    public static int RawEliminateEnemy(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-1) as EnemyBase;
        enemy.Eliminate(eEnemyEliminateDef.CodeRawEliminate);
        return 0;
    }

    /// <summary>
    /// 设置移动范围
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyWanderRange(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-5) as EnemyBase;
        float minX = (float)luaState.ToNumber(-4);
        float maxX = (float)luaState.ToNumber(-3);
        float minY = (float)luaState.ToNumber(-2);
        float maxY = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        enemy.SetWanderRange(minX, maxX, minY, maxY);
        return 0;
    }

    /// <summary>
    /// 设置每次移动的距离限制
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyWanderAmplitude(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-5) as EnemyBase;
        float minX = (float)luaState.ToNumber(-4);
        float maxX = (float)luaState.ToNumber(-3);
        float minY = (float)luaState.ToNumber(-2);
        float maxY = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        enemy.SetWanderAmplitude(minX, maxX, minY, maxY);
        return 0;
    }

    /// <summary>
    /// 设置移动的模式
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyWanderMode(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-3) as EnemyBase;
        InterpolationMode moveMode = (InterpolationMode)luaState.ToInteger(-2);
        DirectionMode dirMode = (DirectionMode)luaState.ToInteger(-1);
        luaState.Pop(3);
        enemy.SetWanderMode(moveMode, dirMode);
        return 0;
    }

    /// <summary>
    /// 进行移动
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyDoWander(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int duration = luaState.ToInteger(-1);
        luaState.Pop(2);
        enemy.DoWander(duration);
        return 0;
    }

    /// <summary>
    /// 设置敌机的最大血量，同时同步当前血量到最大血量
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyMaxHp(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int maxHp = luaState.ToInteger(-1);
        luaState.Pop(2);
        enemy.SetMaxHp(maxHp);
        return 0;
    }

    /// <summary>
    /// 获取boss当前符卡的hp比例
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetBossSpellCardHpRate(ILuaState luaState)
    {
        int index = luaState.ToInteger(-1);
        luaState.Pop(1);
        SpellCard sc = STGStageManager.GetInstance().GetSpellCard();
        Boss boss = sc.GetBossByIndex(index);
        float rate = 0f;
        if ( boss != null )
        {
            rate = (float)boss.GetCurHp() / boss.GetMaxHp();
        }
        luaState.PushNumber(rate);
        return 1;
    }

    public static int GetSpellCardTimeLeftRate(ILuaState luaState)
    {
        SpellCard sc = STGStageManager.GetInstance().GetSpellCard();
        float rate = sc.GetTimeRate();
        luaState.PushNumber(rate);
        return 1;
    }
}
