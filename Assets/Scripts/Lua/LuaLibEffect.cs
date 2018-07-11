﻿using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    public static int CreateEffectByType(ILuaState luaState)
    {
        EffectType type = (EffectType)luaState.ToInteger(-1);
        IEffect effect = EffectsManager.GetInstance().CreateEffectByType(type);
        luaState.Pop(1);
        luaState.PushLightUserData(effect);
        return 1;
    }

    public static int SetEffectToPos(ILuaState luaState)
    {
        IEffect effect = luaState.ToUserData(-3) as IEffect;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        effect.SetToPos(posX, posY);
        return 0;
    }

    public static int SetEffectFinish(ILuaState luaState)
    {
        IEffect effect = luaState.ToUserData(-1) as IEffect;
        luaState.Pop(1);
        effect.FinishEffect();
        return 0;
    }

    /// <summary>
    /// 创建一个精灵特效
    /// <para>string effectSpName</para>
    /// <para>float width</para>
    /// <para>float height</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSpriteEffect(ILuaState luaState)
    {
        string effectSpName = luaState.ToString(-5);
        float width = (float)luaState.ToNumber(-4);
        float height = (float)luaState.ToNumber(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        SpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as SpriteEffect;
        effect.SetSprite(effectSpName);
        effect.SetSize(width, height);
        effect.SetToPos(posX, posY);
        luaState.PushLightUserData(effect);
        return 1;
    }

    public static int SetSpriteEffectColor(ILuaState luaState)
    {
        SpriteEffect effect = luaState.ToUserData(-5) as SpriteEffect;
        float rValue = (float)luaState.ToNumber(-4);
        float gValue = (float)luaState.ToNumber(-3);
        float bValue = (float)luaState.ToNumber(-2);
        float aValue = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        effect.SetSpriteColor(rValue, gValue, bValue, aValue);
        return 0;
    }

    public static int SpriteEffectScaleWidth(ILuaState luaState)
    {
        SpriteEffect effect = luaState.ToUserData(-4) as SpriteEffect;
        float toScale = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode scaleMode = (InterpolationMode)luaState.ToInteger(-1);
        luaState.Pop(4);
        effect.DoScaleWidth(toScale, duration, scaleMode);
        return 0;
    }

    public static int SpriteEffectScaleHeight(ILuaState luaState)
    {
        SpriteEffect effect = luaState.ToUserData(-4) as SpriteEffect;
        float toScale = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode scaleMode = (InterpolationMode)luaState.ToInteger(-1);
        luaState.Pop(4);
        effect.DoScaleHeight(toScale, duration, scaleMode);
        return 0;
    }
}
