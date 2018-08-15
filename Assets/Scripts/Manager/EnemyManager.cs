using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager
{
    private static EnemyManager _instance;

    public static EnemyManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new EnemyManager();
        }
        return _instance;
    }

    private EnemyManager()
    {
        _normalEnemies = new List<EnemyBase>();
        _clearList = new List<EnemyBase>();
        _enemiesPool = new Dictionary<EnemyType, Stack<EnemyBase>>();
        _enemyDatabase = DataManager.GetInstance().GetDatasByName("EnemyCfgs") as Dictionary<string, IParser>;
        _bossRefDatabase = new Dictionary<string, BossRefData>();
        _enemyObjectsPool = new Dictionary<EnemyObjectType, Stack<EnemyObjectBase>>();
    }
    /// <summary>
    /// 待清除的敌机列表
    /// </summary>
    private List<EnemyBase> _clearList;

    private List<EnemyBase> _normalEnemies;
    /// <summary>
    /// 对象池
    /// </summary>
    private Dictionary<EnemyType,Stack<EnemyBase>> _enemiesPool;

    private Dictionary<string, IParser> _enemyDatabase;

    private Dictionary<string, BossRefData> _bossRefDatabase;

    private Dictionary<EnemyObjectType, Stack<EnemyObjectBase>> _enemyObjectsPool;

    public void Init()
    {

    }

    public EnemyCfg GetEnemyCfgById(string id)
    {
        IParser parser;
        if ( !_enemyDatabase.TryGetValue(id,out parser) )
        {
            return null;
        }
        return parser as EnemyCfg; ;
    }

    //protected T NewEnemy<T>()
    //    where T:EnemyBase,new()
    //{
        
    //}

    public EnemyBase CreateEnemy(EnemyType type)
    {
        EnemyBase enemy = NewEnemy(type);
        RegisterEnemy(enemy);
        enemy.Init();
        return enemy;
    }

    protected EnemyBase NewEnemy(EnemyType type)
    {
        Stack<EnemyBase> stack;
        EnemyBase enemy = null;
        if ( _enemiesPool.TryGetValue(type,out stack) )
        {
            if ( stack.Count > 0 )
            {
                enemy = stack.Pop();
            }
        }
        else
        {
            stack = new Stack<EnemyBase>();
            _enemiesPool.Add(type, stack);
        }
        if (enemy == null)
        {
            switch (type)
            {
                case EnemyType.NormalEnemy:
                    enemy = new NormalEnemy();
                    break;
                case EnemyType.Boss:
                    enemy = new Boss();
                    break;
            }
        }
        return enemy;
    }

    public void RestoreEnemyToPool(EnemyBase enemy)
    {
        
    }

    public bool RegisterEnemy(EnemyBase enemy)
    {
        _normalEnemies.Add(enemy);
        //switch ( enemy.Type )
        //{
        //    case EnemyType.NormalEnemy:
        //        _normalEnemies.Add(enemy as NormalEnemy);
        //        break;
        //}
        return true;
    }

    public void AddToClearList(EnemyBase enemy)
    {
        _clearList.Add(enemy);
    }

    public void Update()
    {
        UpdateEnemies();
        CheckClearEnemies();
    }

    /// <summary>
    /// 获取敌机列表
    /// </summary>
    /// <returns></returns>
    public List<EnemyBase> GetEnemyList()
    {
        return _normalEnemies;
    }

    /// <summary>
    /// 获得玩家子弹击中的敌机
    /// </summary>
    /// <param name="bullet">玩家发射的子弹</param>
    /// <returns></returns>
    public EnemyBase GetHitEnemy(PlayerBulletBase bullet)
    {
        EnemyBase hitEnemy = null, tmpEnemy;
        float bulletX, bulletY, bulletHalfWidth, bulletHalfHeight;
        bullet.GetCollisionParams(out bulletX, out bulletY, out bulletHalfWidth, out bulletHalfHeight);
        //TODO 以后将normalEnemies统一成EnemyBase
        int tmpCount = _normalEnemies.Count;
        float enemyX, enemyY, enemyHalfWidth, enemyHalfHeight;
        for (int i=0;i<tmpCount;i++)
        {
            tmpEnemy = _normalEnemies[i];
            if ( tmpEnemy != null && tmpEnemy.CanHit() )
            {
                tmpEnemy.GetCollisionParams(out enemyX, out enemyY, out enemyHalfWidth, out enemyHalfHeight);
                if (Mathf.Abs(bulletX - enemyX) <= bulletHalfWidth + enemyHalfWidth && Mathf.Abs(bulletY - enemyY) <= bulletHalfHeight + enemyHalfHeight)
                {
                    hitEnemy = tmpEnemy;
                    break;
                }
            }
        }
        return hitEnemy;
    }

    public EnemyBase GetRandomEnemy()
    {
        EnemyBase enemy = null;
        if ( _normalEnemies.Count > 0 )
        {
            enemy = _normalEnemies[MTRandom.GetNextInt(0, _normalEnemies.Count-1)];
        }
        return enemy;
    }

    private void UpdateEnemies()
    {
        EnemyBase ememy;
        int tmpCount, i, j, findFlag;
        tmpCount = _normalEnemies.Count;
        for (i = 0, j = i + 1; i < tmpCount; i++, j++)
        {
            findFlag = 1;
            if (_normalEnemies[i] == null)
            {
                j = j == 1 ? i + 1 : j;
                findFlag = 0;
                for (; j < tmpCount; j++)
                {
                    if (_normalEnemies[j] != null)
                    {
                        _normalEnemies[i] = _normalEnemies[j];
                        _normalEnemies[j] = null;
                        findFlag = 1;
                        break;
                    }
                }
            }
            ememy = _normalEnemies[i];
            if (ememy != null)
            {
                ememy.Update();
            }
            if (findFlag == 0)
            {
                _normalEnemies.RemoveRange(i, tmpCount - i);
                break;
            }
        }
    }

    private void CheckClearEnemies()
    {
        EnemyBase enemy;
        int tmpCount, i;
        for (i = 0, tmpCount = _normalEnemies.Count; i < tmpCount; i++)
        {
            enemy = _normalEnemies[i];
            if (enemy.ClearFlag == 1)
            {
                _normalEnemies[i] = null;
                _clearList.Add(enemy);
            }
        }
        tmpCount = _clearList.Count;
        for (i = 0; i < tmpCount; i++)
        {
            enemy = _clearList[i];
            enemy.Clear();
            //BulletPool.GetInstance().RestoreBullet(enemy);
        }
        _clearList.Clear();
    }

    public void AddBossRefData(string bossName,BossRefData refData)
    {
        _bossRefDatabase.Add(bossName, refData);
    }

    public void AddBossRefData(string bossName,int initRef,int taskRef)
    {
        BossRefData refData = new BossRefData()
        {
            initFuncRef = initRef,
            taskFuncRef = taskRef,
        };
        _bossRefDatabase.Add(bossName, refData);
    }

    public BossRefData GetRefDataByName(string bossName)
    {
        BossRefData refData;
        if ( _bossRefDatabase.TryGetValue(bossName,out refData) )
        {
            return refData;
        }
        Logger.LogError("RefData by name " + bossName + " is not exist!");
        return refData;
    }

    public void EliminateAllEnemyByCode(bool isIncludingBoss)
    {
        int tmpCount = _normalEnemies.Count;
        EnemyBase enemy;
        for (int i=0;i<tmpCount;i++)
        {
            enemy = _normalEnemies[i];
            if ( enemy != null && enemy.ClearFlag == 0 )
            {
                if ( enemy.Type != EnemyType.Boss || isIncludingBoss )
                {
                    enemy.Eliminate(eEnemyEliminateDef.CodeEliminate);
                }
            }
        }
    }

    public void RawEliminateAllEnemyByCode(bool isIncludingBoss)
    {
        int tmpCount = _normalEnemies.Count;
        EnemyBase enemy;
        for (int i = 0; i < tmpCount; i++)
        {
            enemy = _normalEnemies[i];
            if (enemy != null && enemy.ClearFlag == 0)
            {
                if (enemy.Type != EnemyType.Boss || isIncludingBoss)
                {
                    enemy.Eliminate(eEnemyEliminateDef.CodeRawEliminate);
                }
            }
        }
    }

    public EnemyObjectBase CreateEnemyObjectByType(EnemyObjectType type)
    {
        EnemyObjectBase enemyObj = null;
        Stack<EnemyObjectBase> _stack;
        if ( _enemyObjectsPool.TryGetValue(type,out _stack) )
        {
            if ( _stack.Count > 0 )
            {
                return _stack.Pop();
            }
        }
        else
        {
            _stack = new Stack<EnemyObjectBase>();
            _enemyObjectsPool.Add(type, _stack);
        }
        switch ( type )
        {
            case EnemyObjectType.Fairy:
                enemyObj = new Fairy();
                break;
            case EnemyObjectType.SpinningEnemy:
                enemyObj = new SpinningEnemy();
                break;
        }
        return enemyObj;
    }

    public void RestoreEnemyObjectToPool(EnemyObjectBase enemyObject)
    {
        if ( enemyObject != null )
        {
            enemyObject.Clear();
            Stack<EnemyObjectBase> _stack;
            if (_enemyObjectsPool.TryGetValue(enemyObject.GetObjectType(), out _stack))
            {
                _stack.Push(enemyObject);
            }
            else
            {
                Logger.Log("Restore EnemyObject Fail!Reason : stack of type " + enemyObject.GetObjectType() + " is not found!");
            }
        }
    }
}
