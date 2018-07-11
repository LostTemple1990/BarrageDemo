using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletReimuASub1 :PlayerBulletBase
{
    protected static Vector3 RotateEuler = new Vector3(0f, 0f, 20f);
    protected static float DefaultVelocity = 10f;
    /// <summary>
    /// 默认初始速度
    /// </summary>
    private const float DefaultSpeedX = 0f;
    private const float DefaultSpeedY = 10f;

    /// <summary>
    /// 跟踪目标
    /// </summary>
    private EnemyBase _target;
    /// <summary>
    /// 标识该子弹是否已经锁定过目标
    /// </summary>
    private int _targetFlag = 0;

    public BulletReimuASub1()
    {

    }

    public override void Init()
    {
        _id = BulletId.BulletId_ReimuA_Sub1;
        _prefabName = "ReimuASub1";
        base.Init();
        UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.PlayerBarage);
        BulletsManager.GetInstance().RegisterPlayerBullet(this);
        _target = null;
        _targetFlag = 0;
    }

    public override void Update()
    {
        if ( _targetFlag == 0 )
        {
            GetRandomTarget();
        }
        if ( _isMoving )
        {
            Move();
        }
        UpdateAni();
        UpdatePos();
        CheckHitEnemy();
    }

    protected virtual void GetRandomTarget()
    {
        EnemyBase target = EnemyManager.GetInstance().GetRandomEnemy();
        if ( target != null && target.CanHit() )
        {
            _target = target;
            // 子弹指向目标的向量
            Vector3 vec = _target.CurPos - _curPos;
            float length = vec.magnitude;
            if ( length == 0 )
            {
                return;
            }
            float deg = Mathf.Acos(vec.x / length) * Mathf.Rad2Deg;
            if ( vec.y < 0 )
            {
                deg = 360f - deg;
            }
            DoMove(_curVelocity, deg);
            _targetFlag = 1;
        }
    }

    protected override void Move()
    {
        _curPos.x += _vx;
        _curPos.y += _vy;
        if (IsOutOfBorder())
        {
            _clearFlag = 1;
        }
    }

    protected void UpdateAni()
    {
        _bullet.transform.Rotate(RotateEuler);
    }

    public override int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        arg1 = _curPos.x;
        arg2 = _curPos.y;
        arg3 = 8;
        arg4 = 8;
        return Consts.CollisionType_Rect;
    }

    protected override float GetDamage()
    {
        return 1;
    }
}
