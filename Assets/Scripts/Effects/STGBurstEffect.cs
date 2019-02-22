using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class STGBurstEffect : STGEffectBase
{
    /// <summary>
    /// 默认尺寸
    /// </summary>
    private const float DefaultSize = 64;
    private const int CircleBurstDuration = 30;
    private const int BurstDuration = 60;
    
    private List<BurstObject> _burstList;
    private int _burstCount;
    /// <summary>
    /// 特效的容器tf
    /// </summary>
    private Transform _effectContainerTf;
    private Transform _circleTf;
    private SpriteRenderer _circleSpRenderer;

    private bool _isCircleBurst;
    private int _circleBurstTime;
    private int _burstTime;

    public STGBurstEffect()
    {
        _effectType = EffectType.BurstEffect;
    }

    public override void Init()
    {
        base.Init();
        // 容器
        if ( _effectContainerTf == null )
        {
            _effectContainerTf = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "STGEffectContainer").transform;
            UIManager.GetInstance().AddGoToLayer(_effectContainerTf.gameObject, LayerId.STGNormalEffect);
        }
        // 炸开的圆形
        if ( _circleTf == null )
        {
            _circleTf = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect").transform;
            _circleTf.parent = _effectContainerTf;
            _circleTf.localScale = new Vector3(0, 0, 1);
            _circleSpRenderer = _circleTf.Find("Sprite").GetComponent<SpriteRenderer>();
            _circleSpRenderer.sprite = ResourceManager.GetInstance().GetSprite(Consts.EffectAtlasName,"TransparentCircle");
            _circleSpRenderer.color = new Color(0.95f,0.55f,0.9f,0.5f);
        }
        if ( _burstCount == 0 )
        {
            InitBurstObjects();
        }
        _circleBurstTime = 0;
        _isCircleBurst = true;
        _burstTime = 0;
    }

    private void InitBurstObjects()
    {
        _burstList = new List<BurstObject>();
        _burstCount = Random.Range(15, 25);
        for (int i=0;i<_burstCount;i++)
        {
            BurstObject burstObject = new BurstObject();
            GameObject burstGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "EffectMapleLeaf0");
            burstObject.Init(burstGo);
            float angle = Random.Range(0, 360);
            float dis = Random.Range(150f,240f);
            float toScale = Random.Range(1.5f, 3f);
            Vector3 rotateAngle = new Vector3(Random.Range(0f, 3f), Random.Range(0f, 3f), Random.Range(0f, 3f));
            burstObject.SetParas(angle, dis, toScale, rotateAngle, BurstDuration);
            _burstList.Add(burstObject);
        }
    }

    /// <summary>
    /// 设置特效的尺寸，默认值为DefaultSize
    /// </summary>
    public void SetSize(float size = DefaultSize)
    {
        float scale = size / DefaultSize;
        Vector3 newScale = new Vector3(scale, scale, 1);
        _effectContainerTf.localScale = newScale;
    }

    public override void Update()
    {
        _burstTime++;
        if ( _isCircleBurst )
        {
            UpdateCircleTf();
        }
        UpdateBurstObjects();
        if ( _burstTime >= BurstDuration )
        {
            _isFinish = true;
        }
    }

    private void UpdateCircleTf()
    {
        _circleBurstTime++;
        if (_circleBurstTime <= CircleBurstDuration)
        {
            float scale = MathUtil.GetEaseInQuadInterpolation(0, 4, _circleBurstTime, CircleBurstDuration);
            _circleTf.localScale = new Vector3(scale, scale, 1);
        }
        else
        {
            _circleTf.gameObject.SetActive(false);
            _isCircleBurst = false;
        }
    }

    private void UpdateBurstObjects()
    {
        foreach (BurstObject bo in _burstList)
        {
            bo.Update();
        }
    }

    public override void SetPosition(float posX, float posY)
    {
        _effectContainerTf.localPosition = new Vector3(posX, posY, 0);
    }

    public override void Clear()
    {
        int i;
        BurstObject bo;
        for (i = 0; i < _burstCount; i++)
        {
            bo = _burstList[i];
            bo.Clear();
        }
        _burstList.Clear();
        _burstCount = 0;
        GameObject.Destroy(_effectContainerTf.gameObject);
    }
}

class BurstObject
{
    public GameObject go;
    public Transform tf;
    public SpriteRenderer spRenderer;
    public int time;
    public int duration;
    float toScale;
    Vector3 ratoteAngle;
    public int state;
    public float dAlpha;
    public Color tmpColor;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool isComplete;

    public void Init(GameObject go)
    {
        this.go = go;
        go.SetActive(true);
        tf = go.transform;
        UIManager.GetInstance().AddGoToLayer(go, LayerId.STGNormalEffect);
        tf.localPosition = Vector3.zero;
        spRenderer = tf.Find("Sprite").GetComponent<SpriteRenderer>();
        tf.localScale = new Vector3(0, 0, 1);
        isComplete = false;
    }

    public void SetParas(float angle,float dis,float toScale,Vector3 rotateAngle,int burstDuration)
    {
        // 归一化速度向量
        Vector3 vDirVec = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        // 求出起始和结束为止
        startPos = tf.localPosition;
        endPos = startPos + vDirVec * dis;
        // 设置变形参数
        this.toScale = toScale;
        this.ratoteAngle = rotateAngle;
        // 设置时间
        time = 0;
        duration = burstDuration;
    }

    public void Update()
    {
        time++;
        // 减速移动到指定位置
        Vector3 pos = MathUtil.GetEaseInQuadInterpolation(startPos, endPos, time, duration);
        tf.localPosition = pos;
        // 放大倍数
        float scale = MathUtil.GetEaseOutQuadInterpolation(0, toScale, time, duration);
        tf.localScale = new Vector3(scale,scale, 1);
        // 旋转角度
        tf.Rotate(ratoteAngle);
        // 透明度
        if ( time >= duration )
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
