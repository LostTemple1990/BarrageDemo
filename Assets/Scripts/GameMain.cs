using System.Collections;
using System.Collections.Generic;
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
    }

    private void Init()
    {
        Global.IsPause = false;
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
        _fsm.SetNextStateId((int)eGameState.STG);

        Application.targetFrameRate = 60;
    }
}
