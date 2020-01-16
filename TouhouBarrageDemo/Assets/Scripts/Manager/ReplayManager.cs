//#define UseReplayInfoFile

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

    private bool _isEnable;

    public void Init()
    {
#if UseReplayInfoFile
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
#else
        // 挨个读取rep文件生成replay数据
        _replayInfoList = new List<ReplayInfo>();
        string repPath;
        FileStream fs;
        BinaryFormatter bf = new BinaryFormatter();
        for (int i=0;i<Consts.MaxReplayCount;i++)
        {
            repPath = Application.streamingAssetsPath + "/Rep/replay" + i + ".rep";
            if (System.IO.File.Exists(repPath))
            {
                fs = new FileStream(repPath, FileMode.Open);
                ReplayData repData = (ReplayData)bf.Deserialize(fs);
                fs.Close();
                // 添加repInfo
                _replayInfoList.Add(repData.info);
            }
        }
#endif
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
#if UseReplayInfoFile
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
#else
        bool isExist = false;
        for (int i = 0; i < _replayInfoList.Count; i++)
        {
            if (_replayInfoList[i].replayIndex == info.replayIndex)
            {
                isExist = true;
                _replayInfoList[i] = info;
                break;
            }
        }
        if (!isExist)
        {
            _replayInfoList.Add(info);
        }
#endif
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
    /// 设置是否允许录像
    /// </summary>
    /// <param name="value"></param>
    public void SetReplayEnable(bool value)
    {
        _isEnable = value;
    }

    /// <summary>
    /// 是否允许录像
    /// </summary>
    /// <returns></returns>
    public bool IsReplayEnable()
    {
        return _isEnable;
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
