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

    private float _laserTotalLen;
    /// <summary>
    /// 当前射线长度
    /// </summary>
    private float _curLaserLen;
    /// <summary>
    /// 上一帧射线的长度，未赋值之前为-1
    /// </summary>
    private float _preLaserLen;

    public PlayerLaser()
    {
        _prefabName = "PlayerLaser";
    }

    public override void Init()
    {
        base.Init();
        _laserTf = _trans.Find("Laser");
        _laserSr = _laserTf.GetComponent<SpriteRenderer>();
        UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.PlayerBarage);
        _trans.localRotation = Quaternion.Euler(0f, 0f, 90f);
        BulletsManager.GetInstance().RegisterPlayerBullet(this);
        _isCached = false;
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
        _laserSr.sprite = ResourceManager.GetInstance().GetSprite(packName, texName);
        _texWidth = texWidth;
        _repeatCount = repeatCount;
    }

    public void SetAngle(float angle)
    {
        _curAngle = angle;
        // 角度
        _laserTf.rotation = Quaternion.Euler(0, 0, _curAngle);
    }

    public override void Update()
    {
        if (!_isCached) Cache();
        CheckHitEnemy();
        if ( _preLaserLen != _curLaserLen )
        {
            RenderLaser();
        }
    }

    private void Cache()
    {
        _laserTotalLen = _texWidth * _repeatCount;
        _preLaserLen = -1;
        _curLaserLen = _laserTotalLen;
        // shader赋值
        _laserSr.material.SetFloat("_TotalLen", _texWidth);
        //_laserSr.material.SetFloat("_CurLen", _curLaserLen);
        _laserSr.material.SetFloat("_RepeatCount", _repeatCount);
        _isCached = true;
    }

    public override void CheckHitEnemy()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int enemyCount = enemyList.Count;
        EnemyBase enemy;
        CollisionDetectParas collDetParas;
        // 遍历所有敌机，检测激光最先碰到的敌机
        for (int i=0;i<enemyCount;i++)
        {
            enemy = enemyList[i];
            if ( enemy != null && enemy.CanHit() )
            {
                collDetParas = enemy.GetCollisionDetectParas();
            }
        }
    }

    private void RenderLaser()
    {
        _laserSr.material.SetFloat("_CurLen", _curLaserLen);
        _preLaserLen = _curLaserLen;
    }

    public override int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        arg1 = _curPos.x;
        arg2 = _curPos.y;
        arg3 = 8;
        arg4 = 8;
        return Consts.CollisionType_Rect;
    }

    protected override int GetDamage()
    {
        return 2;
    }

    public override void Clear()
    {
        _laserSr.sprite = null;
        _laserSr = null;
        _laserTf = null;
        base.Clear();
    }
}
