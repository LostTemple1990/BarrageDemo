using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWeaponReimuA : SubWeaponBase
{
    protected static Vector3 RotateEuler = new Vector3(0f, 0f, 2f);

    protected int _powerLevel;

    protected float[][] _shootAngles;

    public override void Init(GameObject go,CharacterBase character)
    {
        base.Init(go,character);
        _posOffset = new Vector3[4][];
        _posOffset[0] = new Vector3[] { new Vector3(0f,32f,0) };
        _posOffset[1] = new Vector3[] { new Vector3(-12f, 32f, 0), new Vector3(12f, 32f, 0) };
        _posOffset[2] = new Vector3[] { new Vector3(-32f, 0f, 0), new Vector3(0f, 32f, 0), new Vector3(32f, 0f, 0) };
        _posOffset[3] = new Vector3[] { new Vector3(-32f, 0f, 0), new Vector3(-12f, 32f, 0), new Vector3(12f, 32f, 0), new Vector3(32f, 0f, 0) };
        _shootAngles = new float[4][];
        _shootAngles[0] = new float[] { 90f };
        _shootAngles[1] = new float[] { 135f, 45f };
        _shootAngles[2] = new float[] { 135f, 90f, 45f };
        _shootAngles[3] = new float[] { 135f, 90f, 90f, 45f };
        _curShootCD = 0;
        _subWeaponRenderer = _subWeapon.GetComponent<SpriteRenderer>();
        _subWeaponRenderer.sprite = ResourceManager.GetInstance().GetSprite(Consts.ReimuAtlasName, "pl00_21");
    }

    public override void Update()
    {
        _moveMode = _character.CurModeMode;
        _powerLevel = PlayerService.GetInstance().GetPower() / 100;
        UpdatePos();
        Shoot();
        UpdateAni();
    }

    protected void UpdatePos()
    {
        _subWeapon.transform.localPosition = _posOffset[_powerLevel-1][_subIndex];
    }

    protected void Shoot()
    {
        // 更新CD
        if ( _curShootCD > 0 )
        {
            _curShootCD--;
        }
        if ( _character.CanShoot() && _character.IsInShootingStatus() )
        {
            if ( _curShootCD == 0 )
            {
                float posX, posY;
                _character.GetPosition(out posX, out posY);
                BulletReimuASub1 bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.ReimuA_Sub1) as BulletReimuASub1;
                bullet.ChangeStyleById("501000");
                bullet.SetPosition(posX + _subWeapon.transform.localPosition.x, posY + _subWeapon.transform.localPosition.y);
                bullet.DoMove(8,GetShootBulletAngle());
                _curShootCD = ShootCD;
            }
        }
    }

    protected void UpdateAni()
    {
        _subWeapon.transform.Rotate(RotateEuler);
    }

    /// <summary>
    /// 根据副武器id获取发射的子弹的角度
    /// </summary>
    /// <returns></returns>
    private float GetShootBulletAngle()
    {
        float angle;
        if ( _moveMode == ePlayerMoveMode.LowSpeed )
        {
            angle = 90f;
        }
        else
        {
            return _shootAngles[_powerLevel-1][_subIndex];
        }
        return angle;
    }

    /// <summary>
    /// 射击间隔
    /// </summary>
    protected override int ShootCD
    {
        get { return 12; }
    }

    protected override BulletType BulletId
    {
        get
        {
            return BulletType.ReimuA_Sub1;
        }
    }
}
