using UnityEngine;
using System.Collections;

public class STGEffectBase
{
    protected string _effectName;
    protected bool _isFinish;

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
}
