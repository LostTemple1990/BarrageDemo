﻿using UnityEngine;
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
        float angle = MathUtil.GetAngleBetweenXAxis(Global.PlayerPos - pos);
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
        IAttachment attachment = luaState.ToUserData(-6) as IAttachment;
        float offsetX = (float)luaState.ToNumber(-5);
        float offsetY = (float)luaState.ToNumber(-4);
        float relativeRotation = (float)luaState.ToNumber(-3);
        bool isFollowMasterRotation = luaState.ToBoolean(-2);
        bool isFollowingContinuously = luaState.ToBoolean(-1);
        luaState.Pop(5);
        attachment.SetRelativePos(offsetX, offsetY, relativeRotation, isFollowMasterRotation, isFollowingContinuously);
        return 0;
    }

    /// <summary>
    /// 做直线运动
    /// <para>movableObject 运动的物体</para>
    /// <para>float v 速度</para>
    /// <para>float angle 速度方向</para>
    /// <para>bool isAimToPlayer 是否朝向玩家</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int STGMovableDoStraightMove(ILuaState luaState)
    {
        ISTGMovable movableObject = luaState.ToUserData(-4) as ISTGMovable;
        float v = (float)luaState.ToNumber(-3);
        float angle = (float)luaState.ToNumber(-2);
        bool isAimToPlayer = luaState.ToBoolean(-1);
        luaState.Pop(4);
        if ( isAimToPlayer )
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            angle += MathUtil.GetAngleBetweenXAxis(playerPos - movableObject.GetPosition());
        }
        movableObject.DoStraightMove(v, angle);
        return 0;
    }

    /// <summary>
    /// 在一段时间内做直线运动
    /// <para>movableObject 运动的物体</para>
    /// <para>float v 速度</para>
    /// <para>float angle 速度方向</para>
    /// <para>bool isAimToPlayer 是否朝向玩家</para>
    /// <para>int duration 持续时间</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int STGMovableDoStraightMoveWithLimitation(ILuaState luaState)
    {
        ISTGMovable movableObject = luaState.ToUserData(-5) as ISTGMovable;
        float v = (float)luaState.ToNumber(-4);
        float angle = (float)luaState.ToNumber(-3);
        bool isAimToPlayer = luaState.ToBoolean(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(5);
        if (isAimToPlayer)
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            angle += MathUtil.GetAngleBetweenXAxis(playerPos - movableObject.GetPosition());
        }
        movableObject.DoStraightMoveWithLimitation(v, angle, duration);
        return 0;
    }

    /// <summary>
    /// 做加速运动
    /// <para>movableObject 运动的物体</para>
    /// <para>float acce 加速度</para>
    /// <para>float angle 速度方向 or bool useVAngle使用速度方向</para>
    /// <para>bool isAimToPlayer 是否朝向玩家</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int STGMovableDoAcceleration(ILuaState luaState)
    {
        ISTGMovable movableObject = luaState.ToUserData(-4) as ISTGMovable;
        float acce = (float)luaState.ToNumber(-3);
        float angle = luaState.Type(-2) == LuaType.LUA_TBOOLEAN ? movableObject.VAngle : (float)luaState.ToNumber(-2);
        bool isAimToPlayer = luaState.ToBoolean(-1);
        luaState.Pop(4);
        if (isAimToPlayer)
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            angle += MathUtil.GetAngleBetweenXAxis(playerPos - movableObject.GetPosition());
        }
        movableObject.DoAcceleration(acce, angle);
        return 0;
    }

    /// <summary>
    /// 做加速运动(限制最大速度)
    /// <para>movableObject 运动的物体</para>
    /// <para>float acce 加速度</para>
    /// <para>float angle 速度方向 or bool useVAngle使用速度方向</para>
    /// <para>bool isAimToPlayer 是否朝向玩家</para>
    /// <para>float maxVelocity 最大速度限制</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int STGMovableDoAccelerationWithLimitation(ILuaState luaState)
    {
        ISTGMovable movableObject = luaState.ToUserData(-5) as ISTGMovable;
        float acce = (float)luaState.ToNumber(-4);
        float angle = luaState.Type(-3) == LuaType.LUA_TBOOLEAN ? movableObject.VAngle : (float)luaState.ToNumber(-3);
        bool isAimToPlayer = luaState.ToBoolean(-2);
        float maxVelocity = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        if (isAimToPlayer)
        {
            Vector2 playerPos = PlayerService.GetInstance().GetCharacter().GetPosition();
            angle += MathUtil.GetAngleBetweenXAxis(playerPos - movableObject.GetPosition());
        }
        movableObject.DoAccelerationWithLimitation(acce, angle, maxVelocity);
        return 0;
    }

    /// <summary>
    /// 物体在duration的时间内移动到目标位置
    /// <para>movableObject 运动的物体</para>
    /// <para>float toX 目标点X坐标</para>
    /// <para>float toY 目标点Y坐标</para>
    /// <para>int duration 时间间隔</para>
    /// <para>mode 插值方式</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int STGMovableDoMoveTo(ILuaState luaState)
    {
        ISTGMovable movableObject = luaState.ToUserData(-5) as ISTGMovable;
        float toX = (float)luaState.ToNumber(-4);
        float toY = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        InterpolationMode mode = (InterpolationMode)luaState.ToInteger(-1);
        luaState.Pop(5);
        movableObject.DoMoveTo(toX, toY, duration, mode);
        return 0;
    }

    /// <summary>
    /// 物体做极坐标运动
    /// <para>movableObject 运动的物体</para>
    /// <para>float radius 起始半径</para>
    /// <para>float angle 起始角度</para>
    /// <para>float deltaR 半径增量</para>
    /// <para>float omega 角速度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int STGMovableDoCurvedMove(ILuaState luaState)
    {
        ISTGMovable movableObject = luaState.ToUserData(-5) as ISTGMovable;
        float radius = (float)luaState.ToNumber(-4);
        float angle = (float)luaState.ToNumber(-3);
        float deltaR = (float)luaState.ToNumber(-2);
        float omega = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        movableObject.DoCurvedMove(radius, angle, deltaR, omega);
        return 0;
    }

    /// <summary>
    /// 给ITaskExecuter添加task
    /// <para>executer task执行者</para>
    /// <para>lua Func task函数</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int AddTask(ILuaState luaState)
    {
        ITaskExecuter executer = luaState.ToUserData(-2) as ITaskExecuter;
        int funcRef = luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        luaState.Pop(1);
        Task task = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        task.funcRef = funcRef;
        executer.AddTask(task);
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
