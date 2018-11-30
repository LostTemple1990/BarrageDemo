using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniLua;

public class BCColliderTrigger : BulletComponent
{
    private EnemyBulletBase _bullet;
    /// <summary>
    /// 会触发触发器的碰撞类型
    /// </summary>
    private int _triggerType;
    /// <summary>
    /// 触发函数
    /// </summary>
    private int _triggerFuncRef;

    public override void Init(EnemyBulletBase bullet)
    {
        _bullet = bullet;
    }

    public void Register(int triggerType,int triggerFuncRef)
    {
        _triggerType = triggerType;
        _triggerFuncRef = triggerFuncRef;
    }

    public override void Update()
    {
        if ( _triggerType != 0 )
        {
            int listCount;
            List<ObjectColliderBase> colliderList = ColliderManager.GetInstance().GetColliderList(out listCount);
            ObjectColliderBase collider;
            for (int i=0;i<listCount;i++)
            {
                collider = colliderList[i];
                // 非空，切满足触发条件
                if ( collider != null && ((int)collider.GetEliminateType() | _triggerType) != 0)
                {
                    int nextColliderIndex = 0;
                    int curColliderIndex;
                    do
                    {
                        CollisionDetectParas collParas = _bullet.GetCollisionDetectParas(nextColliderIndex);
                        curColliderIndex = nextColliderIndex;
                        nextColliderIndex = collParas.nextIndex;
                        if (collider.DetectCollisionWithCollisionParas(collParas))
                        {
                            //InterpreterManager.GetInstance().AddPara(_bullet, LuaParaType.LightUserData);
                            InterpreterManager.GetInstance().AddPara(collider, LuaParaType.LightUserData);
                            InterpreterManager.GetInstance().AddPara(curColliderIndex, LuaParaType.Int);
                            InterpreterManager.GetInstance().CallLuaFunction(_triggerFuncRef, 2, 0);
                        }
                    } while (nextColliderIndex != -1);
                }
            }
        }
    }

    public override void Clear()
    {
        _bullet = null;
        _triggerType = 0;
        if ( _triggerFuncRef != 0 )
        {
            InterpreterManager.GetInstance().UnrefLuaFunction(_triggerFuncRef);
        }
    }
}