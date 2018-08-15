using System.Collections;
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
        }
        _trans = _bullet.GetComponent<RectTransform>();
        _renderer = _trans.Find("Bullet").GetComponent<SpriteRenderer>();
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
        _renderer = null;
        _destroyFlag = 0;
        _clearFlag = 0;
        UIManager.GetInstance().HideGo(_bullet);
        _trans = null;
        _bullet = null;
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

    /// <summary>
    /// 子弹对应的伤害
    /// </summary>
    /// <returns></returns>
    protected virtual float GetDamage()
    {
        throw new System.NotImplementedException();
    }
}
