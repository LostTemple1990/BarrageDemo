using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenPos2D : TweenBase
{
    private bool _isCache = false;
    private Vector2 _start;
    private Vector2 _end;
    private Transform _tf;

    private void Cache()
    {
        _isCache = true;
        _tf = _tweenGo.transform;
        _start = _tf.localPosition;
    }

    public void SetParas(Vector2 end, InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _end = end;
        SetInterpolationMode(mode);
    }

    public void SetParas(Vector2 start, Vector2 end, InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _start = start;
        _end = end;
        SetInterpolationMode(mode);
    }

    protected override void OnUpdate(float interpolationValue)
    {
        Vector2 value = Vector2.Lerp(_start, _end, interpolationValue);
        _tf.localPosition = value;
    }

    public override void RestoreToPool()
    {
        ObjectsPool.GetInstance().RestorePoolClassToPool<TweenPos2D>(this);
    }

    public override void Clear()
    {
        _tf = null;
        base.Clear();
    }
}
