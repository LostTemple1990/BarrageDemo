using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenAnchoredPos : TweenBase
{
    private bool _isCache = false;
    private Vector2 _start;
    private Vector2 _end;
    private RectTransform _tf;

    private void Cache()
    {
        _isCache = true;
        _tf = _tweenGo.GetComponent<RectTransform>();
        _start = _tf.anchoredPosition;
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
        _tf.anchoredPosition = value;
    }

    public override void RestoreToPool()
    {
        ObjectsPool.GetInstance().RestorePoolClassToPool<TweenAnchoredPos>(this);
    }

    public override void Clear()
    {
        _isCache = false;
        _tf = null;
        base.Clear();
    }
}
