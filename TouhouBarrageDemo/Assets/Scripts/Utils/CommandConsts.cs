public class CommandConsts 
{
    public const int NewSpellCardTime = 1000;
    public const int UpdateSpellCardTime = 1001;
    /// <summary>
    /// 显示Boss信息
    /// <para>name Boss名称</para>
    /// <para>scLeft 剩余符卡数量</para>
    /// </summary>
    public const int ShowBossInfo = 1002;
    /// <summary>
    /// 显示Boss位置提示信息
    /// <para>boss boss本体</para>
    /// <para>bool isShow 是否显示</para>
    /// <para>[optional] posX boss的x坐标</para>
    /// </summary>
    public const int ShowBossPosHint = 1003;

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
    /// 创建对话
    /// </summary>
    public const int StartDialog = 2021;
    /// <summary>
    /// 创建剧情对话CG
    /// <para>string name 自定义名称</para>
    /// <para>string spName sprite的资源名称</para>
    /// <para>float posX float posY CG位置</para>
    /// </summary>
    public const int CreateDialogCG = 2022;
    /// <summary>
    /// 高亮剧情对话CG
    /// <para>string name </para>
    /// <para>bool highlight 高亮/取消高亮</para>
    /// </summary>
    public const int HighlightDialogCG = 2023;
    /// <summary>
    /// 淡出剧情对话CG
    /// <para>string name</para>
    /// </summary>
    public const int FadeOutDialogCG = 2024;
    /// <summary>
    /// 创建对话框
    /// <para>int style 对话框样式</para>
    /// <para>string text 文本</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// <para>duration 持续时间</para>
    /// <para>float scale</para>
    /// </summary>
    public const int CreateDialogBox = 2025;
    /// <summary>
    /// 删除对话框
    /// </summary>
    public const int DelDialogBox = 2026;
    /// <summary>
    /// 更新对话进程
    /// <para>dTime</para>
    /// </summary>
    public const int UpdateDialog = 2027;
    /// <summary>
    /// 结束对话进程
    /// </summary>
    public const int FinishDialog = 2027;


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
    /// 玩家获取额外残机
    /// </summary>
    public const int PlayerExtend = 2111;
    /// <summary>
    /// 玩家吃到满P
    /// </summary>
    public const int PlayerGetFullPower = 2112;

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
    /// <summary>
    /// 加载关卡的初始背景完成
    /// </summary>
    public const int STGLoadStageDefaultBgComplete = 3005;

    public const int RetryStage = 3010;
    public const int RetryGame = 3011;
    /// <summary>
    /// 继续游戏
    /// </summary>
    public const int ContinueGame = 3012;
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public const int PauseGame = 3013;
    /// <summary>
    /// 疮痍之后继续游戏
    /// </summary>
    public const int ContinueGameAfterGameOver = 3104;
    /// <summary>
    /// 关卡完成
    /// </summary>
    public const int StageClear = 3015;
    /// <summary>
    /// 保存录像
    /// </summary>
    public const int SaveReplay = 3016;
    /// <summary>
    /// 返回主菜单
    /// </summary>
    public const int BackToTitle = 3017;

    /// <summary>
    /// 选择难度
    /// </summary>
    public const int SelectDifficulty = 3101;
    /// <summary>
    /// 选择角色
    /// <para>int characterIndex</para>
    /// <para>0 灵梦 1 魔理沙</para>
    /// </summary>
    public const int SelectCharacter = 3102;
    /// <summary>
    /// 取消角色选择，返回主界面
    /// </summary>
    public const int CancelSelectCharacter = 3103;
    /// <summary>
    /// 取消录像选择，返回主界面
    /// </summary>
    public const int CancelSelectReplay = 3104;
    /// <summary>
    /// 保存录像成功
    /// </summary>
    public const int SaveReplaySuccess = 3105;
    /// <summary>
    /// 载入录像成功
    /// </summary>
    public const int LoadReplaySuccess = 3106;
    /// <summary>
    /// 进入标题画面
    /// </summary>
    public const int OnEnterTitle = 3107;

    /// <summary>
    /// 预加载完成
    /// </summary>
    public const int PreloadComplete = 3201;
    /// <summary>
    /// 游戏本体焦点事件
    /// <para>bool focus 丢失/获取焦点</para>
    /// </summary>
    public const int OnApplicationFocus = 3202;
}
