//#define CheckSTGFrameTime

using UnityEngine;
using System.Collections;

public class STGMain
{
    OperationController _opController;
    CharacterBase _char;

    public void Update()
    {
        if (CheckPause())
        {
            return;
        }
        CommandManager.GetInstance().RunCommand(CommandConsts.STGFrameStart);

#if CheckSTGFrameTime
        long[] timeArr = new long[10];
        long frameBeginTime = System.DateTime.Now.Ticks;
        _opController.Update();
        _char.Update();
        timeArr[0] = System.DateTime.Now.Ticks - frameBeginTime;
        ColliderManager.GetInstance().Update();
        timeArr[1] = System.DateTime.Now.Ticks - timeArr[0];
        EnemyManager.GetInstance().Update();
        timeArr[2] = System.DateTime.Now.Ticks - timeArr[1];
        BulletsManager.GetInstance().Update();
        timeArr[3] = System.DateTime.Now.Ticks - timeArr[2];
        ItemManager.GetInstance().Update();
        timeArr[4] = System.DateTime.Now.Ticks - timeArr[3];
        AnimationManager.GetInstance().Update();
        timeArr[5] = System.DateTime.Now.Ticks - timeArr[4];
        ExtraTaskManager.GetInstance().Update();
        timeArr[6] = System.DateTime.Now.Ticks - timeArr[5];
        STGStageManager.GetInstance().Update();
        timeArr[7] = System.DateTime.Now.Ticks - timeArr[6];
        BackgroundManager.GetInstance().Update();
        timeArr[8] = System.DateTime.Now.Ticks - timeArr[7];
        EffectsManager.GetInstance().Update();
        timeArr[9] = System.DateTime.Now.Ticks - timeArr[8];
        frameNode++;
        // 背景部分暂时写这，之后转移到lua
        if (frameNode % 30 == 0)
        {
            BgSpriteObject spObj = BackgroundManager.GetInstance().CreateBgSpriteObject("MapleLeaf1");
            float posX = Random.Range(80, 150);
            float posY = Random.Range(200, 225);
            spObj.SetToPos(posX, posY);
            float scale = Random.Range(0.2f, 1);
            spObj.SetScale(new Vector3(scale, scale));
            spObj.SetVelocity(Random.Range(1f, 3f), Random.Range(-150, -30));
            spObj.SetSelfRotateAngle(new Vector3(0, 0, Random.Range(1f, 2f)));
            spObj.DoFade(Random.Range(90, 180), Random.Range(180, 300));
            BackgroundManager.GetInstance().AddBgSpriteObject(spObj);
        }
        long frameEndTime = System.DateTime.Now.Ticks;
        if ( frameEndTime - frameBeginTime >= 50000 )
        {
            string logStr = "Frame " + STGStageManager.GetInstance().GetFrameSinceStageStart() + " cost time " + (frameEndTime - frameBeginTime) / 10000f + "ms\n";
            logStr += "Character Update Cost Time = " + timeArr[0] / 10000f + "ms\n";
            logStr += "ColliderManager Update Cost Time = " + timeArr[1] / 10000f + "ms\n";
            logStr += "EnemyManager Update Cost Time = " + timeArr[2] / 10000f + "ms\n";
            logStr += "BulletsManager Update Cost Time = " + timeArr[3] / 10000f + "ms\n";
            logStr += "ItemManager Update Cost Time = " + timeArr[4] / 10000f + "ms\n";
            logStr += "AnimationManager Update Cost Time = " + timeArr[5] / 10000f + "ms\n";
            logStr += "ExtraTaskManager Update Cost Time = " + timeArr[6] / 10000f + "ms\n";
            logStr += "STGStageManager Update Cost Time = " + timeArr[7] / 10000f + "ms\n";
            logStr += "BackgroundManager Update Cost Time = " + timeArr[8] / 10000f + "ms\n";
            logStr += "EffectsManager Update Cost Time = " + timeArr[9] / 10000f + "ms\n";
            Logger.LogWarn(logStr);
            CommandManager.GetInstance().RunCommand(CommandConsts.LogFrameStatistics);
            Logger.Log("------------------------------------------------");
        }
#else
        _opController.Update();
        _char.Update();
        ColliderManager.GetInstance().Update();
        EnemyManager.GetInstance().Update();
        BulletsManager.GetInstance().Update();
        ItemManager.GetInstance().Update();
        AnimationManager.GetInstance().Update();
        ExtraTaskManager.GetInstance().Update();
        STGStageManager.GetInstance().Update();
        BackgroundManager.GetInstance().Update();
        EffectsManager.GetInstance().Update();
        frameNode++;
        // 背景部分暂时写这，之后转移到lua
        if (frameNode % 30 == 0)
        {
            BgSpriteObject spObj = BackgroundManager.GetInstance().CreateBgSpriteObject("MapleLeaf1");
            float posX = Random.Range(80, 150);
            float posY = Random.Range(200, 225);
            spObj.SetToPos(posX, posY);
            float scale = Random.Range(0.2f, 1);
            spObj.SetScale(new Vector3(scale, scale));
            spObj.SetVelocity(Random.Range(1f, 3f), Random.Range(-150, -30));
            spObj.SetSelfRotateAngle(new Vector3(0, 0, Random.Range(1f, 2f)));
            spObj.DoFade(Random.Range(90, 180), Random.Range(180, 300));
            BackgroundManager.GetInstance().AddBgSpriteObject(spObj);
        }
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
        AnimationManager.GetInstance().Init();
        ColliderManager.GetInstance().Init();
        BulletsManager.GetInstance().Init();
        EnemyManager.GetInstance().Init();
        ItemManager.GetInstance().Init();

        InterpreterManager.GetInstance().Init();
        STGStageManager.GetInstance().Init();

        ExtraTaskManager.GetInstance().Init();

        EffectsManager.GetInstance().Init();

        // 测试抖动效果
        //ShakeEffect shakeEffect = EffectsManager.GetInstance().CreateEffectByType(EffectType.ShakeEffect) as ShakeEffect;
        //shakeEffect.DoShake(200, 9999, 6, 1, 5, 3, 15);
    }

    /// <summary>
    /// 检测游戏是否暂停
    /// </summary>
    /// <returns></returns>
    private bool CheckPause()
    {
        if (!Global.IsPause && Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.GetInstance().ShowView(WindowName.STGPauseView);
        }
        return Global.IsPause;
    }

    public void Clear(eSTGClearType type)
    {
        PlayerService.GetInstance().Clear();
        _char = null;
        _opController.Clear();
        ColliderManager.GetInstance().Clear();
        BulletsManager.GetInstance().Clear();
        EnemyManager.GetInstance().Clear();
        ItemManager.GetInstance().Clear();
        EffectsManager.GetInstance().Clear();
        ExtraTaskManager.GetInstance().Clear();
        BackgroundManager.GetInstance().Clear();
        STGStageManager.GetInstance().Clear();
        InterpreterManager.GetInstance().Clear(type);
    }

    /// <summary>
    /// 初始化STG的数据、玩家对象、控制器等
    /// </summary>
    public void InitSTG()
    {
        // 初始化随机数种子
        long seed = System.DateTime.Now.Ticks % 0xffffffff;
        MTRandom.Init(seed);
        PlayerService.GetInstance().Init();
        _char = PlayerService.GetInstance().GetCharacter();
        _opController = new OperationController();
        _opController.InitCharacter(_char);
        BackgroundManager.GetInstance().Init();

        CommandManager.GetInstance().RunCommand(CommandConsts.STGInitComplete);
    }

    public void EnterStage(int stageId)
    {
        STGStageManager.GetInstance().LoadStage(stageId);
        CommandManager.GetInstance().RunCommand(CommandConsts.STGLoadStageLuaComplete);
    }
}
