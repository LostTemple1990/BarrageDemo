using UnityEngine;
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
        float damage = (float)luaState.ToNumber(-1);
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

    public static int GetBossSpellCardHpRate(ILuaState luaState)
    {
        Boss boss = Global.Boss;
        luaState.PushNumber(boss.GetSpellCardHpRate());
        return 1;
    }

    public static int GetBossSpellCardTimeLeftRate(ILuaState luaState)
    {
        Boss boss = Global.Boss;
        luaState.PushNumber(boss.GetSpellCardTimeLeftRate());
        return 1;
    }
}
