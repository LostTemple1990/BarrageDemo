﻿public class CommandConsts 
{
    public const int NewSpellCardTime = 1000;
    public const int UpdateSpellCardTime = 1001;

    public const int CreateBgDistortEffect = 2000;
    public const int UpdateBgDistortEffectProps = 2001;
    public const int RemoveBgDistortEffect = 2002;
    /// <summary>
    /// 播放人物立绘动画
    /// </summary>
    public const int PlayCharacterCGAni = 2010;
    /// <summary>
    /// 显示符卡信息
    /// </summary>
    public const int ShowSpellCardInfo = 2011;
    /// <summary>
    /// 符卡结束
    /// </summary>
    public const int SpellCardFinish = 2012;

    /// <summary>
    /// 进入关卡
    /// </summary>
    public const int EnterStage = 3000;
    /// <summary>
    /// STG部分初始化完成
    /// </summary>
    public const int STGInitComplete = 3001;
    /// <summary>
    /// 解析stage.lua完成
    /// </summary>
    public const int STGLoadStageLuaComplete = 3002;
    /// <summary>
    /// STG每帧开始
    /// </summary>
    public const int STGFrameStart = 3003;
    /// <summary>
    /// 打印当前帧的一些统计数据
    /// </summary>
    public const int LogFrameStatistics = 3004;

    public const int RetryStage = 3010;
    public const int RetryGame = 3011;
}
