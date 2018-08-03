using UnityEngine;
using System.Collections;

public class SpriteEffect : STGEffectBase
{
    private GameObject _effectGo;
    private Transform _effectTf;
    private SpriteRenderer _spRenderer;

    private float _curWidthScale;
    private float _curHeightScale;

    private bool _isScalingWidth;
    private int _scaleWidthTime;
    private int _scaleWidthDuration;
    private InterpolationMode _scaleWidthMode;
    private float _fromWidthScale;
    private float _toWidthScale;

    private bool _isScalingHeight;
    private int _scaleHeightTime;
    private int _scaleHeightDuration;
    private InterpolationMode _scaleHeightMode;
    private float _fromHeightScale;
    private float _toHeightScale;

    public override void Clear()
    {
        GameObject.Destroy(_effectGo);
        _effectGo = null;
        _effectTf = null;
        _spRenderer = null;
    }

    public override void Init()
    {
        base.Init();
        _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects","SpriteEffect");
        _effectTf = _effectGo.transform;
        _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        _isFinish = false;
        UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.GameEffect);
    }

    public override void SetToPos(float posX, float posY)
    {
        _effectTf.localPosition = new Vector2(posX, posY);
    }

    public void SetSize(float width,float height)
    {
        _curWidthScale = width / 256;
        _curHeightScale = height / 256;
        _effectTf.localScale = new Vector3(_curWidthScale, _curHeightScale, 1);
    }

    public override void Update()
    {
        if ( _isScalingWidth )
        {
            ScaleWidth();
        }
        if ( _isScalingHeight )
        {
            ScaleHeight();
        }
        if ( _isScalingWidth || _isScalingHeight )
        {
            _effectTf.localScale = new Vector3(_curWidthScale, _curHeightScale, 1);
        }
    }

    public void DoScaleWidth(float toScale,int duration,InterpolationMode scaleMode)
    {
        _fromWidthScale = _curWidthScale;
        _toWidthScale = toScale;
        _scaleWidthTime = 0;
        _scaleWidthDuration = duration;
        _scaleWidthMode = scaleMode;
        _isScalingWidth = true;
    }

    public void DoScaleHeight(float toScale, int duration, InterpolationMode scaleMode)
    {
        _fromHeightScale = _curWidthScale;
        _toHeightScale = toScale;
        _scaleHeightTime = 0;
        _scaleHeightDuration = duration;
        _scaleHeightMode = scaleMode;
        _isScalingHeight = true;
    }

    private void ScaleWidth()
    {
        _scaleWidthTime++;
        switch ( _scaleWidthMode )
        {
            case InterpolationMode.EaseInQuad:
                _curWidthScale = MathUtil.GetEaseInQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseOutQuad:
                _curWidthScale = MathUtil.GetEaseOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Linear:
                _curWidthScale = MathUtil.GetLinearInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseInOutQuad:
                _curWidthScale = MathUtil.GetEaseInOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Sin:
                _curWidthScale = MathUtil.GetSinInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
        }
        if ( _scaleWidthTime >= _scaleWidthDuration )
        {
            _isScalingWidth = false;
        }
    }

    private void ScaleHeight()
    {
        _scaleHeightTime++;
        switch (_scaleHeightMode)
        {
            case InterpolationMode.EaseInQuad:
                _curHeightScale = MathUtil.GetEaseInQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseOutQuad:
                _curHeightScale = MathUtil.GetEaseOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Linear:
                _curHeightScale = MathUtil.GetLinearInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseInOutQuad:
                _curHeightScale = MathUtil.GetEaseInOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Sin:
                _curHeightScale = MathUtil.GetSinInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
        }
        if (_scaleHeightTime >= _scaleHeightDuration)
        {
            _isScalingHeight = false;
        }
    }

    public void SetSprite(string spName)
    {
        _spRenderer.sprite = ResourceManager.GetInstance().GetSprite(Consts.EffectAtlasName, spName);
    }

    public void SetSpriteColor(float rValue,float gValue,float bValue,float aValue)
    {
        _spRenderer.material.color = new Color(rValue, gValue, bValue, aValue);
    }
}
