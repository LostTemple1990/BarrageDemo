using UnityEngine;
using System.Collections;

public class AnimationCharacter : AnimationBase
{
    protected UnitAniCfg _cfg;

    //public override bool Play(string aniName, AniActionType type, int dir, int interval, bool isLoop = true, int loopCount = int.MaxValue)
    //{
    //    _curAction = type;
    //    _curDir = dir;
    //    base.Play(aniName,type)
    //    _cache = AnimationManager.GetInstance().GetAnimationCache(aniName);
    //    _isPlaying = true;
    //    _curFrame = 0;
    //    _spList = _cache.GetSprites(get);
    //    _totalFrame = _spList.Length;
    //    _isLoop = isLoop;
    //    _curLoopCount = 1;
    //    _totalLoopCount = loopCount;
    //    return true;
    //}

    public virtual bool Play(string aniId, AniActionType type, int dir)
    {
        _cfg = AnimationManager.GetInstance().GetUnitAniCfgById(aniId);
        if (_cfg == null)
            return false;
        _cache = AnimationManager.GetInstance().GetAnimationCache(_cfg.aniName);
        return Play(type, dir);
    }

    public virtual bool Play(AniActionType type, int dir, int loopCount=int.MaxValue)
    {
        if (_cfg == null)
            return false;
        if ( type == AniActionType.Idle || type == AniActionType.Cast )
        {
            dir = Consts.DIR_NULL;
        }
        if ( type == _curAction && dir == _curDir )
        {
            return false;
        }
        string key;
        int interval;
        Vector3 lScale;
        if ( _cfg.isFlapX && dir == Consts.DIR_LEFT )
        {
            lScale = _aniTf.localScale;
            lScale.x = -Mathf.Abs(lScale.x);
            _aniTf.localScale = lScale;
        }
        else
        {
            lScale = _aniTf.localScale;
            lScale.x = Mathf.Abs(lScale.x);
            _aniTf.localScale = lScale;
        }
        if ( type == AniActionType.Move && _curAction != AniActionType.FadeToMove )
        {
            key = GetPlayString(AniActionType.FadeToMove, dir);
            _cfg.aniIntervalMap.TryGetValue(key, out interval);
            if (Play(AniActionType.FadeToMove, dir, interval, false, 1))
            {
                SetPlayCompleteCallBack(FadeCompleteCBFunc, dir);
                return true;
            }
            else
            {
                return Play(AniActionType.Move, dir, interval, true);
            }
            //Logger.Log("AnimationPlay FadeToMove dir = " + dir);
        }
        key = GetPlayString(type, dir);
        _cfg.aniIntervalMap.TryGetValue(key, out interval);
        return Play(type, dir, interval, true, int.MaxValue);
    }

    protected void FadeCompleteCBFunc(object cbData)
    {
        _completeCBFunc = null;
        _completeCBPara = null;
        int dir = (int)cbData;
        Play(AniActionType.Move, dir);
    }

    /// <summary>
    /// 根据参数获得对应动画数组的key
    /// </summary>
    /// <param name="type"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    protected override string GetPlayString(AniActionType type, int dir)
    {
        string key = "";
        if ( type == AniActionType.Idle )
        {
            key = "Idle";
        }
        else if ( type == AniActionType.Cast )
        {
            key = "Cast";
        }
        else
        {
            // action
            switch ( type )
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
                if (!_cfg.isFlapX)
                {
                    dirStr = "Left";
                }
                else
                {
                    dirStr = "Right";
                }
            }
            key = key + dirStr;
        }
        return key;
    }

    public override void Clear()
    {
        base.Clear();
        _cfg = null;
    }
}
