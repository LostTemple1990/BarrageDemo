using UnityEngine;
using System.Collections;

public class EnemyObjectBase
{
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

    }

    public virtual void Update()
    {

    }

    public virtual void SetEnemyAni(string aniId)
    {

    }

    public virtual void DoAction(AniActionType actType,int dir,int duration=Consts.MaxDuration)
    {

    }

    public virtual void SetToPosition(float posX,float posY)
    {

    }

    public virtual void SetToPosition(Vector2 pos)
    {

    }

    public virtual void Clear()
    {

    }
}
