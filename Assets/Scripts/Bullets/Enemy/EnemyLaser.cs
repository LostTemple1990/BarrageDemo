using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : EnemyBulletBase
{
    /// <summary>
    /// 擦弹冷却时间
    /// </summary>
    private const int GrazeCoolDown = 3;

    protected float _laserRotateOmega;
    protected int _rotateCounter;
    protected int _rotateDuration;
    protected float _rotateToAngle;
    protected bool _isRotating;
    protected bool _moveFlag;

    protected Vector3 _endPos;
    protected int _moveCounter;
    protected int _moveDuration;

    protected float _grazeHalfWidth;
    protected float _grazeHalfHeight;
    protected float _collisionHalfWidth;
    protected float _collisionHalfHeight;
    /// <summary>
    /// 激光的宽度
    /// </summary>
    protected float _laserWidth;
    /// <summary>
    /// 激光的长度
    /// </summary>
    protected float _laserLength;
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

    public EnemyLaser()
    {
        _sysBusyWeight = 3;
        _type = BulletType.Enemy_Laser;
    }

    public override void Init()
    {
        base.Init();
        _moveFlag = false;
        _curPos = Vector2.zero;
        _existTime = 0;
        _existDuration = -1;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        _isChangingWidth = false;
        _isChangingLength = false;
        _isChangingAlpha = false;
        _laserLength = _laserHalfWidth = -1;
        _isDirty = false;
        _collisionFactor = 0.8f;
        _isRotating = false;
        _resistEliminateFlag |= (int)(eEliminateDef.HitPlayer | eEliminateDef.PlayerSpellCard);
    }

    public override void SetBulletTexture(string texture)
    {
        if ( _laserObj == null )
        {
            _laserObj = ResourceManager.GetInstance().GetPrefab("BulletPrefab", _prefabName);
            _objTrans = _laserObj.transform;
            _laser = _objTrans.Find("LaserSprite").GetComponent<SpriteRenderer>();
            _laserTrans = _laser.transform;
            UIManager.GetInstance().AddGoToLayer(_laserObj, LayerId.EnemyBarrage);
        }
        _laser.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName,"Bullet" + texture);
        _isDirty = true;
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
        _moveFlag = true;
        _isDirty = true;
    }

    public override void SetToPosition(Vector2 vec)
    {
        _curPos.x = vec.x;
        _curPos.y = vec.y;
        _moveFlag = true;
    }

    public override void SetToPosition(float posX, float posY)
    {
        _curPos = new Vector2(posX, posY);
        _moveFlag = true;
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
            _changeFromWidth = _laserWidth;
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
            _changeFromLength = _laserLength;
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

    public virtual void SetLaserSize(float length,float width)
    {
        if (length != Consts.OriginalHeight)
        {
            SetLength(length);
        }
        if ( width != Consts.OriginalWidth )
        {
            SetWidth(width);
        }
    }

    public void DoMove(Vector3 endPos,int duration)
    {
        _endPos = endPos;
        _moveCounter = 0;
        _moveDuration = duration;
        _vx = (endPos.x - _curPos.x) / duration;
        _vy = (endPos.y - _curPos.y) / duration;
        _isMoving = true;
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
        if ( _isSetRelativePosToMaster )
        {
            if ( _attachableMaster != null )
            {
                Vector2 relativePos = _relativePosToMaster;
                if ( _isFollowMasterRotation )
                {
                    relativePos = MathUtil.GetVec2AfterRotate(relativePos.x, relativePos.y, 0, 0, _attachableMaster.GetRotation());
                    SetRotation(_attachableMaster.GetRotation() + _relativeRotationToMaster);
                }
                _curPos = relativePos + _attachableMaster.GetPosition();
                _moveFlag = true;
            }
        }
        else if ( _isMoving )
        {
            Move();
        }
        if ( _isRotating )
        {
            Rotate();
        }
        UpdateGrazeCoolDown();
        UpdatePosition();
        CheckCollisionWithCharacter();
        UpdateExistTime();
        if (_isDirty)
        {
            Resize();
        }
        if ( _isColorChanged )
        {
            _laser.color = new Color(1, 1, 1, _curAlpha);
        }
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
                _changeFromWidth = _laserWidth;
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
                _changeFromLength = _laserLength;
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

    protected override void Move()
    {
        //_dx = _curPos.x;
        //_dy = _curPos.y;
        _curPos.x += _vx;
        _curPos.y += _vy;
        _moveCounter++;
        if ( _moveCounter >= _moveDuration )
        {
            _curPos = _endPos;
            _isMoving = false;
        }
        _moveFlag = true;
    }

    protected virtual void Rotate()
    {
        float curRotation = _curRotation + _laserRotateOmega;
        _rotateCounter++;
        if ( _rotateCounter >= _rotateDuration )
        {
            while (curRotation < 0 )
            {
                curRotation += 360;
            }
            curRotation = curRotation % 360;
            _isRotating = false;
        }
        SetRotation(curRotation);
    }

    protected virtual void UpdatePosition()
    {
        if ( _isRotationDirty )
        {
            _isRotationDirty = false;
            _objTrans.localRotation = Quaternion.Euler(0, 0, _curRotation - _cfg.texDefaultRotation);
        }
        if ( _moveFlag )
        {
            _moveFlag = false;
            _objTrans.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
        }
    }

    protected virtual void Resize()
    {
        _laser.size = new Vector2(_laserLength, _laserWidth);
        Vector3 pos = Vector3.zero;
        pos.x = _laserHalfLength;
        _laserTrans.localPosition = pos;
        _isDirty = false;
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
        paras.halfWidth = _laserHalfWidth;
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
        Vector2 center = new Vector2();
        float cos = Mathf.Cos(_curRotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(_curRotation * Mathf.Deg2Rad);
        // 矩形中心坐标
        center.x = _laserHalfLength * cos + _curPos.x;
        center.y = _laserHalfLength * sin + _curPos.y;
        Vector2 vec = Global.PlayerPos - center;
        Vector2 relativeVec = new Vector2();
        // 向量顺时针旋转laserAngle的度数
        relativeVec.x = cos * vec.x + sin * vec.y;
        relativeVec.y = -sin * vec.x + cos * vec.y;
        // 判定是否在矩形内
        float len = relativeVec.magnitude;
        float rate = (len - Global.PlayerGrazeRadius) / len;
        bool isGrazing = false;
        if ( rate < 0 )
        {
            isGrazing = true;
        }
        else
        {
            Vector2 tmpVec = relativeVec * rate;
            if (Mathf.Abs(tmpVec.x) < _laserHalfLength && Mathf.Abs(tmpVec.y) < _laserHalfWidth)
            {
                isGrazing = true;
            }
        }
        if ( isGrazing )
        {
            if ( !_isGrazed )
            {
                _isGrazed = true;
                PlayerService.GetInstance().AddGraze(1);
                _grazeCoolDown = GrazeCoolDown;
            }
            rate = (len - Global.PlayerCollisionVec.z) / len;
            relativeVec *= rate;
            if (rate <= 0 || (Mathf.Abs(relativeVec.x) < _laserHalfLength && Mathf.Abs(relativeVec.y) < _laserHalfWidth * _collisionFactor))
            {
                Eliminate(eEliminateDef.HitPlayer);
                PlayerService.GetInstance().GetCharacter().BeingHit();
            }
        }
    }
    #endregion

    /// <summary>
    /// 设置激光的宽度
    /// </summary>
    /// <param name="value"></param>
    private void SetWidth(float value)
    {
        if ( value != _laserWidth )
        {
            _laserWidth = value;
            _laserHalfWidth = value / 2;
            _isDirty = true;
        }
    }

    /// <summary>
    /// 设置激光的高度
    /// </summary>
    /// <param name="value"></param>
    private void SetLength(float value)
    {
        if ( value != _laserLength )
        {
            _laserLength = value;
            _laserHalfLength = value / 2;
            _isDirty = true;
        }
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
                value = _laserLength;
                return true;
            case BulletParaType.LaserWidth:
                value = _laserWidth;
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
        base.Clear();
    }
}
