using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCRebound : BulletComponent
{
    private EnemyBulletMovable _bullet;
    private int _reboundPara;
    private int _reboundCount;
    private float _changeV;

    private float _left, _bottom, _right, _top;

    public override void Init(EnemyBulletBase bullet)
    {
        _bullet = bullet as EnemyBulletMovable;
        _reboundCount = 0;
        _reboundPara = 0;

        _left = Global.GameLBBorderPos.x;
        _right = Global.GameRTBorderPos.x;
        _bottom = Global.GameLBBorderPos.y;
        _top = Global.GameRTBorderPos.y;
    }

    public override void Update()
    {
        if ( _reboundCount > 0 )
        {
            Rebound();
        }
    }

    public override void Clear()
    {
        _bullet = null;
    }

    public void AddReboundPara(int reboundPara, int reboundCount,float changeVelocity=-1)
    {
        _reboundPara = reboundPara;
        _reboundCount = reboundCount;
    }

    private void Rebound()
    {
        int reboundFlag = 0;
        Vector2 curPos = new Vector2(_bullet.PosX, _bullet.PosY);
        float vAngle;
        _bullet.GetBulletPara(BulletParaType.VAngel, out vAngle);
        if ((_reboundPara & Consts.ReboundLeft) > 0 && curPos.x < _left)
        {
            curPos.x = 2 * _left - curPos.x;
            vAngle = 180 - vAngle;
            //_curAccAngle = 180 - _curAccAngle;
            reboundFlag = 1;
        }
        if ((_reboundPara & Consts.ReboundRight) > 0 && curPos.x > _right)
        {
            curPos.x = 2 * _right - curPos.x;
            vAngle = 180 - vAngle;
            //_curAccAngle = 180 - _curAccAngle;
            reboundFlag = 1;
        }
        if ((_reboundPara & Consts.ReboundBottom) > 0 && curPos.y < _bottom)
        {
            curPos.y = 2 * _bottom - curPos.y;
            vAngle = 360f - vAngle;
            //_curAccAngle = 360f - _curAngle;
            reboundFlag = 1;
        }
        if ((_reboundPara & Consts.ReboundTop) > 0 && curPos.y > _top)
        {
            curPos.y = 2 * _top - curPos.y;
            vAngle = 360f - vAngle;
            //_curAccAngle = 360f - _curAngle;
            reboundFlag = 1;
        }
        if (reboundFlag == 1)
        {
            //DoMove(_curVelocity * Consts.TargetFrameRate, _curAngle, _curAcceleration * Consts.TargetFrameRate, _curAccAngle);
            float velocity;
            _bullet.GetBulletPara(BulletParaType.Velocity, out velocity);
            _bullet.DoMoveStraight(velocity, vAngle);
            _reboundCount--;
        }
    }
}
