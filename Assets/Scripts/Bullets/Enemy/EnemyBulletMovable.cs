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
    /// <summary>
    /// 加速运动的最大速度限制
    /// </summary>
    protected float _maxVelocity;
    /// <summary>
    /// 加速运动最大速度的平方
    /// </summary>
    protected float _sqrMaxV;
    /// <summary>
    /// 是否设置过初始速度
    /// </summary>
    protected bool _isInitVelocity;

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

    protected bool _isMovingStraight;
    protected bool _isMovingCurve;

    /// <summary>
    /// 是否被赋予额外的速度
    /// </summary>
    protected bool _hasExtraSpeed;
    /// <summary>
    /// 被赋予的额外速度的x分量
    /// </summary>
    protected float _extraVelocityX;
    /// <summary>
    /// 被赋予的额外速度的y分量
    /// </summary>
    protected float _extraVelocityY;
    /// <summary>
    /// 被赋予的额外加速度x分量
    /// </summary>
    protected float _extraAcceX;
    /// <summary>
    /// 被赋予的额外加速度的y分量
    /// </summary>
    protected float _extraAcceY;


    public override void Init()
    {
        base.Init();
        _isMoving = false;
        _isMovingStraight = false;
        _isMovingCurve = false;
        _vx = _vy = _dvx = _dvy = _curVelocity = 0;
        _maxVelocity = -1;
        _isInitVelocity = false;
        ResetExtraSpeedParas();
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
    }

    public override void Update()
    {
        base.Update();
        _dx = 0;
        _dy = 0;
        if ( !_isFollowingMasterContinuously )
        {
            if (_isMovingStraight)
            {
                MoveStraight();
            }
            if (_isMovingCurve)
            {
                MoveCurve();
            }
            if ( _hasExtraSpeed )
            {
                _dx += _extraVelocityX + _extraAcceX;
                _dy += _extraVelocityY + _extraAcceY;
            }
            _curPos.x += _dx;
            _curPos.y += _dy;
        }
        else
        {
            if ( _attachableMaster != null )
            {
                Vector2 relativePos = _relativePosToMaster;
                if ( _isFollowMasterRotation )
                {
                    relativePos = MathUtil.GetVec2AfterRotate(relativePos.x, relativePos.y, 0, 0, _attachableMaster.GetRotation());
                }
                _curPos = relativePos + _attachableMaster.GetPosition();
            }
        }
        UpdateComponents();
    }

    #region 直线运动
    public virtual void DoMoveStraight(float v,float angle)
    {
        _curVelocity = v;
        _curVAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = Consts.MaxDuration;
        _vx = _curVelocity * Mathf.Cos(_curVAngle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(_curVAngle * Mathf.Deg2Rad);
        _isInitVelocity = true;
        _isMovingStraight = true;
    }

    public virtual void DoMoveStraightWithLimitation(float v,float angle,int duration)
    {
        _curVelocity = v;
        _curVAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = duration;
        _isInitVelocity = true;
        _isMovingStraight = true;
    }

    public virtual void DoAcceleration(float acce,float accAngle)
    {
        _curAcce = acce;
        _curAccAngle = accAngle==Consts.VelocityAngle ? _curVAngle : accAngle;
        if ( !_isInitVelocity )
        {
            _curVAngle = _curAccAngle;
            _isInitVelocity = true;
        }
        _maxVelocity = -1;
        // 计算速度增量
        _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    public virtual void DoAccelerationWithLimitation(float acce, float accAngle,float maxVelocity)
    {
        _curAcce = acce;
        _curAccAngle = accAngle == Consts.VelocityAngle ? _curVAngle : accAngle;
        if (!_isInitVelocity)
        {
            _curVAngle = _curAccAngle;
            _isInitVelocity = true;
        }
        _maxVelocity = maxVelocity;
        _sqrMaxV = _maxVelocity * _maxVelocity;
        // 计算速度增量
        _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    protected virtual void MoveStraight()
    {
        _vx += _dvx;
        _vy += _dvy;
        if ( _maxVelocity != -1 )
        {
            float value = _vx * _vx + _vy * _vy;
            if ( value > _sqrMaxV )
            {
                value = Mathf.Sqrt(_sqrMaxV / value);
                _vx *= value;
                _vy *= value;
            }
        }
        _dx += _vx;
        _dy += _vy;
        if ( _moveStraightDuration > 0 )
        {
            _curStraightTime++;
            if ( _curStraightTime >= _moveStraightDuration )
            {
                _curStraightTime = 0;
                _moveStraightDuration = 0;
                _isMovingStraight = false;
            }
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

    /// <summary>
    /// 设置额外的直线运动的参数
    /// <para>一般是被引力场影响</para>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public override void AddExtraSpeedParas(float v, float angle, float acce, float accAngle)
    {
        _hasExtraSpeed = true;
        _extraVelocityX += v * Mathf.Cos(angle * Mathf.Deg2Rad);
        _extraVelocityY += v * Mathf.Sin(angle * Mathf.Deg2Rad);
        _extraAcceX += acce * Mathf.Cos(accAngle * Mathf.Deg2Rad);
        _extraAcceY += acce * Mathf.Cos(accAngle * Mathf.Deg2Rad);
    }

    /// <summary>
    /// 重置额外直线运动的参数
    /// </summary>
    protected void ResetExtraSpeedParas()
    {
        _extraVelocityX = 0;
        _extraVelocityY = 0;
        _extraAcceX = 0;
        _extraAcceY = 0;
        _hasExtraSpeed = false;
    }

    protected virtual void UpdatePos()
    {
        _trans.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
    }

    public override void Clear()
    {
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        _bullet = null;
        _trans = null;
        base.Clear();
    }

    #region 设置/获取移动参数的相关public方法
    public override bool GetBulletPara(BulletParaType paraType, out float value)
    {
        value = 0;
        switch ( paraType )
        {
            case BulletParaType.Velocity:
                value = _curVelocity;
                return true;
            case BulletParaType.VAngel:
                value = _curVAngle;
                return true;
            case BulletParaType.Acce:
                value = _curAcce;
                return true;
            case BulletParaType.AccAngle:
                value = _curAccAngle;
                return true;
            case BulletParaType.CurveAngle:
                value = _curCurveAngle;
                return true;
            case BulletParaType.CurveRadius:
                value = _curRadius;
                return true;
            case BulletParaType.CurveDeltaR:
                value = _deltaRadius;
                return true;
            case BulletParaType.CurveOmega:
                value = _curOmiga;
                return true;
            case BulletParaType.CurveCenterX:
                value = _centerPos.x;
                return true;
            case BulletParaType.CurveCenterY:
                value = _centerPos.y;
                return true;
        }
        return false;
    }

    public override bool SetBulletPara(BulletParaType paraType, float value)
    {
        switch (paraType)
        {
            case BulletParaType.Velocity:
                _curVelocity = value;
                return true;
            case BulletParaType.VAngel:
                _curVAngle = value;
                return true;
            case BulletParaType.Acce:
                _curAcce = value;
                return true;
            case BulletParaType.AccAngle:
                _curAccAngle = value;
                return true;
            case BulletParaType.CurveAngle:
                _curCurveAngle = value;
                return true;
            case BulletParaType.CurveRadius:
                _curRadius = value;
                return true;
            case BulletParaType.CurveDeltaR:
                _deltaRadius = value;
                return true;
            case BulletParaType.CurveOmega:
                _curOmiga = value;
                return true;
            case BulletParaType.CurveCenterX:
                _centerPos.x = value;
                return true;
            case BulletParaType.CurveCenterY:
                _centerPos.y = value;
                return true;
        }
        return false;
    }
    #endregion
}
