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

    public const float HighSpeed = 4.5f;
    public const float SlowSpeed = 2f;

    public const int CharID_Reimu = 1;
    public const int CharID_Marisa = 2;

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

    public const int BulletId_ReimuA_Main = 110;
    public const int BulletId_ReimuA_Sub1 = 111;
    public const int BulletId_ReimuA_Sub2 = 112;

    public const int PlayerHalfWidth = 16;
    public const int PlayerHalfHeight = 24;

    public const int TargetFrameRate = 60;

    public const float AutoGetItemY = 140;
    public const float ItemBottomBorderY = -240;
    public const float ItemTopBorderY = GameHeight / 2;
    /// <summary>
    /// 最大信号强度
    /// </summary>
    public const float MaxSignalValue = 130f;

    public const int ReboundLeft = 0x01;
    public const int ReboundRight = 0x02;
    public const int ReboundTop = 0x04;
    public const int ReboundBottom = 0x08;

    /// <summary>
    /// 最大持续时间
    /// </summary>
    public const int MaxDuration = 99999;
    /// <summary>
    /// 当前速度对应的角度
    /// </summary>
    public const float VelocityAngle = -999;
    /// <summary>
    /// 原本的旋转角度
    /// </summary>
    public const float OriginalRotation = -9999.5f;
    /// <summary>
    /// 最大速度限制
    /// </summary>
    public const float MaxVelocity = 9999f;
    public const float OriginalWidth = -1;
    public const float OriginalHeight = -1;

    /// <summary>
    /// 松开射击键之后的持续射击时间
    /// <para>单位：帧</para>
    /// </summary>
    public const int MaxShootDurationAfterKeyUp = 20;

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
    /// 初始残机数目
    /// </summary>
    public const int STGInitLifeCount = 5;
    /// <summary>
    /// 初始符卡数目
    /// </summary>
    public const int STGInitSpellCardCount = 3;


    public const string EffectAtlasName = "STGEffectAtlas";
    public const string ReimuAtlasName = "STGReimuAtlas";
    public const string MarisaAtlasName = "STGMarisaAtlas";
    public const string STGMainViewAtlasName = "STGMainViewAtlas";
    public const string STGCommonAtlasName = "STGCommonAtlas";
    public const string STGBulletsAtlasName = "STGBulletsAtlas";
    public const string STGLaserAtlasName = "STGLaserAtlas";

    /// <summary>
    /// 每帧最多销毁的item对象的个数
    /// </summary>
    public const int MaxDestroyItemCountPerFrame = 10;
    public const int MaxDestroyPrefabCountPerFrame = 10;
    public const int MaxDestroyClassCountPerFrame = 2;
    /// <summary>
    /// 不执行对象回收的系统繁忙最小值
    /// </summary>
    public const int SysBusyValue = 200;

    public const int STGBottomViewLayerPosZ = 22;
    public const int STGEffectLayerPosZ = 20;
    public const int STGItemLayerPosZ = 19;
    public const int STGEnemyLayerPosZ = 18;
    public const int STGHighLightEffectLayerPosZ = 17;
    public const int STGPlayerBarrageLayerPosZ = 16;
    public const int STGPlayerLayerPosZ = 15;
    public const int STGEnemyBarrageLayerPosZ = 14;
    public const int STGPlayerCollisionPointLayerPosZ = 13;
    public const int STGTopEffectLayerPosZ = 12;
    public const int STGInfoLayerPosZ = 10;

    /// <summary>
    /// 默认UI音效的音量大小
    /// </summary>
    public const float DefaultUISEVolume = 0.25f;
}
