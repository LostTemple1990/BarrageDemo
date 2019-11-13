#define SoundEnable 

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

    private Dictionary<string, int> _soundLoadedMap;

    public void Init(Transform tf)
    {
        _toPlayList = new List<SoundEntity>();
        _toPlayListCount = 0;
        _curPlayList = new List<SoundEntity>();
        _defaultSourceTf = tf;
        _pool = new Stack<SoundEntity>();
        _soundLoadedMap = new Dictionary<string, int>();
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
                break;
            }
        }
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
                entity.source.Stop();
                entity.isFinish = true;
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
                entity.source.Stop();
                entity.isFinish = true;
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
            if (entity.source != null)
            {
                entity.source.Stop();
            }
            entity.isFinish = true;
        }
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
            // 结束播放时间
            toPlayEntity.endTime = toPlayEntity.isLoop ? -1 : curTime + source.clip.length;
            //Logger.Log("Begin Play sound " + toPlayEntity.soundName + " length = " + source.clip.length);
            toPlayEntity.isFinish = false;
            // 添加到播放列表中
            _curPlayList.Add(toPlayEntity);
            _curPlayCount++;
            source.Play();
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
            source.clip = null;
        }
        isFinish = false;
        isPause = false;
    }
}
