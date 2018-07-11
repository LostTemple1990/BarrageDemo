using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletSimple : EnemyBulletMovable
{
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

    protected float _collisionRadius;

    protected GrazeDetectParas _grazeParas;
    protected CollisionDetectParas _collisionParas;
    #endregion

    protected int _orderInLayer;

    public override void Init()
    {
        base.Init();
        _id = BulletId.BulletId_Enemy_Simple;
        _orderInLayer = 0;
    }

    public override void Update()
    {
        _lastPos = _curPos;
        base.Update();
        CheckRotateImg();
        CheckCollisionWithCharacter();
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
        else
        {
            UpdatePos();
        }
    }

    public override void SetBulletTexture(string texture)
    {
        _prefabName = texture;
        _bullet = ObjectsPool.GetInstance().CreateBulletPrefab(_prefabName);
        _trans = _bullet.transform;
    }

    public virtual void SetSelfRotation(bool isSelfRotation, float angle)
    {
        _isSelfRotation = isSelfRotation;
        if (_isSelfRotation)
        {
            _selfRotationAngle = new Vector3(0, 0, angle);
        }
    }

    public virtual void SetRotatedByVelocity(bool value)
    {
        _isRotatedByVelocity = value;
        _imgRotatedFlag = 1;
    }

    public virtual void ChangeStyleById(string id)
    {
        // 根据传入的id读取配置，重新生成数据
        EnemyBulletDefaultCfg cfg = BulletsManager.GetInstance().GetBulletDefaultCfgById(id);
        if ( cfg == null )
        {
            Logger.Log("Bullet ChangeStyle Fail! SysId  " + id + " is not exist!");
            return;
        }
        // 回收现有的prefab
        SetOrderInLayer(0);
        UIManager.GetInstance().RemoveGoFromLayer(_bullet);
        ObjectsPool.GetInstance().RestoreBullet(_prefabName, _bullet);
        SetBulletTexture(cfg.prefabName);
        SetToPosition(_curPos.x, _curPos.y);
        SetRotatedByVelocity(cfg.isRotatedByVAngle);
        SetSelfRotation(cfg.selfRotationAngle != 0, cfg.selfRotationAngle);
        CollisionDetectParas detectParas = new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            radius = cfg.collisionRadius,
        };
        GrazeDetectParas grazeParas = new GrazeDetectParas
        {
            type = GrazeDetectType.Rect,
            halfWidth = cfg.grazeHalfWidth,
            halfHeight = cfg.grazeHalfHeight,
        };
        SetCollisionDetectParas(detectParas);
        SetGrazeDetectParas(grazeParas);
    }

    public virtual void SetOrderInLayer(int order)
    {
        if ( order != _orderInLayer )
        {
            _orderInLayer = order;
            SpriteRenderer sp = _trans.Find("BulletSprite").GetComponent<SpriteRenderer>();
            sp.sortingOrder = _orderInLayer;
        }
    }

    public virtual void RotateImgByVelocity()
    {
        Vector3 dv = _curPos - _lastPos;
        float rotateAngle = MathUtil.GetAngleBetweenXAxis(dv.x, dv.y, false) - 90;
        _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
    }

    protected void CheckRotateImg()
    {
        if (_imgRotatedFlag == 1)
        {
            RotateImgByVelocity();
        }
        if ( _isRotatedByVelocity )
        {
            if ( _isMovingStraight )
            {
                if (_curAcceleration == 0 || _curAngle == _curAccAngle)
                {
                    _imgRotatedFlag = 0;
                    return;
                }
            }
            _imgRotatedFlag = 1;
            //if ( _isMovingCurve )
            //{
            //    _imgRotatedFlag = 1;
            //}
            //else if (_curAcceleration == 0 || _curAngle == _curAccAngle)
            //{
            //    _imgRotatedFlag = 0;
            //}
            //else
            //{
            //    _imgRotatedFlag = 1;
            //}
        }
    }

    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
        _grazeParas = paras;
    }

    public override void SetCollisionDetectParas(CollisionDetectParas paras)
    {
        _collisionRadius = paras.radius;
        _collisionParas = paras;
    }

    public override CollisionDetectParas GetCollisionDetectParas()
    {
        CollisionDetectParas paras = new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            centerPos = _curPos,
            radius = _collisionRadius,
        };
        return paras;
    }

    protected virtual void CheckCollisionWithCharacter()
    {
        float playerX = Global.PlayerPos.x;
        float playerY = Global.PlayerPos.y;
        // 首先检测是否在擦弹范围
        if (Mathf.Abs(_curPos.x - playerX) <= _grazeHalfWidth + Global.PlayerCollisionVec.x &&
            Mathf.Abs(_curPos.y - playerY) <= _grazeHalfHeight + Global.PlayerCollisionVec.y)
        {
            // TODO 擦弹数+1
            // 在擦弹范围内，进行实际的碰撞检测
            float len = Mathf.Sqrt((_curPos.x - playerX) * (_curPos.x - playerX) + (_curPos.y - playerY) * (_curPos.y - playerY));
            if (len <= Global.PlayerCollisionVec.z + _collisionRadius)
            {
                _clearFlag = 1;
                PlayerService.GetInstance().GetCharacter().BeingHit();
            }
        }
    }

    public override void Clear()
    {
        SetOrderInLayer(0);
        base.Clear();
    }
}
