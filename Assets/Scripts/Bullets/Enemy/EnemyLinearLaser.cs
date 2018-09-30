using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLinearLaser : EnemyBulletBase
{
    private const float DefaultLaserHalfHeight = 6f;
    private const float DefaultCollisionHalfHeight = 3f;

    private const string HeadSpriteNameWhite = "LaserHead_W";
    private const string HeadSpriteNameBlue = "LaserHead_B";
    private const string HeadSpriteNameRed = "LaserHead_R";
    private const string HeadSpriteNameGreen = "LaserHead_G";

    private const string SourceSpriteName = "LaserSource_";
    /// <summary>
    /// 发射源的自旋速度
    /// </summary>
    private static Vector3 SourceSpinSpeed = new Vector3(0, 0, 18);

    private const int HeadAniInterval = 6;
    private const int HeadAniTotalFrame = 4;
    /// <summary>
    /// 擦弹冷却时间
    /// </summary>
    private const int GrazeCoolDown = 2;

    private static string GetHeadSpriteNameByType(eLaserHeadType type)
    {
        switch ( type )
        {
            case eLaserHeadType.Blue:
                return HeadSpriteNameBlue;
            case eLaserHeadType.Green:
                return HeadSpriteNameGreen;
            case eLaserHeadType.White:
                return HeadSpriteNameWhite;
            case eLaserHeadType.Red:
                return HeadSpriteNameRed;
        }
        return "";
    }

    protected MovableObject _movableObj;
    protected List<Vector2> _pathList;
    protected int _pathCount;
    protected int _laserLen;
    protected int _existDuration;
    protected int _pathBeginIndex;
    protected int _pathEndIndex;

    protected int _accDuration;
    protected string _textureName;

    protected float _grazeHalfWidth;
    protected float _grazeHalfHeight;
    protected float _collisionHalfWidth;
    protected float _collisionHalfHeight;

    protected float _laserHalfHeight;
    protected float _laserHalfWidth;
    protected bool _isSized;

    /// <summary>
    /// 激光容器Object
    /// </summary>
    protected GameObject _laserObj;
    /// <summary>
    /// 激光容器tf
    /// </summary>
    protected Transform _objTrans;
    /// <summary>
    /// 射线部分的sp
    /// </summary>
    protected SpriteRenderer _laser;
    /// <summary>
    /// 射线部分的tf
    /// </summary>
    protected Transform _laserTrans;
    /// <summary>
    /// 激光头部tf
    /// </summary>
    protected Transform _headTf;
    /// <summary>
    /// 激光头部sp
    /// </summary>
    protected SpriteRenderer _headSp;

    protected int _existTime;

    protected float _curHalfWidth;
    /// <summary>
    /// 是否需要Resize激光
    /// </summary>
    protected bool _isDirty;
    /// <summary>
    /// 激光头部是否显示
    /// </summary>
    protected bool _isHeadEnable;
    protected eLaserHeadType _headType;
    protected int _headAniTime;
    protected int _headAniIndex;

    /// <summary>
    /// 激光发射源是否显示
    /// </summary>
    protected bool _isSourceEnable;
    /// <summary>
    /// 发射源的图片索引
    /// </summary>
    protected int _laserSourceIndex;

    protected Transform _laserSourceTf;

    protected SpriteRenderer _laserSourceSp;

    private List<Vector2> _availableIndexRangeList;
    /// <summary>
    /// 近似碰撞点的list
    /// <para>与其他不规则物体碰撞时</para>
    /// <para>拆分成若干条直线，每条直线取中点做圆形检测</para>
    /// </summary>
    private List<Vector2> _collisionPointList;
    /// <summary>
    /// 近似碰撞点的个数
    /// </summary>
    private int _collisionPointCount;
    /// <summary>
    /// 表示是否已经缓存了近似碰撞点的数据
    /// <para>若激光属性发生变化，这个值会设置成false</para>
    /// <para>表示需要重新计算</para>
    /// </summary>
    private bool _isCachedCollisionPoint;

    private const int SegmentLen = 2;


    public EnemyLinearLaser()
    {
        _pathList = new List<Vector2>();
        _prefabName = "LinearLaser";
        _availableIndexRangeList = new List<Vector2>();
        _collisionPointList = new List<Vector2>();
        _sysBusyWeight = 3;
    }

    public override void Init()
    {
        base.Init();
        _isSized = false;
        _curPos = Vector3.zero;
        _existDuration = -1;
        _id = BulletId.Enemy_LinearLaser;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        _isDirty = false;
        _pathCount = 0;
        _pathBeginIndex = 0;
        _pathEndIndex = -1;
        _clearFlag = 0;
        _isHeadEnable = false;
        _isSourceEnable = false;
        _isCachedCollisionPoint = false;
        if ( _movableObj == null )
        {
            _movableObj = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        }
        if (_laserObj == null)
        {
            _laserObj = ResourceManager.GetInstance().GetPrefab("BulletPrefab", _prefabName);
            _objTrans = _laserObj.transform;
            _laser = _objTrans.Find("LaserSprite").GetComponent<SpriteRenderer>();
            _laserTrans = _laser.transform;
            _headTf = _objTrans.Find("HeadSprite");
            _headSp = _headTf.GetComponent<SpriteRenderer>();
            _laserSourceTf = _objTrans.Find("LaserSource");
            _laserSourceSp = _laserSourceTf.GetComponent<SpriteRenderer>();
            UIManager.GetInstance().AddGoToLayer(_laserObj, LayerId.EnemyBarrage);
        }
    }

    public override void SetBulletTexture(string texture)
    {
        _textureName = texture;
        _laser.sprite = ResourceManager.GetInstance().GetResource<Sprite>("etama9", texture);
        _laser.size = Vector2.zero;
    }

    public virtual void SetLength(int length)
    {
        _laserLen = length;
        _isDirty = true;
    }

    public int GetLength()
    {
        return _laserLen;
    }

    public virtual void DoStraightMove(float velocity,float angle,float acce,int accDuration)
    {
        _curVelocity = velocity;
        _curAngle = angle;
        _curAcceleration = acce;
        _accDuration = accDuration;
        _movableObj.DoMoveStraight(velocity, angle);
        _movableObj.DoAccelerationWithLimitation(acce, Consts.VelocityAngle, accDuration);
        _objTrans.localRotation = Quaternion.Euler(0, 0, angle);
        _isMoving = true;
    }

    public virtual void DoAccelerationWithLimitation(float acce,float angle,int accDuration)
    {
        _curAcceleration = acce;
        _movableObj.DoAccelerationWithLimitation(acce, angle, accDuration);
        _isMoving = true;
    }

    public override void Update()
    {
        UpdateComponents();
        _movableObj.Update();
        _curPos = _movableObj.GetPos();
        UpdatePath();
        UpdateGrazeCoolDown();
        if (_isDirty)
        {
            Resize();
        }
        UpdatePos();
        if ( _isHeadEnable )
        {
            UpdateHeadAni();
        }
        if ( _isSourceEnable )
        {
            UpdateLaserSource();
        }
        CheckCollisionWithCharacter();
        //UpdateExistTime();
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
    }

    protected virtual void UpdatePath()
    {
        _pathList.Add(_curPos);
        _pathCount++;
        if ( _pathCount > _laserLen )
        {
            _pathCount--;
            _pathBeginIndex++;
            _pathEndIndex++;
            // 加速度不为0，说明长度改变了，重绘激光图像
            if ( _curAcceleration != 0 )
            {
                _isDirty = true;
            }
            // 每隔60帧清除一次path
            if ( _pathBeginIndex >= 60 )
            {
                _pathBeginIndex -= 60;
                _pathEndIndex -= 60;
                _pathList.RemoveRange(0, 60);
            }
            SetSourceEnable(false);
        }
        else
        {
            _pathEndIndex++;
            _isDirty = true;
        }
    }

    public override void SetToPosition(float posX, float posY)
    {
        base.SetToPosition(posX, posY);
        _movableObj.Reset(posX, posY);
    }

    public override void SetToPosition(Vector2 pos)
    {
        base.SetToPosition(pos);
        _movableObj.Reset(pos.x,pos.y);
    }

    public virtual void SetHeadEnable(bool isEnable,eLaserHeadType headType)
    {
        _isHeadEnable = isEnable;
        if ( isEnable )
        {
            _headTf.gameObject.SetActive(true);
            // 初始化激光头部动画的相关参数
            _headAniTime = 0;
            _headAniIndex = 0;
            _headType = headType;
            _headSp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, GetHeadSpriteNameByType(_headType) + _headAniIndex);
        }
        else
        {
            _headTf.gameObject.SetActive(false);
        }
    }

    public void SetSourceEnable(bool isEnable,int sourceIndex=0)
    {
        if ( _isSourceEnable != isEnable )
        {
            _isSourceEnable = isEnable;
            if ( _isSourceEnable )
            {
                _laserSourceTf.gameObject.SetActive(true);
                _laserSourceSp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, SourceSpriteName + sourceIndex);
            }
            else
            {
                _laserSourceTf.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void UpdatePos()
    {
        _objTrans.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
    }

    /// <summary>
    /// 更新发射源
    /// </summary>
    private void UpdateLaserSource()
    {
        // 位置
        Vector3 offset = _pathList[_pathBeginIndex] - _pathList[_pathEndIndex];
        _laserSourceTf.localPosition = MathUtil.GetVec2AfterRotate(offset.x, offset.y, 0, 0, -_curAngle);
        // 自旋
        _laserSourceTf.Rotate(SourceSpinSpeed);
    }

    /// <summary>
    /// 重新渲染激光
    /// </summary>
    protected virtual void Resize()
    {
        float width = (_pathList[_pathEndIndex] - _pathList[_pathBeginIndex]).magnitude;
        _laser.size = new Vector2(width, DefaultLaserHalfHeight * 2);
        Vector3 pos = Vector3.zero;
        pos.x = -width / 2;
        _laserTrans.localPosition = pos;
        _isDirty = false;
    }

    /// <summary>
    /// 更新激光头部的动画
    /// </summary>
    protected virtual void UpdateHeadAni()
    {
        _headAniTime++;
        int index = (_headAniTime / HeadAniInterval) % HeadAniTotalFrame;
        if ( index != _headAniIndex )
        {
            _headAniIndex = index;
            _headSp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, GetHeadSpriteNameByType(_headType) + _headAniIndex);
        }
    }

    protected override bool IsOutOfBorder()
    {
        if (_pathList[_pathBeginIndex].x < Global.BulletLBBorderPos.x ||
            _pathList[_pathBeginIndex].y < Global.BulletLBBorderPos.y ||
            _pathList[_pathBeginIndex].x > Global.BulletRTBorderPos.x ||
            _pathList[_pathBeginIndex].y > Global.BulletRTBorderPos.y)
        {
            //Logger.Log("Laser is out of border");
            return true;
        }
        return false;
    }

    #region 碰撞检测相关参数
    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
        base.SetGrazeDetectParas(paras);
    }
    public override void SetCollisionDetectParas(CollisionDetectParas paras)
    {
        _collisionHalfWidth = paras.halfWidth;
        _collisionHalfHeight = paras.halfHeight;
        base.SetCollisionDetectParas(paras);
    }

    public override CollisionDetectParas GetCollisionDetectParas()
    {
        CollisionDetectParas paras = new CollisionDetectParas()
        {
            type = CollisionDetectType.Line,
            linePointA = _pathList[_pathBeginIndex],
            linePointB = _pathList[_pathEndIndex],
            radius = DefaultCollisionHalfHeight,
        };
        return paras;
    }

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        Vector2 laserStartPos = _pathList[0];
        Vector2 laserEndPos = _pathList[_laserLen - 1];
        // 计算包围矩形的左下和右上的坐标
        Vector2 tmpLBPos = new Vector2(Mathf.Min(laserStartPos.x, laserEndPos.x), Mathf.Min(laserStartPos.y, laserEndPos.y));
        Vector2 tmpRTPos = new Vector2(Mathf.Max(laserStartPos.x, laserEndPos.x), Mathf.Max(laserStartPos.y, laserEndPos.y));
        Vector2 bulletBoundingBoxLBPos = new Vector2(_curPos.x - DefaultCollisionHalfHeight, _curPos.y - DefaultCollisionHalfHeight);
        Vector2 bulletBoundingBoxRTPos = new Vector2(_curPos.x + DefaultCollisionHalfHeight, _curPos.y + DefaultCollisionHalfHeight);
        // 由于通常情况下大部分子弹都是不在包围盒范围内的
        // 因此选择判断不相交的方式尽可能减少判断的次数
        bool notHit = bulletBoundingBoxRTPos.x < lbPos.x ||    // 子弹包围盒右边缘X坐标小于检测包围盒的左边缘X坐标
            bulletBoundingBoxLBPos.x > rtPos.x ||   // 子弹包围盒左边缘X坐标大于检测包围盒的左右边缘X坐标
            bulletBoundingBoxRTPos.y < lbPos.y ||   // 子弹包围盒上边缘Y坐标小于检测包围盒的下边缘Y坐标
            bulletBoundingBoxLBPos.y > rtPos.y;     // 子弹包围盒下边缘Y坐标大于检测包围盒的上边缘Y坐标
        return !notHit;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        if ( !_isCachedCollisionPoint )
        {
            CacheCollisionPoints();
        }
        CollisionDetectParas paras = new CollisionDetectParas()
        {
            type = CollisionDetectType.Circle,
            centerPos = _collisionPointList[index],
            nextIndex = index + 1 >= _collisionPointCount ? -1 : index + 1,
            radius = DefaultCollisionHalfHeight,
        };
        return paras;
    }

    private void CacheCollisionPoints()
    {
        int laserSegmentCount = _availableIndexRangeList.Count;
        int startIndex,endIndex;
        Vector2 rangeVec,collisionPoint;
        _collisionPointCount = 0;
        for (int i=0;i<laserSegmentCount;i++)
        {
            rangeVec = _availableIndexRangeList[i];
            for (int j=(int)rangeVec.x;j<=rangeVec.y;j+=SegmentLen)
            {
                startIndex = j;
                endIndex = j + SegmentLen > rangeVec.y ? (int)rangeVec.y : j + SegmentLen;
                // 计算近似碰撞点的坐标
                collisionPoint = (_pathList[startIndex] + _pathList[endIndex]) / 2;
                _collisionPointList.Add(collisionPoint);
                _collisionPointCount++;
            }
        }
        _isCachedCollisionPoint = true;
    }

    protected virtual void CheckCollisionWithCharacter()
    {
        // 直线碰撞检测
        float minDis = MathUtil.GetMinDisFromPointToLineSegment(_pathList[_pathBeginIndex], _pathList[_pathEndIndex], Global.PlayerPos);
        // 擦弹判断
        if ( minDis <= DefaultLaserHalfHeight + Global.PlayerGrazeRadius )
        {
            if ( !_isGrazed )
            {
                _isGrazed = true;
                _grazeCoolDown = GrazeCoolDown;
                PlayerService.GetInstance().AddGraze(1);
            }
            if (minDis <= DefaultLaserHalfHeight + Global.PlayerCollisionVec.z)
            {
                PlayerService.GetInstance().GetCharacter().BeingHit();
                Eliminate(eEliminateDef.HitPlayer);
            }
        }
    }
    #endregion

    public string GetTextureName()
    {
        return _textureName;
    }

    public float GetVelocity()
    {
        return _curVelocity;
    }

    public float GetAngle()
    {
        return _curAngle;
    }

    public float GetAcceleration()
    {
        return _curAcceleration;
    }

    public int GetAccDuration()
    {
        return _accDuration;
    }

    public override void Clear()
    {
        UIManager.GetInstance().HideGo(_laserObj);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _laserObj);
        _laserObj = null;
        _objTrans = null;
        // 清除射线部分
        _laser.sprite = null;
        _laser = null;
        _laserTrans = null;
        // 激光头部
        _headSp.sprite = null;
        _headSp = null;
        _headTf = null;
        // 发射源
        _laserSourceSp.sprite = null;
        _laserSourceSp = null;
        _laserSourceTf = null;
        _pathList.Clear();
        // movableObject
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObj);
        _movableObj = null;
        _collisionPointList.Clear();
        _availableIndexRangeList.Clear();
        base.Clear();
    }
}


