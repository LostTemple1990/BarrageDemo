using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
    /// <summary>
    /// 射击游戏本体左下角坐标
    /// </summary>
    public static Vector2 GameLBBorderPos = Vector2.zero;
    /// <summary>
    /// 射击游戏本体右上角坐标
    /// </summary>
    public static Vector2 GameRTBorderPos = Vector2.zero;
    /// <summary>
    /// 弹幕左下边界
    /// </summary>
    public static Vector2 BulletLBBorderPos = Vector2.zero;
    /// <summary>
    /// 弹幕右上边界
    /// </summary>
    public static Vector2 BulletRTBorderPos = Vector2.zero;

    public static Vector2 PlayerLBBorderPos;
    public static Vector2 PlayerRTBorderPos;

    public static float PlayerPower = 4f;

    public static float[] ExtraArgs;

    public static Vector2 PlayerPos = new Vector2();
    /// <summary>
    /// 检测碰撞的相关参数
    /// <para>x 横向半轴长度</para>
    /// <para>y 纵向半轴长度</para>
    /// <para>z 实际碰撞圆半径</para>
    /// </summary>
    public static Vector3 PlayerCollisionVec = new Vector3();

    public static Boss Boss;

    public static List<Vector2> MultiSegmentsCollisionPointList;
}
