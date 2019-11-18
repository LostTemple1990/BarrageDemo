using UnityEngine;
using System.Collections.Generic;
using UniLua;

public partial class LuaLib
{
    /// <summary>
    /// 创建普通敌机
    /// <para>string enemyId 敌机id</para>
    /// <para>int maxHp 初始血量</para>
    /// <para>float posX 起始X坐标</para>
    /// <para>float posY 起始Y坐标</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateNormalEnemyById(ILuaState luaState)
    {
        string enemyId;
        if (luaState.Type(-4) == LuaType.LUA_TNUMBER)
        {
            enemyId = luaState.ToNumber(-4).ToString();
        }
        else
        {
            enemyId = luaState.ToString(-4);
        }
        int maxHp = luaState.ToInteger(-3);
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        luaState.Pop(3);
        EnemyCfg cfg = EnemyManager.GetInstance().GetEnemyCfgById(enemyId);
        NormalEnemy enemy = EnemyManager.GetInstance().CreateEnemyByType(eEnemyType.NormalEnemy) as NormalEnemy;
        enemy.Init(cfg);
        enemy.SetMaxHp(maxHp);
        enemy.SetPosition(new Vector3(posX, posY, 0));
        luaState.PushLightUserData(enemy);
        return 1;
    }

    public static int AddEnemyTask(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int funcRef = InterpreterManager.GetInstance().RefLuaFunction(luaState);
        luaState.Pop(1);
        Task task = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        task.funcRef = funcRef;
        enemy.AddTask(task);
        return 0;
    }

    /// <summary>
    /// 创建自定义的敌机
    /// <para>customizedName 自定义的名称</para>
    /// <para>enemyId 配置的id</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// <para>paras...</para>
    /// <para>numArgs 参数个数</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedEnemy2(ILuaState luaState)
    {
        int numArgs = luaState.ToInteger(-1);
        luaState.Pop(1);
        string customizedName = luaState.ToString(-4 - numArgs);
        string enemyId;
        if (luaState.Type(-3 - numArgs) == LuaType.LUA_TNUMBER)
        {
            enemyId = luaState.ToNumber(-3 - numArgs).ToString();
        }
        else
        {
            enemyId = luaState.ToString(-3 - numArgs);
        }
        float posX = (float)luaState.ToNumber(-2 - numArgs);
        float posY = (float)luaState.ToNumber(-1 - numArgs);
        // 创建敌机
        EnemyCfg cfg = EnemyManager.GetInstance().GetEnemyCfgById(enemyId);
        NormalEnemy enemy = EnemyManager.GetInstance().CreateEnemyByType(eEnemyType.NormalEnemy) as NormalEnemy;
        enemy.Init(cfg);
        enemy.SetPosition(posX, posY);
        int onEliminateFuncRef = InterpreterManager.GetInstance().GetEnemyOnEliminateFuncRef(customizedName);
        if (onEliminateFuncRef != 0)
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

    /// <summary>
    /// 创建自定义的敌机
    /// <para>customizedName 自定义的名称</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// <para>paras...</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int CreateCustomizedEnemy(ILuaState luaState)
    {
        int numArgs = luaState.GetTop() - 3;
        string customizedName = luaState.ToString(-3 - numArgs);
        float posX = (float)luaState.ToNumber(-2 - numArgs);
        float posY = (float)luaState.ToNumber(-1 - numArgs);
        // 创建敌机
        NormalEnemy enemy = EnemyManager.GetInstance().CreateEnemyByType(eEnemyType.NormalEnemy) as NormalEnemy;
        enemy.SetPosition(posX, posY);
        int onEliminateFuncRef = InterpreterManager.GetInstance().GetEnemyOnEliminateFuncRef(customizedName);
        if (onEliminateFuncRef != 0)
        {
            enemy.SetOnEliminateFuncRef(onEliminateFuncRef);
        }
        // init函数
        int initFuncRef = InterpreterManager.GetInstance().GetEnemyInitFuncRef(customizedName);
        luaState.RawGetI(LuaDef.LUA_REGISTRYINDEX, initFuncRef);
        luaState.PushLightUserData(enemy);
        for (int i=0;i<numArgs;i++)
        {
            luaState.PushValue(-2 - numArgs);
        }
        luaState.Call(numArgs + 1, 0);
        // 将返回值压入栈
        luaState.PushLightUserData(enemy);
        return 1;
    }

    /// <summary>
    /// 初始化敌机
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyInit(ILuaState luaState)
    {
        string enemyId;
        NormalEnemy enemy = luaState.ToUserData(-2) as NormalEnemy;
        if (luaState.Type(-1) == LuaType.LUA_TNUMBER)
        {
            enemyId = luaState.ToInteger(-1).ToString();
        }
        else
        {
            enemyId = luaState.ToString(-1);
        }
        enemy.Init(enemyId);
        return 0;
    }

    public static int EnemyMoveTowards(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-4) as EnemyBase;
        float v = (float)luaState.ToNumber(-3);
        float angle = (float)luaState.ToNumber(-2);
        int duration = luaState.ToInteger(-1);
        enemy.MoveTowards(v, angle, duration);
        return 0;
    }

    public static int EnemyMoveToPos(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-5) as EnemyBase;
        float endX = (float)luaState.ToNumber(-4);
        float endY = (float)luaState.ToNumber(-3);
        int duration = luaState.ToInteger(-2);
        int moveMode = luaState.ToInteger(-1);
        enemy.MoveTo(endX, endY, duration, (InterpolationMode)moveMode);
        return 0;
    }

    /// <summary>
    /// 敌机朝着某个方向做加速运动
    /// <para>enemy</para>
    /// <para>velocity 速度</para>
    /// <para>速度的方向</para>
    /// <para>加速度</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyAccMoveTowards(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-4) as EnemyBase;
        float velocity = (float)luaState.ToNumber(-3);
        float angle = (float)luaState.ToNumber(-2);
        float acc = (float)luaState.ToNumber(-1);
        enemy.AccMoveTowards(velocity, angle, acc);
        return 0;
    }

    /// <summary>
    /// 敌机朝着某个方向做加速运动（带最大速度限制）
    /// <para>enemy</para>
    /// <para>velocity 速度</para>
    /// <para>速度的方向</para>
    /// <para>加速度</para>
    /// <para>速度最大值</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyAccMoveTowardsWithLimitation(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-5) as EnemyBase;
        float velocity = (float)luaState.ToNumber(-4);
        float angle = (float)luaState.ToNumber(-3);
        float acc = (float)luaState.ToNumber(-2);
        float maxVelocity = (float)luaState.ToNumber(-1);
        enemy.AccMoveTowardsWithLimitation(velocity, angle, acc, maxVelocity);
        return 0;
    }

    /// <summary>
    /// 获取敌机位置
    /// <para>return value</para>
    /// <para>posX,posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetEnemyPos(ILuaState luaState)
    {
        int top = luaState.GetTop();
        EnemyBase enemy = luaState.ToUserData(-1) as EnemyBase;
        Vector2 pos = enemy.GetPosition();
        luaState.PushNumber(pos.x);
        luaState.PushNumber(pos.y);
        return 2;
    }

    /// <summary>
    /// 设置敌机位置
    /// <para>enemy</para>
    /// <para>posX</para>
    /// <para>posY</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyPos(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-3) as EnemyBase;
        float posX = (float)luaState.ToNumber(-2);
        float posY = (float)luaState.ToNumber(-1);
        enemy.SetPosition(new Vector3(posX, posY));
        return 0;
    }

    public static int HitEnemy(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int damage = luaState.ToInteger(-1);
        enemy.TakeDamage(damage);
        return 0;
    }

    /// <summary>
    /// 设置敌机不会被某些东西所消除
    /// <para>enemyBase</para>
    /// <para>resistFlag</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyResistEliminateFlag(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int resistFlag = luaState.ToInteger(-1);
        enemy.SetResistEliminateFlag(resistFlag);
        return 0;
    }

    public static int EliminateEnemy(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-1) as EnemyBase;
        enemy.Eliminate(eEliminateDef.CodeEliminate);
        return 0;
    }

    public static int RawEliminateEnemy(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-1) as EnemyBase;
        enemy.Eliminate(eEliminateDef.CodeRawEliminate);
        return 0;
    }

    /// <summary>
    /// 设置移动范围
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyWanderRange(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-5) as EnemyBase;
        float minX = (float)luaState.ToNumber(-4);
        float maxX = (float)luaState.ToNumber(-3);
        float minY = (float)luaState.ToNumber(-2);
        float maxY = (float)luaState.ToNumber(-1);
        enemy.SetWanderRange(minX, maxX, minY, maxY);
        return 0;
    }

    /// <summary>
    /// 设置每次移动的距离限制
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyWanderAmplitude(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-5) as EnemyBase;
        float minX = (float)luaState.ToNumber(-4);
        float maxX = (float)luaState.ToNumber(-3);
        float minY = (float)luaState.ToNumber(-2);
        float maxY = (float)luaState.ToNumber(-1);
        enemy.SetWanderAmplitude(minX, maxX, minY, maxY);
        return 0;
    }

    /// <summary>
    /// 设置移动的模式
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyWanderMode(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-3) as EnemyBase;
        InterpolationMode moveMode = (InterpolationMode)luaState.ToInteger(-2);
        DirectionMode dirMode = (DirectionMode)luaState.ToInteger(-1);
        enemy.SetWanderMode(moveMode, dirMode);
        return 0;
    }

    /// <summary>
    /// 进行移动
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int EnemyDoWander(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        int duration = luaState.ToInteger(-1);
        enemy.DoWander(duration);
        return 0;
    }

    /// <summary>
    /// 设置敌机的最大血量，同时同步当前血量到最大血量
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyMaxHp(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        float maxHp = (float)luaState.ToNumber(-1);
        enemy.SetMaxHp(maxHp);
        return 0;
    }

    /// <summary>
    /// 获取boss当前符卡的hp比例
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int GetBossSpellCardHpRate(ILuaState luaState)
    {
        int index = luaState.ToInteger(-1);
        luaState.Pop(1);
        SpellCard sc = STGStageManager.GetInstance().GetSpellCard();
        Boss boss = sc.GetBossByIndex(index);
        float rate = 0f;
        if ( boss != null )
        {
            rate = (float)boss.GetCurHp() / boss.GetMaxHp();
        }
        luaState.PushNumber(rate);
        return 1;
    }

    public static int GetSpellCardTimeLeftRate(ILuaState luaState)
    {
        SpellCard sc = STGStageManager.GetInstance().GetSpellCard();
        float rate = sc.GetTimeRate();
        luaState.PushNumber(rate);
        return 1;
    }


    /// <summary>
    /// 设置一个敌机死亡时掉落的物品
    /// <para>enemy</para>
    /// <para>dropItemDatas List{itemType,itemCount}</para>
    /// <para>halfWidth</para>
    /// <para>halfHeight</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyDropItems(ILuaState luaState)
    {
        int top = luaState.GetTop();
        NormalEnemy enemy = luaState.ToUserData(-top) as NormalEnemy;
        float halfWidth = (float)luaState.ToNumber(-2);
        float halfHeight = (float)luaState.ToNumber(-1);
        // 遍历dropItemDatas
        List<int> itemDatas = new List<int>();
        for (int index = -(top - 1); index <= -3; index += 2)
        {
            itemDatas.Add(luaState.ToInteger(index));
            itemDatas.Add(luaState.ToInteger(index+1));
        }
        enemy.SetDropItemDatas(itemDatas, halfWidth, halfHeight);
        return 0;
    }

    /// <summary>
    /// 掉落物品
    /// <para>dropItemDatas List{itemType,itemCount}</para>
    /// <para>centerX</para>
    /// <para>centerY</para>
    /// <para>halfWidth</para>
    /// <para>halfHeight</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int DropItems(ILuaState luaState)
    {
        int top = luaState.GetTop();
        float centerX = (float)luaState.ToNumber(-top);
        float centerY = (float)luaState.ToNumber(-top + 1);
        float halfWidth = (float)luaState.ToNumber(-top + 2);
        float halfHeight = (float)luaState.ToNumber(-top + 3);
        // 遍历
        List<int> itemDatas = new List<int>();
        for (int index = -top + 4; index <= -1; index += 2)
        {
            itemDatas.Add(luaState.ToInteger(index));
            itemDatas.Add(luaState.ToInteger(index + 1));
        }
        ItemManager.GetInstance().DropItems(itemDatas, centerX, centerY, halfWidth, halfHeight);
        return 0;
    }

    /// <summary>
    /// 设置敌机是否可以交互
    /// <para>enemy</para>
    /// <para>isInteractive 是否可以交互</para>
    /// </summary>
    /// <param name="luaState"></param>
    /// <returns></returns>
    public static int SetEnemyInteractive(ILuaState luaState)
    {
        EnemyBase enemy = luaState.ToUserData(-2) as EnemyBase;
        bool isInteractive = luaState.ToBoolean(-1);
        enemy.SetInteractive(isInteractive);
        return 0;
    }
}
