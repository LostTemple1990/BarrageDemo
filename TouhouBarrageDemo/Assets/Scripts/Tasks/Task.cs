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
        luaState = null;
        isFinish = false;
        isStarted = false;
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
        isFinish = false;
        luaState = null;
        isStarted = false;
    }
}
