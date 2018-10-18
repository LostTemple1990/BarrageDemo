using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaser : PlayerBulletBase
{
    /// <summary>
    /// 是否已经生成贴图
    /// </summary>
    private bool _isCached;
    private Transform _laserTf;
    private SpriteRenderer _laserSr;

    private float _texWidth;
    private float _repeatCount;
    /// <summary>
    /// 射线的总长度，即贴图长度*重复次数
    /// </summary>
    private float _laserTotalLen;
    /// <summary>
    /// 当前射线长度
    /// </summary>
    private float _curLaserLen;
    /// <summary>
    /// 上一帧射线的长度，未赋值之前为-1
    /// </summary>
    private float _preLaserLen;
    /// <summary>
    /// 射线的方向向量
    /// </summary>
    private Vector2 _dirVec;

    public PlayerLaser()
    {
        _id = BulletId.Player_Laser;
        _prefabName = "PlayerLaser";
    }

    public override void Init()
    {
        base.Init();
        BulletsManager.GetInstance().RegisterPlayerBullet(this);
        _isCached = false;
    }

    public override void ChangeStyleById(string id)
    {
        if (_bullet != null)
        {
            UIManager.GetInstance().HideGo(_bullet);
            ObjectsPool.GetInstance().RestorePrefabToPool(_bulletCfg.textureName, _bullet);
        }
        _bulletCfg = BulletsManager.GetInstance().GetPlayerBulletCfgById(id);
        _prefabName = id;
        _bullet = BulletsManager.GetInstance().CreateBulletGameObject(BulletId.Player_Laser, int.Parse(id));
        _trans = _bullet.transform;
        _laserTf = _trans.Find("Laser");
        _laserSr = _laserTf.GetComponent<SpriteRenderer>();
        _laserSr.sprite = ResourceManager.GetInstance().GetSprite(_bulletCfg.packName, _bulletCfg.textureName);
        UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.PlayerBarage);
        _texWidth = 256;
        _repeatCount = 2f;
        _laserTf.localPosition = new Vector3(_texWidth * _repeatCount / 2, 0, 0);
    }

    /// <summary>
    /// 设置激光贴图的属性
    /// </summary>
    /// <param name="packName">包名</param>
    /// <param name="texName">贴图名称</param>
    /// <param name="texWidth">贴图的宽度，代表激光的长度</param>
    /// <param name="repeatCount">平铺的次数</param>
    public void SetTextureProps(string packName,string texName,float texWidth,float repeatCount)
    {

        _trans.localRotation = Quaternion.Euler(0f, 0f, 90f);
        _laserTf = _trans.Find("Laser");
        _laserSr = _laserTf.GetComponent<SpriteRenderer>();
        UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.PlayerBarage);
        _laserSr.sprite = ResourceManager.GetInstance().GetSprite(packName, texName);
        _texWidth = texWidth;
        _repeatCount = repeatCount;
    }

    public void SetAngle(float angle)
    {
        _curAngle = angle;
        // 角度
        _trans.rotation = Quaternion.Euler(0, 0, _curAngle);
        _dirVec = new Vector2(Mathf.Cos(_curAngle * Mathf.Deg2Rad), Mathf.Sin(_curAngle * Mathf.Deg2Rad));
    }

    public override void Update()
    {
        if (!_isCached) Cache();
        CheckHitEnemy();
        if ( _preLaserLen != _curLaserLen )
        {
            RenderLaser();
        }
        UpdatePos();
    }

    private void Cache()
    {
        _laserTotalLen = _texWidth * _repeatCount;
        _preLaserLen = -1;
        _curLaserLen = _laserTotalLen;
        // shader赋值
        _laserSr.material.SetFloat("_TotalLen", _texWidth);
        _laserSr.material.SetFloat("_RepeatCount", _repeatCount);
        _isCached = true;
    }

    public void CheckHitEnemy()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int enemyCount = enemyList.Count;
        EnemyBase enemy,hitEnemy = null;
        CollisionDetectParas collDetParas;
        // verticalDis 圆心到射线的垂直距离
        // angle 两项链的夹角
        // dis 从射线起始点指向敌机中心的向量的长度
        // minDis 击中敌人的最小距离,即射线第一个击中的敌人
        float verticalDis,angle,dis,minDis,horizonDis;
        minDis = _laserTotalLen;
        // vecA为射线发射点到敌机的向量
        Vector2 vecA;
        // 遍历所有敌机，检测激光最先碰到的敌机
        for (int i=0;i<enemyCount;i++)
        {
            enemy = enemyList[i];
            if ( enemy != null && enemy.CanHit() )
            {
                collDetParas = enemy.GetCollisionDetectParas();
                // 计算vecA
                vecA = collDetParas.centerPos - new Vector2(_curPos.x,_curPos.y);
                // 计算夹角
                angle = Vector2.Angle(_dirVec, vecA);
                if (angle > 90) continue;
                dis = vecA.magnitude;
                verticalDis = dis * Mathf.Sin(Mathf.Deg2Rad * angle);
                // 判断垂直距离
                if ( verticalDis <= collDetParas.halfWidth )
                {
                    horizonDis = dis * Mathf.Cos(Mathf.Deg2Rad * angle);
                    if ( horizonDis < minDis )
                    {
                        hitEnemy = enemy;
                        minDis = horizonDis;
                    }
                }
            }
        }
        // 如果有击中敌机
        if ( hitEnemy != null )
        {
            hitEnemy.GetHit(GetDamage());
        }
        _curLaserLen = minDis;
    }

    private void RenderLaser()
    {
        _laserSr.material.SetFloat("_CurLaserLen", _curLaserLen);
        //Logger.Log("Set CurLaserLen To " + _curLaserLen);
        _preLaserLen = _curLaserLen;
    }

    protected override int GetDamage()
    {
        return 2;
    }

    public override void Clear()
    {
        UIManager.GetInstance().HideGo(_bullet);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        _laserSr.sprite = null;
        _laserSr = null;
        _laserTf = null;
        base.Clear();
    }
}
