﻿using UnityEngine;
using System.Collections;

public class ObjectColliderBase
{
    protected float _curPosX;
    protected float _curPosY;
    protected Vector2 _curPos;

    protected int _colliderGroups;

    protected int _existTime;
    protected int _existDuration;

    /// <summary>
    /// 是否清除
    /// </summary>
    protected int _clearFlag;
    /// <summary>
    /// 是否正在缩放
    /// </summary>
    protected bool _isScaling;
    protected int _scaleTime;
    protected int _scaleDuration;

    protected eColliderType _type;

    public ObjectColliderBase()
    {
        _clearFlag = 0;
        _existDuration = 0;
        _isScaling = false;
    }

    public void SetToPositon(float posX,float posY)
    {
        _curPosX = posX;
        _curPosY = posY;
        _curPos = new Vector2(posX, posY);
    }

    public virtual void SetSize(float arg0,float arg1)
    {

    }

    public void SetColliderGroup(int groups)
    {
        _colliderGroups = groups;
    }

    /// <summary>
    /// 缩放至指定的尺寸
    /// </summary>
    /// <param name="toArg0"></param>
    /// <param name="toArg1"></param>
    /// <param name="duration"></param>
    public virtual void ScaleToSize(float toArg0,float toArg1,int duration)
    {
        _isScaling = true;
        _scaleTime = 0;
        _scaleDuration = duration;
    }

    /// <summary>
    /// 设置存在时间
    /// </summary>
    /// <param name="existDuration"></param>
    public void SetExistDuration(int existDuration)
    {
        _existTime = 0;
        _existDuration = existDuration;
    }

    public void Update()
    {
        if (_clearFlag == 1) return;
        if ( _isScaling )
        {
            Scale();
        }
        if ( (_colliderGroups & (int)eColliderGroup.Player) == 1 )
        {
            CheckCollisionWithPlayer();
        }
        if ((_colliderGroups & (int)eColliderGroup.PlayerBullet) == 1)
        {
            CheckCollisionWithPlayerBullet();
        }
        if ((_colliderGroups & (int)eColliderGroup.Enemy) == 1)
        {
            CheckCollisionWithEnemy();
        }
        if ((_colliderGroups & (int)eColliderGroup.EnemyBullet) == 1)
        {
            CheckCollisionWithEnemyBullet();
        }
        if ((_colliderGroups & (int)eColliderGroup.Item) == 1)
        {
            CheckCollisionWithItem();
        }
        if ( _existDuration > 0 )
        {
            _existTime++;
            if ( _existTime >= _existDuration )
            {
                _clearFlag = 1;
            }
        }
    }

    protected virtual void CheckCollisionWithPlayer()
    {

    }

    protected virtual void CheckCollisionWithPlayerBullet()
    {

    }

    protected virtual void CheckCollisionWithEnemy()
    {

    }

    protected virtual void CheckCollisionWithEnemyBullet()
    {

    }

    protected virtual void CheckCollisionWithItem()
    {

    }

    /// <summary>
    /// 执行缩放
    /// </summary>
    protected virtual void Scale()
    {

    }

    public void ClearSelf()
    {
        _clearFlag = 1;
    }

    public int ClearFlag
    {
        get { return _clearFlag; }
    }


    public void Clear()
    {

    }
}