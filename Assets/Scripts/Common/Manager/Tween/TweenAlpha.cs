using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenAlpha : TweenBase
{
    private bool _isCache = false;
    private float _startAlpha;
    private float _endAlpha;
    private Graphic _grahpic;
    private SpriteRenderer _spRenderer;
    private Material _material;

    private void Cache()
    {
        _isCache = true;
        _grahpic = _tweenGo.GetComponent<Graphic>();
        if (_grahpic != null)
        {
            _startAlpha = _grahpic.color.a;
            return;
        }
        _spRenderer = _tweenGo.GetComponent<SpriteRenderer>();
        if (_spRenderer != null)
        {
            _startAlpha = _spRenderer.color.a;
            return;
        }
        Renderer renderer = _tweenGo.GetComponent<Renderer>();
        _material = renderer.material;
        if (_material != null)
        {
            _startAlpha = _material.color.a;
        }
    }

    public void SetParas(float endAlpha,InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _endAlpha = endAlpha;
        SetInterpolationMode(mode);
    }

    public void SetParas(float startAlpha,float endAlpha,InterpolationMode mode)
    {
        if (!_isCache) Cache();
        _startAlpha = startAlpha;
        _endAlpha = endAlpha;
        SetInterpolationMode(mode);
    }

    protected override void OnUpdate(float interpolationValue)
    {
        float value = Mathf.Lerp(_startAlpha, _endAlpha, interpolationValue);
        if ( _grahpic != null )
        {
            Color c = _grahpic.color;
            c.a = value;
            _grahpic.color = c;
        }
        else if ( _spRenderer != null )
        {
            Color c = _spRenderer.color;
            c.a = value;
            _spRenderer.color = c;
        }
        else if ( _material != null )
        {
            Color c = _material.color;
            c.a = value;
            _material.color = c;
        }
    }

    public override void RestoreToPool()
    {
        ObjectsPool.GetInstance().RestorePoolClassToPool<TweenAlpha>(this);
    }

    public override void Clear()
    {
        _isCache = false;
        _grahpic = null;
        _spRenderer = null;
        _material = null;
        base.Clear();
    }
}
