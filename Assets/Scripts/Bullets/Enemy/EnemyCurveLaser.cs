﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCurveLaser : EnemyBulletBase
{
    /// <summary>
    /// 擦弹判定冷却时间
    /// </summary>
    private const int GrazeCoolDown = 2;

    protected GameObject _bullet;
    protected Transform _trans;

    /// <summary>
    /// 轨迹集合
    /// </summary>
    protected List<Vector3> _trailsList;
    /// <summary>
    /// 长度
    /// </summary>
    protected int _maxTrailLen;
    protected int _curTrailLen;

    protected float _laserHalfWidth;

    protected Mesh _mesh;
    /// <summary>
    /// 是否需要重新计算三角形
    /// </summary>
    protected bool _triModified;
    /// <summary>
    /// 是否需要重新计算uv
    /// </summary>
    protected bool _uvModified;

    protected float _relationX, _relationY;

    protected MovableObject _movableObj;

    protected int _textureIndex;
    protected float _vUnit;
    protected float _startV;
    protected float _endV;

    #region 碰撞相关参数
    /// <summary>
    /// 擦弹检测类型，默认为矩形
    /// </summary>
    protected int _grazeType = Consts.GrazeType_Rect;
    protected float _grazeHalfWidth;
    protected float _grazeHalfHeight;
    /// <summary>
    /// 碰撞检测类型
    /// </summary>
    protected CollisionDetectType _collisionType;
    /// <summary>
    /// 线段判定半径
    /// </summary>
    protected float _collisionRadius;
    /// <summary>
    /// 擦弹检测半径，暂时默认碰撞检测的1.5倍
    /// </summary>
    private float _grazeRadius;

    private List<Vector3> _collisionPointList;
    #endregion
    /// <summary>
    /// 消去的范围
    /// </summary>
    private List<Vector2> _eliminateRangeList;
    /// <summary>
    /// 消去的范围数目
    /// </summary>
    private int _eliminateRangeListCount;
    /// <summary>
    /// 路径起始下标
    /// </summary>
    private int _pathBeginIndex;
    /// <summary>
    /// 路径结束下标
    /// </summary>
    private int _pathEndIndex;
    /// <summary>
    /// 有效的路径下标范围
    /// </summary>
    private List<Vector2> _availableIndexRangeList;
    /// <summary>
    /// 有效的激光段数目
    /// </summary>
    private int _availableCount;

    public EnemyCurveLaser()
    {
        _trailsList = new List<Vector3>();
        _id = BulletId.Enemy_CurveLaser;
        _prefabName = "CurveLaser";
        _sysBusyWeight = 20;
        _availableIndexRangeList = new List<Vector2>();
        _eliminateRangeList = new List<Vector2>();
        _collisionPointList = new List<Vector3>();
    }

    public override void Init()
    {
        base.Init();
        _isMoving = false;
        _curTrailLen = 0;
        _laserHalfWidth = 5;
        _collisionRadius = 3;
        _availableCount = 0;
        _eliminateRangeListCount = 0;
        if ( _movableObj == null )
        {
            _movableObj = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        }
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
    }

    public override void SetBulletTexture(string texture)
    {
        if ( _bullet == null )
        {
            _bullet = ResourceManager.GetInstance().GetPrefab("BulletPrefab", _prefabName);
            UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.EnemyBarrage);
        }
        _trans = _bullet.transform;
        Transform laserTrans = _trans.Find("LaserObject");
        _mesh = laserTrans.GetComponent<MeshFilter>().mesh;
        // 设置sortingLayer
        MeshRenderer renderer = laserTrans.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "STG";
        // todo 以后根据名称/配置来设置显示
        int index = int.Parse(texture.Substring(texture.IndexOf('_') + 1));
        //renderer.material.SetFloat("_Index", index);
        _textureIndex = index;
        _vUnit = 1f / 16;
        _startV = _vUnit * (index + 1);
        _endV = _vUnit * index;
    }

    public override void SetToPosition(float posX, float posY)
    {
        base.SetToPosition(posX, posY);
        _trans.localPosition = _curPos;
        _movableObj.SetPos(0, 0);
        _relationX = posX;
        _relationY = posY;
        _trailsList.Add(Vector3.zero);
    }

    /// <summary>
    /// 设置曲线激光的长度
    /// </summary>
    /// <param name="len"></param>
    public void SetLength(int len)
    {
        _maxTrailLen = len;
    }

    public void SetWidth(float halfWidth,float collisionHalfWidth)
    {
        _laserHalfWidth = halfWidth;
        _collisionRadius = collisionHalfWidth;
        _grazeRadius = collisionHalfWidth * 1.5f;
        // 设置碰撞参数
        _collisionParas = new CollisionDetectParas
        {
            type = CollisionDetectType.MultiSegments,
            radius = _collisionRadius,
            multiSegmentPointList = _collisionPointList,
        };
    }

    public override void Update()
    {
        _triModified = false;
        _uvModified = false;
        CheckDivideIntoMultiple();
        UpdateGrazeCoolDown();
        UpdatePath();
        if (IsOutOfBorder())
        {
            _clearFlag = 1;
            return;
        }
        PopulateMesh();
        CheckCollisionWithCharacter();
    }

    #region 设置碰撞检测相关参数
    public override void SetCollisionDetectParas(CollisionDetectParas paras)
    {
        _collisionParas = paras;
        _collisionRadius = paras.radius;
    }

    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
    }

    public override CollisionDetectParas GetCollisionDetectParas()
    {
        return _collisionParas;
    }
    #endregion

    public virtual void SetStraightParas(float v, float angle, float acce, float accAngle)
    {
        _movableObj.DoMoveStraight(v, angle);
        _movableObj.DoAcceleration(acce, accAngle);
    }

    public virtual void SetAcceParas(float acce,float accAngle,int duration)
    {
        _movableObj.DoAccelerationWithLimitation(acce, accAngle, duration);
    }

    public virtual void SetCurveParas(float radius,float angle,float deltaR,float omiga)
    {
        _movableObj.DoMoveCurve(radius, angle, deltaR, omiga);
    }

    protected virtual void UpdatePath()
    {
        _movableObj.Update();
        _curPos = _movableObj.GetPos();
        // 将当前位置添加到点集中
        if (_curTrailLen >= _maxTrailLen)
        {
            _trailsList.RemoveAt(0);
            _collisionPointList.RemoveAt(0);
        }
        else
        {
            // 标识三角形数据需要重新计算
            _triModified = true;
            _uvModified = true;
            // 长度增加，因此要修改availableRangeList
            _curTrailLen++;
            if ( _availableCount == 0 )
            {
                // 添加第一段
                _availableIndexRangeList.Add(new Vector2(0, 1));
                _availableCount = 1;
            }
            else
            {
                // 先检测第一段是否在起始位置
                Vector2 availableRange = _availableIndexRangeList[0];
                if ( availableRange.x == 0 )
                {
                    availableRange.y += 1;
                    _availableIndexRangeList[0] = availableRange;
                }
                else
                {
                    // 插入(0,1)段
                    _availableIndexRangeList.Add(new Vector2(0, 1));
                    _availableCount++;
                }
                for (int i=1;i<_availableCount;i++)
                {
                    availableRange = _availableIndexRangeList[i];
                    _availableIndexRangeList[i] = new Vector2(availableRange.x + 1, availableRange.y + 1);
                }
            }
        }
        _trailsList.Add(new Vector3(_curPos.x, _curPos.y, 0));
        _collisionPointList.Add(new Vector3(_curPos.x+_relationX, _curPos.y+_relationY, 0));
    }

    private void PopulateMesh()
    {
        if (_curTrailLen < 2) return;
        int i,startIndex,endIndex,tmpVerCount,totalVerCount;
        List<Vector3> tmpVerticesList = new List<Vector3>();
        List<Vector3> meshVerticesList = new List<Vector3>();
        List<int> triList = new List<int>();
        List<Vector2> uvList = new List<Vector2>();
        // 遍历availableIndexRange，计算需要渲染的顶点
        totalVerCount = 0;
        for (i=0;i<_availableCount;i++)
        {
            startIndex = (int)_availableIndexRangeList[i].x;
            endIndex = (int)_availableIndexRangeList[i].y;
            if ( endIndex - startIndex >= 1 )
            {
                tmpVerCount = GetVertices(startIndex, endIndex, tmpVerticesList);
                meshVerticesList.AddRange(tmpVerticesList);
                if ( _triModified )
                {
                    AddTris(totalVerCount, tmpVerCount, triList);
                }
                if ( _uvModified )
                {
                    AddUVs(startIndex, endIndex, uvList);
                }
                // 更新总顶点数
                totalVerCount += tmpVerCount;
                tmpVerticesList.Clear();
            }
        }
        // 渲染
        if ( _triModified )
        {
            _mesh.Clear();
        }
        //Logger.Log("Vertices Len = " + meshVerticesList.Count + " uv len = " + uvList.Count);
        _mesh.vertices = meshVerticesList.ToArray();
        if ( _triModified )
        {
            _mesh.triangles = triList.ToArray();
        }
        if ( _uvModified )
        {
            _mesh.uv = uvList.ToArray();
        }
    }

    /// <summary>
    /// 计算需要渲染的顶点
    /// </summary>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <param name="verticesList"></param>
    /// <returns>顶点个数</returns>
    private int GetVertices(int startIndex,int endIndex,List<Vector3> verticesList)
    {
        int i;
        Vector3 tmpPos0, tmpPos1 = Vector3.zero, vec0, vec1;
        Vector3 tmpVec, normalVec = Vector3.zero;
        float scale = 1f;
        for (i = startIndex; i < endIndex; i++)
        {
            tmpPos0 = _trailsList[i];
            tmpPos1 = _trailsList[i + 1];
            // 当前点指向下一个点的向量
            tmpVec = tmpPos1 - tmpPos0;
            // 当前点法线向量
            normalVec = new Vector3(-tmpVec.y, tmpVec.x);
            normalVec = normalVec.normalized;
            normalVec *= _laserHalfWidth;
            vec0 = new Vector3(tmpPos0.x + normalVec.x, tmpPos0.y + normalVec.y, 0);
            vec1 = new Vector3(tmpPos0.x - normalVec.x, tmpPos0.y - normalVec.y, 0);
            verticesList.Add(vec0 * scale);
            verticesList.Add(vec1 * scale);
        }
        verticesList.Add(new Vector3(tmpPos1.x + normalVec.x, tmpPos1.y + normalVec.y, 0) * scale);
        verticesList.Add(new Vector3(tmpPos1.x - normalVec.x, tmpPos1.y - normalVec.y, 0) * scale);
        return (endIndex+1-startIndex)<<1;
    }

    /// <summary>
    /// 增加三角形
    /// </summary>
    /// <param name="startIndex">起始点下标</param>
    /// <param name="count">数目</param>
    /// <param name="tris">添加到的三角形数组</param>
    private void AddTris(int startIndex, int count,List<int> tris)
    {
        int tmpIndex;
        for (int i = 0; i < count - 2; i+=2)
        {
            tmpIndex = startIndex + i;
            tris.Add(tmpIndex);
            tris.Add(tmpIndex + 2);
            tris.Add(tmpIndex + 1);
            tris.Add(tmpIndex + 2);
            tris.Add(tmpIndex + 3);
            tris.Add(tmpIndex + 1);
        }
    }

    /// <summary>
    /// 添加uv
    /// <para>注意：这个下标是指轨迹点的下标</para>
    /// <para>而不是mesh中顶点对应的下标</para>
    /// <para>一个下标对应2个vertices</para>
    /// </summary>
    /// <param name="startIndex">轨迹点起始下标</param>
    /// <param name="endIndex">轨迹点结束下标</param>
    /// <param name="uvs"></param>
    private void AddUVs(int startIndex,int endIndex,List<Vector2> uvs)
    {
        int segment = endIndex - startIndex;
        float segLen = 1f / segment;
        float u = 0f;
        for (int i = 0; i < segment; i++)
        {
            uvs.Add(new Vector2(u, _endV));
            uvs.Add(new Vector2(u, _startV));
            u += segLen;
        }
        uvs.Add(new Vector2(1, _endV));
        uvs.Add(new Vector2(1, _startV));
    }

    protected virtual void UpdatePos()
    {
        _trans.localPosition = _curPos;
    }

    protected override bool IsOutOfBorder()
    {
        Vector2 checkPoint = new Vector2(_trailsList[0].x+_relationX,_trailsList[0].y+_relationY);
        if ( checkPoint.x < Global.BulletLBBorderPos.x ||
            checkPoint.y < Global.BulletLBBorderPos.y ||
            checkPoint.x > Global.BulletRTBorderPos.x ||
            checkPoint.y > Global.BulletRTBorderPos.y )
        {
            return true;
        }
        return false;
    }

    private void CheckCollisionWithCharacter()
    {
        // 一组中检测的个数,以及对应的循环增量
        int numInGroup = 2, dNumInGroup = numInGroup - 1;
        int i, j, k, tmpIdx = 0;
        int startIndex, endIndex;
        Vector2 availableRange;
        float minX, minY, maxX, maxY, centerX, centerY, grazeHW, grazeHH, minDis;
        float playerX = Global.PlayerPos.x;
        float playerY = Global.PlayerPos.y;
        float playerGrazeRadius = Global.PlayerGrazeRadius;
        Vector3 vecA, vecB, vecP;
        for (k=0;k<_availableCount;k++)
        {
            availableRange = _availableIndexRangeList[k];
            startIndex = (int)availableRange.x;
            endIndex = (int)availableRange.y;
            for (i = startIndex; i < endIndex; i += dNumInGroup)
            {
                minX = _trailsList[i].x;
                minY = _trailsList[i].y;
                maxX = _trailsList[i].x;
                maxY = _trailsList[i].y;
                for (j = 1; j < numInGroup; j++)
                {
                    tmpIdx = i + j;
                    if (tmpIdx > endIndex)
                    {
                        // 取回上一个节点的下标
                        tmpIdx -= 1;
                        break;
                    }
                    minX = _trailsList[tmpIdx].x < minX ? _trailsList[tmpIdx].x : minX;
                    minY = _trailsList[tmpIdx].y < minY ? _trailsList[tmpIdx].y : minY;
                    maxX = _trailsList[tmpIdx].x > maxX ? _trailsList[tmpIdx].x : maxX;
                    maxY = _trailsList[tmpIdx].y > maxY ? _trailsList[tmpIdx].y : maxY;
                }
                centerX = (minX + maxX) / 2;
                centerY = (minY + maxY) / 2;
                grazeHW = centerX - minX + _grazeRadius;
                grazeHH = centerY - minY + _grazeRadius;
                // 检测是否在擦弹范围内
                if (Mathf.Abs(centerX + _relationX - playerX) <= grazeHW + playerGrazeRadius &&
                     Mathf.Abs(centerY + _relationY - playerY) <= grazeHH + playerGrazeRadius)
                {
                    if (!_isGrazed)
                    {
                        _isGrazed = true;
                        PlayerService.GetInstance().AddGraze(1);
                        _grazeCoolDown = GrazeCoolDown;
                    }
                    // 检测_trailsList[i]与_trailsList[tmpIdx]之间的线段与玩家的碰撞判定
                    vecA = new Vector2(_trailsList[i].x + _relationX, _trailsList[i].y + _relationY);
                    vecB = new Vector2(_trailsList[tmpIdx].x + _relationX, _trailsList[tmpIdx].y + _relationY);
                    vecP = new Vector2(playerX, playerY);
                    minDis = MathUtil.GetMinDisFromPointToLineSegment(vecA, vecB, vecP);
                    if (minDis < _collisionRadius + Global.PlayerCollisionVec.z)
                    {
                        EliminateByRange(i, tmpIdx);
                        PlayerService.GetInstance().GetCharacter().BeingHit();
                    }
                }
            }
        }
    }

    public void EliminateByRange(int startIndex,int endIndex)
    {
        _eliminateRangeList.Add(new Vector2(startIndex, endIndex));
        _eliminateRangeListCount++;
    }

    /// <summary>
    /// 检测是否因为部分被消除而分隔成多个激光
    /// </summary>
    private void CheckDivideIntoMultiple()
    {
        if (_eliminateRangeListCount == 0) return;
        DivideAvailableRange();
        _triModified = true;
        _uvModified = true;
    }

    /// <summary>
    /// 根据被消除的部分重新分割激光的有效范围
    /// </summary>
    private void DivideAvailableRange()
    {
        int i,j,segmentLen;
        Vector2 eliminateRange,availableRange,divideRange0,divideRange1;
        for (i=0;i<_eliminateRangeListCount;i++)
        {
            eliminateRange = _eliminateRangeList[i];
            //Logger.Log("--------------------------------------------------");
            //Logger.Log("eliminateRange = " + eliminateRange);
            for (j = 0; j < _availableCount; j++)
            {
                availableRange = _availableIndexRangeList[j];
                segmentLen = (int)(availableRange.y - availableRange.x);
                if ( segmentLen > 0 && (eliminateRange.x >= availableRange.y || eliminateRange.y <= availableRange.x) )
                {
                    continue;
                }
                //分四种情况，上为eliminateRange，下为availableRange
                // 最优先判断会被全部截取掉的情况
                //    ----------
                //      ------
                if (eliminateRange.x <= availableRange.x && eliminateRange.y >= availableRange.y)
                {
                    _availableIndexRangeList.RemoveAt(j);
                    _availableCount--;
                    j--;
                }
                //    ---------
                //  -----
                else if ( eliminateRange.x >= availableRange.x && eliminateRange.y >= availableRange.y )
                {
                    divideRange0 = new Vector2(availableRange.x, eliminateRange.x);
                    _availableIndexRangeList[j] = divideRange0;
                }
                //    ---------
                //  -------------
                else if (eliminateRange.x >= availableRange.x && eliminateRange.y <= availableRange.y )
                {
                    divideRange0 = new Vector2(availableRange.x, eliminateRange.x);
                    divideRange1 = new Vector2(eliminateRange.y, availableRange.y);
                    _availableIndexRangeList[j] = divideRange0;
                    _availableIndexRangeList.Insert(j + 1, divideRange1);
                    _availableCount++;
                    j++;
                }
                //    ---------
                //      -----------
                else if (eliminateRange.x <= availableRange.x && eliminateRange.y <= availableRange.y)
                {
                    divideRange0 = new Vector2(eliminateRange.y, availableRange.y);
                    _availableIndexRangeList[j] = divideRange0;
                }
            }
            //Logger.Log("----------  availableRange  -------------------------");
            //for (j = 0; j < _availableCount; j++)
            //{
            //    Logger.Log(_availableIndexRangeList[j]);
            //}
            //Logger.Log("--------------------------------------------------");
        }
        _eliminateRangeList.Clear();
        _eliminateRangeListCount = 0;
    }

    public override void Clear()
    {
        _trailsList.Clear();
        _collisionPointList.Clear();
        _curTrailLen = _maxTrailLen = 0;
        _mesh.Clear();
        _mesh = null;
        UIManager.GetInstance().RemoveGoFromLayer(_bullet);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        _bullet = null;
        _trans = null;
        // 清除movableObject
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObj);
        _movableObj = null;
        _availableIndexRangeList.Clear();
        base.Clear();
    }
}
