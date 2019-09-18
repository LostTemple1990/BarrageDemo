using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 获取子弹的配置id
    /// <para>bullet</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetBulletId(ILuaState luaState)
    {
        BulletBase bullet = luaState.ToUserData(-1) as BulletBase;
        luaState.Pop(1);
        luaState.PushString(bullet.BulletId);
        return 1;
    }

    /// <summary>
    /// 根据id设置bullet的形状
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletStyleById(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        string id;
        if (luaState.Type(-1) == LuaType.LUA_TNUMBER)
        {
            id = luaState.ToNumber(-1).ToString();
        }
        else
        {
            id = luaState.ToString(-1);
        }
        bullet.SetStyleById(id);
        return 0;
    }

    /// <summary>
    /// 设置子弹在z轴上的顺序
    /// <para>bullet</para>
    /// <para>orderInLayer</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletOrderInLayer(ILuaState luaState)
    {
        BulletBase bullet = luaState.ToUserData(-2) as BulletBase;
        int order = luaState.ToInteger(-1);
        luaState.Pop(2);
        bullet.SetOrderInLayer(order);
        return 0;
    }

    #region 获取、设置子弹相关的参数

    /// <summary>
    /// 获取子弹当前位置
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetBulletPos(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-1) as EnemyBulletBase;
        luaState.PushNumber(bullet.posX);
        luaState.PushNumber(bullet.posY);
        return 2;
    }

    /// <summary>
    /// 设置子弹位置
    /// <para>bullet</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletPos(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-3) as EnemyBulletBase;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        bullet.SetPosition(posX, posY);
        return 0;
    }
    #endregion

    /// <summary>
    /// 获取子弹的属性
    /// <para>bullet</para>
    /// <para>BulletParaType paraType</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetBulletPara(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        BulletParaType paraType = (BulletParaType)luaState.ToInteger(-1);
        luaState.Pop(2);
        float value;
        if ( bullet.GetBulletPara(paraType, out value) )
        {
            luaState.PushNumber(value);
            return 1;
        }
        Logger.LogWarn("BulletPara " + paraType + " in " + bullet.Type + " is not exist!");
        return 0;
    }

    /// <summary>
    /// 设置子弹的属性
    /// <para>bullet</para>
    /// <para>BulletParaType paraType</para>
    /// <para>float value</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletPara(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-3) as EnemyBulletBase;
        BulletParaType paraType = (BulletParaType)luaState.ToInteger(-2);
        float value = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        bullet.SetBulletPara(paraType, value);
        return 0;
    }

    public static int EliminateBullet(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-1) as EnemyBulletBase;
        luaState.Pop(1);
        bullet.Eliminate();
        return 0;
    }

    /// <summary>
    /// 设置子弹的透明度
    /// <para>bullet</para>
    /// <para>float alpha 透明度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletAlpha(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        float alpha = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        bullet.SetAlpha(alpha);
        return 0;
    }

    /// <summary>
    /// 设置子弹的颜色
    /// <para>bullet</para>
    /// <para>float rValue 0~255</para>
    /// <para>float gValue 0~255</para>
    /// <para>float bValue 0~255</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletColor(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-4) as EnemyBulletBase;
        float rValue = (float)luaState.ToNumber(-3);
        float gValue = (float)luaState.ToNumber(-2);
        float bValue = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        bullet.SetColor(rValue, gValue, bValue);
        return 0;
    }

    /// <summary>
    /// 设置子弹的颜色与透明度
    /// <para>bullet</para>
    /// <para>float rValue 0~255</para>
    /// <para>float gValue 0~255</para>
    /// <para>float bValue 0~255</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletColorWithAlpha(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-5) as EnemyBulletBase;
        float rValue = (float)luaState.ToNumber(-4);
        float gValue = (float)luaState.ToNumber(-3);
        float bValue = (float)luaState.ToNumber(-2);
        float alpha = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        bullet.SetColor(rValue, gValue, bValue, alpha);
        return 0;
    }

    /// <summary>
    /// 设置子弹是否进行碰撞检测
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletDetectCollision(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        bool value = luaState.ToBoolean(-1);
        luaState.Pop(2);
        bullet.SetDetectCollision(value);
        return 0;
    }

    /// <summary>
    /// 设置子弹的消除抗性
    /// <para>bullet</para>
    /// <para>flag 不能被消除的标识</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletResistEliminatedFlag(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        int flag = luaState.ToInteger(-1);
        luaState.Pop(2);
        bullet.SetResistEliminateFlag(flag);
        return 0;
    }

    /// <summary>
    /// 获取子弹的碰撞参数
    /// <para>bullet</para>
    /// <para>collisionIndex 第n个碰撞体</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetBulletCollisionDetectParas(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        int index = luaState.ToInteger(-1);
        luaState.Pop(2);
        CollisionDetectParas collParas = bullet.GetCollisionDetectParas(index);
        luaState.PushInteger((int)collParas.type);
        luaState.PushNumber(collParas.centerPos.x);
        luaState.PushNumber(collParas.centerPos.y);
        luaState.PushNumber(collParas.radius);
        luaState.PushNumber(collParas.halfWidth);
        luaState.PushNumber(collParas.halfHeight);
        luaState.PushNumber(collParas.angle);
        return 7;
    }

    public static int AddBulletTask(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        int funcRef = InterpreterManager.GetInstance().RefLuaFunction(luaState);
        luaState.Pop(1);
        Task task = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        task.funcRef = funcRef;
        task.isFinish = false;
        task.luaState = null;
        BCCustomizedTask bc = bullet.GetComponent<BCCustomizedTask>();
        bc.AddTask(task);
        return 0;
    }

    /// <summary>
    /// 做直线运动
    /// <para>bullet 敌机子弹</para>
    /// <para>float v 速度</para>
    /// <para>float angle 速度方向</para>
    /// <para>bool isAimToPlayer 是否朝向玩家</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyBulletDoStraightMove(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-4) as EnemyBulletBase;
        float v = (float)luaState.ToNumber(-3);
        float angle = (float)luaState.ToNumber(-2);
        bool isAimToPlayer = luaState.ToBoolean(-1);
        luaState.Pop(4);
        if (isAimToPlayer)
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            angle += MathUtil.GetAngleBetweenXAxis(playerPos - bullet.GetPosition());
        }
        bullet.DoStraightMove(v, angle);
        return 0;
    }

    /// <summary>
    /// 做加速运动
    /// <para>bullet 敌机子弹</para>
    /// <para>float acce 加速度</para>
    /// <para>float angle 速度方向 or bool useVAngle使用速度方向</para>
    /// <para>bool isAimToPlayer 是否朝向玩家</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyBulletDoAcceleration(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-4) as EnemyBulletBase;
        float acce = (float)luaState.ToNumber(-3);
        float angle;
        if (luaState.Type(-2) == LuaType.LUA_TBOOLEAN )
        {
            bullet.GetBulletPara(BulletParaType.VAngel, out angle);
        }
        else
        {
            angle = (float)luaState.ToNumber(-2);
        }
        bool isAimToPlayer = luaState.ToBoolean(-1);
        luaState.Pop(4);
        if (isAimToPlayer)
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            angle += MathUtil.GetAngleBetweenXAxis(playerPos - bullet.GetPosition());
        }
        bullet.DoAcceleration(acce, angle);
        return 0;
    }

    /// <summary>
    /// 做加速运动(限制最大速度)
    /// <para>bullet 敌机子弹</para>
    /// <para>float acce 加速度</para>
    /// <para>float angle 速度方向 or bool useVAngle使用速度方向</para>
    /// <para>bool isAimToPlayer 是否朝向玩家</para>
    /// <para>float maxVelocity 最大速度限制</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyBulletDoAccelerationWithLimitation(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-5) as EnemyBulletBase;
        float acce = (float)luaState.ToNumber(-4);
        float angle;
        if (luaState.Type(-3) == LuaType.LUA_TBOOLEAN)
        {
            bullet.GetBulletPara(BulletParaType.VAngel, out angle);
        }
        else
        {
            angle = (float)luaState.ToNumber(-3);
        }
        bool isAimToPlayer = luaState.ToBoolean(-2);
        float maxVelocity = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        if (isAimToPlayer)
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            angle += MathUtil.GetAngleBetweenXAxis(playerPos - bullet.GetPosition());
        }
        bullet.DoAccelerationWithLimitation(acce, angle, maxVelocity);
        return 0;
    }

    /// <summary>
    /// 设置敌机子弹的直线运动参数
    /// <para>bullet</para>
    /// <para>v</para>
    /// <para>vAngle</para>
    /// <para>bool isAimToPlayer</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// <para>maxV</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyBulletSetStraightParas(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-6) as EnemyBulletBase;
        float v = (float)luaState.ToNumber(-5);
        float angle = (float)luaState.ToNumber(-4);
        bool isAimToPlayer = luaState.ToBoolean(-3);
        float acce = (float)luaState.ToNumber(-2);
        float accAngle;
        if (luaState.Type(-1) == LuaType.LUA_TBOOLEAN)
        {
            accAngle = angle;
        }
        else
        {
            accAngle = (float)luaState.ToNumber(-1);
        }
        if (isAimToPlayer)
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            float relAngle = MathUtil.GetAngleBetweenXAxis(playerPos - bullet.GetPosition());
            angle += relAngle;
            accAngle += relAngle;
        }
        bullet.SetStraightParas(v, angle, acce, accAngle);
        return 0;
    }

    /// <summary>
    /// 物体做极坐标运动
    /// <para>bullet 敌机子弹</para>
    /// <para>float radius 起始半径</para>
    /// <para>float angle 起始角度</para>
    /// <para>float deltaR 半径增量</para>
    /// <para>float omega 角速度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyBulletSetCurvedParas(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-5) as EnemyBulletBase;
        float radius = (float)luaState.ToNumber(-4);
        float angle = (float)luaState.ToNumber(-3);
        float deltaR = (float)luaState.ToNumber(-2);
        float omega = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        bullet.SetPolarParas(radius, angle, deltaR, omega);
        return 0;
    }
}
