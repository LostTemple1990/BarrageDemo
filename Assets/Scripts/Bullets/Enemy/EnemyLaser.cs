using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaser : EnemyBulletBase
{
    /// <summary>
    /// 擦弹冷却时间
    /// </summary>
    private const int GrazeCoolDown = 2;

    protected float _laserAngle;
    protected float _laserRotateOmiga;
    protected int _rotateCounter;
    protected int _rotateDuration;
    protected float _rotateToAngle;
    protected bool _isRotating;

    protected bool _rotateFlag;
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
    protected bool _isSized;

    protected GameObject _laserObj;
    protected Transform _objTrans;
    protected SpriteRenderer _laser;
    protected Transform _laserTrans;

    protected int _existTime;
    protected int _existDuration;

    /// <summary>
    /// 表示当前正在改变宽度
    /// </summary>
    protected bool _isChangingWidth;
    protected int _changeDuration;
    protected int _changeTime;
    protected int _changeWidthDelay;
    protected float _changeFromWidth;
    protected float _changeToWidth;

    /// <summary>
    /// 是否需要Resize激光
    /// </summary>
    protected bool _isDirty;

    public EnemyLaser()
    {
        _prefabName = "Laser";
        _sysBusyWeight = 3;
    }

    public override void Init()
    {
        base.Init();
        _isSized = false;
        _rotateFlag = false;
        _moveFlag = false;
        _curPos = Vector3.zero;
        _existDuration = -1;
        _id = BulletId.BulletId_Enemy_Laser;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        _isChangingWidth = false;
        _isDirty = false;
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
        _laser.sprite = ResourceManager.GetInstance().GetResource<Sprite>("etama",texture);
        if ( _isSized )
        {
            Resize();
        }
    }

    public override void SetToPosition(Vector3 vec)
    {
        _curPos.x = vec.x;
        _curPos.y = vec.y;
        _moveFlag = true;
        if ( vec.z < 360 )
        {
            _laserAngle = vec.z;
            _rotateFlag = true;
        }
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
        _changeToWidth = toWidth;
        _changeDuration = duration;
        _changeTime = 0;
        _isChangingWidth = true;
    }

    public virtual void SetPosition(float posX,float posY,float angle)
    {
        _curPos.x = posX;
        _curPos.y = posY;
        _laserAngle = angle;
        _moveFlag = true;
        _rotateFlag = true;
    }

    public override void SetToPosition(float posX, float posY)
    {
        base.SetToPosition(posX, posY);
        _moveFlag = true;
    }

    public virtual void SetLaserAngle(float angle)
    {
        _laserAngle = angle;
        _rotateFlag = true;
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
            _isSized = true;
        }
        if ( height != Consts.OriginalHeight )
        {
            _laserHeight = height;
            _laserHalfHeight = height / 2;
            _isSized = true;
        }
        if (_laser != null && _isSized)
        {
            Resize();
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
        _laserRotateOmiga = (toAngle - _laserAngle) / duration;
        _isRotating = true;
    }

    public override void Update()
    {
        UpdateComponents();
        if ( _isChangingWidth )
        {
            ChangingWidth();
        }
        if ( _isMoving )
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
        }
        else
        {
            _changeTime++;
            _laserHalfWidth = MathUtil.GetLinearInterpolation(_changeFromWidth, _changeToWidth, _changeTime, _changeDuration);
            if ( _changeTime >= _changeDuration )
            {
                _isChangingWidth = false;
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
        _laserAngle += _laserRotateOmiga;
        _rotateCounter++;
        if ( _rotateCounter >= _rotateDuration )
        {
            _laserAngle = _rotateToAngle;
            while ( _laserAngle < 0 )
            {
                _laserAngle += 360;
            }
            _laserAngle = _laserAngle % 360;
            _isRotating = false;
        }
        _rotateFlag = true;
    }

    protected virtual void UpdatePosition()
    {
        if ( _rotateFlag )
        {
            _rotateFlag = false;
            _objTrans.localRotation = Quaternion.Euler(0, 0, _laserAngle - 90);
        }
        if ( _moveFlag )
        {
            _moveFlag = false;
            _objTrans.localPosition = _curPos;
        }
    }

    protected virtual void Resize()
    {
        _laser.size = new Vector2(_laserHalfWidth * 2, _laserHalfHeight * 2);
        Vector3 pos = Vector3.zero;
        pos.y = _laserHalfHeight;
        _laserTrans.localPosition = pos;
        _isDirty = false;
    }

    protected void UpdateExistTime()
    {
        _existTime++;
        if ( _existDuration != -1 && _existTime >= _existDuration )
        {
            _clearFlag = 1;
        }
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
            type = CollisionDetectType.Rect,
        };
        if ( !_detectCollision )
        {
            paras.halfHeight = paras.halfHeight = paras.angle = 0;
            paras.centerPos = Vector2.zero;
        }
        else
        {
            paras.halfWidth = _laserHalfWidth;
            paras.halfHeight = _laserHalfHeight;
            paras.angle = _laserAngle;
            // 计算矩形中心坐标
            Vector2 center = new Vector2();
            float cos = Mathf.Cos(_laserAngle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(_laserAngle * Mathf.Deg2Rad);
            // 矩形中心坐标
            center.x = _collisionHalfHeight * cos + _curPos.x;
            center.y = _collisionHalfHeight * sin + _curPos.y;
            paras.centerPos = center;
        }
        return paras;
    }

    protected virtual void CheckCollisionWithCharacter()
    {
        Vector2 center = new Vector2();
        float cos = Mathf.Cos(_laserAngle * Mathf.Deg2Rad);
        float sin = Mathf.Sin(_laserAngle * Mathf.Deg2Rad);
        // 矩形中心坐标
        center.x = _collisionHalfHeight * cos + _curPos.x;
        center.y = _collisionHalfHeight * sin + _curPos.y;
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
            if (Mathf.Abs(tmpVec.x) < _collisionHalfHeight && Mathf.Abs(tmpVec.y) < _collisionHalfWidth)
            {
                _isGrazed = true;
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
            if (rate <= 0 || (Mathf.Abs(relativeVec.x) < _collisionHalfHeight && Mathf.Abs(relativeVec.y) < _collisionHalfWidth))
            {
                Eliminate(eEliminateDef.HitPlayer);
                PlayerService.GetInstance().GetCharacter().BeingHit();
            }
        }
    }
    #endregion

    public override void Clear()
    {
        UIManager.GetInstance().HideGo(_laserObj);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _laserObj);
        _laserObj = null;
        _objTrans = null;
        _laser.sprite = null;
        _laser = null;
        _laserTrans = null;
        base.Clear();
    }
}
