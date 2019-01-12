using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderCircle : ObjectColliderBase
{
    protected float _radius;
    protected float _fromRaidus;
    protected float _toRaidus;

    public ColliderCircle()
        :base()
    {
        _type = eColliderType.Circle;
    }

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
        if ( _scaleTime >= _scaleDuration )
        {
            _radius = _toRaidus;
            _isScaling = false;
        }
        else
        {
            _radius = MathUtil.GetLinearInterpolation(_fromRaidus, _toRaidus, _scaleTime, _scaleDuration);
        }
    }

    protected override void CheckCollisionWithPlayer()
    {
        float dis = Vector2.Distance(_curPos, Global.PlayerPos);
        if ( dis <= Global.PlayerCollisionVec.z + _radius )
        {
            CollidedByPlayer();
        }
    }

    /// <summary>
    /// 与玩家发生碰撞
    /// </summary>
    protected virtual void CollidedByPlayer()
    {
        PlayerService.GetInstance().GetCharacter().BeingHit();
    }

    protected override void CheckCollisionWithPlayerBullet()
    {
        // 计算碰撞盒参数
        Vector2 lbPos = new Vector2(_curPosX - _radius, _curPosY - _radius);
        Vector2 rtPos = new Vector2(_curPosX + _radius, _curPosY + _radius);
        int i, bulletCount;
        List<PlayerBulletBase> bulletList = BulletsManager.GetInstance().GetPlayerBulletList();
        PlayerBulletBase bullet;
        bulletCount = bulletList.Count;
        for (i = 0; i < bulletCount; i++)
        {
            bullet = bulletList[i];
            // 判断是否要进行碰撞检测
            if (bullet != null &&
                bullet.ClearFlag == 0 &&
                bullet.DetectCollision() &&
                bullet.CheckBoundingBoxesIntersect(lbPos, rtPos))
            {
                DetectCollisionWithPlayerBullet(bullet);
            }
        }
    }

    private bool DetectCollisionWithPlayerBullet(PlayerBulletBase bullet)
    {
        int nextColliderIndex = 0;
        int curColliderIndex;
        bool isCollided = false;
        do
        {
            CollisionDetectParas collParas = bullet.GetCollisionDetectParas(nextColliderIndex);
            curColliderIndex = nextColliderIndex;
            nextColliderIndex = collParas.nextIndex;
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
                        CollidedByPlayerBullet(bullet, curColliderIndex);
                        isCollided = true;
                    }
                }
            }
        } while (nextColliderIndex != -1);
        return isCollided;
    }

    /// <summary>
    /// 与玩家子弹发生碰撞
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="curColliderIndex"></param>
    protected virtual void CollidedByPlayerBullet(PlayerBulletBase bullet, int curColliderIndex)
    {
        bullet.CollidedByObject(curColliderIndex);
    }

    protected override void CheckCollisionWithEnemy()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int count = enemyList.Count;
        EnemyBase enemy;
        CollisionDetectParas para;
        float dx, dy;
        for (int i=0;i<count;i++)
        {
            enemy = enemyList[i];
            if ( enemy != null && enemy.IsInteractive )
            {
                para = enemy.GetCollisionDetectParas(0);
                // 敌机全部使用矩形判定
                dx = Mathf.Abs(_curPosX - para.centerPos.x);
                dy = Mathf.Abs(_curPosY - para.centerPos.y);
                if ( dx <= _radius + para.halfWidth && dy <= _radius + para.halfHeight )
                {
                    CollidedByEnemy(enemy);
                }
            }
        }
    }

    protected virtual void CollidedByEnemy(EnemyBase enemy)
    {
        enemy.TakeDamage(_hitEnemyDamage, _eliminateType);
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
                bullet.CheckBoundingBoxesIntersect(lbPos,rtPos) )
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
        int nextColliderIndex = 0;
        int curColliderIndex;
        bool isCollided = false;
        do
        {
            CollisionDetectParas collParas = bullet.GetCollisionDetectParas(nextColliderIndex);
            curColliderIndex = nextColliderIndex;
            nextColliderIndex = collParas.nextIndex;
            if ( DetectCollisionWithCollisionParas(collParas) )
            {
                CollidedByEnemyBullet(bullet, curColliderIndex);
                isCollided = true;
            }
        } while (nextColliderIndex != -1);
        return isCollided;
    }

    /// <summary>
    /// 与敌机子弹发生碰撞
    /// </summary>
    /// <param name="bullet"></param>
    /// <param name="curColliderIndex"></param>
    protected virtual void CollidedByEnemyBullet(EnemyBulletBase bullet, int curColliderIndex)
    {
        bullet.CollidedByObject(curColliderIndex);
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
        else if (collParas.type == CollisionDetectType.Rect || collParas.type == CollisionDetectType.ItalicRect )
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
}
