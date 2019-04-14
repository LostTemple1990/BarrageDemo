using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommonUtils
{
    /// <summary>
    /// 清空数组内的空元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="count">list.Count，传入该参数代表默认已知list的长度</param>
    /// <returns>返回清空之后的数组长度</returns>
    public static int RemoveNullElementsInList<T>(List<T> list,int count)
    {
        int i, j,findFlag;
        for (i=0,j=1;i<count;i++)
        {
            if ( list[i] == null )
            {
                findFlag = 0;
                j = j == 1 ? i + 1 : j;
                for (;j<count;j++)
                {
                    if ( list[j] != null )
                    {
                        findFlag = 1;
                        list[i] = list[j];
                        list[j] = default(T);
                        break;
                    }
                }
                if ( findFlag == 0 )
                {
                    break;
                }
            }
        }
        list.RemoveRange(i, count - i);
        return i;
    }

    /// <summary>
    /// 清空数组内的空元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns>返回清空之后的数组长度</returns>
    public static int RemoveNullElementsInList<T>(List<T> list)
    {
        int i, j, findFlag;
        int count = list.Count;
        for (i = 0, j = 1; i < count; i++)
        {
            if (list[i] == null)
            {
                findFlag = 0;
                j = j == 1 ? i + 1 : j;
                for (; j < count; j++)
                {
                    if (list[j] != null)
                    {
                        findFlag = 1;
                        list[i] = list[j];
                        list[j] = default(T);
                        break;
                    }
                }
                if (findFlag == 0)
                {
                    break;
                }
            }
        }
        list.RemoveRange(i, count - i);
        return i;
    }
}
