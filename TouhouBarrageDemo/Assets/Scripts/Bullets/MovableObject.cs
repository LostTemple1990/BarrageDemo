using UnityEngine;

public class MovableObject : IPoolClass
{
    private Vector2 _curPos;

    protected float _curVelocity;
    protected float _curVAngle;
    protected float _vx, _vy;
    protected float _curAcce;
    protected float _curAccAngle;
    protected float _dvx, _dvy;
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
    /// 标识当前是否在直线运动
    /// </summary>
    protected bool _isMovingStraight;
    /// <summary>
    /// 标识当前是否在曲线运动
    /// </summary>
    protected bool _isMovingCurve;
    protected bool _isMovingTo;

    float _dx, _dy;

    private MathUtil.InterpolationFloatFunc _moveFunc;
    private float _beginX, _beginY, _endX, _endY;
    private int _moveToTime;
    private int _moveToDuration;
    /// <summary>
    /// 是否在运动中
    /// </summary>
    private bool _isActive;
    /// <summary>
    /// 当前朝向
    /// </summary>
    private float _curRotation;

    /// <summary>
    /// 是否拥有额外速度
    /// </summary>
    private bool _hasExtraSpeed;
    /// <summary>
    /// x方向的额外速度
    /// </summary>
    private float _extraVelocityX;
    /// <summary>
    /// y方向的额外速度
    /// </summary>
    private float _extraVelocityY;
    /// <summary>
    /// x方向的额外加速度
    /// </summary>
    private float _extraAcceX;
    /// <summary>
    /// y方向的额外加速度
    /// </summary>
    private float _extraAcceY;
    /// <summary>
    /// 标识是否需要重新计算速度方向
    /// <para>当有加速度的时候一般都需要重新计算</para>
    /// </summary>
    private bool _reCalVAngle;
    /// <summary>
    /// 是否需要重新计算速度分量
    /// <para>默认为true</para>
    /// </summary>
    protected bool _reCalV;

    public void Update()
    {
        _dx = 0;
        _dy = 0;
        if ( _isMovingTo )
        {
            MovingTo();
        }
        else
        {
            if (_isMovingStraight)
            {
                MoveStraight();
            }
            if (_isMovingCurve)
            {
                MoveCurve();
            }
            // 计算额外速度分量
            if ( _hasExtraSpeed )
            {
                _dx += _extraVelocityX + _extraAcceX;
                _dy += _extraVelocityY + _extraAcceY;
            }
            _curPos.x += _dx;
            _curPos.y += _dy;
        }
        // 计算面向
        _curRotation = MathUtil.GetAngleBetweenXAxis(_dx, _dy);
        if ( !_isMovingTo && !_isMovingStraight && !_isMovingCurve && !_hasExtraSpeed )
        {
            _isActive = false;
        }
        ResetExtraSpeedParas();
    }

    #region 直线运动
    public virtual void DoStraightMove(float v, float angle)
    {
        if ( _maxVelocity > 0 )
        {
            _curVelocity = v > _maxVelocity ? _maxVelocity : v;
        }
        else
        {
            _curVelocity = v;
        }
        _curVAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = -1;
        _vx = _curVelocity * Mathf.Cos(_curVAngle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(_curVAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
        _isActive = true;
    }

    public virtual void MoveTowards(float v, float angle, int duration)
    {
        _curVelocity = v;
        _curVAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = duration;
        _vx = _curVelocity * Mathf.Cos(_curVAngle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(_curVAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
        _isActive = true;
    }

    public virtual void DoAcceleration(float acce, float accAngle)
    {
        _curAcce = acce;
        _curAccAngle = accAngle == Consts.VelocityAngle ? _curVAngle : accAngle;
        _maxVelocity = -1;
        // 计算速度增量
        _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
        _isActive = true;
    }

    public virtual void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity)
    {
        _curAcce = acce;
        _curAccAngle = accAngle == Consts.VelocityAngle ? _curVAngle : accAngle;
        _maxVelocity = maxVelocity;
        _sqrMaxV = _maxVelocity * _maxVelocity;
        // 计算速度增量
        _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
        _isActive = true;
    }

    public virtual void SetStraightParas(float v,float vAngle,float acce,float accAngle)
    {
        DoStraightMove(v, vAngle);
        DoAcceleration(acce, accAngle);
    }

    public virtual void MoveTo(float endX,float endY,int duration,InterpolationMode mode)
    {
        _beginX = _curPos.x;
        _beginY = _curPos.y;
        _endX = endX;
        _endY = endY;
        _moveToTime = 0;
        _moveToDuration = duration;
        _moveFunc = MathUtil.GetInterpolationFloatFunc(mode);
        _isMovingTo = true;
        _isActive = true;
    }

    protected void MovingTo()
    {
        _moveToTime++;
        // 临时记录
        _dx = _curPos.x;
        _dy = _curPos.y;
        // 计算新的位置
        _curPos.x = _moveFunc(_beginX, _endX, _moveToTime, _moveToDuration);
        _curPos.y = _moveFunc(_beginY, _endY, _moveToTime, _moveToDuration);
        if ( _moveToTime >= _moveToDuration )
        {
            _curPos.x = _endX;
            _curPos.y = _endY;
            _moveFunc = null;
            _isMovingTo = false;
        }
        // 计算位置增量
        _dx = _curPos.x - _dx;
        _dy = _curPos.y - _dy;
    }

    protected virtual void MoveStraight()
    {
        // 根据加速度计算新的速度
        if (_curAcce != 0)
        {
            _reCalVAngle = true;
            _vx += _dvx;
            _vy += _dvy;
            if (_maxVelocity > 0)
            {
                float value = _vx * _vx + _vy * _vy;
                if (value > _sqrMaxV)
                {
                    value = Mathf.Sqrt(_sqrMaxV / value);
                    _vx *= value;
                    _vy *= value;
                }
            }
        }
        _dx += _vx;
        _dy += _vy;
        if (_moveStraightDuration > 0)
        {
            _curStraightTime++;
            if (_curStraightTime >= _moveStraightDuration)
            {
                _curStraightTime = 0;
                _moveStraightDuration = 0;
                _isMovingStraight = false;
            }
        }
    }

    /// <summary>
    /// 设置额外的直线运动的参数
    /// <para>一般是被引力场影响</para>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public void AddExtraSpeedParas(float v, float angle, float acce, float accAngle)
    {
        _extraVelocityX += v * Mathf.Cos(angle * Mathf.Deg2Rad);
        _extraVelocityY += v * Mathf.Sin(angle * Mathf.Deg2Rad);
        _extraAcceX += acce * Mathf.Cos(accAngle * Mathf.Deg2Rad);
        _extraAcceY += acce * Mathf.Sin(accAngle * Mathf.Deg2Rad);
        _hasExtraSpeed = true;
    }

    public void ResetExtraSpeedParas()
    {
        _extraVelocityX = 0;
        _extraVelocityY = 0;
        _extraAcceX = 0;
        _extraAcceY = 0;
        _hasExtraSpeed = false;
    }
    #endregion

    #region 极坐标相关运动

    /// <summary>
    /// 做圆周运动，原点为初始位置
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="angle"></param>
    /// <param name="deltaR"></param>
    /// <param name="omega"></param>
    public virtual void SetPolarParas(float radius, float angle, float deltaR, float omega)
    {
        _centerPos = new Vector2(_curPos.x, _curPos.y);
        _curRadius = radius;
        _curCurveAngle = angle;
        _deltaRadius = deltaR;
        _curOmega = omega;
        _lastCurvePos = _centerPos;
        _isMovingCurve = true;
        _isActive = true;
    }

    protected virtual void MoveCurve()
    {
        _curRadius += _deltaRadius;
        _curCurveAngle += _curOmega;
        float dstX = _curRadius * Mathf.Cos(_curCurveAngle * Mathf.Deg2Rad) + _centerPos.x;
        float dstY = _curRadius * Mathf.Sin(_curCurveAngle * Mathf.Deg2Rad) + _centerPos.y;
        // 更新位置增量
        _dx += dstX - _lastCurvePos.x; ;
        _dy += dstY - _lastCurvePos.y;
        _lastCurvePos.x = dstX;
        _lastCurvePos.y = dstY;
    }
    #endregion

    public void Reset()
    {
        _moveFunc = null;
        _isMovingTo = false;
        _isMovingStraight = false;
        _isMovingCurve = false;
        _vx = _vy = _dvx = _dvy = 0;
        _maxVelocity = -1;
        _isActive = false;
        _curRotation = 0;
        _reCalVAngle = false;
        ResetExtraSpeedParas();
    }

    public void Reset(float x,float y)
    {
        Reset();
        SetPos(x, y);
    }

    public void SetPos(float x,float y)
    {
        _curPos.x = x;
        _curPos.y = y;
    }

    public Vector2 GetPos()
    {
        return _curPos;
    }

    #region 运动相关参数的get/set方法
    /// <summary>
    /// 速度
    /// </summary>
    public float Velocity
    {
        get { return Mathf.Sqrt(_vx * _vx + _vy * _vy); }
        set
        {
            if (_maxVelocity < 0)
            {
                _curVelocity = value;
            }
            else
            {
                if (value > _maxVelocity)
                {
                    _curVelocity = _maxVelocity;
                    _reCalV = true;
                }
                else
                {
                    _curVelocity = value;
                }
            }
            if (_reCalVAngle)
            {
                _curVAngle = MathUtil.GetAngleBetweenXAxis(_vx, _vy);
                _reCalVAngle = false;
            }
            if (_reCalV)
            {
                _vx = _curVelocity * Mathf.Cos(_curVAngle * Mathf.Deg2Rad);
                _vy = _curVelocity * Mathf.Sin(_curVAngle * Mathf.Deg2Rad);
            }
            _reCalV = true;
        }
    }

    /// <summary>
    /// x轴方向的速度
    /// </summary>
    public float Vx
    {
        get { return _vx; }
        set
        {
            _vx = value;
            _reCalV = false;
            _reCalVAngle = true;
            Velocity = Mathf.Sqrt(_vx * _vx + _vy * _vy);
        }
    }

    /// <summary>
    /// y轴方向的速度
    /// </summary>
    public float Vy
    {
        get { return _vy; }
        set
        {
            _vy = value;
            _reCalV = false;
            _reCalVAngle = true;
            Velocity = Mathf.Sqrt(_vx * _vx + _vy * _vy);
        }
    }

    /// <summary>
    /// 速度方向
    /// </summary>
    public float VAngle
    {
        get
        {
            if (_reCalVAngle)
            {
                _curVAngle = MathUtil.GetAngleBetweenXAxis(_vx, _vy);
                _reCalVAngle = false;
            }
            return _curVAngle;
        }
        set
        {
            _curVAngle = value;
            _reCalV = true;
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
        set
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
        set
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
            if (_maxVelocity<0) return int.MaxValue;
            return _maxVelocity;
        }
        set
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
        set { _curRadius = value; }
    }

    /// <summary>
    /// 极坐标运动的半径增量
    /// </summary>
    public float CurveDeltaRadius
    {
        get { return _deltaRadius; }
        set { _deltaRadius = value; }
    }

    /// <summary>
    /// 极坐标运动的当前角度
    /// </summary>
    public float CurveAngle
    {
        get { return _curCurveAngle; }
        set { _curCurveAngle = value; }
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

    /// <summary>
    /// 获取与上一帧的距离增量
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDeltaPos()
    {
        return new Vector2(_dx, _dy);
    }

    public float dx
    {
        get { return _dx; }
    }

    public float dy
    {
        get { return _dy; }
    }

    /// <summary>
    /// 当前是否激活
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return _isActive;
    }

    public float Rotation
    {
        get { return _curRotation; }
    }

    public virtual void Clear()
    {
        Reset();
    }
}
