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
    private Dictionary<string, Vector2> _luaGlobalVec2Map;
    private Dictionary<string, float> _luaGlobalNumberMap;
    private Dictionary<string, object> _luaGlobalObjectMap;

    private int _traceBackIndex;

    public InterpreterManager()
    {
        _customizedInitFuncMap = new Dictionary<string, int>();
        _customizedEnemyInitMap = new Dictionary<string, int>();
        _customizedEnemyOnEliminateMap = new Dictionary<string, int>();
        _luaGlobalNumberMap = new Dictionary<string, float>();
        _luaGlobalVec2Map = new Dictionary<string, Vector2>();
        _luaGlobalObjectMap = new Dictionary<string, object>();
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

    public void DoStageLua(int stageId)
    {
        var status = _luaState.L_DoFile("stages/stage" + stageId + ".lua");
        if (status != ThreadStatus.LUA_OK)
        {
            throw new Exception(_luaState.ToString(-1));
        }
        // 存放自定义的子弹task
        RefCustomizedBullet();
        RefCustomizedEnemy();
        RefBossTable();
        //CallStageCoroutine();
        //LuaCoroutineData coData = new LuaCoroutineData();
    }

    private void RefCustomizedBullet()
    {
        _luaState.GetField(-1, "CustomizedTable");
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
            Logger.Log("Get Task func , funcRef = " + task.funcRef);
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
            Logger.Log("StopTask Success!");
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
            Logger.Log("StopTask Success! ref = " + taskFuncRef);
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

    //public void createSkill()
    //{
    //    this.loadSkillScript("skill1");
    //    string scriptName = "skills/skill1.lua";
    //    var status = this._luaState.L_DoFile(scriptName);
    //    if (status != ThreadStatus.LUA_OK)
    //    {
    //        throw new Exception(this._luaState.ToString(-1));
    //    }

    //}

    //public int initSkill(string skillId, Unit unit)
    //{
    //    if (this.loadSkillScript(skillId) != BattleConsts.LUA_OPERATION_SUCCESS)
    //    {
    //        Debug.LogError("lua script " + skillId + ".lua occurs an error!");
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    if (!this._currentState.IsTable(-1))
    //    {
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    this._currentState.GetField(-1, "initSkill");
    //    if (!this._currentState.IsFunction(-1))
    //    {
    //        Debug.LogError("method initSkill is not exist!");
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    this.pushLightUserData(this._currentState, unit);
    //    this._currentState.PCall(1, 0, 0);
    //    return BattleConsts.LUA_OPERATION_SUCCESS;
    //}

    //public void registerEffect(SkillEffect effect)
    //{
    //    this._currentState.PushLightUserData(effect);
    //    effect.setRef(this._currentState.L_Ref(LuaDef.LUA_REGISTRYINDEX));
    //}

    //public void registerLightUserData(ILuaUserData userData)
    //{
    //    this._currentState.PushLightUserData(userData);
    //    userData.setRef(this._currentState.L_Ref(LuaDef.LUA_REGISTRYINDEX));
    //}

    //public int callFunction(int funcRef, int paramCount, int retCount = 0)
    //{
    //    this._currentState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
    //    if (!this._currentState.IsFunction(-1))
    //    {
    //        Debug.LogError("funcRef doesn't point to a function!");
    //        this._currentState.Pop(1);
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    if (this._params.Count != paramCount)
    //    {
    //        Debug.Log("param count is not match!");
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    this.pushParams(this._currentState);
    //    //int numArgs = args==null ? 0 : 
    //    var status = this._currentState.PCall(paramCount, retCount, 0);
    //    if (status != ThreadStatus.LUA_OK)
    //    {
    //        Debug.LogError(this._currentState.ToString(-1));
    //        this._currentState.Pop(1);
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    return BattleConsts.LUA_OPERATION_SUCCESS;
    //}

    //public int callCoroutine(int funcRef, int paramCount)
    //{
    //    if (funcRef == 0)
    //    {
    //        Debug.LogError("\"CallRoutine\" : attempt to call a null funciton!");
    //        this.clearParams();
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    if (this._params.Count != paramCount)
    //    {
    //        Debug.LogError("\"CallRoutine\" : Param count is not match!");
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    ILuaState newState;
    //    if (this._coroutineDic.TryGetValue(funcRef, out newState))
    //    {
    //        newState = this._luaState.NewThread();
    //        newState.RawGetI(funcRef, LuaDef.LUA_REGISTRYINDEX);
    //        if (!newState.IsFunction(-1))
    //        {
    //            Debug.LogError("\"CallRoutine\" : FuncRef doesn't point to a function!");
    //            this.clearParams();
    //            return BattleConsts.LUA_OPERATION_FAIL;
    //        }
    //        this._coroutineDic.Add(funcRef, newState);
    //    }
    //    this.pushParams(newState);
    //    ThreadStatus status = newState.Resume(newState, paramCount);
    //    if (status == ThreadStatus.LUA_OK)
    //    {
    //        this._coroutineDic.Remove(funcRef);
    //        this._currentState = this._luaState;
    //        return BattleConsts.LUA_COROUTINE_FINISH;
    //    }
    //    else if (status == ThreadStatus.LUA_YIELD)
    //    {
    //        return BattleConsts.LUA_COROUTINE_YIELD;
    //    }
    //    else
    //    {
    //        this._currentState = this._luaState;
    //        this._coroutineDic.Remove(funcRef);
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //}

    ///// <summary>
    ///// 检测条件
    ///// </summary>
    ///// <param name="funcRef">函数索引</param>
    ///// <param name="paramCount">参数个数</param>
    ///// <returns></returns>
    //public bool checkCondition(int funcRef, int paramCount)
    //{
    //    if (funcRef == 0)
    //    {
    //        this.clearParams();
    //        return true;
    //    }
    //    int result = this.callFunction(funcRef, paramCount, 1);
    //    if (result != BattleConsts.LUA_OPERATION_SUCCESS)
    //    {
    //        Debug.LogError("condition function call fail!");
    //        this.clearParams();
    //        return false;
    //    }
    //    bool checkResult = this._currentState.ToBoolean(-1);
    //    this._currentState.Pop(1);
    //    return checkResult;
    //}

    //public int loadSkillScript(string skillId)
    //{
    //    this._currentState.GetGlobal(skillId);
    //    if (this._currentState.IsNil(-1))
    //    {
    //        this._currentState.Pop(1);
    //        this._currentState.CreateTable(0, 0);
    //        this._currentState.SetGlobal(skillId);
    //        this._currentState.GetGlobal(skillId);
    //        return this.loadScript("skills/" + skillId + ".lua");
    //    }
    //    return BattleConsts.LUA_OPERATION_SUCCESS;
    //}

    //public void createSkillBySkillId(string skillId)
    //{

    //}

    //private int loadScript(string scriptName)
    //{
    //    var status = this._currentState.L_DoFile(scriptName);
    //    if (status != ThreadStatus.LUA_OK)
    //    {
    //        Debug.Log(this._currentState.ToString(-1));
    //        this._currentState.Pop(1);
    //        return BattleConsts.LUA_OPERATION_FAIL;
    //    }
    //    return BattleConsts.LUA_OPERATION_SUCCESS;
    //}

    ///// <summary>
    ///// 添加lua函数的参数
    ///// </summary>
    ///// <param name="param">参数</param>
    ///// <param name="paramType">参数类型</param>
    //public void addParam(object param, BattleConsts.ParamType paramType)
    //{
    //    this._params.Add(this.genLuaParam(param, paramType));
    //}

    ///// <summary>
    ///// 获取参数个数
    ///// </summary>
    ///// <returns></returns>
    //public int getParamCount()
    //{
    //    return this._params.Count;
    //}

    //private LuaParam genLuaParam(object param, BattleConsts.ParamType paramType)
    //{
    //    LuaParam luaParam = new LuaParam();
    //    luaParam.param = param;
    //    luaParam.paramType = paramType;
    //    return luaParam;
    //}

    ///// <summary>
    ///// 将缓存的参数压入栈
    ///// </summary>
    //private void pushParams(ILuaState luaState)
    //{
    //    int len = this._params.Count;
    //    for (int i = 0; i < len; i++)
    //    {
    //        LuaParam it = this._params[i];
    //        switch (it.paramType)
    //        {
    //            case BattleConsts.ParamType.Boolean:
    //                luaState.PushBoolean((bool)it.param);
    //                break;
    //            case BattleConsts.ParamType.Int:
    //                luaState.PushInteger((int)it.param);
    //                break;
    //            case BattleConsts.ParamType.String:
    //                luaState.PushString((string)it.param);
    //                break;
    //            case BattleConsts.ParamType.VO:
    //                luaState.PushLightUserData(it.param);
    //                break;
    //            case BattleConsts.ParamType.Event:
    //                luaState.PushLightUserData(it.param);
    //                break;
    //        }
    //    }
    //    this.clearParams();
    //}

    ///// <summary>
    ///// 清楚缓存的参数
    ///// </summary>
    //private void clearParams()
    //{
    //    int len = this._params.Count;
    //    for (int i = 0; i < len; i++)
    //    {
    //        this._params[i].param = null;
    //    }
    //    this._params.Clear();
    //}

    //public void pushEffect(ILuaState luaState, ISkillEffect effect)
    //{
    //    if (effect == null || effect.getRef() == 0)
    //    {
    //        luaState.PushNil();
    //    }
    //    else
    //    {
    //        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, effect.getRef());
    //    }
    //}

    //public void pushLightUserData(ILuaState luaState, ILuaUserData userData)
    //{
    //    if (userData == null || userData.getRef() == 0)
    //    {
    //        luaState.PushNil();
    //    }
    //    else
    //    {
    //        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, userData.getRef());
    //    }
    //}

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
        ClearStageTask();
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