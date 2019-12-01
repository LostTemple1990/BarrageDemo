using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCurveLaser : EnemyBulletBase
{
    /// <summary>
    /// 擦弹判定冷却时间
    /// </summary>
    private const int GrazeCoolDown = 2;
    private const int CollisionSegmentLen = 1;

    protected GameObject _bullet;
    protected Transform _trans;

    /// <summary>
    /// 轨迹集合
    /// </summary>
    protected List<Vector2> _trailsList;
    /// <summary>
    /// 长度
    /// </summary>
    protected int _maxTrailLen;
    /// <summary>
    /// 当前激光的长度
    /// <para>即，路径点的个数-1</para>
    /// <para>在曲线激光的处理中，第一个加入的点为vec2(0,0)</para>
    /// </summary>
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
    private bool _isPosModified;
    /// <summary>
    /// 是否已经初始化位置
    /// </summary>
    private bool _isInitPos;

    protected MovableObject _movableObj;

    protected int _textureIndex;
    protected float _vUnit;
    protected float _startV;
    protected float _endV;

    #region 碰撞相关参数
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
    /// <summary>
    /// 是否已经缓存好碰撞的段
    /// </summary>
    private bool _isCachedCollisionSegments;
    private List<Vector2> _collisionSegmentsList;
    private int _collisionSegmentsCount;
    /// <summary>
    /// 配置
    /// </summary>
    private EnemyCurveLaserCfg _cfg;

    public EnemyCurveLaser()
    {
        _trailsList = new List<Vector2>();
        _type = BulletType.Enemy_CurveLaser;
        _prefabName = "CurveLaser";
        _sysBusyWeight = 20;
        _availableIndexRangeList = new List<Vector2>();
        _eliminateRangeList = new List<Vector2>();
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
        _isCachedCollisionSegments = false;
        _collisionSegmentsList = new List<Vector2>();
        _collisionSegmentsCount = 0;
        _isPosModified = false;
        _isInitPos = false;
        if ( _movableObj == null )
        {
            _movableObj = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        }
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
    }

    public override void SetStyleById(string id)
    {
        EnemyCurveLaserCfg cfg = BulletsManager.GetInstance().GetCurveLaserCfgById(id);
        if ( cfg == null )
        {
            Logger.LogError("Bullet ChangeStyle Fail! SysId  " + id + " is not exist!");
            return;
        }
        if ( _bullet != null )
        {
            ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        }
        _cfg = cfg;
        _bullet = BulletsManager.GetInstance().CreateBulletGameObject(BulletType.Enemy_CurveLaser, cfg.id);
        _prefabName = cfg.id;
        _trans = _bullet.transform;
        Transform laserTrans = _trans.Find("LaserObject");
        _mesh = laserTrans.GetComponent<MeshFilter>().mesh;
        if (_cfg.aniCount == 0)
        {
            int index = cfg.colorIndex;
            _textureIndex = 16 - index;
            _vUnit = 1f / cfg.colorsCount;
            _startV = _vUnit * (_textureIndex - 1);
            _endV = _vUnit * _textureIndex;
        }
        _isPosModified = true;
    }

    public override void SetPosition(float posX, float posY)
    {
        if (!_isInitPos)
        {
            _isInitPos = true;
            _relationX = posX;
            _relationY = posY;
            _trailsList.Add(Vector2.zero);
            _isPosModified = true;
        }
        base.SetPosition(posX, posY);
        _movableObj.SetPos(posX, posY);
    }

    public override void SetPosition(Vector2 pos)
    {
        SetPosition(pos.x, pos.y);
    }

    /// <summary>
    /// 设置曲线激光的长度
    /// </summary>
    /// <param name="len"></param>
    public void SetLength(int len)
    {
        _maxTrailLen = len;
    }

    public void SetWidth(float width)
    {
        _laserHalfWidth = width / 2;
        _collisionRadius = _laserHalfWidth * 0.45f;
        _grazeRadius = _laserHalfWidth + 12;
    }

    public override void Update()
    {
        base.Update();
        UpdateComponents();
        CheckDivideIntoMultiple();
        UpdateGrazeCoolDown();
        UpdatePath();
        if (IsOutOfBorder())
        {
            _clearFlag = 1;
            return;
        }
        CheckCollisionWithCharacter();
    }

    public override void Render()
    {
        // 检测是否需要重新计算uv
        if (_cfg.aniCount != 0)
        {
            if (_timeSinceCreated % 4 == 0)
                _uvModified = true;
        }
        PopulateMesh();
        if (_isPosModified)
            RenderPosition();
    }

    protected override void OnFrameStarted()
    {
        _triModified = false;
        _uvModified = false;
        base.OnFrameStarted();
    }

    #region 设置碰撞检测相关参数

    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
    }

    /// <summary>
    /// 曲线激光不进行碰撞盒判定
    /// </summary>
    /// <param name="lbPos"></param>
    /// <param name="rtPos"></param>
    /// <returns></returns>
    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        return true;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        if ( !_isCachedCollisionSegments )
        {
            CacheCollisionSegments();
        }
        if ( _collisionSegmentsCount == 0 )
        {
            return new CollisionDetectParas
            {
                type = CollisionDetectType.Null,
                nextIndex = -1,
            };
        }
        int realIndex = index >= 0 ? index : _collisionSegmentsCount + index;
        Vector2 segmentVec = _collisionSegmentsList[realIndex];
        Vector2 centerPos = (_trailsList[(int)segmentVec.x] + _trailsList[(int)segmentVec.y]) / 2 + new Vector2(_relationX, _relationY);
        return new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            centerPos = centerPos,
            radius = _collisionRadius,
            nextIndex = realIndex + 1 >= _collisionSegmentsCount ? -1 : realIndex + 1,
        };
    }

    /// <summary>
    /// 缓存碰撞段
    /// <para></para>
    /// </summary>
    private void CacheCollisionSegments()
    {
        _collisionSegmentsList.Clear();
        _collisionSegmentsCount = 0;
        int endIndex,segmentEndIndex;
        Vector2 segmentVec;
        for (int i=0;i<_availableCount;i++)
        {
            segmentVec = _availableIndexRangeList[i];
            segmentEndIndex = (int)segmentVec.y;
            for (int j=(int)segmentVec.x;j< segmentEndIndex; j+=CollisionSegmentLen)
            {
                endIndex = j + CollisionSegmentLen > segmentEndIndex ? segmentEndIndex : j + CollisionSegmentLen;
                // 该段不为一个点，则加入到碰撞段中
                if ( endIndex != j )
                {
                    _collisionSegmentsList.Add(new Vector2(j, endIndex));
                    _collisionSegmentsCount++;
                }
            }
        }
        _isCachedCollisionSegments = true;
    }

    public override void CollidedByObject(int n = 0, eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {
        _eliminateRangeList.Add(_collisionSegmentsList[n]);
        _eliminateRangeListCount++;
    }
    #endregion

    #region ISTGMovable
    public override void DoStraightMove(float v, float angle)
    {
        _movableObj.DoStraightMove(v, angle);
    }

    public override void DoAcceleration(float acce, float accAngle)
    {
        _movableObj.DoAcceleration(acce, accAngle);
    }

    public override void SetStraightParas(float v, float angle, float acce, float accAngle)
    {
        _movableObj.SetStraightParas(v, angle, acce, accAngle);
    }

    public override void DoAccelerationWithLimitation(float acce,float accAngle,float maxVelocity)
    {
        _movableObj.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
    }

    public override void SetPolarParas(float radius,float angle,float deltaR,float omega)
    {
        _movableObj.SetPolarParas(radius, angle, deltaR, omega);
    }

    public override void SetPolarParas(float radius, float angle, float deltaR, float omega, float centerPosX, float centerPosY)
    {
        _movableObj.SetPolarParas(radius, angle, deltaR, omega, centerPosX, centerPosY);
    }

    public override float velocity
    {
        get { return _movableObj.velocity; }
        set { _movableObj.velocity = value; }
    }

    public override float vx
    {
        get { return _movableObj.vx; }
        set { _movableObj.vx = value; }
    }

    public override float vy
    {
        get { return _movableObj.vy; }
        set { _movableObj.vy = value; }
    }

    public override float maxVelocity
    {
        get { return _movableObj.maxVelocity; }
        set { _movableObj.maxVelocity = value; }
    }

    public override float vAngle
    {
        get { return _movableObj.vAngle; }
        set { _movableObj.vAngle = value; }
    }

    public override float acce
    {
        get { return _movableObj.acce; }
        set { _movableObj.acce = value; }
    }

    public override float accAngle
    {
        get { return _movableObj.accAngle; }
        set { _movableObj.accAngle = value; }
    }

    #endregion

    public override void AddExtraSpeedParas(float v, float vAngle, float acce, float accAngle)
    {
        _movableObj.AddExtraSpeedParas(v, vAngle, acce, accAngle);
    }

    protected virtual void UpdatePath()
    {
        _movableObj.Update();
        _curPos = _movableObj.GetPos();
        // 将当前位置添加到点集中
        if (_curTrailLen >= _maxTrailLen)
        {
            _trailsList.RemoveAt(0);
        }
        else
        {
            // 标识三角形数据需要重新计算
            _triModified = true;
            _uvModified = true;
            _isCachedCollisionSegments = false;
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
        _trailsList.Add(new Vector2(_curPos.x - _relationX, _curPos.y - _relationY));
    }

    private void PopulateMesh()
    {
        if (_curTrailLen < 1) return;
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
        if (_cfg.aniCount > 0)
        {
            int index = (_timeSinceCreated >> 2) % 4;
            _startV = 1f / 4 * index;
            _endV = 1f / 4 * (index + 1);
        }
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

    protected void RenderPosition()
    {
        _trans.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
        _isPosModified = false;
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
                        PlayerInterface.GetInstance().AddGraze(1);
                        _grazeCoolDown = GrazeCoolDown;
                    }
                    // 检测_trailsList[i]与_trailsList[tmpIdx]之间的线段与玩家的碰撞判定
                    vecA = new Vector2(_trailsList[i].x + _relationX, _trailsList[i].y + _relationY);
                    vecB = new Vector2(_trailsList[tmpIdx].x + _relationX, _trailsList[tmpIdx].y + _relationY);
                    vecP = new Vector2(playerX, playerY);
                    minDis = MathUtil.GetMinDisFromPointToLineSegment(vecA, vecB, vecP);
                    if (minDis < _collisionRadius + Global.PlayerCollisionVec.z)
                    {
                        //EliminateByRange(i, tmpIdx);
                        PlayerInterface.GetInstance().GetCharacter().BeingHit();
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
        _isCachedCollisionSegments = true;
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
                //      -----------
                else if (eliminateRange.x <= availableRange.x && eliminateRange.y <= availableRange.y)
                {
                    divideRange0 = new Vector2(eliminateRange.y, availableRange.y);
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

    public override bool GetBulletPara(BulletParaType paraType, out float value)
    {
        switch ( paraType)
        {
            case BulletParaType.Velocity:
                value = _movableObj.velocity;
                return true;
            case BulletParaType.VAngel:
                value = _movableObj.vAngle;
                return true;
            case BulletParaType.Acce:
                value = _movableObj.acce;
                return true;
            case BulletParaType.AccAngle:
                value = _movableObj.accAngle;
                return true;
            case BulletParaType.CurveRadius:
                value = _movableObj.curveRadius;
                return true;
            case BulletParaType.CurveAngle:
                value = _movableObj.curveAngle;
                return true;
            case BulletParaType.CurveDeltaR:
                value = _movableObj.curveDeltaRadius;
                return true;
            case BulletParaType.CurveOmega:
                value = _movableObj.curveOmega;
                return true;
            case BulletParaType.MaxVelocity:
                value = _movableObj.maxVelocity;
                return true;
        }
        value = 0;
        return false;
    }

    public override bool SetBulletPara(BulletParaType paraType, float value)
    {
        switch (paraType)
        {
            case BulletParaType.Velocity:
                _movableObj.velocity = value;
                return true;
            case BulletParaType.VAngel:
                _movableObj.vAngle = value;
                return true;
            case BulletParaType.Acce:
                _movableObj.acce = value;
                return true;
            case BulletParaType.AccAngle:
                _movableObj.accAngle = value;
                return true;
            case BulletParaType.CurveRadius:
                _movableObj.curveRadius = value;
                return true;
            case BulletParaType.CurveAngle:
                _movableObj.curveAngle = value;
                return true;
            case BulletParaType.CurveDeltaR:
                _movableObj.curveDeltaRadius = value;
                return true;
            case BulletParaType.CurveOmega:
                _movableObj.curveOmega = value;
                return true;
            case BulletParaType.MaxVelocity:
                _movableObj.maxVelocity = value;
                return true;
        }
        return false;
    }

    public override void SetOrderInLayer(int orderInLayer)
    {
        base.SetOrderInLayer(orderInLayer);
        _isPosModified = true;
    }

    public override float GetRotation()
    {
        return _movableObj.vAngle;
    }

    public override void Clear()
    {
        _trailsList.Clear();
        _curTrailLen = _maxTrailLen = 0;
        _mesh.Clear();
        _mesh = null;
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
