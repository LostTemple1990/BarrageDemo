using UnityEngine;
using System.Collections;

public class TweenBase : IPoolClass
{
    /// <summary>
    /// 当前时间
    /// </summary>
    protected int _curTime;
    /// <summary>
    /// 是否已经完成
    /// </summary>
    protected bool _isFinish;
    /// <summary>
    /// 持续时间
    /// </summary>
    protected int _duration;
    /// <summary>
    /// 延迟执行的时间
    /// </summary>
    protected int _delay;
    /// <summary>
    /// 是否在暂停的时候继续执行
    /// </summary>
    protected bool _ignoreTimeScale;
    /// <summary>
    /// 系数每次执行的增量
    /// </summary>
    protected float _dFactor;
    /// <summary>
    /// 系数
    /// </summary>
    protected float _factor;
    /// <summary>
    /// 播放模式
    /// </summary>
    protected ePlayMode _playMode;
    /// <summary>
    /// 插值方式
    /// </summary>
    protected InterpolationMode _interpolationMode;
    /// <summary>
    /// 插值函数
    /// </summary>
    protected MathUtil.InterpolationFloatFunc _interpolationFunc;

    protected bool _isEnable;

    public delegate void TweenFinishCallBack(GameObject go);

    protected GameObject _tweenGo;
    protected TweenFinishCallBack _finishCallBack;

    public TweenBase()
    {
        _ignoreTimeScale = true;
        _isFinish = false;
        _isEnable = true;
    }

    public void SetParas(GameObject go,int delay,int duration,ePlayMode playMode)
    {
        _tweenGo = go;
        _delay = delay;
        _duration = duration;
        _curTime = 0;
        _dFactor = duration <= 0 ? 1f :1f / _duration;
        _playMode = playMode;
        // 初始化
        _factor = 0f;
        _curTime = 0;
        _isFinish = false;
        _isEnable = true;
    }

    /// <summary>
    /// 设置播放完成回调
    /// </summary>
    /// <param name="callBack"></param>
    public void SetFinishCallBack(TweenFinishCallBack callBack)
    {
        _finishCallBack = callBack;
    }

    public void Update()
    {
        if (!_isEnable) return;
        // TODO 稍后加入游戏暂停处理
        if ( _curTime < _delay )
        {
            _curTime++;
            return;
        }
        _factor += _dFactor;
        // 循环播放
        if ( _playMode == ePlayMode.Loop )
        {
            if ( _factor > 1f )
            {
                _factor = _factor - Mathf.Floor(_factor);
            }
        }
        // 乒乓播放
        else if ( _playMode == ePlayMode.PingPong )
        {
            if ( _factor > 1f )
            {
                _factor = 1f - (_factor - Mathf.Floor(_factor));
                _dFactor = -_dFactor;
            }
            else if ( _factor < 0f )
            {
                _factor = -_factor;
                _factor = _factor - Mathf.Floor(_factor);
                _dFactor = -_dFactor;
            }
        }

        // 判断是否已经播放完成
        if ( _playMode == ePlayMode.Once && (_duration == 0f || _factor >= 1f) )
        {
            _factor = 1f;
            OnUpdate(GetInterpolationValue(_factor));
            _isFinish = true;
            if ( _finishCallBack != null )
            {
                _finishCallBack(_tweenGo);
            }
        }
        else
        {
            OnUpdate(GetInterpolationValue(_factor));
        }
        _curTime++;
    }

    /// <summary>
    /// 设置插值的方式
    /// </summary>
    /// <param name="mode"></param>
    protected void SetInterpolationMode(InterpolationMode mode)
    {
        _interpolationMode = mode;
        _interpolationFunc = MathUtil.GetInterpolationFloatFunc(mode);
    }
    
    /// <summary>
    /// 获取插值
    /// </summary>
    /// <param name="factor"></param>
    /// <returns></returns>
    protected float GetInterpolationValue(float factor)
    {
        return _interpolationFunc(0, 1, factor, 1);
    }

    protected virtual void OnUpdate(float interpolationValue)
    {

    }

    public GameObject GetTweenObject()
    {
        return _tweenGo;
    }

    public bool IsFinish
    {
        get { return _isFinish; }
    }

    public int delay
    {
        get { return _delay; }
    }

    public int duration
    {
        get { return _duration; }
    }

    public virtual void SetStartToCurrentValue() { }

    public virtual void SetEndToCurrentValue() { }

    public void SetIgnoreTimeScale(bool value)
    {
        _ignoreTimeScale = value;
    }

    public virtual void RestoreToPool()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Clear()
    {
        _isEnable = false;
        _isFinish = true;
        _tweenGo = null;
        _finishCallBack = null;
    }
}
