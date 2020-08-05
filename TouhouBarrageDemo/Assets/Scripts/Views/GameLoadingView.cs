using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLoadingView : ViewBase ,ICommand
{
    /// <summary>
    /// 当前需要等待的总指令数目
    /// </summary>
    private int _totalWaitCommandCount;
    /// <summary>
    /// 当前已经完成的指令数目
    /// </summary>
    private int _curFinishCommandCount;

    private List<int> _waitCommandList;

    private GameObject _bgGo;

    public GameLoadingView()
    {
        _waitCommandList = new List<int>();
    }

    public void Execute(int cmd, object data)
    {
        int index = _waitCommandList.IndexOf(cmd);
        if ( index != -1 )
        {
            _curFinishCommandCount++;
            CommandManager.GetInstance().Remove(cmd, this);
        }
    }

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        _bgGo = _viewTf.Find("Background").gameObject;
    }

    protected override void OnShow(object data=null)
    {
        object[] datas = data as object[];
        _curFinishCommandCount = 0;
        _totalWaitCommandCount = 0;
        if (datas != null )
        {
            int count = datas.Length;
            int cmd;
            for (int i=0;i<count;i++)
            {
                cmd = (int)datas[i];
                _waitCommandList.Add(cmd);
                CommandManager.GetInstance().Register(cmd, this);
            }
            _totalWaitCommandCount = count;
        }
        // 设置初始颜色
        _bgGo.GetComponent<RawImage>().color = new Color(0, 0, 0, 1);
        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    public override void Update()
    {
        if ( _curFinishCommandCount >= _totalWaitCommandCount )
        {
            UIManager.GetInstance().UnregisterViewUpdate(this);
            // 缓动
            TweenAlpha tween = TweenManager.GetInstance().Create<TweenAlpha>();
            tween.SetParas(_bgGo, 0, 20,ePlayMode.Once);
            tween.SetParas(1, 0, InterpolationMode.Linear);
            tween.SetFinishCallBack(TweenCallback);
            TweenManager.GetInstance().AddTween(tween);
        }
    }

    private void TweenCallback(GameObject go)
    {
        Hide();
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Top;
    }
}
