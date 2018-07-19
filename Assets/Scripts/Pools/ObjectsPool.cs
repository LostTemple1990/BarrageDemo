using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// todo 改成ObjectPool
/// </summary>
public class ObjectsPool
{
    private static ObjectsPool _instance;

    public static ObjectsPool GetInstance()
    {
        if (_instance == null )
        {
            _instance = new ObjectsPool();
        }
        return _instance;
    }

    private List<PlayerBulletBase> _playerBullets;
    private List<PlayerBulletBase> _playerSubBullets;
    private List<int> _enemyBullets;

    private Dictionary<BulletId, Stack<BulletBase>> _bulletsPool;
    private Dictionary<string, Stack<GameObject>> _bulletPrefabsPool;
    private Dictionary<string, Stack<IPoolClass>> _dataClassPool;

    private ObjectsPool()
    {
        Init();
    }

    private void Init()
    {
        _playerBullets = new List<PlayerBulletBase>();
        _bulletsPool = new Dictionary<BulletId, Stack<BulletBase>>();
        _bulletPrefabsPool = new Dictionary<string, Stack<GameObject>>();
        _dataClassPool = new Dictionary<string, Stack<IPoolClass>>();
    }

    /// <summary>
    /// 根据子弹id创建对应的子弹
    /// </summary>
    /// <param name="bulletID"></param>
    /// <returns></returns>
    public BulletBase CreateBullet(BulletId bulletID)
    {
        BulletBase bullet = NewBullet(bulletID);
        if ( bullet != null )
        {
            bullet.Init();
        }
        return bullet;
    }

    public GameObject CreateBulletPrefab(string resName)
    {
        GameObject bullet = null;
        Stack<GameObject> _stack;
        if ( _bulletPrefabsPool.TryGetValue(resName, out _stack) )
        {
            if ( _stack.Count > 0 )
            {
                bullet = _stack.Pop();
            }
        }
        else
        {
            _stack = new Stack<GameObject>();
            _bulletPrefabsPool.Add(resName, _stack);
        }
        if ( bullet == null )
        {
            bullet = ResourceManager.GetInstance().GetPrefab("BulletPrefab", resName);
            UIManager.GetInstance().AddGoToLayer(bullet, LayerId.EnemyBarrage);
        }
        return bullet;
    }

    /// <summary>
    /// 从缓存池中取出prefab
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public GameObject GetPrefabAtPool(string prefabName)
    {
        GameObject prefab = null;
        Stack<GameObject> _stack;
        if (_bulletPrefabsPool.TryGetValue(prefabName, out _stack))
        {
            if (_stack.Count > 0)
            {
                prefab = _stack.Pop();
            }
        }
        else
        {
            _stack = new Stack<GameObject>();
            _bulletPrefabsPool.Add(prefabName, _stack);
        }
        return prefab;
    }


    public void RestorePrefabToPool(string prefabName,GameObject go)
    {
        Stack<GameObject> _stack;
        if ( !_bulletPrefabsPool.TryGetValue(prefabName,out _stack) )
        {
            Logger.Log("PrefabStack is not exist!PrefabName = " + prefabName);
            _stack = new Stack<GameObject>();
            _bulletPrefabsPool.Add(prefabName, _stack);
        }
        _stack.Push(go);
    }

    private BulletBase NewBullet(BulletId bulletID)
    {
        BulletBase bullet = null;
        Stack<BulletBase> _stack;
        // 判断对象池中是否有该类型的对象，如果有，则取出
        if ( _bulletsPool.TryGetValue(bulletID,out _stack))
        {
            if ( _stack.Count > 0 )
            {
                bullet = _stack.Pop();
            }
        }
        else
        {
            _stack = new Stack<BulletBase>();
            _bulletsPool.Add(bulletID, _stack);
        }
        // 如果对象池内没有该对象，则创建
        if ( bullet == null )
        {
            switch (bulletID)
            {
                case BulletId.BulletId_ReimuA_Main:
                    bullet = new BulletReimuAMain();
                    break;
                case BulletId.BulletId_ReimuA_Sub1:
                    bullet = new BulletReimuASub1();
                    break;
                case BulletId.BulletId_Enemy_Straight:
                    bullet = new EnemyBulletStraight();
                    break;
                case BulletId.BulletId_Enemy_Rebound:
                    bullet = new EnemyBulletRebound();
                    break;
                case BulletId.BulletId_Enemy_Curve:
                    bullet = new EnemyBulletCurve();
                    break;
                case BulletId.BulletId_Enemy_Laser:
                    bullet = new EnemyLaser();
                    break;
                case BulletId.BulletId_Enemy_CurveLaser:
                    bullet = new EnemyCurveLaser();
                    break;
                case BulletId.BulletId_Enemy_Simple:
                    bullet = new EnemyBulletSimple();
                    break;
                case BulletId.BulletId_Enemy_LinearLaser:
                    bullet = new EnemyLinearLaser();
                    break;
                default:
                    Logger.Log("Create Bullet Fail! Invalid BulledId!");
                    break;
            }
        }
        return bullet;
    }

    public void RestoreBullet(BulletBase bullet)
    {
        BulletId id = bullet.Id;
        Stack<BulletBase> _stack;
        if ( _bulletsPool.TryGetValue(id,out _stack) )
        {
            _stack.Push(bullet);
        }
        else
        {
            Logger.Log("Pool TryGetValue fail! BulletId = " + id);
        }
    }

    public void RestoreBullet(string resName,GameObject bullet)
    {
        Stack<GameObject> _stack;
        if (_bulletPrefabsPool.TryGetValue(resName, out _stack))
        {
            _stack.Push(bullet);
        }
        else
        {
            Logger.Log("Pool TryGetValue fail!");
        }
    }

    public T GetPoolClassAtPool<T>() where T:class,IPoolClass,new()
    {
        string className = typeof(T).ToString();
        Stack<IPoolClass> _stack;
        IPoolClass poolCls = null;
        if (_dataClassPool.TryGetValue(className, out _stack))
        {
            if ( _stack.Count > 0 )
            {
                poolCls = _stack.Pop();
            }
        }
        else
        {
            _stack = new Stack<IPoolClass>();
            _dataClassPool.Add(className, _stack);
        }
        if ( poolCls == null )
        {
            poolCls = new T();
        }
        return poolCls as T;
    }

    public void RestorePoolClassToPool<T>(T poolCls) where T : class, IPoolClass
    {
        if (poolCls == null)
        {
            return;
        }
        string className = typeof(T).ToString();
        Stack<IPoolClass> _stack;
        if (_dataClassPool.TryGetValue(className, out _stack))
        {
            poolCls.Clear();
            _stack.Push(poolCls);
        }
    }
}

public enum PoolObjectType :int
{

}
