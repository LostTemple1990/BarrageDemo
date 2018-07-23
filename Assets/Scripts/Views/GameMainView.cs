using UnityEngine;
using System.Collections.Generic;

public class GameMainView : ViewBase,ICommand
{
    private GameObject _viewPanel;
    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
    }

    public void Execute(int cmd,object[] args)
    {

    }
}
