using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletSimple : PlayerBulletBase
{
    private MovableObject _movableObject;

    protected bool _isRotatedByVAngle;
    protected float _selfRotationAngle;

    protected float _collisionRadius;

    protected bool _isEliminating;
    protected int _eliminatingTime;
    protected int _eliminatingDuration;
    protected Color _bulletColor;

    public PlayerBulletSimple()
    {
        _type = BulletType.Player_Simple;
    }

    public override void Init()
    {
        base.Init();
        _isEliminating = false;
        BulletsManager.GetInstance().RegisterPlayerBullet(this);
        _movableObject = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
    }

    public override void ChangeStyleById(string id)
    {
        if ( _bullet != null )
        {
            UIManager.GetInstance().HideGo(_bullet);
            ObjectsPool.GetInstance().RestorePrefabToPool(_bulletCfg.textureName, _bullet);
        }
        _bulletCfg = BulletsManager.GetInstance().GetPlayerBulletCfgById(id);
        _prefabName = id;
        _bullet = BulletsManager.GetInstance().CreateBulletGameObject(BulletType.Player_Simple, id);
        _trans = _bullet.transform;
        _renderer = _trans.Find("BulletSprite").GetComponent<SpriteRenderer>();
        _trans.localPosition = _curPos;
        _isRotatedByVAngle = _bulletCfg.isRotatedByVAngle;
        _selfRotationAngle = _bulletCfg.selfRotationAngle;
        // 碰撞相关参数
        _collisionRadius = _bulletCfg.collisionRadius;
    }

    public override void Update()
    {
        base.Update();
        if ( !_isEliminating )
        {
            _lastPos = _curPos;
            _movableObject.Update();
            _curPos = _movableObject.GetPos();
            CheckRotated();
            CheckHitEnemy();
            if (IsOutOfBorder())
            {
                _clearFlag = 1;
            }
            else
            {
                UpdatePos();
            }
        }
        else
        {
            UpdateEliminating();
        }
    }

    public override void SetToPosition(Vector2 pos)
    {
        _curPos.x = pos.x;
        _curPos.y = pos.y;
        _movableObject.SetPos(_curPos.x, _curPos.y);
    }

    public override void SetToPosition(float posX, float posY)
    {
        _curPos.x = posX;
        _curPos.y = posY;
        _movableObject.SetPos(_curPos.x, _curPos.y);
    }

    /// <summary>
    /// 匀速直线运动
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    public void DoMoveStraight(float v,float angle)
    {
        _movableObject.DoMoveStraight(v, angle);
    }

    /// <summary>
    /// 加速运动
    /// </summary>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public void DoAcceleration(float acce,float accAngle)
    {
        _movableObject.DoAcceleration(acce, accAngle);
    }

    /// <summary>
    /// 有限制的加速运动
    /// </summary>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    /// <param name="accDuration"></param>
    public void DoAccelerationWithLimitation(float acce, float accAngle,float maxVelocity)
    {
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
    }

    protected void CheckRotated()
    {
        if ( _selfRotationAngle != 0 )
        {
            _trans.Rotate(new Vector3(0, 0, _bulletCfg.selfRotationAngle));
        }
        else if ( _isRotatedByVAngle )
        {
            Vector3 dv = _curPos - _lastPos;
            float rotateAngle = MathUtil.GetAngleBetweenXAxis(dv.x, dv.y, false);
            _trans.localRotation = Quaternion.Euler(new Vector3(0, 0, rotateAngle));
        }
    }

    public void CheckHitEnemy()
    {
        List<EnemyBase> enemyList = EnemyManager.GetInstance().GetEnemyList();
        int enemyCount = enemyList.Count;
        EnemyBase enemy;
        for (int i=0;i<enemyCount;i++)
        {
            enemy = enemyList[i];
            // 自机子弹与敌机使用方形检测
            if ( enemy != null && enemy.CanHit() )
            {
                CollisionDetectParas paras = enemy.GetCollisionDetectParas();
                if ( Mathf.Abs(_curPos.x-paras.centerPos.x) <= _collisionRadius + paras.halfWidth &&
                    Mathf.Abs(_curPos.y - paras.centerPos.y) <= _collisionRadius + paras.halfHeight )
                {
                    enemy.TakeDamage(GetDamage());
                    BeginEliminating();
                }
            }
        }
    }

    public override bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        return true;
    }

    public override CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        return new CollisionDetectParas
        {
            type = CollisionDetectType.Circle,
            centerPos = _curPos,
            radius = _collisionRadius,
            nextIndex = -1,
        };
    }

    public override void CollidedByObject(int n = 0, eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {
        BeginEliminating();
    }

    /// <summary>
    /// 开始消弹
    /// </summary>
    protected virtual void BeginEliminating()
    {
        _detectCollision = false;
        if ( _bulletCfg.eliminatedEffectType == 1 )
        {
            string[] paras = _bulletCfg.eliminatedEffectParas;
            string atlasName = paras[0];
            string spriteName = paras[1];
            eBlendMode blendMode = (eBlendMode)int.Parse(paras[2]);
            int tmpInt = int.Parse(paras[3]);
            LayerId layerId = tmpInt == -1 ? LayerId.STGNormalEffect : (LayerId)tmpInt;
            STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
            effect.SetSprite(atlasName, spriteName, blendMode, layerId, true);
            SetSpriteEffectParas(effect, paras);
            _clearFlag = 1;
        }
    }

    #region 根据配置设置SpriteEffect的属性
    protected virtual void SetSpriteEffectParas(STGSpriteEffect effect,string[] paras)
    {
        // 根据配置设置SpriteEffect的属性
        int parasLen = paras.Length;
        for (int i = 4; i < parasLen;)
        {
            // 设置位置，参数为增量
            if (paras[i] == "1")
            {
                float dx = float.Parse(paras[i + 1]);
                float dy = float.Parse(paras[i + 2]);
                effect.SetToPos(_curPos.x + dx, _curPos.y + dy);
                i += 3;
            }
            // 设置位置，参数为绝对位置
            else if (paras[i] == "2")
            {
                float posX = float.Parse(paras[i + 1]);
                float posY = float.Parse(paras[i + 2]);
                effect.SetToPos(posX, posY);
                i += 3;
            }
            // 设置速度，v,dAngle,acce
            // 即速度，与当前速度方向的差值，加速度
            // dAngle为0则表示当前速度方向
            else if (paras[i] == "3")
            {
                float velocity = float.Parse(paras[i + 1]);
                float angle = _movableObject.GetVAngle() + float.Parse(paras[i + 2]);
                float acce = float.Parse(paras[i + 3]);
                effect.DoMove(velocity, angle, acce);
                i += 4;
            }
            // 设置速度，v,angle,acce
            // 即速度，速度方向，加速度
            else if (paras[i] == "4")
            {
                float velocity = float.Parse(paras[i + 1]);
                float angle = float.Parse(paras[i + 2]);
                float acce = float.Parse(paras[i + 3]);
                effect.DoMove(velocity, angle, acce);
                i += 4;
            }
            // 图像旋转角度
            // dRotationAngle,与当前子弹速度方向的差值
            else if (paras[i] == "5")
            {
                float angle = _movableObject.GetVAngle() + float.Parse(paras[i + 1]);
                effect.SetRotation(angle);
                i += 2;
            }
            // 图像旋转角度
            // angle,指定角度
            else if (paras[i] == "6")
            {
                float angle = float.Parse(paras[i + 1]);
                effect.SetRotation(angle);
                i += 2;
            }
            // orderInLayer
            else if (paras[i] == "9")
            {
                int orderInLayer = int.Parse(paras[i + 1]);
                effect.SetOrderInLayer(orderInLayer);
                i += 2;
            }
            // 设置alpha
            else if (paras[i] == "10")
            {
                float alpha = float.Parse(paras[i + 1]);
                effect.SetSpritAlpha(alpha);
                i += 2;
            }
            // 透明度渐变
            // toAlpha,duration
            else if (paras[i] == "11")
            {
                float toAlpha = float.Parse(paras[i + 1]);
                int duration = int.Parse(paras[i + 2]);
                effect.DoTweenAlpha(toAlpha, duration);
                i += 3;
            }
            // 设置缩放
            // scaleX,scaleY
            else if (paras[i] == "15")
            {
                float scaleX = float.Parse(paras[i + 1]);
                float scaleY = float.Parse(paras[i + 2]);
                effect.SetScale(scaleX, scaleY);
                i += 3;
            }
            // 设置scaleX缩放动画
            // toScaleX duration
            else if (paras[i] == "16")
            {
                float toScaleX = float.Parse(paras[i + 1]);
                int duration = int.Parse(paras[i + 2]);
                effect.DoScaleWidth(toScaleX, duration, InterpolationMode.Linear);
                i += 3;
            }
            // 设置scaleY缩放动画
            // toScaleY duration
            else if (paras[i] == "17")
            {
                float toScaleY = float.Parse(paras[i + 1]);
                int duration = int.Parse(paras[i + 2]);
                effect.DoScaleHeight(toScaleY, duration, InterpolationMode.Linear);
                i += 3;
            }
            // 持续时间
            else if (paras[i] == "30")
            {
                int duration = int.Parse(paras[i + 1]);
                effect.SetExistDuration(duration);
                i += 2;
            }
        }
    }
    #endregion

    /// <summary>
    /// 消弹动画
    /// </summary>
    protected virtual void UpdateEliminating()
    {
        if ( _eliminatingTime < _eliminatingDuration )
        {
            float alhpa = 1 - (float)_eliminatingTime / _eliminatingDuration;
            _bulletColor.a = alhpa;
            _renderer.color = _bulletColor;
            _eliminatingTime++;

        }
        else
        {
            _isEliminating = false;
            _clearFlag = 1;
        }
    }

    public override bool DetectCollision()
    {
        return _detectCollision && !_isEliminating;
    }

    protected override int GetDamage()
    {
        return 2;
    }

    public override void Clear()
    {
        _renderer.color = new Color(1, 1, 1, 1);
        ObjectsPool.GetInstance().RestorePrefabToPool(_prefabName, _bullet);
        _bullet = null;
        _bulletCfg = null;
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
        base.Clear();
    }
}
