using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBulletCurve : EnemyBulletBase
{
    protected GameObject _bullet;
    protected Transform _trans;
    protected Image _bulletImg;

    protected bool _isRotatedByV;
    protected bool _velocityParamIsDirty;

    protected float _dvx, _dvy;

    protected Vector2 _center;
    protected float _curRadius;
    protected float _omiga;
    protected float _deltaR;

    protected int _flag = 0;
    protected int _flagCount = 0;


    public override void Init()
    {
        base.Init();
        _isMoving = false;
        _id = BulletId.BulletId_Enemy_Curve;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
    }

    public override void SetBulletTexture(string texture)
    {
        _prefabName = texture;
        _bullet = ObjectsPool.GetInstance().CreateBulletPrefab(_prefabName);
        _trans = _bullet.transform;
    }

    public override void Update()
    {
        if ( _isMoving )
        {
            //UpdateVelocityParams();
            Move();
        }
        UpdatePos();
        RotateImgByVelocity();
    }

    public void DoMove(Vector2 center,float radius,float angle,float deltaR,float omiga)
    {
        _center = center;
        _curRadius = radius;
        _curAngle = angle;
        _deltaR = deltaR / Consts.TargetFrameRate;
        _omiga = omiga / Consts.TargetFrameRate;
        _isMoving = true;
        _flag = _flagCount = 0;
        _oriRadius = _curRadius;
    }

    protected float _oriRadius;

    protected override void Move()
    {
        _dx = _curPos.x;
        _dy = _curPos.y;
        _curRadius += _deltaR;
        //if ( _flagCount < 220 )
        //{
        //    _curRadius = _oriRadius * Mathf.Sin(_flagCount * 90f / 220 * Mathf.Deg2Rad);
        //}
        //else if ( _flagCount >= 220 )
        //{
        //    _curRadius = _oriRadius * Mathf.Sin((90f+((_flagCount-220) * 90f / 120)) * Mathf.Deg2Rad);
        //    if ( _flagCount >= 340 )
        //    {
        //        _flagCount = 0;
        //    }
        //}
        _curAngle += _omiga;
        _curPos.x = _curRadius * Mathf.Cos(_curAngle * Mathf.Deg2Rad) + _center.x;
        _curPos.y = _curRadius * Mathf.Sin(_curAngle * Mathf.Deg2Rad) + _center.y;
        _dx = _curPos.x - _dx;
        _dy = _curPos.y - _dy;
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
    }

    public virtual void RotateImgByVelocity()
    {
        float rotateAngle = MathUtil.GetAngleBetweenXAxis(_dx, _dy, false) - 90;
        _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
    }

    protected virtual void UpdatePos()
    {
        _trans.localPosition = _curPos;
    }

    protected void UpdateVelocityParams()
    {
        if ( _curAngleVelocity != 0 )
        {
            _curAngle += _curAngleVelocity;
            _velocityParamIsDirty = true;
        }
        if (_velocityParamIsDirty)
        {
            if ( _curAcceleration != 0 )
            {

            }
            _vx += _curAcceleration * Mathf.Cos(_curAngle * Mathf.Deg2Rad);
            _vy += _curAcceleration * Mathf.Sin(_curAngle * Mathf.Deg2Rad);
            _velocityParamIsDirty = false;
        }
    }

    public override void Clear()
    {
        base.Clear();
        UIManager.GetInstance().RemoveGoFromLayer(_bullet);
        ObjectsPool.GetInstance().RestoreBullet(_prefabName, _bullet);
        _bullet = null;
    }
}
