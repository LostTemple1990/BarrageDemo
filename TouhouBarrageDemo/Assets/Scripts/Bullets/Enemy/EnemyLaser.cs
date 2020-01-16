using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : EnemyBulletBase
{
    /// <summary>
    /// 激光源自转的速度
    /// </summary>
    private const int SourceRotateOmega = 18;
    /// <summary>
    /// 默认的发射源的大小
    /// </summary>
    private const float SourceDefaultSize = 0.32f;
    /// <summary>
    /// 擦弹冷却时间
    /// </summary>
    private const int GrazeCoolDown = 3;

    protected float _laserRotateOmega;
    protected int _rotateCounter;
    protected int _rotateDuration;
    protected float _rotateToAngle;
    protected bool _isRotating;

    protected Vector3 _endPos;
    protected int _moveCounter;
    protected int _moveDuration;

    protected float _grazeHalfWidth;
    protected float _grazeHalfHeight;
    protected float _collisionHalfWidth;
    protected float _collisionHalfHeight;

    /// <summary>
    /// 设定的宽度
    /// </summary>
    protected float _width;
    /// <summary>
    /// 设定的长度
    /// </summary>
    protected float _length;
    /// <summary>
    /// 当前的宽度
    /// </summary>
    protected float _curWidth;
    /// <summary>
    /// 当前的长度
    /// </summary>
    protected float _curLength;
    /// <summary>
    /// 激光长度的一半
    /// </summary>
    protected float _laserHalfLength;
    /// <summary>
    /// 激光宽度的一半
    /// </summary>
    protected float _laserHalfWidth;

    /// <summary>
    /// 激光容器object
    /// </summary>
    protected GameObject _laserObj;
    /// <summary>
    /// 激光容器tf
    /// </summary>
    protected Transform _objTrans;
    /// <summary>
    /// 激光本体SpriteRenderer
    /// </summary>
    protected SpriteRenderer _laser;
    /// <summary>
    /// 激光本体tf
    /// </summary>
    protected Transform _laserTrans;
    /// <summary>
    /// 激光源是否可见
    /// </summary>
    private bool _isSourceEnable;
    /// <summary>
    /// 激光源tf
    /// </summary>
    private Transform _sourceTf;
    /// <summary>
    /// 激光源sp
    /// </summary>
    private SpriteRenderer _sourceSp;

    protected int _existTime;
    protected int _existDuration;

    /// <summary>
    /// 表示当前正在改变宽度
    /// </summary>
    protected bool _isChangingWidth;
    protected int _changeWidthDuration;
    protected int _changeWidthTime;
    protected int _changeWidthDelay;
    protected float _changeFromWidth;
    protected float _changeToWidth;

    /// <summary>
    /// 表示当前正在改变长度
    /// </summary>
    protected bool _isChangingLength;
    protected int _changeLengthDuration;
    protected int _changeLengthTime;
    protected int _changeLengthDelay;
    protected float _changeFromLength;
    protected float _changeToLength;

    /// <summary>
    /// 当前是否正在改变alpha
    /// </summary>
    protected bool _isChangingAlpha;
    protected int _changeAlphaDelay;
    protected int _changeAlphaDuration;
    protected int _changeAlphaTime;
    protected float _changeFromAlpha;
    protected float _changeToAlpha;

    private bool _isTurningOn;
    /// <summary>
    /// 激光展开的计时
    /// </summary>
    private int _turnOnTime;
    /// <summary>
    /// 激光展开的总时间
    /// </summary>
    private int _turnOnDuration;

    /// <summary>
    /// 设置激光碰撞宽度和宽度的比例
    /// </summary>
    protected float _collisionFactor;

    /// <summary>
    /// 是否需要Resize激光
    /// </summary>
    protected bool _isDirty;
    /// <summary>
    /// 对应激光的配置
    /// </summary>
    protected EnemyLinearLaserCfg _cfg;
    /// <summary>
    /// 运动物体
    /// </summary>
    private MovableObject _movableObject;

    public EnemyLaser()
    {
        _sysBusyWeight = 3;
        _type = BulletType.Enemy_Laser;
    }

    public override void Init()
    {
        base.Init();
        _curPos = Vector2.zero;
        _existTime = 0;
        _existDuration = -1;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        _isChangingWidth = false;
        _isChangingLength = false;
        _isChangingAlpha = false;
        _curWidth = _laserHalfWidth = 0;
        _curAlpha = 0;
        _isDirty = false;
        _collisionFactor = 0.5f;
        _isRotating = false;
        _isSourceEnable = false;
        _isTurningOn = false;
        _resistEliminateFlag |= (int)(eEliminateDef.HitPlayer | eEliminateDef.PlayerSpellCard);
        _movableObject = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
    }

    public override void SetStyleById(string id)
    {
        EnemyLinearLaserCfg cfg = BulletsManager.GetInstance().GetLinearLaserCfgById(id);
        if (cfg == null)
        {
            Logger.LogError("LaserCfg with id " + id + " is not exist!");
            return;
        }
        if ( _laserObj != null )
        {
            ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _laserObj);
        }
        _cfg = cfg;
        _prefabName = _cfg.id;
        _laserObj = BulletsManager.GetInstance().CreateBulletGameObject(BulletType.Enemy_Laser, _cfg.id);
        _objTrans = _laserObj.transform;
        _laserTrans = _objTrans.Find("LaserSprite");
        _laser = _laserTrans.GetComponent<SpriteRenderer>();
        _sourceTf = _objTrans.Find("Source");
        _sourceSp = _sourceTf.GetComponent<SpriteRenderer>();
        _isDirty = true;
    }

    public override void SetPosition(Vector2 vec)
    {
        _curPos.x = vec.x;
        _curPos.y = vec.y;
        _movableObject.SetPos(vec.x, vec.y);
    }

    public override void SetPosition(float posX, float posY)
    {
        _curPos = new Vector2(posX, posY);
        _movableObject.SetPos(posX, posY);
    }

    /// <summary>
    /// delay帧之后改变宽度到toWidth，改变时间为duration帧
    /// </summary>
    /// <param name="toWidth"></param>
    /// <param name="duration"></param>
    /// <param name="delay"></param>
    public virtual void ChangeToWidth(float toWidth,int delay, int duration)
    {
        _changeWidthDelay = delay;
        if ( delay == 0 )
        {
            _changeFromWidth = _curWidth;
            if (duration==0)
            {
                SetWidth(toWidth);
                return;
            }
        }
        _changeToWidth = toWidth;
        _changeWidthDuration = duration;
        _changeWidthTime = 0;
        _isChangingWidth = true;
    }

    /// <summary>
    /// delay帧之后改变宽度到toHeight，改变时间为duration帧
    /// </summary>
    /// <param name="toWidth"></param>
    /// <param name="duration"></param>
    /// <param name="delay"></param>
    public virtual void ChangeToLength(float toLength, int delay, int duration)
    {
        _changeLengthDelay = delay;
        if ( delay == 0 )
        {
            _changeFromLength = _curLength;
            if(duration==0)
            {
                SetLength(toLength);
                return;
            }
        }
        _changeToLength = toLength;
        _changeLengthDuration = duration;
        _changeLengthTime = 0;
        _isChangingLength = true;
    }

    public void ChangeToAlpha(float toAlpha,int delay,int duration)
    {
        _changeAlphaDelay = delay;
        if ( delay == 0 )
        {
            _changeFromAlpha = _curAlpha;
            if(duration==0)
            {
                SetAlpha(toAlpha);
                return;
            }
        }
        _changeToAlpha = toAlpha;
        _changeAlphaDuration = duration;
        _changeAlphaTime = 0;
        _isChangingAlpha = true;
    }

    public virtual void SetLaserExistDuration(int existDuration)
    {
        _existTime = 0;
        _existDuration = existDuration;
    }

    public virtual void SetSize(float width, float length)
    {
        if (width != Consts.OriginalWidth)
            _width = width;
        if (length != Consts.OriginalHeight)
            SetLength(length);
    }

    public void SetSourceSize(float size)
    {
        _isSourceEnable = size != 0;
        _sourceTf.gameObject.SetActive(_isSourceEnable);
        if (_isSourceEnable)
        {
            float scale = size / SourceDefaultSize;
            _sourceSp.transform.localScale = new Vector3(scale, scale, 1);
        }
    }

    public void TurnOn(int duration)
    {
        if (duration > 0 )
        {
            _isTurningOn = true;
            _turnOnDuration = duration;
            _turnOnTime = 0;
        }
        ChangeToWidth(_width, 0, duration);
        ChangeToAlpha(1, 0, duration);
    }

    public void TurnHalfOn(float toWidth,int duration)
    {
        ChangeToWidth(toWidth, 0, duration);
        ChangeToAlpha(0.5f, 0, duration);
    }

    public void TurnOff(int duration)
    {
        ChangeToWidth(0, 0, duration);
        ChangeToAlpha(0, 0, duration);
    }

    public override void DoStraightMove(float v, float angle)
    {
        _movableObject.DoStraightMove(v, angle);
    }

    public override void DoAcceleration(float acce, float accAngle)
    {
        _movableObject.DoAcceleration(acce, accAngle);
    }

    public override void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity)
    {
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
    }

    public override void SetStraightParas(float v, float angle, float acce, float accAngle)
    {
        _movableObject.DoStraightMove(v, angle);
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, -1);
    }

    public override void SetPolarParas(float radius, float angle, float deltaR, float omega)
    {
        _movableObject.SetPolarParas(radius, angle, deltaR, omega);
    }

    public override void SetPolarParas(float radius, float angle, float deltaR, float omega, float centerPosX, float centerPosY)
    {
        _movableObject.SetPolarParas(radius, angle, deltaR, omega, centerPosX, centerPosY);
    }

    public void DoRotate(float toAngle,int duration)
    {
        _rotateToAngle = toAngle;
        _rotateCounter = 0;
        _rotateDuration = duration;
        _laserRotateOmega = (toAngle - _curRotation) / duration;
        _isRotating = true;
    }

    public void DoRotateWithOmega(float omega,int duration)
    {
        _rotateToAngle = _curRotation + duration * omega;
        _rotateCounter = 0;
        _rotateDuration = duration;
        _laserRotateOmega = omega;
        _isRotating = true;
    }

    /// <summary>
    /// 设置碰撞系数
    /// </summary>
    /// <param name="factor"></param>
    public void SetCollisionFactor(float factor)
    {
        _collisionFactor = Mathf.Clamp01(factor);
    }

    public override void Update()
    {
        base.Update();
        UpdateComponents();
        if (_isChangingWidth) ChangingWidth();
        if (_isChangingLength) ChangingLength();
        if (_isChangingAlpha) ChangingAlpha();
        if (_isRotating) Rotate();
        if (_turnOnDuration > 0) TurningOn();
        UpdatePosition();
        UpdateGrazeCoolDown();
        UpdateExistTime();
        CheckCollisionWithCharacter();
    }

    public override void Render()
    {
        RenderTransform();
        if (_isDirty)
            Resize();
        if (_isColorChanged)
        {
            _laser.color = new Color(1, 1, 1, _curAlpha);
        }
        if (_isSourceEnable)
            RenderSource();
    }

    /// <summary>
    /// update-宽度变更
    /// </summary>
    protected void ChangingWidth()
    {
        if ( _changeWidthDelay > 0 )
        {
            _changeWidthDelay--;
            if ( _changeWidthDelay == 0 )
            {
                _changeFromWidth = _curWidth;
            }
        }
        else
        {
            _changeWidthTime++;
            float newWidth = MathUtil.GetLinearInterpolation(_changeFromWidth, _changeToWidth, _changeWidthTime, _changeWidthDuration);
            SetWidth(newWidth);
            if ( _changeWidthTime >= _changeWidthDuration )
            {
                _isChangingWidth = false;
            }
        }
    }

    /// <summary>
    /// update-长度变更
    /// </summary>
    protected void ChangingLength()
    {
        if (_changeLengthDelay > 0)
        {
            _changeLengthDelay--;
            if (_changeLengthDelay == 0)
            {
                _changeFromLength = _curLength;
            }
        }
        else
        {
            _changeLengthTime++;
            float newLen = MathUtil.GetLinearInterpolation(_changeFromLength, _changeToLength, _changeLengthTime, _changeLengthDuration);
            SetLength(newLen);
            if (_changeLengthTime >= _changeLengthDuration)
            {
                _isChangingLength = false;
            }
        }
    }

    /// <summary>
    /// update--alpha变更
    /// </summary>
    protected void ChangingAlpha()
    {
        if ( _changeAlphaDelay > 0 )
        {
            _changeAlphaDelay--;
            if ( _changeAlphaDelay == 0 )
            {
                _changeFromAlpha = _curAlpha;
            }
        }
        else
        {
            _changeAlphaTime++;
            SetAlpha(Mathf.Lerp(_changeFromAlpha,_changeToAlpha,(float)_changeAlphaTime/_changeAlphaDuration));
            if ( _changeAlphaTime >= _changeAlphaDuration )
            {
                _isChangingAlpha = false;
            }
        }
    }

    /// <summary>
    /// 激光展开
    /// </summary>
    private void TurningOn()
    {
        _turnOnTime++;
        if (_turnOnTime >= _turnOnDuration)
        {
            _detectCollision = true;
            _turnOnDuration = -1;
        }
    }

    private void UpdatePosition()
    {
        if (_isFollowingMasterContinuously)
        {
            if (_attachableMaster != null)
            {
                Vector2 relativePos = _relativePosToMaster;
                if (_isFollowMasterRotation)
                {
                    relativePos = MathUtil.GetVec2AfterRotate(relativePos.x, relativePos.y, 0, 0, _attachableMaster.GetRotation());
                    SetRotation(_attachableMaster.GetRotation() + _relativeRotationToMaster);
                }
                _curPos = relativePos + _attachableMaster.GetPosition();
            }
        }
        else if ( _movableObject.IsActive() )
        {
            _movableObject.Update();
            _curPos = _movableObject.GetPos();
        }
    }

    protected virtual void Rotate()
    {
        float curRotation = _curRotation + _laserRotateOmega;
        _rotateCounter++;
        if ( _rotateCounter >= _rotateDuration )
        {
            curRotation %= 360f;
            if (curRotation < 0)
                curRotation += 360f;
            _isRotating = false;
        }
        SetRotation(curRotation);
    }

    protected virtual void RenderTransform()
    {
        if ( _isRotationDirty )
        {
            _isRotationDirty = false;
            _objTrans.localRotation = Quaternion.Euler(0, 0, _curRotation - _cfg.texDefaultRotation);
        }
        _objTrans.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
    }

    protected virtual void Resize()
    {
        _laser.size = new Vector2(_curLength, _curWidth);
        Vector3 pos = Vector3.zero;
        pos.x = _laserHalfLength;
        _laserTrans.localPosition = pos;
        _isDirty = false;
    }

    /// <summary>
    /// 渲染激光源
    /// </summary>
    private void RenderSource()
    {
        _sourceTf.localRotation = Quaternion.Euler(0, 0, _timeSinceCreated * SourceRotateOmega);
    }

    protected void UpdateExistTime()
    {
        if (_existDuration == -1) return;
        _existTime++;
        if ( _existTime >= _existDuration )
        {
            Eliminate(eEliminateDef.CodeEliminate);
        }
    }

    #region 碰撞检测相关参数

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        return true;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index=0)
    {
        CollisionDetectParas paras = new CollisionDetectParas();
        paras.nextIndex = -1;
        paras.type = CollisionDetectType.Rect;
        paras.halfWidth = _laserHalfWidth * _collisionFactor;
        paras.halfHeight = _laserHalfLength;
        paras.angle = _curRotation;
        // 计算矩形中心坐标
        Vector2 center = new Vector2();
        float cos = Mathf.Cos(_curRotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(_curRotation * Mathf.Deg2Rad);
        // 矩形中心坐标
        center.x = _laserHalfLength * cos + _curPos.x;
        center.y = _laserHalfLength * sin + _curPos.y;
        paras.centerPos = center;
        return paras;
    }

    public override void CollidedByObject(int n = 0, eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {
        Eliminate(eliminateDef);
    }

    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
        base.SetGrazeDetectParas(paras);
    }

    protected virtual void CheckCollisionWithCharacter()
    {
        if (!_detectCollision) return;
        if (_curWidth == 0 || _curLength == 0 || _curAlpha < 1) return;
        Vector2 center = new Vector2();
        float cos = Mathf.Cos(_curRotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(_curRotation * Mathf.Deg2Rad);
        // 矩形中心坐标
        center.x = _laserHalfLength * cos + _curPos.x;
        center.y = _laserHalfLength * sin + _curPos.y;
        Vector2 vec = _player.GetPosition() - center;
        float dw = Mathf.Max(0, Mathf.Abs(cos * vec.x + sin * vec.y) - _laserHalfLength);
        float tmpY = Mathf.Abs(-sin * vec.x + cos * vec.y);
        float dh = Mathf.Max(0, tmpY - _laserHalfWidth);
        float sqrDis = dw * dw + dh * dh;
        if (sqrDis <= _player.grazeRadius * _player.grazeRadius)
        {
            if (!_isGrazed)
            {
                _isGrazed = true;
                PlayerInterface.GetInstance().AddGraze(1);
                _grazeCoolDown = GrazeCoolDown;
            }
            dh = Mathf.Max(0, tmpY - _laserHalfWidth * _collisionFactor);
            sqrDis = dw * dw + dh * dh;
            if (sqrDis <= _player.collisionRadius * _player.collisionRadius)
            {
                Eliminate(eEliminateDef.HitPlayer);
                PlayerInterface.GetInstance().GetCharacter().BeingHit();
            }
        }
    }
    #endregion

    /// <summary>
    /// 设置激光的宽度
    /// </summary>
    /// <param name="value"></param>
    public void SetWidth(float value)
    {
        if ( value != _curWidth )
        {
            _curWidth = value;
            _laserHalfWidth = value / 2;
            _isDirty = true;
        }
    }

    /// <summary>
    /// 设置激光的高度
    /// </summary>
    /// <param name="value"></param>
    public void SetLength(float value)
    {
        if ( value != _curLength )
        {
            _curLength = value;
            _laserHalfLength = value / 2;
            _isDirty = true;
        }
    }

    /// <summary>
    /// 激光长度
    /// </summary>
    /// <returns></returns>
    public float GetLength()
    {
        return _curLength;
    }

    /// <summary>
    /// 激光宽度
    /// </summary>
    /// <returns></returns>
    public float GetWidth()
    {
        return _curWidth;
    }

    public override string BulletId
    {
        get
        {
            return _cfg.id;
        }
    }

    #region 设置/获取子弹参数的相关public方法
    public override bool GetBulletPara(BulletParaType paraType, out float value)
    {
        value = 0;
        switch (paraType)
        {
            case BulletParaType.Alpha:
                value = _curAlpha;
                return true;
            case BulletParaType.LaserLength:
                value = _curLength;
                return true;
            case BulletParaType.LaserWidth:
                value = _curWidth;
                return true;
            case BulletParaType.CurveOmega:
                value = _laserRotateOmega;
                return true;
        }
        return false;
    }

    public override bool SetBulletPara(BulletParaType paraType, float value)
    {
        switch (paraType)
        {
            case BulletParaType.Alpha:
                SetAlpha(value);
                return true;
            case BulletParaType.LaserLength:
                SetLength(value);
                return true;
            case BulletParaType.LaserWidth:
                SetWidth(value);
                return true;
            case BulletParaType.CurveOmega:
                _laserRotateOmega = value;
                return true;
        }
        return false;
    }

    public override float velocity
    {
        get { return _movableObject.velocity; }
        set { _movableObject.velocity = value; }
    }

    public override float vx
    {
        get { return _movableObject.vx; }
        set { _movableObject.vx = value; }
    }

    public override float vy
    {
        get { return _movableObject.vy; }
        set { _movableObject.vy = value; }
    }

    public override float maxVelocity
    {
        get { return _movableObject.maxVelocity; }
        set { _movableObject.maxVelocity = value; }
    }

    public override float vAngle
    {
        get { return _movableObject.vAngle; }
        set { _movableObject.vAngle = value; }
    }

    public override float acce
    {
        get { return _movableObject.acce; }
        set { _movableObject.acce = value; }
    }

    public override float accAngle
    {
        get { return _movableObject.accAngle; }
        set { _movableObject.accAngle = value; }
    }

    #endregion

    public override void Clear()
    {
        if ( _laserObj != null )
        {
            _laser.size = Vector2.zero;
            if ( !_isOriginalColor )
            {
                _laser.color = new Color(1, 1, 1, 1);
            }
            ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _laserObj);
        }
        _laserObj = null;
        _objTrans = null;
        _laser = null;
        _laserTrans = null;
        if (_isSourceEnable)
        {
            _sourceTf.gameObject.SetActive(false);
        }
        _sourceTf = null;
        _sourceSp = null;
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
        base.Clear();
    }
}
