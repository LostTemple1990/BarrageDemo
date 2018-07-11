using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager
{
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

    private float _curVolume = 0.0f;

    public void Init(Transform tf)
    {
        _toPlayList = new List<SoundEntity>();
        _toPlayListCount = 0;
        _curPlayList = new List<SoundEntity>();
        _defaultSourceTf = tf;
        _pool = new Stack<SoundEntity>();
    }

    public void Play(string name,bool isLoop)
    {
        int i;
        for (i=0;i<_toPlayListCount;i++)
        {
            if ( _toPlayList[i].soundName == name )
            {
                return;
            }
        }
        //Logger.Log("Play New Sound " + name);
        SoundEntity entity = GetAtPool();
        entity.soundName = name;
        entity.isLoop = isLoop;
        _toPlayList.Add(entity);
        _toPlayListCount++;
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
                    curEntity.endTime = curTime + curEntity.source.clip.length;
                    curEntity.source.Play();
                    //Logger.Log("Sound exist, Play Again");
                    isInCurPlayList = true;
                    break;
                }
            }
            if ( isInCurPlayList )
            {
                continue;
            }
            // 没有播放，则创建新的音效
            GameObject soundObj = toPlayEntity.soundObj;
            if ( soundObj == null )
            {
                soundObj = new GameObject();
                soundObj.transform.parent = _defaultSourceTf;
                toPlayEntity.soundObj = soundObj;
            }
            soundObj.name = "Audio_" + toPlayEntity.soundName;
            AudioSource source = toPlayEntity.source;
            if ( source == null )
            {
                source = soundObj.AddComponent<AudioSource>();
                source.volume = _curVolume;
                toPlayEntity.source = source;
            }
            source.clip = Resources.Load<AudioClip>("Sounds/" + toPlayEntity.soundName);
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
            if ( !curPlayEntity.isLoop && curPlayEntity.endTime < curTime )
            {
                curPlayEntity.isFinish = true;
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
        if ( entity.source != null )
        {
            entity.source.Stop();
            entity.source.clip = null;
        }
        _pool.Push(entity);
    }
}

public class SoundEntity
{
    public string soundId;
    public string soundName;
    public bool isLoop;
    public double endTime;
    public bool isFinish;
    public GameObject soundObj;
    public AudioSource source;
}
