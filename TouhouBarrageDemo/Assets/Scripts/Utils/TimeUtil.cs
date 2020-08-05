using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TimeUtil
{
    private static long lastTick;
    private static long curTick;
    private static int curCount;
    private static int totalCount;
    private static long sum;
    private static int countFlag = 0;

    public static void BeginSample(int count=1)
    {
        lastTick = Stopwatch.GetTimestamp();
        if ( countFlag == 0 )
        {
            curCount = 0;
            totalCount = count;
            countFlag = 1;
            sum = 0;
        }
    }

    public static void EndSample()
    {
        curTick = Stopwatch.GetTimestamp();
        curCount++;
        sum += (curTick - lastTick);
        if ( curCount >= totalCount )
        {
            countFlag = 0;
            Logger.Log("SampleTime : " + sum*0.1d/totalCount + "us");
            sum = 0;
        }
    }

    /// <summary>
    /// 结束采样
    /// <para>传入参数使用string.format中的格式，要包含{0}</para>
    /// </summary>
    /// <param name="formatStr"></param>
    public static void EndSample(string formatStr)
    {
        curTick = Stopwatch.GetTimestamp();
        curCount++;
        sum += (curTick - lastTick);
        if (curCount >= totalCount)
        {
            countFlag = 0;
            double time = sum * 0.0001d / totalCount;
            Logger.Log(string.Format(formatStr, time + "ms"));
            sum = 0;
        }
    }

    public static void LogNowTime()
    {
        long current = Stopwatch.GetTimestamp();
        System.DateTime now = new System.DateTime(current);
        Logger.Log("NowTime = " + now.Minute + ":" + now.Second + ":" + now.Millisecond);
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
