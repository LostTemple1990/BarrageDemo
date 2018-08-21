using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWeaponBase
{
    /// <summary>
    ///  标识是第几个副武器
    /// </summary>
    protected int _subIndex;
    /// <summary>
    /// 所属角色
    /// </summary>
    protected CharacterBase _character;

    protected GameObject _subWeapon;

    protected SpriteRenderer _subWeaponRenderer;

    protected Vector3[][] _posOffset;

    protected int _curShootCD;

    protected int _moveMode;

    public virtual void Init(GameObject go,CharacterBase character)
    {
        _subWeapon = go;
        _character = character;
    }

    public virtual void Update(int moveMode)
    {

    }

    /// <summary>
    /// 射击间隔
    /// </summary>
    protected virtual int ShootCD
    {
        get { return 12; }
    }

    /// <summary>
    /// 生成的子弹id
    /// </summary>
    protected virtual BulletId BulletId
    {
        get { return 0; }
    }

    public int SubWeaponIndex
    {
        set { _subIndex = value; }
    }

    public void SetActive(bool value)
    {
        _subWeapon.SetActive(value);
    }

    public virtual void Clear()
    {
        _character = null;
        _subWeapon = null;
        _subWeaponRenderer = null;
    }
}
