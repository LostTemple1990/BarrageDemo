using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PreloadView : ViewBase, ICommand
{
    private GameObject _bgGo;
    private Image _loadingImg;

    public PreloadView()
    {
        
    }

    public void Execute(int cmd, object data)
    {
        if (cmd == CommandConsts.PreloadComplete)
        {
            TweenAlpha tween = TweenManager.GetInstance().Create<TweenAlpha>();
            tween.SetParas(_bgGo, 0, 20, ePlayMode.Once);
            tween.SetParas(1, 0, InterpolationMode.Linear);
            tween.SetFinishCallBack(TweenCallback);
            TweenManager.GetInstance().AddTween(tween);
        }
    }

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        _bgGo = _viewTf.Find("Bg").gameObject;
        _loadingImg = _viewTf.Find("Bg/NowLoading").GetComponent<Image>();
    }

    protected override void OnShow(object data = null)
    {
        _bgGo.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        _loadingImg.color = new Color(1, 1, 1, 1);

        TweenAlpha tween = TweenManager.GetInstance().Create<TweenAlpha>();
        tween.SetParas(_loadingImg.gameObject, 0, 60, ePlayMode.PingPong);
        tween.SetParas(1, 0, InterpolationMode.Linear);
        TweenManager.GetInstance().AddTween(tween);

        CommandManager.GetInstance().Register(CommandConsts.PreloadComplete, this);
        //UIManager.GetInstance().RegisterViewUpdate(this);
    }

    private void TweenCallback(GameObject go)
    {
        Destroy();
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Top;
    }

    protected override void OnHide()
    {
        TweenManager.GetInstance().RemoveTweenByGo(_loadingImg.gameObject);
        base.OnHide();
    }
}
