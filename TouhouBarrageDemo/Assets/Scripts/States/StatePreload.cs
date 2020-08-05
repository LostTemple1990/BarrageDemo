using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class StatePreload : IState, ICommand
{

    enum ePreloadState : byte
    {
#if Release
        CreateLogFile,
        CreateReplayFolder,
#endif
        PreloadAtlas,
        InitReplay,
        Wait,
        PreloadFinish,
    }

    private ePreloadState _curState;

    public StatePreload()
    {

    }

    private GameStateMachine _fsm;

    public void Execute(int cmd, object data)
    {
        //switch (cmd)
        //{
            
        //}
    }

    public int GetStateId()
    {
        return (int)eGameState.Preload;
    }

    public void OnInit(IFSM fsm)
    {
        _fsm = fsm as GameStateMachine;
    }

    public void OnStateEnter(object data = null)
    {
        // 打开预加载界面
        UIManager.GetInstance().ShowView(WindowName.PreloadView);

        if (_preloadAtlasList == null)
        {
            _preloadAtlasList = new List<string>();
            _preloadAtlasList.Add("ItemAtlas");
            _preloadAtlasList.Add("STGBulletsAtlas");
            _preloadAtlasList.Add("STGLaserAtlas0");
            _preloadAtlasList.Add("STGLaserAtlas1");
            _preloadAtlasList.Add("STGEffectAtlas");
            _preloadAtlasList.Add("STGPlayerLaserAtlas");
        }
#if Debug
        SetNextState(ePreloadState.PreloadAtlas);
#elif Release
        SetNextState(ePreloadState.CreateLogFile);
#endif
    }

    public void OnStateExit()
    {
        
    }

    public void OnUpdate()
    {
        if (_curState == ePreloadState.Wait)
        {
            OnPreloadWaitUpdate();
        }
#if Release
        else if (_curState == ePreloadState.CreateLogFile)
        {
            OnCreateLogFileUpdate();
        }
        else if (_curState == ePreloadState.CreateReplayFolder)
        {
            OnCreateReplayFolderUpdate();
        }
#endif
        else if (_curState == ePreloadState.PreloadAtlas)
        {
            OnPereloadAtlasUpdate();
        }
        else if (_curState == ePreloadState.InitReplay)
        {
            OnInitReplayUpdate();
        }
        else if (_curState == ePreloadState.PreloadFinish)
        {
            OnPreloadFinishUpdate();
        }
    }

    private int _listCounter;

    private void SetNextState(ePreloadState nextState,object data=null)
    {
        _curState = nextState;
        if (nextState == ePreloadState.PreloadAtlas)
        {
            _listCounter = 0;
        }
        else if (nextState == ePreloadState.Wait)
        {
            _waitData = (WaitData)data;
#if Debug
            // debug模式下不等待
            _waitData.waitTime = 0;
#endif
            _waitData.waitCounter = 0;
        }
    }

#region CreateNecessaryFiles
#if Release
    private void OnCreateLogFileUpdate()
    {
        WaitData data = new WaitData();
        data.waitTime = 10;
        data.nextState = ePreloadState.CreateReplayFolder;
        SetNextState(ePreloadState.Wait, data);
    }

    private void OnCreateReplayFolderUpdate()
    {
        string path = Application.dataPath + "/../Rep";
        if (!System.IO.Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        WaitData data = new WaitData();
        data.waitTime = 10;
        data.nextState = ePreloadState.PreloadAtlas;
        SetNextState(ePreloadState.Wait, data);
    }
#endif
    #endregion

#region PreloadAtlas
    private List<string> _preloadAtlasList;

    private void OnPereloadAtlasUpdate()
    {
        string atlasName = _preloadAtlasList[_listCounter];
        ResourceManager.GetInstance().GetSpriteAtlas(atlasName);

        _listCounter++;
        if (_listCounter>=_preloadAtlasList.Count)
        {
            WaitData data = new WaitData
            {
                waitTime = 10,
                nextState = ePreloadState.InitReplay,
            };
            SetNextState(ePreloadState.Wait, data);
        }
    }
#endregion

#region InitReplay
    private void OnInitReplayUpdate()
    {
        ReplayManager.GetInstance().Init();
        WaitData data = new WaitData
        {
            waitTime = 60,
            nextState = ePreloadState.PreloadFinish,
        };
        SetNextState(ePreloadState.Wait, data);
    }
#endregion

#region Wait
    struct WaitData
    {
        public int waitCounter;
        public int waitTime;
        public object data;
        public ePreloadState nextState;
    }

    private WaitData _waitData;

    private void OnPreloadWaitUpdate()
    {
        _waitData.waitCounter++;
        if (_waitData.waitCounter >= _waitData.waitTime)
        {
            SetNextState(_waitData.nextState, _waitData.data);
        }
    }
#endregion

#region PreloadFinish
    private void OnPreloadFinishUpdate()
    {
        CommandManager.GetInstance().RunCommand(CommandConsts.PreloadComplete);
#if StartFromGame
#if StartWithReimu
        STGData data = new STGData()
        {
            stageName = "Stage1",
            characterIndex = 0,
            isReplay = false,
        };
#elif StartWithMarisa
        STGData data = new STGData()
        {
            stageName = "Stage1",
            characterIndex = 1,
            isReplay = false,
        };
#endif
        _fsm.SetNextStateId((int)eGameState.STG, data);
#else
        _fsm.SetNextStateId((int)eGameState.Title);
#endif
    }
#endregion
}