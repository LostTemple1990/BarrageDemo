//define 定义
// LogLuaFuncRef  是否log出记录在c#这边的lua变量的索引

using UnityEngine;

public class GameMain : MonoBehaviour
{
    OperationController _opController;
    CharacterBase _char;

    private int _state;

    private GameStateMachine _fsm;

	// Use this for initialization
	void Start ()
    {
        Init();
	}
	
	// Update is called once per frame
	void Update ()
    {
        _fsm.Update();
        TimerManager.GetInstance().Update();
        TweenManager.GetInstance().Update();
        UIManager.GetInstance().Update();
        SoundManager.GetInstance().Update();
        // 销毁检测
        if ( Global.SysBusyValue < Consts.SysBusyValue)
        {
            ObjectsPool.GetInstance().CheckDestroyPoolObjects();
            if ( Global.SysBusyValue == 0 )
            {
                ObjectsPool.GetInstance().DestroyProtoTypes();
            }
        }
    }

    private void Init()
    {
        CommandManager.GetInstance().Init();
        DataManager.GetInstance().Init();
        ResourceManager.GetInstance().Init();
        SoundManager.GetInstance().Init(transform);
        TimerManager.GetInstance().Init();
        TweenManager.GetInstance().Init();
        UIManager.GetInstance().Init();
        _fsm = new GameStateMachine();
        _fsm.Init();
        _fsm.AddState((int)eGameState.STG, new StateSTGMain());
        _fsm.AddState((int)eGameState.Title, new StateTitle());
        //STGData data = new STGData()
        //{
        //    stageName = "Stage1",
        //    characterIndex = 1,
        //};
        //_fsm.SetNextStateId((int)eGameState.STG, data);
        _fsm.SetNextStateId((int)eGameState.Title);
        Application.targetFrameRate = 60;
    }
}
