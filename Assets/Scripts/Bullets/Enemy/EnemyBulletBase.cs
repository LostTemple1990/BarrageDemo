using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase :BulletBase
{
    protected string _prefabName;
    protected List<BulletComponent> _components;
    protected int _componentsCount;

    protected GrazeDetectParas _grazeParas;
    protected CollisionDetectParas _collisionParas;

    public EnemyBulletBase()
    {
        _components = new List<BulletComponent>();
    }

    public override void Init()
    {
        base.Init();
        _componentsCount = 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="rotation">是否根据子弹当前速度方向旋转图像</param>
    public virtual void SetBulletTexture(string texture)
    {

    }

    public virtual T AddComponent<T>()
        where T : BulletComponent, new()
    {
        for (int i=0;i<_componentsCount;i++)
        {
            if ( _components[i].GetType() == typeof(T) )
            {
                return _components[i] as T;
            }
        }
        T component = new T();
        component.Init(this);
        _components.Add(component);
        _componentsCount++;
        return component;
    }

    public virtual T GetComponent<T>()
        where T : BulletComponent
    {
        foreach (BulletComponent bc in _components)
        {
            if ( bc.GetType() == typeof(T) )
            {
                return bc as T;
            }
        }
        return null;
    }

    protected virtual void UpdateComponents()
    {
        for (int i = 0; i < _componentsCount; i++)
        {
            if ( _components[i] != null )
            {
                _components[i].Update();
            }
        }
    }

    public virtual void SetExtraDatas(int type,object[] datas)
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetCollisionDetectParas(CollisionDetectParas paras)
    {
        _collisionParas = paras;
    }

    public virtual void SetGrazeDetectParas(GrazeDetectParas paras)
    {
        _grazeParas = paras;
    }

    public virtual CollisionDetectParas GetCollisionDetectParas()
    {
        throw new System.NotImplementedException();
    }

    public virtual GrazeDetectParas GetGrazeDetectParas()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Eliminate(int duration=0)
    {
        _clearFlag = 1;
    }

    /// <summary>
    /// 是否可以进行碰撞检测
    /// </summary>
    /// <returns></returns>
    public virtual bool CanDetectCollision()
    {
        if ( _clearFlag == 1 )
        {
            return false;
        }
        return true;
    }

    public override void Clear()
    {
        base.Clear();
        for (int i=0;i<_componentsCount;i++)
        {
            _components[i].Clear();
        }
        _components.Clear();
        _componentsCount = 0;
    }
}
