using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgSpriteObject : IPoolClass
{
    private static string PrefabName = "BgSpriteObject";

    protected MovableObject _movableObject;
    protected GameObject _spriteObject;
    protected Transform _spriteTf;
    protected SpriteRenderer _spRenderer;
    protected Vector3 _curPos;
    protected float _leftBorder;
    protected float _rightBorder;
    protected float _bottomBorder;
    protected float _topBorder;

    protected bool _isSelfRotating;
    protected Vector3 _selfRotateAngle;

    protected bool _isActive;

    /// <summary>
    /// 消失的延迟时间
    /// </summary>
    protected int _fadeDelay;
    /// <summary>
    /// 消失总时间
    /// </summary>
    protected int _fadeDuration;
    /// <summary>
    /// 消失已经经过的时间
    /// </summary>
    protected int _fadeTime;
    /// <summary>
    /// 是否正在消失
    /// </summary>
    protected bool _isFading;

    public BgSpriteObject()
    {
        _curPos = new Vector3();
    }

    public void Init(Transform parentTf)
    {
        _leftBorder = Global.GameLBBorderPos.x - 30;
        _bottomBorder = Global.GameLBBorderPos.y - 30;
        _rightBorder = Global.GameRTBorderPos.x + 30;
        _topBorder = Global.GameRTBorderPos.y + 30;
        if ( _movableObject == null )
        {
            _movableObject = new MovableObject();
        }
        if ( _spriteObject == null )
        {
            _spriteObject = ResourceManager.GetInstance().GetPrefab("Prefab/Background", PrefabName);
        }
        _spriteTf = _spriteObject.transform;
        _spRenderer = _spriteTf.GetComponent<SpriteRenderer>();
        // 设置父亲节点
        _spriteTf.parent = parentTf;
        _spriteTf.localScale = Vector3.one;
        _spriteTf.localRotation = Quaternion.Euler(0, 0, 0);
        _isActive = true;
    }

    public void SetSprite(string spriteName)
    {
        //_spRenderer.sprite = ResourceManager.GetInstance().GetResource<Sprite>("Background", spriteName);
        _spRenderer.sprite = Resources.Load<Sprite>("Background/" + spriteName);
    }

    /// <summary>
    /// 设置当前旋转
    /// </summary>
    /// <param name="eulerAngle"></param>
    public void SetRotation(Vector3 eulerAngle)
    {
        _spriteTf.localRotation = Quaternion.Euler(eulerAngle);
    }

    /// <summary>
    /// 设置缩放
    /// </summary>
    /// <param name="scaleX"></param>
    /// <param name="scaleY"></param>
    /// <param name="scaleZ"></param>
    public void SetScale(float scaleX,float scaleY,float scaleZ)
    {
        _spriteTf.localScale = new Vector3(scaleX, scaleY, scaleZ);
    }

    /// <summary>
    /// 设置缩放
    /// </summary>
    /// <param name="scale"></param>
    public void SetScale(Vector3 scale)
    {
        _spriteTf.localScale = scale;
    }

    public void SetSelfRotateAngle(Vector3 rotateAngle)
    {
        _selfRotateAngle = rotateAngle;
        if (_selfRotateAngle != Vector3.zero)
        {
            _isSelfRotating = true;
        }
    }

    public void SetToPos(float posX,float posY)
    {
        _curPos.x = posX;
        _curPos.y = posY;
        _movableObject.SetPos(posX, posY);
    }

    public void SetVelocity(float v,float angle)
    {
        _movableObject.DoStraightMove(v, angle);
    }

    public void SetAcce(float acce,float accAngle,int duration=Consts.MaxDuration)
    {
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, duration);
    }

    public void DoFade(int duration,int delay)
    {
        _isFading = true;
        _fadeTime = 0;
        _fadeDuration = duration;
        _fadeDelay = delay;
    }

    public void Update()
    {
        _movableObject.Update();
        _curPos = _movableObject.GetPos();
        if ( _isSelfRotating )
        {
            RotateSelf();
        }
        if ( _isFading )
        {
            Fade();
        }
        if ( _curPos.x < _leftBorder || _curPos.x > _rightBorder || 
            _curPos.y < _bottomBorder || _curPos.y > _topBorder )
        {
            _isActive = false;
        }
        else
        {
            _spriteTf.localPosition = new Vector3(_curPos.x / 100, _curPos.y / 100);
        }
    }

    protected void RotateSelf()
    {
        _spriteTf.Rotate(_selfRotateAngle);
    }

    protected void Fade()
    {
        _fadeTime++;
        if ( _fadeDelay > 0 )
        {
            if ( _fadeTime >= _fadeDelay )
            {
                _fadeTime -= _fadeDelay;
                _fadeDelay = 0;
            }
        }
        else
        {
            Color color = _spRenderer.color;
            color.a = 1 - (float)_fadeTime / _fadeDuration;
            _spRenderer.color = color;
            // 消失
            if (_fadeTime >= _fadeDuration )
            {
                _isFading = false;
                _isActive = false;
            }
        }
    }

    #region 设置边界
    public void SetLeftBorder(float value)
    {
        _leftBorder = value;
    }

    public void SetRightBorder(float value)
    {
        _rightBorder = value;
    }

    public void SetBottomBorder(float value)
    {
        _bottomBorder = value;
    }

    public void SetTopBorder(float value)
    {
        _topBorder = value;
    }
    #endregion

    public void Clear()
    {
        _spriteTf.localPosition = new Vector3(2000, 2000, 0);
        _spriteTf = null;
        // 恢复透明度
        _spRenderer.sprite = null;
        Color color = _spRenderer.color;
        color.a = 1;
        _spRenderer.color = color;
        // 回收到对象池
        ObjectsPool.GetInstance().RestorePrefabToPool(PrefabName, _spriteObject);
        _spriteObject = null;
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
        _isActive = false;
    }

    public bool IsActive
    {
        get { return _isActive; }
    }
}
