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

    public const int RetryStage = 3010;
    public const int RetryGame = 3011;
}
