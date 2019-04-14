public class Timer : IPoolClass
{
    /// <summary>
    /// timer标识的字符串
    /// </summary>
    public string timerStr;
    /// <summary>
    /// 延迟delay秒后才开始执行timer
    /// </summary>
    public int delay;
    /// <summary>
    /// 执行的次数
    /// </summary>
    public int executeCount;
    /// <summary>
    /// 执行的总次数
    /// </summary>
    public int executeTotalCount;
    /// <summary>
    /// 执行的间隔统计
    /// </summary>
    public int intervalCount;
    /// <summary>
    /// 执行的间隔
    /// </summary>
    public int interval;
    /// <summary>
    /// 执行的回调
    /// </summary>
    public TimerManager.TimerCallBack timerCallBack;
    public bool isComplete;
    /// <summary>
    /// 是否在执行完之后自动销毁
    /// </summary>
    public bool isAutoDestroyOnComplete;
    /// <summary>
    /// 执行结束的回调
    /// </summary>
    public TimerManager.TimerCompleteCallBack completeCallBack;

    public void Clear()
    {
        timerCallBack = null;
        completeCallBack = null;
    }
}