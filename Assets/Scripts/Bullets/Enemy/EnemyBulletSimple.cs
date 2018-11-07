using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletSimple : EnemyBulletMovable
{
    #region 碰撞相关参数
    /// <summary>
    /// 擦弹检测类型，默认为矩形
    /// </summary>
    protected int _grazeType = Consts.GrazeType_Rect;
    protected float _grazeHalfWidth;
    protected float _grazeHalfHeight;
    /// <summary>
    /// 碰撞检测类型
    /// </summary>
    protected int _collisionType;
    /// <summary>
    /// 碰撞检测参数0，圆的半径/宽的一半
    /// </summary>
    protected float _collisionArg0;
    /// <summary>
    /// 碰撞检测参数1，圆的半径/高的一半
    /// </summary>
    protected float _collisionArg1;

    protected float _collisionRadius;

    #endregion
    /// <summary>
    /// 表示当前是否虚化状态
    /// </summary>
    protected bool _isInUnrealState;
    protected int _unRealTime;
    protected int _unRealDuration;
    /// <summary>
    /// 原先的颜色设置
    /// </summary>
    protected Color _originalColor;
    protected bool _colorIsChange;

    protected SpriteRenderer _spRenderer;
    protected EnemyBulletDefaultCfg _cfg;
    /// <summary>
    /// 检测碰撞判断用的一个值
    /// <para>这个值等于子弹碰撞半径与玩家碰撞半径的和的平方</para>
    /// </summary>
    protected float _detCollisonValue;
    /// <summary>
    /// 检测擦弹判断用的一个值
    /// <para>这个值等于子弹碰撞半径与玩家擦弹半径的平方</para>
    /// </summary>
    protected float _detGrazeValue;

    #region 缩放相关参数
    /// <summary>
    /// 当前正在改变尺寸
    /// </summary>
    protected bool _isScalingSize;
    /// <summary>
    /// 本体的缩放系数
    /// </summary>
    protected float _scaleFactor;
    /// <summary>
    /// 源缩放尺寸
    /// </summary>
    protected float _scaleFrom;
    /// <summary>
    /// 目标缩放尺寸
    /// </summary>
    protected float _scaleTo;
    /// <summary>
    /// 缩放时间
    /// </summary>
    protected int _scaleTime;
    /// <summary>
    /// 缩放持续时间
    /// </summary>
    protected int _scaleDuration;
    /// <summary>
    /// 缩放延迟
    /// </summary>
    protected int _scaleDelay;
    #endregion

    public override void Init()
    {
        base.Init();
        _id = BulletId.Enemy_Simple;
        _orderInLayer = 0;
        _isInUnrealState = false;
        _isScalingSize = false;
    }

    public override void Update()
    {
        _lastPos = _curPos;
        base.Update();
        CheckRotateImg();
        if ( _isInUnrealState )
        {
            UpdateUnrealState();
        }
        CheckCollisionWithCharacter();
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
        else
        {
            UpdatePos();
        }
    }

    public override void SetBulletTexture(string texture)
    {
        _prefabName = _cfg.id.ToString();
        //_bullet = ResourceManager.GetInstance().GetPrefab("BulletPrefab", _prefabName);
        //UIManager.GetInstance().AddGoToLayer(_bullet, LayerId.EnemyBarrage);
        _bullet = BulletsManager.GetInstance().CreateBulletGameObject(_id, _cfg.id);
        _trans = _bullet.transform;
        _spRenderer = _trans.Find("BulletSprite").GetComponent<SpriteRenderer>();
    }

    public virtual void SetSelfRotation(bool isSelfRotation, float angle)
    {
        _isSelfRotation = isSelfRotation;
        if (_isSelfRotation)
        {
            _selfRotationAngle = new Vector3(0, 0, angle);
        }
    }

    public virtual void SetRotatedByVelocity(bool value)
    {
        _isRotatedByVelocity = value;
        _imgRotatedFlag = 1;
    }

    public virtual void ChangeStyleById(string id)
    {
        // 根据传入的id读取配置，重新生成数据
        EnemyBulletDefaultCfg cfg = BulletsManager.GetInstance().GetBulletDefaultCfgById(id);
        if ( cfg == null )
        {
            Logger.LogError("Bullet ChangeStyle Fail! SysId  " + id + " is not exist!");
            return;
        }
        if ( _bullet != null )
        {
            // 回收现有的prefab
            UIManager.GetInstance().RemoveGoFromLayer(_bullet);
            ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        }
        _cfg = cfg;
        SetBulletTexture(cfg.spriteName);
        SetToPosition(_curPos.x, _curPos.y);
        SetRotatedByVelocity(cfg.isRotatedByVAngle);
        SetSelfRotation(cfg.selfRotationAngle != 0, cfg.selfRotationAngle);
        GrazeDetectParas grazeParas = new GrazeDetectParas
        {
            type = GrazeDetectType.Rect,
            halfWidth = cfg.grazeHalfWidth,
            halfHeight = cfg.grazeHalfHeight,
        };
        _collisionRadius = cfg.collisionRadius * _scaleFactor;
        // 计算用于擦弹以及碰撞检测的两个数值
        // 擦弹
        float value = Global.PlayerGrazeRadius + _collisionRadius;
        _detGrazeValue = value * value;
        // 碰撞
        value = Global.PlayerCollisionVec.z + _collisionRadius;
        _detCollisonValue = value * value;

        SetGrazeDetectParas(grazeParas);
        _cfg = cfg;
    }

    /// <summary>
    /// 将子弹设置成虚化状态
    /// </summary>
    /// <param name="time"></param>
    public virtual void SetToUnrealState(int duration)
    {
        // 保存原有的颜色数据
        _originalColor = _spRenderer.color;
        Color unrealColor = _originalColor;
        unrealColor.a = 0.3f;
        _spRenderer.color = unrealColor;
        _colorIsChange = true;
        // 设置时间
        _unRealTime = 0;
        _unRealDuration = duration;
        _isInUnrealState = true;
    }

    protected void UpdateUnrealState()
    {
        _unRealTime++;
        if ( _unRealTime >= _unRealDuration )
        {
            _spRenderer.color = _originalColor;
            _colorIsChange = false;
            _isInUnrealState = false;
        }
    }

    /// <summary>
    /// 缩放
    /// </summary>
    protected void UpdateScaling()
    {
        if ( _scaleDelay > 0 )
        {
            _scaleDelay--;
            return;
        }
        _scaleTime++;
        float scaleFactor;
        if ( _scaleTime >= _scaleDuration )
        {
            _isScalingSize = false;
            scaleFactor = _scaleTo;
        }
        else
        {
            scaleFactor = Mathf.Lerp(_scaleFrom, _scaleTo, _scaleTime / _scaleDuration);
        }
        SetToScale(scaleFactor);
    }

    /// <summary>
    /// 设置缩放
    /// </summary>
    /// <param name="scale"></param>
    public void SetToScale(float scale)
    {
        _scaleFactor = scale;
        // 计算碰撞参数
        // 计算用于擦弹以及碰撞检测的两个数值
        // 擦弹
        float value = Global.PlayerGrazeRadius + _collisionRadius;
        _detGrazeValue = value * value;
        // 碰撞
        value = Global.PlayerCollisionVec.z + _collisionRadius;
        _detCollisonValue = value * value;
        // 设置scale
        _trans.localScale = new Vector3(_scaleFactor, _scaleFactor, 1);
    }

    /// <summary>
    /// 执行缩放
    /// </summary>
    /// <param name="toScale"></param>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    public void DoScale(float toScale,int delay,int duration)
    {
        _scaleFrom = _scaleFactor;
        _scaleTo = toScale;
        _scaleDelay = delay;
        _scaleTime = 0;
        _scaleDuration = duration;
        _isScalingSize = true;
    }

    protected virtual void RotateImgByVelocity()
    {
        Vector2 dv = _curPos - _lastPos;
        float rotateAngle = MathUtil.GetAngleBetweenXAxis(dv.x, dv.y, false) - 90;
        _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
    }

    protected void CheckRotateImg()
    {
        if (_imgRotatedFlag == 1)
        {
            RotateImgByVelocity();
        }
        if ( _isRotatedByVelocity )
        {
            if ( _isMovingStraight )
            {
                if (_curAcceleration == 0 || _curAngle == _curAccAngle)
                {
                    _imgRotatedFlag = 0;
                    return;
                }
            }
            _imgRotatedFlag = 1;
        }
    }

    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
        _grazeParas = paras;
    }

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        Vector2 bulletBoundingBoxLBPos = new Vector2(_curPos.x - _collisionRadius, _curPos.y - _collisionRadius);
        Vector2 bulletBoundingBoxRTPos = new Vector2(_curPos.x + _collisionRadius, _curPos.y + _collisionRadius);
        // 由于通常情况下大部分子弹都是不在包围盒范围内的
        // 因此选择判断不相交的方式尽可能减少判断的次数
        bool notHit =  bulletBoundingBoxRTPos.x < lbPos.x ||    // 子弹包围盒右边缘X坐标小于检测包围盒的左边缘X坐标
            bulletBoundingBoxLBPos.x > rtPos.x ||   // 子弹包围盒左边缘X坐标大于检测包围盒的左右边缘X坐标
            bulletBoundingBoxRTPos.y < lbPos.y ||   // 子弹包围盒上边缘Y坐标小于检测包围盒的下边缘Y坐标
            bulletBoundingBoxLBPos.y > rtPos.y;     // 子弹包围盒下边缘Y坐标大于检测包围盒的上边缘Y坐标
        return !notHit;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        CollisionDetectParas paras = new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            centerPos = _curPos,
            radius = _collisionRadius,
            nextIndex = -1,
        };
        return paras;
    }

    public override void CollidedByObject(int n = 0, eEliminateDef eliminateType = eEliminateDef.HitObjectCollider)
    {
        Eliminate(eliminateType);
    }

    /// <summary>
    /// 与玩家的碰撞检测
    /// </summary>
    protected virtual void CheckCollisionWithCharacter()
    {
        // 虚化状态不进行碰撞判定
        if ( _isInUnrealState )
        {
            return;
        }
        if ( !_detectCollision )
        {
            return;
        }
        float dx = Mathf.Abs(_curPos.x - Global.PlayerPos.x);
        float dy = Mathf.Abs(_curPos.y - Global.PlayerPos.y);
        // 子弹中心与玩家中心的距离的平方
        float detValue = dx * dx + dy * dy;
        // 首先检测是否在擦弹范围
        if ( detValue <= _detGrazeValue )
        {
            if ( !_isGrazed )
            {
                PlayerService.GetInstance().AddGraze(1);
                _isGrazed = true;
            }
            // 在擦弹范围内，进行实际的碰撞检测
            if (detValue <= _detCollisonValue)
            {
                Eliminate(eEliminateDef.HitPlayer);
                PlayerService.GetInstance().GetCharacter().BeingHit();
            }
        }
    }

    public override void Clear()
    {
        SetOrderInLayer(0);
        if ( _colorIsChange )
        {
            _spRenderer.color = _originalColor;
            _colorIsChange = false;
        }
        if ( _scaleFactor != 1 )
        {
            _trans.localScale = Vector3.one;
        }
        _spRenderer = null;
        _cfg = null;
        base.Clear();
    }

    public override bool Eliminate(eEliminateDef eliminateType = eEliminateDef.ForcedDelete)
    {
        if ( base.Eliminate(eliminateType) )
        {
            if ( eliminateType != eEliminateDef.PlayerSpellCard && eliminateType != eEliminateDef.ForcedDelete )
            {
                Color eliminateColor = _cfg.eliminateColor;
                STGBulletEliminateEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.BulletEliminate) as STGBulletEliminateEffect;
                effect.SetColor(eliminateColor);
                effect.SetToPos(_curPos.x, _curPos.y);
            }
            return true;
        }
        return false;
    }
}
