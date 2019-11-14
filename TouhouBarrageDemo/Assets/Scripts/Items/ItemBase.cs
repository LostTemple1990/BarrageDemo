﻿using UnityEngine;
using System.Collections;

public class ItemBase
{
    protected ItemType _itemType;

    protected int _curState;

    /// <summary>
    /// 上升阶段
    /// </summary>
    protected const int StateUp = 1;
    /// <summary>
    /// 下降阶段
    /// </summary>
    protected const int StateDown = 2;
    /// <summary>
    /// 飞向玩家的阶段
    /// </summary>
    protected const int StateToPlayer = 3;

    protected const float UpVy = 0.6f;
    protected const float DownVy = -1;

    protected const float PlayerCollHalfWidth = 12;
    protected const float PlayerCollHalfHeight = 12;
    protected const float NearbyDis = 24;

    protected float _vy;
    protected float _vx;
    protected int _curTime;
    protected int _curDuration;

    protected GameObject _itemGO;
    protected Vector3 _curPos;

    protected float _collisionHalfWidth;
    protected float _collisionHalfHeight;
    /// <summary>
    /// 图像半宽
    /// </summary>
    protected float _halfWidth;
    /// <summary>
    /// 图像半高
    /// </summary>
    protected float _halfHeight;
    /// <summary>
    /// 在上边界上方
    /// </summary>
    protected bool _isAboveBorder;
    /// <summary>
    /// _curPos.y超过这个Y值时使用up图像
    /// </summary>
    protected float _aboveY;
    /// <summary>
    /// 使用up图像时的位置
    /// </summary>
    protected float _upPosY;
    /// <summary>
    /// 默认的SpriteName
    /// </summary>
    protected string _defaultSp;
    /// <summary>
    /// 超过上边界时使用的SpriteName
    /// </summary>
    protected string _upSp;
    protected SpriteRenderer _sr;

    protected int _clearFlag;

    protected CharacterBase _character;

    public ItemBase()
    {
        _curPos = Vector2.zero;
    }

    public virtual void Init()
    {
        _curState = StateUp;
        _curTime = 0;
        _curDuration = 90;
        _vy = UpVy;
        _clearFlag = 0;
        _character = PlayerInterface.GetInstance().GetCharacter();
        _isAboveBorder = false;
        _aboveY = Consts.ItemTopBorderY;
    }

    public virtual void Update()
    {
        // 上升
        if (_curState == StateUp)
        {
            _curPos.y += _vy;
            _curTime++;
            if (_curTime >= _curDuration)
            {
                _curState = StateDown;
                _vy = DownVy;
                _curTime = 0;
                _curDuration = -1;
            }
            // 检测玩家是否已经到达ItemGetLine
            if (Global.PlayerPos.y >= Consts.AutoGetItemY)
            {
                _curState = StateToPlayer;
                _curTime = 0;
                _curDuration = 60;
            }
        }
        // 下降
        else if (_curState == StateDown)
        {
            _curPos.y += _vy;
            _curTime++;
            Vector2 playerPos = _character.GetPosition();
            // 检测玩家是否已经到达ItemGetLine
            if (playerPos.y >= Consts.AutoGetItemY)
            {
                _curState = StateToPlayer;
                _curTime = 0;
                _curDuration = 60;
            }
            // 检测玩家是否已经到了物品附近
            else if ( Mathf.Abs(playerPos.x-_curPos.x) <= NearbyDis && Mathf.Abs(playerPos.y - _curPos.y) <= NearbyDis)
            {
                _curState = StateToPlayer;
                _curTime = 0;
                _curDuration = 30;
            }
            // 检测是否越界
            if ( IsOutOfBorder() )
            {
                _clearFlag = 1;
            }
        }
        // 飞向玩家
        else if (_curState == StateToPlayer)
        {
            Vector2 playerPos = _character.GetPosition();
            _curPos.x = MathUtil.GetEaseInQuadInterpolation(_curPos.x, playerPos.x, _curTime, _curDuration);
            _curPos.y = MathUtil.GetEaseInQuadInterpolation(_curPos.y, playerPos.y, _curTime, _curDuration);
            _curTime++;
        }
        DetectCollisionWithPlayer();
        Render();
    }

    public void SetToPosition(float x, float y)
    {
        _curPos.x = x;
        _curPos.y = y;
    }

    /// <summary>
    /// 玩家接触到之后的效果
    /// </summary>
    protected virtual void DoEffect()
    {
        throw new System.NotImplementedException();
    }

    protected void DetectCollisionWithPlayer()
    {
        Vector2 playerPos = _character.GetPosition();
        // 矩形碰撞检测
        if ( Mathf.Abs(playerPos.x-_curPos.x) <= PlayerCollHalfWidth + _collisionHalfWidth &&
            Mathf.Abs(playerPos.y - _curPos.y) <= PlayerCollHalfHeight + _collisionHalfHeight )
        {
            DoEffect();
            _clearFlag = 1;
        }
    }

    protected bool IsOutOfBorder()
    {
        return _curPos.y <= Consts.ItemBottomBorderY;
    }

    protected void Render()
    {
        Vector3 pos = _curPos;
        if (_curPos.y >= _aboveY)
        {
            if (!_isAboveBorder)
            {
                _isAboveBorder = true;
                _sr.sprite = ResourceManager.GetInstance().GetSprite("ItemAtlas", _upSp);
            }
            pos.y = _upPosY;
        }
        else
        {
            if (_isAboveBorder)
            {
                _isAboveBorder = false;
                _sr.sprite = ResourceManager.GetInstance().GetSprite("ItemAtlas", _defaultSp);
            }
        }
        _itemGO.transform.localPosition = pos;
    }

    public int clearFlag
    {
        get { return _clearFlag; }
    }

    public ItemType itemType
    {
        get { return _itemType; }
    }

    public virtual void Clear()
    {
        _character = null;
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(_itemGO);
        _itemGO = null;
        _sr = null;
    }
}
