using System.Collections.Generic;

public class STGStageManager
{
    private const int StateGetStageTask = 1;
    private const int StateUpdateStageTask = 2;
    private const int StateUpdateSpellCard = 3;

    private static STGStageManager _instance = new STGStageManager();

    public static STGStageManager GetInstance()
    {
        return _instance;
    }

    private int _state;

    private Task _curStageTask;
    private SpellCard _curSpellCard;
    /// <summary>
    /// 是否正在符卡阶段
    /// </summary>
    private bool _isCastingSpellCard;
    /// <summary>
    /// 是否等待符卡结束
    /// </summary>
    private bool _isWaitingForSpellCard;

    public STGStageManager()
    {
        _curSpellCard = new SpellCard();
    }

    public void Init()
    {
        _state = StateGetStageTask;
    }

    public void Update()
    {
        if ( _state == StateUpdateStageTask)
        {
            if ( !_isWaitingForSpellCard )
            {
                OnStageTaskUpdate();
            }
            if (_isCastingSpellCard)
            {
                if ( !_curSpellCard.IsComplete() )
                {
                    _curSpellCard.Update();
                }
                else
                {
                    _isWaitingForSpellCard = false;
                }
            }
        }

    }

    /// <summary>
    /// 加载关卡
    /// </summary>
    /// <param name="stageId"></param>
    public void LoadStage(int stageId)
    {
        _curStageTask = InterpreterManager.GetInstance().LoadStage(stageId);
        _state = StateUpdateStageTask;
    }

    /// <summary>
    /// 符卡开始
    /// </summary>
    /// <param name="scFuncRef"></param>
    /// <param name="bossList"></param>
    public void StartSpellCard(int scFuncRef,List<Boss> bossList)
    {
        _curSpellCard.SetTask(scFuncRef, bossList);
        _isCastingSpellCard = true;
    }

    /// <summary>
    /// 设置符卡结束的清除相关函数
    /// </summary>
    /// <param name="funcRef"></param>
    public void SetSpellCardFinishFunc(int funcRef)
    {
        _curSpellCard.SetFinishFuncRef(funcRef);
    }

    /// <summary>
    /// 获取当前符卡
    /// </summary>
    /// <returns></returns>
    public SpellCard GetSpellCard()
    {
        return _curSpellCard;
    }

    /// <summary>
    /// 设置关卡task等待符卡结束再执行
    /// </summary>
    /// <param name="value"></param>
    public void SetStageTaskWaitingForSpellCardFinish(bool value)
    {
        _isWaitingForSpellCard = value;
    }

    private void OnStageTaskUpdate()
    {
        InterpreterManager.GetInstance().CallTaskCoroutine(_curStageTask);
    }

    public void Clear()
    {
        _curSpellCard.Clear();
        InterpreterManager.GetInstance().StopTaskThread(_curStageTask);
    }
}