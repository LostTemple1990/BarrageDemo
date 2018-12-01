using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniLua;

public class STGBottomView : ViewBase,ICommand
{
    /// <summary>
    /// 人物立绘GameObject
    /// </summary>
    private GameObject _charCGGo;
    private Image _charCGImg;
    /// <summary>
    /// 是否正在播放人物CG动画
    /// </summary>
    private bool _isPlayingCharCG;

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        _charCGGo = _viewTf.Find("CG").gameObject;
        _charCGImg = _charCGGo.GetComponent<Image>();
        CommandManager.GetInstance().Register(CommandConsts.PlayCharacterCGAni, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryGame, this);
        Reset();
    }

    public void Execute(int cmd, object[] datas)
    {
        switch ( cmd )
        {
            case CommandConsts.PlayCharacterCGAni:
                PlayCGAni(datas);
                break;
            case CommandConsts.RetryStage:
            case CommandConsts.RetryGame:
                Reset();
                break;
        }
    }

    private void PlayCGAni(object[] datas)
    {
        _charCGGo.SetActive(true);
        _isPlayingCharCG = true;
        string path = (string)datas[0];
        _charCGImg.sprite = Resources.Load<Sprite>(path);
        _charCGImg.SetNativeSize();
        ILuaState luaState = (ILuaState)datas[1];
        List<TweenBase> tweenList = ParseLuaTableToTweenList(luaState);
        int count = tweenList.Count;
        int maxTime = 0;
        TweenBase tween;
        for (int i=0;i<count;i++)
        {
            tween = tweenList[i];
            if ( maxTime < tween.delay + tween.duration )
            {
                maxTime = tween.delay + tween.duration;
            }
        }
        TweenPos2D endTween = new TweenPos2D();
        endTween.SetParas(_charCGGo, maxTime + 1, 0, ePlayMode.Once);
        endTween.SetFinishCallBack(OnPlayCGFinish);
        endTween.SetIgnoreTimeScale(false);
        TweenManager.GetInstance().AddTweens(_charCGGo, tweenList);
    }

    /// <summary>
    /// 解析table，转换成对应的tweenList
    /// </summary>
    /// <param name="luaState"></param>
    private List<TweenBase> ParseLuaTableToTweenList(ILuaState luaState)
    {
        List<TweenBase> tweenList = new List<TweenBase>();
        TweenBase tween;
        luaState.PushNil();
        while ( luaState.Next(-2) )
        {
            tween = CreateTweenByTable(luaState);
            tween.SetIgnoreTimeScale(false);
            tweenList.Add(tween);
            luaState.Pop(1);
        }
        return tweenList;
    }

    /// <summary>
    /// 根据栈顶的table创建对应的tween
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    private TweenBase CreateTweenByTable(ILuaState luaState)
    {
        luaState.GetField(-1, "type");
        // 缓动类型
        eTweenType type = (eTweenType)luaState.ToInteger(-1);
        luaState.Pop(1);
        // delay and duration
        int delay, duration;
        luaState.GetField(-1, "delay");
        delay = luaState.ToInteger(-1);
        luaState.GetField(-2, "duration");
        duration = luaState.ToInteger(-1);
        luaState.Pop(2);
        switch (type)
        {
            case eTweenType.Alhpa:
                {
                    TweenAlpha tween = TweenManager.GetInstance().Create<TweenAlpha>();
                    float begin, end;
                    InterpolationMode mode;
                    luaState.GetField(-1, "beginValue");
                    begin = (float)luaState.ToNumber(-1);
                    luaState.GetField(-2, "endValue");
                    end = (float)luaState.ToNumber(-1);
                    luaState.GetField(-3, "mode");
                    mode = (InterpolationMode)luaState.ToInteger(-1);
                    luaState.Pop(3);
                    tween.SetParas(_charCGGo, delay, duration, ePlayMode.Once);
                    tween.SetParas(begin, end, mode);
                    return tween;
                }
            case eTweenType.Color:
                {
                    TweenColor tween = TweenManager.GetInstance().Create<TweenColor>();
                    Color begin, end;
                    InterpolationMode mode;
                    luaState.GetField(-1, "beginValue");
                    begin = InterpreterManager.GetInstance().TranslateTableToColor(luaState);
                    luaState.GetField(-2, "endValue");
                    end = InterpreterManager.GetInstance().TranslateTableToColor(luaState);
                    luaState.GetField(-3, "mode");
                    mode = (InterpolationMode)luaState.ToInteger(-1);
                    luaState.Pop(3);
                    tween.SetParas(_charCGGo, delay, duration, ePlayMode.Once);
                    tween.SetParas(begin, end, mode);
                    return tween;
                }
            case eTweenType.Pos2D:
                {
                    TweenPos2D tween = TweenManager.GetInstance().Create<TweenPos2D>();
                    Vector2 begin, end;
                    InterpolationMode mode;
                    luaState.GetField(-1, "beginValue");
                    begin = InterpreterManager.GetInstance().TranslateTableToVector2(luaState);
                    luaState.GetField(-2, "endValue");
                    end = InterpreterManager.GetInstance().TranslateTableToVector2(luaState);
                    luaState.GetField(-3, "mode");
                    mode = (InterpolationMode)luaState.ToInteger(-1);
                    luaState.Pop(3);
                    tween.SetParas(_charCGGo, delay, duration, ePlayMode.Once);
                    tween.SetParas(begin, end, mode);
                    return tween;
                }
            case eTweenType.Pos3D:
                {
                    TweenPos3D tween = TweenManager.GetInstance().Create<TweenPos3D>();
                    Vector3 begin, end;
                    InterpolationMode mode;
                    luaState.GetField(-1, "beginValue");
                    begin = InterpreterManager.GetInstance().TranslateTableToVector3(luaState);
                    luaState.GetField(-2, "endValue");
                    end = InterpreterManager.GetInstance().TranslateTableToVector3(luaState);
                    luaState.GetField(-3, "mode");
                    mode = (InterpolationMode)luaState.ToInteger(-1);
                    luaState.Pop(3);
                    tween.SetParas(_charCGGo, delay, duration, ePlayMode.Once);
                    tween.SetParas(begin, end, mode);
                    return tween;
                }
            case eTweenType.Rotation:
                {
                    TweenRotation tween = TweenManager.GetInstance().Create<TweenRotation>();
                    Vector3 begin, end;
                    InterpolationMode mode;
                    luaState.GetField(-1, "beginValue");
                    begin = InterpreterManager.GetInstance().TranslateTableToVector3(luaState);
                    luaState.GetField(-2, "endValue");
                    end = InterpreterManager.GetInstance().TranslateTableToVector3(luaState);
                    luaState.GetField(-3, "mode");
                    mode = (InterpolationMode)luaState.ToInteger(-1);
                    luaState.Pop(3);
                    tween.SetParas(_charCGGo, delay, duration, ePlayMode.Once);
                    tween.SetParas(begin, end, mode);
                    return tween;
                }
            case eTweenType.Scale:
                {
                    TweenScale tween = TweenManager.GetInstance().Create<TweenScale>();
                    Vector3 begin, end;
                    InterpolationMode mode;
                    luaState.GetField(-1, "beginValue");
                    begin = InterpreterManager.GetInstance().TranslateTableToVector3(luaState);
                    luaState.GetField(-2, "endValue");
                    end = InterpreterManager.GetInstance().TranslateTableToVector3(luaState);
                    luaState.GetField(-3, "mode");
                    mode = (InterpolationMode)luaState.ToInteger(-1);
                    luaState.Pop(3);
                    tween.SetParas(_charCGGo, delay, duration, ePlayMode.Once);
                    tween.SetParas(begin, end, mode);
                    return tween;
                }
        }
        Logger.LogError("Create Tween by table fail! eTweenType not match!");
        return null;
    }

    private void OnPlayCGFinish(GameObject go)
    {
        Reset();
    }

    private void Reset()
    {
        _charCGImg.sprite = null;
        _charCGGo.SetActive(false);
        _isPlayingCharCG = false;
        _charCGGo.transform.localPosition = new Vector3(2000, 2000, 0);
    }

    public override LayerId GetLayerId()
    {
        return LayerId.STGBottomView;
    }
}