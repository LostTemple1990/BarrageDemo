using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenColor : TweenBase
{
    private bool _isCache = false;
    private Color _start;
    private Color _end;
    private Graphic _grahpic;
    private SpriteRenderer _spRenderer;
    private Material _material;

    private void Cache()
    {
        _isCache = true;
        _grahpic = _tweenGo.GetComponent<Graphic>();
        if (_grahpic != null)
        {
            _start = _grahpic.color;
            return;
        }
        _spRenderer = _tweenGo.GetComponent<SpriteRenderer>();
        if (_spRenderer != null)
        {
            _start = _spRenderer.color;
            return;
        }
        Renderer renderer = _tweenGo.GetComponent<Renderer>();
        _material = renderer.material;
        if (_material != null)
        {
            _start = _material.color;
        }
    }

    public void SetParas(Color endColor, InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _end = endColor;
        SetInterpolationMode(mode);
    }

    public void SetParas(Color startColor, Color endColor, InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _start = startColor;
        _end = endColor;
        SetInterpolationMode(mode);
    }

    protected override void OnUpdate(float interpolationValue)
    {
        Color value = Color.Lerp(_start, _end, interpolationValue);
        if (_grahpic != null)
        {
            _grahpic.color = value;
        }
        else if (_spRenderer != null)
        {
            _spRenderer.color = value;
        }
        else if (_material != null)
        {
            _material.color = value;
        }
    }

    public override void RestoreToPool()
    {
        ObjectsPool.GetInstance().RestorePoolClassToPool<TweenColor>(this);
    }

    public override void Clear()
    {
        _grahpic = null;
        _spRenderer = null;
        _material = null;
        base.Clear();
    }
}
