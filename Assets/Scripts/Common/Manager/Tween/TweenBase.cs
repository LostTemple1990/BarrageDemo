using UnityEngine;
using System.Collections;

public class TweenBase
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
    /// 起始时间
    /// </summary>
    protected int _startTime;
    /// <summary>
    /// 结束时间
    /// </summary>
    protected int _endTime;
    /// <summary>
    /// 持续时间
    /// </summary>
    protected int _duration;

    public delegate void TweenFinishCallBack(GameObject go);

    protected GameObject _tweenGo;
    protected eUIType _uiType;
    protected TweenFinishCallBack _callBack;

    public TweenBase()
    {
        _isFinish = false;
    }

    public virtual void SetParas(GameObject go,eUIType uiType,int startTime,int endTime,TweenFinishCallBack callBack)
    {
        _tweenGo = go;
        _uiType = uiType;
        _startTime = startTime;
        _endTime = endTime;
        _duration = _endTime - _startTime;
        _callBack = callBack;
    }

    public void Update()
    {
        if ( _curTime >= _startTime && _curTime <= _endTime )
        {
            OnUpdate();
        }
        _curTime++;
        if ( _curTime >= _endTime )
        {
            _isFinish = true;
        }
    }

    protected virtual void OnUpdate()
    {

    }

    public bool IsFinish
    {
        get { return _isFinish; }
    }

    public virtual void Clear()
    {
        _tweenGo = null;
        _callBack = null;
    }
}
