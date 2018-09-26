using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColliderRect : ObjectColliderBase
{
    private float _width;
    private float _height;
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
        _width = arg0;
        _height = arg1;
    }

    public override void ScaleToSize(float toArg0, float toArg1, int duration)
    {
        _fromWidth = _width;
        _toWidth = toArg0;
        _fromHeight = _height;
        _toHeight = toArg1;
        base.ScaleToSize(toArg0, toArg1, duration);
    }

    protected override void Scale()
    {
        _scaleTime++;
        if (_scaleTime >= _scaleDuration)
        {
            _width = _toWidth;
            _height = _toHeight;
            _isScaling = false;
        }
        else
        {
            _width = MathUtil.GetLinearInterpolation(_fromWidth, _toWidth, _scaleTime, _scaleDuration);
            _height = MathUtil.GetLinearInterpolation(_fromHeight, _toHeight, _scaleTime, _scaleDuration);
        }
    }

    protected override void CheckCollisionWithPlayer()
    {
        //float disX = Mathf.Abs(_cur)
        float dis = Vector2.Distance(_curPos, Global.PlayerPos);
        if (dis <= Global.PlayerCollisionVec.z + _width)
        {
            PlayerService.GetInstance().GetCharacter().BeingHit();
        }
    }

    protected override void CheckCollisionWithPlayerBullet()
    {
        //List<PlayerBulletBase> _bulletList = BulletsManager.GetInstance().
    }

    protected override void CheckCollisionWithEnemyBullet()
    {
        int i, bulletCount;
        List<EnemyBulletBase> bulletList = BulletsManager.GetInstance().GetEnemyBulletList();
        EnemyBulletBase bullet;
        bulletCount = bulletList.Count;
        for (i = 0; i < bulletCount; i++)
        {
            bullet = bulletList[i];
            // 判断是否要进行碰撞检测
            if (bullet != null && bullet.ClearFlag == 0 && bullet.CanBeEliminated(eEliminateDef.HitObject))
            {
                DetectCollision(bullet);
            }
        }
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="collParas"></param>
    /// <returns></returns>
    private bool DetectCollision(EnemyBulletBase bullet)
    {
        CollisionDetectParas collParas = bullet.GetCollisionDetectParas();
        if (collParas.type == CollisionDetectType.Circle)
        {
            // 子弹为圆形判定，先检测外切正方形
            float dx = Mathf.Abs(_curPosX - collParas.centerPos.x);
            float dy = Mathf.Abs(_curPosY - collParas.centerPos.y);
            // 两圆的半径和
            float sumOfRadius = _width + collParas.radius;
            if (dx <= sumOfRadius && dy <= sumOfRadius)
            {
                if (dx * dx + dy * dy <= sumOfRadius * sumOfRadius)
                {
                    bullet.Eliminate(eEliminateDef.HitObject);
                    return true;
                }
            }
        }
        else if (collParas.type == CollisionDetectType.Rect)
        {
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
            float rate = (len - _width) / len;
            relativeVec *= rate;
            if (Mathf.Abs(relativeVec.x) < collParas.halfHeight && Mathf.Abs(relativeVec.y) < collParas.halfWidth)
            {
                bullet.Eliminate(eEliminateDef.HitObject);
                return true;
            }
        }
        else if (collParas.type == CollisionDetectType.Line)
        {
            float dis = MathUtil.GetMinDisFromPointToLineSegment(collParas.linePointA, collParas.linePointB, _curPos);
            if (dis <= _width + collParas.radius)
            {
                bullet.Eliminate(eEliminateDef.HitObject);
                return true;
            }
        }
        // 多线段集合，判断圆心到每个点的距离即可
        else if (collParas.type == CollisionDetectType.MultiSegments)
        {
            if (bullet.Id == BulletId.Enemy_CurveLaser)
            {
                EnemyCurveLaser curveLaser = bullet as EnemyCurveLaser;
                List<Vector2> pointList = collParas.multiSegmentPointList;
                int pointCount = pointList.Count;
                float dx, dy, sum;
                int eliminateStart = -1;
                int eliminateEnd = -1;
                for (int i = 0; i < pointCount; i++)
                {
                    dx = pointList[i].x - _curPosX;
                    dy = pointList[i].y - _curPosY;
                    sum = collParas.radius + _width;
                    if (dx * dx + dy * dy <= sum * sum)
                    {
                        if (eliminateStart == -1)
                        {
                            eliminateStart = i;
                        }
                        eliminateEnd = i;
                    }
                    else
                    {
                        if (eliminateStart != -1)
                        {
                            curveLaser.EliminateByRange(eliminateStart, eliminateEnd);
                            eliminateStart = -1;
                        }
                    }
                }
                if (eliminateStart != -1)
                {
                    curveLaser.EliminateByRange(eliminateStart, eliminateEnd);
                }
            }
            else if (bullet.Id == BulletId.Enemy_LinearLaser)
            {

            }
        }
        return false;
    }
}
