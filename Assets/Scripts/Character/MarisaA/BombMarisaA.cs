using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombMarisaA : BombBase
{
    private const int MasterSparkPosZ = -5;
    public const int MovementObjectCount = 5;
    private const int BombCount = 3;
    private const float DefaultRadius = 32;
    /// <summary>
    /// 魔炮位置与自机位置的偏移量
    /// </summary>
    private Vector3 _posOffset;
    private GameObject _masterSparkObject;
    private Transform _masterSparkTf;
    private Transform _movementBodyTf;
    private List<Transform> _movementObjectTfList;
    private List<MovementObjectData> _movementDataList;
    /// <summary>
    /// 移动的对象的数量
    /// </summary>
    private int _movementCount;

    struct MovementObjectData
    {
        public Transform tf;
        public int state;
        public int time;
        public int duration;
    };

    private int _curState;
    private int _time;
    private int _duration;
    private float _curScaleX;
    /// <summary>
    /// 下一个需要移动的环
    /// </summary>
    private int _nextMovementIndex;
    /// <summary>
    /// 魔炮时候的黑底SpriteEffect
    /// </summary>
    private SpriteEffect _effect;

    private float _detectRadius;
    private Vector2 _detectCenter;

    public BombMarisaA()
    {
        _posOffset = new Vector3(0, 200, 0);
        _movementObjectTfList = new List<Transform>();
        _movementDataList = new List<MovementObjectData>();
    }

    public override void Start()
    {
        _masterSparkObject = ResourceManager.GetInstance().GetPrefab("Prefab/Character/MarisaA", "MasterSpark");
        UIManager.GetInstance().AddGoToLayer(_masterSparkObject, LayerId.PlayerBarage);
        _masterSparkTf = _masterSparkObject.transform;
        Vector2 playerPos = Global.PlayerPos;
        Vector3 pos = new Vector3(playerPos.x + _posOffset.x, playerPos.y + _posOffset.y, MasterSparkPosZ);
        _masterSparkTf.localPosition = pos;
        Transform tf;
        for (int i=0;i< MovementObjectCount; i++)
        {
            tf = _masterSparkTf.Find("MovementBody" + i);
            _movementObjectTfList.Add(tf);
        }
        _movementBodyTf = _masterSparkTf.Find("MovementBody");
        // 黑底
        _effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as SpriteEffect;
        _effect.SetSprite("STGCommonAtlas","Circle",false);
        _effect.SetLayer(LayerId.STGBottomEffect);
        _effect.SetScale(0, 0);
        _effect.SetSpriteColor(0, 0, 0, 1);
        _effect.DoScaleWidth(16, 30, InterpolationMode.EaseInQuad);
        _effect.DoScaleHeight(16, 30, InterpolationMode.EaseInQuad);
        _effect.SetToPos(playerPos.x, playerPos.y);
        // 基础属性
        _curState = 1;
        _time = 0;
        _duration = 20;
        _isFinish = false;
    }

    public override void Update()
    {
        // 魔炮跟随自机移动
        Vector2 playerPos = Global.PlayerPos;
        Vector3 pos = new Vector3(playerPos.x + _posOffset.x, playerPos.y + _posOffset.y, MasterSparkPosZ);
        _masterSparkTf.localPosition = pos;
        if ( true )
        {
            float posY = _movementBodyTf.localPosition.y;
            posY += Random.Range(-1f, 1f);
            posY = Mathf.Clamp(posY, 0, 20);
            _movementBodyTf.localPosition = new Vector3(0, posY, -1);
        }
        // 更新状态
        if (_curState == 1)
        {
            UpdateState1();
        }
        else if (_curState == 2)
        {
            UpdateState2();
        }
        else if ( _curState == 3 )
        {
            UpdateState3();
        }
        else
        {
            _isFinish = true;
        }
    }

    /// <summary>
    /// 魔炮张开的阶段
    /// </summary>
    public void UpdateState1()
    {
        _curScaleX = MathUtil.GetEaseInQuadInterpolation(0, 1, _time, _duration);
        _masterSparkTf.localScale = new Vector3(_curScaleX, 1, 1);
        if (_time >= _duration)
        {
            _curState = 2;
            _nextMovementIndex = 0;
            _time = 0;
            _duration = 180;
        }
        _time++;
    }

    /// <summary>
    /// 魔炮持续时间
    /// </summary>
    public void UpdateState2()
    {
        // 每20帧一个动画
        if ( _time % 20 == 0 )
        {
            MovementObjectData data = new MovementObjectData();
            data.tf = _movementObjectTfList[_nextMovementIndex];
            data.tf.localPosition = new Vector3(0, -270, -2);
            data.state = 1;
            data.time = 0;
            data.duration = 20;
            _movementDataList.Add(data);
            _movementCount++;
            // 计算下一个需要取的下标
            _nextMovementIndex = (_nextMovementIndex + 1) % MovementObjectCount;
        }
        float scale;
        Transform tf;
        Vector3 pos;
        for (int i=0;i<_movementCount;i++)
        {
            MovementObjectData data = _movementDataList[i];
            tf = data.tf;
            if ( data.state == 1 )
            {
                scale = Mathf.Lerp(50, 125, (float)data.time / data.duration);
                tf.localScale = new Vector3(scale, scale, 1);
                if ( data.time >= data.duration )
                {
                    data.state = 2;
                }
            }
            pos = tf.localPosition;
            pos.y += 10f;
            if ( pos.y >= 300 )
            {
                // 将scale设置成0当做是隐藏
                tf.localScale = new Vector3(0, 0, 1);
                //  清空
                data.tf = null;
                _movementDataList.RemoveAt(i);
                _movementCount--;
                i--;
            }
            else
            {
                tf.localPosition = pos;
                data.time++;
                _movementDataList[i] = data;
            }
        }
        if ( _time >= _duration )
        {
            // 黑底Sprite消失
            _effect.DoFade(20);
            _curState = 3;
            _time = 0;
            _duration = 10;
        }
        _time++;
    }

    public void UpdateState3()
    {
        _curScaleX = MathUtil.GetEaseInQuadInterpolation(1, 0, _time, _duration);
        _masterSparkTf.localScale = new Vector3(_curScaleX, 1, 1);
        if (_time >= _duration)
        {
            _curState = 4;
        }
        _time++;
    }

    //private void CheckCollisionWithEnemy()
    //{
    //    List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
    //    // 对敌机依然采用矩形检测即可
    //    EnemyBase enemy;
    //    int count = enemyList.Count;
    //    int i, j;
    //    for (i = 0; i < BombCount; i++)
    //    {
    //        _detectCenter = _bombsTf[i].localPosition;
    //        for (j = 0; j < count; j++)
    //        {
    //            enemy = enemyList[j];
    //            if (enemy != null && enemy.IsInteractive)
    //            {
    //                CollisionDetectParas collParas = enemy.GetCollisionDetectParas();
    //                if (Mathf.Abs(_detectCenter.x - collParas.centerPos.x) <= _detectRadius + collParas.halfWidth &&
    //                    Mathf.Abs(_detectCenter.y - collParas.centerPos.y) <= _detectRadius + collParas.halfHeight)
    //                {
    //                    // 造成1点伤害
    //                    enemy.GetHit(1);
    //                }
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 碰撞检测，为计算简便，圆形B暂时统一使用方形判定
    /// </summary>
    /// <param name="collParas"></param>
    /// <returns></returns>
    private bool DetectCollision(CollisionDetectParas collParas)
    {
        if (collParas.type == CollisionDetectType.Circle)
        {
            // 子弹为圆周判定，近似取外切正四边形
            if (Mathf.Abs(_detectCenter.x - collParas.centerPos.x) <= _detectRadius + collParas.radius &&
                Mathf.Abs(_detectCenter.y - collParas.centerPos.y) <= _detectRadius + collParas.radius)
            {
                return true;
            }
        }
        else if (collParas.type == CollisionDetectType.Rect)
        {
            // 子弹为矩形判定
            // 以子弹中心点为圆心，将B的判定中心旋转angle的角度计算判定
            Vector2 vec = new Vector2(_detectCenter.x - collParas.centerPos.x, _detectCenter.y - collParas.centerPos.y);
            float cos = Mathf.Cos(collParas.angle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(collParas.angle * Mathf.Deg2Rad);
            Vector2 relativeVec = new Vector2();
            // 向量顺时针旋转laserAngle的度数
            relativeVec.x = cos * vec.x + sin * vec.y;
            relativeVec.y = -sin * vec.x + cos * vec.y;
            // 计算圆和矩形的碰撞
            float len = relativeVec.magnitude;
            float rate = (len - _detectRadius) / len;
            relativeVec *= rate;
            if (Mathf.Abs(relativeVec.x) < collParas.halfHeight && Mathf.Abs(relativeVec.y) < collParas.halfWidth)
            {
                return true;
            }
        }
        else if (collParas.type == CollisionDetectType.Line)
        {
            float dis = MathUtil.GetMinDisFromPointToLineSegment(collParas.linePointA, collParas.linePointB, _detectCenter);
            if (dis <= _detectRadius + collParas.radius)
            {
                return true;
            }
        }
        else if (collParas.type == CollisionDetectType.MultiSegments)
        {
            List<Vector2> pointList = collParas.multiSegmentPointList;
            int pointCount = pointList.Count;
            int groupNum = Consts.NumInMultiSegmentsGroup;
            int lastIndex;
            for (int i = 1; i < pointCount - 1; i = i + groupNum)
            {
                lastIndex = i + groupNum - 1;
                if (lastIndex >= pointCount - 1)
                {
                    lastIndex = pointCount - 1;
                }
                Vector2 vecA = new Vector2(pointList[i].x + collParas.centerPos.x, pointList[i].y + collParas.centerPos.y);
                Vector2 vecB = new Vector2(pointList[lastIndex].x + collParas.centerPos.x, pointList[lastIndex].y + collParas.centerPos.y);
                float minDis = MathUtil.GetMinDisFromPointToLineSegment(vecA, vecB, _detectCenter);
                if (minDis <= collParas.radius + _detectRadius)
                {
                    return true;
                }
            }
        }
        return false;
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
            float dx = Mathf.Abs(_detectCenter.x - collParas.centerPos.x);
            float dy = Mathf.Abs(_detectCenter.y - collParas.centerPos.y);
            // 两圆的半径和
            float sumOfRadius = _detectRadius + collParas.radius;
            if (dx <= sumOfRadius && dy <= sumOfRadius)
            {
                if (dx * dx + dy * dy <= sumOfRadius * sumOfRadius)
                {
                    bullet.Eliminate(eEliminateDef.PlayerSpellCard);
                    return true;
                }
            }
        }
        else if (collParas.type == CollisionDetectType.Rect)
        {
            // 子弹为矩形判定
            // 以子弹中心点为圆心，将B的判定中心旋转angle的角度计算判定
            Vector2 vec = new Vector2(_detectCenter.x - collParas.centerPos.x, _detectCenter.y - collParas.centerPos.y);
            float cos = Mathf.Cos(collParas.angle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(collParas.angle * Mathf.Deg2Rad);
            Vector2 relativeVec = new Vector2();
            // 向量顺时针旋转laserAngle的度数
            relativeVec.x = cos * vec.x + sin * vec.y;
            relativeVec.y = -sin * vec.x + cos * vec.y;
            // 计算圆和矩形的碰撞
            float len = relativeVec.magnitude;
            float rate = (len - _detectRadius) / len;
            relativeVec *= rate;
            if (Mathf.Abs(relativeVec.x) < collParas.halfHeight && Mathf.Abs(relativeVec.y) < collParas.halfWidth)
            {
                bullet.Eliminate(eEliminateDef.PlayerSpellCard);
                return true;
            }
        }
        else if (collParas.type == CollisionDetectType.Line)
        {
            float dis = MathUtil.GetMinDisFromPointToLineSegment(collParas.linePointA, collParas.linePointB, _detectCenter);
            if (dis <= _detectRadius + collParas.radius)
            {
                bullet.Eliminate(eEliminateDef.PlayerSpellCard);
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
                    dx = pointList[i].x - _detectCenter.x;
                    dy = pointList[i].y - _detectCenter.y;
                    sum = collParas.radius + _detectRadius;
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

    public override void Clear()
    {
        _effect = null;
        _movementObjectTfList.Clear();
        _movementDataList.Clear();
        GameObject.Destroy(_masterSparkObject);
        _masterSparkObject = null;
        _masterSparkTf = null;
        _movementBodyTf = null;
        _movementCount = 0;
    }
}
