using UnityEngine;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 根据类型创建一个collider
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateObjectColliderByType(ILuaState luaState)
    {
        eColliderType type = (eColliderType)luaState.ToInteger(-1);
        ObjectColliderBase collider = ColliderManager.GetInstance().CreateColliderByType(type);
        luaState.PushLightUserData(collider);
        return 1;
    }

    /// <summary>
    /// 创建自定义的collider
    /// <para>customizedName 自定义的类型名称</para>
    /// <para>eColliderType collider的形状类别</para>
    /// <para>参数...</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedCollider(ILuaState luaState)
    {
        int top = luaState.GetTop();
        int numArgs = top - 2;
        string customizedName = luaState.ToString(-top);
        eColliderType type = (eColliderType)luaState.ToInteger(-top + 1);
        int funcRef = InterpreterManager.GetInstance().GetCustomizedFuncRef(customizedName, eCustomizedType.Collider, eCustomizedFuncRefType.Init);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        ObjectColliderBase collider = ColliderManager.GetInstance().CreateColliderByType(type);
        luaState.PushLightUserData(collider);
        // 复制参数
        int copyIndex = -numArgs - 2;
        for (int i = 0; i < numArgs; i++)
        {
            luaState.PushValue(copyIndex);
        }
        luaState.Call(numArgs + 1, 0);
        // 返回结果
        luaState.PushLightUserData(collider);
        return 1;
    }

    /// <summary>
    /// 根据类型创建一个引力场
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateGravitationFieldByType(ILuaState luaState)
    {
        eColliderType type = (eColliderType)luaState.ToInteger(-1);
        ObjectColliderBase collider = ColliderManager.GetInstance().CreateGravitationFieldByType(type);
        luaState.PushLightUserData(collider);
        return 1;
    }

    /// <summary>
    /// 初始化引力场基本属性
    /// <para>field 引力场</para>
    /// <para>fieldType 引力场类型</para>
    /// <para>velocity 速度</para>
    /// <para>velocityOffset 速度偏移</para>
    /// <para>vAngle 速度方向</para>
    /// <para>vAngleOffset 速度方向偏移量</para>
    /// <para>acce 加速度</para>
    /// <para>acceOffset 加速度偏移量</para>
    /// <para>accAngle 加速度方向</para>
    /// <para>accAngleOffset 加速度方向偏移</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int InitGravitationField(ILuaState luaState)
    {
        IGravitationField field = luaState.ToUserData(-10) as IGravitationField;
        int fieldType = luaState.ToInteger(-9);
        float velocity = (float)luaState.ToNumber(-8);
        float velocityOffset = (float)luaState.ToNumber(-7);
        float vAngle = (float)luaState.ToNumber(-6);
        float vAngleOffset = (float)luaState.ToNumber(-5);
        float acce = (float)luaState.ToNumber(-4);
        float acceOffset = (float)luaState.ToNumber(-3);
        float accAngle = (float)luaState.ToNumber(-2);
        float accAngleOffset = (float)luaState.ToNumber(-1);
        luaState.Pop(10);
        field.Init(fieldType, velocity, velocityOffset, vAngle, vAngleOffset, acce, acceOffset, accAngle, accAngleOffset);
        return 0;
    }

    /// <summary>
    /// 设置物体碰撞器的位置
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderToPos(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-3) as ObjectColliderBase;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        collider.SetPosition(posX, posY);
        return 0;
    }

    /// <summary>
    /// 设置碰撞器的tag
    /// <para>collider</para>
    /// <para>stirng tag 标签</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderTag(ILuaState luaState)
    {
        IObjectCollider collider = luaState.ToUserData(-2) as IObjectCollider;
        string tag = luaState.ToString(-1);
        luaState.Pop(2);
        collider.SetTag(tag);
        return 0;
    }

    /// <summary>
    /// 设置物体碰撞器的尺寸
    /// <para>arg0 矩形碰撞器的宽、圆碰撞器的半径</para>
    /// <para>arg1 矩形碰撞器的高，圆碰撞器不使用</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderSize(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-3) as ObjectColliderBase;
        float arg0 = (float)luaState.ToNumber(-2);
        float arg1 = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        collider.SetSize(arg0, arg1);
        return 0;
    }

    /// <summary>
    /// 设置物体碰撞器的碰撞组
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderColliderGroup(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        int colliderGroup = luaState.ToInteger(-1);
        collider.SetColliderGroup((eColliderGroup)colliderGroup);
        return 0;
    }

    /// <summary>
    /// 设置物体碰撞器的生存时间
    /// <para>duration 生存时间</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderExistDuration(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        int duration = luaState.ToInteger(-1);
        luaState.Pop(2);
        collider.SetExistDuration(duration);
        return 0;
    }

    /// <summary>
    /// 将ObjectCollider缩放至指定尺寸
    /// <para>toWidth 圆的半径、矩形的宽</para>
    /// <para>toHeight</para>
    /// <para>duration 持续时间</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ObjectColliderScaleToSize(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-4) as ObjectColliderBase;
        float toWidth = (float)luaState.ToNumber(-3);
        float toHeight = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        collider.ScaleToSize(toWidth, toHeight, duration);
        return 0;
    }

    /// <summary>
    /// ObjectCollider清除自身
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ObjectColliderClearSelf(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-1) as ObjectColliderBase;
        luaState.Pop(1);
        collider.ClearSelf();
        return 0;
    }

    /// <summary>
    /// 设置ObjectCollider的清除类型
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderEliminateType(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        eEliminateDef eliminateType = (eEliminateDef)luaState.ToInteger(-1);
        luaState.Pop(2);
        collider.SetEliminateType(eliminateType);
        return 0;
    }

    /// <summary>
    /// 设置ObjectCollider击中敌机的伤害
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetObjectColliderHitEnemyDamage(ILuaState luaState)
    {
        ObjectColliderBase collider = luaState.ToUserData(-2) as ObjectColliderBase;
        int damage = luaState.ToInteger(-1);
        luaState.Pop(2);
        collider.SetHitEnemyDamage(damage);
        return 0;
    }

    /// <summary>
    /// 根据tag移除引力场
    /// <para>string tag</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int RemoveGravitationFieldByTag(ILuaState luaState)
    {
        string tag = luaState.ToString(-1);
        ColliderManager.GetInstance().RemoveGravitationFieldByTag(tag);
        return 0;
    }

    /// <summary>
    /// 根据tag移除碰撞器
    /// <para>string tag</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int RemoveObjectColliderByTag(ILuaState luaState)
    {
        string tag = luaState.ToString(-1);
        ColliderManager.GetInstance().RemoveColliderByTag(tag);
        return 0;
    }
}
