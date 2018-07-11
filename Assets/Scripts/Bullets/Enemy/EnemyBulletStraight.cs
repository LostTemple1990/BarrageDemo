using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBulletStraight : EnemyBulletBase
{
    protected GameObject _bullet;
    protected Transform _trans;
    protected Image _bulletImg;

    protected float _curAccAngle;
    protected float _dvx, _dvy;

    /// <summary>
    /// 自身旋转
    /// </summary>
    protected bool _isSelfRotation;
    protected Vector3 _selfRotationAngle;

    protected bool _isRotatedByVelocity;
    protected int _imgRotatedFlag;

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
    protected int _collisionType;
    /// <summary>
    /// 碰撞检测参数0，圆的半径/宽的一半
    /// </summary>
    protected float _collisionArg0;
    /// <summary>
    /// 碰撞检测参数1，圆的半径/高的一半
    /// </summary>
    protected float _collisionArg1;
    #endregion


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
        _imgRotatedFlag = 0;
        _isSelfRotation = false;
        _isRotatedByVelocity = false;
        _id = BulletId.BulletId_Enemy_Straight;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
    }

    public override void SetBulletTexture(string texture)
    {
        //_bulletImg.texture = ResourceManager.GetInstance().GetTexture("etama",texture);

        //TimeUtil.BeginSample(100);
        //_bulletImg.sprite = ResourceManager.GetInstance().GetResource<Sprite>("etama", texture);
        //TimeUtil.EndSample();
        //_bulletImg.SetNativeSize();
        _prefabName = texture;
        _bullet = ObjectsPool.GetInstance().CreateBulletPrefab(_prefabName);
        _trans = _bullet.transform;
    }

    public override void Update()
    {
        if (_isMoving)
        {
            Move();
        }
        CheckRotateImg();
        UpdatePos();
        CheckCollisionWithCharacter();
    }

    #region 设置碰撞检测相关参数
    public virtual void SetGrazeParams(float halfWidth,float halfHeight)
    {
        _grazeHalfWidth = halfWidth;
        _grazeHalfHeight = halfHeight;
    }

    public virtual void SetCollisionParams(int type,float arg0,float arg1)
    {
        _collisionType = type;
        _collisionArg0 = arg0;
        _collisionArg1 = arg1;
    }
    #endregion

    protected void CheckRotateImg()
    {
        if (_imgRotatedFlag == 1)
        {
            RotateImgByVelocity();
        }
        if ( _curAcceleration == 0 || _curAngle == _curAccAngle )
        {
            _imgRotatedFlag = 0;
        }
        else if ( _isRotatedByVelocity )
        {
            _imgRotatedFlag = 1;
        }
    }

    public virtual void DoMove(float v, float angle, float acce,float accAngle)
    {
        _curVelocity = v / Consts.TargetFrameRate;
        _curAngle = angle;
        _curAcceleration = acce / Consts.TargetFrameRate;
        _curAccAngle = accAngle;
        _vx = _curVelocity * Mathf.Cos(angle * Mathf.Deg2Rad);
        _vy = _curVelocity * Mathf.Sin(angle * Mathf.Deg2Rad);
        if ( acce != 0 )
        {
            _dvx = _curAcceleration * Mathf.Cos(accAngle * Mathf.Deg2Rad);
            _dvy = _curAcceleration * Mathf.Sin(accAngle * Mathf.Deg2Rad);
            _vx -= _dvx;
            _vy -= _dvy;
            // 加速度方向和速度方向不一致时，设置子弹图像旋转flag
            if ( _curAccAngle != _curAngle && _isRotatedByVelocity )
            {
                _imgRotatedFlag = 1;
            }
        }
        _isMoving = true;
    }

    public virtual void SetSelfRotation(bool isSelfRotation,float angle)
    {
        _isSelfRotation = isSelfRotation;
        if ( _isSelfRotation )
        {
            _selfRotationAngle = new Vector3(0,0,angle / Consts.TargetFrameRate);
        }
    }

    public virtual void SetRotatedByVelocity(bool value)
    {
        _isRotatedByVelocity = value;
        _imgRotatedFlag = 1;
    }

    public virtual void RotateImgByVelocity()
    {
        float rotateAngle = MathUtil.GetAngleBetweenXAxis(_vx, _vy, false) - 90;
        _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
    }

    protected override void Move()
    {
        // 更新速度
        _vx += _dvx;
        _vy += _dvy;
        // 更新位置
        _curPos.x += _vx;
        _curPos.y += _vy;
        if (IsOutOfBorder())
        {
            _clearFlag = 1;
            return;
        }
        if ( _isSelfRotation )
        {
            _trans.Rotate(_selfRotationAngle);
        }
    }

    protected virtual void UpdatePos()
    {
        _trans.localPosition = _curPos;
    }

    protected virtual void CheckCollisionWithCharacter()
    {
        // 首先检测是否在擦弹范围
        if ( Mathf.Abs(_curPos.x-Global.PlayerPos.x) <= _grazeHalfWidth + Global.PlayerCollisionVec.x &&
            Mathf.Abs(_curPos.y - Global.PlayerPos.y) <= _grazeHalfHeight + Global.PlayerCollisionVec.y )
        {
            // TODO 擦弹数+1
            // 在擦弹范围内，进行实际的碰撞检测
            float playerX = Global.PlayerPos.x;
            float playerY = Global.PlayerPos.y;
            if ( _collisionType == Consts.CollisionType_Rect )
            {
                float len = Mathf.Sqrt((_curPos.x - playerX) * (_curPos.x - playerX) + (_curPos.y - playerY) * (_curPos.y - playerY));
                Vector2 vec = new Vector2(playerX - _curPos.x, playerY - _curPos.y);
                float rate = (len - Global.PlayerCollisionVec.z) / len;
                if ( rate > 0 )
                {
                    vec *= rate;
                    if (Mathf.Abs(vec.x) > _collisionArg0 || Mathf.Abs(vec.y) > _collisionArg1)
                    {
                        return;
                    }
                }
                _clearFlag = 1;
                Logger.Log("Hit Player!");
            }
            else if ( _collisionType == Consts.CollisionType_Circle )
            {
                float len = Mathf.Sqrt((_curPos.x - playerX) * (_curPos.x - playerX) + (_curPos.y - playerY) * (_curPos.y - playerY));
                if ( len <= Global.PlayerCollisionVec.z + _collisionArg0 )
                {
                    _clearFlag = 1;
                    //Logger.Log("Hit Player!");
                }
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        UIManager.GetInstance().RemoveGoFromLayer(_bullet);
        ObjectsPool.GetInstance().RestoreBullet(_prefabName,_bullet);
        _bullet = null;
    }
}
