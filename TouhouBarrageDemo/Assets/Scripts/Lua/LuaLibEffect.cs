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
        effect.SetPosition(posX, posY);
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
        effect.SetPosition(posX, posY);
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
        STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
        effect.SetSprite(atlasName, spriteName, blendMode, layerId, cached);
        effect.SetOrderInLayer(orderInLayer);
        luaState.PushLightUserData(effect);
        return 1;
    }

    public static int SetSTGObjectProps(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-6) as STGSpriteEffect;
        string atlasName = luaState.ToString(-5);
        string spriteName = luaState.ToString(-4);
        eBlendMode blendMode = (eBlendMode)luaState.ToInteger(-3);
        LayerId layerId = (LayerId)luaState.ToInteger(-2);
        bool cached = luaState.ToBoolean(-1);
        effect.SetSprite(atlasName, spriteName, blendMode, layerId, cached);
        return 0;
    }

    /// <summary>
    /// 创建自定义的STGObject
    /// <para>string customizedName 自定义名称</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// <para>args...</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedSTGObject(ILuaState luaState)
    {
        int top = luaState.GetTop();
        int numArgs = top - 3;
        string customizedName = luaState.ToString(-top);
        float posX = (float)luaState.ToNumber(-top + 1);
        float posY = (float)luaState.ToNumber(-top + 2);
        int funcRef = InterpreterManager.GetInstance().GetCustomizedFuncRef(customizedName, eCustomizedType.STGObject, eCustomizedFuncRefType.Init);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
        effect.SetPosition(posX, posY);
        luaState.PushLightUserData(effect);
        // 复制参数
        int copyIndex = -numArgs - 2;
        for (int i=0;i<numArgs;i++)
        {
            luaState.PushValue(copyIndex);
        }
        luaState.Call(numArgs + 1, 0);
        // 返回结果
        luaState.PushLightUserData(effect);
        return 1;
    }

    /// <summary>
    /// 设置SpriteEffect的尺寸
    /// <para>width</para>
    /// <para>height</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetSpriteEffectSize(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-3) as STGSpriteEffect;
        float width = (float)luaState.ToNumber(-2);
        float height = (float)luaState.ToNumber(-1);
        effect.SetSize(width, height);
        return 0;
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
        int top = luaState.GetTop();
        // 检测带不带alpha
        if (top == 5)
        {
            STGSpriteEffect effect = luaState.ToUserData(-5) as STGSpriteEffect;
            float rValue = (float)luaState.ToNumber(-4);
            float gValue = (float)luaState.ToNumber(-3);
            float bValue = (float)luaState.ToNumber(-2);
            float aValue = (float)luaState.ToNumber(-1);
            effect.SetSpriteColor(rValue, gValue, bValue, aValue);
        }
        else if (top == 4)
        {
            STGSpriteEffect effect = luaState.ToUserData(-4) as STGSpriteEffect;
            float rValue = (float)luaState.ToNumber(-3);
            float gValue = (float)luaState.ToNumber(-2);
            float bValue = (float)luaState.ToNumber(-1);
            effect.SetSpriteColor(rValue, gValue, bValue);
        }
        else if (top == 1)
        {
            STGSpriteEffect effect = luaState.ToUserData(-2) as STGSpriteEffect;
            long color = (long)luaState.ToNumber(-1);
            float bValue = color & 0xff;
            color >>= 8;
            float gValue = color & 0xff;
            color >>= 8;
            float rValue = color & 0xff;
            color >>= 8;
            effect.SetSpriteColor(rValue / 255f, gValue / 255f, bValue / 255f);
        }
        return 0;
    }
    
    /// <summary>
    /// 设置混合模式
    /// <para>effect</para>
    /// <para>blendMode</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetSpriteEffectBlendMode(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-2) as STGSpriteEffect;
        eBlendMode blendMode = (eBlendMode)luaState.ToInteger(-1);
        effect.SetBlendMode(blendMode);
        return 0;
    }

    public static int SpriteEffectChangeWidthTo(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-4) as STGSpriteEffect;
        float toWidth = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode scaleMode = (InterpolationMode)luaState.ToInteger(-1);
        effect.ChangeWidthTo(toWidth, duration, scaleMode);
        return 0;
    }

    public static int SpriteEffectChangeHeightTo(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-4) as STGSpriteEffect;
        float toHeight = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode scaleMode = (InterpolationMode)luaState.ToInteger(-1);
        effect.ChangeHeightTo(toHeight, duration, scaleMode);
        return 0;
    }

    /// <summary>
    /// 执行alpha缓动
    /// <para>effect</para>
    /// <para>[0,1] toAlpha</para>
    /// <para>int duration</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SpriteEffectChangeAlphaTo(ILuaState luaState)
    {
        STGSpriteEffect effect = luaState.ToUserData(-3) as STGSpriteEffect;
        float toAlpha = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        effect.DoTweenAlpha(toAlpha, duration);
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
    /// 抖动屏幕
    /// <para>delay</para>
    /// <para>duration</para>
    /// <para>interval 抖动间隔</para>
    /// <para>shakeLevel 震幅</para>
    /// <para>minShakeLevel 最小震幅</para>
    /// <para>maxRange 最大范围</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ShakeScreen(ILuaState luaState)
    {
        ShakeEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.ShakeEffect) as ShakeEffect;
        string name = luaState.ToString(-7);
        int delay = luaState.ToInteger(-6);
        int shakeDuration = luaState.ToInteger(-5);
        int shakeInterval = luaState.ToInteger(-4);
        float shakeLevel = (float)luaState.ToNumber(-3);
        float minShakeLevel = (float)luaState.ToNumber(-2);
        float maxRange = (float)luaState.ToNumber(-1);
        effect.SetName(name);
        effect.DoShake(delay, shakeDuration, shakeInterval, shakeLevel, minShakeLevel, maxRange);
        return 0;
    }

    /// <summary>
    /// 停止对应名称的抖动特效
    /// <para>name</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int StopShakeScreen(ILuaState luaState)
    {
        string name = luaState.ToString(-1);
        EffectsManager.GetInstance().FinishEffectByName(name);
        return 0;
    }

    /// <summary>
    /// 创建蓄力特效
    /// <para>posX</para>
    /// <para>posY</para>
    /// <para>[optional]scale</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateChargeEffect(ILuaState luaState)
    {
        int top = luaState.GetTop();
        STGChargeEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.ChargeEffect) as STGChargeEffect;
        if (top == 2)
        {
            float posX = (float)luaState.ToNumber(-2);
            float posY = (float)luaState.ToNumber(-1);
            effect.SetPosition(posX, posY);
        }
        else
        {
            float posX = (float)luaState.ToNumber(-3);
            float posY = (float)luaState.ToNumber(-2);
            float scale = (float)luaState.ToNumber(-1);
            effect.SetPosition(posX, posY);
            effect.SetScale(scale);
        }
        luaState.PushLightUserData(effect);
        return 1;
    }

    /// <summary>
    /// 创建爆发特效
    /// <para>posX</para>
    /// <para>posY</para>
    /// <para>[optional]scale</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateBurstEffect(ILuaState luaState)
    {
        int top = luaState.GetTop();
        STGBurstEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.BurstEffect) as STGBurstEffect;
        if (top == 2)
        {
            float posX = (float)luaState.ToNumber(-2);
            float posY = (float)luaState.ToNumber(-1);
            effect.SetPosition(posX, posY);
        }
        else
        {
            float posX = (float)luaState.ToNumber(-3);
            float posY = (float)luaState.ToNumber(-2);
            float scale = (float)luaState.ToNumber(-1);
            effect.SetPosition(posX, posY);
            effect.SetScale(scale);
        }
        luaState.PushLightUserData(effect);
        return 1;
    }

    public static int CreateShakeEffect(ILuaState luaState)
    {
        return 0;
    }

    /// <summary>
    /// 创建boss死亡特效（暂用
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateBossDeadEffect(ILuaState luaState)
    {
        STGPlayerDeadEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.PlayerDeadEffect) as STGPlayerDeadEffect;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        effect.SetPosition(posX, posY);
        luaState.PushLightUserData(effect);
        return 1;
    }
}
