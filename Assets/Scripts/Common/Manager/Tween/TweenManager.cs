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
        _tweenMap = new Dictionary<GameObject, List<TweenBase>>();
    }

    private Dictionary<GameObject, List<TweenBase>> _tweenMap;

    public void AddTween(GameObject go,TweenBase tween)
    {
        List<TweenBase> tweenList;
        if ( !_tweenMap.TryGetValue(go,out tweenList) )
        {
            tweenList = new List<TweenBase>();
            _tweenMap.Add(go, tweenList);
        }
        tweenList.Add(tween);
    }

    public void Update()
    {
        
    }
}
