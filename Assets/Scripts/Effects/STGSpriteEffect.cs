using UnityEngine;
using System.Collections;

public class STGSpriteEffect : STGEffectBase
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

    private bool _isRotating;
    /// <summary>
    /// 每帧旋转的角度
    /// </summary>
    private float _rotateAngle;
    /// <summary>
    /// 旋转时间
    /// </summary>
    private int _rotateTime;
    /// <summary>
    /// 旋转持续时间
    /// </summary>
    private int _rotateDuration;
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
    /// <summary>
    /// 是否在移动
    /// </summary>
    private bool _isMoving;
    /// <summary>
    /// 移动速度
    /// </summary>
    private float _velocity;
    /// <summary>
    /// 速度的角度
    /// </summary>
    private float _moveAngle;
    /// <summary>
    /// 移动加速度
    /// </summary>
    private float _moveAcce;
    /// <summary>
    /// 位移增量
    /// </summary>
    private float _dx, _dy;
    /// <summary>
    /// 速度增量
    /// </summary>
    private float _dvx, _dvy;

    /// <summary>
    /// 是否正在做alhpa渐变
    /// </summary>
    private bool _isDoingTweenAlhpa;
    private float _startAlhpa;
    private float _endAlpha;
    private int _tweenAlphaTime;
    private int _tweenAlphaDuration;

    private int _existDuration;

    public STGSpriteEffect()
    {
        _effectType = EffectType.SpriteEffect;
    }

    public override void Clear()
    {
        if ( _isUsingCache )
        {
            _effectTf.localScale = Vector3.one;
            _effectTf.localRotation = Quaternion.Euler(0, 0, 0);
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
        _isScalingWidth = false;
        _isScalingHeight = false;
        _isRotating = false;
        _isMoving = false;
        _isDoingTweenAlhpa = false;
        _existDuration = -1;
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
        if ( _isRotating )
        {
            Rotate();
        }
        if ( _isDoingTweenAlhpa )
        {
            UpdateTweenAlpha();
        }
        if ( _isMoving )
        {
            Move();
        }
        if ( _existDuration > 0 )
        {
            _existDuration--;
            if ( _existDuration == 0 )
            {
                _isFinish = true;
            }
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

    /// <summary>
    /// 执行alhpa缓动
    /// </summary>
    /// <param name="startAlhpa">起始透明度</param>
    /// <param name="endAlpha">结束透明度</param>
    /// <param name="duration">持续时间</param>
    public void DoTweenAlpha(float startAlhpa,float endAlpha,int duration)
    {
        _spriteColor = _spRenderer.material.color;
        _startAlhpa = startAlhpa;
        _endAlpha = endAlpha;
        _tweenAlphaTime = 0;
        _tweenAlphaDuration = duration;
        _isDoingTweenAlhpa = true;
    }

    /// <summary>
    /// 执行alpha缓动
    /// <para>起始透明度默认为当前透明度</para>
    /// </summary>
    /// <param name="endAlpha">结束透明度</param>
    /// <param name="duration">持续时间</param>
    public void DoTweenAlpha(float endAlpha,int duration)
    {
        _spriteColor = _spRenderer.material.color;
        _startAlhpa = _spriteColor.a;
        _endAlpha = endAlpha;
        _tweenAlphaTime = 0;
        _tweenAlphaDuration = duration;
        _isDoingTweenAlhpa = true;
    }

    /// <summary>
    /// 更新alpha缓动
    /// </summary>
    private void UpdateTweenAlpha()
    {
        _tweenAlphaTime++;
        if (_tweenAlphaTime < _tweenAlphaDuration)
        {
            _spriteColor.a = Mathf.Lerp(_startAlhpa, _endAlpha, (float)_tweenAlphaTime / _tweenAlphaDuration);
            _spRenderer.color = _spriteColor;
        }
        else
        {
            _isDoingTweenAlhpa = false;
        }
    }

    public void SetExistDuration(int duration)
    {
        _existDuration = duration;
    }

    /// <summary>
    /// 渐隐消失
    /// </summary>
    /// <param name="duration"></param>
    public void DoFade(int duration)
    {
        DoTweenAlpha(0, duration);
        SetExistDuration(duration);
    }

    public void DoMove(float v,float angle,float acce)
    {
        _velocity = v;
        _moveAngle = angle;
        _moveAcce = acce;
        // 计算参数
        float sin = Mathf.Sin(Mathf.Deg2Rad * angle);
        float cos = Mathf.Cos(Mathf.Deg2Rad * angle);
        _dx = v * cos;
        _dy = v * sin;
        _dvx = acce * cos;
        _dvy = acce * sin;
        // 设置移动状态
        _isMoving = true;
    }

    public void StopMove()
    {
        _isMoving = false;
    }

    private void Move()
    {
        // X方向的位移增量
        _dx += _dvx;
        _curPosX += _dx;
        // Y方向的位移增量
        _dy += _dvy;
        _curPosY += _dy;
        // 设置位置
        _effectTf.localPosition = new Vector3(_curPosX, _curPosY, -_orderInLayer);
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

    public void SetSprite(string atlasName,string spName,eBlendMode blendMode=eBlendMode.Normal,LayerId layerId=LayerId.STGNormalEffect,bool isUsingCache=false)
    {
        _isUsingCache = isUsingCache;
        // 不使用缓存，直接创建
        if ( !isUsingCache)
        {
            _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
            _effectTf = _effectGo.transform;
            _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
            if ( blendMode != eBlendMode.Normal )
            {
                _spRenderer.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
            }
            _spRenderer.sprite = ResourceManager.GetInstance().GetSprite(atlasName, spName);
            UIManager.GetInstance().AddGoToLayer(_effectGo, layerId);
        }
        else
        {
            _effectGoName = "SpriteEffect_" + atlasName + "_" + spName + "_" + blendMode;
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
                    if (blendMode != eBlendMode.Normal)
                    {
                        sr.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
                    }
                    sr.sprite = ResourceManager.GetInstance().GetSprite(atlasName, spName);
                    UIManager.GetInstance().AddGoToLayer(protoType, LayerId.STGNormalEffect);
                    ObjectsPool.GetInstance().AddProtoType(_effectGoName, protoType);
                }
                _effectGo = GameObject.Instantiate<GameObject>(protoType);
                UIManager.GetInstance().AddGoToLayer(_effectGo, layerId);
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

    /// <summary>
    /// 设置透明度
    /// </summary>
    /// <param name="alpha"></param>
    public void SetSpritAlpha(float alpha)
    {
        Color color = _spRenderer.color;
        color.a = alpha;
        _spRenderer.color = color;
    }

    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="rValue"></param>
    /// <param name="gValue"></param>
    /// <param name="bValue"></param>
    /// <param name="aValue"></param>
    public void SetSpriteColor(float rValue,float gValue,float bValue,float aValue)
    {
        _spRenderer.material.color = new Color(rValue, gValue, bValue, aValue);
    }

    public void SetRotation(float angle)
    {
        _effectTf.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void DoRotation(float rotateAngle,int duration)
    {
        _rotateAngle = rotateAngle;
        _rotateTime = 0;
        _rotateDuration = duration;
        _isRotating = true;
    }

    private void Rotate()
    {
        _rotateTime++;
        _effectTf.Rotate(0, 0, _rotateAngle);
        if ( _rotateTime >= _rotateAngle )
        {
            _isRotating = false;
        }
    }

    public override bool NeedToBeRestoredToPool()
    {
        return true;
    }
}
