using UniLua;
using UnityEngine;

public partial class LuaLib
{
    /// <summary>
    /// 创建一个用于背景前景显示的精灵对象
    /// <para>spName string 精灵对象的资源名称</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateBgSpriteObject(ILuaState luaState)
    {
        string spName = luaState.ToString(-1);
        luaState.Pop(1);
        BgSpriteObject spObj = BackgroundManager.GetInstance().CreateBgSpriteObject(spName);
        luaState.PushLightUserData(spObj);
        return 1;
    }

    /// <summary>
    /// 设置前景对象的位置
    /// <para>spObj 对象本体</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBgSprteObjectPos(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-3) as BgSpriteObject;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        spObj.SetToPos(posX, posY);
        return 0;
    }

    /// <summary>
    /// 设置前景对象的缩放
    /// <para>spObj 对象本体</para>
    /// <para>scaleX</para>
    /// <para>scaleY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBgSpriteObjectScale(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-3) as BgSpriteObject;
        float scaleX = (float)luaState.ToNumber(-2);
        float scaleY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        spObj.SetScale(scaleX, scaleY, 1);
        return 0;
    }

    /// <summary>
    /// 设置前景对象的速度
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBgSpriteObjectVelocity(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-3) as BgSpriteObject;
        float velocity = (float)luaState.ToNumber(-2);
        float angle = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        spObj.SetVelocity(velocity, angle);
        return 0;
    }

    /// <summary>
    /// 加速运动
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBgSpriteObjectAcce(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-3) as BgSpriteObject;
        float acce = (float)luaState.ToNumber(-2);
        float angle = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        spObj.SetAcce(acce, angle);
        return 0;
    }

    /// <summary>
    /// 有限制的加速运动
    /// <para>acce</para>
    /// <para>angle</para>
    /// <para>duration</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBgSpriteObjectAcceWithLimitation(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-4) as BgSpriteObject;
        float acce = (float)luaState.ToNumber(-3);
        float angle = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        spObj.SetAcce(acce, angle, duration);
        return 0;
    }

    /// <summary>
    /// 设置前景对象的自旋速度
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBgSpriteObjectSelfRotateAngle(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-4) as BgSpriteObject;
        float xAngle = (float)luaState.ToNumber(-3);
        float yAngle = (float)luaState.ToNumber(-2);
        float zAngle = (float)luaState.ToNumber(-2);
        luaState.Pop(4);
        spObj.SetSelfRotateAngle(new Vector3(xAngle,yAngle,zAngle));
        return 0;
    }

    /// <summary>
    /// 设置前景对象的当前旋转角度
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBgSpriteObjectRotation(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-4) as BgSpriteObject;
        float xAngle = (float)luaState.ToNumber(-3);
        float yAngle = (float)luaState.ToNumber(-2);
        float zAngle = (float)luaState.ToNumber(-2);
        luaState.Pop(4);
        spObj.SetRotation(new Vector3(xAngle, yAngle, zAngle));
        return 0;
    }

    /// <summary>
    /// 延时之后前景对象逐渐消失的效果
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int BgSpriteObjectDoFade(ILuaState luaState)
    {
        BgSpriteObject spObj = luaState.ToUserData(-3) as BgSpriteObject;
        int fadeDuration = luaState.ToInteger(-2);
        int fadeDelay = luaState.ToInteger(-1);
        luaState.Pop(3);
        spObj.DoFade(fadeDuration, fadeDelay);
        return 0;
    }
}
