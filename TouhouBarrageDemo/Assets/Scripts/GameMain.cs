using UnityEngine;

public class GameMain : MonoBehaviour
{
    private GameStateMachine _fsm;

	// Use this for initialization
	void Start ()
    {
        GameObject.DontDestroyOnLoad(GameObject.Find("Main Camera"));
        GameObject.DontDestroyOnLoad(GameObject.Find("EventSystem"));
        GameObject.DontDestroyOnLoad(GameObject.Find("Sound"));
        GameObject.DontDestroyOnLoad(GameObject.Find("UIRoot"));
        GameObject.DontDestroyOnLoad(GameObject.Find("GameMainCanvas"));
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
        SoundManager.GetInstance().Init(GameObject.Find("Sound").transform);
        TimerManager.GetInstance().Init();
        TweenManager.GetInstance().Init();
        UIManager.GetInstance().Init();
        _fsm = new GameStateMachine();
        _fsm.Init();
        _fsm.AddState((int)eGameState.STG, new StateSTGMain());
        _fsm.AddState((int)eGameState.Title, new StateTitle());
        STGData data = new STGData()
        {
            stageName = "Stage1",
            characterIndex = 1,
            isReplay = false,
        };
        _fsm.SetNextStateId((int)eGameState.STG, data);
        ReplayManager.GetInstance().Init();
        //_fsm.SetNextStateId((int)eGameState.Title);
        Application.targetFrameRate = 60;
    }
}
