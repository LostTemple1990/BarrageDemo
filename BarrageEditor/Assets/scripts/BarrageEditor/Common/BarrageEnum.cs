﻿using System;

namespace BarrageEditor
{
    [Flags]
    public enum eEliminatedTypes : int
    {
        /// <summary>
        /// 强制直接删除
        /// </summary>
        ForcedDelete = 0,
        /// <summary>
        /// 玩家B触发的消除
        /// </summary>
        PlayerSpellCard = 1,
        /// <summary>
        /// 玩家死亡触发的消除
        /// </summary>
        PlayerDead = 2,
        /// <summary>
        /// 击中玩家触发的消除
        /// </summary>
        HitPlayer = 4,
        /// <summary>
        /// 玩家子弹击中触发的消除(针对敌机)
        /// </summary>
        PlayerBullet = 8,
        /// <summary>
        /// 击中某些物体触发的消除
        /// </summary>
        HitObjectCollider = 16,
        /// <summary>
        /// 引力场
        /// </summary>
        GravitationField = 32,
        /// <summary>
        /// 直接调用代码触发的消除
        /// </summary>
        CodeEliminate = 2 << 8,
        /// <summary>
        /// 直接调用代码，不触发消除触发的函数
        /// </summary>
        CodeRawEliminate = 2 << 9,
        CustomizedType0 = 2 << 10,
        CustomizedType1 = 2 << 11,
        CustomizedType2 = 2 << 12,
        CustomizedType3 = 2 << 13,
        CustomizedType4 = 2 << 14,
        CustomizedType5 = 2 << 15,
    }

    [Flags]
    public enum eColliderGroup : int
    {
        Player = 1,
        PlayerBullet = 2,
        Enemy = 4,
        EnemyBullet = 8,
        Item = 16,
        Boss = 32,
        CustomizedType0 = 2 << 10,
        CustomizedType1 = 2 << 11,
        CustomizedType2 = 2 << 12,
        CustomizedType3 = 2 << 13,
        CustomizedType4 = 2 << 14,
        CustomizedType5 = 2 << 15,
    }

    [Flags]
    public enum eReboundBorder : int
    {
        Left = 0x01,
        Right = 0x02,
        Top = 0x04,
        Bottom = 0x08,
    }

    public enum eDropItemType : byte
    {
        PowerNormal = 1,
        PowerBig = 2,
        PowerFull = 3,
        LifeFragment = 4,
        Life = 5,
        BombFragment = 6,
        Bomb = 7,
    }

    public class BarrageEnum
    {
    }
}
