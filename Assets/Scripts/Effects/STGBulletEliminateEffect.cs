using UnityEngine;
using System.Collections;

public class STGBulletEliminateEffect : STGEffectBase
{
    private const int StateAppearance = 1;
    private const int StateFade = 2;
    private const int StateFinish = 3;

    private const int AppearnaceDuration = 20;
    private const int FadeDuration = 30;

    private const float AppearanceStartScale = 0.5f;
    private const float AppearanceEndScale = 1.2f;

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
    /// <summary>
    /// 临时存储颜色
    /// </summary>
    private Color _tmpColor;

    public STGBulletEliminateEffect()
    {
        _effectType = EffectType.BulletEliminate;
    }

    public override void Clear()
    {
        // 恢复透明度
        _tmpColor.a = 1;
        _spRenderer.color = _tmpColor;
        ObjectsPool.GetInstance().RestorePrefabToPool("BulletEliminateEffect", _effectGo);
        GameObject.Destroy(_effectGo);
        _effectGo = null;
        _effectTf = null;
        _spRenderer = null;
    }

    public override void Init()
    {
        base.Init();
        _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "BulletEliminateEffect");
        _effectTf = _effectGo.transform;
        _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        _tmpColor = _spRenderer.color;
        _isFinish = false;
        _state = StateAppearance;
        _time = 0;
        _effectTf.localScale = new Vector3(AppearanceStartScale, AppearanceStartScale, 1);
        UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.STGNormalEffect);
    }

    public override void SetToPosition(float posX, float posY)
    {
        _effectTf.localPosition = new Vector2(posX, posY);
    }

    public override void Update()
    {
        if ( _state == StateAppearance )
        {
            UpdateAppearanceState();
        }
        else if ( _state == StateFade )
        {
            UpdateFadeState();
        }
    }

    private void UpdateAppearanceState()
    {
        _time++;
        // 设置scale
        float scale = MathUtil.GetEaseInQuadInterpolation(AppearanceStartScale, AppearanceEndScale, _time, AppearnaceDuration);
        _effectTf.localScale = new Vector3(scale, scale, 1);
        if ( _time >= AppearnaceDuration )
        {
            _time = 0;
            _state = StateFade;
        }
    }

    private void UpdateFadeState()
    {
        _time++;
        // 更改alhpa
        float alpha = 1f - (float)_time / FadeDuration;
        _tmpColor.a = alpha;
        _spRenderer.color = _tmpColor;
        if ( _time >= FadeDuration )
        {
            _state = StateFinish;
            _isFinish = true;
        }
    }

    public void SetSprite(string spName)
    {
        _spRenderer.sprite = ResourceManager.GetInstance().GetSprite(Consts.EffectAtlasName, spName);
        _tmpColor = _spRenderer.color;
    }

    public void SetColor(float r,float g,float b,float alpha)
    {
        Color color = new Color(r, g, b, alpha);
        _spRenderer.color = color;
        _tmpColor = color;
    }

    public void SetColor(Color value)
    {
        Color color = value;
        _spRenderer.color = color;
        _tmpColor = color;
    }
}
