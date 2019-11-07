using UniLua;
using UnityEngine;

public class ReimuALuaInterface
{
    private static bool _isInit = false;

    private static LuaCsClosureValue _funcSetPos;
    private static LuaCsClosureValue _funcGetPos;


    public static void Init()
    {
        if (!_isInit)
        {
            _funcSetPos = new LuaCsClosureValue(LuaLib.IPosition_SetPosition);
            _funcGetPos = new LuaCsClosureValue(LuaLib.IPosition_GetPosition);

            _isInit = true;
        }
    }

    public static bool Get(object o, TValue key, out TValue res)
    {
        Reimu character = (Reimu)o;
        res = new TValue();
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                case "x":
                    {
                        res.SetNValue(character.GetPosition().x);
                        return true;
                    }
                case "y":
                    {
                        res.SetNValue(character.GetPosition().y);
                        return true;
                    }
                case "SetPos":
                    {
                        res.SetClCsValue(_funcSetPos);
                        return true;
                    }
                case "GetPos":
                    {
                        res.SetClCsValue(_funcGetPos);
                        return true;
                    }
                case "characterIndex":
                    {
                        res.SetNValue(character.CharacterIndex);
                        return true;
                    }
            }
        }
        res.SetSValue(string.Format("GetField from userData fail!Invalid key {0} for type {1}", key, typeof(Reimu).Name));
        return false;
    }

    public static bool Set(object o, TValue key, ref TValue value)
    {
        Reimu character = (Reimu)o;
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                case "x":
                    {
                        Vector2 pos = character.GetPosition();
                        pos.x = (float)value.NValue;
                        character.SetPosition(pos);
                        return true;
                    }
                case "y":
                    {
                        Vector2 pos = character.GetPosition();
                        pos.y = (float)value.NValue;
                        character.SetPosition(pos);
                        return true;
                    }
            }
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(Reimu).Name));
        return false;
    }
}