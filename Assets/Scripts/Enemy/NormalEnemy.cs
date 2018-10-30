using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    protected int _nextDir;

    //protected AnimationCharacter _enemyAni;
    protected EnemyObjectBase _enemyObj;
    /// <summary>
    /// 掉落的道具数据
    /// </summary>
    protected List<int> _dropItemDatas;
    /// <summary>
    /// 掉落道具的范围-宽度的一半
    /// </summary>
    protected float _dropHalfWidth;
    /// <summary>
    /// 掉落道具的范围-高度的一半
    /// </summary>
    protected float _dropHalfHeight;

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
        _dropItemDatas = null;
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

    public override void GetHit(int damage,eEliminateDef eliminateType=eEliminateDef.PlayerBullet)
    {
        if ((_resistEliminateFlag & (int)eliminateType) != 0) return;
        if (!_isInteractive) return;
        _curHp -= damage;
        if ( _curHp <= 0 )
        {
            _isInteractive = false;
            _curHp = 0;
            Eliminate(eliminateType);
        }
    }

    public override void Clear()
    {
        EnemyManager.GetInstance().RestoreEnemyObjectToPool(_enemyObj);
        _enemyObj = null;
        _dropItemDatas = null;
        base.Clear();
    }

    /// <summary>
    /// 设置敌机的死亡掉落
    /// </summary>
    public void SetDropItemDatas(List<int> itemDatas,float halfWidth,float halfHeight)
    {
        _dropItemDatas = itemDatas;
        _dropHalfWidth = halfWidth;
        _dropHalfHeight = halfHeight;
    }

    /// <summary>
    /// 掉落物体
    /// </summary>
    protected void DropItems()
    {
        ItemManager.GetInstance().DropItems(_dropItemDatas, _curPos.x, _curPos.y, _dropHalfWidth, _dropHalfHeight);
    }

    public override bool Eliminate(eEliminateDef eliminateType = eEliminateDef.ForcedDelete)
    {
        _curHp = 0;
        if ( base.Eliminate(eliminateType) )
        {
            if ( eliminateType != eEliminateDef.ForcedDelete )
            {
                SoundManager.GetInstance().Play("se_tan02", false);
            }
            if ( eliminateType != eEliminateDef.ForcedDelete || eliminateType != eEliminateDef.CodeRawEliminate )
            {
                if ( _dropItemDatas != null )
                {
                    DropItems();
                }
            }
            return true;
        }
        return false;
    }
}
