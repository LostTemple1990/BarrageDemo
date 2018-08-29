using UnityEngine;
using System.Collections;
using UniLua;

public class Task : IPoolClass
{
    public int funcRef;
    public ILuaState luaState;
    public int curWaitTime;
    public int totalWaitTime;
    public bool isFinish;
    public bool isStarted;

    public Task()
    {
        isStarted = false;
        isFinish = false;
    }

    public void Update()
    {
        if ( !isFinish )
        {
            InterpreterManager.GetInstance().CallTaskCoroutine(this);
        }
    }

    public void Clear()
    {
        isStarted = false;
        isFinish = false;
        luaState = null;
    }
}
