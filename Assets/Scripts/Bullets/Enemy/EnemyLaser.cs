﻿using System.Collections;
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

    protected float _laserWidth;
    protected float _laserHeight;
    protected float _laserHalfHeight;
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
    /// 表示当前正在改变高度
    /// </summary>
    protected bool _isChangingHeight;
    protected int _changeHeightDuration;
    protected int _changeHeightTime;
    protected int _changeHeightDelay;
    protected float _changeFromHeight;
    protected float _changeToHeight;

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
        _curPos = Vector3.zero;
        _existTime = 0;
        _existDuration = -1;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        _isChangingWidth = false;
        _isChangingHeight = false;
        _laserHalfWidth = _laserHalfHeight = 0;
        _isDirty = false;
        _collisionFactor = 0.8f;
        _isRotating = false;
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
    public virtual void ChangeToWidth(float toWidth,int duration,int delay)
    {
        _changeWidthDelay = delay;
        if ( delay == 0 )
        {
            _changeFromWidth = _laserHalfWidth;
        }
        _changeToWidth = toWidth / 2;
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
    public virtual void ChangeToHeight(float toHeight, int duration, int delay)
    {
        _changeHeightDelay = delay;
        if ( delay == 0 )
        {
            _changeFromHeight = _laserHalfHeight;
        }
        _changeToHeight = toHeight / 2;
        _changeHeightDuration = duration;
        _changeHeightTime = 0;
        _isChangingHeight = true;
    }

    public virtual void SetLaserExistDuration(int existDuration)
    {
        _existTime = 0;
        _existDuration = existDuration;
    }

    public virtual void SetLaserSize(float width,float height)
    {
        if ( width != Consts.OriginalWidth )
        {
            _laserWidth = width;
            _laserHalfWidth = width / 2;
            _isDirty = true;
        }
        if ( height != Consts.OriginalHeight )
        {
            _laserHeight = height;
            _laserHalfHeight = height / 2;
            _isDirty = true;
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
        if ( _isChangingWidth )
        {
            ChangingWidth();
        }
        if ( _isChangingHeight )
        {
            ChangingHeight();
        }
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
        if ( _isDirty )
        {
            Resize();
        }
        UpdateGrazeCoolDown();
        UpdatePosition();
        CheckCollisionWithCharacter();
        UpdateExistTime();
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
                _changeFromWidth = _laserHalfWidth;
            }
        }
        else
        {
            _changeWidthTime++;
            _laserHalfWidth = MathUtil.GetLinearInterpolation(_changeFromWidth, _changeToWidth, _changeWidthTime, _changeWidthDuration);
            if ( _changeWidthTime >= _changeWidthDuration )
            {
                _isChangingWidth = false;
            }
            _isDirty = true;
        }
    }

    /// <summary>
    /// update-高度变更
    /// </summary>
    protected void ChangingHeight()
    {
        if (_changeHeightDelay > 0)
        {
            _changeHeightDelay--;
            if (_changeHeightDelay == 0)
            {
                _changeFromHeight = _laserHalfHeight;
            }
        }
        else
        {
            _changeHeightTime++;
            _laserHalfHeight = MathUtil.GetLinearInterpolation(_changeFromHeight, _changeToHeight, _changeHeightTime, _changeHeightDuration);
            if (_changeHeightTime >= _changeHeightDuration)
            {
                _isChangingHeight = false;
            }
            _isDirty = true;
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
        _curRotation += _laserRotateOmega;
        _rotateCounter++;
        if ( _rotateCounter >= _rotateDuration )
        {
            _curRotation = _rotateToAngle;
            while (_curRotation < 0 )
            {
                _curRotation += 360;
            }
            _curRotation = _curRotation % 360;
            _isRotating = false;
        }
        _isRotationDirty = true;
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
        _laser.size = new Vector2(_laserHalfWidth * 2, _laserHalfHeight * 2);
        Vector3 pos = Vector3.zero;
        pos.x = _laserHalfWidth;
        _laserTrans.localPosition = pos;
        _isDirty = false;
    }

    protected void UpdateExistTime()
    {
        if (_existDuration == -1) return;
        _existTime++;
        if ( _existTime >= _existDuration )
        {
            _clearFlag = 1;
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
        paras.halfHeight = _laserHalfHeight;
        paras.angle = _curRotation;
        // 计算矩形中心坐标
        Vector2 center = new Vector2();
        float cos = Mathf.Cos(_curRotation * Mathf.Deg2Rad);
        float sin = Mathf.Sin(_curRotation * Mathf.Deg2Rad);
        // 矩形中心坐标
        center.x = _laserHalfHeight * cos + _curPos.x;
        center.y = _laserHalfHeight * sin + _curPos.y;
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
        center.x = _laserHalfHeight * cos + _curPos.x;
        center.y = _laserHalfHeight * sin + _curPos.y;
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
            if (Mathf.Abs(tmpVec.x) < _laserHalfHeight && Mathf.Abs(tmpVec.y) < _laserHalfWidth)
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
            if (rate <= 0 || (Mathf.Abs(relativeVec.x) < _laserHalfHeight && Mathf.Abs(relativeVec.y) < _laserHalfWidth * _collisionFactor))
            {
                Eliminate(eEliminateDef.HitPlayer);
                PlayerService.GetInstance().GetCharacter().BeingHit();
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

    public override void Clear()
    {
        if ( _laserObj != null )
        {
            _laser.size = Vector2.zero;
            ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _laserObj);
        }
        _laserObj = null;
        _objTrans = null;
        _laser = null;
        _laserTrans = null;
        base.Clear();
    }
}
