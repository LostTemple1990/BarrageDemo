using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reimu : CharacterBase
{
    /// <summary>
    /// 子机总个数
    /// </summary>
    private const int SubWeaponTotalCount = 4;
    /// <summary>
    /// 高速状态下的移动速度
    /// </summary>
    private const float HighSpeedStateSpeed = 4.5f;
    /// <summary>
    /// 低速状态下的移动速度
    /// </summary>
    private const float LowSpeedStateSpeed = 2f;

    /// <summary>
    /// 子机在高低速切换下的移动时间
    /// </summary>
    private const int SubMoveDuration = 5;
    /// <summary>
    /// 当前帧副武器的数目改变了
    /// </summary>
    private bool _isChangingSubCount;
    /// <summary>
    /// 副武器的位置偏移量
    /// </summary>
    private Vector2[][][] _subPosOffset;
    /// <summary>
    /// 子机正在高低切换的移动中
    /// </summary>
    private bool _isSubMoving;
    /// <summary>
    /// 子机高低速切换移动时间
    /// </summary>
    private int _subMoveTime;
    /// <summary>
    /// 子机高低速切换移动总时间
    /// </summary>
    private int _subMoveDuration;
    /// <summary>
    /// 高低速切换起始模式
    /// </summary>
    private int _subMoveFromMode;
    /// <summary>
    /// 高低速切换结束模式
    /// </summary>
    private int _subMoveToMode;

    public override void Init()
    {
        _charAniId = "1001";
        _character = ResourceManager.GetInstance().GetPrefab("Prefab/Character", "Reimu");
        _aniChar = AnimationManager.GetInstance().CreateAnimation<AnimationCharacter>(_character,"ReimuAni",LayerId.Player);
        _aniChar.Play(_charAniId, AniActionType.Idle, Consts.DIR_NULL);
        base.Init();
        _charID = Consts.CharID_Reimu;
        _subID = 0;
        _shootCoolDown = 6;
        _mainBulletId = "500000";
        int i;
        GameObject subWeapon;
        for (i = 0; i < 4; i++)
        {
            // 副武器GO
            subWeapon = _subLayerTrans.Find("SubWeapon" + i).gameObject;
            // 初始化
            _subWeapons[i] = new SubWeaponReimuA();
            _subWeapons[i].Init(subWeapon, this);
            _subWeapons[i].SubWeaponIndex = i;
        }
        // 初始化炸弹
        _bomb = new BombReimuA();
        // 初始化碰撞、擦弹半径
        _collisionRadius = 2.0f;
        _grazeRadius = 4f;
        _bombCoolDown = 300;
        _bombInvincibleDuration = 300;
        // 初始化副武器的位置偏移量
        _subPosOffset = new Vector2[2][][];
        // 高速
        _subPosOffset[Consts.MoveModeHighSpeed] = new Vector2[4][];
        _subPosOffset[Consts.MoveModeHighSpeed][0] = new Vector2[] { new Vector2(0f, 32f) };
        _subPosOffset[Consts.MoveModeHighSpeed][1] = new Vector2[] { new Vector2(-12f, 32f), new Vector2(12f, 32f) };
        _subPosOffset[Consts.MoveModeHighSpeed][2] = new Vector2[] { new Vector2(-32f, 0f), new Vector2(0f, 32f), new Vector2(32f, 0f) };
        _subPosOffset[Consts.MoveModeHighSpeed][3] = new Vector2[] { new Vector2(-32f, 0f), new Vector2(-12f, 32f), new Vector2(12f, 32f), new Vector2(32f, 0f) };
        // 低速
        _subPosOffset[Consts.ModeModeLowSpeed] = new Vector2[4][];
        _subPosOffset[Consts.ModeModeLowSpeed][0] = new Vector2[] { new Vector2(0f, 28f) };
        _subPosOffset[Consts.ModeModeLowSpeed][1] = new Vector2[] { new Vector2(-12f, 28f), new Vector2(12f, 28f) };
        _subPosOffset[Consts.ModeModeLowSpeed][2] = new Vector2[] { new Vector2(-28f, 4f), new Vector2(0f, 28f), new Vector2(28f, 4f) };
        _subPosOffset[Consts.ModeModeLowSpeed][3] = new Vector2[] { new Vector2(-28f, 4f), new Vector2(-12f, 28f), new Vector2(12f, 28f), new Vector2(28f, 4f) };
        _isSubMoving = false;
        _subMoveDuration = SubMoveDuration;
    }

    protected override void CreateMainBullets()
    {
        // 左侧子弹
        PlayerBulletSimple bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.Player_Simple) as PlayerBulletSimple;
        bullet.ChangeStyleById(_mainBulletId);
        bullet.DoMoveStraight(18f, 90f);
        bullet.SetToPosition(_curPos.x + _leftBulletOffset.x, _curPos.y + _leftBulletOffset.y);
        // 右侧子弹
        bullet = ObjectsPool.GetInstance().CreateBullet(BulletType.Player_Simple) as PlayerBulletSimple;
        bullet.ChangeStyleById(_mainBulletId);
        bullet.DoMoveStraight(18f, 90f);
        bullet.SetToPosition(_curPos.x + _rightBulletOffset.x, _curPos.y + _rightBulletOffset.y);
    }

    protected override void UpdateSubWeapons()
    {
        _isChangingSubCount = false;
        UpdateSubWeaponsAvailableCount();
        UpdateSubWeaponsPosition();
        for (int i = 0; i < _availableSubCount; i++)
        {
            _subWeapons[i].Update();
        }
    }

    /// <summary>
    /// 更新子机可用的数目
    /// </summary>
    private void UpdateSubWeaponsAvailableCount()
    {
        int availableCount = PlayerService.GetInstance().GetPower() / 100;
        if (availableCount != _availableSubCount)
        {
            SubWeaponBase subWeapon;
            _availableSubCount = availableCount;
            for (int i = 0; i < 4; i++)
            {
                subWeapon = _subWeapons[i];
                subWeapon.SetActive(i < _availableSubCount);
            }
            _isChangingSubCount = true;
        }
    }

    /// <summary>
    /// 更新子机的位置
    /// </summary>
    private void UpdateSubWeaponsPosition()
    {
        if (_isChangingSubCount)
        {
            SubWeaponBase subWeapon;
            for (int i = 0; i < _availableSubCount; i++)
            {
                subWeapon = _subWeapons[i];
                Vector2 pos = _subPosOffset[_curMoveMode][_availableSubCount - 1][i];
                subWeapon.SetToPosition(pos);
            }
        }
        else if (_isSubMoving)
        {
            SubWeaponBase subWeapon;
            Vector2 pos, startPos, endPos;
            float rate;
            // 增加时间
            _subMoveTime++;
            // 位置插值
            for (int i = 0; i < _availableSubCount; i++)
            {
                subWeapon = _subWeapons[i];
                rate = (float)_subMoveTime / _subMoveDuration;
                startPos = _subPosOffset[_subMoveFromMode][_availableSubCount - 1][i];
                endPos = _subPosOffset[_subMoveToMode][_availableSubCount - 1][i];
                pos = Vector2.Lerp(startPos, endPos, rate);
                subWeapon.SetToPosition(pos);
            }
            if (_subMoveTime >= _subMoveDuration)
            {
                _isSubMoving = false;
            }
        }
        else if (_preMoveMode != _curMoveMode)
        {
            // 设置子机位移的时间
            if (_isSubMoving)
            {
                _subMoveTime = _subMoveDuration - _subMoveTime;
            }
            else
            {
                _subMoveTime = 0;
            }
            // 设置子机位移的起始结束模式
            _subMoveFromMode = _preMoveMode;
            _subMoveToMode = _curMoveMode;
            _isSubMoving = true;
        }
    }

    public override float GetMoveSpeed()
    {
        float speed = _inputMoveMode == Consts.MoveModeHighSpeed ? HighSpeedStateSpeed : LowSpeedStateSpeed;
        return speed;
    }

    protected override void ResetSubWeapons()
    {
        _availableSubCount = -1;
        UpdateSubWeaponsAvailableCount();
        _availableSubCount = PlayerService.GetInstance().GetPower() / 100;
        SubWeaponBase subWeapon;
        for (int i = 0; i < _availableSubCount; i++)
        {
            subWeapon = _subWeapons[i];
            Vector2 pos = _subPosOffset[_curMoveMode][_availableSubCount - 1][i];
            subWeapon.SetToPosition(pos);
        }
    }
}
