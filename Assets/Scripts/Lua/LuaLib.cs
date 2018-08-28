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
            new NameFuncPair("CreateSimpleBullet", CreateSimpleBullet),
            new NameFuncPair("CreateSimpleBulletById", CreateSimpleBulletById),
            new NameFuncPair("SetBulletStraightParas",SetBulletStraightParas),
            new NameFuncPair("SetBulletCurvePara",SetBulletCurvePara),
            new NameFuncPair("DoBulletAcceleration",DoBulletAcceleration),
            new NameFuncPair("DoBulletAccelerationWithLimitation",DoBulletAccelerationWithLimitation),
            new NameFuncPair("ChangeBulletStyleById",ChangeBulletStyleById),
            new NameFuncPair("SetBulletOrderInLayer",SetBulletOrderInLayer),
            new NameFuncPair("EliminateBullet",EliminateBullet),
            new NameFuncPair("SetBulletToUnrealState",SetBulletToUnrealState),
            new NameFuncPair("SetBulletDetectCollision",SetBulletDetectCollision),
            new NameFuncPair("SetBulletResistEliminatedFlag",SetBulletResistEliminatedFlag),
            // 设置、获取子弹相关参数
            new NameFuncPair("GetBulletPos",GetBulletPos),
            new NameFuncPair("SetBulletPos",SetBulletPos),
            // 自定义子弹
            new NameFuncPair("CreateCustomizedBullet",CreateCustomizedBullet),
            new NameFuncPair("AddBulletTask",AddBulletTask),
            new NameFuncPair("CreateCustomizedLaser",CreateCustomizedLaser),

            new NameFuncPair("CreateNormalEnemyById",CreateNormalEnemyById),
            new NameFuncPair("CreateCustomizedEnemy",CreateCustomizedEnemy),
            new NameFuncPair("HitEnemy",HitEnemy),
            new NameFuncPair("EliminateEnemy",EliminateEnemy),
            new NameFuncPair("RawEliminateEnemy",RawEliminateEnemy),
            new NameFuncPair("SetEnemyWanderRange",SetEnemyWanderRange),
            new NameFuncPair("SetEnemyWanderAmplitude",SetEnemyWanderAmplitude),
            new NameFuncPair("SetEnemyWanderMode",SetEnemyWanderMode),
            new NameFuncPair("EnemyDoWander",EnemyDoWander),
            // 直线激光相关
            new NameFuncPair("CreateLaser",CreateLaser),
            new NameFuncPair("SetLaserProps",SetLaserProps),
            new NameFuncPair("SetLaserPos",SetLaserPos),
            new NameFuncPair("SetLaserAngle",SetLaserAngle),
            new NameFuncPair("SetLaserExistDuration",SetLaserExistDuration),
            new NameFuncPair("SetLaserSize",SetLaserSize),
            new NameFuncPair("SetLaserRotatePara",SetLaserRotatePara),
            new NameFuncPair("ChangeLaserWidth",ChangeLaserWidth),
            new NameFuncPair("SetLaserGrazeDetectParas",SetLaserGrazeDetectParas),
            new NameFuncPair("SetLaserCollisionDetectParas",SetLaserCollisionDetectParas),

            new NameFuncPair("CreateLinearLaser",CreateLinearLaser),
            new NameFuncPair("DoLinearLaserMove",DoLinearLaserMove),
            new NameFuncPair("GetLinearLaserProps",GetLinearLaserProps),
            new NameFuncPair("SetLinearLaserProps",SetLinearLaserProps),
            new NameFuncPair("SetLinearLaserHeadEnable",SetLinearLaserHeadEnable),
            new NameFuncPair("SetLinearLaserSourceEnable",SetLinearLaserSourceEnable),
            new NameFuncPair("CreateCustomizedLinearLaser",CreateCustomizedLinearLaser),

            // 曲线激光
            new NameFuncPair("CreateCurveLaser",CreateCurveLaser),
            new NameFuncPair("SetCurveLaserStraightParas",SetCurveLaserStraightParas),
            new NameFuncPair("SetCurveLaserAcceParas",SetCurveLaserAcceParas),
            new NameFuncPair("SetCurveLaserCurveParas",SetCurveLaserCurveParas),
            new NameFuncPair("CreateCustomizedCurveLaser",CreateCustomizedCurveLaser),
            new NameFuncPair("SetCurveLaserLength",SetCurveLaserLength),
            new NameFuncPair("SetCurveLaserWidth",SetCurveLaserWidth),

            new NameFuncPair("AddEnemyTask",AddEnemyTask),
            //new NameFuncPair("EnemyMoveTo",EnemyMoveTo),
            new NameFuncPair("EnemyMoveTowards",EnemyMoveTowards),
            new NameFuncPair("EnemyMoveToPos",EnemyMoveToPos),
            new NameFuncPair("GetEnemyPos", GetEnemyPos),
            new NameFuncPair("PlaySound", PlaySound),
            // Boss相关
            new NameFuncPair("CreateBoss",CreateBoss),
            new NameFuncPair("SetBossPos",SetBossPos),
            new NameFuncPair("SetBossAni",SetBossAni),
            new NameFuncPair("PlayBossAni",PlayBossAni),
            new NameFuncPair("SetEnemyCollisionParas",SetEnemyCollisionParas),
            new NameFuncPair("SetBossCurPhaseData",SetBossCurPhaseData),
            new NameFuncPair("GetBossSpellCardHpRate",GetBossSpellCardHpRate),
            new NameFuncPair("GetBossSpellCardTimeLeftRate",GetBossSpellCardTimeLeftRate),

            new NameFuncPair("CreateSpellCard",CreateSpellCard),
            new NameFuncPair("SetSpellCardTask",SetSpellCardTask),
            new NameFuncPair("SetSpellCardFinishFunc",SetSpellCardFinishFunc),
            new NameFuncPair("EnterSpellCard",EnterSpellCard),
            // 特效
            new NameFuncPair("SetEffectToPos",SetEffectToPos),
            new NameFuncPair("SetEffectFinish",SetEffectFinish),
            new NameFuncPair("CreateSpriteEffect",CreateSpriteEffect),
            new NameFuncPair("SetSpriteEffectColor",SetSpriteEffectColor),
            new NameFuncPair("SpriteEffectScaleWidth",SpriteEffectScaleWidth),
            new NameFuncPair("SpriteEffectScaleHeight",SpriteEffectScaleHeight),
            new NameFuncPair("ShakeEffectDoShake",ShakeEffectDoShake),
            new NameFuncPair("ShakeEffectDoShakeWithLimitation",ShakeEffectDoShakeWithLimitation),
            // 子弹组件
            new NameFuncPair("AddBulletComponent",AddBulletComponent),
            new NameFuncPair("AddBulletParaChangeEvent",AddBulletParaChangeEvent),
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
            // 通用类相关
            new NameFuncPair("GetRandomInt", GetRandomInt),
            new NameFuncPair("GetRandomFloat", GetRandomFloat),
            new NameFuncPair("GetPosAfterRotate", GetPosAfterRotate),
            new NameFuncPair("GetAimToPlayerAngle", GetAimToPlayerAngle),
            new NameFuncPair("GetPlayerPos",GetPlayerPos),
            new NameFuncPair("LogLuaNumber", LogLuaNumber),
            new NameFuncPair("GetVectorLength",GetVectorLength),
            new NameFuncPair("SetGlobalVector2", SetGlobalVector2),
            new NameFuncPair("GetGlobalVector2", GetGlobalVector2),
            new NameFuncPair("SetGlobalFloat",SetGlobalFloat),
            new NameFuncPair("GetGlobalFloat", GetGlobalFloat),
            new NameFuncPair("SetGlobalUserData", SetGlobalUserData),
            new NameFuncPair("GetGlobalUserData",GetGlobalUserData),
            new NameFuncPair("RemoveGlobalUserData", RemoveGlobalUserData),
        };
        luaState.L_NewLib(define);
        return 1;
    }

    /// <summary>
    /// 创建一颗简单的子弹
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// <para>string textureName</para>
    /// <para>bool isRotatedByVAngle</para>
    /// <para>float selfRotAngle</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSimpleBullet(ILuaState luaState)
    {
        float posX = (float)luaState.ToNumber(-5);
        float posY = (float)luaState.ToNumber(-4);
        string textureName = luaState.ToString(-3);
        bool isRotatedByVAngle = luaState.ToBoolean(-2);
        float selfRotAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        EnemyBulletSimple bullet = ObjectsPool.GetInstance().CreateBullet(BulletId.Enemy_Simple) as EnemyBulletSimple;
        bullet.SetBulletTexture(textureName);
        bullet.SetToPosition(posX, posY);
        bullet.SetRotatedByVelocity(isRotatedByVAngle);
        bullet.SetSelfRotation(selfRotAngle != 0, selfRotAngle);
        luaState.PushLightUserData(bullet);
        return 1;
    }

    /// <summary>
    /// 根据id创建SimpleBullet
    /// <para>id 配置里面的id</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSimpleBulletById(ILuaState luaState)
    {
        string sysId = luaState.ToString(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        EnemyBulletDefaultCfg cfg = BulletsManager.GetInstance().GetBulletDefaultCfgById(sysId);
        EnemyBulletSimple bullet = ObjectsPool.GetInstance().CreateBullet(BulletId.Enemy_Simple) as EnemyBulletSimple;
        bullet.ChangeStyleById(sysId);
        //bullet.SetBulletTexture(cfg.prefabName);
        bullet.SetToPosition(posX, posY);
        //bullet.SetRotatedByVelocity(cfg.isRotatedByVAngle);
        //bullet.SetSelfRotation(cfg.selfRotationAngle!=0,cfg.selfRotationAngle);
        //CollisionDetectParas detectParas = new CollisionDetectParas
        //{
        //    type = CollisionDetectType.Circle,
        //    radius = cfg.collisionRadius,
        //};
        //GrazeDetectParas grazeParas = new GrazeDetectParas
        //{
        //    type = GrazeDetectType.Rect,
        //    halfWidth = cfg.grazeHalfWidth,
        //    halfHeight = cfg.grazeHalfHeight,
        //};
        //bullet.SetCollisionDetectParas(detectParas);
        //bullet.SetGrazeDetectParas(grazeParas);
        luaState.PushLightUserData(bullet);
        return 1;
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

    /// <summary>
    /// <para>bullet</para>
    /// <para>velocity</para>
    /// <para>vAngle</para>
    /// <para>isAimToPlayer</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletStraightParas(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-6) as EnemyBulletSimple;
        float velocity = (float)luaState.ToNumber(-5);
        float vAngle = (float)luaState.ToNumber(-4);
        bool isAimToPlayer = luaState.ToBoolean(-3);
        float acce = (float)luaState.ToNumber(-2);
        float accAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(6);
        // 设置子弹线性运动
        if (isAimToPlayer)
        {
            vAngle += MathUtil.GetAngleBetweenXAxis(Global.PlayerPos.x - bullet.PosX, Global.PlayerPos.y - bullet.PosY, false);
        }
        bullet.DoMoveStraight(velocity, vAngle);
        bullet.DoAcceleration(acce, accAngle);
        return 0;
    }

    /// <summary> 赋予子弹加速度
    /// <para>bullet</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DoBulletAcceleration(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-3) as EnemyBulletSimple;
        float acce = (float)luaState.ToNumber(-2);
        float accAngle = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        bullet.DoAcceleration(acce, accAngle);
        return 0;
    }

    /// <summary> 赋予子弹加速度
    /// <para>bullet</para>
    /// <para>acce</para>
    /// <para>accAngle</para>
    /// <para>duration</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DoBulletAccelerationWithLimitation(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-4) as EnemyBulletSimple;
        float acce = (float)luaState.ToNumber(-3);
        float accAngle = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        luaState.Pop(4);
        bullet.DoAccelerationWithLimitation(acce, accAngle, duration);
        return 0;
    }

    /// <summary>
    /// <para>bullet</para>
    /// <para>radius</para>
    /// <para>curveAngle</para>
    /// <para>deltaR</para>
    /// <para>omiga</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletCurvePara(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-5) as EnemyBulletSimple;
        float radius = (float)luaState.ToNumber(-4);
        float curveAngle = (float)luaState.ToNumber(-3);
        float deltaR = (float)luaState.ToNumber(-2);
        float omiga = (float)luaState.ToNumber(-1);
        luaState.Pop(5);
        bullet.DoMoveCurve(radius, curveAngle, deltaR, omiga);
        return 0;
    }

    public static int ChangeBulletStyleById(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-2) as EnemyBulletSimple;
        string id = luaState.ToString(-1);
        luaState.Pop(2);
        bullet.ChangeStyleById(id);
        return 0;
    }

    public static int SetBulletOrderInLayer(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-2) as EnemyBulletSimple;
        int order = luaState.ToInteger(-1);
        luaState.Pop(2);
        bullet.SetOrderInLayer(order);
        return 0;
    }

    public static int EliminateBullet(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-1) as EnemyBulletSimple;
        luaState.Pop(1);
        bullet.Eliminate();
        return 0;
    }

    /// <summary>
    /// 将子弹设置为虚化状态
    /// <para>bullet</para>
    /// <para>int unrealDuration 虚化的时间</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletToUnrealState(ILuaState luaState)
    {
        EnemyBulletSimple bullet = luaState.ToUserData(-2) as EnemyBulletSimple;
        int unrealDuration = luaState.ToInteger(-1);
        luaState.Pop(2);
        bullet.SetToUnrealState(unrealDuration);
        return 0;
    }

    public static int SetBulletDetectCollision(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        bool value = luaState.ToBoolean(-1);
        luaState.Pop(2);
        bullet.SetDetectCollision(value);
        return 0;
    }

    /// <summary>
    /// 设置子弹的消除抗性
    /// <para>bullet</para>
    /// <para>flag 不能被消除的标识</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletResistEliminatedFlag(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        int flag = luaState.ToInteger(-1);
        luaState.Pop(2);
        bullet.SetResistEliminateFlag(flag);
        return 0;
    }

    #region 获取、设置子弹相关的参数

    /// <summary>
    /// 获取子弹当前位置
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetBulletPos(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-1) as EnemyBulletBase;
        luaState.Pop(1);
        luaState.PushNumber(bullet.PosX);
        luaState.PushNumber(bullet.PosY);
        return 2;
    }

    /// <summary>
    /// 设置子弹位置
    /// <para>bullet</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetBulletPos(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-3) as EnemyBulletBase;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        bullet.SetToPosition(posX, posY);
        return 0;
    }
    #endregion

    #region 自定义simpleBullet

    /// <summary>
    /// 创建自定义的simpleBullet
    /// <para>customizedName 自定义的子弹名称</para>
    /// <para>id 默认的id</para>
    /// <para>float posX</para>
    /// <para>float posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedBullet(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        string customizedName = luaState.ToString(-5-numArgs);
        string sysId = luaState.ToString(-4-numArgs);
        float posX = (float)luaState.ToNumber(-3-numArgs);
        float posY = (float)luaState.ToNumber(-2-numArgs);
        luaState.Pop(1);
        EnemyBulletDefaultCfg cfg = BulletsManager.GetInstance().GetBulletDefaultCfgById(sysId);
        EnemyBulletSimple bullet = ObjectsPool.GetInstance().CreateBullet(BulletId.Enemy_Simple) as EnemyBulletSimple;
        bullet.SetBulletTexture(cfg.prefabName);
        bullet.SetToPosition(posX, posY);
        bullet.SetRotatedByVelocity(cfg.isRotatedByVAngle);
        bullet.SetSelfRotation(cfg.selfRotationAngle != 0, cfg.selfRotationAngle);
        // 设置碰撞参数
        CollisionDetectParas detectParas = new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            radius = cfg.collisionRadius,
        };
        GrazeDetectParas grazeParas = new GrazeDetectParas
        {
            type = GrazeDetectType.Rect,
            halfWidth = cfg.grazeHalfWidth,
            halfHeight = cfg.grazeHalfHeight,
        };
        bullet.SetCollisionDetectParas(detectParas);
        bullet.SetGrazeDetectParas(grazeParas);
        // 设置自定义的数据
        BCCustomizedTask bc = bullet.AddComponent<BCCustomizedTask>();
        //InterpreterManager.GetInstance().CallCostomizedInitFunc(bullet, customizedName,numArgs);
        int funcRef = InterpreterManager.GetInstance().GetInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        if (!luaState.IsFunction(-1))
        {
            Logger.Log("InitFuncRef of " + customizedName + " is not point to a function");
        }
        luaState.PushLightUserData(bullet);
        // todo 以后有配置文件之后这个写法一定要改
        // 将函数和第一个参数bullet移动到指定位置
        //Logger.Log(luaState.GetTop());
        luaState.Replace(-3 - numArgs);
        luaState.Replace(-3 - numArgs);
        luaState.Call(numArgs + 1, 0);
        // 弹出剩余两个参数
        luaState.Pop(2);
        luaState.PushLightUserData(bullet);
        return 1;
    }

    public static int AddBulletTask(ILuaState luaState)
    {
        EnemyBulletBase bullet = luaState.ToUserData(-2) as EnemyBulletBase;
        int funcRef = luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        luaState.Pop(1);
        Task task = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        task.funcRef = funcRef;
        task.isFinish = false;
        task.luaState = null;
        BCCustomizedTask bc = bullet.GetComponent<BCCustomizedTask>();
        bc.AddTask(task);
        return 0;
    }

    public static int CreateCustomizedLaser(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        string customizedName = luaState.ToString(-3 - numArgs);
        string laserTexture = luaState.ToString(-2 - numArgs);
        // 弹出参数个数
        luaState.Pop(1);
        EnemyLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletId.Enemy_Laser) as EnemyLaser;
        laser.SetBulletTexture(laserTexture);
        // 设置自定义的数据
        BCCustomizedTask bc = laser.AddComponent<BCCustomizedTask>();
        int funcRef = InterpreterManager.GetInstance().GetInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, funcRef);
        if (!luaState.IsFunction(-1))
        {
            Logger.Log("InitFuncRef of " + customizedName + " is not point to a function");
        }
        luaState.PushLightUserData(laser);
        luaState.Replace(-3 - numArgs);
        luaState.Replace(-3 - numArgs);
        luaState.Call(numArgs + 1, 0);
        // 将laser压入栈
        luaState.PushLightUserData(laser);
        return 1;
    }
    #endregion

    #region 直线激光相关

    public static int CreateLaser(ILuaState luaState)
    {
        string texture = luaState.ToString(-7);
        float posX = (float)luaState.ToNumber(-6);
        float posY = (float)luaState.ToNumber(-5);
        float angle = (float)luaState.ToNumber(-4);
        float width = (float)luaState.ToNumber(-3);
        float height = (float)luaState.ToNumber(-2);
        int existDuration = luaState.ToInteger(-1);
        luaState.Pop(7);
        EnemyLaser laser = ObjectsPool.GetInstance().CreateBullet(BulletId.Enemy_Laser) as EnemyLaser;
        laser.SetBulletTexture(texture);
        laser.SetPosition(posX, posY, angle);
        laser.SetLaserSize(width, height);
        laser.SetLaserExistDuration(existDuration);
        luaState.PushLightUserData(laser);
        return 1;
    }

    public static int ChangeLaserWidth(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-4) as EnemyLaser;
        float toWidth = (float)luaState.ToNumber(-3);
        int changeDuration = luaState.ToInteger(-2);
        int changeDelay = luaState.ToInteger(-1);
        luaState.Pop(4);
        laser.ChangeToWidth(toWidth, changeDuration, changeDelay);
        return 0;
    }
    
    /// <summary>
    /// 设置激光的属性
    /// <para>laser 激光本体</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// <para>angle 激光的角度，x正半轴为0,逆时针增加角度</para>
    /// <para>width 激光宽度</para>
    /// <para>height 激光高度</para>
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
        float width = (float)luaState.ToNumber(-3);
        float height = (float)luaState.ToNumber(-2);
        int existDuration = luaState.ToInteger(-1);
        luaState.Pop(7);
        laser.SetPosition(posX, posY, angle);
        laser.SetLaserSize(width, height);
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
        laser.SetToPosition(posX, posY);
        return 0;
    }

    public static int SetLaserAngle(ILuaState luaState)
    {
        EnemyLaser laser = luaState.ToUserData(-2) as EnemyLaser;
        float angle = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        laser.SetLaserAngle(angle);
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
        float halfWidht = (float)luaState.ToNumber(-2);
        float halfHeight = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        laser.SetLaserSize(halfWidht, halfHeight);
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
        EnemyLaser laser = luaState.ToUserData(-3) as EnemyLaser;
        CollisionDetectParas detectParas = new CollisionDetectParas()
        {
            type = CollisionDetectType.Rect,
            halfWidth = (float)luaState.ToNumber(-2),
            halfHeight = (float)luaState.ToNumber(-1),
        };
        luaState.Pop(3);
        laser.SetCollisionDetectParas(detectParas);
        return 0;
    }

    #endregion


    #region 创建敌机相关
    /// <summary>
    /// 创建普通敌机
    /// <para>string enemyId 敌机id</para>
    /// <para>float posX 起始X坐标</para>
    /// <para>float posY 起始Y坐标</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateNormalEnemyById(ILuaState luaState)
    {
        string enemyId = luaState.ToString(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        EnemyCfg cfg = EnemyManager.GetInstance().GetEnemyCfgById(enemyId);
        NormalEnemy enemy = EnemyManager.GetInstance().CreateEnemyByType(EnemyType.NormalEnemy) as NormalEnemy;
        enemy.Init(cfg);
        enemy.SetToPosition(new Vector3(posX, posY, 0));
        luaState.PushLightUserData(enemy);
        Logger.Log("Create Normal Enemy Complete");
        return 1;
    }

    public static int AddEnemyTask(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int funcRef = luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        luaState.Pop(1);
        Task task = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        task.funcRef = funcRef;
        task.isFinish = false;
        task.luaState = null;
        enemy.AddTask(task);
        return 0;
    }

    public static int CreateCustomizedEnemy(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        luaState.Pop(1);
        string customizedName = luaState.ToString(-4 - numArgs);
        string enemyId = luaState.ToString(-3 - numArgs);
        float posX = (float)luaState.ToNumber(-2 - numArgs);
        float posY = (float)luaState.ToNumber(-1 - numArgs);
        // 创建敌机
        EnemyCfg cfg = EnemyManager.GetInstance().GetEnemyCfgById(enemyId);
        NormalEnemy enemy = EnemyManager.GetInstance().CreateEnemyByType(EnemyType.NormalEnemy) as NormalEnemy;
        enemy.Init(cfg);
        enemy.SetToPosition(new Vector3(posX, posY, 0));
        int onEliminateFuncRef = InterpreterManager.GetInstance().GetEnemyOnEliminateFuncRef(customizedName);
        if (onEliminateFuncRef != 0 )
        {
            enemy.SetOnEliminateFuncRef(onEliminateFuncRef);
        }
        // init函数
        int initFuncRef = InterpreterManager.GetInstance().GetEnemyInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, initFuncRef);
        luaState.PushLightUserData(enemy);
        luaState.Replace(-3 - numArgs);
        luaState.Replace(-3 - numArgs);
        luaState.Call(numArgs + 1, 0);
        // 弹出剩余两个参数
        luaState.Pop(2);
        // 将返回值压入栈
        luaState.PushLightUserData(enemy);
        return 1;
    }

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
    public static int CreateBoss(ILuaState luaState)
    {
        string bossName = luaState.ToString(-1);
        luaState.Pop(1);
        Boss boss = EnemyManager.GetInstance().CreateEnemyByType(EnemyType.Boss) as Boss;
        boss.Init(bossName);
        Global.Boss = boss;
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
        boss.SetToPosition(new Vector3(posX, posY,0));
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
        string aniId = luaState.ToString(-1);
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

    /// <summary> 创建一个符卡对象
    /// <para>type 符卡类型</para>
    /// <para>name 符卡名称</para>
    /// <para>scHp 符卡血量</para>
    /// <para>scDuration 符卡持续时间，单位秒</para>
    /// <para>invincibleDuration 符卡起始无敌时间,单位秒</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateSpellCard(ILuaState luaState)
    {
        string scName = luaState.ToString(-4);
        float scHp = (float)luaState.ToNumber(-3);
        float scDuration = (float)luaState.ToNumber(-2);
        float invincibleDuration = (float)luaState.ToNumber(-1);
        luaState.Pop(4);
        SpellCard sc = new SpellCard();
        sc.name = scName;
        sc.maxHp = scHp;
        sc.spellDuration = scDuration;
        sc.invincibleDuration = invincibleDuration;
        luaState.PushLightUserData(sc);
        return 1;
    }

    public static int SetSpellCardTask(ILuaState luaState)
    {
        SpellCard sc = luaState.ToUserData(-2) as SpellCard;
        sc.taskRef = luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        Logger.Log("Ref SpellCard Task, FuncRef = " + sc.taskRef);
        luaState.Pop(1);
        return 0;
    }

    public static int SetSpellCardFinishFunc(ILuaState luaState)
    {
        SpellCard sc = luaState.ToUserData(-2) as SpellCard;
        sc.finishFuncRef = luaState.L_Ref(LuaDef.LUA_REGISTRYINDEX);
        luaState.Pop(1);
        return 0;
    }

    /// <summary>
    /// 进入符卡
    /// <para>boss</para>
    /// <para>sc</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnterSpellCard(ILuaState luaState)
    {
        Boss boss = luaState.ToUserData(-2) as Boss;
        SpellCard sc = luaState.ToUserData(-1) as SpellCard;
        luaState.Pop(2);
        boss.EnterSpellCard(sc);
        luaState.PushInteger(1);
        luaState.Yield(1);
        return 0;
    }

    public static int SetBossCurPhaseData(ILuaState luaState)
    {
        int count = luaState.ToInteger(-1);
        int index = -1 - count;
        List<float> weights = new List<float>();
        Boss boss = luaState.ToUserData(index-1) as Boss;
        for (;index<-1;index++)
        {
            weights.Add((float)luaState.ToNumber(index));
        }
        boss.SetCurPhaseData(weights);
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
        luaState.Pop(2);
        luaState.PushInteger(MTRandom.GetNextInt(begin,end));
        return 1;
    }

    public static int GetRandomFloat(ILuaState luaState)
    {
        float begin = (float)luaState.ToNumber(-2);
        float end = (float)luaState.ToNumber(-1);
        luaState.Pop(2);
        luaState.PushNumber(MTRandom.GetNextFloat(begin, end));
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
        float angle = MathUtil.GetAngleBetweenXAxis(Global.PlayerPos.x - posX, Global.PlayerPos.y - posY, false);
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

    /// <summary>
    /// 获取玩家坐标
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetPlayerPos(ILuaState luaState)
    {
        luaState.PushNumber(Global.PlayerPos.x);
        luaState.PushNumber(Global.PlayerPos.y);
        return 2;
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
