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

    private List<IEffect> _effectList;
    private int _effectsCount;
    private int _clearTime;

    public void Init()
    {
        if ( _effectList == null )
        {
            _effectList = new List<IEffect>();
        }
        _effectsCount = 0;
        _clearTime = 0;
    }

    public IEffect CreateEffectByType(EffectType type)
    {
        IEffect effect = null;
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
        IEffect effect;
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
            _effectsCount = CommonUtils.RemoveNullElementsInList<IEffect>(_effectList,_effectsCount);
        }
    }
}

public enum EffectType : byte
{
    SpriteEffect = 1,
    ShakeEffect = 2,
    BreakScreenEffect = 3,
}
