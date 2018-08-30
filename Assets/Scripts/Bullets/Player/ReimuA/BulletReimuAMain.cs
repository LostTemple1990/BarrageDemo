using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletReimuAMain : PlayerBulletBase
{
    /// <summary>
    /// 默认初始速度
    /// </summary>
    private const float DefaultSpeedY = 10f;

    public BulletReimuAMain()
    {
        _id = BulletId.ReimuA_Main;
        _prefabName = "ReimuAMain";
    }

    public override void Init()
    {
        base.Init();
        UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.PlayerBarage);
        DoMove(DefaultSpeedY, 90);
        _trans.localRotation = Quaternion.Euler(0f, 0f, 90f); ;
        BulletsManager.GetInstance().RegisterPlayerBullet(this);
    }

    public override void Update()
    {
        Move();
        UpdatePos();
        CheckHitEnemy();
    }

    protected override void Move()
    {
        _curPos.y += _vy;
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
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
}
