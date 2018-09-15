﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBulletBase : BulletBase
{
    protected GameObject _bullet;
    protected RectTransform _trans;
    protected SpriteRenderer _renderer;
    protected string _prefabName;

    public override void Init()
    {
        base.Init();
        if ( _bullet == null )
        {
            _bullet = ResourceManager.GetInstance().GetPrefab("Prefab/PlayerBullets",_prefabName);
            _trans = _bullet.GetComponent<RectTransform>();
        }
    }

    public override void Update()
    {

    }

    protected virtual void UpdatePos()
    {
        _trans.localPosition = _curPos;
    }

    public override void Clear()
    {
        _destroyFlag = 0;
        _clearFlag = 0;
        UIManager.GetInstance().HideGo(_bullet);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        _trans = null;
        _bullet = null;
        _renderer = null;
        base.Clear();
    }

    public virtual void CheckHitEnemy()
    {
        EnemyBase hitEnemy = EnemyManager.GetInstance().GetHitEnemy(this);
        if ( hitEnemy != null )
        {
            hitEnemy.GetHit(GetDamage());
            _clearFlag = 1;
            //TODO 炸弹撞到单位的特效
        }
    }

    public virtual void Eliminate()
    {
        _clearFlag = 1;
    }

    public BulletId id
    {
        set { _id = value; }
    }

    /// <summary>
    /// 子弹对应的伤害
    /// </summary>
    /// <returns></returns>
    protected virtual int GetDamage()
    {
        throw new System.NotImplementedException();
    }
}
