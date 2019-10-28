using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 开启剧情模式
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int StartDialog(ILuaState luaState)
    {
        int funcRef = luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        STGStageManager.GetInstance().StartDialog(funcRef);
        return luaState.Yield(0);
    }

    /// <summary>
    /// 创建剧情人物CG
    /// <para>string name 索引名称</para>
    /// <para>string spName 立绘文件名称</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateDialogCG(ILuaState luaState)
    {
        string name = luaState.ToString(-4);
        string spName = luaState.ToString(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        STGStageManager.GetInstance().CreateDialogCG(name, spName, posX, posY);
        return 0;
    }

    /// <summary>
    /// 高亮角色立绘CG
    /// <para>string name 索引名称</para>
    /// <para>bool highlight 高亮/取消高亮</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int HighlightDialogCG(ILuaState luaState)
    {
        string name = luaState.ToString(-2);
        bool highlight = luaState.ToBoolean(-1);
        STGStageManager.GetInstance().HighlightDialogCG(name, highlight);
        return 0;
    }

    /// <summary>
    /// 角色立绘淡出
    /// <para>string name 索引名称</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int FadeOutDialogCG(ILuaState luaState)
    {
        string name = luaState.ToString(-1);
        STGStageManager.GetInstance().DelDialogCG(name);
        return 0;
    }

    /// <summary>
    /// 创建剧情对话框
    /// <para>int style 对话框类型</para>
    /// <para>string text 对话框文本</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// <para>int duration 对话框持续时间</para>
    /// <para>float scale 对话框高度缩放</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateDialogBox(ILuaState luaState)
    {
        int style = luaState.ToInteger(-6);
        string text = luaState.ToString(-5);
        float posX = (float)luaState.ToNumber(-4);
        float posY = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        float scale = (float)luaState.ToNumber(-1);
        STGStageManager.GetInstance().CreateDialogBox(style, text, posX, posY, duration, scale);
        return 0;
    }

    public static int DelDialogBox(ILuaState luaState)
    {
        
        return 0;
    }
}
