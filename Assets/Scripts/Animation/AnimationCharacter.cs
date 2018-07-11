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
        // 设置当前播放的动作类型
        _curAction = type;
        _curDir = type==AniActionType.Idle ? Consts.DIR_NULL : dir;
        _cache = AnimationManager.GetInstance().GetAnimationCache(_cfg.aniName);
        _isPlaying = true;
        _curFrame = 0;
        _spList = _cache.GetSprites(GetPlayString(type,dir));
        _totalFrame = _spList.Length;
        _isLoop = true;
        _curLoopCount = 1;
        _totalLoopCount = int.MaxValue;
        return true;
    }

    public virtual bool Play(AniActionType type, int dir)
    {
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
            SetPlayCompleteCallBack(FadeCompleteCBFunc,dir);
            _cfg.aniIntervalMap.TryGetValue(key, out interval);
            Play(AniActionType.FadeToMove, dir, interval, false, 1);
            //Logger.Log("AnimationPlay FadeToMove dir = " + dir);
            return true;
        }
        key = GetPlayString(type, dir);
        _cfg.aniIntervalMap.TryGetValue(key, out interval);
        Play(type, dir, interval, true, int.MaxValue);
        //Logger.Log("AnimationPlay " + type + " dir = " + dir);
        return true;
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
