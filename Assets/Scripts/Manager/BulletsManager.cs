using System.Collections;
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

    public void Init()
    {
        _playerBullets = new List<PlayerBulletBase>();
        _enemyBullets = new List<EnemyBulletBase>();
        _enemyBullets.Capacity = 5000;
        _clearList = new List<BulletBase>();
        _enemyDefaultBulletDataBase = DataManager.GetInstance().GetDatasByName("EnemyBulletDefaultCfgs") as Dictionary<string, IParser>;
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

    public EnemyBulletDefaultCfg GetBulletDefaultCfgById(string id)
    {
        IParser parser;
        if ( !_enemyDefaultBulletDataBase.TryGetValue(id,out parser) )
        {
            return null;
        }
        return parser as EnemyBulletDefaultCfg;
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
        int tmpCount, i, j;
        BulletBase bullet;
        // 己方子弹
        tmpCount = _playerBullets.Count;
        for (i=0;i<tmpCount;i++)
        {
            bullet = _playerBullets[i];
            if ( bullet != null )
            {
                bullet.Clear();
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
            }
        }
        _enemyBullets.Clear();
    }
}
