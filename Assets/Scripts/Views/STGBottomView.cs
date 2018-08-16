using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        _isPlayingCharCG = false;
    }

    public void Execute(int cmd, object[] datas)
    {
        switch ( cmd )
        {
            case CommandConsts.PlayCharacterCGAni:
                PlayCGAni(datas);
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
        List<TweenBase> tweenList = (List<TweenBase>)datas[1];
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
        TweenManager.GetInstance().AddTweens(_charCGGo, tweenList);
    }

    private void OnPlayCGFinish(GameObject go)
    {
        _charCGImg.sprite = null;
        _charCGGo.SetActive(false);
        _isPlayingCharCG = false;
    }

    public override LayerId GetLayerId()
    {
        return LayerId.STGBottomView;
    }
}