using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationCache
{
    public string AniName;
    private Dictionary<string, Sprite[]> _actionSpsMap;

    public void SetActionSpritesMap(Dictionary<string, Sprite[]> map)
    {
        _actionSpsMap = map;
    }

    public Sprite[] GetSprites(AniActionType type,int dir)
    {
        Sprite[] sp;
        string key = "Idle";
        if ( type == AniActionType.Move )
        {
            key = "Right";
        }
        if ( _actionSpsMap.TryGetValue(key, out sp) )
        {
            return sp;
        }
        return null;
    }

    public Sprite[] GetSprites(string key)
    {
        Sprite[] sp;
        if (_actionSpsMap.TryGetValue(key, out sp))
        {
            return sp;
        }
        return new Sprite[0];
    }
}
