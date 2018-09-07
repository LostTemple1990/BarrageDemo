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

    public ColliderManager()
    {
        _colliderList = new List<ObjectColliderBase>();
        _colliderCount = 0;
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
                collider = new  ColliderRect();
                break;
            default:
                Logger.LogError("Collider Type " + type + " is not exist!");
                break;
        }
        _colliderList.Add(collider);
        _colliderCount++;
        return collider;
    }

    public void Init()
    {

    }

    public void Update()
    {
        ObjectColliderBase collider;
        for (int i=0;i<_colliderCount;i++)
        {
            collider = _colliderList[i];
            if ( collider != null )
            {
                collider.Update();
                if ( collider.ClearFlag == 1)
                {
                    collider.Clear();
                    _colliderList[i] = null;
                }
            }
        }
    }

    public void Clear()
    {
        _colliderList.Clear();
        _colliderCount = 0;
    }
}
