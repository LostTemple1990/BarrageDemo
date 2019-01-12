using UnityEngine;
using System.Collections;

public class BuffGravitation : IBuff
{
    /// <summary>
    /// 被影响的物体
    /// </summary>
    private IAffectedMovableObject _obj;
    private int _timeSinceAdded;

    public bool AddTo(IBuffContainer container)
    {
        _obj = container as IAffectedMovableObject;
        _timeSinceAdded = 0;
        if (_obj != null) return true;
        return false;
    }

    public IBuffContainer GetOwner()
    {
        return _obj as IBuffContainer;
    }

    public int GetTimeSinceAdded()
    {
        return _timeSinceAdded;
    }

    public int GetTotalDuration()
    {
        throw new System.NotImplementedException();
    }

    public bool IsValid()
    {
        throw new System.NotImplementedException();
    }

    public void OnAdded(IBuffContainer container)
    {
        throw new System.NotImplementedException();
    }

    public void OnLogic()
    {
        _timeSinceAdded++;
    }

    public bool Remove()
    {
        throw new System.NotImplementedException();
    }

    public void SetOverlay(int value)
    {

    }
}
