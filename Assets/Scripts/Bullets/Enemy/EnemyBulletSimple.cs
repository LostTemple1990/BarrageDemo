using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletSimple : EnemyBulletMovable
{
    /// <summary>
    /// 子弹出现特效的存在时间
    /// </summary>
    public const int AppearEffectExistDuration = 11;

    #region 碰撞相关参数
    protected float _grazeHalfWidth;
    protected float _grazeHalfHeight;
    /// <summary>
    /// 碰撞检测参数0，圆的半径/宽的一半
    /// </summary>
    protected float _collisionHalfWidth;
    /// <summary>
    /// 碰撞检测参数1，圆的半径/高的一半
    /// </summary>
    protected float _collisionHalfHeight;

    protected float _collisionRadius;

    #endregion
    /// <summary>
    /// 原先的颜色设置
    /// </summary>
    protected Color _originalColor;

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

    protected float _scaleX;
    protected float _scaleY;
    /// <summary>
    /// 当前帧缩放是否改变
    /// </summary>
    protected bool _isScaleChanged;
    #endregion

    /// <summary>
    /// 子弹出现的特效
    /// </summary>
    protected STGSpriteEffect _appearEffect;

    public override void Init()
    {
        base.Init();
        _id = BulletId.Enemy_Simple;
        _orderInLayer = 0;
        _isScalingSize = false;
        _scaleX = _scaleY = 1;
        _originalColor = new Color(1, 1, 1, 1);
    }

    public override void Update()
    {
        _lastPos = _curPos;
        base.Update();
        if (_appearEffect != null)
        {
            UpdateAppearEffect();
        }
        if (_isScalingSize) UpdateScaling();
        CheckCollisionWithCharacter();
        if ( IsOutOfBorder() )
        {
            _clearFlag = 1;
        }
        else
        {
            if (_isScaleChanged) UpdateScale();
            if (_isColorChanged) UpdateColor();
            CheckRotateImg();
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
            _imgRotatedFlag = 1;
        }
    }

    public virtual void SetRotatedByVelocity(bool value)
    {
        _isRotatedByVelocity = value;
        _imgRotatedFlag = 1;
    }

    public override void SetStyleById(string id)
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
        _collisionHalfWidth = cfg.collisionRadius * _scaleX;
        _collisionHalfHeight = cfg.collisionRadius * _scaleY;
        _collisionRadius = cfg.collisionRadius * _scaleX;
        // 计算用于擦弹以及碰撞检测的两个数值
        // 擦弹
        float value = Global.PlayerGrazeRadius + _collisionRadius;
        _detGrazeValue = value * value;
        // 碰撞
        value = Global.PlayerCollisionVec.z + _collisionRadius;
        _detCollisonValue = value * value;

        SetGrazeDetectParas(grazeParas);
    }

    public void CreateAppearEffect()
    {
        if (_timeSinceCreated != 0) return;
        if (_cfg.appearEffectSizeFrom == 0) return;
        _appearEffect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
        _appearEffect.SetSprite(Consts.STGBulletsAtlasName, _cfg.appearEffectName, _cfg.blendMode, LayerId.EnemyBarrage, true);
        //_appearEffect.SetOrderInLayer(10);
        _appearEffect.SetToPos(_curPos.x, _curPos.y);
        _appearEffect.SetScale(_cfg.appearEffectSizeFrom, _cfg.appearEffectSizeFrom);
        //_appearEffect.DoScaleWidth(_cfg.appearEffectSizeTo, AppearEffectExistDuration, InterpolationMode.Linear);
        //_appearEffect.DoScaleHeight(_cfg.appearEffectSizeTo, AppearEffectExistDuration, InterpolationMode.Linear);
        _appearEffect.DoFade(AppearEffectExistDuration);
    }

    private void UpdateAppearEffect()
    {
        _appearEffect.SetToPos(_curPos.x, _curPos.y);
        float factor = (float)_timeSinceCreated / AppearEffectExistDuration;
        float scaleX = Mathf.Lerp(_cfg.appearEffectSizeFrom, _cfg.appearEffectSizeTo, factor) * _scaleX;
        float scaleY = Mathf.Lerp(_cfg.appearEffectSizeFrom, _cfg.appearEffectSizeTo, factor) * _scaleY;
        _appearEffect.SetScale(scaleX, scaleY);
        if ( _timeSinceCreated >= AppearEffectExistDuration)
        {
            //_appearEffect.FinishEffect();
            _appearEffect = null;
        }
    }

    public override void SetAlpha(float alpha)
    {
        _curAlpha = alpha;
        _isColorChanged = true;
        _isOriginalColor = false;
    }

    public override void SetColor(float r, float g, float b)
    {
        _curColor = new Color(r / 255, g / 255, b / 255);
        _isColorChanged = true;
        _isOriginalColor = false;
    }

    public override void SetColor(float r, float g, float b, float a)
    {
        _curColor = new Color(r / 255, g / 255, b / 255, a);
        _curAlpha = a;
        _isColorChanged = true;
        _isOriginalColor = false;
    }

    protected void UpdateColor()
    {
        _spRenderer.color = new Color(_curColor.r, _curColor.g, _curColor.b, _curAlpha);
        _isColorChanged = false;
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
            scaleFactor = Mathf.Lerp(_scaleFrom, _scaleTo, (float)_scaleTime / _scaleDuration);
        }
        SetScale(scaleFactor);
    }

    /// <summary>
    /// 设置缩放
    /// </summary>
    /// <param name="scale"></param>
    public void SetScale(float scale)
    {
        _scaleX = _scaleY = scale;
        _isScaleChanged = true;
        // 计算碰撞参数
        _collisionHalfWidth = _cfg.collisionRadius * _scaleX;
        _collisionHalfHeight = _cfg.collisionRadius * _scaleY;
        // 计算用于擦弹以及碰撞检测的两个数值
        // 擦弹
        float value = Global.PlayerGrazeRadius + _collisionHalfWidth;
        _detGrazeValue = value * value;
        // 碰撞
        value = Global.PlayerCollisionVec.z + _collisionHalfWidth;
        _detCollisonValue = value * value;
        _isScaleChanged = true;
    }

    /// <summary>
    /// 执行缩放
    /// </summary>
    /// <param name="toScale"></param>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    public void DoScale(float toScale,int delay,int duration)
    {
        // X，Y轴缩放不相等，则不执行这个方法
        if (_scaleX != _scaleY) return;
        _scaleFrom = _scaleX;
        _scaleTo = toScale;
        _scaleDelay = delay;
        _scaleTime = 0;
        _scaleDuration = duration;
        _isScalingSize = true;
    }

    protected void UpdateScale()
    {
        _trans.localScale = new Vector3(_scaleX, _scaleY, 1);
        _isScaleChanged = false;
    }

    protected virtual void RotateImgByVelocity()
    {
        Vector2 dv = _curPos - _lastPos;
        float rotateAngle;
        if ( dv.x == 0 && dv.y == 0 )
        {
            rotateAngle = _curAngle - 90;
        }
        else
        {
            rotateAngle = MathUtil.GetAngleBetweenXAxis(dv.x, dv.y, false) - 90;
        }
        _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
    }


    protected void CheckRotateImg()
    {
        if (_imgRotatedFlag == 1)
        {
            if ( _isRotatedByVelocity )
            {
                RotateImgByVelocity();
            }
            else
            {
                _trans.Rotate(0, 0, _cfg.selfRotationAngle);
            }
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
        }
        _imgRotatedFlag = 1;
    }

    public override void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeHalfWidth = paras.halfWidth;
        _grazeHalfHeight = paras.halfHeight;
        _grazeParas = paras;
    }

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        // 当碰撞宽度 = 碰撞宽度时，采取正常的矩形检测
        if ( _collisionHalfWidth == _collisionHalfHeight )
        {
            Vector2 bulletBoundingBoxLBPos = new Vector2(_curPos.x - _collisionHalfWidth, _curPos.y - _collisionHalfHeight);
            Vector2 bulletBoundingBoxRTPos = new Vector2(_curPos.x + _collisionHalfWidth, _curPos.y + _collisionHalfHeight);
            // 由于通常情况下大部分子弹都是不在包围盒范围内的
            // 因此选择判断不相交的方式尽可能减少判断的次数
            bool notHit = bulletBoundingBoxRTPos.x < lbPos.x ||    // 子弹包围盒右边缘X坐标小于检测包围盒的左边缘X坐标
                bulletBoundingBoxLBPos.x > rtPos.x ||   // 子弹包围盒左边缘X坐标大于检测包围盒的左右边缘X坐标
                bulletBoundingBoxRTPos.y < lbPos.y ||   // 子弹包围盒上边缘Y坐标小于检测包围盒的下边缘Y坐标
                bulletBoundingBoxLBPos.y > rtPos.y;     // 子弹包围盒下边缘Y坐标大于检测包围盒的上边缘Y坐标
            return !notHit;
        }
        return true;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        if ( _collisionHalfWidth == _collisionHalfHeight )
        {
            CollisionDetectParas paras = new CollisionDetectParas
            {
                type = CollisionDetectType.Circle,
                centerPos = _curPos,
                radius = _collisionHalfWidth,
                nextIndex = -1,
            };
            return paras;
        }
        else
        {
            CollisionDetectParas paras = new CollisionDetectParas
            {
                type = CollisionDetectType.ItalicRect,
                centerPos = _curPos,
                halfWidth = _collisionHalfWidth,
                halfHeight = _collisionHalfHeight,
                angle = _curAngle,
                nextIndex = -1,
            };
            return paras;
        }
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
        if (!_detectCollision) return;
        if ( _collisionHalfWidth == _collisionHalfHeight )
        {
            float dx = Mathf.Abs(_curPos.x - Global.PlayerPos.x);
            float dy = Mathf.Abs(_curPos.y - Global.PlayerPos.y);
            // 子弹中心与玩家中心的距离的平方
            float detValue = dx * dx + dy * dy;
            // 首先检测是否在擦弹范围
            if (detValue <= _detGrazeValue)
            {
                if (!_isGrazed)
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
        else
        {
            // 碰撞判定宽高不相等时，采用矩形检测
            Vector2 relativePos = Global.PlayerPos - _curPos;
            Vector2 relativeVec = MathUtil.GetVec2AfterRotate(relativePos.x, relativePos.y, 0, 0, _curAngle);
            // 判定是否在矩形内
            float len = relativeVec.magnitude;
            float rate = (len - Global.PlayerGrazeRadius) / len;
            bool isGrazing = false;
            if (rate < 0)
            {
                isGrazing = true;
            }
            else
            {
                Vector2 tmpVec = relativeVec * rate;
                if (Mathf.Abs(tmpVec.x) < _collisionHalfHeight && Mathf.Abs(tmpVec.y) < _collisionHalfWidth)
                {
                    isGrazing = true;
                }
            }
            if (isGrazing)
            {
                if (!_isGrazed)
                {
                    _isGrazed = true;
                    PlayerService.GetInstance().AddGraze(1);
                }
                rate = (len - Global.PlayerCollisionVec.z) / len;
                relativeVec *= rate;
                if (rate <= 0 || (Mathf.Abs(relativeVec.x) < _collisionHalfHeight && Mathf.Abs(relativeVec.y) < _collisionHalfWidth))
                {
                    Eliminate(eEliminateDef.HitPlayer);
                    PlayerService.GetInstance().GetCharacter().BeingHit();
                }
            }
        }
    }

    public override void Clear()
    {
        if ( _appearEffect != null )
        {
            _appearEffect.FinishEffect();
            _appearEffect = null;
        }
        SetOrderInLayer(0);
        if ( _curAlpha != 1 || !_isOriginalColor )
        {
            _spRenderer.color = new Color(1, 1, 1, 1);
        }
        if ( _scaleX != 1 || _scaleY != 1 )
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

    public override bool GetBulletPara(BulletParaType paraType, out float value)
    {
        float returnValue;
        if ( base.GetBulletPara(paraType, out returnValue) )
        {
            value = returnValue;
            return true;
        }
        switch (paraType)
        {
            case BulletParaType.Alpha:
                value = _curAlpha;
                return true;
            case BulletParaType.ScaleX:
                value = _scaleX;
                return true;
            case BulletParaType.ScaleY:
                value = _scaleY;
                return true;
        }
        value = 0;
        return false;
    }

    public override bool SetBulletPara(BulletParaType paraType, float value)
    {
        if (base.SetBulletPara(paraType, value)) return true;
        switch (paraType)
        {
            case BulletParaType.Alpha:
                SetAlpha(value);
                return true;
            case BulletParaType.ScaleX:
                SetScaleX(value);
                return true;
            case BulletParaType.ScaleY:
                SetScaleY(value);
                return true;
        }
        return false;
    }

    /// <summary>
    /// 设置scaleX
    /// </summary>
    /// <param name="value"></param>
    private void SetScaleX(float value)
    {
        _scaleX = value;
        _collisionHalfWidth = _cfg.collisionRadius * value;
        _isScaleChanged = true;
    }

    /// <summary>
    /// 设置scaleY
    /// </summary>
    /// <param name="value"></param>
    private void SetScaleY(float value)
    {
        _scaleY = value;
        _collisionHalfHeight = _cfg.collisionRadius * value;
        _isScaleChanged = true;
    }
}
