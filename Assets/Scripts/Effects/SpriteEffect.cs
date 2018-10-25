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
    /// <summary>
    /// 标识显示对象是否缓存在ObjectsPool中
    /// </summary>
    private bool _isUsingCache;
    /// <summary>
    /// 缓存名称标识
    /// <para>"SpriteEffect_"+AtlasName+"_"+SpriteName</para>
    /// </summary>
    private string _effectGoName;

    /// <summary>
    /// 是否正在渐隐消失
    /// </summary>
    private bool _isFading;
    /// <summary>
    /// 渐隐时间
    /// </summary>
    private int _fadeTime;
    /// <summary>
    /// 渐隐总时长
    /// </summary>
    private int _fadeDuration;
    private Color _spriteColor;
    /// <summary>
    /// 渐隐起始时候的alpha
    /// </summary>
    private float _fadeBeginAlhpa;
    /// <summary>
    /// 在层级的顺序
    /// </summary>
    private int _orderInLayer;

    private float _curPosX;
    private float _curPosY;

    public override void Clear()
    {
        if ( _isUsingCache )
        {
            _effectTf.localScale = Vector3.one;
            _spRenderer.color = new Color(1, 1, 1, 1);
            ObjectsPool.GetInstance().RestorePrefabToPool(_effectGoName, _effectGo);
        }
        else
        {
            GameObject.Destroy(_effectGo);
        }
        _effectGo = null;
        _effectTf = null;
        _spRenderer = null;
    }

    public override void Init()
    {
        base.Init();
        _isFinish = false;
        _isUsingCache = false;
        _isFading = false;
        _orderInLayer = 0;
    }

    public override void SetToPos(float posX, float posY)
    {
        _curPosX = posX;
        _curPosY = posY;
        _effectTf.localPosition = new Vector3(posX, posY, -_orderInLayer);
    }

    public void SetScale(float scaleX,float scaleY)
    {
        _curWidthScale = scaleX;
        _curHeightScale = scaleY;
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
        if ( _isFading )
        {
            Fade();
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

    public void DoFade(int duration)
    {
        _isFading = true;
        _fadeTime = 0;
        _fadeDuration = duration;
        _spriteColor = _spRenderer.material.color;
        _fadeBeginAlhpa = _spriteColor.a;
    }

    private void Fade()
    {
        _fadeTime++;
        if ( _fadeTime < _fadeDuration )
        {
            _spriteColor.a = Mathf.Lerp(_fadeBeginAlhpa, 0, (float)_fadeTime / _fadeDuration);
            _spRenderer.material.color = _spriteColor;
        }
        else
        {
            _isFading = false;
            _isFinish = true;
        }
    }

    public void SetSprite(string spName)
    {
        _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
        _effectTf = _effectGo.transform;
        _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        _spRenderer.sprite = ResourceManager.GetInstance().GetSprite(Consts.EffectAtlasName, spName);
        _isUsingCache = false;
        UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.STGNormalEffect);
    }

    public void SetSprite(string atlasName,string spName,bool isUsingCache=false)
    {
        _isUsingCache = isUsingCache;
        // 不使用缓存，直接创建
        if ( !isUsingCache)
        {
            _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
            _effectTf = _effectGo.transform;
            _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
            _spRenderer.sprite = ResourceManager.GetInstance().GetSprite(atlasName, spName);
        }
        else
        {
            _effectGoName = "SpriteEffect_" + atlasName + "_" + spName;
            _effectGo = ObjectsPool.GetInstance().GetPrefabAtPool(_effectGoName);
            if ( _effectGo == null )
            {
                GameObject protoType = ObjectsPool.GetInstance().GetProtoType(_effectGoName);
                if ( protoType == null )
                {
                    protoType = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
                    protoType.name = _effectGoName;
                    Transform tf = protoType.transform;
                    tf.localPosition = new Vector3(2000, 2000, 0);
                    SpriteRenderer sr = tf.Find("Sprite").GetComponent<SpriteRenderer>();
                    sr.sprite = ResourceManager.GetInstance().GetSprite(atlasName, spName);
                    UIManager.GetInstance().AddGoToLayer(protoType, LayerId.STGNormalEffect);
                    ObjectsPool.GetInstance().AddProtoType(_effectGoName, protoType);
                }
                _effectGo = GameObject.Instantiate<GameObject>(protoType);
                UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.STGNormalEffect);
            }
            _effectTf = _effectGo.transform;
            _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        }
    }

    /// <summary>
    /// 设置特效所在的层级
    /// </summary>
    /// <param name="layerId"></param>
    public void SetLayer(LayerId layerId)
    {
        UIManager.GetInstance().AddGoToLayer(_effectGo, layerId);
    }

    public void SetOrderInLayer(int orderInLayer)
    {
        _orderInLayer = orderInLayer;
        _effectTf.localPosition = new Vector3(_curPosX, _curPosY, -orderInLayer);
    }

    public void SetSpriteColor(float rValue,float gValue,float bValue,float aValue)
    {
        _spRenderer.material.color = new Color(rValue, gValue, bValue, aValue);
    }
}
