using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase :BulletBase
{
    /// <summary>
    /// bulletId
    /// <para>6位数字</para>
    /// <para>开头为1</para>
    /// <para>1,XX,YY,Z</para>
    /// <para>XX为子弹的大类</para>
    /// <para>YY为子弹的颜色类型</para>
    /// <para>颜色从0~15为别为：</para>
    /// <para>Z为是否高亮</para>
    /// </summary>
    protected string _prefabName;
    protected List<BulletComponent> _components;
    protected int _componentsCount;

    /// <summary>
    /// 标识不会被某些东西消除
    /// </summary>
    protected int _resistEliminateFlag;
    /// <summary>
    /// 是否已经擦过弹
    /// </summary>
    protected bool _isGrazed;
    /// <summary>
    /// 擦弹的冷却时间
    /// <para>用作重复擦弹的判断</para>
    /// <para>用作激光的擦弹判断</para>
    /// </summary>
    protected int _grazeCoolDown;

    public EnemyBulletBase()
    {
        _components = new List<BulletComponent>();
    }

    public override void Init()
    {
        base.Init();
        _componentsCount = 0;
        _detectCollision = true;
        _isGrazed = false;
        _grazeCoolDown = 0;
        _resistEliminateFlag = 0;
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

    /// <summary>
    /// 消除子弹
    /// </summary>
    /// <param name="eliminateType">消除方式</param>
    /// <returns></returns>
    public virtual bool Eliminate(eEliminateDef eliminateType=0)
    {
        if ( ((int)eliminateType & _resistEliminateFlag) != 0 )
        {
            return false;
        }
        _clearFlag = 1;
        return true;
    }

    /// <summary>
    /// 判断是否能被该种类型消除
    /// </summary>
    /// <param name="eliminateType"></param>
    /// <returns></returns>
    public virtual bool CanBeEliminated(eEliminateDef eliminateType)
    {
        return ((int)eliminateType & _resistEliminateFlag) == 0;
    }

    public virtual void SetResistEliminateFlag(int flag)
    {
        _resistEliminateFlag = flag;
    }

    /// <summary>
    /// 更新擦弹冷却时间
    /// </summary>
    protected virtual void UpdateGrazeCoolDown()
    {
        if ( _isGrazed )
        {
            _grazeCoolDown--;
            if ( _grazeCoolDown == 0 )
            {
                _isGrazed = false;
            }
        }
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
        for (int i=0;i<_componentsCount;i++)
        {
            _components[i].Clear();
        }
        _components.Clear();
        _componentsCount = 0;
        base.Clear();
    }
}
