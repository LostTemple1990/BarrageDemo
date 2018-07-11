using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructUtil {

}

#region 碰撞检测相关的结构
public struct CollisionDetectParas
{
    public CollisionDetectType type;
    public float halfWidth;
    public float halfHeight;
    /// <summary>
    /// <para>圆判定时代表半径</para>
    /// <para>直线判定时代表判定距离</para>
    /// </summary>
    public float radius;
    public Vector2 centerPos;
    public Vector2 linePointA;
    public Vector2 linePointB;
    public float angle;
}

public struct GrazeDetectParas
{
    public GrazeDetectType type;
    public float halfWidth;
    public float halfHeight;
}

public enum CollisionDetectType : int
{
    /// <summary>
    /// 圆周判定
    /// </summary>
    Circle = 1,
    /// <summary>
    /// 矩形判定
    /// </summary>
    Rect = 2,
    /// <summary>
    /// 直线判定
    /// </summary>
    Line = 3,
    /// <summary>
    /// 多个线段集合
    /// </summary>
    MultiSegments = 4,
}

public enum GrazeDetectType : int
{
    Rect = 2,
}
#endregion

#region 子弹id
public enum BulletId : int
{
    BulletId_ReimuA_Main = 110,
    BulletId_ReimuA_Sub1 = 111,
    BulletId_ReimuA_Sub2 = 112,
    BulletId_Enemy_Straight = 501,
    BulletId_Enemy_Rebound = 502,
    BulletId_Enemy_Curve = 503,
    BulletId_Enemy_Laser = 504,
    BulletId_Enemy_CurveLaser = 505,
    BulletId_Enemy_Simple = 506,
    BulletId_Enemy_LinearLaser = 507,
}
#endregion

public enum LuaOperationStatus
{
    FAIL = 0,
    SUCCESS = 1,
}

public enum AniActionType : byte
{
    Idle = 0,
    Move = 1,
    FadeToMove = 2,
    Cast = 3,
}

public struct BossRefData
{
    public int initFuncRef;
    public int taskFuncRef;
}

public enum BulletComponentType : byte
{
    CustomizedTask = 1,
    MoveParasChange = 2,
    Rebound = 3,
}

public enum DirectionMode : byte
{
    MoveXTowardsPlayer = 1,
    MoveYTowardsPlayer = 2,
    MoveTowardsPlayer = 3,
    MoveRandom = 4,
}

public enum EnemyObjectType  : byte
{
    Fairy = 1,
    SpinningEnemy = 2,
    Ghost = 3,
}