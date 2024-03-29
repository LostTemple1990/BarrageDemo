﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public const float kEpsilonNormalSqrt = 1e-15f;

    public static float GetAngleBetweenXAxis(Vector2 vec)
    {
        float denominator = vec.magnitude;
        if (denominator < kEpsilonNormalSqrt) return 0f;
        float dot = vec.x / denominator;
        float radius = Mathf.Acos(dot);
        if (vec.y < 0) radius = -radius;
        return radius * Mathf.Rad2Deg;
    }

    public static float GetAngleBetweenXAxis(float x,float y)
    {
        float denominator = Mathf.Sqrt(x*x+y*y);
        if (denominator < kEpsilonNormalSqrt) return 0f;
        float dot = x / denominator;
        float radius = Mathf.Acos(dot);
        if (y < 0) radius = -radius;
        return radius * Mathf.Rad2Deg;
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
        // 线段起始坐标与结束坐标相同，即为同一个点，则直接计算距离
        if (pointA == pointB) return Vector2.Distance(pointA, pointP);
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

    public static bool DetectCollisionBetweenCircleAndAABB(Vector2 circlePos, float radius, Vector2 aabbPos, float halfWidth, float halfHeight)
    {
        // 计算圆与aabb是否碰撞
        float dw = Mathf.Max(0, Mathf.Abs(circlePos.x - aabbPos.x) - halfWidth);
        float dh = Mathf.Max(0, Mathf.Abs(circlePos.y - aabbPos.y) - halfHeight);
        return radius * radius >= dw * dw + dh * dh;
    }

    /// <summary>
    /// 检测两矩形是否碰撞
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="a0">矩形半宽</param>
    /// <param name="b0">矩形半高</param>
    /// <param name="p1"></param>
    /// <param name="a1"></param>
    /// <param name="b1"></param>
    /// <returns></returns>
    public static bool DetectCollisionBetweenAABBAndAABB(Vector2 p0,float a0,float b0,Vector2 p1,float a1,float b1)
    {
        Vector2 dp = p0 - p1;
        return Mathf.Abs(dp.x) <= a0 + a1 && Mathf.Abs(dp.y) <= b0 + b1;
    }

    /// <summary>
    /// 检测圆和斜向矩形的碰撞
    /// </summary>
    /// <param name="circlePos"></param>
    /// <param name="radius"></param>
    /// <param name="obbPos"></param>
    /// <param name="halfWidth"></param>
    /// <param name="halfHeight"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    public static bool DetectCollisionBetweenCircleAndOBB(Vector2 circlePos,float radius,Vector2 obbPos,float halfWidth,float halfHeight,float rot)
    {
        // 将圆心顺时针旋转rot角度，将obb转换成aabb
        float sin = Mathf.Sin(rot * Mathf.Deg2Rad);
        float cos = Mathf.Cos(rot * Mathf.Deg2Rad);
        Vector2 obbToCircleVec = circlePos - obbPos;
        //Vector2 relVec = new Vector2(cos * obbToCircleVec.x + sin * obbToCircleVec.y, -sin * obbToCircleVec.x + cos * obbToCircleVec.y);
        // 计算圆与aabb是否碰撞
        float dw = Mathf.Max(0, Mathf.Abs(cos * obbToCircleVec.x + sin * obbToCircleVec.y) - halfWidth);
        float dh = Mathf.Max(0, Mathf.Abs(-sin * obbToCircleVec.x + cos * obbToCircleVec.y) - halfHeight);
        return radius * radius >= dw * dw + dh * dh;
    }

    /// <summary>
    /// 检测obb和obb的碰撞
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="a0"></param>
    /// <param name="b0"></param>
    /// <param name="rot0"></param>
    /// <param name="p1"></param>
    /// <param name="a1"></param>
    /// <param name="b1"></param>
    /// <param name="rot1"></param>
    /// <returns></returns>
    public static bool DetectCollisionBetweenOBBAndOBB(Vector2 p0,float a0,float b0,float rot0,Vector2 p1,float a1,float b1,float rot1)
    {
        float cos0 = Mathf.Cos(Mathf.Deg2Rad * rot0);
        float sin0 = Mathf.Sin(Mathf.Deg2Rad * rot0);
        float cos1 = Mathf.Cos(Mathf.Deg2Rad * rot1);
        float sin1 = Mathf.Sin(Mathf.Deg2Rad * rot1);
        //rect0分离轴
        Vector2 rect0Vec0 = new Vector2(cos0, sin0);
        Vector2 rect0Vec1 = new Vector2(-sin0, cos0);
        // rect1分离轴
        Vector2 rect1Vec0 = new Vector2(cos1, sin1);
        Vector2 rect1Vec1 = new Vector2(-sin1, cos1);
        List<Vector2> rectVecList = new List<Vector2> { rect0Vec0, rect0Vec1, rect1Vec0, rect1Vec1 };
        // 两矩形中心的向量
        Vector2 centerVec = new Vector2(p1.x - p0.x, p1.y - p0.y);
        bool rectIsCollided = true;
        for (int i = 0; i < rectVecList.Count; i++)
        {
            // 投影轴
            Vector2 vec = rectVecList[i];
            // rect0的投影半径对于该投影轴的投影
            float projectionRadius0 = Mathf.Abs(Vector2.Dot(rect0Vec0, vec) * a0) + Mathf.Abs(Vector2.Dot(rect0Vec1, vec) * b0);
            projectionRadius0 = Mathf.Abs(projectionRadius0);
            // rect1的投影半径对于投影轴的投影
            float projectionRadius1 = Mathf.Abs(Vector2.Dot(rect1Vec0, vec) * a1) + Mathf.Abs(Vector2.Dot(rect1Vec1, vec) * b1);
            projectionRadius1 = Mathf.Abs(projectionRadius1);
            // 连线对于投影轴的投影
            float centerVecProjection = Vector2.Dot(centerVec, vec);
            centerVecProjection = Mathf.Abs(centerVecProjection);
            // 投影的和小于轴半径的长度,说明没有碰撞
            if (projectionRadius0 + projectionRadius1 <= centerVecProjection)
            {
                rectIsCollided = false;
                break;
            }
        }
        return rectIsCollided;
    }

    /// <summary>
    /// 获取绕centerX,centerY点逆时针旋转angle之后的点
    /// <para>逆时针的旋转矩阵是</para>
    /// <para>[cos -sin]</para>
    /// <para>[sin  cos]</para>
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

    /// <summary>
    /// 将指定角度转换成0~360之间
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float ClampAngle(float angle)
    {
        angle %= 360f;
        return angle < 0 ? angle + 360f : angle;
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

    public static Vector3 GetEaseInQuadInterpolation(Vector3 start, Vector3 end, float time, float duration)
    {
        float posX, posY, posZ;
        posX = start.x + (end.x - start.x) * (time /= duration) * time;
        posY = start.y + (end.y - start.y) * time * time;
        posZ = start.z + (end.z - start.z) * time * time;
        return new Vector3(posX, posY, posZ);
    }

    public static Vector2 GetEaseInQuadInterpolation(Vector2 start, Vector2 end, float time, float duration)
    {
        float posX, posY;
        posX = start.x + (end.x - start.x) * (time /= duration) * time;
        posY = start.y + (end.y - start.y) * time * time;
        return new Vector2(posX, posY);
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

    public static Vector3 GetEaseOutQuadInterpolation(Vector3 start, Vector3 end, float time, float duration)
    {
        float posX, posY, posZ;
        posX = start.x + (start.x - end.x) * (time /= duration) * (time - 2);
        posY = start.y + (start.y - end.y) * time * (time - 2);
        posZ = start.z + (start.z - end.z) * time * (time - 2);
        return new Vector3(posX, posY, posZ);
    }

    public static Vector2 GetEaseOutQuadInterpolation(Vector2 start, Vector2 end, float time, float duration)
    {
        float posX, posY;
        posX = start.x + (start.x - end.x) * (time /= duration) * (time - 2);
        posY = start.y + (start.y - end.y) * time * (time - 2);
        return new Vector2(posX, posY);
    }

    /// <summary>
    /// 二次函数的缓动，先加速再减速
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static float GetEaseInOutQuadInterpolation(float start, float end, float time, float duration)
    {
        if ( (time /= duration/2) < 1 )
        {
            return start + (end - start) / 2 * time * time;
        }
        return start + (start - end) / 2 * ((--time) * (time - 2) - 1);
    }

    public static Vector3 GetEaseInOutQuadInterpolation(Vector3 start, Vector3 end, float time, float duration)
    {
        float posX, posY, posZ;
        if ((time /= duration / 2) < 1)
        {
            posX = start.x + (end.x - start.x) / 2 * time * time;
            posY = start.y + (end.y - start.y) / 2 * time * time;
            posZ = start.z + (end.z - start.z) / 2 * time * time;
        }
        else
        {
            posX = start.x + (start.x - end.x) / 2 * ((--time) * (time - 2) - 1);
            posY = start.y + (start.y - end.y) / 2 * (time * (time - 2) - 1);
            posZ = start.z + (start.z - end.z) / 2 * (time * (time - 2) - 1);
        }
        return new Vector3(posX, posY, posZ);
    }

    public static Vector2 GetEaseInOutQuadInterpolation(Vector2 start, Vector2 end, float time, float duration)
    {
        float posX, posY;
        if ((time /= duration / 2) < 1)
        {
            posX = start.x + (end.x - start.x) / 2 * time * time;
            posY = start.y + (end.y - start.y) / 2 * time * time;
        }
        else
        {
            posX = start.x + (start.x - end.x) / 2 * ((--time) * (time - 2) - 1);
            posY = start.y + (start.y - end.y) / 2 * (time * (time - 2) - 1);
        }
        return new Vector2(posX, posY);
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

    public static Vector3 GetLinearInterpolation(Vector3 start, Vector3 end, float time, float duration)
    {
        float posX, posY, posZ;
        posX = start.x + (end.x - start.x) * time / duration;
        posY = start.y + (end.y - start.y) * time / duration;
        posZ = start.z + (end.z - start.z) * time / duration;
        return new Vector3(posX, posY, posZ);
    }

    public static Vector2 GetLinearInterpolation(Vector2 start, Vector2 end, float time, float duration)
    {
        float posX, posY;
        posX = start.x + (end.x - start.x) * time / duration;
        posY = start.y + (end.y - start.y) * time / duration;
        return new Vector2(posX, posY);
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
        return start + (end - start) * Mathf.Sin(time / duration * Mathf.PI / 2);
    }

    public static Vector3 GetSinInterpolation(Vector3 start, Vector3 end, float time, float duration)
    {
        float posX, posY, posZ;
        float sin = Mathf.Sin(time / duration * Mathf.PI / 2);
        posX = start.x + (end.x - start.x) * sin;
        posY = start.y + (end.y - start.y) * sin;
        posZ = start.z + (end.z - start.z) * sin;
        return new Vector3(posX, posY, posZ);
    }

    public static Vector2 GetSinInterpolation(Vector2 start, Vector2 end, float time, float duration)
    {
        float posX, posY;
        float sin = Mathf.Sin(time / duration * Mathf.PI / 2);
        posX = start.x + (end.x - start.x) * sin;
        posY = start.y + (end.y - start.y) * sin;
        return new Vector2(posX, posY);
    }

    /// <summary>
    /// 余弦插值
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public static float GetCosInterpolation(float start,float end,float time,float duration)
    {
        return end + (start - end) * Mathf.Cos(time / duration * Mathf.PI / 2);
    }
    
    public static Vector2 GetCosInterpolation(Vector2 start, Vector2 end, float time, float duration)
    {
        float cos = Mathf.Cos(time / duration * Mathf.PI / 2);
        float x = end.x + (start.x - end.x) * cos;
        float y = end.y + (start.y - end.y) * cos;
        return new Vector2(x, y);
    }

    public static Vector3 GetCosInterpolation(Vector3 start, Vector3 end, float time, float duration)
    {
        float cos = Mathf.Cos(time / duration * Mathf.PI / 2);
        float x = end.x + (start.x - end.x) * cos;
        float y = end.y + (start.y - end.y) * cos;
        float z = end.y + (start.z - end.z) * cos;
        return new Vector3(x, y, z);
    }

    public static float GetNoneInterpolation(float start, float end, float time, float duration)
    {
        return time < duration ? start : end;
    }

    public static Vector3 GetNoneInterpolation(Vector3 start, Vector3 end, float time, float duration)
    {
        return time < duration ? start : end;
    }

    public static Vector2 GetNoneInterpolation(Vector2 start, Vector2 end, float time, float duration)
    {
        return time < duration ? start : end;
    }

    #endregion

#region 获取插值的函数
    public delegate float InterpolationFloatFunc(float start, float end, float time, float duration);
    public delegate Vector2 InterpolationVec2Func(Vector2 start, Vector2 end, float time, float duration);
    public delegate Vector3 InterpolationVec3Func(Vector3 start, Vector3 end, float time, float duration);

    public static InterpolationFloatFunc GetInterpolationFloatFunc(InterpolationMode mode)
    {
        switch (mode)
        {
            case InterpolationMode.None:
                return GetNoneInterpolation;
            case InterpolationMode.Linear:
                return GetLinearInterpolation;
            case InterpolationMode.EaseInQuad:
                return GetEaseInQuadInterpolation;
            case InterpolationMode.EaseOutQuad:
                return GetEaseOutQuadInterpolation;
            case InterpolationMode.EaseInOutQuad:
                return GetEaseInOutQuadInterpolation;
            case InterpolationMode.Sin:
                return GetSinInterpolation;
            case InterpolationMode.Cos:
                return GetCosInterpolation;
        }
        return null;
    }

    public static InterpolationVec2Func GetInterpolationVec2Func(InterpolationMode mode)
    {
        switch (mode)
        {
            case InterpolationMode.None:
                return GetNoneInterpolation;
            case InterpolationMode.Linear:
                return GetLinearInterpolation;
            case InterpolationMode.EaseInQuad:
                return GetEaseInQuadInterpolation;
            case InterpolationMode.EaseOutQuad:
                return GetEaseOutQuadInterpolation;
            case InterpolationMode.EaseInOutQuad:
                return GetEaseInOutQuadInterpolation;
            case InterpolationMode.Sin:
                return GetSinInterpolation;
            case InterpolationMode.Cos:
                return GetCosInterpolation;
        }
        return null;
    }

    public static InterpolationVec3Func GetInterpolationVec3Func(InterpolationMode mode)
    {
        switch (mode)
        {
            case InterpolationMode.None:
                return GetNoneInterpolation;
            case InterpolationMode.Linear:
                return GetLinearInterpolation;
            case InterpolationMode.EaseInQuad:
                return GetEaseInQuadInterpolation;
            case InterpolationMode.EaseOutQuad:
                return GetEaseOutQuadInterpolation;
            case InterpolationMode.EaseInOutQuad:
                return GetEaseInOutQuadInterpolation;
            case InterpolationMode.Sin:
                return GetSinInterpolation;
            case InterpolationMode.Cos:
                return GetCosInterpolation;
        }
        return null;
    }
    #endregion
}
