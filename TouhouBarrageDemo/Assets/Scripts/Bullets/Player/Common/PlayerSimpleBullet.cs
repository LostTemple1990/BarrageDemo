using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletSimple : PlayerBulletBase
{
    private MovableObject _movableObject;

    protected bool _isRotatedByVAngle;
    protected float _selfRotationAngle;

    protected float _collisionRadius;

    protected bool _isEliminating;
    protected int _eliminatingTime;
    protected int _eliminatingDuration;
    protected Color _bulletColor;
    /// <summary>
    /// 击中时造成的伤害
    /// </summary>
    protected float _damage;

    public PlayerBulletSimple()
    {
        _type = BulletType.Player_Simple;
    }

    public override void Init()
    {
        base.Init();
        _isEliminating = false;
        BulletsManager.GetInstance().RegisterPlayerBullet(this);
        _movableObject = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
    }

    public override void ChangeStyleById(string id)
    {
        if ( _bullet != null )
        {
            UIManager.GetInstance().HideGo(_bullet);
            ObjectsPool.GetInstance().RestorePrefabToPool(_bulletCfg.textureName, _bullet);
        }
        _bulletCfg = BulletsManager.GetInstance().GetPlayerBulletCfgById(id);
        _prefabName = id;
        _bullet = BulletsManager.GetInstance().CreateBulletGameObject(BulletType.Player_Simple, id);
        _trans = _bullet.transform;
        _renderer = _trans.Find("BulletSprite").GetComponent<SpriteRenderer>();
        _trans.localPosition = _curPos;
        _isRotatedByVAngle = _bulletCfg.isRotatedByVAngle;
        _selfRotationAngle = _bulletCfg.selfRotationAngle;
        // 碰撞相关参数
        _collisionRadius = _bulletCfg.collisionRadius;
    }

    public override void Update()
    {
        base.Update();
        UpdatePosition();
        if (IsOutOfBorder())
        {
            _clearFlag = 1;
        }
        else
        {
            CheckHitEnemy();
        }
    }

    public override void Render()
    {
        RenderPosition();
        OnFrameStarted();
    }

    protected override void RenderPosition()
    {
        CheckRotated();
        base.RenderPosition();
        if (_isRotationDirty)
        {
            _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, _curRotation));
            _isRotationDirty = false;
        }
    }

    protected virtual void UpdatePosition()
    {
        _movableObject.Update();
        _curPos = _movableObject.GetPos();
    }

    public override void SetPosition(Vector2 pos)
    {
        base.SetPosition(pos);
        _movableObject.SetPos(_curPos.x, _curPos.y);
    }

    public override void SetPosition(float posX, float posY)
    {
        base.SetPosition(posX, posY);
        _movableObject.SetPos(_curPos.x, _curPos.y);
    }

    /// <summary>
    /// 匀速直线运动
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    public void DoStraightMove(float v,float angle)
    {
        if (_isRotatedByVAngle)
            SetRotation(angle);
        _movableObject.DoStraightMove(v, angle);
    }

    /// <summary>
    /// 加速运动
    /// </summary>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public void DoAcceleration(float acce,float accAngle)
    {
        if (_isRotatedByVAngle)
            SetRotation(accAngle);
        _movableObject.DoAcceleration(acce, accAngle);
    }

    /// <summary>
    /// 有限制的加速运动
    /// </summary>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    /// <param name="accDuration"></param>
    public void DoAccelerationWithLimitation(float acce, float accAngle,float maxVelocity)
    {
        if (_isRotatedByVAngle)
            SetRotation(accAngle);
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
    }

    protected void CheckRotated()
    {
        if ( _selfRotationAngle != 0 )
        {
            _curRotation = _curRotation + _bulletCfg.selfRotationAngle;
            _isRotationDirty = true;
        }
        else if ( _isRotatedByVAngle )
        {
            Vector3 dv = _curPos - _lastPos;
            _curRotation = MathUtil.GetAngleBetweenXAxis(dv.x, dv.y);
            _isRotationDirty = true;
        }
    }

    public void CheckHitEnemy()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int enemyCount = enemyList.Count;
        EnemyBase enemy;
        for (int i=0;i<enemyCount;i++)
        {
            enemy = enemyList[i];
            // 自机子弹与敌机使用方形检测
            if ( enemy != null && enemy.CanHit() )
            {
                CollisionDetectParas paras = enemy.GetCollisionDetectParas();
                if ( Mathf.Abs(_curPos.x-paras.centerPos.x) <= _collisionRadius + paras.halfWidth &&
                    Mathf.Abs(_curPos.y - paras.centerPos.y) <= _collisionRadius + paras.halfHeight )
                {
                    enemy.TakeDamage(GetDamage());
                    _hitPos = _curPos;
                    BeginEliminating();
                    break;
                }
            }
        }
    }

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        return true;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        return new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            centerPos = _curPos,
            radius = _collisionRadius,
            nextIndex = -1,
        };
    }

    public override void CollidedByObject(int n = 0, eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {
        _hitPos = _curPos;
        BeginEliminating();
    }

    /// <summary>
    /// 开始消弹
    /// </summary>
    protected virtual void BeginEliminating()
    {
        _detectCollision = false;
        CreateHitEffect(_bulletCfg.hitEffectParas);
        _clearFlag = 1;
    }

    public override bool DetectCollision()
    {
        return _detectCollision && !_isEliminating;
    }

    public void SetDamage(float value)
    {
        _damage = value;
    }

    protected override float GetDamage()
    {
        return _damage;
    }

    public override void Clear()
    {
        _renderer.color = new Color(1, 1, 1, 1);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        _bullet = null;
        _bulletCfg = null;
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
        base.Clear();
    }
}
