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

    private bool _isFirst = true;
    private long _lastFrameTime;

	// Update is called once per frame
	void Update ()
    {
        FPSController.GetInstance().SleepToNextFrame();
        //if (_isFirst)
        //{
        //    _isFirst = false;
        //    _lastFrameTime = TimeUtil.GetTimestamp();
        //}
        //else
        //{
        //    long curFrameTime = TimeUtil.GetTimestamp();
        //    Logger.Log("FrameDuration = " + 1000f * (curFrameTime - _lastFrameTime) / 10000000);
        //    _lastFrameTime = curFrameTime;
        //}
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
#if Release
        Logger.Open();
#endif
        CommandManager.GetInstance().Init();
        DataManager.GetInstance().Init();
        ResourceManager.GetInstance().Init();
        SoundManager.GetInstance().Init(GameObject.Find("Sound").transform);
        TimerManager.GetInstance().Init();
        TweenManager.GetInstance().Init();
        UIManager.GetInstance().Init();

        _fsm = new GameStateMachine();
        _fsm.Init();
        _fsm.AddState((int)eGameState.Preload, new StatePreload());
        _fsm.AddState((int)eGameState.STG, new StateSTGMain());
        _fsm.AddState((int)eGameState.Title, new StateTitle());

        _fsm.SetNextStateId((int)eGameState.Preload);

        //Application.targetFrameRate = 60;
    }

#if Release
    public void OnApplicationFocus(bool focus)
    {
        Global.IsApplicationFocus = focus;
    }
#endif
}
