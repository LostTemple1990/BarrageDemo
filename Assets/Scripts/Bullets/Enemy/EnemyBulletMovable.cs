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
    /// <summary>
    /// 是否需要重新计算速度方向
    /// </summary>
    protected bool _reCalVAngle;

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
    protected float _curOmega;
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
    public virtual void DoStraightMove(float v,float angle)
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

    public virtual void DoStraightMoveWithLimitation(float v,float angle,int duration)
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
        if ( _maxVelocity >= 0 )
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
        _curOmega = omiga;
        _lastCurvePos = _centerPos;
        _isMovingCurve = true;
    }

    protected virtual void MoveCurve()
    {
        _curRadius += _deltaRadius;
        _curCurveAngle += _curOmega;
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
        _extraAcceY += acce * Mathf.Sin(accAngle * Mathf.Deg2Rad);
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
            case BulletParaType.Vx:
                value = _vx;
                return true;
            case BulletParaType.Vy:
                value = _vy;
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
            case BulletParaType.MaxVelocity:
                value = _maxVelocity;
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
                value = _curOmega;
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
                Velocity = value;
                return true;
            case BulletParaType.Vx:
                Vx = value;
                return true;
            case BulletParaType.Vy:
                Vy = value;
                return true;
            case BulletParaType.VAngel:
                VAngle = value;
                return true;
            case BulletParaType.Acce:
                Acce = value;
                return true;
            case BulletParaType.AccAngle:
                AccAngle = value;
                return true;
            case BulletParaType.MaxVelocity:
                MaxVelocity = value;
                return true;
            case BulletParaType.CurveAngle:
                CurveAngle = value;
                return true;
            case BulletParaType.CurveRadius:
                CurveRadius = value;
                return true;
            case BulletParaType.CurveDeltaR:
                CurveDeltaRadius = value;
                return true;
            case BulletParaType.CurveOmega:
                CurveOmega = value;
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

    /// <summary>
    /// 速度
    /// </summary>
    public float Velocity
    {
        get { return _vx; }
        protected set
        {
            if (_maxVelocity < 0)
            {
                _curVelocity = value;
            }
            else
            {
                _curVelocity = value > _maxVelocity ? _maxVelocity : value;
            }
            if (_reCalVAngle)
            {
                _curVAngle = MathUtil.GetAngleBetweenXAxis(_vx, _vy);
                _reCalVAngle = false;
            }
            _vx = _curVelocity * Mathf.Cos(_curVAngle * Mathf.Deg2Rad);
            _vy = _curVelocity * Mathf.Sin(_curVAngle * Mathf.Deg2Rad);
        }
    }

    /// <summary>
    /// x轴方向的速度
    /// </summary>
    public float Vx
    {
        get { return _vx; }
        protected set
        {
            _vx = value;
            Velocity = Mathf.Sqrt(_vx * _vx + _vy * _vy);
        }
    }

    /// <summary>
    /// y轴方向的速度
    /// </summary>
    public float Vy
    {
        get { return _vy; }
        protected set
        {
            _vy = value;
            Velocity = Mathf.Sqrt(_vx * _vx + _vy * _vy);
        }
    }

    /// <summary>
    /// 速度方向
    /// </summary>
    public float VAngle
    {
        get { return _curVAngle; }
        protected set
        {
            _curVAngle = value;
            _reCalVAngle = false;
            Velocity = Mathf.Sqrt(_vx * _vx + _vy * _vy);
        }
    }

    /// <summary>
    /// 加速度
    /// </summary>
    public float Acce
    {
        get { return _curAcce; }
        protected set
        {
            _curAcce = value;
            _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
            _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
            _isMovingStraight = true;
        }
    }

    /// <summary>
    /// 加速度方向
    /// </summary>
    public float AccAngle
    {
        get { return _curAccAngle; }
        protected set
        {
            _curAccAngle = value;
            _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
            _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        }
    }

    /// <summary>
    /// 最大速度限制
    /// </summary>
    public float MaxVelocity
    {
        get
        {
            if (_maxVelocity < 0) return int.MaxValue;
            return _maxVelocity;
        }
        protected set
        {
            _maxVelocity = value;
            // 若value小于0，说明取消了速度限制，则直接返回
            if (value < 0) return;
            _sqrMaxV = value * value;
            // 判断现有速度是否超过了最大的速度限制
            float sqrV = _vx * _vx + _vy * _vy;
            if (sqrV > _sqrMaxV)
            {
                float factor = Mathf.Sqrt(_sqrMaxV / sqrV);
                _vx *= factor;
                _vy *= factor;
            }
        }
    }

    /// <summary>
    /// 极坐标运动的半径
    /// </summary>
    public float CurveRadius
    {
        get { return _curRadius; }
        protected set { _curRadius = value; }
    }

    /// <summary>
    /// 极坐标运动的半径增量
    /// </summary>
    public float CurveDeltaRadius
    {
        get { return _deltaRadius; }
        protected set { _deltaRadius = value; }
    }

    /// <summary>
    /// 极坐标运动的当前角度
    /// </summary>
    public float CurveAngle
    {
        get { return _curCurveAngle; }
        protected set { _curCurveAngle = value; }
    }

    /// <summary>
    /// 极坐标运动的角速度
    /// </summary>
    public float CurveOmega
    {
        get { return _curOmega; }
        set { _curOmega = value; }
    }
    #endregion
}
