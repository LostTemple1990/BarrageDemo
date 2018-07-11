using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MTRandom
{
    private const int N = 624;
    private static long[] mt;
    private static int index;
    private static bool isInit = false;

    public MTRandom()
    {
        //this._mt = new long[N];
        //this._index = 0;
    }
     
    public static void Init(long seed=19650218)
    {
        mt = new long[N];
        index = 0;
        mt[0] = seed;
        for (int i=1;i<N;i++)
        {
            mt[i] = 0xffffffff & (0x6c078965 * (mt[i - 1] ^ (mt[i - 1] >> 30)) + i);
        }
        isInit = true;
    }

    private static void NextState()
    {
        for (int i=0;i<N;i++)
        {
            long y = (mt[i] & 0x80000000) + (mt[(i + 1) % N] & 0x7fffffff);
            mt[i] = mt[(i + 397) % N] ^ (y >> 1);
            if (y % 2 != 0)
            {
                mt[i] ^= 0x9908b0df;
            }
        }
    }

    private static long GetNext()
    {
        if ( index == 0 )
        {
            NextState();
        }
        long y = mt[index];
        y ^= (y >> 11);
        y ^= ((y << 7) & 0x9d2c5680);
        y ^= ((y << 15) & 0xefc60000);
        y ^= (y >> 18);
        index = (index + 1) % N;
        return y;
    }

    /// <summary>
    /// 获取min到max之间的随机整数N，[min,max]
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int GetNextInt(int min,int max)
    {
        return (int)(GetNext() % (max - min + 1)) + min;
    }

    /// <summary>
    /// 获取min到max之间的随机浮点数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float GetNextFloat(float min,float max)
    {
        return (float)GetNext() / 0xffffffff * (max - min) + min;
    }
}

