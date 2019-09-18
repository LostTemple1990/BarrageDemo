using UniLua;
using UnityEngine;

public class EnemySimpleBulletLuaInterface
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
    private static LuaCsClosureValue _funcSetResistEliminatedTypes;
    private static LuaCsClosureValue _funcChangeProperty;


    public static void Init()
    {
        if ( !_isInit )
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
            _funcSetSelfRotaion = new LuaCsClosureValue(LuaLib.SetBulletSelfRotation);
            _funcSetStyleById = new LuaCsClosureValue(LuaLib.SetBulletStyleById);
            _funcSetResistEliminatedTypes = new LuaCsClosureValue(LuaLib.SetBulletResistEliminatedFlag);
            _funcChangeProperty = new LuaCsClosureValue(LuaLib.AddBulletParaChangeEvent);

            _isInit = true;
        }
    }

    public static bool Get(object o,TValue key,out TValue res)
    {
        EnemySimpleBullet bullet = (EnemySimpleBullet)o;
        res = new TValue();
        if ( key.TtIsString() )
        {
            switch ( key.SValue() )
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
                        res.SetNValue(bullet.Velocity);
                        return true;
                    }
                case "vx":
                    {
                        res.SetNValue(bullet.Vx);
                        return true;
                    }
                case "vy":
                    {
                        res.SetNValue(bullet.Vy);
                        return true;
                    }
                case "vAngle":
                    {
                        res.SetNValue(bullet.VAngle);
                        return true;
                    }
                case "maxV":
                    {
                        res.SetNValue(bullet.MaxVelocity);
                        return true;
                    }
                case "acce":
                    {
                        res.SetNValue(bullet.Acce);
                        return true;
                    }
                case "accAngle":
                    {
                        res.SetNValue(bullet.AccAngle);
                        return true;
                    }
                #endregion
                #region 子弹类专属变量
                case "timer":
                    {
                        res.SetNValue(bullet.timeSinceCreated);
                        return true;
                    }
                case "alpha":
                    {
                        res.SetNValue(bullet.alpha);
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
                case "SetResistEliminatedTypes":
                    {
                        res.SetClCsValue(_funcSetResistEliminatedTypes);
                        return true;
                    }
                case "ChangeProperty":
                    {
                        res.SetClCsValue(_funcChangeProperty);
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
            }
        }
        res.SetSValue(string.Format("GetField from userData fail!Invalid key {0} for type {1}", key, typeof(EnemySimpleBullet).Name));
        return false;
    }

    public static bool Set(object o, TValue key, ref TValue value)
    {
        EnemySimpleBullet bullet = (EnemySimpleBullet)o;
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
                        bullet.Velocity = (float)value.NValue;
                        return true;
                    }
                case "vx":
                    {
                        bullet.Vx = (float)value.NValue;
                        return true;
                    }
                case "vy":
                    {
                        bullet.Vy = (float)value.NValue;
                        return true;
                    }
                case "vAngle":
                    {
                        bullet.VAngle = (float)value.NValue;
                        return true;
                    }
                case "maxV":
                    {
                        bullet.MaxVelocity = (float)value.NValue;
                        return true;
                    }
                case "acce":
                    {
                        bullet.Acce = (float)value.NValue;
                        return true;
                    }
                case "accAngle":
                    {
                        bullet.AccAngle = (float)value.NValue;
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
                case "navi":
                    {
                        bullet.SetRotatedByVelocity(value.BValue());
                        return true;
                    }
                case "checkBorder":
                    {
                        bullet.SetCheckOutOfBorder(value.BValue());
                        return true;
                    }
                case "alpha":
                    {
                        bullet.SetAlpha((float)value.NValue);
                        return true;
                    }
                #endregion
            }
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(EnemySimpleBullet).Name));
        return false;
    }
}