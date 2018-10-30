using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionObject
{
    /// <summary>
    /// 设置是否进行碰撞检测
    /// </summary>
    /// <param name="value"></param>
    void SetDetectCollision(bool value);
    /// <summary>
    /// 是否进行碰撞检测
    /// </summary>
    /// <returns></returns>
    bool DetectCollision();
    /// <summary>
    /// 检测两物体包围盒是否相交
    /// <para>用于未知形状碰撞体之间的快速排斥试验</para>
    /// <para>for example</para>
    /// <para>玩家符卡与子弹之间的碰撞</para>
    /// <para>玩家符卡与敌机子弹形状都不是固定的</para>
    /// <para>因此会先进行快速排斥试验</para>
    /// </summary>
    /// <param name="lbPos">左下坐标</param>
    /// <param name="rtPos">右上坐标</param>
    /// <returns></returns>
    bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos);
    /// <summary>
    /// 获取碰撞检测的参数
    /// </summary>
    /// <param name="index">对应的第n个碰撞盒的参数</param>
    /// <returns></returns>
    CollisionDetectParas GetCollisionDetectParas(int n = 0);
    /// <summary>
    /// 物体第n个碰撞盒被物体碰撞了
    /// </summary>
    /// <param name="index"></param>
    void CollidedByObject(int n = 0,eEliminateDef eliminateDef=eEliminateDef.HitObjectCollider);
}