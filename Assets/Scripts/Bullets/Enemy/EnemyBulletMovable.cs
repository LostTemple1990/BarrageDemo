using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBulletMovable : EnemyBulletBase
{
    protected GameObject _bullet;
    protected Transform _trans;

    protected float _curAccAngle;
    protected float _dvx, _dvy;
    protected Vector3 _moveStraightEndPos;
    protected int _curStraightTime;
    protected int _moveStraightDuration;
    protected int _accTime;
    protected int _accDuration;

    #region 极坐标运动相关参数
    protected Vector2 _centerPos;
    protected float _curRadius;
    protected float _deltaRadius;
    protected Vector2 _lastCurvePos;
    /// <summary>
    /// 极坐标当前角度
    /// </summary>
    protected float _curCurveAngle;
    /// <summary>
    /// 当前角速度
    /// </summary>
    protected float _curOmiga;
    #endregion

    /// <summary>
    /// 自身旋转
    /// </summary>
    protected bool _isSelfRotation;
    protected Vector3 _selfRotationAngle;

    protected bool _isRotatedByVelocity;
    protected int _imgRotatedFlag;

    protected bool _isMovingStraight;
    protected bool _isMovingCurve;


    public override void Init()
    {
        base.Init();
        _isMoving = false;
        _isMovingStraight = false;
        _isMovingCurve = false;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
    }

    public override void Update()
    {
        _dx = 0;
        _dy = 0;
        if ( _isMovingStraight )
        {
            MoveStraight();
        }
        if (_isMovingCurve)
        {
            MoveCurve();
        }
        _curPos.x += _dx;
        _curPos.y += _dy;
        UpdateComponents();
    }

    #region 直线运动
    public virtual void DoMoveStraight(float v,float angle)
    {
        _curVelocity = v;
        _curAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = Consts.MaxDuration;
        _vx = _curVelocity * Mathf.Cos(_curAngle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(_curAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    public virtual void DoMoveStraightWithLimitation(float v,float angle,int duration)
    {
        _curVelocity = v;
        _curAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = duration;
        _isMovingStraight = true;
    }

    public virtual void DoAcceleration(float acce,float accAngle)
    {
        _curAcceleration = acce;
        _curAccAngle = accAngle==Consts.VelocityAngle ? _curAngle : accAngle;
        _accTime = 0;
        _accDuration = Consts.MaxDuration;
        // 计算速度增量
        _dvx = _curAcceleration * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcceleration * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    public virtual void DoAccelerationWithLimitation(float acce, float accAngle,int accDuration)
    {
        _curAcceleration = acce;
        _curAccAngle = accAngle == Consts.VelocityAngle ? _curAngle : accAngle;
        _accTime = 0;
        _accDuration = accDuration;
        // 计算速度增量
        _dvx = _curAcceleration * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcceleration * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    protected virtual void MoveStraight()
    {
        // 根据加速度计算新的速度
        if ( _curAcceleration != 0 )
        {
            _vx += _dvx;
            _vy += _dvy;
            _accTime++;
            if ( _accTime >= _accDuration )
            {
                _curAcceleration = 0;
                _dvx = _dvy = 0;
            }
        }
        if ( _moveStraightDuration > 0 )
        {
            _dx += _vx;
            _dy += _vy;
            _curStraightTime++;
            if ( _curStraightTime >= _moveStraightDuration )
            {
                _curStraightTime = 0;
                _moveStraightDuration = 0;
                _isMovingStraight = false;
            }
        }
        else
        {
            // 更新位置增量
            _dx += _vx;
            _dy += _vy;
        }
    }
    #endregion

    #region 极坐标相关运动

    /// <summary>
    /// 做圆周运动，原点为初始位置
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angle"></param>
    /// <param name="deltaR"></param>
    /// <param name="omiga"></param>
    public virtual void DoMoveCurve(float radius, float angle, float deltaR, float omiga)
    {
        _centerPos = new Vector2(_curPos.x, _curPos.y);
        _curRadius = radius;
        _curCurveAngle = angle;
        _deltaRadius = deltaR;
        _curOmiga = omiga;
        _lastCurvePos = _centerPos;
        _isMovingCurve = true;
    }

    protected virtual void MoveCurve()
    {
        _curRadius += _deltaRadius;
        _curCurveAngle += _curOmiga;
        float dstX = _curRadius * Mathf.Cos(_curCurveAngle * Mathf.Deg2Rad) + _centerPos.x;
        float dstY = _curRadius * Mathf.Sin(_curCurveAngle * Mathf.Deg2Rad) + _centerPos.y;
        // 更新位置增量
        _dx += dstX - _lastCurvePos.x;
        _dy += dstY - _lastCurvePos.y;
        _lastCurvePos.x = dstX;
        _lastCurvePos.y = dstY;
    }
    #endregion

    protected virtual void UpdatePos()
    {
        _trans.localPosition = _curPos;
    }

    public override void Clear()
    {
        base.Clear();
        UIManager.GetInstance().RemoveGoFromLayer(_bullet);
        ObjectsPool.GetInstance().RestoreBullet(_prefabName, _bullet);
        _bullet = null;
        _trans = null;
    }

    #region 设置/获取移动参数的相关public方法
    public void SetVelocity(float value)
    {
        _curVelocity = value;
    }
    public float getVelocity()
    {
        return _curVelocity;
    }

    public void SetVAngle(float value)
    {
        _curAngle = value;
    }
    public float GetVAnlge()
    {
        return _curAngle;
    }

    public void SetAcce(float value)
    {
        _curAcceleration = value;
    }
    public float GetAcce()
    {
        return _curAcceleration;
    }

    public void SetAccAngle(float value)
    {
        _curAccAngle = value;
    }
    public float GetAccAngle()
    {
        return _curAccAngle;
    }

    public void SetCirAngle(float value)
    {
        _curCurveAngle = value;
    }
    public float GetCirAngle()
    {
        return _curCurveAngle;
    }

    public void SetCirRadius(float value)
    {
        _curRadius = value;
    }
    public float GetCirRadius()
    {
        return _curRadius;
    }

    public void SetCirDeltaR(float value)
    {
        _deltaRadius = value;
    }
    public float GetCirDeltaR()
    {
        return _deltaRadius;
    }

    public void SetCirOmiga(float value)
    {
        _curOmiga = value;
    }
    public float GetCirOmiga()
    {
        return _curOmiga;
    }

    public void SetCirCenterX(float value)
    {
        _centerPos.x = value;
    }
    public float GetCirCenterX()
    {
        return _centerPos.x;
    }

    public void SetCirCenterY(float value)
    {
        _centerPos.y = value;
    }
    public float GetCirCenterY()
    {
        return _centerPos.y;
    }
    #endregion
}
