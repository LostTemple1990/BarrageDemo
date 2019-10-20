using UnityEngine;
using System.Collections.Generic;

public class STGPlayerDeadEffect : STGEffectBase
{
    private const float CircleRadius = 32f;
    private const string EffectGoName = "DeadEffectGo";

    private GameObject _effectGo;
    private Transform _effectTf;
    /// <summary>
    /// 计时
    /// </summary>
    private int _time;
    /// <summary>
    /// 持续时间
    /// </summary>
    private int _duration;
    private List<Transform> _tfList;

    private bool _isCached;

    private Vector2 _curPos;

    public STGPlayerDeadEffect()
    {
        _tfList = new List<Transform>();
        _effectType = EffectType.PlayerDeadEffect;
    }

    public override void Clear()
    {
        for (int i=0;i<5;i++)
        {
            _tfList[i].localScale = new Vector3(0, 0, 1);
        }
        ObjectsPool.GetInstance().RestorePrefabToPool(EffectGoName, _effectGo);
        _effectGo = null;
        _effectTf = null;
        _tfList.Clear();
    }

    public override void Init()
    {
        base.Init();
        _isFinish = false;
        _time = 0;
        _isCached = false;
    }

    public override void SetPosition(float posX, float posY)
    {
        _curPos = new Vector2(posX, posY);
    }

    public override void Update()
    {
        if (!_isCached) Cache();
        UpdateEffect();
    }

    private void Cache()
    {
        _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "DeadEffectGo");
        _effectTf = _effectGo.transform;
        Transform tf = _effectTf.Find("CenterCircle");
        _tfList.Add(tf);
        for (int i = 0; i <= 3; i++)
        {
            tf = _effectTf.Find("Circle" + i);
            _tfList.Add(tf);
        }
        _duration = 60;
        UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.STGTopEffect);
        // 设置位置
        _effectTf.localPosition = new Vector3(_curPos.x, _curPos.y, 0);
        _isCached = true;
    }

    private void UpdateEffect()
    {
        _time++;
        float scale0 = _time * 12 / CircleRadius;
        float scale1 = _time * 8 / CircleRadius;
        Transform tf = _tfList[0];
        tf.localScale = new Vector3(scale0, scale0, 1);
        for (int i = 1; i <= 4; i++)
        {
            tf = _tfList[i];
            tf.localScale = new Vector3(scale1, scale1, 1);
        }
        if (_time >= _duration)
        {
            _isFinish = true;
        }
    }
}
