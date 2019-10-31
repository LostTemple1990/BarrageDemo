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
    private long _seed;

    public void SaveReplay()
    {
        if (Global.IsInReplayMode)
            return;
        _keyList = OperationController.GetInstance().GetOperationKeyList();
        _lastFrame = STGStageManager.GetInstance().GetFrameSinceStageStart();
        _seed = Global.RandomSeed;
    }

    public void SetReplayData(List<eSTGKey> keyList,int lastFrame)
    {
        _keyList = keyList;
        _lastFrame = lastFrame;
    }

    public long GetSeed()
    {
        return _seed;
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
