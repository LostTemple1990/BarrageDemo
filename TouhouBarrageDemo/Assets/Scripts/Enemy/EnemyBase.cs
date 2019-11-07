using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase :IAttachable,IAttachment,IAffectedMovableObject,ITaskExecuter,ISTGMovable
{
    /// <summary>
    /// 当前生命值
    /// </summary>
    protected int _curHp;
    /// <summary>
    /// 最大生命值
    /// </summary>
    protected int _maxHp;
   /// <summary>
   /// 当前位置
   /// </summary>
    protected Vector2 _curPos;
    /// <summary>
    /// 敌机本体gameobject
    /// </summary>
    protected GameObject _enemyGo;
    /// <summary>
    /// 敌机本体tf
    /// </summary>
    protected Transform _enemyTf;

    /// <summary>
    /// 当前速度
    /// </summary>
    protected float _curVelocity;
    /// <summary>
    /// 当前角度
    /// </summary>
    protected float _curAngle;
    protected float _moveTime;
    protected float _moveDuration;
    protected bool _isMoving;
    protected float _curVx;
    protected float _curVy;

    protected MovableObject _movableObj;

    protected bool _isDirty;

    protected EnemyType _type;

    protected int _clearFlag;

    protected int _curDir;

    protected bool _isExplosive;

    protected List<Task> _tasks;
    protected int _taskCount;

    protected float _collisionHalfWidth;
    protected float _collisionHalfHeight;

    protected Vector2 _wanderRangeX;
    protected Vector2 _wanderRangeY;
    protected Vector2 _wanderAmplitudeX;
    protected Vector2 _wanderAmplitudeY;
    protected InterpolationMode _wanderMode;
    protected DirectionMode _dirMode;
    protected int _wanderTime;
    protected int _wanderDuration;
    protected bool _isWandering;

    protected bool _isInteractive;
    /// <summary>
    /// 被击中时调用函数
    /// </summary>
    protected int _onHitFuncRef;
    /// <summary>
    /// 被消灭的时候调用函数
    /// </summary>
    protected int _onEliminateFuncRef;
    /// <summary>
    /// 表示不会被某些东西消除
    /// <para>Enemy部分基本使用</para>
    /// <para>PlayerSpellCard,PlayerBullet</para>
    /// </summary>
    protected int _resistEliminateFlag;
    /// <summary>
    /// 标识进行边界检测以便回收
    /// </summary>
    protected bool _checkOutOfBorder;
    /// <summary>
    /// 附件物体的列表
    /// </summary>
    protected List<IAttachment> _attachmentsList;
    /// <summary>
    /// 依附物体的个数
    /// </summary>
    protected int _attachmentsCount;
    /// <summary>
    /// 依附到的对象
    /// </summary>
    protected IAttachable _attachableMaster;
    /// <summary>
    /// 是否在master被销毁的时候一同销毁
    /// </summary>
    protected bool _isEliminatedWithMaster;
    /// <summary>
    /// 是否设置了相对于依附对象的相对位置
    /// </summary>
    protected bool _isFollowingMasterContinuously;
    /// <summary>
    /// 相对于依附对象的位置
    /// </summary>
    protected Vector2 _relativePosToMaster;
    /// <summary>
    /// 相对于依附对象的旋转角度
    /// </summary>
    protected float _relativeRotationToMaster;
    /// <summary>
    /// 是否随着依附对象的旋转而改变角度
    /// </summary>
    protected bool _isFollowMasterRotation;

    public EnemyBase()
    {
        _attachmentsList = new List<IAttachment>();
    }

    public virtual void Init()
    {
        _clearFlag = 0;
        _curDir = Consts.DIR_NULL;
        _isExplosive = false;
        if ( _tasks == null )
        {
            _tasks = new List<Task>();
        }
        if ( _movableObj == null )
        {
            _movableObj = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        }
        _taskCount = 0;
        _isWandering = false;
        _isInteractive = true;
        _onHitFuncRef = 0;
        _onEliminateFuncRef = 0;
        _resistEliminateFlag = 0;
        _attachmentsCount = 0;
        _isFollowingMasterContinuously = false;
        _isFollowMasterRotation = false;
    }

    public virtual void Update()
    {
        if ( _isMoving )
        {
            Move();
        }
        UpdateTransform();
        _isDirty = false;
    }

    public virtual void SetPosition(float posX,float posY)
    {
        _curPos = new Vector2(posX, posY);
        _movableObj.SetPos(posX, posY);
        //_enemyTf.localPosition = _curPos;
    }

    public virtual void SetPosition(Vector2 pos)
    {
        _curPos = pos;
        _movableObj.SetPos(_curPos.x, _curPos.y);
        //_enemyTf.localPosition = pos;
    }

    public virtual void AddTask(Task task)
    {
        _tasks.Add(task);
        _taskCount++;
    }

    protected virtual void UpdateTask()
    {
        Task task;
        for (int i=0;i<_taskCount;i++)
        {
            task = _tasks[i];
            if ( task != null )
            {
                task.Update();
                if (task.isFinish)
                {
                    ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
                    _tasks[i] = null;
                }
            }
        }
    }

    /// <summary>
    /// 添加额外的速度参数
    /// </summary>
    /// <param name="v"></param>
    /// <param name="vAngle"></param>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public virtual void AddExtraSpeedParas(float v,float vAngle,float acce,float accAngle)
    {
        _movableObj.AddExtraSpeedParas(v, vAngle, acce, accAngle);
    }

    public virtual void MoveTo(float posX,float posY,int duration,InterpolationMode mode)
    {
        _movableObj.Reset(_curPos.x, _curPos.y);
        _movableObj.MoveTo(posX, posY, duration, mode);
        _isMoving = true;
        _moveTime = 0;
        _moveDuration = duration;
    }

    public virtual void MoveTowards(float velocity,float angle,int duration)
    {
        _movableObj.MoveTowards(velocity, angle, duration);
        _isMoving = true;
        _moveTime = 0;
        _moveDuration = duration;
    }

    /// <summary>
    /// 加速移动
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="angle"></param>
    /// <param name="acc"></param>
    public virtual void AccMoveTowards(float velocity,float angle,float acc)
    {
        _movableObj.DoStraightMove(velocity, angle);
        _movableObj.DoAcceleration(acc, angle);
        _isMoving = true;
    }

    /// <summary>
    /// 加速移动(最大速度限制)
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="angle"></param>
    /// <param name="acc"></param>
    /// <param name="maxVelocity"></param>
    public virtual void AccMoveTowardsWithLimitation(float velocity,float angle,float acc,float maxVelocity)
    {
        _movableObj.DoStraightMove(velocity, angle);
        _movableObj.DoAccelerationWithLimitation(acc, angle, maxVelocity);
        _isMoving = true;
    }

    protected void CheckCollisionWithCharacter()
    {
        Vector2 playerPos = Global.PlayerPos;
        if ( Mathf.Abs(playerPos.x-_curPos.x) <= _collisionHalfWidth + Global.PlayerCollisionVec.z &&
            Mathf.Abs(playerPos.y - _curPos.y) <= _collisionHalfHeight + Global.PlayerCollisionVec.z )
        {
            //Logger.Log("Hit By Enemy!");
            PlayerInterface.GetInstance().GetCharacter().BeingHit();
        }
    }

    public virtual bool Eliminate(eEliminateDef eliminateType=0)
    {
        if (_clearFlag == 1) return false;
        if ((_resistEliminateFlag & (int)eliminateType) != 0) return false;
        OnEliminate(eliminateType);
        _clearFlag = 1;
        return true;
    }

    /// <summary>
    /// 设置被消除的时候触发的函数
    /// </summary>
    /// <param name="funcRef"></param>
    public void SetOnEliminateFuncRef(int funcRef)
    {
        _onEliminateFuncRef = funcRef;
    }

    /// <summary>
    /// 设置被击中的时候触发的函数
    /// </summary>
    /// <param name="funcRef"></param>
    public void SetOnHitFuncRef(int funcRef)
    {
        _onHitFuncRef = funcRef;
    }

    protected virtual void Move()
    {
        
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="eliminateType"></param>
    public virtual void TakeDamage(int damage,eEliminateDef eliminateType=eEliminateDef.PlayerBullet)
    {

    }

    /// <summary>
    /// 被消除的时候调用
    /// </summary>
    protected void OnEliminate(eEliminateDef eliminateType)
    {
        if ( eliminateType != eEliminateDef.ForcedDelete && eliminateType != eEliminateDef.CodeRawEliminate )
        {
            if (_onEliminateFuncRef != 0)
            {
                InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
                InterpreterManager.GetInstance().CallLuaFunction(_onEliminateFuncRef, 1);
            }
        }
        if ( _attachableMaster != null )
        {
            IAttachable master = _attachableMaster;
            _attachableMaster = null;
            master.OnAttachmentEliminated(this);
        }
        if ( _attachmentsCount != 0 )
        {
            int count = _attachmentsCount;
            _attachmentsCount = 0;
            IAttachment attachment;
            for (int i=0;i<count;i++)
            {
                attachment = _attachmentsList[i];
                if ( attachment != null )
                {
                    attachment.OnMasterEliminated(eliminateType);
                }
            }
            _attachmentsList.Clear();
        }
    }

    /// <summary>
    /// 被击中的时候调用
    /// </summary>
    protected void OnHit()
    {
        InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
        InterpreterManager.GetInstance().CallLuaFunction(_onHitFuncRef, 1);
    }

    protected virtual void UpdateTransform()
    {
        _enemyTf.localPosition = _curPos;
    }

    public virtual CollisionDetectParas GetCollisionDetectParas(int index=0)
    {
        CollisionDetectParas paras = new CollisionDetectParas
        {
            type = CollisionDetectType.Rect,
            centerPos = new Vector2(_curPos.x,_curPos.y),
            halfWidth = _collisionHalfWidth,
            halfHeight = _collisionHalfHeight,
            radius = Mathf.Min(_collisionHalfWidth,_collisionHalfHeight),
            nextIndex = -1,
            angle = 0,
        };
        return paras;
    }

    /// <summary>
    /// 检测是否越界
    /// </summary>
    /// <returns></returns>
    protected bool IsOutOfBorder()
    {
        if (!_checkOutOfBorder) return false;
        if (_curPos.x < Global.BulletLBBorderPos.x ||
            _curPos.y < Global.BulletLBBorderPos.y ||
            _curPos.x > Global.BulletRTBorderPos.x ||
            _curPos.y > Global.BulletRTBorderPos.y)
        {
            return true;
        }
        return false;
    }

    public virtual void SetCollisionParams(float collisionHW,float collisionHH)
    {
        _collisionHalfWidth = collisionHW;
        _collisionHalfHeight = collisionHH;
    }

    public virtual void SetMaxHp(int maxHp)
    {
        _maxHp = maxHp;
        _curHp = maxHp;
    }

    /// <summary>
    /// 设置移动范围
    /// </summary>
    /// <param name="minX"></param>
    /// <param name="maxX"></param>
    /// <param name="minY"></param>
    /// <param name="maxY"></param>
    public virtual void SetWanderRange(float minX,float maxX,float minY,float maxY)
    {
        _wanderRangeX = new Vector2(minX, maxX);
        _wanderRangeY = new Vector2(minY, maxY);
    }

    /// <summary>
    /// 设置每次移动的范围
    /// </summary>
    /// <param name="lrX">X的增量最小值 leftRangeX</param>
    /// <param name="rrX">X的增量最大值 rightRangeX</param>
    /// <param name="lrY">Y的增量最小值 leftRangeY</param>
    /// <param name="rrY">Y的增量最大值 rightRangeY</param>
    public virtual void SetWanderAmplitude(float lrX,float rrX,float lrY,float rrY)
    {
        _wanderAmplitudeX = new Vector2(lrX, rrX);
        _wanderAmplitudeY = new Vector2(lrY, rrY);
    }

    public virtual void SetWanderMode(InterpolationMode moveMode,DirectionMode dirMode)
    {
        _wanderMode = moveMode;
        _dirMode = dirMode;
    }

    /// <summary>
    /// 敌机在duration的持续时间内进行移动
    /// <para>移动的目标地点以及方式在之前设置的范围、振幅、模式里面</para>
    /// </summary>
    /// <param name="duration"></param>
    public virtual void DoWander(int duration)
    {
        // 记录移动方向是否有效的数组
        bool[] dirAvailable = {true,true};
        int dir;
        float minX, maxX, minY, maxY,toX,toY;
        // 根据dirMode计算重新计算边界
        Vector2 playerPos = Global.PlayerPos;
        if (_curPos.x - _wanderAmplitudeX.x < _wanderRangeX.x) dirAvailable[0] = false;
        if (_curPos.x + _wanderAmplitudeX.x > _wanderRangeX.y) dirAvailable[1] = false;
        if (!dirAvailable[0] && !dirAvailable[1])
        {
            toX = _curPos.x;
        }
        else
        {
            dir = MTRandom.GetNextInt(0, 1);
            if (_dirMode == DirectionMode.MoveXTowardsPlayer || _dirMode == DirectionMode.MoveTowardsPlayer)
            {
                //往左移动
                if (playerPos.x < _curPos.x)
                {
                    dir = 0;
                }
                else
                {
                    dir = 1;
                }
            }
            // 这步算出来的dir一定是有效的
            if ( dirAvailable[dir] == false )
            {
                dir = 1 - dir;
            }
            // 左移动，计算左移动的最大值和最小值
            if ( dir == 0 )
            {
                maxX = _curPos.x - _wanderAmplitudeX.x;
                minX = _curPos.x - _wanderAmplitudeX.y < _wanderRangeX.x ? _wanderRangeX.x : _curPos.x - _wanderAmplitudeX.y;
            }
            else
            {
                minX = _curPos.x + _wanderAmplitudeX.x;
                maxX = _curPos.x + _wanderAmplitudeX.y > _wanderRangeX.y ? _wanderRangeX.y : _curPos.x + _wanderAmplitudeX.y;
            }
            toX = MTRandom.GetNextFloat(minX, maxX);
        }
        // 计算Y的范围 0 = 下, 1 = 上
        dirAvailable[0] = true;
        dirAvailable[1] = true;
        if (_curPos.y - _wanderAmplitudeY.x < _wanderRangeY.x) dirAvailable[0] = false;
        if (_curPos.y + _wanderAmplitudeY.x > _wanderRangeY.y) dirAvailable[1] = false;
        if (!dirAvailable[0] && !dirAvailable[1])
        {
            toY = _curPos.y;
        }
        else
        {
            dir = MTRandom.GetNextInt(0, 1);
            if (_dirMode == DirectionMode.MoveYTowardsPlayer || _dirMode == DirectionMode.MoveTowardsPlayer)
            {
                //往下移动
                if (playerPos.y < _curPos.y)
                {
                    dir = 0;
                }
                else
                {
                    dir = 1;
                }
            }
            // 这步算出来的dir一定是有效的
            if (dirAvailable[dir] == false)
            {
                dir = 1 - dir;
            }
            // 向下移动
            if (dir == 0)
            {
                maxY = _curPos.y - _wanderAmplitudeY.x;
                minY = _curPos.y - _wanderAmplitudeY.y < _wanderRangeY.y ? _wanderRangeY.y : _curPos.y - _wanderAmplitudeY.y;
            }
            else
            {
                minY = _curPos.y + _wanderAmplitudeY.x;
                maxY = _curPos.y + _wanderAmplitudeY.y > _wanderRangeY.y ? _wanderRangeY.y : _curPos.y + _wanderAmplitudeY.y;
            }
            toY = MTRandom.GetNextFloat(minY, maxY);
        }
        MoveTo(toX, toY, duration, _wanderMode);
    }

    public virtual void Clear()
    {
        ClearTasks();
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObj);
        _movableObj = null;
        _enemyGo = null;
        _enemyTf = null;
        _attachableMaster = null;
        if ( _attachmentsCount != 0 )
        {
            _attachmentsList.Clear();
            _attachmentsCount = 0;
        }
    }

    protected virtual void ClearTasks()
    {
        Task task;
        for (int i = 0; i < _taskCount; i++)
        {
            task = _tasks[i];
            if (task != null)
            {
                if ( !task.isFinish  )
                {
                    InterpreterManager.GetInstance().StopTaskThread(task);
                }
                ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(task);
            }
        }
        _tasks.Clear();
        _taskCount = 0;
    }

    /// <summary>
    /// 设置是否进行边界检测
    /// </summary>
    /// <param name="value"></param>
    public virtual void SetCheckOutOfBorder(bool value)
    {
        _checkOutOfBorder = value;
    }

    /// <summary>
    /// 设置是否可以交互、即能否被破坏之类的
    /// </summary>
    /// <param name="value"></param>
    public void SetInteractive(bool value)
    {
        _isInteractive = value;
    }

    /// <summary>
    /// 设置不会被某些东西消除
    /// </summary>
    /// <param name="flag"></param>
    public void SetResistEliminateFlag(int flag)
    {
        _resistEliminateFlag = flag;
    }

    public EnemyType GetEnemyType()
    {
        return _type;
    }

    public bool IsInteractive
    {
        get { return _isInteractive; }
    }

    /// <summary>
    /// 判断是否有击中判定
    /// </summary>
    /// <returns></returns>
    public virtual bool CanHit()
    {
        if (!_isInteractive) return false;
        if (_clearFlag == 1) return false;
        return _curHp > 0;
    }

    /// <summary>
    /// 获取当前血量
    /// </summary>
    /// <returns></returns>
    public int GetCurHp()
    {
        return _curHp;
    }

    /// <summary>
    /// 获取当前最大血量
    /// </summary>
    /// <returns></returns>
    public int GetMaxHp()
    {
        return _maxHp;
    }

    public EnemyType Type
    {
        get { return _type; }
    }

    public int ClearFlag
    {
        get { return _clearFlag; }
    }

    public float velocity
    {
        get { return _movableObj.velocity; }
        set { _movableObj.velocity = value; }
    }

    public float vx
    {
        get { return _movableObj.vx; }
        set { _movableObj.vx = value; }
    }

    public float vy
    {
        get { return _movableObj.vy; }
        set { _movableObj.vy = value; }
    }

    public float maxVelocity
    {
        get { return _movableObj.maxVelocity; }
        set { _movableObj.maxVelocity = value; }
    }

    public float vAngle
    {
        get { return _movableObj.vAngle; }
        set { _movableObj.vAngle = value; }
    }

    public float acce
    {
        get { return _movableObj.acce; }
        set { _movableObj.acce = value; }
    }

    public float accAngle
    {
        get { return _movableObj.accAngle; }
        set { _movableObj.accAngle = value; }
    }

    public float dx
    {
        get { return _movableObj.dx; }
    }

    public float dy
    {
        get { return _movableObj.dy; }
    }

    public Vector2 GetPosition()
    {
        return _curPos;
    }

    public float GetRotation()
    {
        return 0;
    }

    public void SetRotation(float rotation)
    {

    }

    public void AddAttachment(IAttachment attachment)
    {
        for (int i=0;i<_attachmentsCount;i++)
        {
            if ( _attachmentsList[i] == attachment )
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

    public void AttachTo(IAttachable master, bool eliminatedWithMaster)
    {
        if (_attachableMaster != null || master == null) return;
        _attachableMaster = master;
        _isEliminatedWithMaster = eliminatedWithMaster;
        master.AddAttachment(this);
    }

    public void SetRelativePos(float offsetX, float offsetY, float rotation, bool followMasterRotation,bool isFollowingMasterContinuously)
    {
        _relativePosToMaster = new Vector2(offsetX, offsetY);
        _isFollowingMasterContinuously = isFollowingMasterContinuously;
        _relativeRotationToMaster = rotation;
        SetRotation(rotation);
        _isFollowMasterRotation = followMasterRotation;
        if (_attachableMaster != null)
        {
            Vector2 relativePos = _relativePosToMaster;
            if (_isFollowMasterRotation)
            {
                relativePos = MathUtil.GetVec2AfterRotate(relativePos.x, relativePos.y, 0, 0, _attachableMaster.GetRotation());
            }
            _curPos = relativePos + _attachableMaster.GetPosition();
        }
    }

    public void OnMasterEliminated(eEliminateDef eliminateType)
    {
        _attachableMaster = null;
        _isFollowingMasterContinuously = false;
        if ( _isEliminatedWithMaster )
        {
            Eliminate(eliminateType);
        }
    }

    public void DoStraightMove(float v, float angle)
    {
        _movableObj.DoStraightMove(v, angle);
    }

    public void DoAcceleration(float acce, float accAngle)
    {
        _movableObj.DoAcceleration(acce, accAngle);
    }

    public void DoAccelerationWithLimitation(float acce, float accAngle, float maxVelocity)
    {
        _movableObj.DoAccelerationWithLimitation(acce, accAngle, maxVelocity);
    }

    public void SetPolarParas(float radius, float angle, float deltaR, float omega)
    {
        _movableObj.SetPolarParas(radius, angle, deltaR, omega);
    }
}

public enum EnemyType : byte
{
    NormalEnemy = 1,
    Boss = 2,
}