using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TweenManager
{
    private static TweenManager _instance = new TweenManager();

    public static TweenManager GetInstance()
    {
        return _instance;
    }

    private TweenManager()
    {
        _tweenMap = new Dictionary<int, List<TweenBase>>();
    }

    private Dictionary<int, List<TweenBase>> _tweenMap;
    private List<int> _curTweenGoList;
    private int _curTweenGoCount;
    private List<int> _clearList;
    private int _clearListCount;

    public void Init()
    {
        if ( _curTweenGoList == null )
        {
            _curTweenGoList = new List<int>();
        }
        _curTweenGoCount = 0;
        if ( _clearList == null )
        {
            _clearList = new List<int>();
        }
        _clearListCount = 0;
    }

    public void AddTween(TweenBase tween)
    {
        List<TweenBase> tweenList;
        GameObject go = tween.GetTweenObject();
        if ( go != null )
        {
            int hash = go.GetHashCode();
            if (!_tweenMap.TryGetValue(hash, out tweenList))
            {
                tweenList = new List<TweenBase>();
                _tweenMap.Add(hash, tweenList);
                _curTweenGoList.Add(hash);
                _curTweenGoCount++;
            }
            tweenList.Add(tween);
        }
    }

    /// <summary>
    /// 添加缓动动画
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tween"></param>
    public void AddTween(GameObject go,TweenBase tween)
    {
        List<TweenBase> tweenList;
        int hash = go.GetHashCode();
        if ( !_tweenMap.TryGetValue(hash, out tweenList) )
        {
            tweenList = new List<TweenBase>();
            _tweenMap.Add(hash, tweenList);
            _curTweenGoList.Add(hash);
            _curTweenGoCount++;
        }
        tweenList.Add(tween);
    }

    /// <summary>
    /// 添加缓动动画
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tweens"></param>
    public void AddTweens(GameObject go,List<TweenBase> tweens)
    {
        List<TweenBase> tweenList;
        int hash = go.GetHashCode();
        if (!_tweenMap.TryGetValue(hash, out tweenList))
        {
            tweenList = new List<TweenBase>();
            _tweenMap.Add(hash, tweenList);
            _curTweenGoList.Add(hash);
            _curTweenGoCount++;
        }
        tweenList.AddRange(tweens);
    }

    /// <summary>
    /// 移除物体的所有缓动
    /// </summary>
    /// <param name="go"></param>
    public void RemoveTweenByGo(GameObject go)
    {
        int hash = go.GetHashCode();
        if ( _curTweenGoList.IndexOf(hash) != -1 )
        {
            List<TweenBase> list;
            if (_tweenMap.TryGetValue(hash, out list))
            {
                TweenBase tween;
                int listCount = list.Count;
                for (int i=0;i<listCount;i++)
                {
                    tween = list[i];
                    if ( tween != null )
                    {
                        tween.RestoreToPool();
                        list[i] = null;
                    }
                }
            }
        }
    }

    public void Update()
    {
        if ( _curTweenGoCount > 0 )
        {
            UpdateTweens();
        }
        // 清除
        if ( _clearListCount > 0 )
        {
            ClearFinishedTweens();
        }
    }

    /// <summary>
    /// 创建一个tween对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Create<T>() where T : TweenBase,new()
    {
        T tween = ObjectsPool.GetInstance().GetPoolClassAtPool<T>();
        return tween;
    }

    private void UpdateTweens()
    {
        TweenBase tween;
        List<TweenBase> list;
        for (int i = 0; i < _curTweenGoCount; i++)
        {
            if (_tweenMap.TryGetValue(_curTweenGoList[i], out list))
            {
                bool isAllFinish = true;
                int listCount = list.Count;
                // 遍历tweenList
                for (int j = 0; j < listCount; j++)
                {
                    tween = list[j];
                    if (tween != null && !tween.IsFinish)
                    {
                        isAllFinish = false;
                        tween.Update();
                        // 先做判断防止已经在回调里面被回收
                        if (list[j] != null && tween.IsFinish)
                        {
                            tween.RestoreToPool();
                            list[j] = null;
                        }
                    }
                }
                if (isAllFinish)
                {
                    list.Clear();
                    _tweenMap.Remove(_curTweenGoList[i]);
                    _clearList.Add(_curTweenGoList[i]);
                    _clearListCount++;
                }
            }
        }
    }

    private void ClearFinishedTweens()
    {
        foreach (int hash in _clearList)
        {
            int index = _curTweenGoList.IndexOf(hash);
            if (index != -1)
            {
                _curTweenGoList.RemoveAt(index);
                _curTweenGoCount--;
            }
        }
        _clearList.Clear();
        _clearListCount = 0;
    }
}
