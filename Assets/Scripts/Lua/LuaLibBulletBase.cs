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
        string id = luaState.ToString(-1);
        luaState.Pop(2);
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
        luaState.Pop(1);
        luaState.PushNumber(bullet.PosX);
        luaState.PushNumber(bullet.PosY);
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
        bullet.SetToPosition(posX, posY);
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
}
