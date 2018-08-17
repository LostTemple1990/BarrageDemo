using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        lastTick = System.DateTime.Now.Ticks;
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
        curTick = System.DateTime.Now.Ticks;
        curCount++;
        sum += (curTick - lastTick);
        if ( curCount >= totalCount )
        {
            countFlag = 0;
            Logger.Log("SampleTime : " + sum*0.1f/totalCount);
            sum = 0;
        }
    }
}
