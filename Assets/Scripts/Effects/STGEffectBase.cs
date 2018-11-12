using UnityEngine;
using System.Collections;

public class STGEffectBase
{
    protected string _effectName;
    protected bool _isFinish;
    protected EffectType _effectType;

    public virtual void Init()
    {
        _isFinish = false;
        _effectName = "";
    }
    public virtual void Update()
    {

    }
    public virtual void SetToPos(float posX, float posY)
    {

    }
    public virtual void Clear()
    {

    }

    public bool IsFinish()
    {
        return _isFinish;
    }
    public void FinishEffect()
    {
        _isFinish = true;
    }

    public void SetName(string name)
    {
        _effectName = name;
    }

    public string GetName()
    {
        return _effectName;
    }

    public EffectType GetEffectType()
    {
        return _effectType;
    }

    /// <summary>
    /// 是否需要在特效完成之后存储回对象池
    /// </summary>
    /// <returns></returns>
    public virtual bool NeedToBeRestoredToPool()
    {
        return false;
    }
}
