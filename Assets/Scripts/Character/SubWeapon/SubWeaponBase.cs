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

    protected Transform _subTf;

    protected SpriteRenderer _subWeaponRenderer;

    protected Vector3[][] _posOffset;

    protected int _curShootCD;

    protected int _moveMode;
    /// <summary>
    /// 当前与自机的位置偏移
    /// </summary>
    protected Vector3 _curPos;

    public virtual void Init(GameObject go,CharacterBase character)
    {
        _subWeapon = go;
        _subTf = go.transform;
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

    public void SetToPosition(Vector2 pos)
    {
        _curPos = pos;
        _subTf.localPosition = pos;
    }

    public Vector2 GetPosition()
    {
        return _subTf.position;
    }

    public virtual void Clear()
    {
        _character = null;
        _subWeapon = null;
        _subTf = null;
        _subWeaponRenderer = null;
    }
}
