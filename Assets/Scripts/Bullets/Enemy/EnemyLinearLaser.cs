﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLinearLaser : EnemyBulletBase
{
    private const float DefaultLaserHalfHeight = 6f;
    private const float DefaultCollisionHalfHeight = 3f;

    private const string HeadSpriteNameWhite = "LaserHead_W";
    private const string HeadSpriteNameBlue = "LaserHead_B";
    private const string HeadSpriteNameRed = "LaserHead_R";
    private const string HeadSpriteNameGreen = "LaserHead_G";

    private const int HeadAniInterval = 6;
    private const int HeadAniTotalFrame = 4;

    private static string GetHeadSpriteNameByType(eLaserHeadType type)
    {
        switch ( type )
        {
            case eLaserHeadType.Blue:
                return HeadSpriteNameBlue;
            case eLaserHeadType.Green:
                return HeadSpriteNameGreen;
            case eLaserHeadType.White:
                return HeadSpriteNameWhite;
            case eLaserHeadType.Red:
                return HeadSpriteNameRed;
        }
        return "";
    }

    protected MovableObject _movableObj;
    protected List<Vector2> _pathList;
    protected int _pathCount;
    protected int _laserLen;
    protected int _existDuration;
    protected int _pathBeginIndex;
    protected int _pathEndIndex;

    protected int _accDuration;
    protected string _textureName;

    protected float _grazeHalfWidth;
    protected float _grazeHalfHeight;
    protected float _collisionHalfWidth;
    protected float _collisionHalfHeight;

    protected float _laserHalfHeight;
    protected float _laserHalfWidth;
    protected bool _isSized;

    protected GameObject _laserObj;
    protected Transform _objTrans;
    protected SpriteRenderer _laser;
    protected Transform _laserTrans;
    protected Transform _headTf;
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
    protected eLaserHeadType _headType;
    protected int _headAniTime;
    protected int _headAniIndex;


    public EnemyLinearLaser()
    {
        _pathList = new List<Vector2>();
        _movableObj = new MovableObject();
    }

    public override void Init()
    {
        base.Init();
        _isSized = false;
        _curPos = Vector3.zero;
        _existDuration = -1;
        _id = BulletId.BulletId_Enemy_LinearLaser;
        BulletsManager.GetInstance().RegisterEnemyBullet(this);
        _isDirty = false;
        _pathCount = 0;
        _pathBeginIndex = 0;
        _pathEndIndex = -1;
        _clearFlag = 0;
        _isHeadEnable = false;
    }

    public override void SetBulletTexture(string texture)
    {
        if (_laserObj == null)
        {
            _laserObj = ResourceManager.GetInstance().GetPrefab("BulletPrefab", "Laser");
            _objTrans = _laserObj.transform;
            _laser = _objTrans.Find("LaserSprite").GetComponent<SpriteRenderer>();
            _laserTrans = _laser.transform;
            _headTf = _objTrans.Find("HeadSprite");
            _headSp = _headTf.GetComponent<SpriteRenderer>();
            UIManager.GetInstance().AddGoToLayer(_laserObj, LayerId.EnemyBarrage);
        }
        _textureName = texture;
        _laser.sprite = ResourceManager.GetInstance().GetResource<Sprite>("etama9", texture);
        _laser.size = Vector2.zero;
    }

    public virtual void SetLength(int length)
    {
        _laserLen = length;
        _isDirty = true;
    }

    public int GetLength()
    {
        return _laserLen;
    }

    public virtual void DoStraightMove(float velocity,float angle,float acce,int accDuration)
    {
        _curVelocity = velocity;
        _curAngle = angle;
        _curAcceleration = acce;
        _accDuration = accDuration;
        _movableObj.DoMoveStraight(velocity, angle);
        _movableObj.DoAccelerationWithLimitation(acce, Consts.VelocityAngle, accDuration);
        _objTrans.localRotation = Quaternion.Euler(0, 0, angle);
        _isMoving = true;
    }

    public virtual void DoAccelerationWithLimitation(float acce,float angle,int accDuration)
    {
        _curAcceleration = acce;
        _movableObj.DoAccelerationWithLimitation(acce, angle, accDuration);
        _isMoving = true;
    }

    public override void Update()
    {
        UpdateComponents();
        _movableObj.Update();
        _curPos = _movableObj.GetPos();
        UpdatePath();
        if (_isDirty)
        {
            Resize();
        }
        UpdatePos();
        if ( _isHeadEnable )
        {
            UpdateHeadAni();
        }
        CheckCollisionWithCharacter();
        //UpdateExistTime();
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
    }

    protected virtual void UpdatePath()
    {
        _pathList.Add(_curPos);
        _pathCount++;
        if ( _pathCount > _laserLen )
        {
            _pathCount--;
            _pathBeginIndex++;
            _pathEndIndex++;
            // 加速度不为0，说明长度改变了，重绘激光图像
            if ( _curAcceleration != 0 )
            {
                _isDirty = true;
            }
            // 每隔60帧清除一次path
            if ( _pathBeginIndex >= 60 )
            {
                _pathBeginIndex -= 60;
                _pathEndIndex -= 60;
                _pathList.RemoveRange(0, 60);
            }
        }
        else
        {
            _pathEndIndex++;
            _isDirty = true;
        }
    }

    public override void SetToPosition(float posX, float posY)
    {
        base.SetToPosition(posX, posY);
        _movableObj.Reset(posX, posY);
    }

    public virtual void SetHeadEnable(bool isEnable,eLaserHeadType headType)
    {
        _isHeadEnable = isEnable;
        if ( isEnable )
        {
            _headTf.gameObject.SetActive(true);
            // 初始化激光头部动画的相关参数
            _headAniTime = 0;
            _headAniIndex = 0;
            _headType = headType;
            _headSp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, GetHeadSpriteNameByType(_headType) + _headAniIndex);
        }
        else
        {
            _headTf.gameObject.SetActive(false);
        }
    }

    protected virtual void UpdatePos()
    {
        _objTrans.localPosition = _curPos;
    }

    /// <summary>
    /// 重新渲染激光
    /// </summary>
    protected virtual void Resize()
    {
        float width = (_pathList[_pathEndIndex] - _pathList[_pathBeginIndex]).magnitude;
        _laser.size = new Vector2(width, DefaultLaserHalfHeight * 2);
        Vector3 pos = Vector3.zero;
        pos.x = -width / 2;
        _laserTrans.localPosition = pos;
        _isDirty = false;
    }

    /// <summary>
    /// 更新激光头部的动画
    /// </summary>
    protected virtual void UpdateHeadAni()
    {
        _headAniTime++;
        int index = (_headAniTime / HeadAniInterval) % HeadAniTotalFrame;
        if ( index != _headAniIndex )
        {
            _headAniIndex = index;
            _headSp.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGBulletsAtlasName, GetHeadSpriteNameByType(_headType) + _headAniIndex);
        }
    }

    protected override bool IsOutOfBorder()
    {
        if (_pathList[_pathBeginIndex].x < Global.BulletLBBorderPos.x ||
            _pathList[_pathBeginIndex].y < Global.BulletLBBorderPos.y ||
            _pathList[_pathBeginIndex].x > Global.BulletRTBorderPos.x ||
            _pathList[_pathBeginIndex].y > Global.BulletRTBorderPos.y)
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
            type = CollisionDetectType.Line,
            linePointA = _pathList[_pathBeginIndex],
            linePointB = _pathList[_pathEndIndex],
            radius = DefaultCollisionHalfHeight,
        };
        return paras;
    }

    protected virtual void CheckCollisionWithCharacter()
    {
        // todo 直线激光的擦弹
        // 直线碰撞检测
        float minDis = MathUtil.GetMinDisFromPointToLineSegment(_pathList[_pathBeginIndex], _pathList[_pathEndIndex], Global.PlayerPos);
        if ( minDis < DefaultLaserHalfHeight + Global.PlayerCollisionVec.z )
        {
            PlayerService.GetInstance().GetCharacter().BeingHit();
            _clearFlag = 1;
        }
    }
    #endregion

    public string GetTextureName()
    {
        return _textureName;
    }

    public float GetVelocity()
    {
        return _curVelocity;
    }

    public float GetAngle()
    {
        return _curAngle;
    }

    public float GetAcceleration()
    {
        return _curAcceleration;
    }

    public int GetAccDuration()
    {
        return _accDuration;
    }

    public override void Clear()
    {
        base.Clear();
        UIManager.GetInstance().HideGo(_laserObj);
        _laser.sprite = null;
        _headSp.sprite = null;
        _pathList.Clear();
    }
}
