﻿using UniLua;
using UnityEngine;

public class STGObjectLuaInterface
{
    private static bool _isInit = false;

    private static LuaCsClosureValue _funcSetV;
    private static LuaCsClosureValue _funcSetAcce;
    private static LuaCsClosureValue _funcSetPolarParas;

    private static LuaCsClosureValue _funcMoveTo;
    private static LuaCsClosureValue _funcMoveTowards;
    private static LuaCsClosureValue _funcSetPos;
    private static LuaCsClosureValue _funcGetPos;

    private static LuaCsClosureValue _funcAttachTo;
    private static LuaCsClosureValue _funcSetRelativePos;

    private static LuaCsClosureValue _funcAddTask;

    private static LuaCsClosureValue _funcSetStraightParas;
    private static LuaCsClosureValue _funcSetSprite;
    private static LuaCsClosureValue _funcSetColor;
    private static LuaCsClosureValue _funcSetSize;
    private static LuaCsClosureValue _funcChangeAlphaTo;
    private static LuaCsClosureValue _funcSetBlendMode;
    private static LuaCsClosureValue _funcSetPrefab;
    private static LuaCsClosureValue _funcSetMaterialFloat;
    private static LuaCsClosureValue _funcSetMaterialInt;
    private static LuaCsClosureValue _funcSetMaterialColor;

    public static void Init()
    {
        if (!_isInit)
        {
            _funcSetV = new LuaCsClosureValue(LuaLib.STGMovableDoStraightMove);
            _funcSetAcce = new LuaCsClosureValue(LuaLib.STGMovableDoAcceleration);
            _funcSetStraightParas = new LuaCsClosureValue(LuaLib.STGMovableSetStraightParas);
            _funcSetPolarParas = new LuaCsClosureValue(LuaLib.STGMovableDoCurvedMove);
            _funcMoveTo = new LuaCsClosureValue(LuaLib.STGMovableMoveTo);
            _funcMoveTowards = new LuaCsClosureValue(LuaLib.STGMovableMoveTowards);
            _funcSetPos = new LuaCsClosureValue(LuaLib.IPosition_SetPosition);
            _funcGetPos = new LuaCsClosureValue(LuaLib.IPosition_GetPosition);

            _funcAttachTo = new LuaCsClosureValue(LuaLib.AttachToMaster);
            _funcSetRelativePos = new LuaCsClosureValue(LuaLib.SetAttachmentRelativePos);

            _funcAddTask = new LuaCsClosureValue(LuaLib.AddTask);

            _funcSetSprite = new LuaCsClosureValue(LuaLib.SetSTGObjectProps);
            _funcSetColor = new LuaCsClosureValue(LuaLib.SetSpriteEffectColor);
            _funcSetSize = new LuaCsClosureValue(LuaLib.SetSpriteEffectSize);
            _funcChangeAlphaTo = new LuaCsClosureValue(LuaLib.SpriteEffectChangeAlphaTo);
            _funcSetBlendMode = new LuaCsClosureValue(LuaLib.SetSpriteEffectBlendMode);
            _funcSetPrefab = new LuaCsClosureValue(LuaLib.SpriteEffectSetPefab);
            _funcSetMaterialFloat = new LuaCsClosureValue(LuaLib.SpriteEffectSetMatFloat);
            _funcSetMaterialInt = new LuaCsClosureValue(LuaLib.SpriteEffectSetMatInt);
            _funcSetMaterialColor = new LuaCsClosureValue(LuaLib.SpriteEffectSetMatColor);

            _isInit = true;
        }
    }

    public static bool Get(object o, TValue key, out TValue res)
    {
        STGSpriteEffect sprite = (STGSpriteEffect)o;
        res = new TValue();
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        res.SetNValue(sprite.GetPosition().x);
                        return true;
                    }
                case "y":
                    {
                        res.SetNValue(sprite.GetPosition().y);
                        return true;
                    }
                case "rot":
                    {
                        res.SetNValue(sprite.GetRotation());
                        return true;
                    }
                case "dx":
                    {
                        res.SetNValue(sprite.dx);
                        return true;
                    }
                case "dy":
                    {
                        res.SetNValue(sprite.dy);
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
                #endregion
                #region 运动相关变量
                case "v":
                    {
                        res.SetNValue(sprite.velocity);
                        return true;
                    }
                case "vx":
                    {
                        res.SetNValue(sprite.vx);
                        return true;
                    }
                case "vy":
                    {
                        res.SetNValue(sprite.vy);
                        return true;
                    }
                case "vAngle":
                    {
                        res.SetNValue(sprite.vAngle);
                        return true;
                    }
                case "maxV":
                    {
                        res.SetNValue(sprite.maxVelocity);
                        return true;
                    }
                case "acce":
                    {
                        res.SetNValue(sprite.acce);
                        return true;
                    }
                case "accAngle":
                    {
                        res.SetNValue(sprite.accAngle);
                        return true;
                    }
                #endregion
                #region STGObject类专属变量
                case "SetSprite":
                    {
                        res.SetClCsValue(_funcSetSprite);
                        return true;
                    }
                case "SetColor":
                    {
                        res.SetClCsValue(_funcSetColor);
                        return true;
                    }
                case "SetSize":
                    {
                        res.SetClCsValue(_funcSetSize);
                        return true;
                    }
                case "ChangeAlphaTo":
                    {
                        res.SetClCsValue(_funcChangeAlphaTo);
                        return true;
                    }
                case "SetBlendMode":
                    {
                        res.SetClCsValue(_funcSetBlendMode);
                        return true;
                    }
                case "alpha":
                    {
                        res.SetNValue(sprite.alpha);
                        return true;
                    }
                case "SetPrefab":
                    {
                        res.SetClCsValue(_funcSetPrefab);
                        return true;
                    }
                case "SetMaterialFloat":
                    {
                        res.SetClCsValue(_funcSetMaterialFloat);
                        return true;
                    }
                case "SetMaterialInt":
                    {
                        res.SetClCsValue(_funcSetMaterialInt);
                        return true;
                    }
                case "SetMaterialColor":
                    {
                        res.SetClCsValue(_funcSetMaterialColor);
                        return true;
                    }
                #endregion
                #region 运动类专属方法 ISTGMovable
                case "SetV":
                    {
                        res.SetClCsValue(_funcSetV);
                        return true;
                    }
                case "SetAcce":
                    {
                        res.SetClCsValue(_funcSetAcce);
                        return true;
                    }
                case "SetStraightParas":
                    {
                        res.SetClCsValue(_funcSetStraightParas);
                        return true;
                    }
                case "SetPolarParas":
                    {
                        res.SetClCsValue(_funcSetPolarParas);
                        return true;
                    }
                case "MoveTo":
                    {
                        res.SetClCsValue(_funcMoveTo);
                        return true;
                    }
                case "MoveTowards":
                    {
                        res.SetClCsValue(_funcMoveTowards);
                        return true;
                    }
                #endregion
                #region IAttachable
                case "AttachTo":
                    {
                        res.SetClCsValue(_funcAttachTo);
                        return true;
                    }
                case "SetRelativePos":
                    {
                        res.SetClCsValue(_funcSetRelativePos);
                        return true;
                    }
                #endregion
                #region ITaskExecuter
                case "AddTask":
                    {
                        res.SetClCsValue(_funcAddTask);
                        return true;
                    }
                    #endregion
            }
        }
        res.SetSValue(string.Format("GetField from userData fail!Invalid key {0} for type {1}", key, typeof(STGSpriteEffect).Name));
        return false;
    }

    public static bool Set(object o, TValue key, ref TValue value)
    {
        STGSpriteEffect sprite = (STGSpriteEffect)o;
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        Vector2 pos = sprite.GetPosition();
                        pos.x = (float)value.NValue;
                        sprite.SetPosition(pos);
                        return true;
                    }
                case "y":
                    {
                        Vector2 pos = sprite.GetPosition();
                        pos.y = (float)value.NValue;
                        sprite.SetPosition(pos);
                        return true;
                    }
                case "rot":
                    {
                        sprite.SetRotation((float)value.NValue);
                        return true;
                    }
                #endregion
                #region 运动相关变量
                case "v":
                    {
                        sprite.velocity = (float)value.NValue;
                        return true;
                    }
                case "vx":
                    {
                        sprite.vx = (float)value.NValue;
                        return true;
                    }
                case "vy":
                    {
                        sprite.vy = (float)value.NValue;
                        return true;
                    }
                case "vAngle":
                    {
                        sprite.vAngle = (float)value.NValue;
                        return true;
                    }
                case "maxV":
                    {
                        sprite.maxVelocity = (float)value.NValue;
                        return true;
                    }
                case "acce":
                    {
                        sprite.acce = (float)value.NValue;
                        return true;
                    }
                case "accAngle":
                    {
                        sprite.accAngle = (float)value.NValue;
                        return true;
                    }
                #endregion
                #region sprite专属变量
                case "orderInLayer":
                    {
                        sprite.SetOrderInLayer((int)value.NValue);
                        return true;
                    }
                case "alpha":
                    {
                        sprite.SetSpriteAlpha((float)value.NValue);
                        return true;
                    }
                #endregion
            }
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(STGSpriteEffect).Name));
        return false;
    }
}