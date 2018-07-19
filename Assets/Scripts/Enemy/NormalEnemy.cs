using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    protected int _nextDir;

    //protected AnimationCharacter _enemyAni;
    protected EnemyObjectBase _enemyObj;

    protected int _onKillFuncRef;

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
        _onKillFuncRef = 0;
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

    public virtual void SetOnKillFuncRef(int funcRef)
    {
        _onKillFuncRef = funcRef;
    }

    protected virtual void CreateOnKillTask()
    {
        if ( _onKillFuncRef != 0 )
        {
            Task task = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
            task.funcRef = _onKillFuncRef;
            InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
            InterpreterManager.GetInstance().CallTaskCoroutine(task, 1);
            ExtraTaskManager.GetInstance().AddTask(task);
        }
    }

    public override void DoHit(float damage)
    {
        _curHp -= damage;
        if ( _curHp <= 0 )
        {
            _clearFlag = 1;
            _curHp = 0;
            SoundManager.GetInstance().Play("se_tan02", false);
            DropItems();
            CreateOnKillTask();
        }
    }

    public override void Clear()
    {
        base.Clear();
        //AnimationManager.GetInstance().RemoveAnimation(_enemyAni);
        //_enemyAni = null;
        _onKillFuncRef = 0;
        EnemyManager.GetInstance().RestoreEnemyObjectToPool(_enemyObj);
        _enemyObj = null;
    }

    public override int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        arg1 = _curPos.x;
        arg2 = _curPos.y;
        arg3 = _collisionHalfWidth;
        arg4 = _collisionHalfHeight;
        return Consts.CollisionType_Rect;
    }

    public override void Kill()
    {
        _clearFlag = 1;
        _curHp = 0;
        SoundManager.GetInstance().Play("se_tan02", false);
        DropItems();
        CreateOnKillTask();
    }

    public override void RawKill()
    {
        _clearFlag = 1;
        _curHp = 0;
        SoundManager.GetInstance().Play("se_tan02", false);
    }
}
