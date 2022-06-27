//#define CheckSTGFrameTime

using UnityEngine;
using System.Collections;
#if CheckSTGFrameTime
using System.Diagnostics;
#endif

public class STGMain
{
    OperationController _opController;
    CharacterBase _char;

    private long _lastFrameTicks;

    public void Update()
    {
        CommandManager.GetInstance().RunCommand(CommandConsts.STGFrameStart);

#if CheckSTGFrameTime
        int index = 0;
        long[] timeArr = new long[20];
        long frameBeginTime = Stopwatch.GetTimestamp();
        _opController.Update();
        STGStageManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        ColliderManager.GetInstance().UpdateFields();
        timeArr[index++] = Stopwatch.GetTimestamp();
        ExtraTaskManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        _char.Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        EnemyManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        BulletsManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        ItemManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        ColliderManager.GetInstance().UpdateColliders();
        timeArr[index++] = Stopwatch.GetTimestamp();
        EffectsManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        BulletsManager.GetInstance().Render();
        timeArr[index++] = Stopwatch.GetTimestamp();
        AnimationManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        BackgroundManager.GetInstance().Update();
        timeArr[index++] = Stopwatch.GetTimestamp();
        frameNode++;
        long frameEndTime = Stopwatch.GetTimestamp();
        if ( frameEndTime - frameBeginTime >= 50000 )
        {
            string logStr = "Frame " + STGStageManager.GetInstance().GetFrameSinceStageStart() + " cost time " + (frameEndTime - frameBeginTime) * 0.0001d + "ms\n";
            index = 0;
            logStr += "STGStageManager Update Cost Time = " + (timeArr[index]-frameBeginTime) * 0.0001d + "ms\n";
            logStr += "GravitationFields Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "ExtraTaskManager Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "Character Update Cost Time = " + (timeArr[++index]-timeArr[index-1]) * 0.0001d + "ms\n";
            logStr += "EnemyManager Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "BulletsManager Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "ItemManager Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "Colliders Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "EffectsManager Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "BulletsManager Render Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "AnimationManager Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            logStr += "BackgroundManager Update Cost Time = " + (timeArr[++index] - timeArr[index - 1]) * 0.0001d + "ms\n";
            Logger.LogWarn(logStr);
            CommandManager.GetInstance().RunCommand(CommandConsts.LogFrameStatistics);
            Logger.Log("------------------------------------------------");
        }
#else
        //if (frameNode == 200)
        //{
        //    _lastFrameTicks = System.DateTime.Now.Ticks;
        //}
        //else if (frameNode > 200 && frameNode <= 300)
        //{
        //    long currentTicks = System.DateTime.Now.Ticks;
        //    Logger.Log("Frame Interval = " + (currentTicks - _lastFrameTicks) * 0.0001f);
        //    _lastFrameTicks = currentTicks;
        //}
        frameNode++;
        // 每帧开始
        BulletsManager.GetInstance().OnSTGFrameStart();
        // 逻辑部分
        _opController.Update();
        STGStageManager.GetInstance().Update();
        ExtraTaskManager.GetInstance().Update();
        ColliderManager.GetInstance().UpdateFields();
        _char.Update();
        EnemyManager.GetInstance().Update();
        BulletsManager.GetInstance().Update();
        ItemManager.GetInstance().Update();
        ColliderManager.GetInstance().UpdateColliders();
        EffectsManager.GetInstance().Update();
        // 渲染部分
        BulletsManager.GetInstance().Render();
        EnemyManager.GetInstance().Render();
        STGEliminateEffectManager.GetInstance().Render();
        ItemManager.GetInstance().Render();
        AnimationManager.GetInstance().Update();
        BackgroundManager.GetInstance().Update();
#if ShowCollisionViewer
        CollisionViewer.Instance.Render();
#endif
        //if (frameNode == 0)
        //    FPSController.GetInstance().Restart(true);
        //if (frameNode % 60 == 0)
        //{
        //   STGBulletEliminateEffect1 effect =  EffectsManager.GetInstance().CreateEffectByType(EffectType.BulletEliminate) as STGBulletEliminateEffect1;
        //    effect.SetColor(1, 0, 0, 1);
        //}
#endif
        //if ( frameNode == 200 )
        //{
        //    List<object> datas = new List<object>();
        //    float centerX = -130;
        //    float centerY = 180;
        //    float radius = 70;
        //    float distortFactor = 0.1f;
        //    Color effectColor = new Color(0.62f, 0.22f, 0.61f, 1f);
        //    datas.Add(centerX);
        //    datas.Add(centerY);
        //    datas.Add(radius);
        //    datas.Add(distortFactor);
        //    datas.Add(effectColor);
        //    CommandManager.GetInstance().RunCommand(CommandConsts.UpdateBgDistortEffectProps, datas.ToArray());
        //}
        //if ( frameNode == 200 )
        //{
        //    STGBurstEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.BurstEffect) as STGBurstEffect;
        //    effect.SetSize(128);
        //    effect.SetToPos(0, 150);
        //}
        //if (frameNode == 200)
        //{
        //    TimeUtil.BeginSample();
        //    STGChargeEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.ChargeEffect) as STGChargeEffect;
        //    effect.SetToPos(0, 100);
        //    TimeUtil.EndSample();
        //}
        //if (frameNode >= 200 && frameNode % 2 == 0 )
        //{
        //    STGBulletEliminateEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.BulletEliminate) as STGBulletEliminateEffect;
        //    effect.SetToPos(Random.Range(-8, 8), Random.Range(-8, 8));
        //    effect.SetColor(Random.value, Random.value, Random.value, 1);
        //}
    }

    private int frameNode = 0;

    public void Init()
    {
        // 游戏本体边界
        Global.GameLBBorderPos = new Vector2(-Consts.GameWidth / 2, -Consts.GameHeight / 2);
        Global.GameRTBorderPos = new Vector2(Consts.GameWidth / 2, Consts.GameHeight / 2);
        // 弹幕边界
        Global.BulletLBBorderPos = new Vector2(Global.GameLBBorderPos.x - 100, Global.GameLBBorderPos.y - 100);
        Global.BulletRTBorderPos = new Vector2(Global.GameRTBorderPos.x + 100, Global.GameRTBorderPos.y + 100);
        // 玩家坐标边界
        Global.PlayerLBBorderPos = new Vector2(Global.GameLBBorderPos.x + 10, Global.GameLBBorderPos.y + 10);
        Global.PlayerRTBorderPos = new Vector2(Global.GameRTBorderPos.x - 10, Global.GameRTBorderPos.y - 10);

        InterpreterManager.GetInstance().Init();
        STGStageManager.GetInstance().Init();
        AnimationManager.GetInstance().Init();
        BulletsManager.GetInstance().Init();
        EnemyManager.GetInstance().Init();
        ItemManager.GetInstance().Init();
        ColliderManager.GetInstance().Init();

        ExtraTaskManager.GetInstance().Init();

        EffectsManager.GetInstance().Init();
        STGEliminateEffectManager.GetInstance().Init();
        BackgroundManager.GetInstance().Init();
#if ShowCollisionViewer
        CollisionViewer.Instance.Init();
#endif
    }

    public void Clear()
    {
        PlayerInterface.GetInstance().Clear();
        _char = null;
        OperationController.GetInstance().Clear();
        ColliderManager.GetInstance().Clear();
        BulletsManager.GetInstance().Clear();
        EnemyManager.GetInstance().Clear();
        ItemManager.GetInstance().Clear();
        EffectsManager.GetInstance().Clear();
        ExtraTaskManager.GetInstance().Clear();
        BackgroundManager.GetInstance().Clear();
        STGStageManager.GetInstance().Clear();
        InterpreterManager.GetInstance().Clear();
        SoundManager.GetInstance().Clear(true);
#if ShowCollisionViewer
        CollisionViewer.Instance.Clear();
#endif
        STGEliminateEffectManager.GetInstance().Clear();
    }

    /// <summary>
    /// 初始化STG的数据、玩家对象、控制器等
    /// </summary>
    public void InitSTG(int characterIndex)
    {
        _char = PlayerInterface.GetInstance().CreateCharacter(characterIndex);
        _opController = OperationController.GetInstance();
        _opController.InitCharacter();
        InterpreterManager.GetInstance().SetGlobalField("player", _char, LuaParaType.LightUserData);

        // 设置初始残机数和符卡数目
        PlayerInterface.GetInstance().SetLifeCounter(Consts.STGInitLifeCount, 0);
        PlayerInterface.GetInstance().SetSpellCardCounter(Consts.STGInitSpellCardCount, 0);

        CommandManager.GetInstance().RunCommand(CommandConsts.STGInitComplete);
        frameNode = 0;
    }
}
