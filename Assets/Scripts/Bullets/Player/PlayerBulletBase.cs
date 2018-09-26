using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBulletBase : BulletBase
{
    protected GameObject _bullet;
    protected Transform _trans;
    protected SpriteRenderer _renderer;
    protected string _prefabName;
    /// <summary>
    /// 子机子弹配置
    /// </summary>
    protected PlayerBulletCfg _bulletCfg;

    public override void Init()
    {
        base.Init();
    }


    public override void Update()
    {

    }

    public virtual void ChangeStyleById(string id)
    {

    }

    protected virtual void UpdatePos()
    {
        _trans.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
    }

    public override void Clear()
    {
        _destroyFlag = 0;
        _clearFlag = 0;
        _trans = null;
        _bullet = null;
        _renderer = null;
        base.Clear();
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
