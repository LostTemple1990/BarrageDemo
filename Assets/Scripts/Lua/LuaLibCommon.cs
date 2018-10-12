using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    public static int PlayCharacterCG(ILuaState luaState)
    {
        string path = luaState.ToString(-2);
        object[] datas = { path,luaState};
        CommandManager.GetInstance().RunCommand(CommandConsts.PlayCharacterCGAni, datas);
        luaState.Pop(2);
        return 0;
    }

    public static int LogFrameSinceStageStart(ILuaState luaState)
    {
        int frame = STGStageManager.GetInstance().GetFrameSinceStageStart();
        Logger.Log("Frame Since Stage Start = " + frame);
        return 0;
    }
}
