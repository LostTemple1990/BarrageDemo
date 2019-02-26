using UniLua;
using UnityEngine;

public class EnemySimpleBulletLuaInterface
{
    public static bool Get(object o,TValue key,out TValue res)
    {
        EnemySimpleBullet bullet = (EnemySimpleBullet)o;
        res = new TValue();
        if ( key.TtIsString() )
        {
            switch ( (string)key.OValue )
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
                        res.SetNValue(bullet.Dx);
                        return true;
                    }
                case "dy":
                    {
                        res.SetNValue(bullet.Dy);
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
            switch ((string)key.OValue)
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
                #endregion
            }
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid key {0} for type {1}", key, typeof(EnemySimpleBullet).Name));
        return false;
    }
}