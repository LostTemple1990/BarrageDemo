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
    /// <summary>
    /// 从关卡开始经过的帧数
    /// </summary>
    private int _frameSinceStageStart;

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
            _frameSinceStageStart++;
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
                    _isCastingSpellCard = false;
                    _isWaitingForSpellCard = false;
                    CommandManager.GetInstance().RunCommand(CommandConsts.SpellCardFinish);
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
        _frameSinceStageStart = 0;
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

    /// <summary>
    /// 执行关卡task
    /// </summary>
    private void OnStageTaskUpdate()
    {
        InterpreterManager.GetInstance().CallTaskCoroutine(_curStageTask);
    }

    /// <summary>
    /// 获取关卡开始之后经过的帧数
    /// </summary>
    /// <returns></returns>
    public int GetFrameSinceStageStart()
    {
        return _frameSinceStageStart;
    }

    public void Clear()
    {
        _curSpellCard.Clear();
        _isCastingSpellCard = false;
        _isWaitingForSpellCard = false;
        InterpreterManager.GetInstance().StopTaskThread(_curStageTask);
    }
}