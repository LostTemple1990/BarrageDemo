using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLinearLaser : EnemyBulletBase
{
    private const float DefaultLaserHalfHeight = 8f;
    private const float DefaultCollisionHalfHeight = 4f;

    private const string HeadSpriteNameWhite = "LaserHead_W";
    private const string HeadSpriteNameBlue = "LaserHead_B";
    private const string HeadSpriteNameRed = "LaserHead_R";
    private const string HeadSpriteNameGreen = "LaserHead_G";

    /// <summary>
    /// 发射源的自旋速度
    /// </summary>
    private static Vector3 SourceSpinSpeed = new Vector3(0, 0, 18);

    private const int HeadAniInterval = 6;
    private const int HeadAniTotalFrame = 4;
    /// <summary>
    /// 擦弹冷却时间
    /// </summary>
    private const int GrazeCoolDown = 3;

    class LaserSegment
    {
        private GameObject _segmentObj;
        private Transform _segmentTf;
        private Transform _laserTf;
        private SpriteRenderer _laserSpriteRenderer;
        private List<Vector2> _pathList;
        private bool _isHeadEnable;
        private GameObject _headGo;
        private Transform _headTf;
        /// <summary>
        /// 激光头部的SpriteRenderer
        /// </summary>
        private SpriteRenderer _headSr;
        /// <summary>
        /// 激光的角度
        /// </summary>
        private float _laserAngle;
        /// <summary>
        /// 是否已经设置过旋转
        /// </summary>
        private bool _isSetRotation;
        /// <summary>
        /// 分段索引标识
        /// </summary>
        private Vector2 _segmentVec;
        /// <summary>
        /// 是否持有原型
        /// </summary>
        private bool _isProtoType;
        /// <summary>
        /// 激光段的像素长度
        /// </summary>
        private float _width;
        /// <summary>
        /// 激光段创建出来的时间
        /// </summary>
        private int _timeSinceSegmentCreated;
        /// <summary>
        /// 颜色是否已经改变
        /// </summary>
        private bool _isColorChanged;
        /// <summary>
        /// 透明度
        /// </summary>
        private float _segmentAlpha;
        /// <summary>
        /// 颜色
        /// </summary>
        private Color _segmentColor;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="protoType"></param>
        /// <param name="isProtoType"></param>
        /// <returns></returns>
        public LaserSegment Init(GameObject protoType,bool isProtoType)
        {
            _isProtoType = isProtoType;
            if ( isProtoType )
            {
                _segmentObj = protoType;
            }
            else
            {
                _segmentObj = GameObject.Instantiate<GameObject>(protoType);
            }
            _segmentTf = _segmentObj.transform;
            _segmentTf.SetParent(protoType.transform.parent, false);
            _laserTf = _segmentTf.Find("LaserSprite");
            _laserSpriteRenderer = _laserTf.GetComponent<SpriteRenderer>();
            // 激光头部相关
            _isHeadEnable = false;
            _headTf = _segmentTf.Find("HeadSprite");
            _headGo = _headTf.gameObject;
            _headSr = _headTf.GetComponent<SpriteRenderer>();
            _timeSinceSegmentCreated = 0;
            _isColorChanged = false;
            _segmentColor = new Color(1, 1, 1);
            _segmentAlpha = 1f;
            return this;
        }

        public LaserSegment SetAngle(float angle)
        {
            _laserAngle = angle;
            _isSetRotation = false;
            return this;
        }

        public LaserSegment SetPath(List<Vector2> path)
        {
            _pathList = path;
            return this;
        }

        public LaserSegment SetSegmentVec(Vector2 vec)
        {
            _segmentVec = vec;
            return this;
        }

        public LaserSegment SetSegmentVec(int startIndex,int endIndex)
        {
            _segmentVec = new Vector2(startIndex,endIndex);
            return this;
        }

        public void Render(bool resize=false)
        {
            _timeSinceSegmentCreated++;
            UpdatePos();
            if (resize) Resize();
            if ( _isColorChanged )
            {
                _laserSpriteRenderer.color = new Color(_segmentColor.r, _segmentColor.g, _segmentColor.b, _segmentAlpha);
                _isColorChanged = false;
            }
            if ( _isHeadEnable )
            {
                int tmp = _timeSinceSegmentCreated % 7;
                if (tmp == 0) _headSr.color = new Color(1, 1, 1, 1);
                if (tmp == 4) _headSr.color = new Color(1, 1, 1, 0.8f);
            }
        }

        public void Resize()
        {
            // 分段长度小于1，不执行resize
            if (_segmentVec.y - _segmentVec.x <= 0) return;
            _width = (_pathList[(int)_segmentVec.y] - _pathList[(int)_segmentVec.x]).magnitude;
            _laserSpriteRenderer.size = new Vector2(_width, DefaultLaserHalfHeight * 2);
            if (_isHeadEnable)
            {
                _headTf.localPosition = new Vector3(_width / 2, 0, 0);
            }
        }

        private void UpdatePos()
        {
            _segmentTf.localPosition = (_pathList[(int)_segmentVec.x] + _pathList[(int)_segmentVec.y]) * 0.5f;
            if ( !_isSetRotation )
            {
                _segmentTf.localRotation = Quaternion.Euler(0, 0, _laserAngle);
                _isSetRotation = true;
            }
        }

        public void Resize(Vector2 segmentVec)
        {
            SetSegmentVec(segmentVec);
            Resize();
        }

        public Vector2 GetSegmentVec()
        {
            return _segmentVec;
        }

        public void SetHeadEnable(bool isEnable)
        {
            if ( isEnable != _isHeadEnable )
            {
                _headGo.SetActive(isEnable);
                _isHeadEnable = isEnable;
            }
        }

        /// <summary>
        /// 设置该激光段是否可见
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public LaserSegment SetEnable(bool isEnable)
        {
            _segmentObj.SetActive(isEnable);
            return this;
        }

        /// <summary>
        /// 设置透明度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public LaserSegment SetAlpha(float value)
        {
            _segmentAlpha = value;
            _isColorChanged = true;
            return this;
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public LaserSegment SetColor(Color value)
        {
            _segmentColor = value;
            _isColorChanged = true;
            return this;
        }

        public void Clear()
        {
            // 销毁非原型的对象
            if ( !_isProtoType )
            {
                GameObject.Destroy(_segmentObj);
            }
            else
            {
                if (!_isColorChanged) _laserSpriteRenderer.color = new Color(1, 1, 1, 1);
                _laserSpriteRenderer.size = Vector2.zero;
            }
            _segmentObj = null;
            _segmentTf = null;
            _laserTf = null;
            _laserSpriteRenderer = null;
            _pathList = null;
            _headGo = null;
            _headTf = null;
        }
    }

    protected MovableObject _movableObj;
    /// <summary>
    /// 直线激光的路径点集合
    /// </summary>
    protected List<Vector2> _pathList;
    /// <summary>
    /// 直线激光路径的点的数目
    /// <para>同时，_pathCount-1也表示激光头部在pathList中的索引数值</para>
    /// </summary>
    protected int _pathCount;
    /// <summary>
    /// 直线激光的长度
    /// <para>该长度表示直线激光的路径要生成至少_laserLen+1个点才完全生成完成</para>
    /// </summary>
    protected int _laserLen;
    protected int _existDuration;

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
    /// 是否已经初始化过角度
    /// <para>直线激光初始化过角度之后不能再被赋值</para>
    /// </summary>
    private bool _isInitAngle;

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

    private GameObject _laserSourceGo;

    protected Transform _laserSourceTf;

    protected SpriteRenderer _laserSourceSp;
    /// <summary>
    /// 是否已经初始化过发射源的位置
    /// </summary>
    private bool _isInitSourcePos;

    /// <summary>
    /// 直线激光的分段数据
    /// x,y分别代表path[startIndex]和path[endIndex]
    /// </summary>
    private List<LaserSegment> _laserSegmentList;
    /// <summary>
    /// 直线激光的分段计数
    /// </summary>
    private int _laserSegmentCount;

    private Stack<LaserSegment> _laserSegmentPool;
    /// <summary>
    /// 近似碰撞点的list
    /// <para>与其他不规则物体碰撞时</para>
    /// <para>拆分成若干条线段，每条线段取中点做圆形检测</para>
    /// <para>这个list记录了每段线段在path中的起始索引和结束索引</para>
    /// </summary>
    private List<Vector2> _collisionSegmentList;
    /// <summary>
    /// 碰撞线段的个数
    /// </summary>
    private int _collisionSegmentCount;
    /// <summary>
    /// 表示是否已经缓存了分段碰撞的数据
    /// <para>若激光属性发生变化，这个值会设置成false</para>
    /// <para>表示需要重新计算</para>
    /// </summary>
    private bool _isCachedCollisionSegment;

    private const int CollisionSegmentLen = 2;
    /// <summary>
    /// 发生碰撞的分段索引list
    /// </summary>
    private List<int> _collidedSegmentIndexList;
    /// <summary>
    /// 发生碰撞的计数
    /// </summary>
    private int _collidedSegmentCount;
    /// <summary>
    /// 激光的配置
    /// </summary>
    private EnemyLinearLaserCfg _cfg;
    /// <summary>
    /// 用于复制的激光段的原型
    /// </summary>
    private GameObject _laserSegmentProtoType;


    public EnemyLinearLaser()
    {
        _type = BulletType.Enemy_LinearLaser;
        _pathList = new List<Vector2>();
        _prefabName = "LinearLaser";
        _laserSegmentList = new List<LaserSegment>();
        _laserSegmentPool = new Stack<LaserSegment>();
        _collisionSegmentList = new List<Vector2>();
        _collidedSegmentIndexList = new List<int>();
        _sysBusyWeight = 3;
    }

    public override void Init()
    {
        base.Init();
        _isSized = false;
        _curPos = Vector3.zero;
        _existDuration = -1;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        _isDirty = false;
        _pathCount = 0;
        _clearFlag = 0;
        _isHeadEnable = false;
        _isSourceEnable = false;
        _isInitSourcePos = true;
        _laserSegmentCount = 0;
        _isCachedCollisionSegment = false;
        _collidedSegmentCount = 0;
        _isInitAngle = false;
        _laserLen = 0;
        if ( _movableObj == null )
        {
            _movableObj = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        }
    }

    /// <summary>
    /// 设置直线激光的角度
    /// </summary>
    /// <param name="angle"></param>
    public void SetAngle(float angle)
    {
        if (_isInitAngle) return;
        _isInitAngle = true;
        _curVAngle = angle;
        for (int i=0;i<_laserSegmentCount;i++)
        {
            _laserSegmentList[i].SetAngle(angle);
        }
    }

    public override void SetStyleById(string id)
    {
        _cfg = BulletsManager.GetInstance().GetLinearLaserCfgById(id);
        if ( _cfg == null )
        {
            Logger.LogError("LinearLaserCfg with id " + id + " is not exist!");
            return;
        }
        _prefabName = _cfg.id.ToString();
        _laserObj = BulletsManager.GetInstance().CreateBulletGameObject(BulletType.Enemy_LinearLaser, _cfg.id);
        _objTrans = _laserObj.transform;
        _objTrans.localPosition = new Vector3(0, 0, -_orderInLayer);
        // 发射源
        _laserSourceTf = _objTrans.Find("Source");
        _laserSourceGo = _laserSourceTf.gameObject;
        // segment原型
        _laserSegmentProtoType = _objTrans.Find("Segment").gameObject;
        // 创建第一段segment
        LaserSegment segment = new LaserSegment();
        segment.Init(_laserSegmentProtoType, true).SetPath(_pathList).SetSegmentVec(0, 0);
        _laserSegmentList.Add(segment);
        _laserSegmentCount = 1;
        _isDirty = true;
    }

    public void SetLength(int length)
    {
        if (_laserLen > 0) return;
        _laserLen = length;
        _isDirty = true;
    }

    public int GetLength()
    {
        return _laserLen;
    }

    public void DoStraightMove(float velocity,float angle,float acce,float maxVelocity)
    {
        _curVelocity = velocity;
        SetAngle(angle);
        _curAcce = acce;
        _movableObj.DoStraightMove(velocity, angle);
        _movableObj.DoAccelerationWithLimitation(acce, Consts.VelocityAngle, maxVelocity);
        _isMoving = true;
    }


    public override void Update()
    {
        base.Update();
        UpdateComponents();
        CheckDivideIntoMutiple();
        UpdatePath();
        if ( _laserSegmentCount > 0 )
        {
            UpdateGrazeCoolDown();
            CheckCollisionWithCharacter();
            Render();
            //UpdateExistTime();
            if (IsOutOfBorder())
            {
                Eliminate(eEliminateDef.ForcedDelete);
            }
        }
        else
        {
            Eliminate(eEliminateDef.ForcedDelete);
        }
    }

    protected virtual void UpdatePath()
    {
        _movableObj.Update();
        _curPos = _movableObj.GetPos();
        // 添加新的路径点
        _pathList.Add(_curPos);
        if ( _pathCount > _laserLen )
        {
            // 加速度不为0，说明长度改变了，重绘激光图像
            if ( _curAcce != 0 )
            {
                _isDirty = true;
            }
            SetSourceEnable(false);
            _pathList.RemoveAt(0);
        }
        else
        {
            _pathCount++;
            if ( _laserSegmentCount == 0 )
            {
                LaserSegment segment = CreateLaserSegment(0, 1);
                _laserSegmentList.Add(segment);
                _laserSegmentCount++;
            }
            else
            {
                Vector2 firstSegmentVec = _laserSegmentList[0].GetSegmentVec();
                if ( firstSegmentVec.x == 0 )
                {
                    firstSegmentVec.y += 1;
                    _laserSegmentList[0].SetSegmentVec(firstSegmentVec);
                }
                else
                {
                    LaserSegment segment = CreateLaserSegment(0, 1);
                    _laserSegmentList.Insert(0, segment);
                    _laserSegmentCount++;
                }
            }
            // 线段索引起始点、终点全部+1
            Vector2 segmentVec;
            for (int i = 1; i < _laserSegmentCount; i++)
            {
                segmentVec = _laserSegmentList[i].GetSegmentVec();
                _laserSegmentList[i].SetSegmentVec(new Vector2(segmentVec.x + 1, segmentVec.y + 1));
            }
            _isCachedCollisionSegment = false;
            _isDirty = true;
        }
    }

    /// <summary>
    /// 创建LaserSegment对象
    /// <para>优先从缓存池中取出</para>
    /// </summary>
    /// <returns></returns>
    private LaserSegment CreateLaserSegment(int startIndex,int endIndex)
    {
        LaserSegment segment;
        if ( _laserSegmentPool.Count > 0 )
        {
            segment = _laserSegmentPool.Pop();
            segment.SetEnable(true);
            segment.SetSegmentVec(startIndex, endIndex);
        }
        else
        {
            segment = new LaserSegment();
            segment.Init(_laserSegmentProtoType,false).SetPath(_pathList).SetSegmentVec(startIndex, endIndex).SetAngle(_curVAngle);
        }
        if (!_isOriginalColor) segment.SetAlpha(_curAlpha).SetColor(_curColor);
        return segment;
    }

    /// <summary>
    /// 缓存LaserSegment对象
    /// </summary>
    /// <param name="segment"></param>
    private void RestoreLaserSegment(LaserSegment segment)
    {
        segment.SetEnable(false);
        _laserSegmentPool.Push(segment);
    }

    public override void SetToPosition(float posX, float posY)
    {
        //Logger.Log("Set LinearLaser To Position " + posX + "  , " + posY);
        _movableObj.Reset(posX, posY);
        _pathList.Add(new Vector2(posX, posY));
    }

    public override void SetToPosition(Vector2 pos)
    {
        //Logger.Log("Set LinearLaser To Position " + pos.x + "  , " + pos.y);
        _movableObj.Reset(pos.x,pos.y);
        _pathList.Add(pos);
    }

    public override void SetOrderInLayer(int orderInLayer)
    {
        base.SetOrderInLayer(orderInLayer);
        if ( _objTrans != null )
        {
            _objTrans.localPosition = new Vector3(0, 0, -_orderInLayer);
        }
    }

    /// <summary>
    /// 设置是否显示激光的头部可见
    /// </summary>
    /// <param name="isEnable"></param>
    /// <param name="headType"></param>
    public virtual void SetHeadEnable(bool isEnable)
    {
        if ( _isHeadEnable != isEnable )
        {
            _isHeadEnable = isEnable;
            for (int i = 0; i < _laserSegmentCount; i++)
            {
                _laserSegmentList[i].SetHeadEnable(_isHeadEnable);
            }
        }
    }

    public void SetSourceEnable(bool isEnable,int sourceIndex=0)
    {
        if ( _isSourceEnable != isEnable )
        {
            _isSourceEnable = isEnable;
            _laserSourceGo.SetActive(_isSourceEnable);
            if ( _isSourceEnable )
            {
                _isInitSourcePos = false;
            }
        }
    }

    private void Render()
    {
        if (_isSourceEnable)
        {
            UpdateLaserSource();
        }
        LaserSegment segment;
        for (int i = 0; i < _laserSegmentCount; i++)
        {
            segment = _laserSegmentList[i];
            if ( _isColorChanged )
            {
                segment.SetColor(_curColor).SetAlpha(_curAlpha);
            }
            segment.Render(_isDirty);
        }
        _isColorChanged = false;
        _isDirty = false;
    }

    /// <summary>
    /// 更新发射源
    /// </summary>
    private void UpdateLaserSource()
    {
        if ( !_isInitSourcePos )
        {
            _laserSourceTf.localPosition = _pathList[0];
            _isInitSourcePos = true;
        }
        // 自旋
        _laserSourceTf.Rotate(SourceSpinSpeed);
    }

    protected override bool IsOutOfBorder()
    {
        if (_pathList[0].x < Global.BulletLBBorderPos.x ||
            _pathList[0].y < Global.BulletLBBorderPos.y ||
            _pathList[0].x > Global.BulletRTBorderPos.x ||
            _pathList[0].y > Global.BulletRTBorderPos.y)
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

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        if (_pathCount <= 1) return false;
        Vector2 laserStartPos = _pathList[0];
        Vector2 laserEndPos = _pathList[_pathCount - 1];
        // 计算包围矩形的左下和右上的坐标
        Vector2 tmpLBPos = new Vector2(Mathf.Min(laserStartPos.x, laserEndPos.x), Mathf.Min(laserStartPos.y, laserEndPos.y));
        Vector2 tmpRTPos = new Vector2(Mathf.Max(laserStartPos.x, laserEndPos.x), Mathf.Max(laserStartPos.y, laserEndPos.y));
        Vector2 bulletBoundingBoxLBPos = new Vector2(tmpLBPos.x - DefaultCollisionHalfHeight, tmpLBPos.y - DefaultCollisionHalfHeight);
        Vector2 bulletBoundingBoxRTPos = new Vector2(tmpRTPos.x + DefaultCollisionHalfHeight, tmpRTPos.y + DefaultCollisionHalfHeight);
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
        if ( !_isCachedCollisionSegment )
        {
            CacheCollisionPoints();
        }
        CollisionDetectParas paras;
        if ( _collisionSegmentCount == 0 )
        {
            paras = new CollisionDetectParas()
            {
                type = CollisionDetectType.Null,
                nextIndex = -1,
            };
        }
        else
        {
            Vector2 segment = _collisionSegmentList[index];
            paras = new CollisionDetectParas()
            {
                type = CollisionDetectType.Circle,
                centerPos = (_pathList[(int)segment.x] + _pathList[(int)segment.y]) / 2,
                nextIndex = index + 1 >= _collisionSegmentCount ? -1 : index + 1,
                radius = DefaultCollisionHalfHeight,
            };
        }
        return paras;
    }

    private void CacheCollisionPoints()
    {
        // 先清除原来缓存的碰撞分段数据
        _collisionSegmentList.Clear();
        _collisionSegmentCount = 0;
        // 重新计算
        int startIndex,endIndex;
        Vector2 rangeVec,collisionSegment;
        for (int i=0;i<_laserSegmentCount;i++)
        {
            rangeVec = _laserSegmentList[i].GetSegmentVec();
            for (int j=(int)rangeVec.x;j<=rangeVec.y;j+=CollisionSegmentLen)
            {
                startIndex = j;
                endIndex = j + CollisionSegmentLen > rangeVec.y ? (int)rangeVec.y : j + CollisionSegmentLen;
                // 只剩余一个点，不做划分
                if (startIndex == endIndex) break;
                // 计算近似碰撞点的坐标
                collisionSegment = new Vector2(startIndex, endIndex);
                _collisionSegmentList.Add(collisionSegment);
                _collisionSegmentCount++;
            }
        }
        _isCachedCollisionSegment = true;
    }

    public override void CollidedByObject(int n = 0, eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {
        if ( _collidedSegmentIndexList.IndexOf(n) == -1 )
        {
            _collidedSegmentIndexList.Add(n);
            _collidedSegmentCount++;
        }
    }

    /// <summary>
    /// 检测是否需要将激光分成多段
    /// </summary>
    private void CheckDivideIntoMutiple()
    {
        if (_collidedSegmentCount == 0) return;
        //Logger.Log("-------LogSegmentBegin-------------------");
        //Logger.Log("-------SegmentsBeforeDivided-------------------");
        //for (int i = 0; i < _laserSegmentCount; i++)
        //{
        //    Logger.Log(_laserSegmentList[i].GetSegmentVec());
        //}
        //Logger.Log("---------CollidedSegments----------------------");
        Vector2 laserSegmentVec,collidedSegmentVec;
        Vector2 divideSegment0, divideSegment1;
        // segment是碰撞分段的起始、结束下标索引
        int laserSegmentLen;
        for (int i = 0; i < _collidedSegmentCount; i++)
        {
            collidedSegmentVec = _collisionSegmentList[_collidedSegmentIndexList[i]];
            //Logger.Log(collidedSegmentVec);
            for (int j = 0; j < _laserSegmentCount; j++)
            {
                laserSegmentVec = _laserSegmentList[j].GetSegmentVec();
                laserSegmentLen = (int)(laserSegmentVec.y - laserSegmentVec.x);
                if (laserSegmentLen > 0 && (collidedSegmentVec.x >= laserSegmentVec.y || collidedSegmentVec.y <= laserSegmentVec.x))
                {
                    continue;
                }
                //分四种情况，上为collidedSegmentVec，下为laserSegmentVec
                // 最优先判断会被全部截取掉的情况
                //    ----------
                //      ------
                if (collidedSegmentVec.x <= laserSegmentVec.x && collidedSegmentVec.y >= laserSegmentVec.y)
                {
                    LaserSegment segment = _laserSegmentList[j];
                    _laserSegmentList.RemoveAt(j);
                    _laserSegmentCount--;
                    j--;
                    RestoreLaserSegment(segment);
                }
                //    ---------
                //  -----
                else if (collidedSegmentVec.x >= laserSegmentVec.x && collidedSegmentVec.y >= laserSegmentVec.y)
                {
                    divideSegment0 = new Vector2(laserSegmentVec.x, collidedSegmentVec.x);
                    _laserSegmentList[j].SetSegmentVec(divideSegment0);
                }
                //    ---------
                //      -----------
                else if (collidedSegmentVec.x <= laserSegmentVec.x && collidedSegmentVec.y <= laserSegmentVec.y)
                {
                    divideSegment0 = new Vector2(collidedSegmentVec.y, laserSegmentVec.y);
                    _laserSegmentList[j].SetSegmentVec(divideSegment0);
                }
                //    ---------
                //  -------------
                else if (collidedSegmentVec.x >= laserSegmentVec.x && collidedSegmentVec.y <= laserSegmentVec.y)
                {
                    divideSegment0 = new Vector2(laserSegmentVec.x, collidedSegmentVec.x);
                    divideSegment1 = new Vector2(collidedSegmentVec.y, laserSegmentVec.y);
                    _laserSegmentList[j].SetSegmentVec(divideSegment0);
                    LaserSegment segment = CreateLaserSegment((int)divideSegment1.x, (int)divideSegment1.y);
                    _laserSegmentList.Insert(j + 1, segment);
                    _laserSegmentCount++;
                    j++;
                }
            }
        }
        _collidedSegmentIndexList.Clear();
        _collidedSegmentCount = 0;
        _isCachedCollisionSegment = false;
        //Logger.Log("-------SegmentsAfterDivided-------------------");
        //for (int i = 0; i < _laserSegmentCount; i++)
        //{
        //    Logger.Log(_laserSegmentList[i].GetSegmentVec());
        //}
        //Logger.Log("----------LogSegmentEnd----------------");
        _isDirty = true;
    }

    protected virtual void CheckCollisionWithCharacter()
    {
        for (int i=0;i<_laserSegmentCount;i++)
        {
            Vector2 segmentVec = _laserSegmentList[i].GetSegmentVec();
            // 分段长度小于等于0，不进行判定
            if ( segmentVec.y - segmentVec.x <= 0 )
            {
                continue;
            }
            // 直线碰撞检测
            float minDis = MathUtil.GetMinDisFromPointToLineSegment(_pathList[(int)segmentVec.x], _pathList[(int)segmentVec.y], Global.PlayerPos);
            // 擦弹判断
            if (minDis <= DefaultLaserHalfHeight + Global.PlayerGrazeRadius)
            {
                if (!_isGrazed)
                {
                    _isGrazed = true;
                    _grazeCoolDown = GrazeCoolDown;
                    PlayerService.GetInstance().AddGraze(1);
                }
                if (minDis <= DefaultLaserHalfHeight + Global.PlayerCollisionVec.z)
                {
                    PlayerService.GetInstance().GetCharacter().BeingHit();
                    // 直线激光击中玩家不消除
                    //Eliminate(eEliminateDef.HitPlayer);
                }
            }
        }
    }
    #endregion

    public override string BulletId
    {
        get
        {
            return _cfg.id;
        }
    }

    public override bool GetBulletPara(BulletParaType paraType, out float value)
    {
        switch ( paraType )
        {
            case BulletParaType.Velocity:
                value = _curVelocity;
                return true;
            case BulletParaType.VAngel:
                value = _curVAngle;
                return true;
            case BulletParaType.Acce:
                value = _curAcce;
                return true;
            case BulletParaType.AccAngle:
                value = _curAccAngle;
                return true;
            case BulletParaType.Alpha:
                value = _curAlpha;
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
                _movableObj.Velocity = value;
                return true;
            case BulletParaType.VAngel:
                if ( !_isInitAngle)
                {
                    _movableObj.VAngle = value;
                    _isInitAngle = true;
                }
                return true;
            case BulletParaType.Acce:
                _movableObj.Acce = value;
                return true;
            case BulletParaType.AccAngle:
                if ( !_isInitAngle )
                {
                    _movableObj.AccAngle = value;
                    _isInitAngle = true;
                }
                return true;
            case BulletParaType.Alpha:
                SetAlpha(value);
                return true;
        }
        value = 0;
        return false;
    }


    public override void Clear()
    {
        // 清除多余的LaserSegment
        int i;
        LaserSegment segment;
        for (i=0;i<_laserSegmentCount;i++)
        {
            _laserSegmentList[i].Clear();
        }
        _laserSegmentList.Clear();
        _laserSegmentCount = 0;
        while ( _laserSegmentPool.Count > 0 )
        {
            segment = _laserSegmentPool.Pop();
            segment.Clear();
        }
        _laserSegmentProtoType = null;
        // 隐藏source
        if ( _laserSourceGo != null )
        {
            _laserSourceGo.SetActive(false);
        }
        _laserSourceGo = null;
        _laserSourceTf = null;
        // LaserObject
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _laserObj);
        _laserObj = null;
        _objTrans = null;
        _pathList.Clear();
        // movableObject
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObj);
        _movableObj = null;
        _collisionSegmentList.Clear();
        _laserSegmentList.Clear();
        base.Clear();
    }
}


