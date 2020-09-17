using UnityEngine;
using System.Collections;

public class EnemyObjectBase
{
    /// <summary>
    /// 颜色是否改变
    /// </summary>
    protected bool _isColorChanged;
    /// <summary>
    /// 当前颜色
    /// </summary>
    protected Color _curColor;
    /// <summary>
    /// 缩放是否改变
    /// </summary>
    protected bool _isScaleChanged;
    /// <summary>
    /// 当前缩放
    /// </summary>
    protected Vector3 _curScale;

    public virtual eEnemyObjectType GetObjectType()
    {
        throw new System.NotImplementedException();
    }

    public virtual GameObject GetObject()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Init()
    {
        _isColorChanged = false;
        _isScaleChanged = false;
    }

    public virtual void Update()
    {

    }

    public virtual void SetEnemyAni(string aniId)
    {

    }

    public virtual void DoAction(AniActionType actType,int dir=Consts.DIR_NULL,int duration=Consts.MaxDuration)
    {

    }

    public virtual void SetToPosition(float posX,float posY)
    {

    }

    public virtual void SetToPosition(Vector2 pos)
    {

    }

    public virtual void SetColor(float r,float g,float b,float a)
    {

    }

    public virtual void SetScale(float scaleX,float scaleY)
    {

    }

    public virtual void Clear()
    {

    }
}
