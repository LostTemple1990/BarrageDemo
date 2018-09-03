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

    /// <summary>
    /// STG本体的实际尺寸
    /// </summary>
    public static Vector2 STGActualSize;
    /// <summary>
    /// STG本体的实际宽度缩放比例
    /// </summary>
    public static float STGAcutalScaleWidth;
    /// <summary>
    /// STG本体的实际高度缩放比例
    /// </summary>
    public static float STGAcutalScaleHeight;

    public static Vector2 PlayerLBBorderPos;
    public static Vector2 PlayerRTBorderPos;

    public static float[] ExtraArgs;

    public static Vector2 PlayerPos = new Vector2();
    /// <summary>
    /// 检测碰撞的相关参数
    /// <para>x 横向半轴长度</para>
    /// <para>y 纵向半轴长度</para>
    /// <para>z 实际碰撞圆半径</para>
    /// </summary>
    public static Vector3 PlayerCollisionVec = new Vector3();
    /// <summary>
    /// 玩家的擦弹检测半径
    /// </summary>
    public static float PlayerGrazeRadius = 16f;

    /// <summary>
    /// STG游戏是否暂停
    /// </summary>
    public static bool IsPause = true;
    /// <summary>
    /// 系统繁忙数值
    /// </summary>
    public static int SysBusyValue = 0;

    public static STGMain STGMain;
}
