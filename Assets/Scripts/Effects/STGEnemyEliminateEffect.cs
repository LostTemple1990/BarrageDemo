using UnityEngine;
using System.Collections.Generic;

public class STGEnemyEliminatedEffect : STGEffectBase
{
    private const float PosZ = 5;
    private const string PrefabNamePrefix = "EnemyEliminatedEffect";

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
    /// <summary>
    /// 样式
    /// </summary>
    private int _curStyle;
    private List<Transform> _tfList;
    private List<SpriteRenderer> _srList;

    private bool _isCached;

    private Vector2 _curPos;

    public STGEnemyEliminatedEffect()
    {
        _tfList = new List<Transform>();
        _srList = new List<SpriteRenderer>();
    }

    public override void Clear()
    {
        ObjectsPool.GetInstance().RestorePrefabToPool(PrefabNamePrefix + _curStyle, _effectGo);
        _effectGo = null;
        _effectTf = null;
        _tfList.Clear();
        _srList.Clear();
    }

    public override void Init()
    {
        base.Init();
        _isFinish = false;
        _time = 0;
        _isCached = true;
        _curStyle = -1;
    }

    public void SetEliminateEffectStyle(int style)
    {
        _curStyle = style;
        _isCached = false;
    }

    public override void SetToPos(float posX, float posY)
    {
        _curPos = new Vector2(posX, posY);
    }

    public override void Update()
    {
        if (!_isCached) Cache();
        if ( _curStyle <= 3 )
        {
            UpdateEffect0();
        }
    }

    private void Cache()
    {
        _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects/EnemyEliminatedEffects", "EnemyEliminatedEffect" + _curStyle);
        _effectTf = _effectGo.transform;
        if ( _curStyle <= 3 )
        {
            Transform tf;
            SpriteRenderer sr;
            for (int i=0;i<3;i++)
            {
                tf = _effectTf.Find("Sprite" + i);
                sr = tf.GetComponent<SpriteRenderer>();
                _tfList.Add(tf);
                _srList.Add(sr);
            }
            _duration = 30;
        }
        UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.Enemy);
        // 设置位置
        _effectTf.localPosition = new Vector3(_curPos.x, _curPos.y, PosZ);
        _isCached = true;
    }

    /// <summary>
    /// Style 0~3
    /// </summary>
    private void UpdateEffect0()
    {
        _time++;
        Transform tf;
        SpriteRenderer sr;
        float scaleX = 40 - _time;
        float scaleY = 70 + _time;
        float tmpValue = (float)_time / _duration;
        float alpha = 1 - tmpValue * tmpValue;
        for (int i = 0; i < 3; i++)
        {
            tf = _effectTf.Find("Sprite" + i);
            sr = tf.GetComponent<SpriteRenderer>();
            tf.localScale = new Vector3(scaleX, scaleY, 1);
            sr.color = new Color(1, 1, 1, alpha);
        }
        if ( _time >= _duration )
        {
            _isFinish = true;
        }
    }
}
