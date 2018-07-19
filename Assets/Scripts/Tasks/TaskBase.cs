using UnityEngine;
using System.Collections;
using UniLua;

public class TaskBase : IPoolClass
{
    public int funcRef;
    public ILuaState luaState;
    public int curWaitTime;
    public int totalWaitTime;
    public bool isFinish;

    public void Update()
    {

    }

    public virtual void Clear()
    {
        luaState = null;
    }
}