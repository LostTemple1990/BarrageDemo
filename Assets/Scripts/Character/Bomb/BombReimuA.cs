﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BombReimuA : BombBase
{
    private const int BombCount = 3;
    private const float DefaultRadius = 20;
    /// <summary>
    /// 生成炸弹的地点偏移量
    /// </summary>
    private List<Vector3> _posOffsets;
    private List<GameObject> _bombs;
    private List<Transform> _bombsTf;
    private List<Color> _colors;

    private int _curState;
    private int _time;
    private int _duration;
    private float _curScale;
    private float _detectRadius;
    private Vector2 _detectCenter;

    public BombReimuA()
    {
        _posOffsets = new List<Vector3>();
        _posOffsets.Add(new Vector3(0,20,0));
        _posOffsets.Add(new Vector3(-14,-14,0));
        _posOffsets.Add(new Vector3(14,-14,0));
        _bombs = new List<GameObject>();
        _bombsTf = new List<Transform>();
        _colors = new List<Color>();
        _colors.Add(new Color(0.5f, 0, 0.5f, 0.5f));
        _colors.Add(new Color(0, 0, 0.5f, 0.5f));
        _colors.Add(new Color(0, 0.5f, 0, 0.5f));
    }

    public override void Start()
    {
        int i;
        GameObject go;
        Transform tf;
        Vector2 playerPos = Global.PlayerPos;
        for (i=0;i<BombCount;i++)
        {
            go = ResourceManager.GetInstance().GetPrefab("Prefab/Character", "ReimuABomb");
            tf = go.transform;
            UIManager.GetInstance().AddGoToLayer(go, LayerId.GameEffect);
            _bombs.Add(go);
            _bombsTf.Add(tf);
            tf.localPosition = new Vector3(playerPos.x + _posOffsets[i].x, playerPos.y + _posOffsets[i].y, 0);
            tf.localScale = Vector3.zero;
            SpriteRenderer sp = tf.Find("Bomb").GetComponent<SpriteRenderer>();
            sp.material.SetColor("_Color", _colors[i]);
        }
        _curState = 1;
        _time = 0;
        _duration = 180;
        _isFinish = false;
    }

    public override void Update()
    {
        if ( _curState == 1 )
        {
            UpdateState1();
        }
        else if ( _curState == 2 )
        {
            UpdateState2();
        }
        else
        {
            _isFinish = true;
        }
    }

    public void UpdateState1()
    {
        _time++;
        _curScale = MathUtil.GetEaseOutQuadInterpolation(0, 2f, _time, _duration);
        _detectRadius = _curScale * DefaultRadius;
        CheckCollisionWithEnemyBullets();
        CheckCollisionWithEnemy();
        for (int i=0;i<BombCount;i++)
        {
            _bombsTf[i].localScale = new Vector3(_curScale, _curScale, 1);
        }
        if (_time >= _duration)
        {
            _curState = 2;
            _time = 0;
            _duration = 60;
        }
    }

    public void UpdateState2()
    {
        _time++;
        _curScale = MathUtil.GetEaseOutQuadInterpolation(2, 20, _time, _duration);
        _detectRadius = _curScale * DefaultRadius;
        CheckCollisionWithEnemyBullets();
        CheckCollisionWithEnemy();
        for (int i = 0; i < BombCount; i++)
        {
            _bombsTf[i].localScale = new Vector3(_curScale, _curScale, 1);
        }
        if ( _time >= _duration )
        {
            // 全屏伤害
            List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
            int count = enemyList.Count;
            for (int i=0;i<count;i++)
            {
                if ( enemyList[i] != null )
                {
                    enemyList[i].DoHit(400);
                }
            }
            _curState = 3;
        }
    }

    private void CheckCollisionWithEnemy()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        // 对敌机依然采用矩形检测即可
        EnemyBase enemy;
        int count = enemyList.Count;
        int i,j;
        for (i=0;i<BombCount;i++)
        {
            _detectCenter = _bombsTf[i].localPosition;
            for (j = 0; j < count; j++)
            {
                enemy = enemyList[j];
                if (enemy != null)
                {
                    CollisionDetectParas collParas = enemy.GetCollisionDetectParas();
                    if ( Mathf.Abs(_detectCenter.x-collParas.centerPos.x) <= _detectRadius + collParas.halfWidth &&
                        Mathf.Abs(_detectCenter.y - collParas.centerPos.y) <= _detectRadius + collParas.halfHeight )
                    {
                        // 造成1点伤害
                        enemy.DoHit(1);
                    }
                }
            }
        }
    }

    private void CheckCollisionWithEnemyBullets()
    {
        Vector3 pos;
        int i, j,bulletCount;
        List<EnemyBulletBase> bulletList = BulletsManager.GetInstance().GetEnemyBulletList();
        EnemyBulletBase bullet;
        for (i=0;i<BombCount;i++)
        {
            pos = _bombsTf[i].localPosition;
            _detectCenter = _bombsTf[i].localPosition;
            bulletCount = bulletList.Count;
            for (j=0;j<bulletCount;j++)
            {
                bullet = bulletList[j];
                // 判断是否要进行碰撞检测
                if ( bullet != null && bullet.CanBeEliminated(eEliminateDef.PlayerBomb) && bullet.ClearFlag == 0 )
                {
                    CollisionDetectParas collParas = bullet.GetCollisionDetectParas();
                    if ( DetectCollision(collParas) )
                    {
                        bullet.Eliminate(eEliminateDef.PlayerBomb);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 碰撞检测，为计算简便，圆形B暂时统一使用方形判定
    /// </summary>
    /// <param name="collParas"></param>
    /// <returns></returns>
    private bool DetectCollision(CollisionDetectParas collParas)
    {
        if ( collParas.type == CollisionDetectType.Circle )
        {
            // 子弹为圆周判定，近似取外切正四边形
            if ( Mathf.Abs(_detectCenter.x-collParas.centerPos.x) <= _detectRadius + collParas.radius &&
                Mathf.Abs(_detectCenter.y - collParas.centerPos.y) <= _detectRadius + collParas.radius )
            {
                return true;
            }
        }
        else if ( collParas.type == CollisionDetectType.Rect )
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
            if (Mathf.Abs(relativeVec.x) < collParas.halfHeight && Mathf.Abs(relativeVec.y) < collParas.halfWidth )
            {
                return true;
            }
        }
        else if ( collParas.type == CollisionDetectType.Line )
        {
            float dis = MathUtil.GetMinDisFromPointToLineSegment(collParas.linePointA, collParas.linePointB, _detectCenter);
            if ( dis <= _detectRadius + collParas.radius )
            {
                return true;
            }
        }
        // todo 曲线激光暂时写成碰到就消
        else if ( collParas.type == CollisionDetectType.MultiSegments )
        {
            List<Vector2> pointList = Global.MultiSegmentsCollisionPointList;
            int pointCount = pointList.Count;
            int groupNum = Consts.NumInMultiSegmentsGroup;
            int lastIndex;
            for (int i=1;i<pointCount-1;i=i+groupNum)
            {
                lastIndex = i + groupNum - 1;
                if ( lastIndex >= pointCount - 1 )
                {
                    lastIndex = pointCount - 1;
                }
                Vector2 vecA = new Vector2(pointList[i].x + collParas.centerPos.x, pointList[i].y + collParas.centerPos.y);
                Vector2 vecB = new Vector2(pointList[lastIndex].x + collParas.centerPos.x, pointList[lastIndex].y + collParas.centerPos.y);
                float minDis = MathUtil.GetMinDisFromPointToLineSegment(vecA, vecB, _detectCenter);
                if ( minDis <= collParas.radius + _detectRadius )
                {
                    return true;
                }
            }
        }
        return false;
    }

    public override void Clear()
    {
        for (int i=0;i<BombCount;i++)
        {
            GameObject.Destroy(_bombs[i]);
        }
        _bombs.Clear();
        _bombsTf.Clear();
    }
}
