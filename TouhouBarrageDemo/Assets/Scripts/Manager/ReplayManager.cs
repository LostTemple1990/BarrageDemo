using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
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

    private STGData _stgData;

    private List<ReplayInfo> _replayInfoList;
    private ReplayData _repData;

    public void Init()
    {
        // 加载info文件
        string path = Application.streamingAssetsPath + "/Rep/data.ri";
        if (!System.IO.File.Exists(path))
        {
            ReplayInfos tmp = new ReplayInfos();
            tmp.infos = new List<ReplayInfo>();
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, tmp);
            fs.Close();
            _replayInfoList = tmp.infos;
        }
        else
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            ReplayInfos tmp = (ReplayInfos)bf.Deserialize(fs);
            fs.Close();
            _replayInfoList = tmp.infos;
        }
    }

    public void SaveReplay(int index, string name, STGData data)
    {
        if (Global.IsInReplayMode)
            return;
        ReplayInfo info = new ReplayInfo
        {
            replayIndex = index,
            name = name,
            dateTick = System.DateTime.Now.Ticks,
            lastFrame = STGStageManager.GetInstance().GetFrameSinceStageStart(),
            stageName = data.stageName,
            characterIndex = data.characterIndex,
        };
        ReplayData repData = new ReplayData();
        repData.info = info;
        repData.keyList = OperationController.GetInstance().GetOperationKeyList();
        repData.lastFrame = STGStageManager.GetInstance().GetFrameSinceStageStart();
        repData.seed = data.seed;
        // 写入info文件
        WriteRepInfoFile(info);
        WriteRepDataFile(repData);
    }

    /// <summary>
    /// 将info写入RepInfo文件中
    /// </summary>
    /// <param name="info"></param>
    private void WriteRepInfoFile(ReplayInfo info)
    {
        string path = Application.streamingAssetsPath + "/Rep/data.ri";
        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        ReplayInfos tmp = (ReplayInfos)bf.Deserialize(fs);
        fs.Close();
        bool isExist = false;
        for (int i=0;i<tmp.infos.Count;i++)
        {
            if (tmp.infos[i].replayIndex == info.replayIndex)
            {
                isExist = true;
                tmp.infos[i] = info;
                break;
            }
        }
        if (!isExist)
        {
            tmp.infos.Add(info);
        }
        fs = new FileStream(path, FileMode.Truncate);
        bf = new BinaryFormatter();
        bf.Serialize(fs, tmp);
        fs.Close();
        // 将更新后的info赋值
        _replayInfoList = tmp.infos;
    }

    /// <summary>
    /// 将ReplayData写入rep文件中
    /// </summary>
    /// <param name=""></param>
    private void WriteRepDataFile(ReplayData repData)
    {
        string path = Application.streamingAssetsPath + "/Rep/replay" + repData.info.replayIndex + ".rep";
        FileStream fs;
        if (System.IO.File.Exists(path))
        {
            fs = new FileStream(path, FileMode.Truncate);
        }
        else
        {
            fs = new FileStream(path, FileMode.Create);
        }
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, repData);
        fs.Close();
    }

    public bool LoadReplay(int replayIndex)
    {
        string path = Application.streamingAssetsPath + "/Rep/replay" + replayIndex + ".rep";
        if (!System.IO.File.Exists(path))
        {
            return false;
        }
        FileStream fs = new FileStream(path, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        ReplayData repData = (ReplayData)bf.Deserialize(fs);
        fs.Close();
        _repData = repData;
        CommandManager.GetInstance().RunCommand(CommandConsts.LoadReplaySuccess, repData);
        return true;
    }

    public long GetSeed()
    {
        return _repData.seed;
    }

    public int GetReplayLastFrame()
    {
        return _repData.lastFrame;
    }

    public List<eSTGKey> GetReplayKeyList()
    {
        return _repData.keyList;
    }

    /// <summary>
    /// 获取当前保存的replay基本信息
    /// </summary>
    /// <returns></returns>
    public List<ReplayInfo> GetReplayInfoList()
    {
        return _replayInfoList;
    }

    [Serializable]
    struct ReplayInfos
    {
        public List<ReplayInfo> infos;
    }
}
