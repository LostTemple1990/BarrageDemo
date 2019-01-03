using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib 
{
    /// <summary>
    /// 创建可发射的直线激光
    /// <para>texture 暂时指向于etama9的激光贴图</para>
    /// <para>laserLen 激光长度</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateLinearLaser(ILuaState luaState)
    {
        string id = luaState.ToString(-4);
        int laserLen = luaState.ToInteger(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        EnemyLinearLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_LinearLaser) as EnemyLinearLaser;
        laser.SetStyleById(id);
        laser.SetLength(laserLen);
        laser.SetToPosition(posX, posY);
        luaState.PushLightUserData(laser);
        return 1;
    }

    /// <summary>
    /// 设置直线激光的长度
    /// <para>laser</para>
    /// <para>int laserLen 激光的长度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetLinearLaserLength(ILuaState luaState)
    {
        EnemyLinearLaser laser = luaState.ToUserData(-2) as EnemyLinearLaser;
        int laserLen = luaState.ToInteger(-1);
        luaState.Pop(2);
        laser.SetLength(laserLen);
        return 0;
    }

    /// <summary>
    /// 设置直线激光的速度等基础属性
    /// <para>laser 直线激光本体</para>
    /// <para>velocity 速度</para>
    /// <para>angle 方向</para>
    /// <para>isAimToPlayer 是否朝向玩家</para>
    /// <para>acce 加速度</para>
    /// <para>maxVelocity 最大速度限制</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int LinearLaserDoStraightMove(ILuaState luaState)
    {
        EnemyLinearLaser laser = luaState.ToUserData(-6) as EnemyLinearLaser;
        float velocity = (float)luaState.ToNumber(-5);
        float angle = (float)luaState.ToNumber(-4);
        bool isAimToPlayer = luaState.ToBoolean(-3);
        if ( isAimToPlayer )
        {
            angle += MathUtil.GetAngleBetweenXAxis(Global.PlayerPos - laser.GetPosition(), false);
        }
        float acce = (float)luaState.ToNumber(-2);
        float maxVelocity = (float)luaState.ToNumber(-1);
        luaState.Pop(6);
        laser.DoStraightMove(velocity, angle, acce, maxVelocity);
        return 0;
    }

    /// <summary>
    /// 设置激光的头部是否可用
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetLinearLaserHeadEnable(ILuaState luaState)
    {
        EnemyLinearLaser laser = luaState.ToUserData(-2) as EnemyLinearLaser;
        bool isEnable = luaState.ToBoolean(-1);
        luaState.Pop(2);
        laser.SetHeadEnable(isEnable);
        return 0;
    }

    /// <summary>
    /// 设置激光的发射源是否显示
    /// </summary>
    /// <param name="luaState"></param>
    /// <para>isEnable 是否显示</para>
    /// <para>sourceIndex 发射源的图像索引</para>
    /// <returns></returns>
    public static int SetLinearLaserSourceEnable(ILuaState luaState)
    {
        EnemyLinearLaser laser = luaState.ToUserData(-3) as EnemyLinearLaser;
        bool isEnable = luaState.ToBoolean(-2);
        int sourceIndex = luaState.ToInteger(-1);
        luaState.Pop(3);
        laser.SetSourceEnable(isEnable, sourceIndex);
        return 0;
    }

    public static int CreateCustomizedLinearLaser(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        luaState.Pop(1);
        string customizedName = luaState.ToString(-1-numArgs);
        int initFuncRef = InterpreterManager.GetInstance().GetInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, initFuncRef);
        luaState.Replace(-2 - numArgs);
        // 将本体插入执行栈中
        EnemyLinearLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_LinearLaser) as EnemyLinearLaser;
        laser.AddComponent<BCCustomizedTask>();
        luaState.PushLightUserData(laser);
        luaState.Insert(-1-numArgs);
        luaState.Call(numArgs + 1, 0);
        // 将返回值压入栈中
        luaState.PushLightUserData(laser);
        return 1;
    }

    /// <summary>
    /// 创建曲线激光
    /// <para>texture 暂时指向于etama9的激光贴图</para>
    /// <para>laserLen 激光长度</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCurveLaser(ILuaState luaState)
    {
        string texture = luaState.ToString(-4);
        int laserLen = luaState.ToInteger(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        EnemyCurveLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_CurveLaser) as EnemyCurveLaser;
        laser.SetBulletTexture(texture);
        laser.SetLength(laserLen);
        laser.SetToPosition(posX, posY);
        luaState.PushLightUserData(laser);
        return 1;
    }

    /// <summary>
    /// 曲线激光的直线运动
    /// <para>v 速度</para>
    /// <para>vAngle 速度方向</para>
    /// <para>bool isAimToPlayer 是否指向玩家</para>
    /// <para>acce 加速度</para>
    /// <para>accAngle 加速度方向 Consts.VelocityAngle时跟速度方向一致</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetCurveLaserStraightParas(ILuaState luaState)
    {
        EnemyCurveLaser laser = luaState.ToUserData(-6) as EnemyCurveLaser;
        float v = (float)luaState.ToNumber(-5);
        float vAngle = (float)luaState.ToNumber(-4);
        bool isAimToPlayer = luaState.ToBoolean(-3);
        float acce = (float)luaState.ToNumber(-2);
        float accAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(6);
        if (isAimToPlayer)
        {
            vAngle += MathUtil.GetAngleBetweenXAxis(Global.PlayerPos.x - laser.PosX, Global.PlayerPos.y - laser.PosY, false);
        }
        laser.DoMove(v, vAngle, acce, accAngle);
        return 0;
    }

    /// <summary>
    /// 加速运动
    /// <para>acce 加速度</para>
    /// <para>accAngle 加速度方向 Consts.VelocityAngle时跟速度方向一致</para>
    /// <para>int duration 加速度时间</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetCurveLaserAcceParas(ILuaState luaState)
    {
        EnemyCurveLaser laser = luaState.ToUserData(-4) as EnemyCurveLaser;
        float acce = (float)luaState.ToNumber(-3);
        float accAngle = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        laser.SetAcceParas(acce, accAngle, duration);
        return 0;
    }

    /// <summary>
    /// 曲线激光的曲线运动
    /// <para>radius 初始半径</para>
    /// <para>angle 初始角度</para>
    /// <para>deltaR 半径增量</para>
    /// <para>omiga 角速度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetCurveLaserCurveParas(ILuaState luaState)
    {
        EnemyCurveLaser laser = luaState.ToUserData(-5) as EnemyCurveLaser;
        float radius = (float)luaState.ToNumber(-4);
        float angle = (float)luaState.ToNumber(-3);
        float deltaR = (float)luaState.ToNumber(-2);
        float omiga = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        laser.SetCurveParas(radius, angle, deltaR, omiga);
        return 0;
    }


    public static int CreateCustomizedCurveLaser(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        luaState.Pop(1);
        string customizedName = luaState.ToString(-5 - numArgs);
        string textureName = luaState.ToString(-4 - numArgs);
        int laserLen = luaState.ToInteger(-3 - numArgs);
        float posX = (float)luaState.ToNumber(-2 - numArgs);
        float posY = (float)luaState.ToNumber(-1 - numArgs);
        int initFuncRef = InterpreterManager.GetInstance().GetInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, initFuncRef);
        // 将本体插入执行栈中
        EnemyCurveLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_CurveLaser) as EnemyCurveLaser;
        laser.AddComponent<BCCustomizedTask>();
        luaState.PushLightUserData(laser);
        luaState.Replace(-3 - numArgs);
        luaState.Replace(-3 - numArgs);
        luaState.Call(numArgs + 1, 0);
        // 将多出的参数弹出
        luaState.Pop(3);
        // 将返回值压入栈中
        luaState.PushLightUserData(laser);
        return 1;
    }

    /// <summary>
    /// 设置曲线激光的长度
    /// <para>laser 曲线激光本体</para>
    /// <para>len 设置的长度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetCurveLaserLength(ILuaState luaState)
    {
        EnemyCurveLaser laser = luaState.ToUserData(-2) as EnemyCurveLaser;
        int len = luaState.ToInteger(-1);
        luaState.Pop(2);
        laser.SetLength(len);
        return 0;
    }

    /// <summary>
    /// 设置曲线激光的宽带已经碰撞检测宽度
    /// <para>laser 曲线激光本体</para>
    /// <para>len 设置的宽度(总宽度/2）</para>
    /// <para>collisionHalfWidth 碰撞检测的宽度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetCurveLaserWidth(ILuaState luaState)
    {
        EnemyCurveLaser laser = luaState.ToUserData(-3) as EnemyCurveLaser;
        float halfWidth = (float)luaState.ToNumber(-2);
        float collisionHalfWidth = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        laser.SetWidth(halfWidth, collisionHalfWidth);
        return 0;
    }
}
