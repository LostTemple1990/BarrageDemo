using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    /// <summary>
    /// 中心点
    /// </summary>
    public Vector2 centerPos;
    public Vector2 linePointA;
    public Vector2 linePointB;
    public float angle;
    public List<Vector3> multiSegmentPointList;
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
    ReimuA_Main = 110,
    ReimuA_Sub1 = 111,
    ReimuA_Sub2 = 112,
    Enemy_Laser = 504,
    Enemy_CurveLaser = 505,
    Enemy_Simple = 506,
    Enemy_LinearLaser = 507,
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

[Flags]
public enum eEliminateDef : int
{
    /// <summary>
    /// 强制直接删除
    /// </summary>
    ForcedDelete = 0,
    /// <summary>
    /// 玩家B触发的消除
    /// </summary>
    PlayerBomb = 1,
    /// <summary>
    /// 玩家死亡触发的消除
    /// </summary>
    PlayerDead = 2,
    /// <summary>
    /// 击中玩家触发的消除
    /// </summary>
    HitPlayer = 4,
    /// <summary>
    /// 符卡结束触发的消除
    /// </summary>
    SpellCardEnd = 8,
    /// <summary>
    /// 击中某些物体触发的消除
    /// </summary>
    HitObject = 16,
    /// <summary>
    /// 直接调用代码触发的消除
    /// </summary>
    CodeEliminate = 32,
    /// <summary>
    /// 直接调用代码，不触发消除触发的函数
    /// </summary>
    CodeRawEliminate = 64,
}

[Flags]
public enum eEnemyEliminateDef :int
{
    ForcedDelete = 0,
    Player = 1,
    SpellCardEnd = 2,
    CodeEliminate = 4,
    CodeRawEliminate = 8,
}

public enum eLaserHeadType :int
{
    Null = 0,
    White = 1,
    Red = 2,
    Green = 3,
    Blue = 4,
}

public enum eUIType : int
{
    Any = 0,
    Image = 4,
    RawImage = 5,
    SpriteRenderer = 6,
    Text = 8,
}

/// <summary>
/// 播放模式
/// </summary>
public enum ePlayMode : int
{
    Once = 0,
    Loop = 1,
    PingPong = 2,
}

public enum eGameState : int
{
    Main = 1,
    STG = 3,
}

public enum eTweenType :int
{
    Alhpa = 1,
    Color = 2,
    Pos2D = 3,
    Pos3D = 4,
    Rotation = 5,
    Scale = 6,
}

#region struct ItemWithFramentsCounter
/// <summary>
/// 带碎片的道具计数器
/// </summary>
public struct ItemWithFramentsCounter
{
    public int itemCount;
    public int maxItemCount;
    public int fragmentCount;
    public int maxFragmentCount;

    public void AddItemCount(int value)
    {
        itemCount = Mathf.Min(maxItemCount, itemCount + value);
    }

    public void AddFragmentCount(int value)
    {
        fragmentCount += value;
        if ( fragmentCount >= maxFragmentCount )
        {
            int extraItemCount = fragmentCount / maxFragmentCount;
            fragmentCount %= maxFragmentCount;
            itemCount += extraItemCount;
            // 最大持有数目不能超过maxItemCount以及maxFragmentCount
            if ( itemCount > maxItemCount )
            {
                itemCount = maxItemCount;
                fragmentCount = maxFragmentCount;
            }
        }
    }

    /// <summary>
    /// 消耗道具
    /// </summary>
    /// <param name="costValue">消耗的数目</param>
    /// <returns></returns>
    public bool CostItem(int costValue)
    {
        // 当itemCount达到上限时先消耗碎片
        if ( itemCount >= maxItemCount && fragmentCount >= maxFragmentCount )
        {
            costValue -= 1;
            fragmentCount = 0;
        }
        if ( itemCount >= costValue )
        {
            itemCount -= costValue;
            return true;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (GetType() != obj.GetType()) return false;
        return Equals((ItemWithFramentsCounter)obj);
    }

    public bool Equals(ItemWithFramentsCounter obj)
    {
        return itemCount == obj.itemCount &&
            maxItemCount == obj.maxItemCount &&
            fragmentCount == obj.fragmentCount &&
            maxFragmentCount == obj.maxFragmentCount;
    }

    public static bool operator ==(ItemWithFramentsCounter left,ItemWithFramentsCounter right)
    {
        if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
        return left.Equals(right);
    }

    public static bool operator !=(ItemWithFramentsCounter left, ItemWithFramentsCounter right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
#endregion