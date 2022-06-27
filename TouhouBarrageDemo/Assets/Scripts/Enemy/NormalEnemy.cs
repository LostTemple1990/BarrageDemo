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
        UpdateComponents();
        UpdatePos();
        if ( !IsOutOfBorder() )
        {
            CheckCollisionWithCharacter();
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

    #region render
    public override void PlayAni(AniActionType actionType, int dir = Consts.DIR_NULL, int duration = int.MaxValue)
    {
        _enemyObj.DoAction(actionType, dir, duration);
    }

    public override void SetColor(float r, float g, float b, float a)
    {
        _enemyObj.SetColor(r, g, b, a);
    }

    public override void SetScale(float scaleX, float scaleY)
    {
        _enemyObj.SetScale(scaleX, scaleY);
    }

    public override void Render()
    {
        if (!_isAvailable)
            return;
        _enemyObj.Update();
        RenderTransform();
    }
    #endregion

    #region IParaChangable
    public override bool SetParaValue(STGObjectParaType paraType, float value)
    {
        switch (paraType)
        {
            case STGObjectParaType.Velocity:
                _movableObj.velocity = value;
                return true;
            case STGObjectParaType.Vx:
                _movableObj.vx = value;
                return true;
            case STGObjectParaType.Vy:
                _movableObj.vy = value;
                return true;
            case STGObjectParaType.VAngel:
                _movableObj.vAngle = value;
                return true;
            case STGObjectParaType.Acce:
                _movableObj.acce = value;
                return true;
            case STGObjectParaType.AccAngle:
                _movableObj.accAngle = value;
                return true;
            case STGObjectParaType.MaxVelocity:
                _movableObj.maxVelocity = value;
                return true;
            case STGObjectParaType.CurveAngle:
                _movableObj.curveAngle = value;
                return true;
            case STGObjectParaType.CurveRadius:
                _movableObj.curveRadius = value;
                return true;
            case STGObjectParaType.CurveDeltaR:
                _movableObj.curveDeltaRadius = value;
                return true;
            case STGObjectParaType.CurveOmega:
                _movableObj.curveOmega = value;
                return true;
            case STGObjectParaType.CurveCenterX:
                _movableObj.curveCenterX = value;
                return true;
            case STGObjectParaType.CurveCenterY:
                _movableObj.curveCenterY = value;
                return true;

            case STGObjectParaType.Alpha:
                {
                    Color col = _enemyObj.GetColor();
                    _enemyObj.SetColor(col.r, col.g, col.b, value);
                    return true;
                }
            case STGObjectParaType.ScaleX:
                {
                    Vector2 scale = _enemyObj.GetScale();
                    _enemyObj.SetScale(value, scale.y);
                    return true;
                }
            case STGObjectParaType.ScaleY:
                {
                    Vector2 scale = _enemyObj.GetScale();
                    _enemyObj.SetScale(scale.x, value);
                    return true;
                }
        }
        return false;
    }

    public override bool GetParaValue(STGObjectParaType paraType, out float value)
    {
        switch (paraType)
        {
            // ------
            case STGObjectParaType.Velocity:
                value = _movableObj.velocity;
                return true;
            case STGObjectParaType.Vx:
                value = _movableObj.vx;
                return true;
            case STGObjectParaType.Vy:
                value = _movableObj.vy;
                return true;
            case STGObjectParaType.VAngel:
                value = _movableObj.vAngle;
                return true;
            case STGObjectParaType.Acce:
                value = _movableObj.acce;
                return true;
            case STGObjectParaType.AccAngle:
                value = _movableObj.accAngle;
                return true;
            case STGObjectParaType.MaxVelocity:
                value = _movableObj.maxVelocity;
                return true;
            case STGObjectParaType.CurveAngle:
                value = _movableObj.curveAngle;
                return true;
            case STGObjectParaType.CurveRadius:
                value = _movableObj.curveRadius;
                return true;
            case STGObjectParaType.CurveDeltaR:
                value = _movableObj.curveDeltaRadius;
                return true;
            case STGObjectParaType.CurveOmega:
                value = _movableObj.curveOmega;
                return true;
            case STGObjectParaType.CurveCenterX:
                value = _movableObj.curveCenterX;
                return true;
            case STGObjectParaType.CurveCenterY:
                value = _movableObj.curveCenterY;
                return true;
             //   ---------------------------------
            case STGObjectParaType.Alpha:
                value = _enemyObj.GetColor().a;
                return true;
            case STGObjectParaType.ScaleX:
                value = _enemyObj.GetScale().x;
                return true;
            case STGObjectParaType.ScaleY:
                value = _enemyObj.GetScale().y;
                return true;
        }
        value = 0;
        return false;
    }

    #endregion

}
