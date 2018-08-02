using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsManager 
{
    private static EffectsManager _instance = new EffectsManager();

    public static EffectsManager GetInstance()
    {
        return _instance;
    }

    private List<STGEffectBase> _effectList;
    private int _effectsCount;
    private int _clearTime;

    public void Init()
    {
        if ( _effectList == null )
        {
            _effectList = new List<STGEffectBase>();
        }
        _effectsCount = 0;
        _clearTime = 0;
    }

    public STGEffectBase CreateEffectByType(EffectType type)
    {
        STGEffectBase effect = null;
        switch ( type )
        {
            case EffectType.SpriteEffect:
                effect = new SpriteEffect();
                break;
            case EffectType.ShakeEffect:
                effect = new ShakeEffect();
                break;
            case EffectType.BreakScreenEffect:
                effect = new STGBreakScreenEffect();
                break;
            case EffectType.BurstEffect:
                effect = new STGBurstEffect();
                break;
            case EffectType.ChargeEffect:
                effect = new STGChargeEffect();
                break;
            case EffectType.BulletEliminate:
                effect = new STGBulletEliminateEffect();
                break;
        }
        if ( effect != null )
        {
            effect.Init();
            _effectList.Add(effect);
            _effectsCount++;
        }
        return effect;
    }

    public void Update()
    {
        int i;
        STGEffectBase effect;
        for (i=0;i<_effectsCount;i++)
        {
            effect = _effectList[i];
            if ( effect != null )
            {
                if (effect.IsFinish())
                {
                    effect.Clear();
                    _effectList[i] = null;
                }
                else
                {
                    effect.Update();
                }
            }
        }
        _clearTime++;
        if ( _clearTime >= 600 )
        {
            _clearTime = 0;
            _effectsCount = CommonUtils.RemoveNullElementsInList<STGEffectBase>(_effectList,_effectsCount);
        }
    }

    /// <summary>
    /// 根据名称拿到对应特效
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public STGEffectBase GetEffectByName(string name)
    {
        STGEffectBase effect;
        for (int i=0;i<_effectsCount;i++)
        {
            effect = _effectList[i];
            if ( effect != null && effect.GetName() == name )
            {
                return effect;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据名称结束特效
    /// </summary>
    /// <param name="name"></param>
    public void FinishEffectByName(string name)
    {
        STGEffectBase effect = GetEffectByName(name);
        if ( effect != null )
        {
            effect.FinishEffect();
        }
    }
}

public enum EffectType : byte
{
    SpriteEffect = 1,
    ShakeEffect = 2,
    BreakScreenEffect = 3,
    BurstEffect = 4,
    ChargeEffect = 5,
    BulletEliminate = 6,
}
