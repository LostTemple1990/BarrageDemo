using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public const int DIR_NULL = 0;
    public const int DIR_LEFT = 1;
    public const int DIR_RIGHT = 2;
    public const int DIR_UP = 4;
    public const int DIR_DOWN = 8;

    public const int SpeedMove = 0;
    public const int SlowMove = 1;

    public const float HighSpeed = 4.5f;
    public const float SlowSpeed = 2f;

    public const int CharID_Reimu = 1;

    public const float RefResolutionX = 640f;
    public const float RefResolutionY = 480f;
    /// <summary>
    /// STG游戏的默认宽度
    /// </summary>
    public const float GameWidth = 384f;
    /// <summary>
    /// STG游戏的默认高度
    /// </summary>
    public const float GameHeight = 448f;

    public const int LayerId_Enemy = 1;
    public const int LayerId_PlayerBarrage = 2;
    public const int LayerId_Barrage = 5;
    public const int LayerId_Player = 10;

    public const int BulletId_ReimuA_Main = 110;
    public const int BulletId_ReimuA_Sub1 = 111;
    public const int BulletId_ReimuA_Sub2 = 112;

    public const int PlayerHalfWidth = 16;
    public const int PlayerHalfHeight = 24;

    public const int CollisionType_Rect = 1;
    public const int CollisionType_Circle = 2;

    public const int GrazeType_Rect = 1;

    public const int TargetFrameRate = 60;

    public const float AutoGetItemY = 140;
    public const float ItemBottomBorderY = -240;

    public const int ReboundLeft = 0x01;
    public const int ReboundRight = 0x02;
    public const int ReboundTop = 0x04;
    public const int ReboundBottom = 0x08;

    public const int MaxDuration = 9999;
    public const float VelocityAngle = -999;
    public const float OriginalWidth = -1;
    public const float OriginalHeight = -1;

    /// <summary>
    /// 自机出现的无敌时间
    /// </summary>
    public const int AppearInvincibleDuration = 300;
    /// <summary>
    /// 多线段集合中每次检测的组中包含的点的个数
    /// </summary>
    public const int NumInMultiSegmentsGroup = 2;

    /// <summary>
    /// 玩家初始火力
    /// </summary>
    public const int PlayerInitPower = 100;
    /// <summary>
    /// 玩家满火力
    /// </summary>
    public const int PlayerMaxPower = 400;
    /// <summary>
    /// 最大残机数
    /// </summary>
    public const int PlayerMaxLifeCount = 8;
    /// <summary>
    /// 残机碎片的最大数目
    /// </summary>
    public const int PlayerMaxLifeFragmentCount = 5;
    /// <summary>
    /// 最大可使用的符卡数
    /// </summary>
    public const int PlayerMaxSpellCardCount = 8;
    /// <summary>
    /// 符卡碎片最大数目
    /// </summary>
    public const int PlayerMaxSpellCardFragmentCount = 5;
    /// <summary>
    /// 初始残疾数目
    /// </summary>
    public const int STGInitLifeCount = 2;
    /// <summary>
    /// 初始符卡数目
    /// </summary>
    public const int STGInitSpellCardCount = 3;


    public const string EffectAtlasName = "STGEffectAtlas";
    public const string ReimuAtlasName = "STGReimuAtlas";
    public const string STGMainViewAtlasName = "STGMainViewAtlas";
    public const string STGCommonAtlasName = "STGCommonAtlas";
    public const string STGBulletsAtlasName = "STGBulletsAtlas";

    /// <summary>
    /// 每帧最多销毁的对象的个数
    /// </summary>
    public const int MaxDestroyCountPerFrame = 10;
    public const int MaxDestroyPrefabCountPerFrame = 2;
    public const int MaxDestroyClassCountPerFrame = 2;
    /// <summary>
    /// 不执行对象回收的系统繁忙最小值
    /// </summary>
    public const int SysBusyValue = 200;
}
