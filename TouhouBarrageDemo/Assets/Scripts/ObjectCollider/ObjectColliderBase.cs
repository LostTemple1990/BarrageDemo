﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class ObjectColliderBase : IAttachment, IAttachable, IObjectCollider, ISTGMovable, ITaskExecuter, ICollisionObject
{
    protected Vector2 _curPos;

    protected int _colliderGroups;

    protected int _existTime;
    protected int _existDuration;

    /// <summary>
    /// 是否清除
    /// </summary>
    protected int _clearFlag;
    /// <summary>
    /// 是否正在缩放
    /// </summary>
    protected bool _isScaling;
    protected int _scaleTime;
    protected int _scaleDuration;

    protected eColliderType _type;
    /// <summary>
    /// 与物体碰撞的消除方式
    /// </summary>
    protected eEliminateDef _eliminateType;
    /// <summary>
    /// 与敌机发生碰撞时对敌机造成的伤害
    /// </summary>
    protected float _hitEnemyDamage;

    /// <summary>
    /// 附件物体的列表
    /// </summary>
    protected List<IAttachment> _attachmentsList;
    /// <summary>
    /// 依附物体的个数
    /// </summary>
    protected int _attachmentsCount;
    protected IAttachable _master;
    /// <summary>
    /// 标识是否随着master被销毁一同消失
    /// </summary>
    protected bool _isEliminatedWithMaster;
    /// <summary>
    /// 与master的相对位置
    /// </summary>
    protected Vector2 _relativePosToMaster;
    /// <summary>
    /// 是否连续跟随master
    /// </summary>
    protected bool _isFollowingMasterContinuously;
    /// <summary>
    /// 移动对象
    /// </summary>
    protected MovableObject _movableObject;
    /// <summary>
    /// 标签
    /// </summary>
    protected string _tag;


    protected List<Task> _taskList;
    protected int _taskCount;

    protected Action<ObjectColliderBase, ICollisionObject> _collidedByPlayer;
    protected Action<ObjectColliderBase, ICollisionObject> _collidedByPlayerBullet;
    protected Action<ObjectColliderBase, ICollisionObject> _collidedByEnemy;
    protected Action<ObjectColliderBase, ICollisionObject> _collidedByEnemyBullet;

    public ObjectColliderBase()
    {
        _clearFlag = 0;
        _existDuration = 0;
        _hitEnemyDamage = 0;
        _eliminateType = eEliminateDef.HitObjectCollider;
        _isScaling = false;
        _movableObject = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        _tag = "";
        _taskList = new List<Task>();
        _taskCount = 0;
        _attachmentsList = new List<IAttachment>();
        _attachmentsCount = 0;
    }

    public void SetTag(string tag)
    {
        _tag = tag;
    }

    public string GetTag()
    {
        return _tag;
    }

    public void SetPosition(float posX,float posY)
    {
        _curPos = new Vector2(posX, posY);
        _movableObject.SetPos(posX, posY);
    }

    public void SetPosition(Vector2 pos)
    {
        _curPos = pos;
        _movableObject.SetPos(pos.x, pos.y);
    }

    public Vector2 GetPosition()
    {
        return _curPos;
    }

    public virtual float GetRotation()
    {
        return 0;
    }

    public virtual void SetRotation(float value)
    {

    }

    public virtual void SetSize(float arg0,float arg1)
    {

    }

    /// <summary>
    /// 设置与物体碰撞器产生碰撞的碰撞组
    /// 类型 eColliderGroup
    /// </summary>
    /// <param name="groups"></param>
    public void SetColliderGroup(eColliderGroup groups)
    {
        _colliderGroups = (int)groups;
    }

    public eColliderGroup GetColliderGroup()
    {
        return (eColliderGroup)_colliderGroups;
    }

    /// <summary>
    /// 缩放至指定的尺寸
    /// </summary>
    /// <param name="toArg0"></param>
    /// <param name="toArg1"></param>
    /// <param name="duration"></param>
    public virtual void ScaleToSize(float toArg0,float toArg1,int duration)
    {
        _isScaling = true;
        _scaleTime = 0;
        _scaleDuration = duration;
    }

    /// <summary>
    /// 设置存在时间
    /// </summary>
    /// <param name="existDuration"></param>
    public void SetExistDuration(int existDuration)
    {
        if (existDuration < 0)
            return;
        _existTime = 0;
        _existDuration = existDuration;
    }

    public void Update()
    {
        UpdateTasks();
        if (_isScaling) Scale();
        if ( _isFollowingMasterContinuously && _master != null )
        {
            SetPosition(_relativePosToMaster + _master.GetPosition());
        }
        else
        {
            if ( _movableObject.IsActive() )
            {
                _movableObject.Update();
                _curPos = _movableObject.GetPos();
            }
        }
        if ((_colliderGroups & (int)eColliderGroup.Player) != 0)
        {
            CheckCollisionWithPlayer();
        }
        if ((_colliderGroups & (int)eColliderGroup.PlayerBullet) != 0)
        {
            CheckCollisionWithPlayerBullet();
        }
        if ((_colliderGroups & (int)eColliderGroup.Enemy) != 0 || (_colliderGroups & (int)eColliderGroup.Boss) != 0)
        {
            CheckCollisionWithEnemy();
        }
        if ((_colliderGroups & (int)eColliderGroup.EnemyBullet) != 0)
        {
            CheckCollisionWithEnemyBullet();
        }
        if ((_colliderGroups & (int)eColliderGroup.Item) != 0)
        {
            CheckCollisionWithItem();
        }
        if ( _existDuration > 0 )
        {
            _existTime++;
            if ( _existTime >= _existDuration )
            {
                _clearFlag = 1;
            }
        }
    }

    protected virtual void CheckCollisionWithPlayer()
    {

    }

    protected virtual void CheckCollisionWithPlayerBullet()
    {

    }

    protected virtual void CheckCollisionWithEnemy()
    {

    }

    protected virtual void CheckCollisionWithEnemyBullet()
    {

    }

    protected virtual void CheckCollisionWithItem()
    {

    }

    public virtual bool DetectCollisionWithCollisionParas(CollisionDetectParas collParas)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 设置ObjectCollider的消除类型
    /// 自机符卡/碰撞物体
    /// </summary>
    /// <param name="eliminateType"></param>
    public void SetEliminateType(eEliminateDef eliminateType)
    {
        _eliminateType = eliminateType;
    }

    /// <summary>
    /// 获取ObjectCollider的消除类型
    /// </summary>
    /// <returns></returns>
    public eEliminateDef GetEliminateType()
    {
        return _eliminateType;
    }

    /// <summary>
    /// 设置ObjectCollider击中敌机时造成的伤害
    /// </summary>
    /// <param name="damage"></param>
    public void SetHitEnemyDamage(float damage)
    {
        _hitEnemyDamage = damage;
    }

    /// <summary>
    /// 执行缩放
    /// </summary>
    protected virtual void Scale()
    {

    }

    public void ClearSelf()
    {
        _clearFlag = 1;
    }

    public int ClearFlag
    {
        get { return _clearFlag; }
    }

    public void AttachTo(IAttachable master, bool eliminatedWithMaster)
    {
        if (_master != null) return;
        _master = master;
        _master.AddAttachment(this);
        _isEliminatedWithMaster = eliminatedWithMaster;
    }

    public void SetRelativePos(float offsetX, float offsetY, float rotation, bool followMasterRotation,bool isFollowingMasterContinuously)
    {
        _relativePosToMaster = new Vector2(offsetX, offsetY);
        _isFollowingMasterContinuously = isFollowingMasterContinuously;
        if ( _master != null )
        {
            SetPosition(_master.GetPosition() + _relativePosToMaster);
        }
    }

    public void OnMasterEliminated(eEliminateDef eliminateType)
    {
        _master = null;
        _clearFlag = 1;
    }

    public void AddAttachment(IAttachment attachment)
    {
        for (int i = 0; i < _attachmentsCount; i++)
        {
            if (_attachmentsList[i] == attachment)
            {
                return;
            }
        }
        _attachmentsList.Add(attachment);
        _attachmentsCount++;
    }

    public void OnAttachmentEliminated(IAttachment attachment)
    {
        for (int i = 0; i < _attachmentsCount; i++)
        {
            if (_attachmentsList[i] == attachment)
            {
                _attachmentsList[i] = null;
            }
        }
    }

    #region ISTGMovable
    public void DoStraightMove(float v, float angle)
    {
        _movableObject.DoStraightMove(v, angle);
    }

    public void MoveTo(float endX, float endY, int duration, InterpolationMode intMode)
    {
        _movableObject.MoveTo(endX, endY, duration, intMode);
    }

    public void MoveTowards(float v, float angle, int duration)
    {
        _movableObject.MoveTowards(v, angle, duration);
    }

    public void DoAcceleration(float acce, float accAngle)
    {
        _movableObject.DoAcceleration(acce, accAngle);
    }

    public void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity)
    {
        _movableObject.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
    }

    public void DoMoveTo(float endX, float endY, int duration, InterpolationMode mode)
    {
        _movableObject.MoveTo(endX, endY, duration, mode);
    }

    public void SetStraightParas(float v, float vAngle, float acce, float accAngle)
    {
        _movableObject.SetStraightParas(v, vAngle, acce, accAngle);
    }

    public void SetPolarParas(float radius, float angle, float deltaR, float omega)
    {
        _movableObject.SetPolarParas(radius, angle, deltaR, omega);
    }

    public void SetPolarParas(float radius, float angle, float deltaR, float omega, float centerPosX, float centerPosY)
    {
        _movableObject.SetPolarParas(radius, angle, deltaR, omega, centerPosX, centerPosY);
    }

    public float velocity
    {
        get { return _movableObject.velocity; }
        set
        {
            _movableObject.velocity = value;
        }
    }

    /// <summary>
    /// x轴方向的速度
    /// </summary>
    public float vx
    {
        get { return _movableObject.vx; }
        set
        {
            _movableObject.vx = value;
        }
    }

    /// <summary>
    /// y轴方向的速度
    /// </summary>
    public float vy
    {
        get { return _movableObject.vy; }
        set
        {
            _movableObject.vy = value;
        }
    }

    public float vAngle
    {
        get { return _movableObject.vAngle; }
        set
        {
            _movableObject.vAngle = value;
        }
    }

    public float acce
    {
        get { return _movableObject.acce; }
        set
        {
            _movableObject.acce = value;
        }
    }

    public float accAngle
    {
        get { return _movableObject.accAngle; }
        set
        {
            _movableObject.accAngle = value;
        }
    }

    public float dx
    {
        get { return _movableObject.dx; }
    }

    public float dy
    {
        get { return _movableObject.dy; }
    }

    public float maxVelocity
    {
        get { return _movableObject.maxVelocity; }
        set
        {
            _movableObject.maxVelocity = value;
        }
    }

    public bool Eliminate(eEliminateDef eliminateType = 0)
    {
        ClearSelf();
        return true;
    }

    #endregion

    #region task
    public void AddTask(Task task)
    {
        _taskList.Add(task);
        _taskCount++;
    }

    private void UpdateTasks()
    {
        Task task;
        for (int i = 0; i < _taskCount; i++)
        {
            task = _taskList[i];
            if (task != null)
            {
                task.Update();
                if (task.isFinish)
                {
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
                    _taskList[i] = null;
                }
            }
        }
    }

    private void ClearTasks()
    {
        Task task;
        for (int i = 0; i < _taskCount; i++)
        {
            task = _taskList[i];
            if (task != null)
            {
                if (task.luaState != null)
                {
                    InterpreterManager.GetInstance().StopTaskThread(task.luaState, task.funcRef);
                }
                ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
            }
        }
        _taskList.Clear();
        _taskCount = 0;
    }
    #endregion

    public void RegisterCallback(eColliderGroup group,Action<ObjectColliderBase,ICollisionObject> collideCallback)
    {
        if ((group & eColliderGroup.Player) != 0)
        {
            _collidedByPlayer += collideCallback;
        }
        if ((group & eColliderGroup.PlayerBullet) != 0)
        {
            _collidedByPlayerBullet += collideCallback;
        }
        if ((group & eColliderGroup.Enemy) != 0)
        {
            _collidedByEnemy += collideCallback;
        }
        if ((group & eColliderGroup.EnemyBullet) != 0)
        {
            _collidedByEnemyBullet += collideCallback;
        }
    }

    #region ICollisionObject

    /// <summary>
    /// 设置是否进行碰撞检测
    /// </summary>
    /// <param name="value"></param>
    public void SetDetectCollision(bool value)
    {
        //_isInteractive = value;
    }
    /// <summary>
    /// 是否进行碰撞检测
    /// </summary>
    /// <returns></returns>
    public bool DetectCollision()
    {
        return true;
    }
    /// <summary>
    /// 检测两物体包围盒是否相交
    /// <para>用于未知形状碰撞体之间的快速排斥试验</para>
    /// <para>for example</para>
    /// <para>玩家符卡与子弹之间的碰撞</para>
    /// <para>玩家符卡与敌机子弹形状都不是固定的</para>
    /// <para>因此会先进行快速排斥试验</para>
    /// </summary>
    /// <param name="lbPos">左下坐标</param>
    /// <param name="rtPos">右上坐标</param>
    /// <returns></returns>
    public virtual bool CheckBoundingBoxesIntersect(Vector2 lbPos, Vector2 rtPos)
    {
        return true;
    }
    /// <summary>
    /// 获取碰撞检测的参数
    /// </summary>
    /// <param name="index">对应的第n个碰撞盒的参数</param>
    /// <returns></returns>
    public virtual CollisionDetectParas GetCollisionDetectParas(int index = 0)
    {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// 物体第n个碰撞盒被物体碰撞了
    /// </summary>
    /// <param name="index"></param>
    public virtual void CollidedByObject(int n = 0, eEliminateDef eliminateDef = eEliminateDef.HitObjectCollider)
    {

    }
    #endregion

    public virtual void Clear()
    {
        ClearTasks();
        _master = null;
        if (_attachmentsCount > 0)
        {
            _attachmentsList.Clear();
            _attachmentsCount = 0;
        }
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
        _collidedByPlayer = null;
        _collidedByPlayerBullet = null;
        _collidedByEnemy = null;
        _collidedByEnemyBullet = null;
    }
}
