using UnityEngine;
using System.Collections.Generic;

public class ColliderManager 
{
    private static ColliderManager _instance = new ColliderManager();

    public static ColliderManager GetInstance()
    {
        return _instance;
    }

    private List<ObjectColliderBase> _colliderList;
    private int _colliderCount;
    private List<ObjectColliderBase> _fieldList;
    private int _fieldCount;

    public ColliderManager()
    {
        _colliderList = new List<ObjectColliderBase>();
        _colliderCount = 0;
        _fieldList = new List<ObjectColliderBase>();
        _fieldCount = 0;
    }

    public ObjectColliderBase CreateColliderByType(eColliderType type)
    {
        ObjectColliderBase collider = null;
        switch (type)
        {
            case eColliderType.Circle:
                collider =  new ColliderCircle();
                break;
            case eColliderType.Rect:
                collider = new ColliderRect();
                break;
            default:
                Logger.LogError("Collider Type " + type + " is not exist!");
                break;
        }
        _colliderList.Add(collider);
        _colliderCount++;
        return collider;
    }

    public ObjectColliderBase CreateGravitationFieldByType(eColliderType type)
    {
        ObjectColliderBase collider = null;
        switch (type)
        {
            case eColliderType.Circle:
                collider = new GravitationFieldCircle();
                break;
            case eColliderType.Rect:
                collider = new GravitationFieldRect();
                break;
            default:
                Logger.LogError("GravitationField Type " + type + " is not exist!");
                break;
        }
        _colliderList.Add(collider);
        _colliderCount++;
        return collider;
    }

    public void Init()
    {

    }

    public void UpdateFields()
    {
        ObjectColliderBase collider;
        // 列表中至少有一个collider
        bool hasNotNullCollider = false;
        for (int i = 0; i < _fieldCount; i++)
        {
            collider = _fieldList[i];
            if (collider != null)
            {
                if ( collider.ClearFlag != 1 )
                {
                    hasNotNullCollider = true;
                    collider.Update();
                }
                if (collider.ClearFlag == 1)
                {
                    collider.Clear();
                    _fieldList[i] = null;
                }
            }
        }
        if (!hasNotNullCollider)
        {
            _fieldList.Clear();
            _fieldCount = 0;
        }
    }

    public void UpdateColliders()
    {
        ObjectColliderBase collider;
        // 列表中至少有一个collider
        bool hasNotNullCollider = false;
        for (int i=0;i<_colliderCount;i++)
        {
            collider = _colliderList[i];
            if ( collider != null )
            {
                if ( collider.ClearFlag != 1 )
                {
                    hasNotNullCollider = true;
                    collider.Update();
                }
                if ( collider.ClearFlag == 1)
                {
                    collider.Clear();
                    _colliderList[i] = null;
                }
            }
        }
        if ( !hasNotNullCollider )
        {
            _colliderList.Clear();
            _colliderCount = 0;
        }
    }

    /// <summary>
    /// 清除所有的ObjectCollider
    /// </summary>
    /// <param name="exceptList">例外列表，即消除类型在这个列表里面的不会被清除</param>
    public void ClearAllObjectCollider(List<eEliminateDef> exceptList=null)
    {
        if ( exceptList == null )
        {
            exceptList = new List<eEliminateDef>();
        }
        ObjectColliderBase collider;
        int exceptListCount = exceptList.Count;
        int i, j;
        bool isInExceptList;
        for (i=0;i<_colliderCount;i++)
        {
            collider = _colliderList[i];
            if ( collider != null && collider.ClearFlag != 1 )
            {
                isInExceptList = false;
                for (j=0;j<exceptListCount;j++)
                {
                    if ( collider.GetEliminateType() == exceptList[j] )
                    {
                        isInExceptList = true;
                        break;
                    }
                }
                if ( !isInExceptList )
                {
                    collider.Clear();
                    _colliderList[i] = null;
                }
            }
        }
    }

    /// <summary>
    /// 获取所有碰撞组
    /// </summary>
    /// <param name="listCount"></param>
    /// <returns></returns>
    public List<ObjectColliderBase> GetColliderList(out int listCount)
    {
        listCount = _colliderCount;
        return _colliderList;
    }

    public void RemoveGravitationFieldByTag(string tag)
    {
        ObjectColliderBase collider;
        for (int i=0;i<_fieldCount;i++)
        {
            collider = _fieldList[i];
            if ( collider != null && collider.GetTag() == tag )
            {
                collider.ClearSelf();
                return;
            }
        }
    }

    public void RemoveColliderByTag(string tag)
    {
        ObjectColliderBase collider;
        for (int i = 0; i < _colliderCount; i++)
        {
            collider = _colliderList[i];
            if (collider != null && collider.GetTag() == tag)
            {
                collider.ClearSelf();
                return;
            }
        }
    }

    public void Clear()
    {
        _colliderList.Clear();
        _colliderCount = 0;
        _fieldList.Clear();
        _fieldCount = 0;
    }
}
