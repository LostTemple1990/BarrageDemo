using UnityEngine;
using System.Collections.Generic;

public class STGSpriteEffect : STGEffectBase, ISTGMovable, IAttachment, IAttachable, ITaskExecuter
{
    private GameObject _effectGo;
    private Transform _effectTf;
    private SpriteRenderer _spRenderer;

    /// <summary>
    /// sprite对象原始宽度
    /// </summary>
    private float _originalWidth;
    /// <summary>
    /// sprite对象原始高度
    /// </summary>
    private float _originalHeight;
    /// <summary>
    /// sprite对象实际宽度
    /// </summary>
    private float _curWidth;
    /// <summary>
    /// sprite对象实际高度
    /// </summary>
    private float _curHeight;

    private float _curWidthScale;
    private float _curHeightScale;

    private bool _isScalingWidth;
    private int _scaleWidthTime;
    private int _scaleWidthDuration;
    private InterpolationMode _scaleWidthMode;
    private float _fromWidthScale;
    private float _toWidthScale;

    private bool _isScalingHeight;
    private int _scaleHeightTime;
    private int _scaleHeightDuration;
    private InterpolationMode _scaleHeightMode;
    private float _fromHeightScale;
    private float _toHeightScale;

    /// <summary>
    /// 当前角度
    /// </summary>
    private float _curRotation;
    private bool _isRotating;
    /// <summary>
    /// 每帧旋转的角度
    /// </summary>
    private float _rotateAngle;
    /// <summary>
    /// 旋转时间
    /// </summary>
    private int _rotateTime;
    /// <summary>
    /// 旋转持续时间
    /// </summary>
    private int _rotateDuration;
    /// <summary>
    /// 标识显示对象是否缓存在ObjectsPool中
    /// </summary>
    private bool _isUsingCache;
    /// <summary>
    /// 缓存名称标识
    /// <para>"SpriteEffect_"+AtlasName+"_"+SpriteName</para>
    /// </summary>
    private string _effectGoName;

    /// <summary>
    /// 是否正在渐隐消失
    /// </summary>
    private bool _isFading;
    /// <summary>
    /// 渐隐时间
    /// </summary>
    private int _fadeTime;
    /// <summary>
    /// 渐隐总时长
    /// </summary>
    private int _fadeDuration;
    private Color _spriteColor;
    private float _spriteAlpha;
    /// <summary>
    /// 渐隐起始时候的alpha
    /// </summary>
    private float _fadeBeginAlhpa;
    /// <summary>
    /// 在层级的顺序
    /// </summary>
    private int _orderInLayer;
    /// <summary>
    /// 当前位置
    /// </summary>
    private Vector2 _curPos;

    /// <summary>
    /// 移动对象
    /// </summary>
    private MovableObject _movableObject;

    /// <summary>
    /// 是否正在做alhpa渐变
    /// </summary>
    private bool _isDoingTweenAlhpa;
    private float _startAlhpa;
    private float _endAlpha;
    private int _tweenAlphaTime;
    private int _tweenAlphaDuration;

    private int _existDuration;

    /// <summary>
    /// 依附到的对象
    /// </summary>
    protected IAttachable _master;
    /// <summary>
    /// 标识是否随着master被销毁一同消失
    /// </summary>
    protected bool _isEliminatedWithMaster;
    /// <summary>
    /// 与master的相对位置
    /// </summary>
    protected Vector2 _relativePosToMaster;
    /// <summary>
    /// 是否连续跟随master
    /// </summary>
    protected bool _isFollowingMasterContinuously;
    /// <summary>
    /// 附件物体的列表
    /// </summary>
    protected List<IAttachment> _attachmentsList;
    /// <summary>
    /// 依附物体的个数
    /// </summary>
    protected int _attachmentsCount;

    protected List<Task> _taskList;
    protected int _taskCount;


    public STGSpriteEffect()
    {
        _effectType = EffectType.SpriteEffect;
        _taskList = new List<Task>();
        _attachmentsList = new List<IAttachment>();
        _attachmentsCount = 0;
    }

    public override void Clear()
    {
        ClearTasks();
        if ( _isUsingCache )
        {
            _effectTf.localScale = Vector3.one;
            _effectTf.localRotation = Quaternion.Euler(0, 0, 0);
            _spRenderer.color = new Color(1, 1, 1, 1);
            ObjectsPool.GetInstance().RestorePrefabToPool(_effectGoName, _effectGo);
        }
        else
        {
            GameObject.Destroy(_effectGo);
        }
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
        _effectGo = null;
        _effectTf = null;
        _spRenderer = null;
        if (_attachmentsCount > 0)
        {
            _attachmentsList.Clear();
            _attachmentsCount = 0;
        }
        _master = null;
    }

    public override void Init()
    {
        base.Init();
        _isFinish = false;
        _isUsingCache = false;
        _isFading = false;
        _orderInLayer = 0;
        _isScalingWidth = false;
        _isScalingHeight = false;
        _isRotating = false;
        _curRotation = 0;
        _isDoingTweenAlhpa = false;
        _existDuration = -1;
        _spriteAlpha = 1;
        _movableObject = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        _taskCount = 0;
        _originalWidth = _originalHeight = 0;
        _curWidth = _curHeight = 0;
    }

    public override void SetPosition(float posX, float posY)
    {
        _curPos = new Vector2(posX, posY);
        _movableObject.SetPos(posX, posY);
    }

    public void SetPosition(Vector2 pos)
    {
        _curPos = pos;
        _movableObject.SetPos(pos.x, pos.y);
    }

    public void SetScale(float scaleX,float scaleY)
    {
        _curWidthScale = scaleX;
        _curHeightScale = scaleY;
        _effectTf.localScale = new Vector3(_curWidthScale, _curHeightScale, 1);
    }

    public void SetSize(float width,float height)
    {
        _curWidth = width;
        _curHeight = height;
        _curWidthScale = _curWidth / _originalWidth;
        _curHeightScale = _curHeight / _originalHeight;
        _effectTf.localScale = new Vector3(_curWidthScale, _curHeightScale, 1);
    }

    public override void Update()
    {
        UpdateTasks();
        if ( _isScalingWidth )
        {
            ScaleWidth();
        }
        if ( _isScalingHeight )
        {
            ScaleHeight();
        }
        if ( _isScalingWidth || _isScalingHeight )
        {
            _curWidth = _curWidthScale * _originalWidth;
            _curHeight = _curWidthScale * _originalHeight;
            _effectTf.localScale = new Vector3(_curWidthScale, _curHeightScale, 1);
        }
        if ( _isRotating )
        {
            Rotate();
        }
        if ( _isDoingTweenAlhpa )
        {
            UpdateTweenAlpha();
        }
        UpdatePosition();
        if ( _existDuration > 0 )
        {
            _existDuration--;
            if ( _existDuration == 0 )
            {
                _isFinish = true;
            }
        }
    }

    public void ChangeWidthTo(float toWidth,int duration,InterpolationMode scaleMode)
    {
        _fromWidthScale = _curWidthScale;
        _toWidthScale = toWidth / _originalWidth;
        _scaleWidthTime = 0;
        _scaleWidthDuration = duration;
        _scaleWidthMode = scaleMode;
        _isScalingWidth = true;
    }

    public void ChangeHeightTo(float toHeight, int duration, InterpolationMode scaleMode)
    {
        _fromHeightScale = _curWidthScale;
        _toHeightScale = toHeight / _originalHeight;
        _scaleHeightTime = 0;
        _scaleHeightDuration = duration;
        _scaleHeightMode = scaleMode;
        _isScalingHeight = true;
    }

    private void ScaleWidth()
    {
        _scaleWidthTime++;
        switch ( _scaleWidthMode )
        {
            case InterpolationMode.EaseInQuad:
                _curWidthScale = MathUtil.GetEaseInQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseOutQuad:
                _curWidthScale = MathUtil.GetEaseOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Linear:
                _curWidthScale = MathUtil.GetLinearInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseInOutQuad:
                _curWidthScale = MathUtil.GetEaseInOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Sin:
                _curWidthScale = MathUtil.GetSinInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
        }
        if ( _scaleWidthTime >= _scaleWidthDuration )
        {
            _isScalingWidth = false;
        }
    }

    private void ScaleHeight()
    {
        _scaleHeightTime++;
        switch (_scaleHeightMode)
        {
            case InterpolationMode.EaseInQuad:
                _curHeightScale = MathUtil.GetEaseInQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseOutQuad:
                _curHeightScale = MathUtil.GetEaseOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Linear:
                _curHeightScale = MathUtil.GetLinearInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.EaseInOutQuad:
                _curHeightScale = MathUtil.GetEaseInOutQuadInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
            case InterpolationMode.Sin:
                _curHeightScale = MathUtil.GetSinInterpolation(_fromWidthScale, _toWidthScale, _scaleWidthTime, _scaleWidthDuration);
                break;
        }
        if (_scaleHeightTime >= _scaleHeightDuration)
        {
            _isScalingHeight = false;
        }
    }

    /// <summary>
    /// 执行alhpa缓动
    /// </summary>
    /// <param name="startAlhpa">起始透明度</param>
    /// <param name="endAlpha">结束透明度</param>
    /// <param name="duration">持续时间</param>
    public void DoTweenAlpha(float startAlhpa,float endAlpha,int duration)
    {
        _spriteColor = _spRenderer.color;
        _startAlhpa = startAlhpa;
        _endAlpha = endAlpha;
        _tweenAlphaTime = 0;
        _tweenAlphaDuration = duration;
        _isDoingTweenAlhpa = true;
    }

    /// <summary>
    /// 执行alpha缓动
    /// <para>起始透明度默认为当前透明度</para>
    /// </summary>
    /// <param name="endAlpha">结束透明度</param>
    /// <param name="duration">持续时间</param>
    public void DoTweenAlpha(float endAlpha,int duration)
    {
        _spriteColor = _spRenderer.color;
        _startAlhpa = _spriteColor.a;
        _endAlpha = endAlpha;
        _tweenAlphaTime = 0;
        _tweenAlphaDuration = duration;
        _isDoingTweenAlhpa = true;
    }

    /// <summary>
    /// 更新alpha缓动
    /// </summary>
    private void UpdateTweenAlpha()
    {
        _tweenAlphaTime++;
        if (_tweenAlphaTime < _tweenAlphaDuration)
        {
            _spriteAlpha = Mathf.Lerp(_startAlhpa, _endAlpha, (float)_tweenAlphaTime / _tweenAlphaDuration);
            _spriteColor.a = _spriteAlpha;
            _spRenderer.color = _spriteColor;
        }
        else
        {
            _isDoingTweenAlhpa = false;
        }
    }

    public void SetExistDuration(int duration)
    {
        _existDuration = duration;
    }

    /// <summary>
    /// 渐隐消失
    /// </summary>
    /// <param name="duration"></param>
    public void DoFade(int duration)
    {
        DoTweenAlpha(0, duration);
        SetExistDuration(duration);
    }

    private void UpdatePosition()
    {
        if (_isFollowingMasterContinuously && _master != null)
        {
            _curPos = _relativePosToMaster + _master.GetPosition();
        }
        else
        {
            if (_movableObject.IsActive())
            {
                _movableObject.Update();
                _curPos = _movableObject.GetPos();
            }
        }
        if (_effectTf != null)
            _effectTf.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
    }

    public void SetSprite(string spName)
    {
        _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
        _effectTf = _effectGo.transform;
        _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        _spRenderer.sprite = ResourceManager.GetInstance().GetSprite(Consts.EffectAtlasName, spName);
        _isUsingCache = false;
        UIManager.GetInstance().AddGoToLayer(_effectGo, LayerId.STGNormalEffect);
    }

    public void SetSprite(string atlasName,string spName,eBlendMode blendMode=eBlendMode.Normal,LayerId layerId=LayerId.STGNormalEffect,bool isUsingCache=false)
    {
        _isUsingCache = isUsingCache;
        // 不使用缓存，直接创建
        if ( !isUsingCache)
        {
            _effectGo = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
            _effectTf = _effectGo.transform;
            _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
            if ( blendMode != eBlendMode.Normal )
            {
                _spRenderer.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
            }
            _spRenderer.sprite = ResourceManager.GetInstance().GetSprite(atlasName, spName);
            UIManager.GetInstance().AddGoToLayer(_effectGo, layerId);
        }
        else
        {
            _effectGoName = "SpriteEffect_" + atlasName + "_" + spName + "_" + blendMode;
            _effectGo = ObjectsPool.GetInstance().GetPrefabAtPool(_effectGoName);
            if ( _effectGo == null )
            {
                GameObject protoType = ObjectsPool.GetInstance().GetProtoType(_effectGoName);
                if ( protoType == null )
                {
                    protoType = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", "SpriteEffect");
                    protoType.name = _effectGoName;
                    Transform tf = protoType.transform;
                    tf.localPosition = new Vector3(2000, 2000, 0);
                    SpriteRenderer sr = tf.Find("Sprite").GetComponent<SpriteRenderer>();
                    if (blendMode != eBlendMode.Normal)
                    {
                        sr.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(blendMode);
                    }
                    sr.sprite = ResourceManager.GetInstance().GetSprite(atlasName, spName);
                    UIManager.GetInstance().AddGoToLayer(protoType, LayerId.STGNormalEffect);
                    ObjectsPool.GetInstance().AddProtoType(_effectGoName, protoType);
                }
                _effectGo = GameObject.Instantiate<GameObject>(protoType);
                UIManager.GetInstance().AddGoToLayer(_effectGo, layerId);
            }
            _effectTf = _effectGo.transform;
            _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        }
        // 获取sprite对象的原始尺寸
        Sprite sp = _spRenderer.sprite;
        if ( sp != null )
        {
            Vector3 size = sp.bounds.extents * 2 * sp.pixelsPerUnit;
            _originalWidth = size.x;
            _originalHeight = size.y;
            // 计算实际尺寸
            _curWidth = _originalWidth;
            _curHeight = _originalHeight;
        }
        else
        {
            _originalWidth = 0;
            _originalHeight = 0;
            // 计算实际尺寸
            _curWidth = 0;
            _curHeight = 0;
        }
    }

    public void SetPrefab(string prefabName, LayerId layerId, bool isUsingCache = false)
    {
        _isUsingCache = isUsingCache;
        _effectGoName = prefabName;
        _effectGo = ObjectsPool.GetInstance().GetPrefabAtPool(_effectGoName);
        if (_effectGo == null)
        {
            GameObject protoType = ObjectsPool.GetInstance().GetProtoType(_effectGoName);
            if (protoType == null)
            {
                protoType = ResourceManager.GetInstance().GetPrefab("Prefab/Effects", prefabName);
                protoType.name = _effectGoName;
                Transform tf = protoType.transform;
                tf.localPosition = new Vector3(2000, 2000, 0);
                UIManager.GetInstance().AddGoToLayer(protoType, layerId);
                ObjectsPool.GetInstance().AddProtoType(_effectGoName, protoType);
            }
            _effectGo = GameObject.Instantiate<GameObject>(protoType);
            UIManager.GetInstance().AddGoToLayer(_effectGo, layerId);
        }
        _effectTf = _effectGo.transform;
        _spRenderer = _effectTf.Find("Sprite").GetComponent<SpriteRenderer>();
        // 获取sprite对象的原始尺寸
        Sprite sp = _spRenderer.sprite;
        if (sp != null)
        {
            Vector3 size = sp.bounds.extents * 2 * sp.pixelsPerUnit;
            _originalWidth = size.x;
            _originalHeight = size.y;
            // 计算实际尺寸
            _curWidth = _originalWidth;
            _curHeight = _originalHeight;
        }
    }

    /// <summary>
    /// 设置特效所在的层级
    /// </summary>
    /// <param name="layerId"></param>
    public void SetLayer(LayerId layerId)
    {
        UIManager.GetInstance().AddGoToLayer(_effectGo, layerId);
    }

    public void SetOrderInLayer(int orderInLayer)
    {
        _orderInLayer = orderInLayer;
    }

    public void SetBlendMode(eBlendMode mode)
    {
        _isUsingCache = false;
        _spRenderer.material = ResourceManager.GetInstance().GetSpriteMatByBlendMode(mode);
    }

    /// <summary>
    /// 设置透明度
    /// </summary>
    /// <param name="alpha"></param>
    public void SetSpriteAlpha(float alpha)
    {
        _spriteAlpha = alpha;
        Color color = _spRenderer.color;
        color.a = alpha;
        _spRenderer.color = color;
    }

    /// <summary>
    /// 透明度
    /// </summary>
    public float alpha
    {
        get { return _spriteAlpha; }
    }

    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="rValue"></param>
    /// <param name="gValue"></param>
    /// <param name="bValue"></param>
    /// <param name="aValue"></param>
    public void SetSpriteColor(float rValue,float gValue,float bValue,float aValue)
    {
        _spriteColor = new Color(rValue, gValue, bValue, aValue);
        _spriteAlpha = aValue;
        _spRenderer.color = _spriteColor;
    }

    public void SetSpriteColor(float rValue, float gValue, float bValue)
    {
        _spRenderer.color = new Color(rValue, gValue, bValue, _spriteAlpha);
    }

    public void SetRotation(float angle)
    {
        _curRotation = angle;
        _effectTf.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public float GetRotation()
    {
        return _curRotation;
    }

    public void DoRotation(float rotateAngle,int duration)
    {
        _rotateAngle = rotateAngle;
        _rotateTime = 0;
        _rotateDuration = duration;
        _isRotating = true;
    }

    private void Rotate()
    {
        _rotateTime++;
        _effectTf.Rotate(0, 0, _rotateAngle);
        if ( _rotateTime >= _rotateAngle )
        {
            _isRotating = false;
        }
    }

    public override bool NeedToBeRestoredToPool()
    {
        return true;
    }

    #region ISTGMovalbe

    public Vector2 GetPosition()
    {
        return _curPos;
    }

    public void DoStraightMove(float v, float angle)
    {
        _movableObject.DoStraightMove(v, angle);
    }

    public void MoveTo(float endX,float endY,int duration,InterpolationMode intMode)
    {
        _movableObject.MoveTo(endX, endY, duration, intMode);
    }

    public void MoveTowards(float v, float angle, int duration)
    {
        _movableObject.MoveTowards(v, angle, duration);
    }

    public void DoAcceleration(float acce, float accAngle)
    {
        _movableObject.DoAcceleration(acce, accAngle);
    }

    public void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity)
    {
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
    }

    public void DoMoveTo(float endX, float endY, int duration, InterpolationMode mode)
    {
        _movableObject.MoveTo(endX, endY, duration, mode);
    }

    public void SetStraightParas(float v, float vAngle, float acce, float accAngle)
    {
        _movableObject.SetStraightParas(v, vAngle, acce, accAngle);
    }

    public void SetPolarParas(float radius, float angle, float deltaR, float omega)
    {
        _movableObject.SetPolarParas(radius, angle, deltaR, omega);
    }

    public void SetPolarParas(float radius, float angle, float deltaR, float omega, float centerPosX, float centerPosY)
    {
        _movableObject.SetPolarParas(radius, angle, deltaR, omega, centerPosX, centerPosY);
    }

    public float velocity
    {
        get { return _movableObject.velocity; }
        set
        {
            _movableObject.velocity = value;
        }
    }
    /// <summary>
    /// x轴方向的速度
    /// </summary>
    public float vx
    {
        get { return _movableObject.vx; }
        set
        {
            _movableObject.vx = value;
        }
    }

    /// <summary>
    /// y轴方向的速度
    /// </summary>
    public float vy
    {
        get { return _movableObject.vy; }
        set
        {
            _movableObject.vy = value;
        }
    }

    public float vAngle
    {
        get { return _movableObject.vAngle; }
        set
        {
            _movableObject.vAngle = value;
        }
    }

    public float acce
    {
        get { return _movableObject.acce; }
        set
        {
            _movableObject.acce = value;
        }
    }

    public float accAngle
    {
        get { return _movableObject.accAngle; }
        set
        {
            _movableObject.accAngle = value;
        }
    }

    public float dx
    {
        get { return _movableObject.dx; }
    }

    public float dy
    {
        get { return _movableObject.dy; }
    }

    public float maxVelocity
    {
        get { return _movableObject.maxVelocity; }
        set
        {
            _movableObject.maxVelocity = value;
        }
    }

    public bool Eliminate(eEliminateDef eliminateType = 0)
    {
        FinishEffect();
        return true;
    }

    #endregion

    #region IAttachment
    public void AttachTo(IAttachable master, bool eliminatedWithMaster)
    {
        if (_master != null) return;
        _master = master;
        _master.AddAttachment(this);
        _isEliminatedWithMaster = eliminatedWithMaster;
    }

    public void SetRelativePos(float offsetX, float offsetY, float rotation, bool followMasterRotation, bool isFollowingMasterContinuously)
    {
        _relativePosToMaster = new Vector2(offsetX, offsetY);
        _isFollowingMasterContinuously = isFollowingMasterContinuously;
        if (_master != null)
        {
            _curPos = _master.GetPosition() + _relativePosToMaster;
        }
    }

    public void OnMasterEliminated(eEliminateDef eliminateType)
    {
        _master = null;
        _isFinish = true;
    }

    public void AddAttachment(IAttachment attachment)
    {
        for (int i = 0; i < _attachmentsCount; i++)
        {
            if (_attachmentsList[i] == attachment)
            {
                return;
            }
        }
        _attachmentsList.Add(attachment);
        _attachmentsCount++;
    }

    public void OnAttachmentEliminated(IAttachment attachment)
    {
        for (int i = 0; i < _attachmentsCount; i++)
        {
            if (_attachmentsList[i] == attachment)
            {
                _attachmentsList[i] = null;
            }
        }
    }

    #endregion

    #region task
    public void AddTask(Task task)
    {
        _taskList.Add(task);
        _taskCount++;
    }

    private void UpdateTasks()
    {
        Task task;
        for (int i = 0; i < _taskCount; i++)
        {
            task = _taskList[i];
            if (task != null)
            {
                task.Update();
                if (task.isFinish)
                {
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
                    _taskList[i] = null;
                }
            }
        }
    }

    private void ClearTasks()
    {
        Task task;
        for (int i = 0; i < _taskCount; i++)
        {
            task = _taskList[i];
            if (task != null)
            {
                if (task.luaState != null)
                {
                    InterpreterManager.GetInstance().StopTaskThread(task.luaState, task.funcRef);
                }
                ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
            }
        }
        _taskList.Clear();
        _taskCount = 0;
    }
    #endregion
}
