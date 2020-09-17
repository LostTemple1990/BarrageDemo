using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// unused 
public interface ILuaWrapperObject
{
    LuaWrapperObjectType GetWrapperObjectType();
}

public enum LuaWrapperObjectType
{
    ReimuA,
    MarisaA,
    NormalEnemy,
    Boss,
    EnemySimpleBullet,
    EnemyLaser,
    EnemyLinearLaser,
    EnemyCurveLaser,
    SpriteObject,
    ColliderCircle,
    ColliderRect,
    ColliderItalicRect,
}
