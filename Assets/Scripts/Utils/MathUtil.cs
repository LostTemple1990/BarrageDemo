using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public static float GetAngleBetweenXAxis(Vector3 vec,bool isRadius)
    {
        float length = vec.magnitude;
        if ( length == 0 )
        {
            return 0;
        }
        float rAngle = Mathf.Acos(vec.x / length);
        if ( vec.y < 0 )
        {
            rAngle = 2 * Mathf.PI - rAngle;
        }
        if ( isRadius )
        {
            return rAngle;
        }
        else
        {
            return rAngle * Mathf.Rad2Deg;
        }
    }
    public static float GetAngleBetweenXAxis(float x,float y,bool isRadius)
    {
        float length = Mathf.Sqrt(x*x+y*y);
        if (length == 0 || y == 0)
        {
            return 0;
        }
        float rAngle = Mathf.Acos(x / length);
        if (y < 0)
        {
            rAngle = 2 * Mathf.PI - rAngle;
        }
        if (isRadius)
        {
            return rAngle;
        }
        else
        {
            return rAngle * Mathf.Rad2Deg;
        }
    }

    /// <summary>
    /// 获取点到线段的最短距离
    /// </summary>
    /// <param name="pointA">线段起始坐标</param>
    /// <param name="pointB">线段终点坐标</param>
    /// <param name="pointP">点坐标</param>
    /// <returns></returns>
    public static float GetMinDisFromPointToLineSegment(Vector2 pointA,Vector2 pointB,Vector2 pointP)
    {
        Vector2 vecAtoB = pointB - pointA;
        Vector2 vecAtoP = pointP - pointA;
        float dot = vecAtoB.x * vecAtoP.x + vecAtoB.y * vecAtoP.y;
        if (dot < 0)
        {
            return vecAtoP.magnitude;
        }
        //向量AB与向量AB的点乘，值为|AB|2
        float d = vecAtoB.x * vecAtoB.x + vecAtoB.y * vecAtoB.y;
        if ( dot > d )
        {
            return Vector2.Distance(pointB, pointP);
        }
        // 点P投影在线段AB上,设投影点为C
        float r = dot / d;
        Vector2 pointC = pointA + r * vecAtoB;
        return Vector2.Distance(pointP, pointC);
    }

    /// <summary>
    /// 获取绕centerX,centerY点顺时针旋转angle之后的点
    /// </summary>
    /// <param name="curX"></param>
    /// <param name="curY"></param>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    /// <param name="rotateAngle"></param>
    /// <returns></returns>
    public static Vector2 GetVec2AfterRotate(float curX,float curY,float centerX,float centerY,float rotateAngle)
    {
        float relativeX = curX - centerX;
        float relativeY = curY - centerY;
        float cos = Mathf.Cos(rotateAngle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(rotateAngle * Mathf.Deg2Rad);
        Vector2 vec = new Vector2(centerX + cos * relativeX - sin * relativeY, centerY + sin * relativeX + cos * relativeY);
        return vec;
    }

#region 插值相关
    /// <summary>
    /// 二次函数的缓动插值 从0开始加速
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    /// <param name="duration"></param>
    /// <para> y = sqr(x)</para>
    /// <returns></returns>
    public static float GetEaseInQuadInterpolation(float start,float end,float time,float duration)
    {
        //return start + (end - start) * (time / duration) * (time / duration);
        return start + (end - start) * (time /= duration) * time;
    }

    /// <summary>
    /// 二次函数的缓动 最终减速到0
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    /// <param name="duration"></param>
    /// <para> y = -sqr(x) + 2x</para>
    /// <returns></returns>
    public static float GetEaseOutQuadInterpolation(float start, float end, float time, float duration)
    {
        //float x = time / duration;
        //float y = -x * x + 2 * x;
        //return start + (end - start) * y;
        return start +(start - end) * (time /= duration) * (time - 2);
    }

    public static float GetEaseInOutQuadInterpolation(float start, float end, float time, float duration)
    {
        if ( (time /= duration/2) < 1 )
        {
            return start + (end - start) / 2 * time * time;
        }
        return start + (start - end) / 2 * ((--time) * (time - 2) - 1);
    }

    /// <summary>
    /// 线性插值
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static float GetLinearInterpolation(float start,float end,float time,float duration)
    {
        return start + (end - start) * time / duration;
    }

    /// <summary>
    /// 正弦插值
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static float GetSinInterpolation(float start, float end, float time, float duration)
    {
        return start + (end - start) * Mathf.Sin(time / duration * Mathf.PI / 2 * Mathf.Deg2Rad);
    }

    public static float GetNoneInterpolation(float start, float end, float time, float duration)
    {
        return time < duration ? start : end;
    }
#endregion
}
