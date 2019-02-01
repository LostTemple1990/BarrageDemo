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
    private Dictionary<EffectType, Stack<STGEffectBase>> _effectPools;

    public void Init()
    {
        if ( _effectList == null )
        {
            _effectList = new List<STGEffectBase>();
        }
        _effectsCount = 0;
        _effectPools = new Dictionary<EffectType, Stack<STGEffectBase>>();
        _clearTime = 0;
    }

    public STGEffectBase CreateEffectByType(EffectType type)
    {
        STGEffectBase effect = GetEffectFromPool(type);
        if ( effect == null )
        {
            switch (type)
            {
                case EffectType.SpriteEffect:
                    effect = new STGSpriteEffect();
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
                case EffectType.EnemyEliminated:
                    effect = new STGEnemyEliminatedEffect();
                    break;
            }
        }
        if ( effect != null )
        {
            effect.Init();
            _effectList.Add(effect);
            _effectsCount++;
        }
        return effect;
    }

    /// <summary>
    /// 获取STGEffect的实例
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private STGEffectBase GetEffectFromPool(EffectType type)
    {
        Stack<STGEffectBase> stack = null;
        STGEffectBase effect = null;
        if ( _effectPools.TryGetValue(type,out stack ) )
        {
            if ( stack.Count > 0 )
            {
                effect = stack.Pop();
            }
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
                    if ( effect.NeedToBeRestoredToPool() )
                    {
                        RestoreEffectToPool(effect);
                    }
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

    /// <summary>
    /// 将特效缓存到对象池中
    /// </summary>
    /// <param name="effect"></param>
    private void RestoreEffectToPool(STGEffectBase effect)
    {
        EffectType type = effect.GetEffectType();
        Stack<STGEffectBase> stack;
        if ( _effectPools.TryGetValue(type,out stack) )
        {
            stack.Push(effect);
        }
    }

    public void Clear()
    {
        int i;
        STGEffectBase effect;
        for (i = 0; i < _effectsCount; i++)
        {
            effect = _effectList[i];
            if (effect != null)
            {
                effect.Clear();
            }
        }
        _effectList.Clear();
        _effectsCount = 0;
    }
}

public enum EffectType : byte
{
    Null = 0,
    SpriteEffect = 1,
    ShakeEffect = 2,
    BreakScreenEffect = 3,
    BurstEffect = 4,
    ChargeEffect = 5,
    BulletEliminate = 6,
    EnemyEliminated = 7,
    TextEffect = 8,
}
