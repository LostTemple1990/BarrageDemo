﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsManager
{
    private static BulletsManager _instance;

    public static BulletsManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new BulletsManager();
        }
        return _instance;
    }

    private BulletsManager()
    {

    }

    /// <summary>
    /// 玩家的子弹
    /// </summary>
    private List<PlayerBulletBase> _playerBullets;
    /// <summary>
    /// 待清除的子弹列表
    /// </summary>
    private List<BulletBase> _clearList;

    private List<EnemyBulletBase> _enemyBullets;

    private Dictionary<string, IParser> _enemyDefaultBulletDataBase;
    private Dictionary<string, IParser> _playerBulletDataBase;
    private Dictionary<string, IParser> _enemyLinearLaserDataBase;

    public void Init()
    {
        _playerBullets = new List<PlayerBulletBase>();
        _enemyBullets = new List<EnemyBulletBase>();
        _enemyBullets.Capacity = 5000;
        _clearList = new List<BulletBase>();
        _enemyDefaultBulletDataBase = DataManager.GetInstance().GetDatasByName("EnemyBulletDefaultCfgs") as Dictionary<string, IParser>;
        _playerBulletDataBase = DataManager.GetInstance().GetDatasByName("PlayerBulletCfgs") as Dictionary<string, IParser>;
        _enemyLinearLaserDataBase = DataManager.GetInstance().GetDatasByName("EnemyLinearLaserCfgs") as Dictionary<string, IParser>;
    }

    public bool RegisterPlayerBullet(PlayerBulletBase bullet)
    {
        if ( bullet == null )
        {
            return false;
        }
        _playerBullets.Add(bullet);
        return true;
    }

    public bool RegisterEnemyBullet(EnemyBulletBase bullet)
    {
        if (bullet == null)
        {
            return false;
        }
        _enemyBullets.Add(bullet);
        return true;
    }

    public void AddToClearList(BulletBase bullet)
    {
        _clearList.Add(bullet);
    }

    public void Update()
    {
        UpdatePlayerBullets();
        UpdateEnemyBullets();
        CheckDestroyBullets(_playerBullets);
        CheckDestroyBullets<EnemyBulletBase>(_enemyBullets);
    }

    private void UpdatePlayerBullets()
    {
        PlayerBulletBase tmpBullet;
        int tmpCount, i, j,findFlag;
        tmpCount = _playerBullets.Count;
        for (i = 0, j = 1; i < tmpCount; i++,j++)
        {
            findFlag = 1;
            if (_playerBullets[i] == null)
            {
                j = j == 1 ? i + 1 : j;
                findFlag = 0;
                for (; j < tmpCount; j++)
                {
                    if (_playerBullets[j] != null)
                    {
                        _playerBullets[i] = _playerBullets[j];
                        _playerBullets[j] = null;
                        findFlag = 1;
                        break;
                    }
                }
            }
            tmpBullet = _playerBullets[i];
            if (tmpBullet != null)
            {
                tmpBullet.Update();
            }
            if (findFlag == 0)
            {
                _playerBullets.RemoveRange(i, tmpCount - i);
                break;
            }
        }
    }

    private void UpdateEnemyBullets()
    {
        EnemyBulletBase tmpBullet;
        int tmpCount, i, j, findFlag;
        tmpCount = _enemyBullets.Count;
        //Logger.Log(tmpCount);
        for (i = 0, j = 1; i < tmpCount; i++, j++)
        {
            findFlag = 1;
            if (_enemyBullets[i] == null)
            {
                j = j == 1 ? i + 1 : j;
                findFlag = 0;
                for (; j < tmpCount; j++)
                {
                    if (_enemyBullets[j] != null)
                    {
                        _enemyBullets[i] = _enemyBullets[j];
                        _enemyBullets[j] = null;
                        findFlag = 1;
                        break;
                    }
                }
            }
            tmpBullet = _enemyBullets[i];
            if (tmpBullet != null)
            {
                tmpBullet.Update();
            }
            if (findFlag == 0)
            {
                _enemyBullets.RemoveRange(i, tmpCount - i);
                break;
            }
        }
    }

    private void CheckDestroyBullets<T>(List<T> list) 
        where T :BulletBase
    {
        BulletBase tmpBullet;
        int tmpCount, i;
        for (i = 0, tmpCount = list.Count; i < tmpCount; i++)
        {
            tmpBullet = list[i];
            if (tmpBullet.ClearFlag == 1)
            {
                list[i] = null;
                _clearList.Add(tmpBullet);
            }
        }
        tmpCount = _clearList.Count;
        for (i = 0; i < tmpCount; i++)
        {
            tmpBullet = _clearList[i];
            tmpBullet.Clear();
            ObjectsPool.GetInstance().RestoreBullet(tmpBullet);
        }
        _clearList.Clear();
    }

    private void CheckDestroyBullets()
    {
        BulletBase tmpBullet;
        int tmpCount, i;
        for (i=0,tmpCount=_playerBullets.Count;i<tmpCount; i++)
        {
            tmpBullet = _playerBullets[i];
            if ( tmpBullet.ClearFlag == 1 )
            {
                _playerBullets[i] = null;
                _clearList.Add(tmpBullet);
            }
        }
        tmpCount = _clearList.Count;
        for (i=0;i<tmpCount;i++)
        {
            tmpBullet = _clearList[i];
            tmpBullet.Clear();
            ObjectsPool.GetInstance().RestoreBullet(tmpBullet);
        }
        _clearList.Clear();
    }

    /// <summary>
    /// 获取敌机子弹配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EnemyBulletDefaultCfg GetBulletDefaultCfgById(string id)
    {
        IParser parser;
        if ( !_enemyDefaultBulletDataBase.TryGetValue(id,out parser) )
        {
            return null;
        }
        return parser as EnemyBulletDefaultCfg;
    }

    /// <summary>
    /// 获取玩家子弹配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public PlayerBulletCfg GetPlayerBulletCfgById(string id)
    {
        IParser parser;
        if (!_playerBulletDataBase.TryGetValue(id, out parser))
        {
            return null;
        }
        return parser as PlayerBulletCfg;
    }

    /// <summary>
    /// 获取敌机激光配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EnemyLinearLaserCfg GetLinearLaserCfgById(string id)
    {
        IParser parser;
        if (!_enemyLinearLaserDataBase.TryGetValue(id, out parser))
        {
            return null;
        }
        return parser as EnemyLinearLaserCfg;
    }

    public List<EnemyBulletBase> GetEnemyBulletList()
    {
        return _enemyBullets;
    }

    public void ClearEnemyBulletsInRange(Vector2 center,float radius)
    {
        Vector2 bulletPos;
        EnemyBulletBase bullet;
        int tmpCount = _enemyBullets.Count;
        for (int i=0;i<tmpCount;i++)
        {
            bullet = _enemyBullets[i];
            if ( bullet != null && bullet.ClearFlag == 0 )
            {
                bulletPos = bullet.CurPos;
                if ( new Vector2(center.x-bulletPos.x,center.y-bulletPos.y).magnitude <= radius )
                {
                    bullet.Eliminate();
                }
            }
        }
    }

    /// <summary>
    /// 获取子弹的Go
    /// </summary>
    /// <param name="bulletId"></param>
    /// <param name="textureId"></param>
    /// <returns></returns>
    public GameObject CreateBulletGameObject(BulletId type,int bulletId)
    {
        string textureName = "Bullet" + bulletId;
        GameObject prefab = ObjectsPool.GetInstance().GetPrefabAtPool(bulletId.ToString());
        if ( prefab == null )
        {
            // 缓存中没有，先取原型
            GameObject protoType = ObjectsPool.GetInstance().GetBulletProtoType(bulletId);
            // 若不存在原型，则先创建一个原型到缓存池中
            if ( protoType == null )
            {
                protoType = CreateProtoType(type, bulletId);
            }
            prefab = GameObject.Instantiate<GameObject>(protoType);
            AddBulletGoToLayer(type, prefab);
            //UIManager.GetInstance().AddGoToLayer(prefab, LayerId.EnemyBarrage);
        }
        return prefab;
    }

    private GameObject CreateProtoType(BulletId type,int bulletId)
    {
        GameObject protoType = null;
        switch ( type )
        {
            case BulletId.Enemy_Simple:
                protoType = CreateEnemySimpleProtoType(bulletId);
                break;
            case BulletId.Player_Simple:
                protoType = CreatePlayerSimpleBulletProtoType(bulletId);
                break;
            case BulletId.Enemy_LinearLaser:
                protoType = CreateEnemyLinearLaserProtoType(bulletId);
                break;
        }
        return protoType;
    }

    /// <summary>
    /// 根据Id创建SimpleBullet原型
    /// </summary>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private GameObject CreateEnemySimpleProtoType(int bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/SimpleBullet");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        string textureName = "Bullet" + bulletId;
        // 设置sprite以及material
        protoType.name = textureName;
        SpriteRenderer sp = protoType.transform.Find("BulletSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, textureName);
        eBlendMode blendMode = (eBlendMode)(bulletId % 10);
        if (blendMode != eBlendMode.Normal)
        {
            sp.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
        }
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.EnemyBarrage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddBulletProtoType(bulletId, protoType);
        // 创建缓存stack
        return protoType;
    }

    /// <summary>
    /// 创建EnemyLaser的原型
    /// </summary>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private GameObject CreateEnemyLaserProtoType(int bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/Laser");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        string textureName = "Bullet" + bulletId;
        // 设置sprite以及material
        protoType.name = textureName;
        SpriteRenderer sp = protoType.transform.Find("LaserSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, textureName);
        eBlendMode blendMode = (eBlendMode)(bulletId % 10);
        if (blendMode != eBlendMode.Normal)
        {
            sp.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
        }
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.EnemyBarrage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddBulletProtoType(bulletId, protoType);
        return protoType;
    }

    private GameObject CreateEnemyLinearLaserProtoType(int bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/LinearLaser");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        EnemyLinearLaserCfg cfg = GetLinearLaserCfgById(bulletId.ToString());
        string textureName = "Bullet" + bulletId;
        // 激光发射源
        SpriteRenderer sourceSp = protoType.transform.Find("Source").GetComponent<SpriteRenderer>();
        sourceSp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, "Bullet" + cfg.laserSourceTexId);
        // 设置sprite以及material
        protoType.name = textureName;
        // 激光段tf
        Transform segmentTf = protoType.transform.Find("Segment");
        // 激光本体
        SpriteRenderer sp = segmentTf.Find("LaserSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, cfg.laserTexName);
        eBlendMode blendMode = (eBlendMode)(bulletId % 10);
        if (blendMode != eBlendMode.Normal)
        {
            sp.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
        }
        // 设置尺寸为0
        sp.size = Vector2.zero;
        // 激光头部
        sp = segmentTf.Find("HeadSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, cfg.laserHeadTexName);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddBulletProtoType(bulletId, protoType);
        return protoType;
    }

    /// <summary>
    /// 创建PlayerSimpleBullet原型
    /// </summary>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private GameObject CreatePlayerSimpleBulletProtoType(int bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/SimpleBullet");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        // 读取配置
        PlayerBulletCfg cfg = BulletsManager.GetInstance().GetPlayerBulletCfgById(bulletId.ToString());
        // 设置sprite以及material
        protoType.name = cfg.textureName;
        SpriteRenderer sp = protoType.transform.Find("BulletSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(cfg.packName, cfg.textureName);
        eBlendMode blendMode = (eBlendMode)(bulletId % 10);
        if (blendMode != eBlendMode.Normal)
        {
            sp.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
        }
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.PlayerBarage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddBulletProtoType(bulletId, protoType);
        return protoType;
    }

    /// <summary>
    /// 将子弹Go添加到对应层级上
    /// </summary>
    /// <param name="type"></param>
    /// <param name="prefab"></param>
    private void AddBulletGoToLayer(BulletId type,GameObject prefab)
    {
        switch ( type )
        {
            case BulletId.Enemy_Simple:
            case BulletId.Enemy_Laser:
            case BulletId.Enemy_LinearLaser:
            case BulletId.Enemy_CurveLaser:
                UIManager.GetInstance().AddGoToLayer(prefab, LayerId.EnemyBarrage);
                break;
            case BulletId.Player_Simple:
            case BulletId.Player_Laser:
                UIManager.GetInstance().AddGoToLayer(prefab, LayerId.PlayerBarage);
                break;
        }
    }

    public void ClearAllEnemyBullets()
    {
        EnemyBulletBase bullet;
        int tmpCount = _enemyBullets.Count;
        for (int i = 0; i < tmpCount; i++)
        {
            bullet = _enemyBullets[i];
            if (bullet != null && bullet.ClearFlag == 0)
            {
                bullet.Eliminate();
            }
        }
    }

    public void Clear()
    {
        int tmpCount, i;
        BulletBase bullet;
        // 己方子弹
        tmpCount = _playerBullets.Count;
        for (i=0;i<tmpCount;i++)
        {
            bullet = _playerBullets[i];
            if ( bullet != null )
            {
                bullet.Clear();
                ObjectsPool.GetInstance().RestoreBullet(bullet);
            }
        }
        _playerBullets.Clear();
        // 敌方子弹
        tmpCount = _enemyBullets.Count;
        for (i = 0; i < tmpCount; i++)
        {
            bullet = _enemyBullets[i];
            if (bullet != null)
            {
                bullet.Clear();
                ObjectsPool.GetInstance().RestoreBullet(bullet);
            }
        }
        _enemyBullets.Clear();
    }
}
