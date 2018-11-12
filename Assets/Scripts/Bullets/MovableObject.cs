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

    protected bool _isMovingStraight;
    protected bool _isMovingCurve;
    protected bool _isMovingTo;

    float _dx, _dy;

    private delegate float InterpolationFunc(float begin, float end, float time, float duration);
    private InterpolationFunc _moveFunc;
    private float _beginX, _beginY, _endX, _endY;
    private int _moveToTime;
    private int _moveToDuration;

    public void Update()
    {
        if ( _isMovingTo )
        {
            MoveTo();
        }
        else
        {
            _dx = 0;
            _dy = 0;
            if (_isMovingStraight)
            {
                MoveStraight();
            }
            if (_isMovingCurve)
            {
                MoveCurve();
            }
            _curPos.x += _dx;
            _curPos.y += _dy;
        }
    }

    #region 直线运动
    public virtual void DoMoveStraight(float v, float angle)
    {
        _curVelocity = v;
        _curVAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = Consts.MaxDuration;
        _vx = _curVelocity * Mathf.Cos(_curVAngle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(_curVAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    public virtual void DoMoveStraightWithLimitation(float v, float angle, int duration)
    {
        _curVelocity = v;
        _curVAngle = angle;
        _curStraightTime = 0;
        _moveStraightDuration = duration;
        _vx = _curVelocity * Mathf.Cos(_curVAngle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(_curVAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    public virtual void DoAcceleration(float acce, float accAngle)
    {
        _curAcce = acce;
        _curAccAngle = accAngle == Consts.VelocityAngle ? _curVAngle : accAngle;
        _accTime = 0;
        _accDuration = Consts.MaxDuration;
        // 计算速度增量
        _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    public virtual void DoAccelerationWithLimitation(float acce, float accAngle, int accDuration)
    {
        _curAcce = acce;
        _curAccAngle = accAngle == Consts.VelocityAngle ? _curVAngle : accAngle;
        _accTime = 0;
        _accDuration = accDuration;
        // 计算速度增量
        _dvx = _curAcce * Mathf.Cos(_curAccAngle * Mathf.Deg2Rad);
        _dvy = _curAcce * Mathf.Sin(_curAccAngle * Mathf.Deg2Rad);
        _isMovingStraight = true;
    }

    public virtual void DoMoveTo(float endX,float endY,int duration,InterpolationMode mode)
    {
        _beginX = _curPos.x;
        _beginY = _curPos.y;
        _endX = endX;
        _endY = endY;
        _moveToTime = 0;
        _moveToDuration = duration;
        switch ( mode )
        {
            case InterpolationMode.None:
                _moveFunc = MathUtil.GetNoneInterpolation;
                break;
            case InterpolationMode.Linear:
                _moveFunc = MathUtil.GetLinearInterpolation;
                break;
            case InterpolationMode.EaseInQuad:
                _moveFunc= MathUtil.GetEaseInQuadInterpolation;
                break;
            case InterpolationMode.EaseOutQuad:
                _moveFunc = MathUtil.GetEaseOutQuadInterpolation;
                break;
            case InterpolationMode.EaseInOutQuad:
                _moveFunc = MathUtil.GetEaseInOutQuadInterpolation;
                break;
            case InterpolationMode.Sin:
                _moveFunc = MathUtil.GetSinInterpolation;
                break;
        }
        _isMovingTo = true;
    }

    protected void MoveTo()
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
            _vx += _dvx;
            _vy += _dvy;
            _accTime++;
            if (_accTime >= _accDuration)
            {
                _curAcce = 0;
                _dvx = _dvy = 0;
            }
        }
        if (_moveStraightDuration > 0)
        {
            _dx += _vx;
            _dy += _vy;
            _curStraightTime++;
            if (_curStraightTime >= _moveStraightDuration)
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

    /// <summary>
    /// 获取直线运动的角度
    /// </summary>
    /// <returns></returns>
    public float GetVAngle()
    {
        return _curVAngle;
    }

    /// <summary>
    /// 获取与上一帧的距离增量
    /// </summary>
    /// <returns></returns>
    public Vector2 GetDeltaPos()
    {
        return new Vector2(_dx, _dy);
    }

    public virtual void Clear()
    {
        Reset();
    }
}
