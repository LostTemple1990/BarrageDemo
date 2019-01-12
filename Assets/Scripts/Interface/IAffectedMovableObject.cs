using UnityEngine;
using System.Collections;

/// <summary>
/// 会被外界影响的可移动物体
/// </summary>
public interface IAffectedMovableObject : IPosition
{
    void SetExtraStraightParas(float v, float angle, float acce, float accAngle);
}