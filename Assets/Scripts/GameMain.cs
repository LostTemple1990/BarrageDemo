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

        //if ( frameNode == 300 )
        //{
        //    NormalEnemy enemy = EnemyManager.GetInstance().CreateEnemy(EnemyType.NormalEnemy) as NormalEnemy;
        //    enemy.Init();
        //    enemy.SetToPosition(new Vector3(0, 200, 0));
        //    enemy.DoMove(0.33f, 315, 1000);
        //}
        //EnemyCurveLaser curveLaser;
        //int i,randomNum;
        //CollisionDetectParas detectParas = new CollisionDetectParas();
        //detectParas.radius = 3;
        //GrazeDetectParas grazeParas = new GrazeDetectParas();
        //grazeParas.halfWidth = 6;
        //grazeParas.halfHeight = 6;
        //if ( frameNode >= 300 && frameNode % 150 == 0 )
        //{
        //    for (i=0;i<20;i++)
        //    {
        //        curveLaser = ObjectsPool.GetInstance().CreateBullet(BulletId.BulletId_Enemy_CurveLaser) as EnemyCurveLaser;
        //        curveLaser.SetBulletTexture("etama9_10");
        //        curveLaser.SetToPosition(0, 180);
        //        randomNum = Random.Range(0, 360);
        //        curveLaser.DoMove(60, i*18, 1, randomNum);
        //        curveLaser.SetCollisionDetectParas(detectParas);
        //        curveLaser.SetGrazeDetectParas(grazeParas);
        //    }
        //}
        InterpreterManager.GetInstance().Update();
        ExtraTaskManager.GetInstance().Update();
        BackgroundManager.GetInstance().Update();
        EffectsManager.GetInstance().Update();
        frameNode++;
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

        UIManager.GetInstance().ShowView(WindowName.GameInfoView, null);

        // 初始化随机数种子
        long seed = System.DateTime.Now.Ticks % 0xffffffff;
        MTRandom.Init(seed);

        Application.targetFrameRate = 60;
    }
}
