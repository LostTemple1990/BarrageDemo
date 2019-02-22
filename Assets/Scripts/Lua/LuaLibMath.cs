using UnityEngine;
using System.Collections;
using UniLua;
using Math = System.Math;

public partial class LuaLib
{
    public static int Math_Sin(ILuaState luaState)
    {
        double rad = luaState.ToNumber(-1) * Mathf.Deg2Rad;
        luaState.PushNumber((float)Math.Sin(rad));
        return 1;
    }

    public static int Math_Cos(ILuaState luaState)
    {
        double rad = luaState.ToNumber(-1) * Mathf.Deg2Rad;
        luaState.PushNumber((float)Math.Cos(rad));
        return 1;
    }

    public static int Math_Tan(ILuaState luaState)
    {
        double rad = luaState.ToNumber(-1) * Mathf.Deg2Rad;
        luaState.PushNumber((float)Math.Tan(rad));
        return 1;
    }

    public static int Math_ASin(ILuaState luaState)
    {
        double rad = luaState.ToNumber(-1) * Mathf.Deg2Rad;
        luaState.PushNumber((float)Math.Asin(rad));
        return 1;
    }

    public static int Math_ACos(ILuaState luaState)
    {
        double rad = luaState.ToNumber(-1) * Mathf.Deg2Rad;
        luaState.PushNumber((float)Math.Acos(rad));
        return 1;
    }

    public static int Math_ATan(ILuaState luaState)
    {
        double rad = luaState.ToNumber(-1) * Mathf.Deg2Rad;
        luaState.PushNumber((float)Math.Atan(rad));
        return 1;
    }

    public static int Math_Abs(ILuaState luaState)
    {
        luaState.PushNumber(Math.Abs(luaState.ToNumber(-1)));
        return 1;
    }

    public static int Math_Sign(ILuaState luaState)
    {
        luaState.PushNumber(Math.Sign(luaState.ToNumber(-1)));
        return 1;
    }

    public static int Math_Int(ILuaState luaState)
    {
        luaState.PushNumber(Math.Floor(luaState.ToNumber(-1)));
        return 1;
    }
}
