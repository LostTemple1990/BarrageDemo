using UnityEngine;
using System.Collections;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 获取IPosition对象相对于自机的角度
    /// <para>IPosition</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetAngleToPlayer(ILuaState luaState)
    {
        IPosition positionCls = luaState.ToUserData(-1) as IPosition;
        luaState.Pop(1);
        Vector2 pos = positionCls.GetPosition();
        float angle = MathUtil.GetAngleBetweenXAxis(Global.PlayerPos - pos, false);
        luaState.PushNumber(angle);
        return 1;
    }
    /// <summary>
    /// 绑定一个附件到Master上
    /// <para>attachment</para>
    /// <para>master</para>
    /// <para>eliminatedWithMaster</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int AttachToMaster(ILuaState luaState)
    {
        IAttachment attachment = luaState.ToUserData(-3) as IAttachment;
        IAttachable master = luaState.ToUserData(-2) as IAttachable;
        bool eliminatedWithMaster = luaState.ToBoolean(-1);
        luaState.Pop(3);
        attachment.AttachTo(master, eliminatedWithMaster);
        return 0;
    }

    /// <summary>
    /// 设置附件相对于Master的相对位置以及旋转角度
    /// <para>attachment</para>
    /// <para>offsetX</para>
    /// <para>offsetY</para>
    /// <para>relativeRotation 相对旋转角度</para>
    /// <para>isFollowMasterRotation 是否跟随master一起旋转</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetAttachmentRelativePos(ILuaState luaState)
    {
        IAttachment attachment = luaState.ToUserData(-5) as IAttachment;
        float offsetX = (float)luaState.ToNumber(-4);
        float offsetY = (float)luaState.ToNumber(-3);
        float relativeRotation = (float)luaState.ToNumber(-2);
        bool isFollowMasterRotation = luaState.ToBoolean(-1);
        luaState.Pop(5);
        attachment.SetRelativePos(offsetX, offsetY, relativeRotation, isFollowMasterRotation);
        return 0;
    }

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
