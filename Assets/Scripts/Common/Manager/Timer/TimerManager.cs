using System;
using System.Collections.Generic;

public class TimerManager
{
    private static TimerManager _instance = new TimerManager();

    public static TimerManager GetInstance()
    {
        return _instance;
    }

    public delegate void TimerCallBack(int tick);
    public delegate void TimerCompleteCallBack();

    private List<Timer> _timerList;
    private int _timerCount;

    private Dictionary<string, Timer> _timerDic;

    private int _tmpIndex;

    public void Init()
    {
        if ( _timerList == null )
        {
            _timerList = new List<Timer>();
        }
        if ( _timerDic == null )
        {
            _timerDic = new Dictionary<string, Timer>();
        }
        _timerList.Clear();
        _timerCount = 0;
        _timerDic.Clear();
    }

    public string AddTimer(int delay,int interval,int executeCount,TimerCallBack timerCallback,bool autoDestroyOnComplete=true,TimerCompleteCallBack timerCompleteCallBack=null)
    {
        // 根据时间获取timerStr
        long nowTicks = System.DateTime.Now.Ticks;
        string timerString = nowTicks + "-" + _tmpIndex;
        _tmpIndex++;
        // 初始化timer
        Timer timer = ObjectsPool.GetInstance().GetPoolClassAtPool<Timer>();
        // 基础属性赋值
        timer.timerStr = timerString;
        timer.delay = delay;
        timer.interval = interval;
        timer.executeTotalCount = executeCount;
        timer.timerCallBack = timerCallback;
        timer.isAutoDestroyOnComplete = autoDestroyOnComplete;
        timer.completeCallBack = timerCompleteCallBack;
        // 初始化
        timer.intervalCount = 0;
        timer.isComplete = false;
        // 添加到执行列表中
        _timerList.Add(timer);
        _timerCount++;
        _timerDic.Add(timerString, timer);
        return timerString;
    }

    public void Update(int tick=1)
    {
        _tmpIndex = 0;
        int i;
        Timer timer;
        for (i=0;i<_timerCount;i++)
        {
            timer = _timerList[i];
            if ( timer != null && !timer.isComplete )
            {
                // 判断是否需要延迟执行
                if ( timer.delay > 0 )
                {
                    timer.delay--;
                }
                else
                {
                    timer.intervalCount++;
                    if (timer.intervalCount >= timer.interval)
                    {
                        timer.timerCallBack(tick);
                        timer.intervalCount = 0;
                        timer.executeCount++;
                        if (timer.executeCount >= timer.executeTotalCount)
                        {
                            timer.isComplete = true;
                            if (timer.isAutoDestroyOnComplete)
                            {
                                _timerList[i] = null;
                                ObjectsPool.GetInstance().RestorePoolClassToPool<Timer>(timer);
                            }
                            else
                            {
                                timer.completeCallBack();
                            }
                        }
                    }
                }
            }
        }
    }

    public void RemoveTimer(string timerStr)
    {
        int i;
        Timer timer;
        if ( _timerDic.TryGetValue(timerStr,out timer) )
        {
            _timerDic.Remove(timerStr);
            for (i=0;i<_timerCount;i++)
            {
                timer = _timerList[i];
                if ( timer != null && timer.timerStr == timerStr )
                {
                    _timerList[i] = null;
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Timer>(timer);
                    break;
                }
            }
        }
    }

    public void Clear()
    {
        int i;
        Timer timer;
        for (i=0;i<_timerCount;i++)
        {
            timer = _timerList[i];
            if ( timer != null )
            {
                ObjectsPool.GetInstance().RestorePoolClassToPool<Timer>(timer);
                _timerList[i] = null;
            }
        }
        _timerList.Clear();
        _timerDic.Clear();
        _timerCount = 0;
    }

    
}
