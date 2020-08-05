using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletReimuASub1 : PlayerBulletSimple
{
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
    /// 目标的实例id
    /// </summary>
    private int _targetInstID;
    /// <summary>
    /// 标识该子弹是否已经锁定过目标
    /// </summary>
    private int _targetFlag = 0;

    public BulletReimuASub1()
    {
        _type = BulletType.ReimuA_Sub1;
        _damage = 0.5f;
    }

    public override void Init()
    {
        base.Init();
        _target = null;
        _targetFlag = 0;
    }

    protected override void UpdatePosition()
    {
        if (_targetFlag == 0)
        {
            _target = FindNewTarget();
            if (_target != null)
            {
                _targetFlag = 1;
                _targetInstID = _target.GetInstanceID();
            }
        }
        if (_target != null && _targetInstID != _target.GetInstanceID())
        {
            _target = null;
        }
        if (_isMoving)
        {
            Move();
        }
    }

    protected EnemyBase FindNewTarget()
    {
        EnemyBase target = null;
        EnemyBase tmpTarget;
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int enemyCount = enemyList.Count;
        float maxK = -1;
        for (int i = 0; i < enemyCount;i++)
        {
            tmpTarget = enemyList[i];
            if (tmpTarget != null && tmpTarget.isAvailable && tmpTarget.CanHit() )
            {
                Vector2 dVec = tmpTarget.GetPosition() - GetPosition();
                float k = dVec.y / (Mathf.Abs(dVec.x) + 0.01f);
                if (k > maxK)
                {
                    maxK = k;
                    target = tmpTarget;
                }
            }
        }
        return target;
    }

    protected override void Move()
    {
        // 目标不为空的时候跟踪目标
        if ( _target != null )
        {
            Vector2 targetPos = _target.GetPosition();
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
    }

    protected override void BeginEliminating()
    {
        // 重新计算一下当前旋转角度，用于生成击中特效
        _curRotation = MathUtil.GetAngleBetweenXAxis(_vx, _vy);
        base.BeginEliminating();
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