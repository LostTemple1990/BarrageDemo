using System.Collections.Generic;

public class BCColliderTrigger : BulletComponent
{
    private struct TriggerData
    {
        public int triggerType;
        public int triggerFuncRef;
    }

    private EnemyBulletBase _bullet;
    /// <summary>
    /// 触发器数据
    /// </summary>
    private List<TriggerData> _triggerDatas;
    /// <summary>
    /// 触发器数目
    /// </summary>
    private int _triggerDataCount;

    public override void Init(EnemyBulletBase bullet)
    {
        _bullet = bullet;
        _triggerDatas = new List<TriggerData>();
        _triggerDataCount = 0;
    }

    public void Register(int triggerType,int triggerFuncRef)
    {
        //for (int i=0;i<_triggerDataCount;i++)
        //{
        //    TriggerData data = _triggerDatas[i];
        //    if ( data.triggerType == triggerType )
        //    {
        //        Logger.LogWarn("Trigger of type " + triggerType + " of this bullet is already exist!Please check lua");
        //        if ( data.triggerFuncRef != 0 )
        //        {
        //            InterpreterManager.GetInstance().UnrefLuaFunction(data.triggerFuncRef);
        //        }
        //        data.triggerFuncRef = triggerFuncRef;
        //        _triggerDatas[i] = data;
        //        return;
        //    }
        //}
        // 暂定：碰撞类型可以重复
        TriggerData newData = new TriggerData
        {
            triggerType = triggerType,
            triggerFuncRef = triggerFuncRef,
        };
        _triggerDatas.Add(newData);
        _triggerDataCount++;
    }

    public override void Update()
    {
        for (int i=0;i<_triggerDataCount;i++)
        {
            TriggerData data = _triggerDatas[i];
            if (data.triggerType == 0) continue;
            int listCount;
            List<ObjectColliderBase> colliderList = ColliderManager.GetInstance().GetColliderList(out listCount);
            ObjectColliderBase collider;
            for (int j=0;j<listCount;j++)
            {
                collider = colliderList[j];
                // 非空，且满足触发条件
                if ( collider != null && ((int)collider.GetEliminateType() & data.triggerType) != 0)
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
                            InterpreterManager.GetInstance().CallLuaFunction(data.triggerFuncRef, 2, 0);
                        }
                    } while (nextColliderIndex != -1);
                }
            }
        }
    }

    public override void Clear()
    {
        _bullet = null;
        for (int i=0;i<_triggerDataCount;i++)
        {
            int funcRef = _triggerDatas[i].triggerFuncRef;
            if (funcRef != 0 )
            {
                InterpreterManager.GetInstance().UnrefLuaFunction(funcRef);
            }
        }
        _triggerDatas.Clear();
        _triggerDataCount = 0;
    }
}