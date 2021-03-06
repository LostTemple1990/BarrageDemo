﻿using UnityEngine;

public class TweenRotation : TweenBase
{
    private bool _isCache = false;
    private Vector3 _start;
    private Vector3 _end;
    private Transform _tf;

    private void Cache()
    {
        _isCache = true;
        _tf = _tweenGo.transform;
        _start = _tf.localEulerAngles;
    }

    public void SetParas(Vector3 end, InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _end = end;
        SetInterpolationMode(mode);
    }

    public void SetParas(Vector3 start, Vector2 end, InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _start = start;
        _end = end;
        SetInterpolationMode(mode);
    }

    protected override void OnUpdate(float interpolationValue)
    {
        Vector3 value = Vector3.Lerp(_start, _end, interpolationValue);
        _tf.localEulerAngles = value;
    }

    public override void RestoreToPool()
    {
        ObjectsPool.GetInstance().RestorePoolClassToPool<TweenRotation>(this);
    }

    public override void Clear()
    {
        _isCache = false;
        _tf = null;
        base.Clear();
    }
}
