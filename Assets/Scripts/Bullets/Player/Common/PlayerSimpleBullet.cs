using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletSimple : PlayerBulletBase
{
    private MovableObject _movableObject;

    protected bool _isRotatedByVAngle;
    protected float _selfRotationAngle;

    protected float _collisionRadius;

    public PlayerBulletSimple()
    {
        _id = BulletId.Player_Simple;
    }

    public override void Init()
    {
        base.Init();
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
        _bullet = BulletsManager.GetInstance().GetBulletGameObject(BulletId.Player_Simple, int.Parse(id));
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
        _lastPos = _curPos;
        _movableObject.Update();
        _curPos = _movableObject.GetPos();
        CheckRotated();
        CheckHitEnemy();
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
        else
        {
            UpdatePos();
        }
    }

    public override void SetToPosition(Vector2 pos)
    {
        _curPos.x = pos.x;
        _curPos.y = pos.y;
        _movableObject.SetPos(_curPos.x, _curPos.y);
    }

    public override void SetToPosition(float posX, float posY)
    {
        _curPos.x = posX;
        _curPos.y = posY;
        _movableObject.SetPos(_curPos.x, _curPos.y);
    }

    /// <summary>
    /// 匀速直线运动
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    public void DoMoveStraight(float v,float angle)
    {
        _movableObject.DoMoveStraight(v, angle);
    }

    /// <summary>
    /// 加速运动
    /// </summary>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public void DoAcceleration(float acce,float accAngle)
    {
        _movableObject.DoAcceleration(acce, accAngle);
    }

    /// <summary>
    /// 有限制的加速运动
    /// </summary>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    /// <param name="accDuration"></param>
    public void DoAccelerationWithLimitation(float acce, float accAngle,int accDuration)
    {
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, accDuration);
    }

    protected void CheckRotated()
    {
        if ( _selfRotationAngle != 0 )
        {
            _trans.Rotate(new Vector3(0, 0, _bulletCfg.selfRotationAngle));
        }
        else if ( _isRotatedByVAngle )
        {
            Vector3 dv = _curPos - _lastPos;
            float rotateAngle = MathUtil.GetAngleBetweenXAxis(dv.x, dv.y, false);
            _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
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
                    enemy.GetHit(GetDamage());
                    _clearFlag = 1;
                }
            }
        }
    }

    public override CollisionDetectParas GetCollisionDetectParas()
    {
        return new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            centerPos = _curPos,
            radius = _collisionRadius,
        };
    }

    protected override int GetDamage()
    {
        return 2;
    }

    public override void Clear()
    {
        UIManager.GetInstance().HideGo(_bullet);
        ObjectsPool.GetInstance().RestorePrefabToPool(_bulletCfg.textureName, _bullet);
        _bullet = null;
        _bulletCfg = null;
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
        base.Clear();
    }
}
