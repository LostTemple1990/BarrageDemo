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
        _enemyList = new List<EnemyBase>();
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

    private List<EnemyBase> _enemyList;
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

    public EnemyBase CreateEnemyByType(EnemyType type)
    {
        EnemyBase enemy = NewEnemy(type);
        RegisterEnemy(enemy);
        enemy.Init();
        return enemy;
    }

    /// <summary>
    /// 从对象池中or新创建enemyBase
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
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
        if (enemy == null) return;
        EnemyType enemyType = enemy.GetEnemyType();
        Stack<EnemyBase> stack;
        if ( _enemiesPool.TryGetValue(enemyType, out stack) )
        {
            enemy.Clear();
            stack.Push(enemy);
        }
        else
        {
            Logger.Log("Restore Enemy Fail!Reason : stack of type " + enemyType + " is not found!");
        }
    }

    public bool RegisterEnemy(EnemyBase enemy)
    {
        _enemyList.Add(enemy);
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
        return _enemyList;
    }

    private void UpdateEnemies()
    {
        EnemyBase ememy;
        int tmpCount, i, j, findFlag;
        tmpCount = _enemyList.Count;
        for (i = 0, j = i + 1; i < tmpCount; i++, j++)
        {
            findFlag = 1;
            if (_enemyList[i] == null)
            {
                j = j == 1 ? i + 1 : j;
                findFlag = 0;
                for (; j < tmpCount; j++)
                {
                    if (_enemyList[j] != null)
                    {
                        _enemyList[i] = _enemyList[j];
                        _enemyList[j] = null;
                        findFlag = 1;
                        break;
                    }
                }
            }
            ememy = _enemyList[i];
            if (ememy != null)
            {
                ememy.Update();
            }
            if (findFlag == 0)
            {
                _enemyList.RemoveRange(i, tmpCount - i);
                break;
            }
        }
    }

    private void CheckClearEnemies()
    {
        EnemyBase enemy;
        int tmpCount, i;
        for (i = 0, tmpCount = _enemyList.Count; i < tmpCount; i++)
        {
            enemy = _enemyList[i];
            if (enemy.ClearFlag == 1)
            {
                _enemyList[i] = null;
                _clearList.Add(enemy);
            }
        }
        tmpCount = _clearList.Count;
        for (i = 0; i < tmpCount; i++)
        {
            enemy = _clearList[i];
            RestoreEnemyToPool(enemy);
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

    /// <summary>
    /// todo 以后这部分全部存到InterpreterManager中
    /// </summary>
    /// <returns></returns>
    public Dictionary<string,BossRefData> GetAllBossRefData()
    {
        return _bossRefDatabase;
    }
    

    public void EliminateAllEnemyByCode(bool isIncludingBoss)
    {
        int tmpCount = _enemyList.Count;
        EnemyBase enemy;
        for (int i=0;i<tmpCount;i++)
        {
            enemy = _enemyList[i];
            if ( enemy != null && enemy.ClearFlag == 0 )
            {
                if ( enemy.Type != EnemyType.Boss || isIncludingBoss )
                {
                    enemy.Eliminate(eEliminateDef.CodeEliminate);
                }
            }
        }
    }

    public void RawEliminateAllEnemyByCode(bool isIncludingBoss)
    {
        int tmpCount = _enemyList.Count;
        EnemyBase enemy;
        for (int i = 0; i < tmpCount; i++)
        {
            enemy = _enemyList[i];
            if (enemy != null && enemy.ClearFlag == 0)
            {
                if (enemy.Type != EnemyType.Boss || isIncludingBoss)
                {
                    enemy.Eliminate(eEliminateDef.CodeRawEliminate);
                }
            }
        }
    }

    /// <summary>
    /// 创建EnemyObject
    /// </summary>
    /// <param name="type">类型</param>
    /// <returns></returns>
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

    /// <summary>
    /// 将enemyObject缓存回对象池
    /// </summary>
    /// <param name="enemyObject"></param>
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

    /// <summary>
    /// clear函数
    /// </summary>
    /// <param name="clearEnemyCache">是否清除敌机object的缓存</param>
    public void Clear(bool clearAllCache=true)
    {
        int i, len;
        EnemyBase enemy;
        len = _enemyList.Count;
        for (i=0;i<len;i++)
        {
            enemy = _enemyList[i];
            if ( enemy != null )
            {
                enemy.Clear();
            }
        }
        _enemyList.Clear();
        if (clearAllCache)
        {
            ClearEnemyCache();
            ClearEnemyObjectCache();
        }
    }

    /// <summary>
    /// 清除敌机的缓存
    /// </summary>
    private void ClearEnemyCache()
    {
        Stack<EnemyBase> stack;
        foreach (KeyValuePair<EnemyType, Stack<EnemyBase>> kv in _enemiesPool)
        {
            stack = kv.Value;
            if (stack != null)
            {
                stack.Clear();
            }
        }
        _enemiesPool.Clear();
    }

    /// <summary>
    /// 清除敌机对象的缓存
    /// </summary>
    private void ClearEnemyObjectCache()
    {
        Stack<EnemyObjectBase> stack;
        foreach (KeyValuePair<EnemyObjectType, Stack<EnemyObjectBase>> kv in _enemyObjectsPool)
        {
            stack = kv.Value;
            if (stack != null)
            {
                stack.Clear();
            }
        }
        _enemyObjectsPool.Clear();
    }

    private void ClearBossRefData()
    {

    }
}
