using UniLua;
using UnityEngine;

public class StageTaskLuaInterface
{
    private static bool _isInit = false;

    private static LuaCsClosureValue _funcAddTask;

    public static void Init()
    {
        if (!_isInit)
        {
            _funcAddTask = new LuaCsClosureValue(LuaLib.AddTask);

            _isInit = true;
        }
    }

    public static bool Get(object o, TValue key, out TValue res)
    {
        ExtraTaskManager mgr = (ExtraTaskManager)o;
        res = new TValue();
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region ITaskExecuter
                case "AddTask":
                    {
                        res.SetClCsValue(_funcAddTask);
                        return true;
                    }
                #endregion
            }
        }
        res.SetSValue(string.Format("GetField from userData fail!Invalid key {0} for type {1}", key, typeof(ExtraTaskManager).Name));
        return false;
    }

    public static bool Set(object o, TValue key, ref TValue value)
    {
        ExtraTaskManager mgr = (ExtraTaskManager)o;
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(ExtraTaskManager).Name));
        return false;
    }
}