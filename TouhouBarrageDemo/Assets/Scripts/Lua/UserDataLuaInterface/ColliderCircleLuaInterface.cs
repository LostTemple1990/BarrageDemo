using UniLua;
using UnityEngine;

public class ColliderCircleLuaInterface
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

    private static LuaCsClosureValue _funcSetSize;
    private static LuaCsClosureValue _funcSetCollisionGroup;
    private static LuaCsClosureValue _funcSetExistDuration;

    public static void Init()
    {
        if (!_isInit)
        {
            _funcSetV = new LuaCsClosureValue(LuaLib.STGMovableDoStraightMove);
            _funcSetAcce = new LuaCsClosureValue(LuaLib.STGMovableDoAcceleration);
            _funcSetPolarParas = new LuaCsClosureValue(LuaLib.STGMovableDoCurvedMove);
            _funcMoveTo = new LuaCsClosureValue(LuaLib.STGMovableMoveTo);
            _funcMoveTowards = new LuaCsClosureValue(LuaLib.STGMovableMoveTowards);
            _funcSetPos = new LuaCsClosureValue(LuaLib.IPosition_SetPosition);
            _funcGetPos = new LuaCsClosureValue(LuaLib.IPosition_GetPosition);

            _funcAttachTo = new LuaCsClosureValue(LuaLib.AttachToMaster);
            _funcSetRelativePos = new LuaCsClosureValue(LuaLib.SetAttachmentRelativePos);

            _funcAddTask = new LuaCsClosureValue(LuaLib.AddTask);

            _funcSetStraightParas = new LuaCsClosureValue(LuaLib.STGMovableSetStraightParas);

            _funcSetSize = new LuaCsClosureValue(LuaLib.SetObjectColliderSize);
            _funcSetCollisionGroup = new LuaCsClosureValue(LuaLib.SetObjectColliderColliderGroup);
            _funcSetExistDuration = new LuaCsClosureValue(LuaLib.SetObjectColliderExistDuration);

            _isInit = true;
        }
    }

    public static bool Get(object o, TValue key, out TValue res)
    {
        ColliderCircle collider = (ColliderCircle)o;
        res = new TValue();
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        res.SetNValue(collider.GetPosition().x);
                        return true;
                    }
                case "y":
                    {
                        res.SetNValue(collider.GetPosition().y);
                        return true;
                    }
                case "rot":
                    {
                        res.SetNValue(collider.GetRotation());
                        return true;
                    }
                case "dx":
                    {
                        res.SetNValue(collider.dx);
                        return true;
                    }
                case "dy":
                    {
                        res.SetNValue(collider.dy);
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
                        res.SetNValue(collider.velocity);
                        return true;
                    }
                case "vx":
                    {
                        res.SetNValue(collider.vx);
                        return true;
                    }
                case "vy":
                    {
                        res.SetNValue(collider.vy);
                        return true;
                    }
                case "vAngle":
                    {
                        res.SetNValue(collider.vAngle);
                        return true;
                    }
                case "maxV":
                    {
                        res.SetNValue(collider.maxVelocity);
                        return true;
                    }
                case "acce":
                    {
                        res.SetNValue(collider.acce);
                        return true;
                    }
                case "accAngle":
                    {
                        res.SetNValue(collider.accAngle);
                        return true;
                    }
                #endregion
                #region Collider类专属变量
                case "SetCollisionGroup":
                    {
                        res.SetClCsValue(_funcSetCollisionGroup);
                        return true;
                    }
                case "SetSize":
                    {
                        res.SetClCsValue(_funcSetSize);
                        return true;
                    }
                case "SetExistDuration":
                    {
                        res.SetClCsValue(_funcSetExistDuration);
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
        res.SetSValue(string.Format("GetField from userData fail!Invalid key {0} for type {1}", key, typeof(ColliderCircle).Name));
        return false;
    }

    public static bool Set(object o, TValue key, ref TValue value)
    {
        ColliderCircle collider = (ColliderCircle)o;
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        Vector2 pos = collider.GetPosition();
                        pos.x = (float)value.NValue;
                        collider.SetPosition(pos);
                        return true;
                    }
                case "y":
                    {
                        Vector2 pos = collider.GetPosition();
                        pos.y = (float)value.NValue;
                        collider.SetPosition(pos);
                        return true;
                    }
                case "rot":
                    {
                        collider.SetRotation((float)value.NValue);
                        return true;
                    }
                #endregion
                #region 运动相关变量
                case "v":
                    {
                        collider.velocity = (float)value.NValue;
                        return true;
                    }
                case "vx":
                    {
                        collider.vx = (float)value.NValue;
                        return true;
                    }
                case "vy":
                    {
                        collider.vy = (float)value.NValue;
                        return true;
                    }
                case "vAngle":
                    {
                        collider.vAngle = (float)value.NValue;
                        return true;
                    }
                case "maxV":
                    {
                        collider.maxVelocity = (float)value.NValue;
                        return true;
                    }
                case "acce":
                    {
                        collider.acce = (float)value.NValue;
                        return true;
                    }
                case "accAngle":
                    {
                        collider.accAngle = (float)value.NValue;
                        return true;
                    }
                #endregion
            }
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(ColliderCircle).Name));
        return false;
    }
}