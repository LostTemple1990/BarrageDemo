using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    protected int _nextDir;

    //protected AnimationCharacter _enemyAni;
    protected EnemyObjectBase _enemyObj;

    public NormalEnemy()
    {
        _type = EnemyType.NormalEnemy;
    }

    public void Init(EnemyCfg cfg)
    {
        base.Init();
        _enemyObj = EnemyManager.GetInstance().CreateEnemyObjectByType(cfg.type);
        _enemyObj.SetEnemyAni(cfg.aniId);
        _enemyGo = _enemyObj.GetObject();
        //SetEnemyAni(cfg.aniId);
        SetCollisionParams(cfg.collisionHalfWidth, cfg.collisionHalfWidth);
        SetMaxHp(cfg.maxHp);
        SetDrop(cfg.dropId, cfg.dropHalfWidth, cfg.dropHalfHeight);
    }

    /// <summary>
    /// 设置敌机贴图
    /// </summary>
    //public virtual void SetEnemyAni(string aniId)
    //{
    //    _enemyAni = AnimationManager.GetInstance().CreateAnimation<AnimationCharacter>(LayerId.Enemy);
    //    _enemyGo = _enemyAni.GetAniParent();
    //    _enemyAni.Play(aniId, AniActionType.Idle,Consts.DIR_NULL);
    //}

    public override void Update()
    {
        UpdateTask();
        _movableObj.Update();
        UpdatePos();
        CheckCollisionWithCharacter();
        _enemyObj.Update();
        //if ( _isMoving )
        //{
        //    _moveTime++;
        //    if ( _moveTime >= _moveDuration )
        //    {
        //        _isMoving = false;
        //        _enemyAni.Play(AniActionType.Idle, Consts.DIR_NULL);
        //    }
        //}
    }

    public override void MoveToPos(float posX, float posY, int duration, InterpolationMode mode)
    {
        base.MoveToPos(posX, posY, duration, mode);
        if ( posX > _curPos.x )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_RIGHT);
            //_enemyAni.Play(AniActionType.Move, Consts.DIR_RIGHT);
        }
        else if ( posX < _curPos.x )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_LEFT);
            //_enemyAni.Play(AniActionType.Move, Consts.DIR_LEFT);
        }
    }

    public override void MoveTowards(float velocity, float angle, int duration)
    {
        while (angle < 0)
        {
            angle += 360f;
        }
        while (angle >= 360)
        {
            angle -= 360f;
        }
        base.MoveTowards(velocity, angle, duration);
        if ( angle > 90 && angle < 270 )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_LEFT);
            //_enemyAni.Play(AniActionType.Move, Consts.DIR_LEFT);
        }
        else if ( angle < 90 || angle > 270 )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_RIGHT);
            //_enemyAni.Play(AniActionType.Move, Consts.DIR_RIGHT);
        }
    }

    public override void GetHit(int damage)
    {
        if (!_isInteractive) return;
        _curHp -= damage;
        if ( _curHp <= 0 )
        {
            _isInteractive = false;
            _curHp = 0;
            Eliminate(eEnemyEliminateDef.Player);
        }
    }

    public override void Clear()
    {
        EnemyManager.GetInstance().RestoreEnemyObjectToPool(_enemyObj);
        _enemyObj = null;
        base.Clear();
    }

    public override int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        arg1 = _curPos.x;
        arg2 = _curPos.y;
        arg3 = _collisionHalfWidth;
        arg4 = _collisionHalfHeight;
        return Consts.CollisionType_Rect;
    }

    public override bool Eliminate(eEnemyEliminateDef eliminateType = eEnemyEliminateDef.ForcedDelete)
    {
        _curHp = 0;
        if ( base.Eliminate(eliminateType) )
        {
            if ( eliminateType != eEnemyEliminateDef.ForcedDelete )
            {
                SoundManager.GetInstance().Play("se_tan02", false);
            }
            if ( eliminateType != eEnemyEliminateDef.ForcedDelete || eliminateType != eEnemyEliminateDef.CodeRawEliminate )
            {
                DropItems();
            }
            return true;
        }
        return false;
    }
}
