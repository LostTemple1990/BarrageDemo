using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletRebound : EnemyBulletStraight
{
    protected int _reboundPara;
    protected int _curReboundCount;
    protected int _totalReboundCount;

    protected float _left, _bottom, _right,_top;

    public override void Init()
    {
        base.Init();
        _left = Global.GameLBBorderPos.x;
        _right = Global.GameRTBorderPos.x;
        _bottom = Global.GameLBBorderPos.y;
        _top = Global.GameRTBorderPos.y;
        _id = BulletId.BulletId_Enemy_Rebound;
    }

    public virtual void SetReboundParams(int reboundPara,int reboundCount)
    {
        _reboundPara = reboundPara;
        _curReboundCount = reboundCount;
    }

    public override void Update()
    {
        base.Update();
        if ( _curReboundCount > 0 )
        {
            Rebound();
        }
    }

    protected void Rebound()
    {
        int reboundFlag = 0;
        if ( (_reboundPara & Consts.ReboundLeft) > 0 && _curPos.x < _left )
        {
            _curPos.x = 2 * _left - _curPos.x;
            _curAngle = 180-_curAngle;
            _curAccAngle = 180-_curAccAngle;
            reboundFlag = 1;
        }
        if ((_reboundPara & Consts.ReboundRight) > 0 && _curPos.x > _right )
        {
            _curPos.x = 2 * _right - _curPos.x;
            _curAngle = 180-_curAngle;
            _curAccAngle = 180-_curAccAngle;
            reboundFlag = 1;
        }
        if ((_reboundPara & Consts.ReboundBottom) > 0 && _curPos.y < _bottom)
        {
            _curPos.y = 2 * _bottom - _curPos.y;
            _curAngle = 360f - _curAngle;
            _curAccAngle = 360f - _curAngle;
            reboundFlag = 1;
        }
        if ((_reboundPara & Consts.ReboundTop) > 0 && _curPos.y > _top)
        {
            _curPos.y = 2 * _top - _curPos.y;
            _curAngle = 360f - _curAngle;
            _curAccAngle = 360f - _curAngle;
            reboundFlag = 1;
        }
        if (reboundFlag == 1)
        {
            DoMove(_curVelocity * Consts.TargetFrameRate, _curAngle, _curAcceleration * Consts.TargetFrameRate, _curAccAngle);
            _curReboundCount--;
        }
    }
}
