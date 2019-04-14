using UnityEngine;
using System.Collections;

public class BombBase
{
    protected bool _isFinish;

    public virtual void Start()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void Clear()
    {

    }

    public bool IsFinish
    {
        get { return _isFinish; }
    }
}
