using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using UnityEngine;

public class FPSController
{
    private static FPSController _instance;

    public static FPSController GetInstance()
    {
        if (_instance == null)
            _instance = new FPSController();
        return _instance;
    }

    private const long TimeUnit = 10000000L;
    /// <summary>
    /// 最大延迟帧率容限
    /// </summary>
    private const int DelayTolerance = 10;

    /// <summary>
    /// 是否第一帧
    /// </summary>
    private bool _isFirstFrame;
    /// <summary>
    /// 上一帧的时间戳
    /// </summary>
    private long _lastFrameTicks;

    private bool _fixedFPS;

    private FPSController()
    {
        _isFirstFrame = true;
        _fixedFPS = true;
        //timeStampList = new long[10000];
    }

    private long[] timeStampList;

    public void SleepToNextFrame()
    {
        if (!_isFirstFrame)
        {
            long nextTicks = _lastFrameTicks + TimeUnit / Consts.TargetFrameRate;
            long currentTicks = Stopwatch.GetTimestamp();
            long dTicks = nextTicks - currentTicks;
            if (dTicks > 0)
            {
                int sleepTime = (int)(1000f * dTicks / TimeUnit);
                //Logger.Log("Sleep dTick " + 1000f * dTicks / TimeUnit);
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);
            }
            //Logger.Log(String.Format("PreWhile dTick {0}\nToNextTick{1}",
            //    1000f * (Stopwatch.GetTimestamp() - _lastFrameTicks) / TimeUnit,
            //    1000f * (nextTicks - Stopwatch.GetTimestamp()) / TimeUnit));
            while (nextTicks - Stopwatch.GetTimestamp() >= 10) ;
            //int count = 0;
            //while (true)
            //{
            //    long dStamp = nextTicks - Stopwatch.GetTimestamp();
            //    if (dStamp < 10)
            //        break;
            //    timeStampList[count] = dStamp;
            //    count++;
            //    if (count >= 10000)
            //    {
            //        string tmp = "";
            //        for (int i=0;i<count;i+=100)
            //        {
            //            tmp += "stamp[" + i + "] = " + timeStampList[i] + "\n";
            //        }
            //        Logger.LogWarn("while timedout...\n" + tmp);
            //        break;
            //    }
            //}
            //Logger.Log("While Count = " + count);
            //Logger.Log("Now dTick " + 1000f * (Stopwatch.GetTimestamp() - _lastFrameTicks) / TimeUnit);
            //_lastFrameTicks = Stopwatch.GetTimestamp();
            if (_fixedFPS && dTicks < -TimeUnit / (float)Consts.TargetFrameRate * DelayTolerance)
            {
                float skipFrameCount = Mathf.Ceil(-dTicks * (float)Consts.TargetFrameRate / TimeUnit - DelayTolerance);
                //Logger.Log("Skip " + skipFrameCount + " frame(s)");
                _lastFrameTicks += (long)skipFrameCount * TimeUnit / Consts.TargetFrameRate;
            }
            else
            {
                _lastFrameTicks = nextTicks;
            }
        }
        else
        {
            _isFirstFrame = false;
            _lastFrameTicks = Stopwatch.GetTimestamp();
        }
    }

    public void Restart(bool fixedFPS)
    {
        //_isFirstFrame = true;
        _lastFrameTicks = Stopwatch.GetTimestamp();
        _fixedFPS = fixedFPS;
    }
}