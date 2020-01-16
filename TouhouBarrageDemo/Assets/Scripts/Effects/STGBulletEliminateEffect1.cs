using UnityEngine;
using System.Collections;

public class STGBulletEliminateEffect1 : STGEffectBase
{
    private const int StateAppearance = 1;
    private const int StateFinish = 2;

    private const int AppearnaceDuration = 50;

    private GameObject _effectGo;
    private Transform _effectTf;
    private SpriteRenderer _spRenderer;
    /// <summary>
    /// 当前状态
    /// </summary>
    private int _state;
    /// <summary>
    /// 计时
    /// </summary>
    private int _time;

    private int _aniIndex;

    private Color _cacheColor;

    public STGBulletEliminateEffect1()
    {
        _effectType = EffectType.BulletEliminate;
    }

    public override void Clear()
    {
        ObjectsPool.GetInstance().RestorePrefabToPool("BulletEliminateEffect1", _effectGo);
        _effectGo = null;
        _effectTf = null;
        _spRenderer = null;
    }

    public override void Init()
    {
        base.Init();
        _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "BulletEliminateEffect1");
        _effectTf = _effectGo.transform;
        _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        _isFinish = false;
        _state = StateAppearance;
        _time = 0;
        _aniIndex = _time / 6;
        UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.STGNormalEffect);
    }

    public override void SetPosition(float posX, float posY)
    {
        _effectTf.localPosition = new Vector2(posX, posY);
    }

    public override void Update()
    {
        if (_state == StateAppearance)
        {
            UpdateAppearanceState();
        }
    }

    private void UpdateAppearanceState()
    {
        _time++;
        int tmpIndex = _time / 6;
        if (tmpIndex != _aniIndex)
        {
            _aniIndex = tmpIndex;
            SetColor(_cacheColor);
        }
        if (_time >= AppearnaceDuration)
        {
            _time = 0;
            _state = StateFinish;
            _isFinish = true;
        }
    }

    public void SetColor(float r, float g, float b, float alpha)
    {
        Color color = new Color(r, g, b, 0.125f * _aniIndex);
        _cacheColor = color;
        _spRenderer.color = color;
    }

    public void SetColor(Color value)
    {
        Color color = new Color(value.r, value.g, value.b, 0.125f * _aniIndex);
        _cacheColor = color;
        _spRenderer.color = color;
    }
}
