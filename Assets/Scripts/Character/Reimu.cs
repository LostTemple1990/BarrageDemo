using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reimu : CharacterBase
{
    public override void Init()
    {
        // 暂用
        _charAniId = "1001";
        _character = ResourceManager.GetInstance().GetPrefab("Prefab/Character", "Reimu");
        _aniChar = AnimationManager.GetInstance().CreateAnimation<AnimationCharacter>(_character,"ReimuAni",LayerId.Player);
        _aniChar.Play(_charAniId, AniActionType.Idle, Consts.DIR_NULL);
        base.Init();
        _charID = Consts.CharID_Reimu;
        _subID = 0;
        _shootCoolDown = 12;
        _mainBulletId = BulletId.ReimuA_Main;
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
    }
}
