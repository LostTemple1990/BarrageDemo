using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    public static int CreateEffectByType(ILuaState luaState)
    {
        EffectType type = (EffectType)luaState.ToInteger(-1);
        STGEffectBase effect = EffectsManager.GetInstance().CreateEffectByType(type);
        luaState.Pop(1);
        luaState.PushLightUserData(effect);
        return 1;
    }

    public static int SetEffectToPos(ILuaState luaState)
    {
        STGEffectBase effect = luaState.ToUserData(-3) as STGEffectBase;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        effect.SetToPosition(posX, posY);
        return 0;
    }

    public static int SetEffectFinish(ILuaState luaState)
    {
        STGEffectBase effect = luaState.ToUserData(-1) as STGEffectBase;
        luaState.Pop(1);
        effect.FinishEffect();
        return 0;
    }

    /// <summary>
    /// 根据名称拿到effect
    /// <para>string name 特效指定的名称</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetEffectByName(ILuaState luaState)
    {
        string name = luaState.ToString(-1);
        luaState.Pop(1);
        STGEffectBase effect = EffectsManager.GetInstance().GetEffectByName(name);
        luaState.PushLightUserData(effect);
        return 1;
    }

    public static int SetEffectFinishByName(ILuaState luaState)
    {
        string name = luaState.ToString(-1);
        luaState.Pop(1);
        EffectsManager.GetInstance().FinishEffectByName(name);
        return 0;
    }

    /// <summary>
    /// 创建一个精灵特效
    /// <para>string effectSpName</para>
    /// <para>float scaleX</para>
    /// <para>float scaleY</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSpriteEffect(ILuaState luaState)
    {
        string effectSpName = luaState.ToString(-5);
        float scaleX = (float)luaState.ToNumber(-4);
        float scaleY = (float)luaState.ToNumber(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
        effect.SetSprite(effectSpName);
        effect.SetScale(scaleX, scaleY);
        effect.SetToPosition(posX, posY);
        luaState.PushLightUserData(effect);
        return 1;
    }

    /// <summary>
    /// 根据属性创建指定的SpriteEffect
    /// <para>atlasName</para>
    /// <para>spriteName</para>
    /// <para>cached 是否缓存</para>
    /// <para>layerId 层级</para>
    /// <para>在层级中的顺序</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSpriteEffectWithProps(ILuaState luaState)
    {
        string atlasName = luaState.ToString(-6);
        string spriteName = luaState.ToString(-5);
        eBlendMode blendMode = (eBlendMode)luaState.ToInteger(-4);
        LayerId layerId = (LayerId)luaState.ToInteger(-3);
        bool cached = luaState.ToBoolean(-2);
        int orderInLayer = luaState.ToInteger(-1);
        luaState.Pop(6);
        STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
        effect.SetSprite(atlasName, spriteName, blendMode, layerId, cached);
        effect.SetOrderInLayer(orderInLayer);
        luaState.PushLightUserData(effect);
        return 1;
    }

    /// <summary>
    /// 设置SpriteEffect的缩放
    /// <para>scaleX</para>
    /// <para>scaleY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetSpriteEffectScale(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-3) as STGSpriteEffect;
        float scaleX = (float)luaState.ToNumber(-2);
        float scaleY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        effect.SetScale(scaleX, scaleY);
        return 0;
    }

    /// <summary>
    /// 设置spriteEffect的颜色
    /// <para>effect</para>
    /// <para>rValue</para>
    /// <para>gValue</para>
    /// <para>bValue</para>
    /// <para>optional aValue</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetSpriteEffectColor(ILuaState luaState)
    {
        // 检测带不带alpha
        if ( luaState.Type(-4) != LuaType.LUA_TLIGHTUSERDATA )
        {
            STGSpriteEffect effect = luaState.ToUserData(-5) as STGSpriteEffect;
            float rValue = (float)luaState.ToNumber(-4);
            float gValue = (float)luaState.ToNumber(-3);
            float bValue = (float)luaState.ToNumber(-2);
            float aValue = (float)luaState.ToNumber(-1);
            luaState.Pop(5);
            effect.SetSpriteColor(rValue, gValue, bValue, aValue);
        }
        else
        {
            STGSpriteEffect effect = luaState.ToUserData(-4) as STGSpriteEffect;
            float rValue = (float)luaState.ToNumber(-3);
            float gValue = (float)luaState.ToNumber(-2);
            float bValue = (float)luaState.ToNumber(-1);
            luaState.Pop(4);
            effect.SetSpriteColor(rValue, gValue, bValue);
        }
        return 0;
    }

    public static int SpriteEffectScaleWidth(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-4) as STGSpriteEffect;
        float toScale = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode scaleMode = (InterpolationMode)luaState.ToInteger(-1);
        luaState.Pop(4);
        effect.DoScaleWidth(toScale, duration, scaleMode);
        return 0;
    }

    public static int SpriteEffectScaleHeight(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-4) as STGSpriteEffect;
        float toScale = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode scaleMode = (InterpolationMode)luaState.ToInteger(-1);
        luaState.Pop(4);
        effect.DoScaleHeight(toScale, duration, scaleMode);
        return 0;
    }

    /// <summary>
    /// 设置SpriteEffect的旋转角度
    /// <para>effect</para>
    /// <para>angle 旋转的角度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetSpriteRotation(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-2) as STGSpriteEffect;
        float angle = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        effect.SetRotation(angle);
        return 0;
    }

    /// <summary>
    /// 使SpriteEffect在duration帧内旋转
    /// <para>旋转速度为rotateAngle</para>
    /// <para>SpriteEffect</para>
    /// <para>rotateAngle 旋转角速度</para>
    /// <para>duration 旋转周期</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SpriteEffectRotate(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-3) as STGSpriteEffect;
        float rotateAngle = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(3);
        effect.DoRotation(rotateAngle, duration);
        return 0;
    }

    /// <summary>
    /// 执行抖动
    /// </summary>
    /// <param name="luaState"></param>
    /// <para>int delay 抖动延迟</para>
    /// <para>int shakeDuration 抖动持续时间</para>
    /// <para>int shakeInterval 抖动间隔</para>
    /// <para>float shakeDelta 抖动系数</para>
    /// <para>float shakeLevel 抖动等级</para>
    /// <returns></returns>
    public static int ShakeEffectDoShake(ILuaState luaState)
    {
        ShakeEffect effect = luaState.ToUserData(-6) as ShakeEffect;
        int delay = luaState.ToInteger(-5);
        int shakeDuration = luaState.ToInteger(-4);
        int shakeInterval = luaState.ToInteger(-3);
        float shakeDelta = (float)luaState.ToNumber(-2);
        float shakeLevel = (float)luaState.ToNumber(-1);
        luaState.Pop(6);
        effect.DoShake(delay, shakeDuration, shakeInterval, shakeDelta, shakeLevel);
        return 0;
    }

    /// <summary>
    /// 执行抖动
    /// </summary>
    /// <param name="luaState"></param>
    /// <para>int delay 抖动延迟</para>
    /// <para>int shakeDuration 抖动持续时间</para>
    /// <para>int shakeInterval 抖动间隔</para>
    /// <para>float shakeDelta 抖动系数</para>
    /// <para>float shakeLevel 抖动等级</para>
    /// <para>float minShakeLevel 最小抖动等级</para>
    /// <para>float maxRange 抖动的最大范围限制</para>
    /// <returns></returns>
    public static int ShakeEffectDoShakeWithLimitation(ILuaState luaState)
    {
        ShakeEffect effect = luaState.ToUserData(-8) as ShakeEffect;
        int delay = luaState.ToInteger(-7);
        int shakeDuration = luaState.ToInteger(-6);
        int shakeInterval = luaState.ToInteger(-5);
        float shakeDelta = (float)luaState.ToNumber(-4);
        float shakeLevel = (float)luaState.ToNumber(-3);
        float minShakeLevel = (float)luaState.ToNumber(-2);
        float maxRange = (float)luaState.ToNumber(-1);
        luaState.Pop(8);
        effect.DoShake(delay, shakeDuration, shakeInterval, shakeDelta, shakeLevel,minShakeLevel, maxRange);
        return 0;
    }

    public static int CreateChargeEffect(ILuaState luaState)
    {
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        STGChargeEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.ChargeEffect) as STGChargeEffect;
        effect.SetToPosition(posX, posY);
        luaState.PushLightUserData(effect);
        return 1;
    }
}
