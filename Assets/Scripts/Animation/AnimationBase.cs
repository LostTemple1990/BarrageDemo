using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void PlayAnimationCompleteCallBack(object cbData);

public class AnimationBase
{
    protected int _curFrame;
    protected int _totalFrame;
    protected int _frameInterval;
    protected int _curTime;

    protected AniActionType _curAction;
    protected int _curDir;

    protected Sprite[] _spList;
    protected bool _isPlaying;
    protected bool _isLoop;
    protected int _curLoopCount;
    protected int _totalLoopCount;

    protected GameObject _aniParent;
    protected Transform _trans;
    /// <summary>
    /// 播放动画的本体GameObject，上面挂载有SpriteRenderer
    /// </summary>
    protected GameObject _aniObject;
    protected Transform _aniTf;
    protected SpriteRenderer _renderer;
    /// <summary>
    /// 是否使用的默认的prefab
    /// <para>当使用默认的prefab时Clear的时候会存入对象池</para>
    /// </summary>
    protected bool _isDefaultPrefab;

    protected AnimationCache _cache;

    protected PlayAnimationCompleteCallBack _completeCBFunc;
    protected object _completeCBPara;

    public virtual void Init(LayerId id)
    {
        _aniParent = ObjectsPool.GetInstance().GetPrefabAtPool("AniParent");
        if ( _aniParent != null )
        {
            _aniParent.SetActive(true);
        }
        else
        {
            _aniParent = ResourceManager.GetInstance().GetPrefab("Animation", "AniParent");
        }
        _trans = _aniParent.transform;
        _aniTf = _aniParent.transform.Find("Animation");
        _aniObject = _aniTf.gameObject;
        _renderer = _aniTf.GetComponent<SpriteRenderer>();
        UIManager.GetInstance().AddGoToLayer(_aniParent, id);
        _isDefaultPrefab = true;
    }

    public virtual void Init(GameObject aniParent,string rendererName,LayerId id)
    {
        _aniParent = aniParent;
        _trans = _aniParent.transform;
        _aniTf = _aniParent.transform.Find(rendererName);
        _aniObject = _aniTf.gameObject;
        _renderer = _aniTf.GetComponent<SpriteRenderer>();
        UIManager.GetInstance().AddGoToLayer(_aniParent, id);
        _isDefaultPrefab = false;
    }

    public virtual void Update ()
    {
		if ( _isPlaying )
        {
            int playFrame = GetFrame();
            if (playFrame != _curFrame )
            {
                _curFrame = playFrame;
                _renderer.sprite = _spList[_curFrame];
            }
            if ( _curLoopCount >= _totalLoopCount )
            {
                if (_completeCBFunc != null)
                {
                    _completeCBFunc(_completeCBPara);
                }
            }
        }
	}

    /// <summary>
    /// 获取当前应该播放的帧
    /// </summary>
    /// <returns></returns>
    protected virtual int GetFrame()
    {
        int frame;
        // 没有达到下一帧的时间间隔，不切换播放帧
        if ( _curTime < _frameInterval )
        {
            frame = _curFrame;
        }
        else
        {
            if (_curFrame < _totalFrame - 1)
            {
                frame = _curFrame + 1;
            }
            else
            {
                _curLoopCount++;
                if (!_isLoop || _curLoopCount >= _totalLoopCount)
                {
                    _isPlaying = false;
                    frame = _curFrame;
                }
                else
                {
                    frame = 0;
                }
            }
            _curTime = 0;
        }
        _curTime++;
        return frame;
    }

    public virtual bool Play(AniActionType type,int dir,int interval,bool isLoop=true,int loopCount=int.MaxValue)
    {
        _curAction = type;
        _curDir = dir;
        _isPlaying = true;
        _curFrame = 0;
        _spList = _cache.GetSprites(GetPlayString(type,dir));
        _totalFrame = _spList.Length;
        _isLoop = isLoop;
        _curLoopCount = 0;
        _totalLoopCount = loopCount;
        return true;
    }

    public virtual bool Play(string aniName,AniActionType type, int dir, int interval, bool isLoop = true, int loopCount = int.MaxValue)
    {
        _cache = AnimationManager.GetInstance().GetAnimationCache(aniName);
        _isPlaying = true;
        _curFrame = 0;
        _spList = _cache.GetSprites(GetPlayString(type, dir));
        _totalFrame = _spList.Length;
        _isLoop = isLoop;
        _curLoopCount = 0;
        _totalLoopCount = loopCount;
        return true;
    }

    /// <summary>
    /// 根据参数获得对应动画数组的key
    /// </summary>
    /// <param name="type"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    protected virtual string GetPlayString(AniActionType type, int dir)
    {
        string key = "";
        if (type == AniActionType.Idle)
        {
            key = "Idle";
        }
        else
        {
            // action
            switch (type)
            {
                case AniActionType.Move:
                    key = "";
                    break;
                case AniActionType.FadeToMove:
                    key = "FadeTo";
                    break;
            }
            // dir
            string dirStr = "";
            if (dir == Consts.DIR_RIGHT)
            {
                dirStr = "Right";
            }
            else if (dir == Consts.DIR_LEFT)
            {
                    dirStr = "Left";
            }
            key = key + dirStr;
        }
        return key;
    }

    public virtual void Clear()
    {
        _spList = null;
        _renderer = null;
        _trans = null;
        if ( _isDefaultPrefab )
        {
            _aniParent.SetActive(false);
            ObjectsPool.GetInstance().RestorePrefabToPool("AniParent",_aniParent);
        }
        else
        {
            GameObject.Destroy(_aniParent);
        }
        _spList = null;
        _renderer = null;
        _trans = null;
        _aniTf = null;
        _aniObject = null;
        _aniParent = null;
        _completeCBFunc = null;
        _completeCBPara = null;
        _cache = null;
    }

    /// <summary>
    /// 动画最上层父类GameObject
    /// </summary>
    /// <returns></returns>
    public virtual GameObject GetAniParent()
    {
        return _aniParent;
    }

    /// <summary>
    /// 播放动画的本体GameObject，上面挂载有SpriteRenderer
    /// </summary>
    /// <returns></returns>
    public virtual GameObject GetAniObject()
    {
        return _aniObject;
    }

    //public virtual GameObject 

    /// <summary>
    /// 设置播放完成的回调
    /// </summary>
    /// <param name="callback"></param>
    public void SetPlayCompleteCallBack(PlayAnimationCompleteCallBack callback,object callBackPara=null)
    {
        _completeCBFunc = callback;
        _completeCBPara = callBackPara;
    }

    public void SetSpriteLayer(string layerName)
    {
        _renderer.sortingLayerName = layerName;
    }

    public void SetSpriteLayer(int layerId)
    {
        _renderer.sortingLayerID = layerId;
    }

    public void SetSpriteOrder(int value)
    {
        _renderer.sortingOrder = value;
    }
}
