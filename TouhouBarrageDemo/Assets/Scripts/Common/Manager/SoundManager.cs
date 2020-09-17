using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager
{
    /// <summary>
    /// 重复播放同一音效的最小间隔
    /// </summary>
    private const float RepeatPlayMinInterval = 0.1f;

    private static SoundManager _instance = new SoundManager();

    public static SoundManager GetInstance()
    {
        return _instance;
    }

    private List<SoundEntity> _toPlayList;
    private int _toPlayListCount;
    private List<SoundEntity> _curPlayList;
    private int _curPlayCount;
    private Transform _defaultSourceTf;
    private Stack<SoundEntity> _pool;
    /// <summary>
    /// 当前是否正在暂停所有STG音效
    /// </summary>
    private bool _isPausingSTGSound;

    private Dictionary<string, int> _soundLoadedMap;

    public void Init(Transform tf)
    {
        _toPlayList = new List<SoundEntity>();
        _toPlayListCount = 0;
        _curPlayList = new List<SoundEntity>();
        _defaultSourceTf = tf;
        _pool = new Stack<SoundEntity>();
        _soundLoadedMap = new Dictionary<string, int>();
        _isPausingSTGSound = false;
    }

    public void Play(string name, bool isLoop = false,bool isSTGSound = true)
    {
#if SoundEnable
        int i;
        for (i = 0; i < _toPlayListCount; i++)
        {
            if (_toPlayList[i].soundName == name)
            {
                return;
            }
        }
        //Logger.Log("PlaySound \"+" + name + "\" PoolCount = " + _pool.Count);
        SoundEntity entity = GetAtPool();
        entity.soundName = name;
        entity.volume = 0.5f;
        entity.isLoop = isLoop;
        entity.isSTGSound = isSTGSound;
        _toPlayList.Add(entity);
        _toPlayListCount++;
#endif
    }

    public void Play(string name, float volume, bool isLoop = false,bool isSTGSound = true)
    {
#if SoundEnable
        int i;
        for (i=0;i<_toPlayListCount;i++)
        {
            if ( _toPlayList[i].soundName == name )
            {
                return;
            }
        }
        //Logger.Log("PlaySound \"" + name + "\" PoolCount = " + _pool.Count);
        SoundEntity entity = GetAtPool();
        entity.soundName = name;
        entity.volume = Mathf.Clamp01(volume);
        entity.isLoop = isLoop;
        entity.isSTGSound = isSTGSound;
        _toPlayList.Add(entity);
        _toPlayListCount++;
#endif
    }

    public void Pause(string name)
    {
        SoundEntity entity;
        for (int i=0;i<_curPlayCount;i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.soundName == name)
            {
                entity.pauseTime = Time.realtimeSinceStartup;
                entity.isPause = true;
                entity.source.Pause();
                break;
            }
        }
    }

    /// <summary>
    /// 停止所有STG音效
    /// </summary>
    public void PauseAllSTGSound()
    {
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.isSTGSound)
            {
                entity.pauseTime = Time.realtimeSinceStartup;
                entity.isPause = true;
                entity.source.Pause();
            }
        }
        _isPausingSTGSound = true;
    }

    /// <summary>
    /// 恢复所有STG音效
    /// </summary>
    public void ResumeAllSTGSound()
    {
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.isSTGSound)
            {
                if (entity.isPause)
                {
                    // 更新该音效的结束时间
                    // 当该音效有明确的结束时间的时候才进行更新
                    // 即，非循环音效
                    if (entity.endTime > 0)
                    {
                        entity.endTime += Time.realtimeSinceStartup - entity.pauseTime;
                    }
                    entity.isPause = false;
                    entity.source.UnPause();
                }
            }
        }
        _isPausingSTGSound = false;
    }

    /// <summary>
    /// 停止所有STG音效
    /// </summary>
    public void StopAllSTGSound()
    {
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.isSTGSound)
            {
                RestoreToPool(entity);
                _curPlayList[i] = null;
            }
        }
    }

    public void Resume(string name)
    {
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.soundName == name)
            {
                if (entity.isPause)
                {
                    // 更新该音效的结束时间
                    // 当该音效有明确的结束时间的时候才进行更新
                    // 即，非循环音效
                    if (entity.endTime > 0)
                    {
                        entity.endTime += Time.realtimeSinceStartup - entity.pauseTime;
                    }
                    entity.isPause = false;
                    entity.source.UnPause();
                }
                break;
            }
        }
    }

    public void Stop(string name)
    {
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.soundName == name)
            {
                RestoreToPool(entity);
                _curPlayList[i] = null;
                break;
            }
        }
    }

    /// <summary>
    /// 输出当前音乐的时间
    /// </summary>
    /// <param name="name"></param>
    public void PrintSoundTime(string name)
    {
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.soundName == name)
            {
                Logger.Log("Current play time of sound " + name + " is " + entity.source.time);
                break;
            }
        }
    }

    public void SetSoundPlayTime(string name, float time)
    {
        if (time < 0)
            return;
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity != null && entity.soundName == name)
            {
                entity.source.time = time;
                break;
            }
        }
    }

    public AudioClip Load(string name,bool isPreload = false)
    {
        int flag;
        if (isPreload && _soundLoadedMap.TryGetValue(name, out flag))
        {
            return null;
        }
        AudioClip clip = Resources.Load<AudioClip>("Sounds/" + name);
        _soundLoadedMap.Add(name, 1);
        if (isPreload)
        {
            //GameObject.Destroy(clip);
            return null;
        }
        return clip;
    }

    public void Clear(bool onlySTGSound = true)
    {
        SoundEntity entity;
        for (int i = 0; i < _curPlayCount; i++)
        {
            entity = _curPlayList[i];
            if (entity == null)
                continue;
            if (onlySTGSound && !entity.isSTGSound)
                continue;
            RestoreToPool(entity);
            _curPlayList[i] = null;
        }
        _isPausingSTGSound = false;
    }

    public void Update()
    {
        AddToCurPlayList();
        UpdateCurPlayList();
    }

    /// <summary>
    /// 将toPlayList的音效放入playlist中
    /// </summary>
    private void AddToCurPlayList()
    {
        int i, j;
        SoundEntity curEntity,toPlayEntity;
        float curTime = Time.realtimeSinceStartup;
        bool isInCurPlayList;
        // 当前音效正在播放，则重复播放
        for (i=0;i<_toPlayListCount;i++)
        {
            toPlayEntity = _toPlayList[i];
            isInCurPlayList = false;
            for (j=0;j<_curPlayCount;j++)
            {
                curEntity = _curPlayList[j];
                if (curEntity == null)
                    continue;
                // 当前音效正在播放，则重复播放
                if ( curEntity.soundName == toPlayEntity.soundName )
                {
                    // 间隔时间大于最小间隔的时候才重复播放
                    if ( curTime - curEntity.startTime >= RepeatPlayMinInterval )
                    {
                        curEntity.endTime = curTime + curEntity.source.clip.length;
                        curEntity.source.Play();
                        //Logger.Log("Sound exist, Play Again");
                    }
                    isInCurPlayList = true;
                    break;
                }
            }
            if ( isInCurPlayList )
            {
                RestoreToPool(toPlayEntity);
                continue;
            }
            // 没有播放，则创建新的音效
            GameObject soundObj = toPlayEntity.soundObj;
            if ( soundObj == null )
            {
                soundObj = new GameObject();
                soundObj.transform.SetParent(_defaultSourceTf, false);
                toPlayEntity.soundObj = soundObj;
            }
            soundObj.name = "Audio_" + toPlayEntity.soundName;
            AudioSource source = toPlayEntity.source;
            if ( source == null )
            {
                source = soundObj.AddComponent<AudioSource>();
                toPlayEntity.source = source;
            }
            source.volume = toPlayEntity.volume;
            // 起始播放时间
            toPlayEntity.startTime = curTime;
            source.clip = Resources.Load<AudioClip>("Sounds/" + toPlayEntity.soundName);
            if (source.clip == null)
                Logger.LogError("Sound " + toPlayEntity.soundName + " is not exist!");
            // 是否循环
            source.loop = toPlayEntity.isLoop;
            // 结束播放时间
            toPlayEntity.endTime = toPlayEntity.isLoop ? -1 : curTime + source.clip.length;
            //Logger.Log("Begin Play sound " + toPlayEntity.soundName + " length = " + source.clip.length);
            toPlayEntity.isFinish = false;
            // 添加到播放列表中
            _curPlayList.Add(toPlayEntity);
            _curPlayCount++;
            source.Play();
            // 如果当前STG音效处于暂停状态，则直接暂停该音效
            if (_isPausingSTGSound && toPlayEntity.isSTGSound)
            {
                Pause(toPlayEntity.soundName);
            }
        }
        // 清空待播放列表
        _toPlayList.Clear();
        _toPlayListCount = 0;
    }

    private void UpdateCurPlayList()
    {
        int i,j;
        SoundEntity curPlayEntity;
        float curTime = Time.realtimeSinceStartup;
        // 检测已经播放完的audioSource
        for (i=0;i<_curPlayCount;i++)
        {
            curPlayEntity = _curPlayList[i];
            if (curPlayEntity == null)
                continue;
            if (curPlayEntity.isPause)
                continue;
            if (!curPlayEntity.isFinish)
            {
                if (!curPlayEntity.isLoop && curPlayEntity.endTime < curTime)
                {
                    curPlayEntity.isFinish = true;
                }
            }
            if (curPlayEntity.isFinish)
            {
                RestoreToPool(curPlayEntity);
                _curPlayList[i] = null;
            }
        }
        // 从playList中移除已经播放完的部分
        int findFlag;
        for (i=0,j=1;i<_curPlayCount;i++)
        {
            if ( _curPlayList[i] == null )
            { 
                findFlag = 0;
                j = j == 1 ? i + 1 : j;
                for ( ; j < _curPlayCount; j++)
                {
                    if ( _curPlayList[j] != null )
                    {
                        findFlag = 1;
                        _curPlayList[i] = _curPlayList[j];
                        _curPlayList[j] = null;
                        break;
                    }
                }
                if ( findFlag == 0 )
                {
                    break;
                }
            }
        }
        _curPlayList.RemoveRange(i, _curPlayCount - i);
        _curPlayCount = i;
    }

    private SoundEntity GetAtPool()
    {
        if ( _pool.Count > 0 )
        {
            return _pool.Pop();
        }
        return new SoundEntity();
    }

    private void RestoreToPool(SoundEntity entity)
    {
        //Logger.Log("Restore \"" + entity.soundName + "\" to Pool ,PoolCount = " + (_pool.Count+1));
        entity.Clear();
        _pool.Push(entity);
    }
}

public class SoundEntity
{
    public string soundId;
    public string soundName;
    public bool isLoop;
    /// <summary>
    /// 播放音效的起始时间
    /// </summary>
    public double startTime;
    /// <summary>
    /// 播放音效的结束时间
    /// </summary>
    public double endTime;
    public bool isFinish;
    public GameObject soundObj;
    public AudioSource source;
    /// <summary>
    /// 音量大小
    /// </summary>
    public float volume;
    /// <summary>
    /// 当前音效是否暂停中
    /// </summary>
    public bool isPause;
    /// <summary>
    /// 暂停的时间
    /// </summary>
    public double pauseTime = 0;
    /// <summary>
    /// 是否STG播放的音效
    /// </summary>
    public bool isSTGSound;

    public SoundEntity()
    {
        isFinish = false;
        isPause = false;
    }

    public void Clear()
    {
        if (source != null)
        {
            source.Stop();
            source.time = 0;
            source.clip = null;
        }
        isFinish = false;
        isPause = false;
    }
}
