using UnityEngine;
using System.Collections.Generic;

public class GameMainView : ViewBase,ICommand
{
    private GameObject _viewPanel;
    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        RectTransform rectTf = _viewTf as RectTransform;
        // 设置宽高
        rectTf.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    public void Execute(int cmd,object[] args)
    {

    }
}
