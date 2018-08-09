using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCurveLaser : EnemyBulletBase
{
    protected GameObject _bullet;
    protected Transform _trans;
    protected Image _bulletImg;

    protected float _curAccAngle;
    protected float _dvx, _dvy;

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
    // 是否需要重新计算uv
    protected int _uvFlag;

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
    #endregion

    public EnemyCurveLaser()
    {
        _trailsList = new List<Vector3>();
        _id = BulletId.BulletId_Enemy_CurveLaser;
        _prefabName = "LaserCurve3D";
    }

    public override void Init()
    {
        base.Init();
        if (_bullet == null)
        {
            //_bullet = ResourceManager.GetInstance().GetPrefab("Prefab", "EnemyBulletNormal");
            //_trans = _bullet.transform;
            //_bulletImg = _bullet.GetComponent<Image>();
            //UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.EnemyBarrage);
        }
        _isMoving = false;
        // 先写死激光最大长度
        _maxTrailLen = 45;
        _curTrailLen = 0;
        _laserHalfWidth = 5;
        _collisionRadius = 3;
        if ( _movableObj == null )
        {
            _movableObj = new MovableObject();
        }
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        Logger.Log("Create new CurveLaser");
    }

    public override void SetBulletTexture(string texture)
    {
        //_bulletImg.texture = ResourceManager.GetInstance().GetTexture("etama",texture);

        //TimeUtil.BeginSample(100);
        //_bulletImg.sprite = ResourceManager.GetInstance().GetResource<Sprite>("etama", texture);
        //TimeUtil.EndSample();
        //_bulletImg.SetNativeSize();
        _bullet = ObjectsPool.GetInstance().CreateBulletPrefab(_prefabName);
        _trans = _bullet.transform;
        Transform laserTrans = _trans.Find("LaserObject");
        _mesh = laserTrans.GetComponent<MeshFilter>().mesh;
        MeshRenderer renderer = laserTrans.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = "STG";
        // todo resourceManager.getTexture以后需要改成正式的
        //renderer.material.mainTexture = ResourceManager.GetInstance().GetTexture("etama9", texture);
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
    }

    public override void Update()
    {
        UpdatePath();
        if (IsOutOfBorder())
        {
            _clearFlag = 1;
            return;
        }
        PopulateMesh();
        //UpdatePos();
        CheckCollisionWithCharacter();
    }

    #region 设置碰撞检测相关参数
    public override void SetCollisionDetectParas(CollisionDetectParas paras)
    {
        _collisionRadius = paras.radius;
    }

    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
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
            _uvFlag = 0;
            _trailsList.RemoveAt(0);
        }
        else
        {
            _curTrailLen++;
            if (_curTrailLen > 2)
            {
                _uvFlag = 1;
            }
        }
        _trailsList.Add(new Vector3(_curPos.x, _curPos.y, 0));
    }

    protected override void Move()
    {
        // 更新速度
        _vx += _dvx;
        _vy += _dvy;
        // 更新位置
        _curPos.x += _vx;
        _curPos.y += _vy;
        // 将当前位置添加到点集中
        if ( _curTrailLen >= _maxTrailLen )
        {
            _uvFlag = 0;
            _trailsList.RemoveAt(0);
        }
        else
        {
            _curTrailLen++;
            if ( _curTrailLen > 2 )
            {
                _uvFlag = 1;
            }
        }
        _trailsList.Add(new Vector3(_curPos.x-_relationX,_curPos.y-_relationY,0));
        if (IsOutOfBorder())
        {
            _clearFlag = 1;
            return;
        }
    }

    protected void PopulateMesh()
    {
        if ( _curTrailLen >= 2 )
        {
            GetVertices();
            if ( _uvFlag == 1 )
            {
                GetTris();
                GetUVs();
            }
        }
    }

    protected void GetVertices()
    {
        List<Vector3> verticesList = new List<Vector3>();
        int i;
        Vector3 tmpPos0,tmpPos1=Vector3.zero,vec0,vec1;
        Vector3 tmpVec,normalVec=Vector3.zero;
        float scale = 1f;
        for (i=0;i<_curTrailLen-1;i++)
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
            verticesList.Add(vec0*scale);
            verticesList.Add(vec1*scale);
        }
        verticesList.Add(new Vector3(tmpPos1.x + normalVec.x, tmpPos1.y + normalVec.y, 0)*scale);
        verticesList.Add(new Vector3(tmpPos1.x - normalVec.x, tmpPos1.y - normalVec.y, 0)*scale);
        _mesh.vertices = verticesList.ToArray();
    }

    protected void GetTris()
    {
        List<int> tris = new List<int>();
        int tmpIndex;
        for (int i=0;i< _curTrailLen-1; i++)
        {
            tmpIndex = i * 2;
            tris.Add(tmpIndex);
            tris.Add(tmpIndex + 2);
            tris.Add(tmpIndex + 1);
            tris.Add(tmpIndex + 2);
            tris.Add(tmpIndex + 3);
            tris.Add(tmpIndex + 1);
        }
        _mesh.triangles = tris.ToArray();
    }

    protected void GetUVs()
    {
        int segment = _curTrailLen - 1;
        float segLen = 1f / segment;
        float u = 0f;
        List<Vector2> uvs = new List<Vector2>();
        for (int i=0;i<_curTrailLen-1;i++)
        {
            uvs.Add(new Vector2(u, _endV));
            uvs.Add(new Vector2(u, _startV));
            u += segLen;
        }
        uvs.Add(new Vector2(1, _endV));
        uvs.Add(new Vector2(1, _startV));
        _mesh.uv = uvs.ToArray();
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


    protected virtual void CheckCollisionWithCharacter()
    {
        // 一组中检测的个数
        int numInGroup = 2;
        int i,j,tmpIdx=0;
        float minX, minY, maxX, maxY,centerX,centerY,grazeHW,grazeHH,minDis;
        float playerX = Global.PlayerPos.x;
        float playerY = Global.PlayerPos.y;
        Vector3 vecA, vecB,vecP;
        // 头尾两端不附加判定
        for (i=1;i<_curTrailLen-1;i=i+numInGroup)
        {
            minX = _trailsList[i].x;
            minY = _trailsList[i].y;
            maxX = _trailsList[i].x;
            maxY = _trailsList[i].y;
            for (j=1;j<numInGroup;j++)
            {
                tmpIdx = i + j;
                if ( tmpIdx >= _curTrailLen-1 )
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
            grazeHW = centerX - minX + _grazeHalfWidth;
            grazeHH = centerY - minY + _grazeHalfHeight;
            // 检测是否在擦弹范围内
            if ( Mathf.Abs(centerX+_relationX-playerX) <= grazeHW + Global.PlayerCollisionVec.x &&
                 Mathf.Abs(centerY + _relationY - playerY) <= grazeHH + Global.PlayerCollisionVec.y )
            {
                // TODO 擦弹数+1
                // 检测_trailsList[i]与_trailsList[tmpIdx]之间的线段与玩家的碰撞判定
                vecA = new Vector2(_trailsList[i].x + _relationX, _trailsList[i].y + _relationY);
                vecB = new Vector2(_trailsList[tmpIdx].x + _relationX, _trailsList[tmpIdx].y + _relationY);
                vecP = new Vector2(playerX, playerY);
                minDis = MathUtil.GetMinDisFromPointToLineSegment(vecA, vecB, vecP);
                if ( minDis < _collisionRadius + Global.PlayerCollisionVec.z )
                {
                    _clearFlag = 1;
                    PlayerService.GetInstance().GetCharacter().BeingHit();
                    //Logger.Log("Curve Laser Hit Player!");
                }
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        _trailsList.Clear();
        _curTrailLen = _maxTrailLen = 0;
        _mesh.Clear();
        UIManager.GetInstance().RemoveGoFromLayer(_bullet);
        ObjectsPool.GetInstance().RestoreBullet(_prefabName, _bullet);
        _bullet = null;
    }
}
