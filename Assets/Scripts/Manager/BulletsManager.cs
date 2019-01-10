using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletsManager : ICommand
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
    private int _playerBulletsCount;
    /// <summary>
    /// 待清除的子弹列表
    /// </summary>
    private List<BulletBase> _clearList;
    /// <summary>
    /// 敌机子弹列表
    /// </summary>
    private List<EnemyBulletBase> _enemyBullets;
    /// <summary>
    /// 敌机子弹计数
    /// </summary>
    private int _enemyBulletsCount;

    private Dictionary<string, IParser> _enemyDefaultBulletDataBase;
    private Dictionary<string, IParser> _playerBulletDataBase;
    private Dictionary<string, IParser> _enemyLinearLaserDataBase;
    private Dictionary<string, IParser> _enemyCurveLaserDataBase;

    #region 子弹生成统计数据相关
    // 一帧中创建的子弹数目
    private int _countOfPlayerBulletCreatedInFrame;
    private int _countOfEnemyBulletCreatedInFrame;
    #endregion

    public void Init()
    {
        _playerBullets = new List<PlayerBulletBase>();
        _playerBulletsCount = 0;
        _enemyBullets = new List<EnemyBulletBase>();
        _enemyBulletsCount = 0;
        _enemyBullets.Capacity = 5000;
        _clearList = new List<BulletBase>();
        _enemyDefaultBulletDataBase = DataManager.GetInstance().GetDatasByName("EnemyBulletDefaultCfgs") as Dictionary<string, IParser>;
        _playerBulletDataBase = DataManager.GetInstance().GetDatasByName("PlayerBulletCfgs") as Dictionary<string, IParser>;
        _enemyLinearLaserDataBase = DataManager.GetInstance().GetDatasByName("EnemyLinearLaserCfgs") as Dictionary<string, IParser>;
        _enemyCurveLaserDataBase = DataManager.GetInstance().GetDatasByName("EnemyCurveLaserCfgs") as Dictionary<string, IParser>;
        CommandManager.GetInstance().Register(CommandConsts.STGFrameStart, this);
        CommandManager.GetInstance().Register(CommandConsts.LogFrameStatistics, this);
    }

    public bool RegisterPlayerBullet(PlayerBulletBase bullet)
    {
        if ( bullet == null )
        {
            return false;
        }
        _playerBullets.Add(bullet);
        _playerBulletsCount++;
        _countOfPlayerBulletCreatedInFrame++;
        return true;
    }

    public bool RegisterEnemyBullet(EnemyBulletBase bullet)
    {
        if (bullet == null)
        {
            return false;
        }
        _enemyBullets.Add(bullet);
        _enemyBulletsCount++;
        _countOfEnemyBulletCreatedInFrame++;
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
        tmpCount = _playerBulletsCount;
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
                _playerBulletsCount = i;
                break;
            }
        }
    }

    private void UpdateEnemyBullets()
    {
        EnemyBulletBase tmpBullet;
        int i, j, findFlag;
        //Logger.Log(tmpCount);
        for (i = 0, j = 1; i < _enemyBulletsCount; i++, j++)
        {
            findFlag = 1;
            if (_enemyBullets[i] == null)
            {
                j = j == 1 ? i + 1 : j;
                findFlag = 0;
                for (; j < _enemyBulletsCount; j++)
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
                _enemyBullets.RemoveRange(i, _enemyBulletsCount - i);
                _enemyBulletsCount = i;
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

    /// <summary>
    /// 获取曲线激光配置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public EnemyCurveLaserCfg GetCurveLaserCfgById(string id)
    {
        IParser parser;
        if (!_enemyCurveLaserDataBase.TryGetValue(id, out parser))
        {
            return null;
        }
        return parser as EnemyCurveLaserCfg;
    }

    /// <summary>
    /// 获取敌机子弹列表
    /// </summary>
    /// <returns></returns>
    public List<EnemyBulletBase> GetEnemyBulletList()
    {
        return _enemyBullets;
    }

    /// <summary>
    /// 获取敌机子弹计数
    /// </summary>
    /// <returns></returns>
    public int GetEnemyBulletsCount()
    {
        return _enemyBulletsCount;
    }

    /// <summary>
    /// 获取自机子弹列表
    /// </summary>
    /// <returns></returns>
    public List<PlayerBulletBase> GetPlayerBulletList()
    {
        return _playerBullets;
    }

    public int GetPlayerBulletsCount()
    {
        return _playerBulletsCount;
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
    public GameObject CreateBulletGameObject(BulletType type,string bulletId)
    {
        //string textureName = "Bullet" + bulletId;
        GameObject prefab = ObjectsPool.GetInstance().GetPrefabAtPool(bulletId);
        if ( prefab == null )
        {
            // 缓存中没有，先取原型
            GameObject protoType = ObjectsPool.GetInstance().GetProtoType(bulletId);
            // 若不存在原型，则先创建一个原型到缓存池中
            if ( protoType == null )
            {
                protoType = CreateBulletProtoType(type, bulletId);
            }
            prefab = GameObject.Instantiate<GameObject>(protoType);
            AddBulletGoToLayer(type, prefab);
            //UIManager.GetInstance().AddGoToLayer(prefab, LayerId.EnemyBarrage);
        }
        return prefab;
    }

    /// <summary>
    /// 根据子弹类型以及id获取子弹原型的名称
    /// </summary>
    /// <param name="type"></param>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private string GetProtoTypeNameByBulletTypeAndId(BulletType type,string bulletId)
    {
        if (type == BulletType.Enemy_Laser) return "EnemyLaser" + bulletId;
        if (type == BulletType.Enemy_LinearLaser) return "EnemyLinearLaser" + bulletId;
        if (type == BulletType.Enemy_CurveLaser) return "EnemyCurveLaser" + bulletId;
        return bulletId;
    }

    private GameObject CreateBulletProtoType(BulletType type,string bulletId)
    {
        GameObject protoType = null;
        switch ( type )
        {
            case BulletType.Enemy_Simple:
                protoType = CreateEnemySimpleProtoType(bulletId);
                break;
            case BulletType.Player_Simple:
                protoType = CreatePlayerSimpleBulletProtoType(bulletId);
                break;
            case BulletType.Enemy_LinearLaser:
                protoType = CreateEnemyLinearLaserProtoType(bulletId);
                break;
            case BulletType.Player_Laser:
                protoType = CreatePlayerLaserProtoType(bulletId);
                break;
            case BulletType.Enemy_Laser:
                protoType = CreateEnemyLaserProtoType(bulletId);
                break;
            case BulletType.Enemy_CurveLaser:
                protoType = CreateEnemyCurveLaserProtoType(bulletId);
                break;
        }
        return protoType;
    }

    /// <summary>
    /// 根据Id创建SimpleBullet原型
    /// </summary>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private GameObject CreateEnemySimpleProtoType(string bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/SimpleBullet");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        EnemyBulletDefaultCfg cfg = BulletsManager.GetInstance().GetBulletDefaultCfgById(bulletId);
        string protoTypeName = "EnemySimpleBullet" + bulletId;
        // 设置sprite以及material
        protoType.name = protoTypeName;
        SpriteRenderer sp = protoType.transform.Find("BulletSprite").GetComponent<SpriteRenderer>();
        if ( cfg.aniFrameCount == 0 )
        {
            sp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, cfg.spriteName);
        }
        else
        {
            sp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, cfg.spriteName + "_0");
        }
        if (cfg.blendMode != eBlendMode.Normal)
        {
            sp.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(cfg.blendMode);
        }
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.EnemyBarrage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddProtoType(bulletId, protoType);
        Logger.Log("Create Bullet ProtoType " + protoTypeName);
        return protoType;
    }

    /// <summary>
    /// 创建EnemyLaser的原型
    /// </summary>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private GameObject CreateEnemyLaserProtoType(string bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/Laser");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        // 读取配置
        EnemyLinearLaserCfg cfg = GetLinearLaserCfgById(bulletId);
        string protoTypeName = "EnemyLaser" + bulletId;
        // 设置sprite以及material
        protoType.name = protoTypeName;
        SpriteRenderer sp = protoType.transform.Find("LaserSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(cfg.laserAtlasName, cfg.laserTexName);
        if (cfg.blendMode != eBlendMode.Normal)
        {
            sp.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(cfg.blendMode);
        }
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.EnemyBarrage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddProtoType(bulletId, protoType);
        Logger.Log("Create Bullet ProtoType " + protoTypeName);
        return protoType;
    }

    private GameObject CreateEnemyLinearLaserProtoType(string bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/LinearLaser");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        // 读取配置
        EnemyLinearLaserCfg cfg = GetLinearLaserCfgById(bulletId);
        string protoTypeName = "EnemyLinearLaser" + bulletId;
        // 激光发射源
        SpriteRenderer sourceSp = protoType.transform.Find("Source").GetComponent<SpriteRenderer>();
        sourceSp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, "Bullet" + cfg.laserSourceTexId);
        // 设置sprite以及material
        protoType.name = protoTypeName;
        // 激光段tf
        Transform segmentTf = protoType.transform.Find("Segment");
        // 激光本体
        SpriteRenderer sp = segmentTf.Find("LaserSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(cfg.laserAtlasName, cfg.laserTexName);
        if ( cfg.blendMode != eBlendMode.Normal )
        {
            sp.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(cfg.blendMode);
        }
        // 设置尺寸为0
        sp.size = Vector2.zero;
        // 激光头部
        sp = segmentTf.Find("HeadSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, cfg.laserHeadTexName);
        // 将原型放置在敌机弹幕层
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.EnemyBarrage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddProtoType(bulletId, protoType);
        Logger.Log("Create Bullet ProtoType " + protoTypeName);
        return protoType;
    }

    /// <summary>
    /// 创建PlayerSimpleBullet原型
    /// </summary>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private GameObject CreatePlayerSimpleBulletProtoType(string bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/SimpleBullet");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        // 读取配置
        PlayerBulletCfg cfg = BulletsManager.GetInstance().GetPlayerBulletCfgById(bulletId.ToString());
        // 设置sprite以及material
        protoType.name = "PlayerSimpleBullet" + cfg.id;
        SpriteRenderer sp = protoType.transform.Find("BulletSprite").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(cfg.packName, cfg.textureName);
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.PlayerBarage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddProtoType(bulletId, protoType);
        Logger.Log("Create ProtoType " + protoType.name);
        return protoType;
    }

    /// <summary>
    /// 创建PlayerLaser的原型
    /// </summary>
    /// <param name="bulletId"></param>
    /// <returns></returns>
    private GameObject CreatePlayerLaserProtoType(string bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/PlayerLaser");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        // 读取配置
        PlayerBulletCfg cfg = BulletsManager.GetInstance().GetPlayerBulletCfgById(bulletId.ToString());
        // 设置sprite以及material
        protoType.name = "PlayerLaser " + cfg.id;
        SpriteRenderer sp = protoType.transform.Find("Laser").GetComponent<SpriteRenderer>();
        sp.sprite = ResourceManager.GetInstance().GetSprite(cfg.packName, cfg.textureName);
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.PlayerBarage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddProtoType(bulletId, protoType);
        Logger.Log("Create ProtoType " + protoType.name);
        return protoType;
    }

    private GameObject CreateEnemyCurveLaserProtoType(string bulletId)
    {
        GameObject original = Resources.Load<GameObject>("BulletPrefab/CurveLaser");
        GameObject protoType = GameObject.Instantiate<GameObject>(original);
        // 读取配置
        EnemyCurveLaserCfg cfg = GetCurveLaserCfgById(bulletId);
        string protoTypeName = "EnemyCurveLaser" + cfg.id;
        // 设置sprite以及material
        protoType.name = protoTypeName;
        // 设置曲线激光的材质
        Transform laserObjectTf = protoType.transform.Find("LaserObject");
        MeshRenderer meshRenderer = laserObjectTf.GetComponent<MeshRenderer>();
        if ( cfg.materialName != "CurveLaserMat000")
        {
            meshRenderer.material = Resources.Load<Material>("materials/" + cfg.materialName);
        }
        meshRenderer.sortingLayerName = "STG";
        // 将原型放置在敌机弹幕层
        UIManager.GetInstance().AddGoToLayer(protoType, LayerId.EnemyBarrage);
        // 添加原型到缓存池中
        ObjectsPool.GetInstance().AddProtoType(bulletId, protoType);
        Logger.Log("Create CurveLaser ProtoType " + protoTypeName);
        return protoType;
    }

    /// <summary>
    /// 将子弹Go添加到对应层级上
    /// </summary>
    /// <param name="type"></param>
    /// <param name="prefab"></param>
    private void AddBulletGoToLayer(BulletType type,GameObject prefab)
    {
        switch ( type )
        {
            case BulletType.Enemy_Simple:
            case BulletType.Enemy_Laser:
            case BulletType.Enemy_LinearLaser:
            case BulletType.Enemy_CurveLaser:
                UIManager.GetInstance().AddGoToLayer(prefab, LayerId.EnemyBarrage);
                break;
            case BulletType.Player_Simple:
            case BulletType.Player_Laser:
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
        int i;
        BulletBase bullet;
        // 己方子弹
        for (i=0;i< _playerBulletsCount; i++)
        {
            bullet = _playerBullets[i];
            if ( bullet != null )
            {
                bullet.Clear();
                ObjectsPool.GetInstance().RestoreBullet(bullet);
            }
        }
        _playerBullets.Clear();
        _playerBulletsCount = 0;
        // 敌方子弹
        for (i = 0; i < _enemyBulletsCount; i++)
        {
            bullet = _enemyBullets[i];
            if (bullet != null)
            {
                bullet.Clear();
                ObjectsPool.GetInstance().RestoreBullet(bullet);
            }
        }
        _enemyBullets.Clear();
        _enemyBulletsCount = 0;
    }

    public void Execute(int cmd, object[] data)
    {
        if ( cmd == CommandConsts.STGFrameStart )
        {
            ResetFrameStatistics();
        }
        else if ( cmd == CommandConsts.LogFrameStatistics )
        {
            LogFrameStatistics();
        }
    }

    private void ResetFrameStatistics()
    {
        _countOfPlayerBulletCreatedInFrame = 0;
        _countOfEnemyBulletCreatedInFrame = 0;
    }

    private void LogFrameStatistics()
    {
        Logger.Log("CountOfPlayerBulletCreated = " + _countOfPlayerBulletCreatedInFrame);
        Logger.Log("CountOfEnemyBulletCreated = " + _countOfEnemyBulletCreatedInFrame);
    }
}
