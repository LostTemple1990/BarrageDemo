using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STGChargeEffect : STGEffectBase
{
    /// <summary>
    /// 默认尺寸
    /// </summary>
    private const float DefaultSize = 256;
    private const int CircleShrinkDuration = 60;

    private const float LimitMinRadius = 300;
    private const float LimitMaxRadius = 500;

    private const int ChargeMinDuration = 50;
    private const int ChargeMaxDuration = 80;

    private const float InitCircleScale = 1.5f;

    private List<ChargeObject> _chargeList;
    private int _chargeCount;
    /// <summary>
    /// 特效的容器tf
    /// </summary>
    private Transform _effectContainerTf;
    private Transform _circleTf;
    private SpriteRenderer _circleSpRenderer;

    private bool _isCircleShrinking;
    private int _circleShrinkTime;
    private int _chargeTime;
    /// <summary>
    /// 是否已经初始化了枫叶对象
    /// </summary>
    private bool _isInitChargeObjects;
    private Vector2 _curPos;

    public STGChargeEffect()
    {
        _effectType = EffectType.ChargeEffect;
    }

    public override void Init()
    {
        base.Init();
        // 容器
        if (_effectContainerTf == null)
        {
            _effectContainerTf = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "STGEffectContainer").transform;
            UIManager.GetInstance().AddGoToLayer(_effectContainerTf.gameObject, LayerId.STGNormalEffect);
        }
        // 收缩的圆形
        if (_circleTf == null)
        {
            _circleTf = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect").transform;
            _circleTf.SetParent(_effectContainerTf, false);
            _circleTf.localPosition = Vector3.zero;
            _circleTf.localScale = new Vector3(InitCircleScale, InitCircleScale, 1);
            _circleSpRenderer = _circleTf.Find("Sprite").GetComponent<SpriteRenderer>();
            _circleSpRenderer.sprite = ResourceManager.GetInstance().GetSprite("ShapeAtlas", "ShapeCircle");
            _circleSpRenderer.color = new Color(0.9f, 0.1f, 0.1f, 0.5f);
        }
        _circleShrinkTime = 0;
        _isCircleShrinking = true;
        _chargeTime = 0;
        _isInitChargeObjects = false;
        _curPos = Vector2.zero;
        SoundManager.GetInstance().Play("castcharge", 0.1f, false, true);
    }

    private void InitChargeObjects()
    {
        _chargeList = new List<ChargeObject>();
        _chargeCount = 50;
        for (int i = 0; i < _chargeCount; i++)
        {
            ChargeObject chargeObj = new ChargeObject();
            chargeObj.Init();
            Vector3 startPos = GetRandomStartPos();
            Vector3 rotateAngle = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 2f));
            int easeDuration = Random.Range(ChargeMinDuration, ChargeMaxDuration);
            chargeObj.SetParas(startPos, _curPos, rotateAngle, easeDuration);
            _chargeList.Add(chargeObj);
        }
        _isInitChargeObjects = true;
    }

    public override void Update()
    {
        if (!_isInitChargeObjects)
        {
            InitChargeObjects();
            return;
        }
        _chargeTime++;
        if (_isCircleShrinking)
        {
            UpdateCircleTf();
        }
        UpdateChargeObjects();
        if (_chargeTime >= ChargeMaxDuration)
        {
            _isFinish = true;
        }
    }

    private void UpdateCircleTf()
    {
        _circleShrinkTime++;
        if (_circleShrinkTime <= CircleShrinkDuration)
        {
            float factor = MathUtil.GetEaseInQuadInterpolation(0, 1, _circleShrinkTime, CircleShrinkDuration);
            float scale = Mathf.Lerp(InitCircleScale, 0, factor);
            _circleTf.localScale = new Vector3(scale, scale, 1);
            float alpha = Mathf.Lerp(0.5f, 0.1f, factor);
            _circleSpRenderer.color = new Color(0.9f, 0.1f, 0.1f, alpha);
        }
        else
        {
            _circleTf.gameObject.SetActive(false);
            _isCircleShrinking = false;
        }
    }

    private void UpdateChargeObjects()
    {
        foreach (ChargeObject co in _chargeList)
        {
            if ( !co.isComplete )
            {
                co.Update();
            }
        }
    }

    /// <summary>
    /// 根据限定的范围求出一个随机的出生点
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomStartPos()
    {
        float angle = Random.Range(0f, 360f);
        float radius = Random.Range(LimitMinRadius, LimitMaxRadius);
        float posX = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float posY = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        return new Vector3(posX + _curPos.x, posY + _curPos.y, 0);
    }

    public override void SetPosition(float posX, float posY)
    {
        _curPos = new Vector2(posX, posY);
        _effectContainerTf.localPosition = new Vector3(posX, posY, 0);
    }

    public void SetScale(float scale)
    {
        _effectContainerTf.localScale = new Vector3(scale, scale, 1);
    }

    public override void Clear()
    {
        int i;
        ChargeObject bo;
        for (i = 0; i < _chargeCount; i++)
        {
            bo = _chargeList[i];
            bo.Clear();
        }
        _chargeList.Clear();
        _chargeCount = 0;
        GameObject.Destroy(_effectContainerTf.gameObject);
    }
}

class ChargeObject
{
    /// <summary>
    /// 蓄力物体的初始scale
    /// </summary>
    private const float InitScaleRangeMin = 3.5f;
    private const float InitScaleRangeMax = 6f;

    private const float EndScaleRangeMin = 2f;
    private const float EndScaleRangeMax = 3f;


    public GameObject go;
    public Transform tf;
    public SpriteRenderer spRenderer;
    public int time;
    public int duration;
    float startScale;
    float toScale;
    Vector3 rotateAngle;
    public int state;
    public float dAlpha;
    public Color tmpColor;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool isComplete;

    public void Init()
    {
        go = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "EffectMapleLeaf0");
        tf = go.transform;
        spRenderer = tf.Find("Sprite").GetComponent<SpriteRenderer>();
        UIManager.GetInstance().AddGoToLayer(go, LayerId.STGNormalEffect);
        tf.localPosition = Vector3.zero;
        // 随机方向
        tf.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        // 一定的透明度
        tmpColor = spRenderer.color;
        tmpColor.a = 0.3f;
        spRenderer.color = tmpColor;
        // 初始scale
        float scale = Random.Range(InitScaleRangeMin, InitScaleRangeMax);
        tf.localScale = new Vector3(scale, scale, 1);
        startScale = scale;
        toScale = Random.Range(EndScaleRangeMin, EndScaleRangeMax);
        go.SetActive(true);
        isComplete = false;
    }

    public void SetParas(Vector3 startPos,Vector3 endPos,Vector3 rotateAngle,int easeDuration)
    {
        // 设置初始位置
        tf.localPosition = startPos;
        // 缓动的起始、结束位置
        this.startPos = startPos;
        this.endPos = endPos;
        this.rotateAngle = rotateAngle;
        // 设置时间
        time = 0;
        this.duration = easeDuration;
    }

    public void Update()
    {
        time++;
        // 减速移动到指定位置
        Vector3 pos = MathUtil.GetLinearInterpolation(startPos, endPos, time, duration);
        tf.localPosition = pos;
        // 放大倍数
        float scale = MathUtil.GetEaseOutQuadInterpolation(startScale, toScale, time, duration);
        tf.localScale = new Vector3(scale, scale, 1);
        // 旋转角度
        tf.Rotate(rotateAngle);
        // 透明度
        if ( time >= duration - 15)
        {
            tmpColor.a = (duration - time) * 12f / 180;
        }
        else
        {
            tmpColor.a = Mathf.Pow((float)time / (duration - 15), 6) * 180;
        }
        spRenderer.color = tmpColor;
        if (time >= duration)
        {
            go.SetActive(false);
            isComplete = true;
        }
    }

    public void Clear()
    {
        go.SetActive(false);
        ObjectsPool.GetInstance().RestorePrefabToPool("EffectMapleLeaf0", go);
        go = null;
        tf = null;
        spRenderer = null;
    }

    public void Destroy()
    {

    }
}
