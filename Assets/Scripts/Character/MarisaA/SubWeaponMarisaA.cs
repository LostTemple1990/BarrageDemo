using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubWeaponMarisaA : SubWeaponBase
{
    private static Vector3 RotateEuler = new Vector3(0f, 0f, 2f);
    /// <summary>
    /// 星星特效的scale变化
    /// </summary>
    private const float ScaleDelta = 0.016f;

    protected float[][] _shootAngles;
    /// <summary>
    /// 当前是否正在射击
    /// </summary>
    private bool _isShooting;

    private Transform _subWeaponTf;
    private Transform _highLightBgTf;
    private Transform _highLightStarTf;

    private PlayerLaser _laser;
    /// <summary>
    /// 副武器闪光星星的scale
    /// <para>取值范围从0.5~1来回浮动</para>
    /// </summary>
    private float _curStarScale;
    /// <summary>
    /// 星星特效的sclae变化量
    /// </summary>
    private float _scaleDelta;

    public override void Init(GameObject go, CharacterBase character)
    {
        base.Init(go, character);
        _posOffset = new Vector3[4][];
        _posOffset[0] = new Vector3[] { new Vector3(0f, 32f, 0) };
        _posOffset[1] = new Vector3[] { new Vector3(-12f, 32f, 0), new Vector3(12f, 32f, 0) };
        _posOffset[2] = new Vector3[] { new Vector3(-32f, 0f, 0), new Vector3(0f, 32f, 0), new Vector3(32f, 0f, 0) };
        _posOffset[3] = new Vector3[] { new Vector3(-32f, 0f, 0), new Vector3(-12f, 32f, 0), new Vector3(12f, 32f, 0), new Vector3(32f, 0f, 0) };
        _shootAngles = new float[4][];
        _shootAngles[0] = new float[] { 90f };
        _shootAngles[1] = new float[] { 135f, 45f };
        _shootAngles[2] = new float[] { 135f, 90f, 45f };
        _shootAngles[3] = new float[] { 135f, 90f, 90f, 45f };
        _subWeaponTf = _subTf.Find("SubWeapon");
        _highLightBgTf = _subTf.Find("HighLightBg");
        _highLightStarTf = _subTf.Find("HighLightStar");
    }

    public override void Update(int moveMode)
    {
        Shoot();
        UpdateAni();
    }

    protected void Shoot()
    {
        if (_character.CanShoot() && _character.InputShoot)
        {
            int subAvailable = PlayerService.GetInstance().GetPower() / 100;
            if ( !_isShooting )
            {
                // 射出激光的特效
                ActiveShootingEffect(true);
                // 创建激光
                if ( _laser == null )
                {
                    _laser = ObjectsPool.GetInstance().CreateBullet(BulletId.Player_Laser) as PlayerLaser;
                    _laser.SetTextureProps(Consts.MarisaAtlasName, "MarisaALaser", 256, 2);
                }
                // 星星特效
                _curStarScale = 1;
                _scaleDelta = -ScaleDelta;
                _highLightStarTf.localScale = Vector3.one;
                // 设置碰撞
                _laser.SetDetectCollision(true);
            }
            // 更新激光的位置
            Vector3 playerPos = _character.GetPosition();
            Vector3 laserPos = playerPos + _curPos;
            _laser.SetToPosition(laserPos);
            // 更新激光角度
            int mode = _character.CurModeMode;
            float laserAngle = mode == Consts.SlowMove ? 90f : _shootAngles[subAvailable - 1][_subIndex];
            _laser.SetAngle(laserAngle);
            _isShooting = true;
        }
        else
        {
            if ( _isShooting )
            {
                ActiveShootingEffect(false);
                _laser.SetToPosition(new Vector3(2000, 2000, 0));
                _laser.SetDetectCollision(false);
            }
            _isShooting = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isActivated"></param>
    private void ActiveShootingEffect(bool isActivated)
    {
        if ( _isShooting != isActivated )
        {
            _highLightBgTf.gameObject.SetActive(isActivated);
            _highLightStarTf.gameObject.SetActive(isActivated);
        }
    }

    /// <summary>
    /// 更新子机的动画效果
    /// </summary>
    protected void UpdateAni()
    {
        _subWeapon.transform.Rotate(RotateEuler);
        if ( _isShooting )
        {
            _curStarScale += _scaleDelta;
            if ( _curStarScale > 1 )
            {
                _curStarScale = 1 - (_curStarScale - 1);
                _scaleDelta = -ScaleDelta;
            }
            else if ( _curStarScale < 0.5f )
            {
                _curStarScale = 0.5f + (0.5f - _curStarScale);
                _scaleDelta = ScaleDelta;
            }
            _highLightStarTf.localScale = new Vector3(_curStarScale, _curStarScale, 1);
        }
    }

    /// <summary>
    /// 射击间隔
    /// </summary>
    protected override int ShootCD
    {
        get { return 12; }
    }

    protected override BulletId BulletId
    {
        get
        {
            return BulletId.ReimuA_Sub1;
        }
    }

    public override void Clear()
    {
        _laser = null;
        _highLightBgTf = null;
        _highLightStarTf = null;
        _subWeaponTf = null;
        base.Clear();
    }
}
