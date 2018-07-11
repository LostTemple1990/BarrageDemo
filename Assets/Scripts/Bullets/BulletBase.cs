using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBase : ICollisionObject
{
    protected Vector3 _lastPos;
    protected Vector3 _curPos;
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
    /// 子弹初始化函数
    /// </summary>
    /// <param name="datas">传入的初始参数
    /// <para>datas[0] Vector3 pos 设定初始位置</para>
    /// <para>datas[1] float velocity 设定初始速度 单位：像素每60帧</para>
    /// <para>datas[2] float angle 设定初始角度</para>
    /// <para>datas[3] float A 设定初始加速度</para>
    /// <para>datas[4] float angle 设定初始角加速度</para>
    /// </param>
    public virtual void Init()
    {
        _clearFlag = 0;
        _destroyFlag = 0;
        _isMoving = false;
        _lastPos = Vector3.zero;
    }

    public virtual void Update()
    {

    }

    public virtual void SetToPosition(Vector3 pos)
    {
        _curPos = pos;
    }

    public virtual void SetToPosition(float posX,float posY)
    {
        _curPos.x = posX;
        _curPos.y = posY;
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

    public virtual void DoAction(object[] datas)
    {

    }

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

    }

    public virtual void Destroy()
    {

    }

    public virtual int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        throw new System.NotImplementedException();
    }
}
