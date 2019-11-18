using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 根据id创建SimpleBullet
    /// <para>id 配置里面的id</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSimpleBulletById(ILuaState luaState)
    {
        string sysId;
        if (luaState.Type(-3) == LuaType.LUA_TNUMBER)
        {
            sysId = luaState.ToNumber(-3).ToString();
        }
        else
        {
            sysId = luaState.ToString(-3);
        }
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        EnemySimpleBullet bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_Simple) as EnemySimpleBullet;
        bullet.SetStyleById(sysId);
        bullet.SetPosition(posX, posY);
        luaState.PushLightUserData(bullet);
        return 1;
    }

    /// <summary>
    /// 设置子弹生成时是否播放生成特效
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletAppearEffectAvailable(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-2) as EnemySimpleBullet;
        bool isAvailable = luaState.ToBoolean(-1);
        bullet.SetAppearEffectAvailable(isAvailable);
        return 0;
    }

    /// <summary>
    /// 禁用子弹的出现特效
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DisableBulletAppearEffect(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-1) as EnemySimpleBullet;
        bullet.SetAppearEffectAvailable(false);
        return 0;
    }

    /// <summary>
    /// 设置子弹是否自旋
    /// <para>SimpleBullet bullet</para>
    /// <para>float omega 自旋角度，为0说明不自旋</para>
    /// <para></para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletSelfRotation(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-2) as EnemySimpleBullet;
        float omega = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        bullet.SetSelfRotation(omega);
        return 0;
    }

    /// <summary>
    /// 设置子弹的缩放
    /// <para>bullet</para>
    /// <para>(1)scale 缩放值</para>
    /// <para>(2)float scaleX,float scaleY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletScale(ILuaState luaState)
    {
        int top = luaState.GetTop();
        if (top == 2)
        {
            EnemySimpleBullet bullet = luaState.ToUserData(-2) as EnemySimpleBullet;
            float scale = (float)luaState.ToNumber(-1);
            bullet.SetScale(scale);
        }
        else
        {
            EnemySimpleBullet bullet = luaState.ToUserData(-3) as EnemySimpleBullet;
            float scaleX = (float)luaState.ToNumber(-2);
            float scaleY = (float)luaState.ToNumber(-1);
            bullet.SetBulletPara(BulletParaType.ScaleX, scaleX);
            bullet.SetBulletPara(BulletParaType.ScaleY, scaleY);
        }
        return 0;
    }

    /// <summary>
    /// SimpleBullet延迟delay帧后经过duration帧缩放至toScale
    /// <para>SimpleBullet bullet</para>
    /// <para>float toScale</para>
    /// <para>int duration</para>
    /// <para>int delay</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int BulletDoScale(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-4) as EnemySimpleBullet;
        float toScale = (float)luaState.ToNumber(-3);
        int delay = luaState.ToInteger(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        bullet.DoScale(toScale, delay, duration);
        return 0;
    }

    /// <summary>
    /// <para>bullet</para>
    /// <para>velocity</para>
    /// <para>vAngle</para>
    /// <para>isAimToPlayer</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletStraightParas(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-6) as EnemySimpleBullet;
        float velocity = (float)luaState.ToNumber(-5);
        float vAngle = (float)luaState.ToNumber(-4);
        bool isAimToPlayer = luaState.ToBoolean(-3);
        float acce = (float)luaState.ToNumber(-2);
        float accAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(6);
        // 设置子弹线性运动
        if (isAimToPlayer)
        {
            vAngle += MathUtil.GetAngleBetweenXAxis(Global.PlayerPos.x - bullet.posX, Global.PlayerPos.y - bullet.posY);
        }
        bullet.DoStraightMove(velocity, vAngle);
        bullet.DoAcceleration(acce, accAngle);
        return 0;
    }

    /// <summary> 赋予子弹加速度
    /// <para>bullet</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DoBulletAcceleration(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-3) as EnemySimpleBullet;
        float acce = (float)luaState.ToNumber(-2);
        float accAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        bullet.DoAcceleration(acce, accAngle);
        return 0;
    }

    /// <summary> 赋予子弹加速度
    /// <para>bullet</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// <para>maxVelocity  速度最大值</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DoBulletAccelerationWithLimitation(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-4) as EnemySimpleBullet;
        float acce = (float)luaState.ToNumber(-3);
        float accAngle = (float)luaState.ToNumber(-2);
        float maxVelocity = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        bullet.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
        return 0;
    }

    /// <summary>
    /// <para>bullet</para>
    /// <para>radius</para>
    /// <para>curveAngle</para>
    /// <para>deltaR</para>
    /// <para>omiga</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletCurvePara(ILuaState luaState)
    {
        EnemySimpleBullet bullet = luaState.ToUserData(-5) as EnemySimpleBullet;
        float radius = (float)luaState.ToNumber(-4);
        float curveAngle = (float)luaState.ToNumber(-3);
        float deltaR = (float)luaState.ToNumber(-2);
        float omiga = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        bullet.SetPolarParas(radius, curveAngle, deltaR, omiga);
        return 0;
    }

    /// <summary>
    /// 创建自定义的simpleBullet
    /// <para>customizedName 自定义的子弹名称</para>
    /// <para>id 默认的id</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedBullet2(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        string customizedName = luaState.ToString(-5 - numArgs);
        string sysId;
        if (luaState.Type(-4 - numArgs) == LuaType.LUA_TNUMBER)
        {
            sysId = luaState.ToNumber(-4 - numArgs).ToString();
        }
        else
        {
            sysId = luaState.ToString(-4 - numArgs);
        }
        float posX = (float)luaState.ToNumber(-3 - numArgs);
        float posY = (float)luaState.ToNumber(-2 - numArgs);
        luaState.Pop(1);
        EnemySimpleBullet bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_Simple) as EnemySimpleBullet;
        bullet.SetStyleById(sysId);
        bullet.SetPosition(posX, posY);
        // 设置自定义的数据
        BCCustomizedTask bc = bullet.AddOrGetComponent<BCCustomizedTask>();
        int funcRef = InterpreterManager.GetInstance().GetBulletInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        if (!luaState.IsFunction(-1))
        {
            Logger.LogError("InitFuncRef of " + customizedName + " is not point to a function");
        }
        luaState.PushLightUserData(bullet);
        luaState.Replace(-3 - numArgs);
        luaState.Replace(-3 - numArgs);
        luaState.Call(numArgs + 1, 0);
        // 弹出剩余两个参数
        luaState.Pop(2);
        luaState.PushLightUserData(bullet);
        return 1;
    }

    /// <summary>
    /// 创建自定义的simpleBullet
    /// <para>customizedName 自定义的子弹名称</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// <para>args...</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedBullet(ILuaState luaState)
    {
        int numArgs = luaState.GetTop() - 3;
        string customizedName = luaState.ToString(-3 - numArgs);
        float posX = (float)luaState.ToNumber(-2 - numArgs);
        float posY = (float)luaState.ToNumber(-1 - numArgs);
        EnemySimpleBullet bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_Simple) as EnemySimpleBullet;
        bullet.SetPosition(posX, posY);
        // 设置自定义的数据
        BCCustomizedTask bc = bullet.AddOrGetComponent<BCCustomizedTask>();
        int funcRef = InterpreterManager.GetInstance().GetBulletInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        if (!luaState.IsFunction(-1))
        {
            Logger.LogError("InitFuncRef of " + customizedName + " is not point to a function");
        }
        luaState.PushLightUserData(bullet);
        for (int i=0;i<numArgs;i++)
        {
            luaState.PushValue(-2 - numArgs);
        }
        luaState.Call(numArgs + 1, 0);
        luaState.PushLightUserData(bullet);
        return 1;
    }
}
