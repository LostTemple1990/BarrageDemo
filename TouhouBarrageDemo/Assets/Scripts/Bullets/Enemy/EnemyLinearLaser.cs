﻿using System.Collections;
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
    /// <summary>
    /// 是否需要渲染本体的位置
    /// </summary>
    protected bool _renderObjPos;

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
    /// <summary>
    /// 自上一次改变速度的时间
    /// </summary>
    private int _timeSinceLastVChanged;
    /// <summary>
    /// 上一帧的速度
    /// </summary>
    private float _preV;
    /// <summary>
    /// 这一帧的速度
    /// </summary>
    private float _curV;
    /// <summary>
    /// 激光发射源是否被消除
    /// </summary>
    private bool _isSourceEliminated;


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
        _curPos = Vector2.zero;
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
        _renderObjPos = true;
        _timeSinceLastVChanged = 0;
        _preV = 0;
        _isSourceEliminated = false;
        if ( _movableObj == null )
        {
            _movableObj = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        }
    }

    public override void SetRotation(float value)
    {
        if (_isInitAngle) return;
        _isInitAngle = true;
        _curVAngle = value;
        for (int i = 0; i < _laserSegmentCount; i++)
        {
            _laserSegmentList[i].SetAngle(value);
        }
        base.SetRotation(value);
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
        // 发射源
        _laserSourceTf = _objTrans.Find("Source");
        _laserSourceGo = _laserSourceTf.gameObject;
        // segment原型
        _laserSegmentProtoType = _objTrans.Find("Segment").gameObject;
    }

    public void SetLength(int length)
    {
        if (_laserLen > 0) return;
        _laserLen = length;
        //if (!_isInitPos)
        //    Logger.LogWarn("It's nessesary to set init pos of linearlaser before setting it's length!");
        //_pathList.Add(_curPos);
        // 创建第一段segment
        LaserSegment segment = new LaserSegment();
        segment.Init(_laserSegmentProtoType, true).SetPath(_pathList).SetSegmentVec(0, 0);
        _laserSegmentList.Add(segment);
        _laserSegmentCount = 1;
        _isDirty = true;
    }

    public int GetLength()
    {
        return _laserLen;
    }

    /// <summary>
    /// 设置额外的直线运动的参数
    /// <para>一般是被引力场影响</para>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public override void AddExtraSpeedParas(float v, float angle, float acce, float accAngle)
    {
        if (!_isInitAngle) return;
        angle = _curRotation;
        // 先计算投影长度
        Vector2 velocityVec = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * v;
        // 在激光角度方向上的速度
        v = velocityVec.x * Mathf.Cos(Mathf.Deg2Rad * _curVAngle) + velocityVec.y * Mathf.Sin(Mathf.Deg2Rad * _curVAngle);
        if (v < 0) v = 0;
        // 先计算投影长度
        Vector2 acceVec = new Vector2(Mathf.Cos(accAngle * Mathf.Deg2Rad), Mathf.Sin(accAngle * Mathf.Deg2Rad)) * acce;
        // 在激光角度方向上的加速度
        acce = acceVec.x * Mathf.Cos(Mathf.Deg2Rad * _curVAngle) + acceVec.y * Mathf.Sin(Mathf.Deg2Rad * _curVAngle);
        if (acce < 0) acce = 0;
        _movableObj.AddExtraSpeedParas(v, _curVAngle, acce, _curVAngle);
    }


    public override void Update()
    {
        base.Update();
        _timeSinceLastVChanged++;
        UpdateComponents();
        CheckDivideIntoMutiple();
        UpdatePath();
        if ( _laserSegmentCount > 0 )
        {
            UpdateGrazeCoolDown();
            CheckCollisionWithCharacter();
            //UpdateExistTime();
            if (IsOutOfBorder())
            {
                Eliminate(eEliminateDef.ForcedDelete);
            }
        }
        else
        {
            if (_laserSegmentCount <= 0 )
            {
                if (_timeSinceCreated >= _laserLen)
                {
                    Eliminate(eEliminateDef.ForcedDelete);
                }
                else if (_isSourceEliminated)
                {
                    Eliminate(eEliminateDef.ForcedDelete);
                }
            }
        }
    }

    protected virtual void UpdatePath()
    {
        _movableObj.Update();
        _curPos = _movableObj.GetPos();
        // 添加新的路径点
        _pathList.Add(_curPos);
        _curV = _movableObj.velocity;
        if (_curV != _preV)
            _timeSinceLastVChanged = 0;
        if ( _pathCount > _laserLen )
        {
            _pathList.RemoveAt(0);
            // 加速度不为0，说明长度改变了，重绘激光图像
            if (_curAcce != 0 || _timeSinceLastVChanged <= _laserLen)
            {
                _isDirty = true;
            }
            SetSourceEnable(false);
        }
        else
        {
            _pathCount++;
            // 激光未生成完全，且激光源未被消除
            int updateIndex = 0;
            if (!_isSourceEliminated)
            {
                if (_laserSegmentCount == 0)
                {
                    LaserSegment segment = CreateLaserSegment(0, 1);
                    _laserSegmentList.Add(segment);
                    _laserSegmentCount++;
                }
                else
                {
                    int firstSegmentIndex = 0;
                    Vector2 firstSegmentVec = _laserSegmentList[firstSegmentIndex].GetSegmentVec();
                    if (firstSegmentVec.x == 0)
                    {
                        firstSegmentVec.y += 1;
                        _laserSegmentList[firstSegmentIndex].SetSegmentVec(firstSegmentVec);
                    }
                    else
                    {
                        LaserSegment segment = CreateLaserSegment(0, 1);
                        _laserSegmentList.Insert(firstSegmentIndex, segment);
                        _laserSegmentCount++;
                    }
                }
                updateIndex = 1;
            }
            // 线段索引起始点、终点全部+1
            Vector2 segmentVec;
            for (int i = updateIndex; i < _laserSegmentCount; i++)
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

    public override void SetPosition(float posX, float posY)
    {
        //Logger.Log("Set LinearLaser To Position " + posX + "  , " + posY);
        if (_isInitPos)
            return;
        _isInitPos = true;
        _movableObj.Reset(posX, posY);
        _curPos = new Vector2(posX, posY);
        _lastPos = _curPos;
        _pathList.Add(_curPos);
    }

    public override void SetPosition(Vector2 pos)
    {
        //Logger.Log("Set LinearLaser To Position " + pos.x + "  , " + pos.y);
        // 直线激光只能设置初始位置
        if (_isInitPos)
            return;
        _isInitPos = true;
        _movableObj.Reset(pos.x,pos.y);
        _curPos = pos;
        _lastPos = _curPos;
        _pathList.Add(_curPos);
    }

    public override void SetOrderInLayer(int orderInLayer)
    {
        base.SetOrderInLayer(orderInLayer);
        _renderObjPos = true;
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

    public void SetSourceEnable(bool isEnable)
    {
        if ( _isSourceEnable != isEnable )
        {
            _isSourceEnable = isEnable;
            _laserSourceGo.SetActive(_isSourceEnable && !_isSourceEliminated);
            if ( _isSourceEnable )
            {
                _isInitSourcePos = false;
            }
        }
    }

    public override void Render()
    {
        if (_renderObjPos)
        {
            _renderObjPos = false;
            _objTrans.localPosition = new Vector3(0, 0, -_orderInLayer);
        }
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
        OnFrameStarted();
    }

    protected override void OnFrameStarted()
    {
        _isColorChanged = false;
        _isDirty = false;
        base.OnFrameStarted();
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
        if (index == 0)
        {
            if (_pathCount <= _laserLen && !_isSourceEliminated)
            {
                paras = new CollisionDetectParas()
                {
                    type = CollisionDetectType.Circle,
                    radius = DefaultCollisionHalfHeight,
                    nextIndex = 1,
                };
            }
            else
            {
                paras = new CollisionDetectParas()
                {
                    type = CollisionDetectType.Null,
                    nextIndex = 1,
                };
            }
        }
        else if ( _collisionSegmentCount < index )
        {
            paras = new CollisionDetectParas()
            {
                type = CollisionDetectType.Null,
                nextIndex = -1,
            };
        }
        else
        {
            index = index > 0 ? index : _collisionSegmentCount + index + 1;
            int segmentIndex = index - 1;
            Vector2 segment = _collisionSegmentList[segmentIndex];
            Vector2 lineVec = _pathList[(int)segment.y] - _pathList[(int)segment.x];
            float segmentLen = lineVec.magnitude;
            paras = new CollisionDetectParas()
            {
                type = CollisionDetectType.ItalicRect,
                centerPos = (_pathList[(int)segment.x] + _pathList[(int)segment.y]) / 2,
                nextIndex = segmentIndex + 1 >= _collisionSegmentCount ? -1 : index + 1,
                halfWidth = segmentLen / 2,
                halfHeight = DefaultCollisionHalfHeight,
                angle = _curRotation,
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
        if (n == 0)
        {
            _isSourceEliminated = true;
            SetSourceEnable(false);
        }
        else if (_collidedSegmentIndexList.IndexOf(n - 1) == -1)
        {
            _collidedSegmentIndexList.Add(n - 1);
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
            if (minDis <= DefaultCollisionHalfHeight + Global.PlayerGrazeRadius)
            {
                if (!_isGrazed)
                {
                    _isGrazed = true;
                    _grazeCoolDown = GrazeCoolDown;
                    PlayerInterface.GetInstance().AddGraze(1);
                }
                if (minDis <= DefaultCollisionHalfHeight + Global.PlayerCollisionVec.z)
                {
                    PlayerInterface.GetInstance().GetCharacter().BeingHit();
                    // 直线激光击中玩家不消除
                    //Eliminate(eEliminateDef.HitPlayer);
                }
            }
        }
    }
    #endregion

    public override void DoStraightMove(float v, float angle)
    {
        SetRotation(angle);
        _movableObj.DoStraightMove(v, _curRotation);
    }

    public override void DoAcceleration(float acce, float accAngle)
    {
        SetRotation(accAngle);
        _movableObj.DoAcceleration(acce, _curRotation);
    }

    public override void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity)
    {
        SetRotation(accAngle);
        _movableObj.DoAccelerationWithLimitation(acce, _curRotation, maxVelocity);
    }

    public override void SetStraightParas(float v, float angle, float acce, float accAngle)
    {
        if (v == 0 && acce != 0)
            SetRotation(accAngle);
        else
            SetRotation(angle);
        _movableObj.SetStraightParas(v, angle, acce, accAngle);
    }

    public override void SetPolarParas(float radius, float angle, float deltaR, float omega)
    {
        return;
    }

    public override void SetPolarParas(float radius, float angle, float deltaR, float omega, float centerPosX, float centerPosY)
    {
        return;
    }

    public override string BulletId
    {
        get
        {
            return _cfg.id;
        }
    }

    public override bool GetParaValue(STGObjectParaType paraType, out float value)
    {
        switch ( paraType )
        {
            case STGObjectParaType.Velocity:
                value = _movableObj.velocity;
                return true;
            case STGObjectParaType.VAngel:
                value = _movableObj.vAngle;
                return true;
            case STGObjectParaType.Acce:
                value = _movableObj.acce;
                return true;
            case STGObjectParaType.AccAngle:
                value = _movableObj.accAngle;
                return true;
            case STGObjectParaType.Alpha:
                value = _curAlpha;
                return true;
        }
        value = 0;
        return false;
    }

    public override bool SetParaValue(STGObjectParaType paraType, float value)
    {
        switch (paraType)
        {
            case STGObjectParaType.Velocity:
                {
                    _movableObj.velocity = value;
                    return true;
                }
            case STGObjectParaType.VAngel:
                if ( !_isInitAngle)
                {
                    _movableObj.vAngle = value;
                    _isInitAngle = true;
                }
                return true;
            case STGObjectParaType.Acce:
                {
                    _movableObj.acce = value;
                    return true;
                }
            case STGObjectParaType.AccAngle:
                if ( !_isInitAngle )
                {
                    _movableObj.accAngle = value;
                    _isInitAngle = true;
                }
                return true;
            case STGObjectParaType.Alpha:
                SetAlpha(value);
                return true;
        }
        value = 0;
        return false;
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


