using UnityEngine;
using System.Collections.Generic;

public class GravitationFieldCircle : ObjectColliderBase
{
    /// <summary>
    /// 引力场半径
    /// </summary>
    protected float _radius;
    protected float _fromRaidus;
    protected float _toRaidus;
    /// <summary>
    /// 引力场类型
    /// </summary>
    private int _fieldType;
    /// <summary>
    /// 赋予的速度
    /// </summary>
    private float _velocity;
    /// <summary>
    /// 速度偏移
    /// </summary>
    private float _velocityOffset;
    private float _vAngle;
    private float _vAngleOffset;
    private float _acce;
    private float _acceOffset;
    private float _accAngle;
    private float _accAngleOffset;
    /// <summary>
    /// 当前被影响的物体列表
    /// </summary>
    private Dictionary<IAffectedMovableObject, GravitationParas> _affectedObjectDic;

    public GravitationFieldCircle()
        :base()
    {
        _type = eColliderType.Circle;
        _affectedObjectDic = new Dictionary<IAffectedMovableObject, GravitationParas>();
    }

    #region Collider基础方法

    public override void SetSize(float arg0, float arg1)
    {
        _radius = arg0;
    }

    public override void ScaleToSize(float toArg0, float toArg1, int duration)
    {
        _fromRaidus = _radius;
        _toRaidus = toArg0;
        base.ScaleToSize(toArg0, toArg1, duration);
    }

    protected override void Scale()
    {
        _scaleTime++;
        if (_scaleTime >= _scaleDuration)
        {
            _radius = _toRaidus;
            _isScaling = false;
        }
        else
        {
            _radius = MathUtil.GetLinearInterpolation(_fromRaidus, _toRaidus, _scaleTime, _scaleDuration);
        }
    }
    #endregion

    #region 碰撞检测相关方法

    protected override void CheckCollisionWithPlayer()
    {
        float dis = Vector2.Distance(_curPos, Global.PlayerPos);
        if (dis <= Global.PlayerCollisionVec.z + _radius)
        {
            CharacterBase player = PlayerService.GetInstance().GetCharacter();
            CollidedByObject(player);
        }
    }

    protected override void CheckCollisionWithPlayerBullet()
    {
        // todo 暂时留空，以后有需求的时候再加入实现
    }

    protected override void CheckCollisionWithEnemy()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int count = enemyList.Count;
        EnemyBase enemy;
        CollisionDetectParas para;
        float dx, dy;
        for (int i = 0; i < count; i++)
        {
            enemy = enemyList[i];
            if (enemy != null && enemy.IsInteractive)
            {
                para = enemy.GetCollisionDetectParas(0);
                // 敌机全部使用矩形判定
                dx = Mathf.Abs(_curPosX - para.centerPos.x);
                dy = Mathf.Abs(_curPosY - para.centerPos.y);
                if (dx <= _radius + para.halfWidth && dy <= _radius + para.halfHeight)
                {
                    CollidedByObject(enemy);
                }
            }
        }
    }

    protected override void CheckCollisionWithEnemyBullet()
    {
        // 计算碰撞盒参数
        Vector2 lbPos = new Vector2(_curPosX - _radius, _curPosY - _radius);
        Vector2 rtPos = new Vector2(_curPosX + _radius, _curPosY + _radius);
        int i, bulletCount;
        List<EnemyBulletBase> bulletList = BulletsManager.GetInstance().GetEnemyBulletList();
        EnemyBulletBase bullet;
        bulletCount = bulletList.Count;
        for (i = 0; i < bulletCount; i++)
        {
            bullet = bulletList[i];
            // 判断是否要进行碰撞检测
            if (bullet != null &&
                bullet.ClearFlag == 0 &&
                bullet.CanBeEliminated(_eliminateType) &&
                bullet.CheckBoundingBoxesIntersect(lbPos, rtPos))
            {
                DetectCollisionWithEnemyBullet(bullet);
            }
        }
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="collParas"></param>
    /// <returns></returns>
    private bool DetectCollisionWithEnemyBullet(EnemyBulletBase bullet)
    {
        if (bullet.Type == BulletType.Enemy_Laser) return false;
        CollisionDetectParas collParas;
        switch ( bullet.Type )
        {
            case BulletType.Enemy_Simple:
                collParas = bullet.GetCollisionDetectParas(0);
                break;
            case BulletType.Enemy_LinearLaser:
            case BulletType.Enemy_CurveLaser:
                collParas = bullet.GetCollisionDetectParas(-1);
                break;
            default:
                collParas = new CollisionDetectParas();
                break;
        }
        if ( DetectCollisionWithCollisionParas(collParas) )
        {
            CollidedByObject(bullet);
            return true;
        }
        return false;
    }

    public override bool DetectCollisionWithCollisionParas(CollisionDetectParas collParas)
    {
        if (collParas.type == CollisionDetectType.Circle)
        {
            // 子弹为圆形判定，先检测外切正方形
            float dx = Mathf.Abs(_curPosX - collParas.centerPos.x);
            float dy = Mathf.Abs(_curPosY - collParas.centerPos.y);
            // 两圆的半径和
            float sumOfRadius = _radius + collParas.radius;
            if (dx <= sumOfRadius && dy <= sumOfRadius)
            {
                if (dx * dx + dy * dy <= sumOfRadius * sumOfRadius)
                {
                    return true;
                }
            }
        }
        else if (collParas.type == CollisionDetectType.Rect || collParas.type == CollisionDetectType.ItalicRect)
        {
            if (_radius == 0) return false;
            // 子弹为矩形判定
            // 以子弹中心点为圆心，将B的判定中心旋转angle的角度计算判定
            Vector2 vec = new Vector2(_curPosX - collParas.centerPos.x, _curPosY - collParas.centerPos.y);
            float cos = Mathf.Cos(collParas.angle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(collParas.angle * Mathf.Deg2Rad);
            Vector2 relativeVec = new Vector2();
            // 向量顺时针旋转laserAngle的度数
            relativeVec.x = cos * vec.x + sin * vec.y;
            relativeVec.y = -sin * vec.x + cos * vec.y;
            // 计算圆和矩形的碰撞
            float len = relativeVec.magnitude;
            float dLen = len - _radius;
            // 若圆心和矩形中心的连线长度小于圆的半径，说明矩形肯定有一部分在圆内
            // 因此直接认定为碰撞
            if (dLen <= 0)
            {
                return true;
            }
            else
            {
                float rate = dLen / len;
                relativeVec *= rate;
                if (Mathf.Abs(relativeVec.x) < collParas.halfHeight && Mathf.Abs(relativeVec.y) < collParas.halfWidth)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void CollidedByObject(IAffectedMovableObject affectedObject)
    {
        int curTime = STGStageManager.GetInstance().GetFrameSinceStageStart();
        GravitationParas paras;
        if (_affectedObjectDic.TryGetValue(affectedObject, out paras))
        {
            if (curTime - paras.lastUpdateTime == 1)
            {
                paras.timeInGravitationField++;
            }
            else
            {
                paras = CreateGravitationParas(affectedObject);
                paras.timeInGravitationField = 1;
            }
        }
        else
        {
            paras = CreateGravitationParas(affectedObject);
            paras.timeInGravitationField = 1;
        }
        paras.lastUpdateTime = curTime;
        _affectedObjectDic[affectedObject] = paras;
        // 设置额外运动参数
        affectedObject.AddExtraSpeedParas(paras.velocity, paras.vAngle, paras.acce, paras.accAngle);
    }
    #endregion

    public void Init(int fieldType, float velocity, float velocityOffest, float vAngle, float angleOffest, float acce, float acceOffset, float accAngle, float accAngleOffset)
    {
        _fieldType = fieldType;
        _velocity = velocity;
        _velocityOffset = velocityOffest;
        _vAngle = vAngle;
        _vAngleOffset = angleOffest;
        _acce = acce;
        _acceOffset = acceOffset;
        _accAngle = accAngle;
        _accAngleOffset = accAngleOffset;
    }

    private GravitationParas CreateGravitationParas(IAffectedMovableObject affectedObj)
    {
        // 偏移计算
        float velocityOffset = _velocityOffset == 0 ? 0 : MTRandom.GetNextFloat(-_velocityOffset, _velocityOffset);
        float vAngleOffset = _vAngleOffset == 0 ? 0 : MTRandom.GetNextFloat(-_vAngleOffset, _vAngleOffset);
        float acceOffset = _acceOffset == 0 ? 0 : MTRandom.GetNextFloat(-_acceOffset, _acceOffset);
        float accAngleOffset = _accAngleOffset == 0 ? 0 : MTRandom.GetNextFloat(-_accAngleOffset, _accAngleOffset);
        // 计算实际的额外运动参数
        float v = _velocity + _velocityOffset;
        float acce = _acce + _acceOffset;
        float vAngle = _vAngle, accAngle = _accAngle;
        // 中心吸力
        if (_fieldType == 1)
        {
            Vector2 affectObjectPos = affectedObj.GetPosition();
            Vector2 fieldPos = _curPos;
            float angle = MathUtil.GetAngleBetweenXAxis(fieldPos - affectObjectPos);
            vAngle = angle;
            accAngle = angle;
        }
        // 中心斥力
        else if (_fieldType == 2)
        {
            Vector2 affectObjectPos = affectedObj.GetPosition();
            Vector2 fieldPos = _curPos;
            float angle = MathUtil.GetAngleBetweenXAxis(affectObjectPos - fieldPos);
            vAngle = angle;
            accAngle = angle;
        }
        // 计算速度方向以及加速度方向
        vAngle += _vAngleOffset;
        accAngle += _accAngleOffset;

        GravitationParas paras = new GravitationParas
        {
            velocity = v,
            vAngle = vAngle,
            acce = acce,
            accAngle = accAngle,
        };
        return paras;
    }
}
