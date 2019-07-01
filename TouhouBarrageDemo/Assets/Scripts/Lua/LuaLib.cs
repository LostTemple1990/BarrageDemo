using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniLua;

public partial class LuaLib
{
    public static int Init(ILuaState luaState)
    {
        var define = new NameFuncPair[]
        {
            new NameFuncPair("SetBulletOrderInLayer",SetBulletOrderInLayer),
            // BulletBase
            new NameFuncPair("GetBulletId",GetBulletId),
            new NameFuncPair("GetBulletPara",GetBulletPara),
            new NameFuncPair("SetBulletPara",SetBulletPara),
            new NameFuncPair("GetBulletPos",GetBulletPos),
            new NameFuncPair("SetBulletPos",SetBulletPos),
            new NameFuncPair("SetBulletColor",SetBulletColor),
            new NameFuncPair("SetBulletColorWithAlpha",SetBulletColorWithAlpha),
            new NameFuncPair("SetBulletStyleById",SetBulletStyleById),
            new NameFuncPair("EliminateBullet",EliminateBullet),
            new NameFuncPair("SetBulletAlpha",SetBulletAlpha),
            new NameFuncPair("SetBulletDetectCollision",SetBulletDetectCollision),
            new NameFuncPair("SetBulletResistEliminatedFlag",SetBulletResistEliminatedFlag),
            new NameFuncPair("AddBulletTask",AddBulletTask),
            new NameFuncPair("GetBulletCollisionDetectParas",GetBulletCollisionDetectParas),
            // SimpleBullet
            new NameFuncPair("CreateSimpleBulletById", CreateSimpleBulletById),
            new NameFuncPair("CreateCustomizedBullet",CreateCustomizedBullet),
            new NameFuncPair("SetBulletSelfRotation",SetBulletSelfRotation),
            new NameFuncPair("SetBulletScale",SetBulletScale),
            new NameFuncPair("BulletDoScale",BulletDoScale),
            new NameFuncPair("SetBulletAppearEffectAvailable", SetBulletAppearEffectAvailable),
            new NameFuncPair("SetBulletStraightParas",SetBulletStraightParas),
            new NameFuncPair("SetBulletCurvePara",SetBulletCurvePara),
            new NameFuncPair("DoBulletAcceleration",DoBulletAcceleration),
            new NameFuncPair("DoBulletAccelerationWithLimitation",DoBulletAccelerationWithLimitation),

            new NameFuncPair("CreateNormalEnemyById",CreateNormalEnemyById),
            new NameFuncPair("CreateCustomizedEnemy",CreateCustomizedEnemy),
            new NameFuncPair("HitEnemy",HitEnemy),
            new NameFuncPair("SetEnemyResistEliminateFlag",SetEnemyResistEliminateFlag),
            new NameFuncPair("EliminateEnemy",EliminateEnemy),
            new NameFuncPair("RawEliminateEnemy",RawEliminateEnemy),
            new NameFuncPair("SetEnemyWanderRange",SetEnemyWanderRange),
            new NameFuncPair("SetEnemyWanderAmplitude",SetEnemyWanderAmplitude),
            new NameFuncPair("SetEnemyWanderMode",SetEnemyWanderMode),
            new NameFuncPair("EnemyDoWander",EnemyDoWander),
            new NameFuncPair("SetEnemyMaxHp",SetEnemyMaxHp),
            // Laser
            new NameFuncPair("CreateLaser",CreateLaser),
            new NameFuncPair("CreateCustomizedLaser",CreateCustomizedLaser),
            new NameFuncPair("SetLaserCollisionFactor",SetLaserCollisionFactor),
            new NameFuncPair("SetLaserProps",SetLaserProps),
            new NameFuncPair("SetLaserPos",SetLaserPos),
            new NameFuncPair("SetLaserAngle",SetLaserAngle),
            new NameFuncPair("SetLaserExistDuration",SetLaserExistDuration),
            new NameFuncPair("SetLaserSize",SetLaserSize),
            new NameFuncPair("SetLaserRotatePara",SetLaserRotatePara),
            new NameFuncPair("SetLaserRotateParaWithOmega",SetLaserRotateParaWithOmega),
            new NameFuncPair("ChangeLaserWidthTo",ChangeLaserWidthTo),
            new NameFuncPair("ChangeLaserLengthTo",ChangeLaserLengthTo),
            new NameFuncPair("ChangeLaserAlphaTo",ChangeLaserAlphaTo),
            new NameFuncPair("SetLaserGrazeDetectParas",SetLaserGrazeDetectParas),
            new NameFuncPair("SetLaserCollisionDetectParas",SetLaserCollisionDetectParas),
            // LinearLaser
            new NameFuncPair("CreateLinearLaser",CreateLinearLaser),
            new NameFuncPair("SetLinearLaserLength",SetLinearLaserLength),
            new NameFuncPair("LinearLaserDoStraightMove",LinearLaserDoStraightMove),
            new NameFuncPair("SetLinearLaserHeadEnable",SetLinearLaserHeadEnable),
            new NameFuncPair("SetLinearLaserSourceEnable",SetLinearLaserSourceEnable),
            new NameFuncPair("CreateCustomizedLinearLaser",CreateCustomizedLinearLaser),

            // 曲线激光
            new NameFuncPair("CreateCurveLaser",CreateCurveLaser),
            new NameFuncPair("SetCurveLaserStraightParas",SetCurveLaserStraightParas),
            new NameFuncPair("DoCurveLaserAccelerationWithLimitation",DoCurveLaserAccelerationWithLimitation),
            new NameFuncPair("SetCurveLaserCurveParas",SetCurveLaserCurveParas),
            new NameFuncPair("CreateCustomizedCurveLaser",CreateCustomizedCurveLaser),
            new NameFuncPair("SetCurveLaserLength",SetCurveLaserLength),
            new NameFuncPair("SetCurveLaserWidth",SetCurveLaserWidth),
            // 敌机
            new NameFuncPair("AddEnemyTask",AddEnemyTask),
            new NameFuncPair("EnemyMoveTowards",EnemyMoveTowards),
            new NameFuncPair("EnemyAccMoveTowards",EnemyAccMoveTowards),
            new NameFuncPair("EnemyAccMoveTowardsWithLimitation",EnemyAccMoveTowardsWithLimitation),
            new NameFuncPair("EnemyMoveToPos",EnemyMoveToPos),
            new NameFuncPair("GetEnemyPos", GetEnemyPos),
            new NameFuncPair("SetEnemyPos", SetEnemyPos),
            new NameFuncPair("PlaySound", PlaySound),
            new NameFuncPair("SetEnemyDropItems",SetEnemyDropItems),
            new NameFuncPair("DropItems",DropItems),
            new NameFuncPair("SetEnemyInteractive",SetEnemyInteractive),
            // Boss相关
            new NameFuncPair("CreateBoss",CreateBoss),
            new NameFuncPair("SetBossPos",SetBossPos),
            new NameFuncPair("SetBossAni",SetBossAni),
            new NameFuncPair("PlayBossAni",PlayBossAni),
            new NameFuncPair("SetEnemyCollisionParas",SetEnemyCollisionParas),
            new NameFuncPair("SetBossCurPhaseData",SetBossCurPhaseData),
            new NameFuncPair("GetBossSpellCardHpRate",GetBossSpellCardHpRate),
            new NameFuncPair("GetSpellCardTimeLeftRate",GetSpellCardTimeLeftRate),
            new NameFuncPair("SetBossInvincible",SetBossInvincible),
            new NameFuncPair("ShowBossBloodBar",ShowBossBloodBar),

            new NameFuncPair("StartSpellCard",StartSpellCard),
            new NameFuncPair("WaitForSpellCardFinish",WaitForSpellCardFinish),
            new NameFuncPair("SetSpellCardProperties",SetSpellCardProperties),
            // 特效
            new NameFuncPair("SetEffectToPos",SetEffectToPos),
            new NameFuncPair("SetEffectFinish",SetEffectFinish),
            new NameFuncPair("CreateSpriteEffect",CreateSpriteEffect),
            new NameFuncPair("CreateSpriteEffectWithProps",CreateSpriteEffectWithProps),
            new NameFuncPair("SetSpriteEffectSize",SetSpriteEffectSize),
            new NameFuncPair("SetSpriteEffectScale",SetSpriteEffectScale),
            new NameFuncPair("SetSpriteEffectColor",SetSpriteEffectColor),
            new NameFuncPair("SpriteEffectScaleWidth",SpriteEffectChangeWidthTo),
            new NameFuncPair("SpriteEffectScaleHeight",SpriteEffectChangeHeightTo),
            new NameFuncPair("ShakeEffectDoShake",ShakeEffectDoShake),
            new NameFuncPair("ShakeEffectDoShakeWithLimitation",ShakeEffectDoShakeWithLimitation),
            new NameFuncPair("CreateChargeEffect",CreateChargeEffect),
            // 子弹组件
            new NameFuncPair("AddBulletComponent",AddBulletComponent),
            new NameFuncPair("AddBulletParaChangeEvent",AddBulletParaChangeEvent),
            new NameFuncPair("AddBulletColliderTriggerEvent",AddBulletColliderTriggerEvent),
            // ObjectCollider
            new NameFuncPair("CreateObjectColliderByType",CreateObjectColliderByType),
            new NameFuncPair("SetObjectColliderTag",SetObjectColliderTag),
            new NameFuncPair("SetObjectColliderToPos",SetObjectColliderToPos),
            new NameFuncPair("SetObjectColliderSize",SetObjectColliderSize),
            new NameFuncPair("SetObjectColliderColliderGroup",SetObjectColliderColliderGroup),
            new NameFuncPair("SetObjectColliderExistDuration",SetObjectColliderExistDuration),
            new NameFuncPair("ObjectColliderScaleToSize",ObjectColliderScaleToSize),
            new NameFuncPair("ObjectColliderClearSelf",ObjectColliderClearSelf),
            new NameFuncPair("SetObjectColliderEliminateType",SetObjectColliderEliminateType),
            new NameFuncPair("SetObjectColliderHitEnemyDamage",SetObjectColliderHitEnemyDamage),
            new NameFuncPair("RemoveObjectColliderByTag",RemoveObjectColliderByTag),

            new NameFuncPair("CreateGravitationFieldByType",CreateGravitationFieldByType),
            new NameFuncPair("InitGravitationField",InitGravitationField),
            new NameFuncPair("RemoveGravitationFieldByTag",RemoveGravitationFieldByTag),
            // 清理子弹
            new NameFuncPair("ClearEnemyBulletsInRange",ClearEnemyBulletsInRange),
            new NameFuncPair("ClearAllEnemyBullets",ClearAllEnemyBullets),
            // 背景相关
            new NameFuncPair("CreateBgSpriteObject",CreateBgSpriteObject),
            new NameFuncPair("SetBgSprteObjectPos",SetBgSprteObjectPos),
            new NameFuncPair("SetBgSpriteObjectScale",SetBgSpriteObjectScale),
            new NameFuncPair("SetBgSpriteObjectVelocity",SetBgSpriteObjectVelocity),
            new NameFuncPair("SetBgSpriteObjectSelfRotateAngle",SetBgSpriteObjectSelfRotateAngle),
            new NameFuncPair("SetBgSpriteObjectRotation",SetBgSpriteObjectRotation),
            new NameFuncPair("BgSpriteObjectDoFade",BgSpriteObjectDoFade),
            new NameFuncPair("SetBgSpriteObjectAcce",SetBgSpriteObjectAcce),
            new NameFuncPair("SetBgSpriteObjectAcceWithLimitation",SetBgSpriteObjectAcceWithLimitation),
            // 绑定附加组件
            new NameFuncPair("AttatchToMaster",AttachToMaster),
            new NameFuncPair("SetAttachmentRelativePos",SetAttachmentRelativePos),
            //Player相关
            new NameFuncPair("GetPlayerPos",GetPlayerPos),
            new NameFuncPair("SetPlayerPos",SetPlayerPos),
            new NameFuncPair("GetPlayerIsMovable",GetPlayerIsMovable),
            new NameFuncPair("SetPlayerIsMovable",SetPlayerIsMovable),
            new NameFuncPair("SetStageIsEnableToShoot",SetStageIsEnableToShoot),
            // ISTGMovable
            new NameFuncPair("MoveTo",STGMovableMoveTo),
            new NameFuncPair("MoveTowards",STGMovableMoveTowards),
            new NameFuncPair("DoStraightMove",STGMovableDoStraightMove),
            new NameFuncPair("DoAcceleration",STGMovableDoAcceleration),
            new NameFuncPair("DoAccelerationWithLimitation",STGMovableDoAccelerationWithLimitation),
            new NameFuncPair("DoCurvedMove",STGMovableDoCurvedMove),
            // 通用类相关
            new NameFuncPair("GetRandomInt", GetRandomInt),
            new NameFuncPair("GetRandomFloat", GetRandomFloat),
            new NameFuncPair("GetPosAfterRotate", GetPosAfterRotate),
            new NameFuncPair("GetAngleToPlayer", GetAngleToPlayer),
            new NameFuncPair("GetAimToPlayerAngle", GetAimToPlayerAngle),
            new NameFuncPair("LogLuaNumber", LogLuaNumber),
            new NameFuncPair("GetVectorLength",GetVectorLength),
            new NameFuncPair("SetGlobalVector2", SetGlobalVector2),
            new NameFuncPair("GetGlobalVector2", GetGlobalVector2),
            new NameFuncPair("SetGlobalFloat",SetGlobalFloat),
            new NameFuncPair("GetGlobalFloat", GetGlobalFloat),
            new NameFuncPair("SetGlobalUserData", SetGlobalUserData),
            new NameFuncPair("GetGlobalUserData",GetGlobalUserData),
            new NameFuncPair("RemoveGlobalUserData", RemoveGlobalUserData),
            new NameFuncPair("PlayCharacterCG", PlayCharacterCG),
            new NameFuncPair("LogFrameSinceStageStart", LogFrameSinceStageStart),
        };
        luaState.L_NewLib(define);
        return 1;
    }

    public static int InitGlobal(ILuaState luaState)
    {
        var define = new NameFuncPair[]
        {
            // IPosition
            new NameFuncPair("GetPos",IPosition_GetPosition),
            new NameFuncPair("SetPos",IPosition_SetPosition),
            new NameFuncPair("Angle",IPosition_Angle),
            // Math
            new NameFuncPair("sin",Math_Sin),
            new NameFuncPair("cos",Math_Cos),
            new NameFuncPair("tan",Math_Tan),
            new NameFuncPair("asin",Math_ASin),
            new NameFuncPair("acos",Math_ACos),
            new NameFuncPair("atan",Math_ATan),
            new NameFuncPair("int",Math_Int),
            new NameFuncPair("abs",Math_Abs),
            new NameFuncPair("sign",Math_Sign),
            new NameFuncPair("RandomInt",GetRandomInt),
            new NameFuncPair("RandomFloat",GetRandomFloat),
            new NameFuncPair("RandomSign",GetRandomSign),
            new NameFuncPair("Wait",Wait),
            // Bullet
            new NameFuncPair("CreateSimpleBulletById", CreateSimpleBulletById),
            new NameFuncPair("CreateCustomizedBullet",CreateCustomizedBullet),
            // Enemy
            new NameFuncPair("CreateNormalEnemyById",CreateNormalEnemyById),
            new NameFuncPair("CreateCustomizedEnemy",CreateCustomizedEnemy),
            // Boss
            new NameFuncPair("CreateBoss",CreateBoss),
            // SpellCard
            new NameFuncPair("SetSpellCardProperties",SetSpellCardProperties),
            new NameFuncPair("StartSpellCard",StartSpellCard),
            new NameFuncPair("WaitForSpellCardFinish",WaitForSpellCardFinish),
        };
        luaState.PushGlobalTable();
        luaState.L_SetFuncs(define, 0);
        return 1;
    }

    private static EnemySimpleBulletLuaInterface _enemySimpleBulletLuaInterface;

    public static void RegisterLightUserDataLuaInterface()
    {
        UniLua.Utl.RegisterGetLightUserDataPropValueFunctionDelegate(GetLightUserDataField);
        UniLua.Utl.RegisterSetLightUserDataPropValueFunctionDelegate(SetLightUserDataField);

        EnemySimpleBulletLuaInterface.Init();
        NormalEnemyLuaInterface.Init();
        BossLuaInterface.Init();
    }

    public static bool GetLightUserDataField(object userData, TValue key, out TValue res)
    {
        switch (userData.GetType().Name)
        {
            case "EnemySimpleBullet":
                return EnemySimpleBulletLuaInterface.Get(userData, key, out res);
            case "NormalEnemy":
                return NormalEnemyLuaInterface.Get(userData, key, out res);
            case "Boss":
                return BossLuaInterface.Get(userData, key, out res);
        }
        res = new TValue();
        res.SetSValue(string.Format("GetField from userData fail!Invalid userData of type {0}", userData.GetType().Name));
        return false;
    }

    public static bool SetLightUserDataField(object userData, TValue key, ref TValue value)
    {
        switch (userData.GetType().Name)
        {
            case "EnemySimpleBullet":
                return EnemySimpleBulletLuaInterface.Set(userData, key, ref value);
            case "NormalEnemy":
                return NormalEnemyLuaInterface.Set(userData, key, ref value);
            case "Boss":
                return NormalEnemyLuaInterface.Set(userData, key, ref value);
        }
        value.SetSValue(string.Format("SetField of userData fail!Invalid userData of type {0}", userData.GetType().Name));
        return false;
    }

    /// <summary>
    /// 获取参数的个数
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    private static int GetNumParams(ILuaState luaState)
    {
        int top = luaState.GetTop();
        int paraCount = 0;
        int index = -1;
        while (paraCount < top && !luaState.IsFunction(index))
        {
            index--;
            paraCount++;
        }
        return paraCount;
    }

    /// <summary>
    /// 创建一颗简单的子弹
    /// <para>id 配置里面的id</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// 
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSimpleSectorBarrage(ILuaState luaState)
    {

        return 0;
    }

    #region 直线激光相关

    public static int CreateLaser(ILuaState luaState)
    {
        string id = luaState.ToString(-7);
        float posX = (float)luaState.ToNumber(-6);
        float posY = (float)luaState.ToNumber(-5);
        float angle = (float)luaState.ToNumber(-4);
        float width = (float)luaState.ToNumber(-3);
        float height = (float)luaState.ToNumber(-2);
        int existDuration = luaState.ToInteger(-1);
        luaState.Pop(7);
        EnemyLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_Laser) as EnemyLaser;
        laser.SetStyleById(id);
        laser.SetPosition(posX, posY);
        laser.SetRotation(angle);
        laser.SetLaserSize(width, height);
        laser.SetLaserExistDuration(existDuration);
        luaState.PushLightUserData(laser);
        return 1;
    }

    /// <summary>
    /// 创建自定义的直线激光
    /// <para>customizedName 自定义直线激光类名</para>
    /// <para>...自定义参数</para>
    /// <para>numArgs 参数个数</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedLaser(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        // 弹出参数个数
        luaState.Pop(1);
        string customizedName = luaState.ToString(-1 - numArgs);
        EnemyLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletType.Enemy_Laser) as EnemyLaser;
        // 设置自定义的数据
        BCCustomizedTask bc = laser.AddComponent<BCCustomizedTask>();
        int funcRef = InterpreterManager.GetInstance().GetInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        if (!luaState.IsFunction(-1))
        {
            Logger.Log("InitFuncRef of " + customizedName + " is not point to a function");
        }
        luaState.Insert(-1 - numArgs);
        luaState.PushLightUserData(laser);
        luaState.Insert(-1 - numArgs);
        luaState.Call(numArgs + 1, 0);
        // 将laser压入栈
        luaState.PushLightUserData(laser);
        return 1;
    }

    public static int SetLaserCollisionFactor(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-2) as EnemyLaser;
        float factor = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        laser.SetCollisionFactor(factor);
        return 0;
    }

    /// <summary>
    /// 更改laser的宽度
    /// <para>toWidth为实际宽度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ChangeLaserWidthTo(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-4) as EnemyLaser;
        float toWidth = (float)luaState.ToNumber(-3);
        int changeDelay = luaState.ToInteger(-2);
        int changeDuration = luaState.ToInteger(-1);
        luaState.Pop(4);
        laser.ChangeToWidth(toWidth, changeDelay, changeDuration);
        return 0;
    }

    /// <summary>
    /// 更改laser的高度长度
    /// <para>toHeight为实际长度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ChangeLaserLengthTo(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-4) as EnemyLaser;
        float toLength = (float)luaState.ToNumber(-3);
        int changeDelay = luaState.ToInteger(-2);
        int changeDuration = luaState.ToInteger(-1);
        luaState.Pop(4);
        laser.ChangeToLength(toLength, changeDelay, changeDuration);
        return 0;
    }

    /// <summary>
    /// 更改laser的alpha
    /// <para>laser</para>
    /// <para>toAlpha 变更到的alpha</para>
    /// <para>delay延迟执行的时间</para>
    /// <para>duration</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ChangeLaserAlphaTo(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-4) as EnemyLaser;
        float toAlpha = (float)luaState.ToNumber(-3);
        int changeDelay = luaState.ToInteger(-2);
        int changeDuration = luaState.ToInteger(-1);
        luaState.Pop(4);
        laser.ChangeToAlpha(toAlpha, changeDelay, changeDuration);
        return 0;
    }

    /// <summary>
    /// 设置激光的属性
    /// <para>laser 激光本体</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// <para>angle 激光的角度，x正半轴为0,逆时针增加角度</para>
    /// <para>length 激光长度</para>
    /// <para>width 激光宽度</para>
    /// <para>existDuration 存活时间 todo:随时砍掉这个</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetLaserProps(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-7) as EnemyLaser;
        float posX = (float)luaState.ToNumber(-6);
        float posY = (float)luaState.ToNumber(-5);
        float angle = (float)luaState.ToNumber(-4);
        float length = (float)luaState.ToNumber(-3);
        float width = (float)luaState.ToNumber(-2);
        int existDuration = luaState.ToInteger(-1);
        luaState.Pop(7);
        laser.SetPosition(posX, posY);
        laser.SetRotation(angle);
        laser.SetLaserSize(length, width);
        laser.SetLaserExistDuration(existDuration);
        luaState.PushLightUserData(laser);
        return 1;
    }

    public static int SetLaserPos(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-3) as EnemyLaser;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        laser.SetPosition(posX, posY);
        return 0;
    }

    public static int SetLaserAngle(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-2) as EnemyLaser;
        float angle = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        laser.SetRotation(angle);
        return 0;
    }

    public static int SetLaserExistDuration(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-2) as EnemyLaser;
        int existDuration = luaState.ToInteger(-1);
        luaState.Pop(2);
        laser.SetLaserExistDuration(existDuration);
        return 0;
    }

    public static int SetLaserSize(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-3) as EnemyLaser;
        float length = (float)luaState.ToNumber(-2);
        float width = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        laser.SetLaserSize(length, width);
        return 0;
    }

    public static int SetLaserRotatePara(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-3) as EnemyLaser;
        float toAngle = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(3);
        laser.DoRotate(toAngle, duration);
        return 0;
    }

    public static int SetLaserRotateParaWithOmega(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-3) as EnemyLaser;
        float omega = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(3);
        laser.DoRotateWithOmega(omega, duration);
        return 0;
    }

    public static int SetLaserGrazeDetectParas(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-3) as EnemyLaser;
        GrazeDetectParas detectParas = new GrazeDetectParas()
        {
            type = GrazeDetectType.Rect,
            halfWidth = (float)luaState.ToNumber(-2),
            halfHeight = (float)luaState.ToNumber(-1),
        };
        luaState.Pop(3);
        laser.SetGrazeDetectParas(detectParas);
        return 0;
    }

    public static int SetLaserCollisionDetectParas(ILuaState luaState)
    {
        Logger.LogError("Try to call unused Method LuaLib:SetLaserCollisionDetectParas");
        EnemyLaser laser = luaState.ToUserData(-3) as EnemyLaser;
        CollisionDetectParas detectParas = new CollisionDetectParas()
        {
            type = CollisionDetectType.Rect,
            halfWidth = (float)luaState.ToNumber(-2),
            halfHeight = (float)luaState.ToNumber(-1),
        };
        luaState.Pop(3);
        //laser.SetCollisionDetectParas(detectParas);
        return 0;
    }

    #endregion


    #region 创建敌机相关

    //public static int EnemyMoveTo(ILuaState luaState)
    //{
    //    NormalEnemy enemy = luaState.ToUserData(-4) as NormalEnemy;
    //    float endPosX = (float)luaState.ToNumber(-3);
    //    float endPosY = (float)luaState.ToNumber(-2);
    //    int duration = luaState.ToInteger(-1);
    //    luaState.Pop(4);
    //    //enemy.DoMove()
    //    return 0;
    //}
    #endregion

    #region BOSS相关
    /// <summary>
    /// 创建boss
    /// <para>string typeName boss的自定义类型名称</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateBoss(ILuaState luaState)
    {
        string bossName = luaState.ToString(-1);
        luaState.Pop(1);
        Boss boss = EnemyManager.GetInstance().CreateEnemyByType(EnemyType.Boss) as Boss;
        boss.Init(bossName);
        luaState.PushLightUserData(boss);
        return 1;
    }

    /// <summary> 设置boss的位置
    /// <para>boss</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBossPos(ILuaState luaState)
    {
        Boss boss = luaState.ToUserData(-3) as Boss;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        boss.SetPosition(new Vector3(posX, posY,0));
        return 0;
    }

    /// <summary> 设置boss的动画
    /// <para>boss</para>
    /// <para>aniId</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBossAni(ILuaState luaState)
    {
        Boss boss = luaState.ToUserData(-2) as Boss;
        string aniId;
        if ( luaState.Type(-1) == LuaType.LUA_TNUMBER)
        {
            aniId = luaState.ToNumber(-1).ToString();
        }
        else
        {
            aniId = luaState.ToString(-1);
        }
        luaState.Pop(2);
        boss.SetAni(aniId);
        return 0;
    }

    /// <summary> 播放BOSS动画
    /// <para>boss</para>
    /// <para>at Enum AniAcitonType</para>
    /// <para>dir 动作方向</para>
    /// <para>duration 持续时间，单位 帧</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int PlayBossAni(ILuaState luaState)
    {
        Boss boss = luaState.ToUserData(-4) as Boss;
        AniActionType at = (AniActionType)luaState.ToInteger(-3);
        int dir = luaState.ToInteger(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        boss.PlayAni(at, dir, duration);
        return 0;
    }

    public static int SetEnemyCollisionParas(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-3) as EnemyBase;
        float hw = (float)luaState.ToNumber(-2);
        float hh = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        enemy.SetCollisionParams(hw, hh);
        return 0;
    }

    /// <summary>
    /// 设置BOSS无敌持续时间
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBossInvincible(ILuaState luaState)
    {
        Boss boss = luaState.ToUserData(-2) as Boss;
        float duration = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        boss.SetInvincible(duration);
        return 0;
    }

    /// <summary>
    /// 设置BOSS是否显示血条
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int ShowBossBloodBar(ILuaState luaState)
    {
        Boss boss = luaState.ToUserData(-2) as Boss;
        bool isShow = luaState.ToBoolean(-1);
        luaState.Pop(2);
        boss.ShowBloodBar(isShow);
        return 0;
    }

    /// <summary>
    /// 符卡开始
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int StartSpellCard(ILuaState luaState)
    {
        int bossCount = 0;
        List<Boss> bossList = new List<Boss>();
        Boss boss;
        // 获取符卡对应的table
        int index = -1;
        while ( !luaState.IsTable(index) )
        {
            boss = luaState.ToUserData(-1) as Boss;
            bossList.Add(boss);
            bossCount++;
            index--;
        }
        // 获取Init函数
        luaState.GetField(index, "Init");
        if (!luaState.IsFunction(-1))
        {
            Logger.Log("field \"Init\" of spell card is not a function!");
            luaState.Pop(1);
            return 0;
        }
        int initFuncRef = InterpreterManager.GetInstance().RefLuaFunction(luaState);
        // 获取OnFinish函数
        luaState.GetField(index, "OnFinish");
        int finishFuncRef = 0;
        if (!luaState.IsFunction(-1))
        {
            luaState.Pop(1);
        }
        else
        {
            finishFuncRef = InterpreterManager.GetInstance().RefLuaFunction(luaState);
        }
        STGStageManager.GetInstance().StartSpellCard(initFuncRef, finishFuncRef, bossList);
        return 0;
    }

    /// <summary>
    /// 关卡主线程挂起等待符卡结束
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int WaitForSpellCardFinish(ILuaState luaState)
    {
        STGStageManager.GetInstance().SetStageTaskWaitingForSpellCardFinish(true);
        luaState.PushInteger(1);
        luaState.Yield(1);
        return 0;
    }

    /// <summary>
    /// 设置符卡的基本属性
    /// <para>name 符卡名称</para>
    /// <para>duration 单位：秒，符卡持续时间</para>
    /// <para>conditon 符卡的条件</para>
    /// <para> finishFunc 可以为nil，符卡结束时候的函数</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetSpellCardProperties(ILuaState luaState)
    {
        SpellCard sc = STGStageManager.GetInstance().GetSpellCard();
        string scName = luaState.ToString(-4);
        float duration = (float)luaState.ToNumber(-3);
        int condition = luaState.ToInteger(-2);
        bool isSpellCard = luaState.ToBoolean(-1);
        sc.SetProperties(scName, duration, (eSpellCardCondition)condition,isSpellCard);
        return 0;
    }

    /// <summary>
    /// 设置boss的当前血条的阶段
    /// <para>boss</para>
    /// <para>....  分段权重</para>
    /// <para>isMultiPhase 是否为多个阶段</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBossCurPhaseData(ILuaState luaState)
    {
        bool isMultiPhase = luaState.ToBoolean(-1);
        int count = 0;
        int index = -2;
        List<float> weights = new List<float>();
        // 取到boss参数的位置，以确定传入的血条阶段的个数
        while ( luaState.Type(index) != LuaType.LUA_TLIGHTUSERDATA )
        {
            index--;
            count++;
        }
        Boss boss = luaState.ToUserData(index) as Boss;
        for (index=index+1;index<-1;index++)
        {
            weights.Add((float)luaState.ToNumber(index));
        }
        boss.SetCurPhaseData(weights, isMultiPhase);
        luaState.Pop(count + 2);
        return 0;
    }
    #endregion

    public static int ClearEnemyBulletsInRange(ILuaState luaState)
    {
        float centerX = (float)luaState.ToNumber(-3);
        float centerY = (float)luaState.ToNumber(-2);
        float radius = (float)luaState.ToNumber(-1);
        BulletsManager.GetInstance().ClearEnemyBulletsInRange(new Vector2(centerX, centerY), radius);
        return 0;
    }

    public static int ClearAllEnemyBullets(ILuaState luaState)
    {
        BulletsManager.GetInstance().ClearAllEnemyBullets();
        return 0;
    }

    #region 音效相关
    /// <summary>
    /// <para>soundName</para>
    /// <para>isLoop</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int PlaySound(ILuaState luaState)
    {
        string soundName = luaState.ToString(-2);
        bool isLoop = luaState.ToBoolean(-1);
        SoundManager.GetInstance().Play(soundName, isLoop);
        return 0;
    }
    #endregion

    #region 随机数
    public static int GetRandomInt(ILuaState luaState)
    {
        int begin = luaState.ToInteger(-2);
        int end = luaState.ToInteger(-1);
        luaState.PushInteger(MTRandom.GetNextInt(begin,end));
        return 1;
    }

    public static int GetRandomFloat(ILuaState luaState)
    {
        float begin = (float)luaState.ToNumber(-2);
        float end = (float)luaState.ToNumber(-1);
        luaState.PushNumber(MTRandom.GetNextFloat(begin, end));
        return 1;
    }

    /// <summary>
    /// 随机返回-1或者1
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetRandomSign(ILuaState luaState)
    {
        int ret = MTRandom.GetNextInt(0, 1);
        luaState.PushNumber(ret == 0 ? -1 : 1);
        return 1;
    }
    #endregion

    /// <summary>
    /// 获取绕中心点顺时针旋转angle度之后的坐标
    /// <para>curX</para>
    /// <para>curY</para>
    /// <para>centerX</para>
    /// <para>centerY</para>
    /// <para>rotateAngle</para>
    /// </summary>
    /// <param name="ILuaState"></param>
    /// <returns></returns>
    public static int GetPosAfterRotate(ILuaState luaState)
    {
        float curX = (float)luaState.ToNumber(-5);
        float curY = (float)luaState.ToNumber(-4);
        float centerX = (float)luaState.ToNumber(-3);
        float centerY = (float)luaState.ToNumber(-2);
        float rotateAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        Vector2 vec = MathUtil.GetVec2AfterRotate(curX, curY, centerX, centerY, rotateAngle);
        luaState.PushNumber(vec.x);
        luaState.PushNumber(vec.y);
        return 2;
    }

    /// <summary>
    /// 指定位置指向自机的向量的角度
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetAimToPlayerAngle(ILuaState luaState)
    {
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        float angle = MathUtil.GetAngleBetweenXAxis(Global.PlayerPos.x - posX, Global.PlayerPos.y - posY);
        luaState.PushNumber(angle);
        return 1;
    }

    public static int LogLuaNumber(ILuaState luaState)
    {
        int count = luaState.ToInteger(-1);
        string str = "";
        for (int index = -1-count;index<-1;index++)
        {
            str += luaState.ToNumber(index) + " , ";
        }
        luaState.Pop(count + 1);
        Logger.Log(str);
        return 0;
    }

    public static int GetVectorLength(ILuaState luaState)
    {
        float len;
        float beginX = (float)luaState.ToNumber(-4);
        float beginY = (float)luaState.ToNumber(-3);
        float endX = (float)luaState.ToNumber(-2);
        float endY = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        len = Mathf.Sqrt((beginX - endX) * (beginX - endX) + (beginY - endY) * (beginY - endY));
        luaState.PushNumber(len);
        return 1;
    }

    public static int Wait(ILuaState luaState)
    {
        return luaState.Yield(luaState.GetTop());
    }

    #region lua全局变量相关
    public static int SetGlobalVector2(ILuaState luaState)
    {
        string key = luaState.ToString(-3);
        float x = (float)luaState.ToNumber(-2);
        float y = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        InterpreterManager.GetInstance().SetGlobalVector2(key, new Vector2(x, y));
        return 0;
    }

    public static int GetGlobalVector2(ILuaState luaState)
    {
        string key = luaState.ToString(-1);
        luaState.Pop(1);
        Vector2 vec = InterpreterManager.GetInstance().GetGlobalVector2(key);
        luaState.PushNumber(vec.x);
        luaState.PushNumber(vec.y);
        return 2;
    }

    public static int SetGlobalFloat(ILuaState luaState)
    {
        string key = luaState.ToString(-2);
        float number = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        InterpreterManager.GetInstance().SetGlobalNumber(key, number);
        return 0;
    }

    public static int GetGlobalFloat(ILuaState luaState)
    {
        string key = luaState.ToString(-1);
        luaState.Pop(1);
        float num = InterpreterManager.GetInstance().GetGlobalNumber(key);
        luaState.PushNumber(num);
        return 1;
    }

    public static int SetGlobalUserData(ILuaState luaState)
    {
        string key = luaState.ToString(-2);
        object userData = luaState.ToUserData(-1);
        luaState.Pop(2);
        InterpreterManager.GetInstance().SetGlobalUserData(key, userData);
        return 0;
    }

    public static int GetGlobalUserData(ILuaState luaState)
    {
        string key = luaState.ToString(-1);
        luaState.Pop(1);
        object userData = InterpreterManager.GetInstance().GetGlobalUserData(key);
        luaState.PushLightUserData(userData);
        return 1;
    }

    public static int RemoveGlobalUserData(ILuaState luaState)
    {
        string key = luaState.ToString(-1);
        luaState.Pop(1);
        InterpreterManager.GetInstance().RemoveGlobalUserData(key);
        return 0;
    }
    #endregion
}
