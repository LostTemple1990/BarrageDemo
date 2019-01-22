using UnityEngine;
using System.Collections;

public class ObjectColliderBase : IAttachment, IObjectCollider,ISTGMovable
{
    protected float _curPosX;
    protected float _curPosY;
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
    protected int _hitEnemyDamage;

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

    public ObjectColliderBase()
    {
        _clearFlag = 0;
        _existDuration = 0;
        _hitEnemyDamage = 0;
        _eliminateType = eEliminateDef.HitObjectCollider;
        _isScaling = false;
        _movableObject = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        _tag = "";
    }

    public void SetTag(string tag)
    {
        _tag = tag;
    }

    public string GetTag()
    {
        return _tag;
    }

    public void SetToPosition(float posX,float posY)
    {
        _curPosX = posX;
        _curPosY = posY;
        _curPos = new Vector2(posX, posY);
        _movableObject.SetPos(posX, posY);
    }

    public void SetToPosition(Vector2 pos)
    {
        _curPosX = pos.x;
        _curPosY = pos.y;
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
        _existTime = 0;
        _existDuration = existDuration;
    }

    public void Update()
    {
        if (_clearFlag == 1) return;
        if (_isScaling) Scale();
        if ( _isFollowingMasterContinuously && _master != null )
        {
            SetToPosition(_relativePosToMaster + _master.GetPosition());
        }
        else
        {
            if ( _movableObject.IsActive() )
            {
                _movableObject.Update();
                _curPos = _movableObject.GetPos();
                _curPosX = _curPos.x;
                _curPosY = _curPos.y;
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
        if ((_colliderGroups & (int)eColliderGroup.Enemy) != 0)
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
    public void SetHitEnemyDamage(int damage)
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
            SetToPosition(_master.GetPosition() + _relativePosToMaster);
        }
    }

    public void OnMasterEliminated(eEliminateDef eliminateType)
    {
        _master = null;
        _clearFlag = 1;
    }

    #region ISTGMovable
    public void DoStraightMove(float v, float angle)
    {
        _movableObject.DoStraightMove(v, angle);
    }

    public void DoStraightMoveWithLimitation(float v, float angle, int duration)
    {
        _movableObject.DoStraightMoveWithLimitation(v, angle, duration);
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
        _movableObject.DoMoveTo(endX, endY, duration, mode);
    }

    public void DoCurvedMove(float radius, float angle, float deltaR, float omega)
    {
        _movableObject.DoCurvedMove(radius, angle, deltaR, omega);
    }

    public float Velocity
    {
        get { return _movableObject.Velocity; }
    }

    public float VAngle
    {
        get { return _movableObject.VAngle; }
    }

    public float Acce
    {
        get { return _movableObject.Acce; }
    }

    public float AccAngle
    {
        get { return _movableObject.AccAngle; }
    }

    #endregion

    public void Clear()
    {
        _master = null;
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObject);
        _movableObject = null;
    }
}
