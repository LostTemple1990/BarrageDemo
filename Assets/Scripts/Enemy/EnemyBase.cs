using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : ICollisionObject
{
    protected float _curHp;
    protected float _maxHp;
   /// <summary>
   /// 当前位置
   /// </summary>
    protected Vector3 _curPos;
    /// <summary>
    /// 敌机本体gameobject
    /// </summary>
    protected GameObject _enemyGo;

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

    protected string _dropId;
    protected float _dropHalfWidth;
    protected float _dropHalfHeight;

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
            _movableObj = new MovableObject();
        }
        _taskCount = 0;
        _dropId = null;
        _isWandering = false;
        _isInteractive = true;
    }

    public virtual void Update()
    {
        if ( _isMoving )
        {
            Move();
        }
        UpdatePos();
        _isDirty = false;
    }

    public virtual void SetToPosition(Vector3 pos)
    {
        _curPos = pos;
        _movableObj.SetPos(_curPos.x, _curPos.y);
        _enemyGo.transform.localPosition = pos;
    }

    public virtual void AddTask(Task task)
    {
        _tasks.Add(task);
        _taskCount++;
    }

    /// <summary>
    /// 设置掉落,掉落范围在以当前位置为中心，长宽分别为2*halfWidth和2*halfHeight的矩形内
    /// </summary>
    /// <param name="id">掉落包id</param>
    /// <param name="halfWidth">掉落矩形宽</param>
    /// <param name="halfHeight">掉落矩形高</param>
    public virtual void SetDrop(string id,float halfWidth,float halfHeight)
    {
        _dropId = id;
        _dropHalfWidth = halfWidth;
        _dropHalfHeight = halfHeight;
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
                    ObjectsPool.GetInstance().RestoreDataClassToPool<Task>(task);
                    _tasks[i] = null;
                }
            }
        }
    }

    public virtual void MoveToPos(float posX,float posY,int duration,InterpolationMode mode)
    {
        _movableObj.Reset(_curPos.x, _curPos.y);
        _movableObj.DoMoveTo(posX, posY, duration, mode);
        _isMoving = true;
        _moveTime = 0;
        _moveDuration = duration;
    }

    public virtual void MoveTowards(float velocity,float angle,int duration)
    {
        _movableObj.DoMoveStraightWithLimitation(velocity, angle, duration);
        _isMoving = true;
        _moveTime = 0;
        _moveDuration = duration;
    }

    protected void CheckCollisionWithCharacter()
    {
        Vector2 playerPos = Global.PlayerPos;
        if ( Mathf.Abs(playerPos.x-_curPos.x) <= _collisionHalfWidth + Global.PlayerCollisionVec.z &&
            Mathf.Abs(playerPos.y - _curPos.y) <= _collisionHalfHeight + Global.PlayerCollisionVec.z )
        {
            Logger.Log("Hit By Enemy!");
            PlayerService.GetInstance().GetCharacter().BeingHit();
        }
    }

    protected virtual void Move()
    {
        
    }

    public virtual void DoHit(float damage)
    {

    }

    protected virtual void UpdatePos()
    {
        _curPos = _movableObj.GetPos();
        _enemyGo.transform.localPosition = _curPos;
    }

    public virtual CollisionDetectParas GetCollisionDetectParas()
    {
        CollisionDetectParas paras = new CollisionDetectParas
        {
            type = CollisionDetectType.Rect,
            centerPos = new Vector2(_curPos.x,_curPos.y),
            halfWidth = _collisionHalfWidth,
            halfHeight = _collisionHalfHeight,
            angle = 0,
        };
        return paras;
    }

    public virtual int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        throw new System.NotImplementedException();
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

    public virtual void Kill()
    {
        
    }

    public virtual void RawKill()
    {
        _curHp = 0;
        _clearFlag = 1;
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

    public virtual void DoWander(int duration)
    {
        // 先计算下一次需要移动到的点
        // 计算移动边界
        float minX = _curPos.x + _wanderAmplitudeX.x < _wanderRangeX.x ? _wanderRangeX.x : _curPos.x + _wanderAmplitudeX.x;
        float maxX = _curPos.x + _wanderAmplitudeX.y > _wanderRangeX.y ? _wanderRangeX.y : _curPos.x + _wanderAmplitudeX.y;
        float minY = _curPos.y + _wanderAmplitudeY.x < _wanderRangeY.x ? _wanderRangeY.x : _curPos.y + _wanderAmplitudeY.x;
        float maxY = _curPos.y + _wanderAmplitudeY.y > _wanderRangeY.y ? _wanderRangeY.y : _curPos.y + _wanderAmplitudeY.y;
        // 根据dirMode计算重新计算边界
        Vector2 playerPos = Global.PlayerPos;
        // X的范围
        if ( _dirMode == DirectionMode.MoveXTowardsPlayer || _dirMode == DirectionMode.MoveTowardsPlayer )
        {
            if ( playerPos.x < _curPos.x ) //往左移动
            {
                minX = minX > playerPos.x ? minX : playerPos.x;
                maxX = _curPos.x;
            }
            else // 往右移动
            {
                minX = _curPos.x;
                maxX = maxX > playerPos.x ? playerPos.x : maxX;
            }
        }
        // Y的范围
        if (_dirMode == DirectionMode.MoveYTowardsPlayer || _dirMode == DirectionMode.MoveTowardsPlayer)
        {
            if (playerPos.y < _curPos.y) //往下移动
            {
                minY = minY > playerPos.y ? minY : playerPos.y;
                maxY = _curPos.y;
            }
            else // 往上移动
            {
                minY = _curPos.y;
                maxY = maxY > playerPos.y ? playerPos.y : maxY;
            }
        }
        float toX = MTRandom.GetNextFloat(minX, maxX);
        float toY = MTRandom.GetNextFloat(minY, maxY); ;
        MoveToPos(toX, toY, duration, _wanderMode);
    }

    /// <summary>
    /// 掉落道具
    /// </summary>
    protected virtual void DropItems()
    {
        if ( _dropId != null )
        {
            ItemManager.GetInstance().DropItems(_dropId, _curPos.x, _curPos.y, _dropHalfWidth, _dropHalfHeight);
        }
    }

    public virtual void Clear()
    {
        ClearTasks();
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
                    InterpreterManager.GetInstance().StopTaskThread(task.luaState, task.funcRef);
                }
                ObjectsPool.GetInstance().RestoreDataClassToPool<Task>(task);
            }
        }
        _tasks.Clear();
        _taskCount = 0;
    }

    /// <summary>
    /// 设置是否可以交互、即能否被破坏之类的
    /// </summary>
    /// <param name="value"></param>
    public void SetInteractive(bool value)
    {
        _isInteractive = value;
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
        if ( _clearFlag == 1 )
        {
            return false;
        }
        return _curHp > 0;
    }

    public EnemyType Type
    {
        get { return _type; }
    }

    public int ClearFlag
    {
        get { return _clearFlag; }
    }

    public Vector3 CurPos
    {
        get { return _curPos; }
    }

}

public enum EnemyType : byte
{
    NormalEnemy = 1,
    Boss = 2,
}