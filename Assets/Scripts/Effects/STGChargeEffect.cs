using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STGChargeEffect : IEffect
{
    /// <summary>
    /// 默认尺寸
    /// </summary>
    private const float DefaultSize = 64;
    private const int CircleShrinkDuration = 60;

    private const float LimitMinX = 100;
    private const float LimitMaxX = 150;
    private const float LimitMinY = 150;
    private const float LimitMaxY = 200;

    private const int ChargeMinDuration = 50;
    private const int ChargeMaxDuration = 70;

    private const float InitCircleScale = 8f;

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

    private bool _isFinish;

    public void Init()
    {
        _isFinish = false;
        // 容器
        if (_effectContainerTf == null)
        {
            _effectContainerTf = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "STGEffectContainer").transform;
            UIManager.GetInstance().AddGoToLayer(_effectContainerTf.gameObject, LayerId.GameEffect);
        }
        if (_chargeCount == 0)
        {
            InitChargeObjects();
        }
        // 收缩的圆形
        if (_circleTf == null)
        {
            _circleTf = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect").transform;
            _circleTf.SetParent(_effectContainerTf, false);
            _circleTf.localScale = new Vector3(InitCircleScale, InitCircleScale, 1);
            _circleSpRenderer = _circleTf.Find("Sprite").GetComponent<SpriteRenderer>();
            _circleSpRenderer.sprite = Resources.Load<Sprite>("pl00/pl00_0");
            _circleSpRenderer.color = new Color(0.95f, 0.1f, 0.1f, 0.5f);
        }
        _circleShrinkTime = 0;
        _isCircleShrinking = true;
        _chargeTime = 0;
    }

    private void InitChargeObjects()
    {
        _chargeList = new List<ChargeObject>();
        _chargeCount = Random.Range(15, 25);
        for (int i = 0; i < _chargeCount; i++)
        {
            ChargeObject chargeObj = new ChargeObject();
            chargeObj.Init(_effectContainerTf);
            Vector3 startPos = GetRandomStartPos();
            Vector3 rotateAngle = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 2f));
            int easeDuration = Random.Range(ChargeMinDuration, ChargeMaxDuration);
            chargeObj.SetParas(startPos, rotateAngle, easeDuration);
            _chargeList.Add(chargeObj);
        }
    }

    public void Update()
    {
        _chargeTime++;
        if (_isCircleShrinking)
        {
            UpdateCircleTf();
        }
        UpdateBurstObjects();
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
            float scale = MathUtil.GetEaseInQuadInterpolation(InitCircleScale, 0, _circleShrinkTime, CircleShrinkDuration);
            _circleTf.localScale = new Vector3(scale, scale, 1);
        }
        else
        {
            _circleTf.gameObject.SetActive(false);
            _isCircleShrinking = false;
        }
    }

    private void UpdateBurstObjects()
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
        int factor;
        float posX, posY;
        factor = Random.Range(0, 2) * 2 - 1;
        posX = factor * Random.Range(LimitMinX, LimitMaxX);
        factor = Random.Range(0, 2) * 2 - 1;
        posY = factor * Random.Range(LimitMinY, LimitMaxY);
        return new Vector3(posX, posY, 0);
    }

    public void SetToPos(float posX, float posY)
    {
        _effectContainerTf.localPosition = new Vector3(posX, posY, 0);
    }

    public void Clear()
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
        GameObject.Destroy(_circleTf.gameObject);
        GameObject.Destroy(_effectContainerTf.gameObject);
    }

    public bool IsFinish()
    {
        return _isFinish;
    }

    public void FinishEffect()
    {
        _isFinish = true;
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
    Vector3 ratoteAngle;
    public int state;
    public float dAlpha;
    public Color tmpColor;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool isComplete;

    public void Init(Transform parent)
    {
        go = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
        tf = go.transform;
        tf.SetParent(parent, false);
        tf.localPosition = Vector3.zero;
        // 随机方向
        tf.localRotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
        spRenderer = tf.Find("Sprite").GetComponent<SpriteRenderer>();
        spRenderer.sprite = Resources.Load<Sprite>("Background/MapleLeaf1");
        // 一定的透明度
        tmpColor = spRenderer.color;
        tmpColor.a = 0.3f;
        spRenderer.color = tmpColor;
        // 初始scale
        float scale = Random.Range(InitScaleRangeMin, InitScaleRangeMax);
        tf.localScale = new Vector3(scale, scale, 1);
        startScale = scale;
        toScale = Random.Range(EndScaleRangeMin, EndScaleRangeMax);
        isComplete = false;
    }

    public void SetParas(Vector3 startPos,Vector3 rotateAngle,int easeDuration)
    {
        // 设置初始位置
        tf.localPosition = startPos;
        // 缓动的起始、结束位置
        this.startPos = startPos;
        endPos = Vector3.zero;
        this.ratoteAngle = rotateAngle;
        // 设置时间
        time = 0;
        this.duration = easeDuration;
    }

    public void Update()
    {
        time++;
        // 减速移动到指定位置
        Vector3 pos = MathUtil.GetEaseInQuadInterpolation(startPos, endPos, time, duration);
        tf.localPosition = pos;
        // 放大倍数
        float scale = MathUtil.GetEaseOutQuadInterpolation(startScale, toScale, time, duration);
        tf.localScale = new Vector3(scale, scale, 1);
        // 旋转角度
        tf.Rotate(ratoteAngle);
        // 透明度
        if (time >= duration)
        {
            go.SetActive(false);
            isComplete = true;
        }
    }

    public void Clear()
    {
        spRenderer.sprite = null;
        GameObject.Destroy(go);
        go = null;
        tf = null;
        spRenderer = null;
    }

    public void Destroy()
    {

    }
}
