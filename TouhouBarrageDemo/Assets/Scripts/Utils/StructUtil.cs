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
    //public Vector2 linePointA;
    //public Vector2 linePointB;
    public float angle;
    /// <summary>
    /// 下一个碰撞盒的索引
    /// </summary>
    public int nextIndex;
    //public List<Vector2> multiSegmentPointList;
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
    /// 无判定
    /// </summary>
    Null = 0,
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
    /// <summary>
    /// 旋转矩形
    /// </summary>
    ItalicRect = 5,
}

public enum GrazeDetectType : int
{
    Rect = 2,
}
#endregion

#region 子弹类型
public enum BulletType : int
{
    ReimuA_Main = 110,
    ReimuA_Sub1 = 111,
    ReimuA_Sub2 = 112,
    Player_Laser = 113,
    Player_Simple = 114,
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
    ParasChange = 2,
    Rebound = 3,
    ColliderTrigger = 4,
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
    PlayerSpellCard = 1,
    /// <summary>
    /// 玩家死亡触发的消除
    /// </summary>
    PlayerDead = 2,
    /// <summary>
    /// 击中玩家触发的消除
    /// </summary>
    HitPlayer = 4,
    /// <summary>
    /// 玩家子弹击中触发的消除(针对敌机)
    /// </summary>
    PlayerBullet = 8,
    /// <summary>
    /// 击中某些物体触发的消除
    /// </summary>
    HitObjectCollider = 16,
    /// <summary>
    /// 引力场
    /// </summary>
    GravitationField = 32,
    /// <summary>
    /// 直接调用代码触发的消除
    /// </summary>
    CodeEliminate = 2 << 8,
    /// <summary>
    /// 直接调用代码，不触发消除触发的函数
    /// </summary>
    CodeRawEliminate = 2 << 9,
    CustomizedType0 = 2 << 10,
    CustomizedType1 = 2 << 11,
    CustomizedType2 = 2 << 12,
    CustomizedType3 = 2 << 13,
    CustomizedType4 = 2 << 14,
    CustomizedType5 = 2 << 15,
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
    Title = 1,
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

/// <summary>
/// 调用clear函数的类型
/// </summary>
public enum eSTGClearType :byte
{
    /// <summary>
    /// 返回到主菜单，全部清除
    /// </summary>
    BackToTitle = 1,
    /// <summary>
    /// 重开当前关卡，各个初始化函数等都不清除
    /// </summary>
    RetryCurStage = 2,
    /// <summary>
    /// 重开游戏，全部清除
    /// </summary>
    RetryGame = 3,
}

public enum eBlendMode : int
{
    Normal = 0,
    SoftAdditive = 1,
}

[Flags]
public enum eColliderGroup : int
{
    Player = 1,
    PlayerBullet = 2,
    Enemy = 4,
    EnemyBullet = 8,
    Item = 16,
    CustomizedType0 = 2 << 10,
    CustomizedType1 = 2 << 11,
    CustomizedType2 = 2 << 12,
    CustomizedType3 = 2 << 13,
    CustomizedType4 = 2 << 14,
    CustomizedType5 = 2 << 15,
}

public enum eColliderType : int
{
    Circle = 1,
    Rect = 2,
}

/// <summary>
/// 自机状态
/// </summary>
public enum eCharacterState : int
{
    /// <summary>
    /// 未被赋值的状态
    /// </summary>
    Undefined = -1,
    /// <summary>
    /// 通常状态
    /// </summary>
    Normal = 1,
    /// <summary>
    /// 从屏幕下方出现的状态
    /// </summary>
    Appear = 2,
    /// <summary>
    /// 决死状态
    /// </summary>
    Dying = 3,
    /// <summary>
    /// 死亡，等待复活状态
    /// </summary>
    WaitReborn = 4,
}

public enum GravitationType : byte
{
    /// <summary>
    /// 普通
    /// </summary>
    Normal = 0,
    /// <summary>
    /// 向心力
    /// </summary>
    Centripetal = 1,
    /// <summary>
    /// 离心力
    /// </summary>
    Centrifugal = 2,
}

public struct GravitationParas
{
    /// <summary>
    /// 赋予的速度
    /// </summary>
    public float velocity;
    /// <summary>
    /// 速度方向
    /// </summary>
    public float vAngle;
    /// <summary>
    /// 加速度
    /// </summary>
    public float acce;
    /// <summary>
    /// 加速度方向
    /// </summary>
    public float accAngle;
    /// <summary>
    /// 在引力场中的时间
    /// </summary>
    public int timeInGravitationField;
    /// <summary>
    /// 上次更新的时间
    /// </summary>
    public int lastUpdateTime;
}

public enum eCustomizedType : byte
{
    Bullet = 1,
    Enemy = 2,
    SpellCard = 3,
    Stage = 4,
    STGObject = 5,
    Collider = 6,
}

public enum eCustomizedFuncRefType : byte
{
    Init = 1,
    OnKill = 2,
    Other = 5,
}

/// <summary>
/// STG的操作按键枚举
/// </summary>
[Flags]
[Serializable]
public enum eSTGKey : int
{
    None = 0,
    KeyLeft = 1 << 0,
    KeyRight = 1 << 1,
    KeyUp = 1 << 2,
    KeyDown = 1 << 3,
    KeyShift = 1 << 4,
    KeyZ = 1 << 5,
    KeyX = 1 << 6,
    KeyC = 1 << 7,
    KeyCtrl = 1 << 8,
}

/// <summary>
/// 玩家的移动状态
/// <para>高速，低速</para>
/// </summary>
public enum ePlayerMoveMode : byte
{
    HighSpeed = 0,
    LowSpeed = 1,
}

public struct STGData
{
    public string stageName;
    public int difficulty;
    public int characterIndex;
    public long seed;
    public bool isReplay;
}

/// <summary>
/// Replay的基础信息
/// </summary>
[Serializable]
public struct ReplayInfo
{
    public int replayIndex;
    public string name;
    public long dateTick;
    public int lastFrame;
    public string stageName;
    public int characterIndex;
}

/// <summary>
/// Replay的详细数据
/// </summary>
[Serializable]
public struct ReplayData
{
    public ReplayInfo info;
    public long seed;
    public List<eSTGKey> keyList;
    public int lastFrame;
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