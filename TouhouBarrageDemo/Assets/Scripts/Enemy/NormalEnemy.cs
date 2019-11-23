using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyBase
{
    protected int _nextDir;

    //protected AnimationCharacter _enemyAni;
    protected EnemyObjectBase _enemyObj;
    /// <summary>
    /// 对应普通敌机的配置
    /// </summary>
    protected EnemyCfg _cfg;
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
        _type = eEnemyType.NormalEnemy;
    }

    public void Init(EnemyCfg cfg)
    {
        _cfg = cfg;
        _enemyObj = EnemyManager.GetInstance().CreateEnemyObjectByType(cfg.type);
        _enemyObj.SetEnemyAni(cfg.aniId);
        _enemyGo = _enemyObj.GetObject();
        _enemyTf = _enemyGo.transform;
        SetCollisionParams(cfg.collisionHalfWidth, cfg.collisionHalfWidth);
        _dropItemDatas = null;
        _checkOutOfBorder = true;
    }

    public void Init(string enemyId)
    {
        EnemyCfg cfg = EnemyManager.GetInstance().GetEnemyCfgById(enemyId);
        Init(cfg);
    }

    public override void Update()
    {
        if (_isInvincible)
        {
            UpdateInvincibleStatus();
        }
        UpdateTask();
        UpdatePos();
        if ( !IsOutOfBorder() )
        {
            CheckCollisionWithCharacter();
            _enemyObj.Update();
            RenderTransform();
        }
        else
        {
            Eliminate(eEliminateDef.ForcedDelete);
        }
    }

    private void UpdatePos()
    {
        if ( !_isFollowingMasterContinuously )
        {
            if (_movableObj.IsActive())
            {
                _movableObj.Update();
                _curPos = _movableObj.GetPos();
            }
        }
        else
        {
            if ( _attachableMaster != null )
            {
                Vector2 relativePos = _relativePosToMaster;
                if ( _isFollowMasterRotation )
                {
                    relativePos = MathUtil.GetVec2AfterRotate(relativePos.x, relativePos.y, 0, 0, _attachableMaster.GetRotation());
                }
                _curPos = relativePos + _attachableMaster.GetPosition();
            }
        }
    }

    public override void MoveTo(float posX, float posY, int duration, InterpolationMode mode)
    {
        base.MoveTo(posX, posY, duration, mode);
        if ( posX > _curPos.x )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_RIGHT);
        }
        else if ( posX < _curPos.x )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_LEFT);
        }
    }

    /// <summary>
    /// 朝某个方向做匀速直线运动
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="angle"></param>
    /// <param name="duration"></param>
    public override void MoveTowards(float velocity, float angle, int duration)
    {
        angle = MathUtil.ClampAngle(angle);
        base.MoveTowards(velocity, angle, duration);
        if ( angle > 90 && angle < 270 )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_LEFT);
        }
        else if ( angle < 90 || angle > 270 )
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_RIGHT);
        }
    }

    /// <summary>
    /// 朝某个方向做加速直线运动
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="angle"></param>
    /// <param name="acc"></param>
    public override void AccMoveTowards(float velocity, float angle, float acc)
    {
        angle = MathUtil.ClampAngle(angle);
        base.AccMoveTowards(velocity, angle, acc);
        if (angle > 90 && angle < 270)
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_LEFT);
        }
        else if (angle < 90 || angle > 270)
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_RIGHT);
        }
    }

    public override void AccMoveTowardsWithLimitation(float velocity, float angle, float acc, float maxVelocity)
    {
        angle = MathUtil.ClampAngle(angle);
        base.AccMoveTowardsWithLimitation(velocity, angle, acc, maxVelocity);
        if (angle > 90 && angle < 270)
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_LEFT);
        }
        else if (angle < 90 || angle > 270)
        {
            _enemyObj.DoAction(AniActionType.Move, Consts.DIR_RIGHT);
        }
    }

    public override void TakeDamage(float damage,eEliminateDef eliminateType=eEliminateDef.PlayerBullet)
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
        _cfg = null;
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
                SoundManager.GetInstance().Play("killenemy", 0.1f, false, true);
                if ( _cfg.eliminatedEffectStyle != 0 )
                {
                    STGEnemyEliminatedEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.EnemyEliminated) as STGEnemyEliminatedEffect;
                    effect.SetEliminateEffectStyle(_cfg.eliminatedEffectStyle);
                    effect.SetPosition(_curPos.x, _curPos.y);
                }
            }
            if ( eliminateType != eEliminateDef.ForcedDelete && eliminateType != eEliminateDef.CodeRawEliminate )
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
