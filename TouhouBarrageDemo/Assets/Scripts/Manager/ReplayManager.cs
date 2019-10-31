using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReplayManager
{
    private static ReplayManager _instance;

    public static ReplayManager GetInstance()
    {
        if (_instance == null)
            _instance = new ReplayManager();
        return _instance;
    }

    private ReplayManager()
    {
        
    }

    private int _lastFrame;
    private List<eSTGKey> _keyList;

    public void SaveReplay()
    {
        if (Global.IsInReplayMode)
            return;
        _keyList = OperationController.GetInstance().GetOperationKeyList();
        _lastFrame = STGStageManager.GetInstance().GetFrameSinceStageStart();
    }

    public void SetReplayData(List<eSTGKey> keyList,int lastFrame)
    {
        _keyList = keyList;
        _lastFrame = lastFrame;
    }

    public int GetReplayLastFrame()
    {
        return _lastFrame;
    }

    public List<eSTGKey> GetReplayKeyList()
    {
        return _keyList;
    }
}
