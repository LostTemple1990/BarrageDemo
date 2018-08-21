﻿using System.Collections;
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
        _id = BulletId.BulletId_ReimuA_Sub1;
        _prefabName = "ReimuASub1";
    }

    public override void Init()
    {
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
        if ( _target != null && !_target.CanHit() )
        {
            _target = null;
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
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        List<int> indexList = new List<int>();
        int indexCount = 0;
        int enemyCount = enemyList.Count;
        int i;
        for (i = 0; i < enemyCount;i++)
        {
            target = enemyList[i];
            if ( target != null && target.CanHit() )
            {
                indexList.Add(i);
                indexCount++;
            }
        }
        if ( indexCount > 0 )
        {
            // 从可以被攻击的目标当中随机选择
            int index = indexList[MTRandom.GetNextInt(0, indexCount - 1)];
            target = enemyList[index];
            // 设置追踪目标
            _target = target;
            _targetFlag = 1;
        }
    }

    protected override void Move()
    {
        // 目标不为空的时候跟踪目标
        if ( _target != null )
        {
            Vector3 targetPos = _target.CurPos;
            Vector2 vecToTarget = targetPos - _curPos;
            Vector2 vVec = new Vector2(_vx, _vy);
            // 计算加速度
            Vector2 dv = (vecToTarget.normalized * _curVelocity*2) - vVec;
            dv *= _curVelocity / vecToTarget.magnitude;
            // 还原速度
            vVec.x += dv.x;
            vVec.y += dv.y;
            vVec = vVec.normalized * _curVelocity;
            // 赋值
            _vx = vVec.x;
            _vy = vVec.y;
        }
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

    public override void Clear()
    {
        _target = null;
        base.Clear();
    }
}
