using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBulletBase : BulletBase
{
    protected GameObject _bullet;
    protected Transform _trans;
    protected SpriteRenderer _renderer;
    protected string _prefabName;
    /// <summary>
    /// 子机子弹配置
    /// </summary>
    protected PlayerBulletCfg _bulletCfg;
    /// <summary>
    /// 子弹击中对象的位置
    /// </summary>
    protected Vector2 _hitPos;

    public override void Init()
    {
        base.Init();
        _detectCollision = true;
    }


    public override void Update()
    {
        base.Update();
    }

    public virtual void ChangeStyleById(string id)
    {

    }

    protected virtual void RenderPosition()
    {
        _trans.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
    }

    public override void Clear()
    {
        _destroyFlag = 0;
        _clearFlag = 0;
        _trans = null;
        _bullet = null;
        _renderer = null;
        base.Clear();
    }

    public virtual void Eliminate()
    {
        _clearFlag = 1;
    }

    public BulletType id
    {
        set { _type = value; }
    }

    /// <summary>
    /// 子弹对应的伤害
    /// </summary>
    /// <returns></returns>
    protected virtual float GetDamage()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 创建自机子弹的击中特效
    /// </summary>
    /// <param name="effectParas">击中特效配置参数</param>
    protected virtual void CreateHitEffect(PlayerBulletHitEffectParas effectParas)
    {
        STGSpriteEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.SpriteEffect) as STGSpriteEffect;
        effect.SetSprite(effectParas.atlasName, effectParas.spriteName, effectParas.blendMode, effectParas.layerId, true);
        List<float[]> parasList = effectParas.effectParasList;
        int listCount = parasList.Count;
        for (int i=0;i<listCount;i++)
        {
            SetSpriteEffectParas(effect, parasList[i]);
        }
    }

    #region 根据配置设置SpriteEffect的属性
    protected void SetSpriteEffectParas(STGSpriteEffect effect,float[] paras)
    {
        if (paras.Length == 0) return;
        switch ((int)paras[0])
        {
            // 设置位置，参数为增量
            case 1:
                {
                    float dx = paras[1];
                    float offsetX = paras[2];
                    float dy = paras[3];
                    float offsetY = paras[4];
                    if (offsetX != 0) dx += Random.Range(-offsetX, offsetX);
                    if (offsetY != 0) dy += Random.Range(-offsetY, offsetY);
                    effect.SetPosition(_hitPos.x + dx, _hitPos.y + dy);
                    break;
                }
            // 设置位置，参数为绝对位置
            case 2:
                {
                    float posX = paras[1];
                    float offsetX = paras[2];
                    float posY = paras[3];
                    float offsetY = paras[4];
                    if (offsetX != 0) posX += Random.Range(-offsetX, offsetX);
                    if (offsetY != 0) posY += Random.Range(-offsetY, offsetY);
                    effect.SetPosition(posX, posY);
                    break;
                }
            // 设置速度，v,dAngle,acce
            // 即速度，与当前速度方向的差值，加速度
            // dAngle为0则表示当前速度方向
            case 3:
                {
                    float velocity = paras[1];
                    float angle = _curRotation + paras[2];
                    float acce = paras[3];
                    effect.DoStraightMove(velocity, angle);
                    effect.DoAcceleration(acce, angle);
                    break;
                }
            // 设置速度，v,angle,acce
            // 即速度，速度方向，加速度
            case 4:
                {
                    float velocity = paras[1];
                    float angle = paras[2];
                    float acce = paras[3];
                    effect.DoStraightMove(velocity, angle);
                    effect.DoAcceleration(acce, angle);
                    break;
                }
            // 图像旋转角度
            // dRotationAngle,与当前子弹速度方向的差值
            case 5:
                {
                    float angle = _curRotation + paras[1];
                    effect.SetRotation(angle);
                    break;
                }
            // 图像旋转角度
            // angle,指定角度
            case 6:
                {
                    float angle = paras[1];
                    effect.SetRotation(angle);
                    break;
                }
            // orderInLayer
            case 9:
                {
                    int orderInLayer = (int)paras[1];
                    effect.SetOrderInLayer(orderInLayer);
                    break;
                }
            // 设置alpha
            case 10:
                {
                    float alpha = paras[1];
                    effect.SetSpriteAlpha(alpha);
                    break;
                }
            // 透明度渐变
            // toAlpha,duration
            case 11:
                {
                    float toAlpha = paras[1];
                    int duration = (int)paras[2];
                    effect.DoTweenAlpha(toAlpha, duration);
                    break;
                }
            // 设置颜色
            case 12:
                {
                    float r = paras[1];
                    float g = paras[2];
                    float b = paras[3];
                    effect.SetSpriteColor(r, g, b);
                    break;
                }
            // 设置带alpha的颜色
            case 13:
                {
                    float r = paras[1];
                    float g = paras[2];
                    float b = paras[3];
                    float aValue = paras[4];
                    effect.SetSpriteColor(r, g, b, aValue);
                    break;
                }
            // 设置缩放
            // scaleX,scaleY
            case 15:
                {
                    float scaleX = paras[1];
                    float scaleY = paras[2];
                    effect.SetScale(scaleX, scaleY);
                    break;
                }
            // 设置scaleX缩放动画
            // toScaleX duration
            case 16:
                {
                    float toScaleX = paras[1];
                    int duration = (int)paras[2];
                    effect.ChangeWidthTo(toScaleX, duration, InterpolationMode.Linear);
                    break;
                }
            // 设置scaleY缩放动画
            // toScaleY duration
            case 17:
                {
                    float toScaleY = paras[1];
                    int duration = (int)paras[2];
                    effect.ChangeHeightTo(toScaleY, duration, InterpolationMode.Linear);
                    break;
                }
            // 持续时间
            case 30:
                {
                    int duration = (int)paras[1];
                    effect.SetExistDuration(duration);
                    break;
                }
        }
    }
    #endregion
}

public class PlayerBulletHitEffectParas
{
    /// <summary>
    /// 特效的图集名称
    /// </summary>
    public string atlasName;
    /// <summary>
    /// 特效精灵名称
    /// </summary>
    public string spriteName;
    /// <summary>
    /// 混合模式
    /// </summary>
    public eBlendMode blendMode;
    /// <summary>
    /// 所在层级
    /// </summary>
    public LayerId layerId;
    /// <summary>
    /// 参数列表
    /// </summary>
    public List<float[]> effectParasList;

    public void Init(string parasStr)
    {
        effectParasList = new List<float[]>();
        if (parasStr == "") return;
        string[] tmpStrs0 = parasStr.Split(';');
        // 基础参数
        string[] tmpStrs1 = tmpStrs0[0].Split(',');
        atlasName = tmpStrs1[0];
        spriteName = tmpStrs1[1];
        blendMode = (eBlendMode)int.Parse(tmpStrs1[2]);
        int tmpInt = int.Parse(tmpStrs1[3]);
        layerId = tmpInt == -1 ? LayerId.STGNormalEffect : (LayerId)tmpInt;
        // 参数列表
        int paraListLen = tmpStrs0.Length - 1;
        for (int i = 0; i < paraListLen; i++)
        {
            string[] paraStr = tmpStrs0[i + 1].Split(',');
            int len = paraStr.Length;
            float[] paraIntArr = new float[len];
            for (int j = 0; j < len; j++)
            {
                paraIntArr[j] = float.Parse(paraStr[j]);
            }
            effectParasList.Add(paraIntArr);
        }
    }
}
