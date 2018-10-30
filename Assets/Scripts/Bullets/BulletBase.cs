using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase :ICollisionObject
{
    protected Vector2 _lastPos;
    protected Vector2 _curPos;
    protected int _clearFlag;
    protected int _destroyFlag;
    protected BulletId _id;

    protected bool _isMoving;
    protected float _curVelocity;
    protected float _curAngle;
    protected float _curAngleVelocity;
    protected float _curAcceleration;
    protected float _curAngleAcceleration;
    protected float _vx, _vy;

    protected float _dx, _dy;
    /// <summary>
    /// 碰撞检测相关参数
    /// </summary>
    protected GrazeDetectParas _grazeParas;
    protected CollisionDetectParas _collisionParas;
    /// <summary>
    /// 表示是否进行碰撞检测
    /// </summary>
    protected bool _detectCollision;
    /// <summary>
    /// 使用Z轴来进行先后的排序
    /// <para>为了保持一致性，设置的时候取反</para>
    /// <para>即，orderInLayer = 5 ，则设置Z = -5</para>
    /// <para>这是为了让order较大的子弹永远显示在前面</para>
    /// </summary>
    protected int _orderInLayer;
    /// <summary>
    /// 标识进行边界检测以便回收
    /// </summary>
    protected bool _checkOutOfBorder;
    /// <summary>
    /// 系统繁忙权重，用于判断当前系统是否繁忙
    /// <para>当系统不繁忙时，会从对象池中destroy多余的prefab</para>
    /// </summary>
    protected int _sysBusyWeight = 1;

    public virtual void Init()
    {
        _clearFlag = 0;
        _destroyFlag = 0;
        _isMoving = false;
        _lastPos = Vector2.zero;
        _curPos = Vector2.zero;
        _orderInLayer = 0;
        _checkOutOfBorder = true;
        _detectCollision = true;
        Global.SysBusyValue += _sysBusyWeight;
    }

    public virtual void Update()
    {

    }

    public virtual void SetToPosition(Vector2 pos)
    {
        _curPos = pos;
    }

    public virtual void SetToPosition(float posX,float posY)
    {
        _curPos.x = posX;
        _curPos.y = posY;
    }

    /// <summary>
    /// 设置子弹的显示层级
    /// </summary>
    /// <param name="orderInLayer"></param>
    public virtual void SetOrderInLayer(int orderInLayer)
    {
        _orderInLayer = orderInLayer;
    }

    /// <summary>
    /// 设置是否进行边界检测
    /// </summary>
    /// <param name="value"></param>
    public void SetCheckOutOfBorder(bool value)
    {
        _checkOutOfBorder = value;
    }

    /// <summary>
    /// 设置是否进行碰撞检测
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetDetectCollision(bool value)
    {
        _detectCollision = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="v">速度</param>
    /// <param name="angle">角度</param>
    /// <param name="av">角速度</param>
    /// <param name="a">加速度</param>
    /// <param name="aa">角加速度</param>
    public virtual void DoMove(float v,float angle,float av=0f,float a=0f,float aa=0f)
    {
        _curVelocity = v;
        _curAngle = angle;
        _curAngleVelocity = av;
        _curAcceleration = a;
        _curAngleAcceleration = aa;
        _vx = _curVelocity * Mathf.Cos(angle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(angle * Mathf.Deg2Rad);
        _isMoving = true;
    }

    protected virtual void Move()
    {

    }

    /// <summary>
    /// 是否进行碰撞检测
    /// </summary>
    /// <returns></returns>
    public virtual bool DetectCollision()
    {
        return false;
    }

    public float PosX
    {
        get { return _curPos.x; }
    }

    public float PosY
    {
        get { return _curPos.y; }
    }

    public virtual Vector2 CurPos
    {
        get { return _curPos; }
    }

    public int DestroyFlag
    {
        get { return _destroyFlag; }
    }

    public int ClearFlag
    {
        get { return _clearFlag; }
    }

    public BulletId Id
    {
        get { return _id; }
    }

    protected virtual bool IsOutOfBorder()
    {
        if ( !_checkOutOfBorder )
        {
            return false;
        }
        if (_curPos.x < Global.BulletLBBorderPos.x || 
            _curPos.y < Global.BulletLBBorderPos.y ||
            _curPos.x > Global.BulletRTBorderPos.x ||
            _curPos.y > Global.BulletRTBorderPos.y)
        {
            //Logger.Log("bullet is out of border");
            return true;
        }
        return false;
    }

    public virtual void Clear()
    {
        Global.SysBusyValue -= _sysBusyWeight;
    }

    public virtual void Destroy()
    {

    }

    public virtual bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        throw new System.NotImplementedException();
    }

    public virtual CollisionDetectParas GetCollisionDetectParas(int index=0)
    {
        throw new System.NotImplementedException();
    }

    public virtual void CollidedByObject(int n = 0,eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetCollisionDetectParas(CollisionDetectParas paras)
    {
        _collisionParas = paras;
    }

    public virtual void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeParas = paras;
    }

    public virtual CollisionDetectParas GetCollisionDetectParas()
    {
        throw new System.NotImplementedException();
    }

    public virtual GrazeDetectParas GetGrazeDetectParas()
    {
        throw new System.NotImplementedException();
    }
}
