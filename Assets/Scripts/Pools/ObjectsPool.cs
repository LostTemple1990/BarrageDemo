using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Dictionary<BulletId, Stack<BulletBase>> _bulletsPool;
    private Dictionary<string, Stack<GameObject>> _bulletPrefabsPool;
    private Dictionary<string, Stack<IPoolClass>> _dataClassPool;
    /// <summary>
    /// 子弹原型字典
    /// </summary>
    private Dictionary<int, GameObject> _bulletProtoTypeDic;

    private ObjectsPool()
    {
        Init();
    }

    private void Init()
    {
        _bulletsPool = new Dictionary<BulletId, Stack<BulletBase>>();
        _bulletPrefabsPool = new Dictionary<string, Stack<GameObject>>();
        _dataClassPool = new Dictionary<string, Stack<IPoolClass>>();
        _bulletProtoTypeDic = new Dictionary<int, GameObject>();
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
                case BulletId.ReimuA_Sub1:
                    bullet = new BulletReimuASub1();
                    break;
                case BulletId.Player_Laser:
                    bullet = new PlayerLaser();
                    break;
                case BulletId.Player_Simple:
                    bullet = new PlayerBulletSimple();
                    break;
                case BulletId.Enemy_Laser:
                    bullet = new EnemyLaser();
                    break;
                case BulletId.Enemy_CurveLaser:
                    bullet = new EnemyCurveLaser();
                    break;
                case BulletId.Enemy_Simple:
                    bullet = new EnemyBulletSimple();
                    break;
                case BulletId.Enemy_LinearLaser:
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

    /// <summary>
    /// 根据子弹贴图id获取贴图原型prefab
    /// </summary>
    /// <param name="textureId"></param>
    /// <returns></returns>
    public GameObject GetBulletProtoType(int textureId)
    {
        GameObject protoType;
        if ( _bulletProtoTypeDic.TryGetValue(textureId,out protoType) )
        {
            return protoType;
        }
        return null;
    }

    /// <summary>
    /// 添加子弹原型到缓存中
    /// </summary>
    /// <param name="textureId"></param>
    /// <param name="protoType"></param>
    public void AddBulletProtoType(int textureId,GameObject protoType)
    {
        try
        {
            _bulletProtoTypeDic.Add(textureId, protoType);
        }
        catch
        {
            throw new System.Exception("Add ProtoType to dic fail! TextureId = " + textureId);
        }
    }

    /// <summary>
    /// 检测是否需要销毁额外的对象
    /// </summary>
    public bool CheckDestroyPoolObjects()
    {
        var keys0 = _bulletPrefabsPool.Keys;
        // 检测顺序
        // 1.子弹GameObject
        foreach(string key in keys0)
        {
            Stack<GameObject> stack;
            GameObject go;
            if ( _bulletPrefabsPool.TryGetValue(key,out stack) )
            {
                int stackCount = stack.Count;
                if (stackCount > 0 )
                {
                    int count = 0;
                    while ( count < Consts.MaxDestroyPrefabCountPerFrame && stackCount > 0 )
                    {
                        go = stack.Pop();
                        GameObject.Destroy(go);
                        count++;
                        stackCount--;
                    }
                    return true;
                }
            }
        }
        // 2.子弹类
        var keys1 = _bulletsPool.Keys;
        foreach (BulletId id in keys1)
        {
            Stack<BulletBase> stack;
            if (_bulletsPool.TryGetValue(id, out stack))
            {
                int stackCount = stack.Count;
                if (stackCount > 0)
                {
                    int count = 0;
                    while (count < Consts.MaxDestroyClassCountPerFrame && stackCount > 0)
                    {
                        stack.Pop();
                        count++;
                        stackCount--;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 清除子弹的原型缓存
    /// </summary>
    public void DestroyBulletProtoTypes()
    {
        var keys = _bulletProtoTypeDic.Keys;
        GameObject protoType;
        foreach (int id in keys)
        {
            if ( _bulletProtoTypeDic.TryGetValue(id,out protoType) )
            {
                GameObject.Destroy(protoType);
            }
        }
        _bulletProtoTypeDic.Clear();
    }
}

public enum PoolObjectType :int
{

}
