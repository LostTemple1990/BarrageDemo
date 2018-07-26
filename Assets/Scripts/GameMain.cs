using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{

    OperationController _opController;
    CharacterBase _char;
	// Use this for initialization
	void Start () {
        Init();
	}
	
	// Update is called once per frame
	void Update ()
    {
        _opController.Update();
        _char.Update();
        EnemyManager.GetInstance().Update();
        BulletsManager.GetInstance().Update();
        ItemManager.GetInstance().Update();
        AnimationManager.GetInstance().Update();
        SoundManager.GetInstance().Update();
        InterpreterManager.GetInstance().Update();
        ExtraTaskManager.GetInstance().Update();
        BackgroundManager.GetInstance().Update();
        EffectsManager.GetInstance().Update();
        TimerManager.GetInstance().Update();
        frameNode++;
        // 背景部分暂时写这，之后转移到lua
        if ( frameNode % 30 == 0 )
        {
            BgSpriteObject spObj = BackgroundManager.GetInstance().CreateBgSpriteObject("MapleLeaf1");
            float posX = Random.Range(80, 150);
            float posY = Random.Range(200, 225);
            spObj.SetToPos(posX, posY);
            float scale = Random.Range(0.2f, 1);
            spObj.SetScale(new Vector3(scale, scale));
            spObj.SetVelocity(Random.Range(1f, 3f), Random.Range(-150, -30));
            spObj.SetSelfRotateAngle(new Vector3(0, 0, Random.Range(1f, 2f)));
            spObj.DoFade(Random.Range(90, 180), Random.Range(180,300));
            BackgroundManager.GetInstance().AddBgSpriteObject(spObj);
        }
    }

    private int frameNode = 0;

    private void Init()
    {
        CommandManager.GetInstance().Init();
        DataManager.GetInstance().Init();
        ResourceManager.GetInstance().Init();
        AnimationManager.GetInstance().Init();
        UIManager.GetInstance().Init();
        PlayerService.GetInstance().Init();
        _char = PlayerService.GetInstance().GetCharacter();
        _opController = new OperationController();
        _opController.InitCharacter(_char);
        BulletsManager.GetInstance().Init();
        EnemyManager.GetInstance().Init();
        ItemManager.GetInstance().Init();
        SoundManager.GetInstance().Init(gameObject.transform);

        InterpreterManager.GetInstance().Init();
        InterpreterManager.GetInstance().DoStageLua(1);

        ExtraTaskManager.GetInstance().Init();

        BackgroundManager.GetInstance().Init();
        EffectsManager.GetInstance().Init();
        TimerManager.GetInstance().Init();

        UIManager.GetInstance().ShowView(WindowName.GameInfoView, null);
        
        // 测试抖动效果
        //ShakeEffect shakeEffect = EffectsManager.GetInstance().CreateEffectByType(EffectType.ShakeEffect) as ShakeEffect;
        //shakeEffect.DoShake(200, 9999, 12, 1, 5);

        // 初始化随机数种子
        long seed = System.DateTime.Now.Ticks % 0xffffffff;
        MTRandom.Init(seed);

        Application.targetFrameRate = 60;
    }
}
