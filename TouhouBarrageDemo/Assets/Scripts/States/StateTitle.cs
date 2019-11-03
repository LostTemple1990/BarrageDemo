using System;
using UnityEngine;
using System.Collections.Generic;

public class StateTitle : IState, ICommand
{
    private int _selDifficulty;
    /// <summary>
    /// 选择的角色索引
    /// </summary>
    private int _selCharacterIndex;

    public StateTitle()
    {
        
    }

    private GameStateMachine _fsm;

    private STGMain _stgMain;

    public void Execute(int cmd, object data)
    {
        switch (cmd)
        {
            case CommandConsts.SelectDifficulty:
                
                break;
            case CommandConsts.SelectCharacter:
                OnCharacterSelected((int)data);
                break;
        }
    }

    public int GetStateId()
    {
        return (int)eGameState.Title;
    }

    public void OnInit(IFSM fsm)
    {
        _fsm = fsm as GameStateMachine;
    }

    public void OnStateEnter(object data = null)
    {
        // 添加监听
        CommandManager.GetInstance().Register(CommandConsts.SelectDifficulty, this);
        CommandManager.GetInstance().Register(CommandConsts.SelectCharacter, this);
        // 打开起始界面
        UIManager.GetInstance().ShowView(WindowName.MainView);
    }

    public void OnStateExit()
    {
        CommandManager.GetInstance().Remove(CommandConsts.SelectDifficulty, this);
        CommandManager.GetInstance().Remove(CommandConsts.SelectCharacter, this);

        UIManager.GetInstance().HideView(WindowName.MainView);
        UIManager.GetInstance().HideView(WindowName.SelectCharView);
    }

    public void OnUpdate()
    {
        
    }

    private void OnCharacterSelected(int characterIndex)
    {
        _selCharacterIndex = characterIndex;
        STGData data = new STGData();
        data.stageName = "Stage1";
        data.characterIndex = _selCharacterIndex;
        _fsm.SetNextStateId((int)eGameState.STG, data);
    }
}