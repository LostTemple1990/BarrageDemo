using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderRect : ObjectColliderBase
{
    /// <summary>
    /// 矩形的宽
    /// </summary>
    private float _rectWidth;
    /// <summary>
    /// 矩形的高
    /// </summary>
    private float _rectHeight;
    /// <summary>
    /// 宽度的一半
    /// </summary>
    private float _halfWidth;
    /// <summary>
    /// 高度的一半
    /// </summary>
    private float _halfHeight;
    private float _fromWidth;
    private float _fromHeight;
    private float _toWidth;
    private float _toHeight;

    public ColliderRect()
        : base()
    {
        _type = eColliderType.Rect;
    }

    public override void SetSize(float arg0, float arg1)
    {
        _rectWidth = arg0;
        _halfWidth = _rectWidth / 2;
        _rectHeight = arg1;
        _halfHeight = _rectHeight / 2;
    }

    public override void ScaleToSize(float toArg0, float toArg1, int duration)
    {
        _fromWidth = _halfWidth;
        _toWidth = toArg0;
        _fromHeight = _halfHeight;
        _toHeight = toArg1;
        base.ScaleToSize(toArg0, toArg1, duration);
    }

    protected override void Scale()
    {
        _scaleTime++;
        if (_scaleTime >= _scaleDuration)
        {
            _halfWidth = _toWidth;
            _halfHeight = _toHeight;
            _isScaling = false;
        }
        else
        {
            _halfWidth = MathUtil.GetLinearInterpolation(_fromWidth, _toWidth, _scaleTime, _scaleDuration);
            _halfHeight = MathUtil.GetLinearInterpolation(_fromHeight, _toHeight, _scaleTime, _scaleDuration);
        }
    }

    protected override void CheckCollisionWithPlayer()
    {
        Vector2 playerPos = Global.PlayerPos;
        float playerCollisionRadius = Global.PlayerCollisionVec.z;
        if ( Mathf.Abs(playerPos.x-_curPosX) <= _halfWidth + playerCollisionRadius &&
            Mathf.Abs(playerPos.y - _curPosY) <= _halfHeight + playerCollisionRadius )
        { 
            PlayerService.GetInstance().GetCharacter().BeingHit();
        }
    }

    protected override void CheckCollisionWithPlayerBullet()
    {
        // 计算碰撞盒参数
        Vector2 lbPos = new Vector2(_curPosX - _halfWidth, _curPosY - _halfHeight);
        Vector2 rtPos = new Vector2(_curPosX + _halfWidth, _curPosY + _halfHeight);
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
                // 子弹为圆形判定，方形判定来检测
                float dx = Mathf.Abs(_curPosX - collParas.centerPos.x);
                float dy = Mathf.Abs(_curPosY - collParas.centerPos.y);
                // 检测该碰撞剧情与方形是否相交
                if (dx <= _halfWidth + collParas.radius && dy <= _halfHeight + collParas.radius)
                {
                    bullet.CollidedByObject(curColliderIndex);
                    isCollided = true;
                }
            }
        } while (nextColliderIndex != -1);
        return isCollided;
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
                para = enemy.GetCollisionDetectParas();
                // 敌机全部使用矩形判定
                dx = Mathf.Abs(_curPosX - para.centerPos.x);
                dy = Mathf.Abs(_curPosY - para.centerPos.y);
                if (dx <= _halfWidth + para.halfWidth && dy <= _halfHeight + para.halfHeight)
                {
                    enemy.GetHit(_hitEnemyDamage,_eliminateType);
                }
            }
        }
    }

    protected override void CheckCollisionWithEnemyBullet()
    {
        // 计算碰撞盒参数
        Vector2 lbPos = new Vector2(_curPosX - _halfWidth, _curPosY - _halfHeight);
        Vector2 rtPos = new Vector2(_curPosX + _halfWidth, _curPosY + _halfHeight);
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
                bullet.CanBeEliminated(eEliminateDef.HitObjectCollider) &&
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
                // 子弹为圆形判定，方形判定来检测
                float dx = Mathf.Abs(_curPosX - collParas.centerPos.x);
                float dy = Mathf.Abs(_curPosY - collParas.centerPos.y);
                // 检测该碰撞剧情与方形是否相交
                if (dx <= _halfWidth + collParas.radius && dy <= _halfHeight + collParas.radius)
                {
                    bullet.CollidedByObject(curColliderIndex);
                    isCollided = true;
                }
            }
            else if (collParas.type == CollisionDetectType.Rect)
            {
                // 计算rect1的分离轴向量
                float angle0 = 0;
                float angle1 = collParas.angle;
                float cos0 = Mathf.Cos(Mathf.Deg2Rad * angle0);
                float sin0 = Mathf.Sin(Mathf.Deg2Rad * angle0);
                float cos1 = Mathf.Cos(Mathf.Deg2Rad * angle1);
                float sin1 = Mathf.Sin(Mathf.Deg2Rad * angle1);
                //rect0分离轴
                Vector2 rect0Vec0 = new Vector2(cos0, sin0);
                Vector2 rect0Vec1 = new Vector2(-sin0, cos0);
                // rect1分离轴
                Vector2 rect1Vec0 = new Vector2(cos1, sin1);
                Vector2 rect1Vec1 = new Vector2(-sin1, cos1);
                List<Vector2> rectVecList = new List<Vector2> { rect0Vec0, rect0Vec1, rect1Vec0, rect1Vec1 };
                // 两矩形中心的向量
                Vector2 centerVec = new Vector2(collParas.centerPos.x - _curPos.x, collParas.centerPos.y - _curPos.y);
                bool rectIsCollided = true;
                for (int i = 0; i < rectVecList.Count; i++)
                {
                    // 投影轴
                    Vector2 vec = rectVecList[i];
                    // rect0的投影半径对于该投影轴的投影
                    float projectionRadius0 = Mathf.Abs(Vector2.Dot(rect0Vec0, vec) * _halfWidth) + Mathf.Abs(Vector2.Dot(rect0Vec1, vec) * _halfHeight);
                    projectionRadius0 = Mathf.Abs(projectionRadius0);
                    // rect1的投影半径对于投影轴的投影
                    float projectionRadius1 = Mathf.Abs(Vector2.Dot(rect1Vec0, vec) * collParas.halfWidth) + Mathf.Abs(Vector2.Dot(rect1Vec1, vec) * collParas.halfHeight);
                    projectionRadius1 = Mathf.Abs(projectionRadius1);
                    // 连线对于投影轴的投影
                    float centerVecProjection = Vector2.Dot(centerVec, vec);
                    centerVecProjection = Mathf.Abs(centerVecProjection);
                    // 投影的和小于轴半径的长度,说明没有碰撞
                    if (projectionRadius0 + projectionRadius1 <= centerVecProjection)
                    {
                        rectIsCollided = false;
                        break;
                    }
                }
                if ( rectIsCollided )
                {
                    bullet.CollidedByObject(curColliderIndex);
                    isCollided = true;
                }
            }
        } while (nextColliderIndex != -1);
        return isCollided;
    }
}
