public class CommandConsts 
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
    /// <para>消息附带的参数</para>
    /// <para>bool isSpellCard 是否为符卡(非符or符卡)</para>
    /// <para>bool isFinishSpellCard 是否成功击破符卡</para>
    /// <para>bool getBonus 是否获得符卡bonus<para>
    /// <para>int timePassed 击破符卡所用的游戏时间</para>
    /// <para>int realTimePassed 击破符卡实际经过的时间</para>
    /// </summary>
    public const int SpellCardFinish = 2012;

    /// <summary>
    /// 玩家进入决死状态
    /// </summary>
    public const int PlayerDying = 2100;
    /// <summary>
    /// 玩家miss
    /// </summary>
    public const int PlayerMiss = 2101;
    /// <summary>
    /// 玩家放B
    /// </summary>
    public const int PlayerCastSC = 2102;

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
    /// <summary>
    /// 继续游戏
    /// </summary>
    public const int ContinueGame = 3012;
}
