using UnityEngine;
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

    protected int _clearFlag;

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
            // 检测玩家是否已经到达ItemGetLine
            if (Global.PlayerPos.y >= Consts.AutoGetItemY)
            {
                _curState = StateToPlayer;
                _curTime = 0;
                _curDuration = 60;
            }
            // 检测玩家是否已经到了物品附近
            else if ( Mathf.Abs(Global.PlayerPos.x-_curPos.x) <= NearbyDis && Mathf.Abs(Global.PlayerPos.y - _curPos.y) <= NearbyDis)
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
            _curPos.x = MathUtil.GetEaseInQuadInterpolation(_curPos.x, Global.PlayerPos.x, _curTime, _curDuration);
            _curPos.y = MathUtil.GetEaseInQuadInterpolation(_curPos.y, Global.PlayerPos.y, _curTime, _curDuration);
            _curTime++;
        }
        UpdatePos();
        DetectCollisionWithPlayer();
    }

    protected void UpdatePos()
    {
        _itemGO.transform.localPosition = _curPos;
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
        float playerX = Global.PlayerPos.x;
        float playerY = Global.PlayerPos.y;
        // 矩形碰撞检测
        if ( Mathf.Abs(playerX-_curPos.x) <= PlayerCollHalfWidth + _collisionHalfWidth &&
            Mathf.Abs(playerY - _curPos.y) <= PlayerCollHalfHeight + _collisionHalfHeight )
        {
            DoEffect();
            _clearFlag = 1;
        }
    }

    protected bool IsOutOfBorder()
    {
        return _curPos.y <= Consts.ItemBottomBorderY;
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
        
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(_itemGO);
        _itemGO = null;
    }
}
