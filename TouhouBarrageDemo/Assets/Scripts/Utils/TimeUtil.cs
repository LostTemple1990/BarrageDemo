using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TimeUtil
{
    private static TimeUtil instance = new TimeUtil();

    private Dictionary<string, SampleData> _dic;

    class SampleData
    {
        public long startTick;
        public long totalTick;

        public SampleData()
        {
            startTick = 0;
            totalTick = 0;
        }
    }

    private TimeUtil()
    {
        _dic = new Dictionary<string, SampleData>();
    }

    private void TimerStart(string key,bool resetTimer = true)
    {
        SampleData data;
        if (!_dic.TryGetValue(key, out data))
        {
            data = new SampleData();
            _dic[key] = data;
        }
        if (resetTimer)
            data.totalTick = 0;
        data.startTick = Stopwatch.GetTimestamp();
    }

    private void TimerEnd(string key)
    {
        long endTick = Stopwatch.GetTimestamp();
        var data = _dic[key];
        data.totalTick += endTick - data.startTick;
    }

    private long GetTotalTick(string key)
    {
        SampleData data;
        if (!_dic.TryGetValue(key, out data))
        {
            return 0;
        }
        return data.totalTick;
    }


    public static void BeginSample(string key)
    {
        instance.TimerStart(key);
    }

    public static void EndSample(string key)
    {
        instance.TimerEnd(key);
    }

    public static void Reset()
    {
        foreach(var data in instance._dic.Values)
        {
            data.totalTick = 0;
            data.startTick = 0;
        }
    }

    public static void Reset(string key)
    {
        SampleData data;
        if (instance._dic.TryGetValue(key, out data))
        {
            data.totalTick = 0;
            data.startTick = 0;
        }
    }

    /// <summary>
    /// 获取采样的时间
    /// 单位ms
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static double GetSampleTick(string key)
    {
        return instance.GetTotalTick(key) * 0.0001d;
    }

    /// <summary>
    /// 输出采样时间
    /// </summary>
    /// <param name="key"></param>
    /// <param name="formatStr"></param>
    public static void LogSampleTick(string key,string formatStr)
    {
        double time = instance.GetTotalTick(key) * 0.0001d;
        Logger.Log(string.Format(formatStr, time + "ms"));
    }

    public static void LogNowTime()
    {
        long current = Stopwatch.GetTimestamp();
        System.DateTime now = new System.DateTime(current);
        Logger.Log(string.Format("NowTime = {0}:{1}:{2}", now.Minute, now.Second, now.Millisecond));
    }

    /// <summary>
    /// 获取当前时间戳
    /// </summary>
    /// <returns></returns>
    public static long GetTimestamp()
    {
        return Stopwatch.GetTimestamp();
    }
}
