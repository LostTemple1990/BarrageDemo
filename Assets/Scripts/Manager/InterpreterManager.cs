using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniLua;
using UnityEngine;


public class InterpreterManager
{
    private static InterpreterManager _instance;

    public static InterpreterManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new InterpreterManager();
        }
        return _instance;
    }

    private ILuaState _luaState;

    private List<LuaPara> _funcParas;

    private LuaCoroutineData _stageCoData;

    private Dictionary<string, int> _customizedInitFuncMap;
    private Dictionary<string, int> _customizedEnemyInitMap;
    private Dictionary<string, int> _customizedEnemyOnEliminateMap;
    /// <summary>
    /// 已经读取过的符卡配置
    /// </summary>
    private List<int> _spellCardLoadedList;
    private Dictionary<string, Vector2> _luaGlobalVec2Map;
    private Dictionary<string, float> _luaGlobalNumberMap;
    private Dictionary<string, object> _luaGlobalObjectMap;
    /// <summary>
    /// 当前正在进行的符卡
    /// </summary>
    private SpellCard _curSpellCard;

    private int _traceBackIndex;

    public InterpreterManager()
    {
        _customizedInitFuncMap = new Dictionary<string, int>();
        _customizedEnemyInitMap = new Dictionary<string, int>();
        _customizedEnemyOnEliminateMap = new Dictionary<string, int>();
        _luaGlobalNumberMap = new Dictionary<string, float>();
        _luaGlobalVec2Map = new Dictionary<string, Vector2>();
        _luaGlobalObjectMap = new Dictionary<string, object>();
        _spellCardLoadedList = new List<int>();
        _funcParas = new List<LuaPara>();
    }

    public void Init()
    {
        _luaState = LuaAPI.NewState();
        _luaState.L_OpenLibs();
        _luaState.L_RequireF("LuaLib", LuaLib.Init, false);
        _luaState.Pop(this._luaState.GetTop());
        // 添加错误log函数
        _luaState.PushCSharpFunction(Traceback);
        _traceBackIndex = _luaState.GetTop();
        // 加载Constants.lua
        var status = _luaState.L_DoFile("Constants.lua");
        if (status != ThreadStatus.LUA_OK)
        {
            throw new Exception(_luaState.ToString(-1));
        }

        _stageCoData = new LuaCoroutineData
        {
            isFinish = false
        };
    }

    /// <summary>
    /// 加载关卡对应的符卡配置
    /// </summary>
    /// <param name="stageId"></param>
    public void LoadSpellCardConfig(int stageId)
    {
        if ( _spellCardLoadedList.IndexOf(stageId) == -1 )
        {
            _spellCardLoadedList.Add(stageId);
            var status = _luaState.L_DoFile("stages/stage" + stageId + "sc.lua");
            if (status != ThreadStatus.LUA_OK)
            {
                throw new Exception(_luaState.ToString(-1));
            }
            RefCustomizedBullet();
            RefCustomizedEnemy();
        }
    }

    public Task LoadStage(int stageId)
    {
        LoadSpellCardConfig(stageId);
        var status = _luaState.L_DoFile("stages/stage" + stageId + ".lua");
        if (status != ThreadStatus.LUA_OK)
        {
            throw new Exception(_luaState.ToString(-1));
        }
        // 存放自定义的子弹task
        RefCustomizedBullet();
        RefCustomizedEnemy();
        RefBossTable();
        Task stageTask = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        InitStageTask(stageTask);
        return stageTask;
    }

    private void RefCustomizedBullet()
    {
        _luaState.GetField(-1, "CustomizedBulletTable");
        string customizedName;
        int initFuncRef;
        if (!_luaState.IsTable(-1))
        {
            Logger.Log("field CustomizedTable is not a table!");
        }
        _luaState.PushNil();
        while (_luaState.Next(-2))
        {
            customizedName = _luaState.ToString(-2);
            if (!_luaState.IsTable(-1))
            {
                Logger.Log("sub field " + customizedName + " is not a table!");
            }
            _luaState.GetField(-1, "Init");
            if (!_luaState.IsFunction(-1))
            {
                Logger.Log("sub field Init is not a function!");
            }
            initFuncRef = _luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
            _luaState.Pop(1);
            _customizedInitFuncMap.Add(customizedName, initFuncRef);
            Logger.Log("InitFunction in Customized Class " + customizedName + " is Ref , ref = " + initFuncRef);
        }
        _luaState.Pop(1);
    }

    private void RefCustomizedEnemy()
    {
        _luaState.GetField(-1, "CustomizedEnemyTable");
        string customizedName;
        int initFuncRef,onKillFuncRef;
        if (!_luaState.IsTable(-1))
        {
            Logger.Log("field CustomizedTable is not a table!");
            _luaState.Pop(1);
            return;
        }
        _luaState.PushNil();
        while (_luaState.Next(-2))
        {
            customizedName = _luaState.ToString(-2);
            if (!_luaState.IsTable(-1))
            {
                Logger.Log("sub field " + customizedName + " is not a table!");
            }
            _luaState.GetField(-1, "Init");
            if (!_luaState.IsFunction(-1))
            {
                Logger.Log("sub field Init is not a function!");
            }
            initFuncRef = _luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
            _customizedEnemyInitMap.Add(customizedName, initFuncRef);
            _luaState.GetField(-1, "OnKill");
            if ( _luaState.IsFunction(-1) )
            {
                onKillFuncRef = _luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
                _customizedEnemyOnEliminateMap.Add(customizedName, onKillFuncRef);
                Logger.Log("OnKillFunction in Customized Enemy " + customizedName + " is Ref , ref = " + onKillFuncRef);
            }
            else
            {
                _luaState.Pop(1);
            }
            _luaState.Pop(1);
            Logger.Log("InitFunction in Customized Class " + customizedName + " is Ref , ref = " + initFuncRef);
        }
        _luaState.Pop(1);
    }

    private void RefBossTable()
    {
        _luaState.GetField(-1, "BossTable");
        string bossName;
        int initRef, taskRef;
        if ( !_luaState.IsTable(-1) )
        {
            Logger.Log("Return value BossTable is not a table!");
        }
        _luaState.PushNil();
        while ( _luaState.Next(-2) )
        {
            bossName = _luaState.ToString(-2);
            if ( !_luaState.IsTable(-1) )
            {
                Logger.Log("Boss " + bossName + " in BossTable is invalid");
            }
            // init
            _luaState.GetField(-1, "Init");
            if ( !_luaState.IsFunction(-1) )
            {
                Logger.Log("Init Funciton of Boss" + bossName + " is invalid");
            }
            initRef = _luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
            // task
            _luaState.GetField(-1, "Task");
            if (!_luaState.IsFunction(-1))
            {
                Logger.Log("Task Funciton of Boss" + bossName + " is invalid");
            }
            taskRef = _luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
            // 弹出table
            _luaState.Pop(1);
            EnemyManager.GetInstance().AddBossRefData(bossName, initRef, taskRef);
            Logger.Log("Boss " + bossName + " is ref");
        }
        //弹出BossTable
        _luaState.Pop(1);
    }

    private void InitStageTask(Task stageTask)
    {
        _luaState.GetField(-1, "StageTask");
        if (!_luaState.IsFunction(-1))
        {
            Logger.LogError("Func StageTask is not exist!");
            return;
        }
        stageTask.funcRef = _luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        // 弹出function StageTask
        _luaState.Pop(1);
    }

    public void Update()
    {
        if ( !_stageCoData.isFinish )
        {
            CallStageCoroutine();
        }
    }

    private void CallStageCoroutine()
    {
        ThreadStatus status;
        if ( _stageCoData.luaState == null )
        {
            if ( !_luaState.IsTable(-1) )
            {
                throw new Exception("stage return is not a table!");
            }
            _luaState.GetField(-1, "StageTask");
            if ( !_luaState.IsFunction(-1) )
            {
                Logger.Log("Func StageTask is not exist!");
                return;
            }
            int funcRef = _luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
            _stageCoData.luaState = _luaState.NewThread();
            _luaState.Pop(1);
            _stageCoData.funcRef = funcRef;
            _stageCoData.totalWaitTime = 0;
            _stageCoData.isFinish = false;
            // 将协程函数压入栈
            _stageCoData.luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
            status = _stageCoData.luaState.Resume(null, 0);
        }
        else
        {
            _stageCoData.curWaitTime++;
            if ( _stageCoData.totalWaitTime != 0 && _stageCoData.curWaitTime < _stageCoData.totalWaitTime )
            {
                return;
            }
            _stageCoData.curWaitTime = 0;
            status = _stageCoData.luaState.Resume(_stageCoData.luaState, 0);
        }
        if ( status == ThreadStatus.LUA_YIELD )
        {
            // get waitTime
            _stageCoData.curWaitTime = 0;
            _stageCoData.totalWaitTime = _stageCoData.luaState.ToInteger(-1);
            _stageCoData.luaState.Pop(1);
            Logger.Log("StageLua wait for " + _stageCoData.totalWaitTime + " frames");
        }
        else if ( status == ThreadStatus.LUA_OK )
        {
            _stageCoData.isFinish = true;
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, _stageCoData.funcRef);
            Logger.Log("StageLua complete!");
        }
    }

    public void CallTaskCoroutine(Task task,int numArgs=0)
    {
        ThreadStatus status;
        if (task.luaState == null)
        {
            task.luaState = _luaState.NewThread();
            _luaState.Pop(1);
            task.luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, task.funcRef);
            //Logger.Log("Get Task func , funcRef = " + task.funcRef);
            if (!task.luaState.IsFunction(-1))
            {
                Logger.LogError("Task funcRef is not point to a function!");
                return;
            }
            PushParasToStack(task.luaState);
            status = task.luaState.Resume(null, numArgs);
            task.isStarted = true;
        }
        else
        {
            task.curWaitTime++;
            if (task.totalWaitTime != 0 && task.curWaitTime < task.totalWaitTime)
            {
                return;
            }
            task.curWaitTime = 0;
            task.luaState.PushBoolean(true);
            status = task.luaState.Resume(task.luaState, 1);
        }
        if (status == ThreadStatus.LUA_YIELD)
        {
            // get waitTime
            task.curWaitTime = 0;
            task.totalWaitTime = task.luaState.ToInteger(-1);
            task.luaState.Pop(1);
            //Logger.Log("EnemyTask wait for " + task.totalWaitTime + " frames");
        }
        else if (status == ThreadStatus.LUA_OK)
        {
            task.isFinish = true;
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, task.funcRef);
            Logger.Log("TaskLua complete! LuaFunc = " + task.funcRef);
        }
    }

    public void StopTaskThread(Task task)
    {
        if ( task.isFinish )
        {
            return;
        }
        ILuaState luaState = task.luaState;
        luaState.PushBoolean(false);
        ThreadStatus status = luaState.Resume(luaState, 1);
        if (status == ThreadStatus.LUA_OK)
        {
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, task.funcRef);
            //Logger.Log("StopTask Success!");
        }
        else
        {
            Logger.LogError("StopTask Fail");
        }
    }

    public void StopTaskThread(ILuaState luaState,int taskFuncRef)
    {
        luaState.PushBoolean(false);
        ThreadStatus status = luaState.Resume(luaState, 1);
        if ( status == ThreadStatus.LUA_OK )
        {
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX,taskFuncRef);
            //Logger.Log("StopTask Success! ref = " + taskFuncRef);
        }
        else
        {
            Logger.LogError("StopTask Fail");
        }
    }

    public void CallCostomizedInitFunc(EnemyBulletBase bullet,string customizedName,int numArgs)
    {
        int funcRef;
        if ( _customizedInitFuncMap.TryGetValue(customizedName, out funcRef) )
        {
            _luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
            if ( !_luaState.IsFunction(-1) )
            {
                Logger.Log("InitFuncRef of " + customizedName + " is not point to a function");
            }
            _luaState.PushLightUserData(bullet);
            // todo 以后有配置文件之后这个写法一定要改
            // 将函数和第一个参数bullet移动到指定位置
            Logger.Log(_luaState.GetTop());
            _luaState.Replace(-4 - numArgs);
            _luaState.Replace(-4 - numArgs);
            _luaState.Call(numArgs+1, 0);
            // 弹出剩余两个参数
            _luaState.Pop(2);
            return;
        }
        Logger.Log("CustomizedFunc by Name " + customizedName + " is not exist");
    }

    public int GetInitFuncRef(string customizedName)
    {
        int funcRef;
        if (_customizedInitFuncMap.TryGetValue(customizedName, out funcRef))
        {
            return funcRef;
        }
        Logger.Log("CustomizedFunc by Name " + customizedName + " is not exist");
        return 0;
    }

    /// <summary>
    /// 获取自定义敌机的初始化函数索引
    /// </summary>
    /// <param name="customizedName"></param>
    /// <returns></returns>
    public int GetEnemyInitFuncRef(string customizedName)
    {
        int funcRef;
        if (_customizedEnemyInitMap.TryGetValue(customizedName, out funcRef))
        {
            return funcRef;
        }
        Logger.Log("CustomizedFunc by Name " + customizedName + " is not exist");
        return 0;
    }

    /// <summary>
    /// 获取自定义敌机的初始化函数索引
    /// </summary>
    /// <param name="customizedName"></param>
    /// <returns></returns>
    public int GetEnemyOnEliminateFuncRef(string customizedName)
    {
        int funcRef;
        if (_customizedEnemyOnEliminateMap.TryGetValue(customizedName, out funcRef))
        {
            return funcRef;
        }
        return 0;
    }

    /// <summary>
    /// 添加调用lua方法需要的参数
    /// </summary>
    /// <param name="para"></param>
    /// <param name="type"></param>
    public void AddPara(object para,LuaParaType type)
    {
        LuaPara luaPara = new LuaPara()
        {
            paraValue = para,
            paraType = type
        };
        _funcParas.Add(luaPara);
    }

    public int CallLuaFunction(int funcRef,int paraCount,int retCount=0)
    {
        _luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        if ( !_luaState.IsFunction(-1) )
        {
            Logger.LogError("FuncRef" + funcRef + " is not point to a function!");
            return 0;
        }
        if ( _funcParas.Count != paraCount )
        {
            Logger.LogError("ParaCount of funcref " + funcRef + " is not match!");
            return 0;
        }
        PushParasToStack(_luaState);
        _luaState.PCall(paraCount, retCount,_traceBackIndex);
        return 1;
    }

    private void PushParasToStack(ILuaState luaState)
    {
        int count = _funcParas.Count;
        LuaPara luaPara;
        for (int i=0;i<count;i++)
        {
            luaPara = _funcParas[i];
            switch ( luaPara.paraType )
            {
                case LuaParaType.Bool:
                    luaState.PushBoolean((bool)luaPara.paraValue);
                    break;
                case LuaParaType.Int:
                    luaState.PushInteger((int)luaPara.paraValue);
                    break;
                case LuaParaType.Number:
                    luaState.PushNumber((double)luaPara.paraValue);
                    break;
                case LuaParaType.String:
                    luaState.PushString((string)luaPara.paraValue);
                    break;
                case LuaParaType.LightUserData:
                    luaState.PushLightUserData(luaPara.paraValue);
                    break;
            }
        }
        _funcParas.Clear();
        return;
    }

    public void SetGlobalVector2(string key,Vector2 value)
    {
        _luaGlobalVec2Map.Remove(key);
        _luaGlobalVec2Map.Add(key, value);
    }

    public Vector2 GetGlobalVector2(string  key)
    {
        Vector2 vec;
        if ( _luaGlobalVec2Map.TryGetValue(key,out vec) )
        {
            return vec;
        }
        Logger.LogError("GlobalVector2 with key " + key + "is not exist!");
        return Vector2.zero;
    }

    public void SetGlobalNumber(string key, float value)
    {
        _luaGlobalNumberMap.Remove(key);
        _luaGlobalNumberMap.Add(key, value);
    }

    public float GetGlobalNumber(string key)
    {
        float num;
        if (_luaGlobalNumberMap.TryGetValue(key, out num))
        {
            return num;
        }
        Logger.LogError("GlobalNumber with key " + key + "is not exist!");
        return 0;
    }

    public void SetGlobalUserData(string key,object value)
    {
        _luaGlobalObjectMap.Remove(key);
        _luaGlobalObjectMap.Add(key, value);
    }

    public object GetGlobalUserData(string key)
    {
        object obj;
        if ( _luaGlobalObjectMap.TryGetValue(key,out obj) )
        {
            return obj;
        }
        Logger.LogError("GlobalUserData with key " + key + "is not exist!");
        return null;
    }

    public void RemoveGlobalUserData(string key)
    {
        _luaGlobalObjectMap.Remove(key);
    }

    public List<TweenBase> TranslateTableToTweenList(ILuaState luaState)
    {
        List<TweenBase> tweenList = new List<TweenBase>();
        luaState.PushNil();
        while ( luaState.Next(-2) )
        {

        }
        return tweenList;
    }

    /// <summary>
    /// 将栈顶的table转换成vec2并弹出该table
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public Vector2 TranslateTableToVector2(ILuaState luaState)
    {
        Vector2 vec = new Vector2();
        luaState.GetField(-1, "x");
        vec.x = (float)luaState.ToNumber(-1);
        luaState.GetField(-2, "y");
        vec.y = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        return vec;
    }

    /// <summary>
    /// 将栈顶的table转换成vec3并弹出该table
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public Vector3 TranslateTableToVector3(ILuaState luaState)
    {
        Vector3 vec = new Vector2();
        luaState.GetField(-1, "x");
        vec.x = (float)luaState.ToNumber(-1);
        luaState.GetField(-2, "y");
        vec.y = (float)luaState.ToNumber(-1);
        luaState.GetField(-3, "z");
        vec.z = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        return vec;
    }

    /// <summary>
    /// 将栈顶的table转换成color并弹出该table
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public Color TranslateTableToColor(ILuaState luaState)
    {
        Color color = new Color();
        luaState.GetField(-1, "r");
        color.r = (float)luaState.ToNumber(-1);
        luaState.GetField(-2, "g");
        color.g = (float)luaState.ToNumber(-1);
        luaState.GetField(-3, "b");
        color.b = (float)luaState.ToNumber(-1);
        luaState.GetField(-4, "a");
        color.a = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        return color;
    }

    private static int Traceback(ILuaState lua)
    {
        var msg = lua.ToString(1);
        if (msg != null)
        {
            lua.L_Traceback(lua, msg, 1);
            Logger.LogError(msg);
        }
        // is there an error object?
        else if (!lua.IsNoneOrNil(1))
        {
            // try its `tostring' metamethod
            if (!lua.L_CallMeta(1, "__tostring"))
            {
                lua.PushString("(no error message)");
            }
        }
        return 1;
    }

    public void Clear()
    {
        _funcParas.Clear();
        UnrefCustomizedBullets();
        UnrefCustomizedEnemy();
        UnrefBoss();
        //ClearStageTask();
        ClearLuaGlobal();
    }

    /// <summary>
    /// 清除自定义子弹的初始化函数
    /// </summary>
    private void UnrefCustomizedBullets()
    {
        foreach (KeyValuePair<string,int> kv in _customizedInitFuncMap)
        {
            string customizedBulletName = kv.Key;
            int initFuncRef = kv.Value;
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, initFuncRef);
            Logger.Log("Unref init function of bullet " + customizedBulletName + " ref = " + initFuncRef);
        }
        _customizedInitFuncMap.Clear();
    }

    /// <summary>
    /// 清除自定义敌机的函数
    /// </summary>
    private void UnrefCustomizedEnemy()
    {
        // 敌机初始化函数
        foreach (KeyValuePair<string, int> kv in _customizedEnemyInitMap)
        {
            string customizedEnemyName = kv.Key;
            int initFuncRef = kv.Value;
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, initFuncRef);
            Logger.Log("Unref init function of enemy " + customizedEnemyName + " ref = " + initFuncRef);
        }
        _customizedEnemyInitMap.Clear();
        // 敌机被消灭触发的函数
        foreach (KeyValuePair<string, int> kv in _customizedEnemyOnEliminateMap)
        {
            string customizedEnemyName = kv.Key;
            int initFuncRef = kv.Value;
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, initFuncRef);
            Logger.Log("Unref onEliminate function of enemy " + customizedEnemyName + " ref = " + initFuncRef);
        }
        _customizedEnemyOnEliminateMap.Clear();
    }

    private void UnrefBoss()
    {
        Dictionary<string, BossRefData> datas = EnemyManager.GetInstance().GetAllBossRefData();
        // BOSS部分
        foreach (KeyValuePair<string, BossRefData> kv in datas)
        {
            string bossName = kv.Key;
            BossRefData refData = kv.Value;
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, refData.initFuncRef);
            _luaState.L_Unref(LuaDef.LUA_REGISTRYINDEX, refData.taskFuncRef);
            Logger.Log("Unref init function of boss " + bossName + " ref = " + refData.initFuncRef + " and " + refData.taskFuncRef);
        }
        datas.Clear();
    }

    private void ClearStageTask()
    {
        StopTaskThread(_stageCoData.luaState, _stageCoData.funcRef);
        _stageCoData.Clear();
    }

    private void ClearLuaGlobal()
    {
        _luaGlobalNumberMap.Clear();
        _luaGlobalObjectMap.Clear();
        _luaGlobalVec2Map.Clear();
    }
}

struct LuaPara
{
    public object paraValue;
    public LuaParaType paraType;
}

public enum LuaParaType : byte
{
    Bool = 1,
    Int = 2,
    Number = 3,
    String = 4,
    LightUserData = 5,
}

struct LuaCoroutineData
{
    public int funcRef;
    public ILuaState luaState;
    public int curWaitTime;
    public int totalWaitTime;
    public bool isFinish;

    public void Clear()
    {
        luaState = null;
    }
}