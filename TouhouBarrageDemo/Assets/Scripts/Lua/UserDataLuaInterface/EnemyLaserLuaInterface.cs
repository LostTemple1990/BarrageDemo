using UniLua;
using UnityEngine;

public class EnemyLaserLuaInterface
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
    private static LuaCsClosureValue _funcSetSelfRotaion;
    private static LuaCsClosureValue _funcSetStyleById;
    private static LuaCsClosureValue _funcAddColliderTrigger;

    private static LuaCsClosureValue _funcSetSourceSize;
    private static LuaCsClosureValue _funcTurnOn;
    private static LuaCsClosureValue _funcTurnHalfOn;
    private static LuaCsClosureValue _funcTurnOff;
    private static LuaCsClosureValue _funcChangeWidthTo;
    private static LuaCsClosureValue _funcChangeLengthTo;
    private static LuaCsClosureValue _funcChangeAlphaTo;
    private static LuaCsClosureValue _funcSetSize;
    private static LuaCsClosureValue _funcSetExistDuration;
    private static LuaCsClosureValue _funcSetLength;
    private static LuaCsClosureValue _funcSetWidth;

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

            _funcSetStraightParas = new LuaCsClosureValue(LuaLib.SetBulletStraightParas);
            _funcSetSelfRotaion = new LuaCsClosureValue(LuaLib.SetBulletSelfRotation);
            _funcSetStyleById = new LuaCsClosureValue(LuaLib.SetBulletStyleById);

            _funcAddColliderTrigger = new LuaCsClosureValue(LuaLib.AddBulletColliderTriggerEvent);

            _funcSetSourceSize = new LuaCsClosureValue(LuaLib.SetLaserSourceSize);
            _funcTurnOn = new LuaCsClosureValue(LuaLib.LaserTurnOn);
            _funcTurnHalfOn = new LuaCsClosureValue(LuaLib.LaserTurnHalfOn);
            _funcTurnOff = new LuaCsClosureValue(LuaLib.LaserTurnOff);
            _funcChangeWidthTo = new LuaCsClosureValue(LuaLib.ChangeLaserWidthTo);
            _funcChangeLengthTo = new LuaCsClosureValue(LuaLib.ChangeLaserLengthTo);
            _funcChangeAlphaTo = new LuaCsClosureValue(LuaLib.ChangeLaserAlphaTo);
            _funcSetSize = new LuaCsClosureValue(LuaLib.SetLaserSize);
            _funcSetExistDuration = new LuaCsClosureValue(LuaLib.SetLaserExistDuration);
            _funcSetWidth = new LuaCsClosureValue(LuaLib.SetLaserWidth);
            _funcSetLength = new LuaCsClosureValue(LuaLib.SetLaserLength);

            _isInit = true;
        }
    }

    public static bool Get(object o, TValue key, out TValue res)
    {
        EnemyLaser bullet = (EnemyLaser)o;
        res = new TValue();
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        res.SetNValue(bullet.GetPosition().x);
                        return true;
                    }
                case "y":
                    {
                        res.SetNValue(bullet.GetPosition().y);
                        return true;
                    }
                case "rot":
                    {
                        res.SetNValue(bullet.GetRotation());
                        return true;
                    }
                case "dx":
                    {
                        res.SetNValue(bullet.dx);
                        return true;
                    }
                case "dy":
                    {
                        res.SetNValue(bullet.dy);
                        return true;
                    }
                #endregion
                #region 运动相关变量
                case "v":
                    {
                        res.SetNValue(bullet.velocity);
                        return true;
                    }
                case "vx":
                    {
                        res.SetNValue(bullet.vx);
                        return true;
                    }
                case "vy":
                    {
                        res.SetNValue(bullet.vy);
                        return true;
                    }
                case "vAngle":
                    {
                        res.SetNValue(bullet.vAngle);
                        return true;
                    }
                case "maxV":
                    {
                        res.SetNValue(bullet.maxVelocity);
                        return true;
                    }
                case "acce":
                    {
                        res.SetNValue(bullet.acce);
                        return true;
                    }
                case "accAngle":
                    {
                        res.SetNValue(bullet.accAngle);
                        return true;
                    }
                #endregion
                #region 子弹类专属变量
                case "timer":
                    {
                        res.SetNValue(bullet.timeSinceCreated);
                        return true;
                    }
                case "SetSelfRotation":
                    {
                        res.SetClCsValue(_funcSetSelfRotaion);
                        return true;
                    }
                case "SetStyleById":
                    {
                        res.SetClCsValue(_funcSetStyleById);
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
                #region 激光相关
                case "SetSourceSize":
                    {
                        res.SetClCsValue(_funcSetSourceSize);
                        return true;
                    }
                case "TurnOn":
                    {
                        res.SetClCsValue(_funcTurnOn);
                        return true;
                    }
                case "TurnHalfOn":
                    {
                        res.SetClCsValue(_funcTurnHalfOn);
                        return true;
                    }
                case "TurnOff":
                    {
                        res.SetClCsValue(_funcTurnOff);
                        return true;
                    }
                case "ChangeWidthTo":
                    {
                        res.SetClCsValue(_funcChangeWidthTo);
                        return true;
                    }
                case "ChangeLengthTo":
                    {
                        res.SetClCsValue(_funcChangeLengthTo);
                        return true;
                    }
                case "ChangeAlphaTo":
                    {
                        res.SetClCsValue(_funcChangeAlphaTo);
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
                case "SetWidth":
                    {
                        res.SetClCsValue(_funcSetWidth);
                        return true;
                    }
                case "SetLength":
                    {
                        res.SetClCsValue(_funcSetLength);
                        return true;
                    }
                case "length":
                    {
                        res.SetNValue(bullet.GetLength());
                        return true;
                    }
                case "width":
                    {
                        res.SetNValue(bullet.GetWidth());
                        return true;
                    }
                #endregion
                #region Component
                case "AddColliderTrigger":
                    {
                        res.SetClCsValue(_funcAddColliderTrigger);
                        return true;
                    }
                    #endregion
            }
        }
        res.SetSValue(string.Format("GetField from userData fail!Invalid key {0} for type {1}", key, typeof(EnemySimpleBullet).Name));
        return false;
    }

    public static bool Set(object o, TValue key, ref TValue value)
    {
        EnemyLaser bullet = (EnemyLaser)o;
        if (key.TtIsString())
        {
            switch (key.SValue())
            {
                #region 基础变量
                case "x":
                    {
                        Vector2 pos = bullet.GetPosition();
                        pos.x = (float)value.NValue;
                        bullet.SetPosition(pos);
                        return true;
                    }
                case "y":
                    {
                        Vector2 pos = bullet.GetPosition();
                        pos.y = (float)value.NValue;
                        bullet.SetPosition(pos);
                        return true;
                    }
                case "rot":
                    {
                        bullet.SetRotation((float)value.NValue);
                        return true;
                    }
                #endregion
                #region 运动相关变量
                case "v":
                    {
                        bullet.velocity = (float)value.NValue;
                        return true;
                    }
                case "vx":
                    {
                        bullet.vx = (float)value.NValue;
                        return true;
                    }
                case "vy":
                    {
                        bullet.vy = (float)value.NValue;
                        return true;
                    }
                case "vAngle":
                    {
                        bullet.vAngle = (float)value.NValue;
                        return true;
                    }
                case "maxV":
                    {
                        bullet.maxVelocity = (float)value.NValue;
                        return true;
                    }
                case "acce":
                    {
                        bullet.acce = (float)value.NValue;
                        return true;
                    }
                case "accAngle":
                    {
                        bullet.accAngle = (float)value.NValue;
                        return true;
                    }
                #endregion
                #region 子弹类专属变量
                case "orderInLayer":
                    {
                        bullet.SetOrderInLayer((int)value.NValue);
                        return true;
                    }
                case "checkCollision":
                    {
                        bullet.SetDetectCollision(value.BValue());
                        return true;
                    }
                case "checkBorder":
                    {
                        bullet.SetCheckOutOfBorder(value.BValue());
                        return true;
                    }
                #endregion
                #region 激光相关参数
                case "length":
                    {
                        bullet.SetLength((float)value.NValue);
                        return true;
                    }
                case "width":
                    {
                        bullet.SetWidth((float)value.NValue);
                        return true;
                    }
                    #endregion
            }
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(EnemySimpleBullet).Name));
        return false;
    }
}