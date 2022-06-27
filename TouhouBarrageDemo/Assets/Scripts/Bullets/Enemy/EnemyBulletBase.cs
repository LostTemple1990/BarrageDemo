using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase :BulletBase,IAttachable,IAttachment,IAffectedMovableObject,ITaskExecuter,ISTGMovable,IParaChangable
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
    protected List<STGObjectComponent> _components;
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

    /// <summary>
    /// 当前透明度
    /// </summary>
    protected float _curAlpha;
    /// <summary>
    /// 当前颜色值，只计算rgb,a分量不考虑
    /// </summary>
    protected Color _curColor;
    /// <summary>
    /// 标识当前颜色是否是初始颜色
    /// <para>即没有更改过颜色</para>
    /// </summary>
    protected bool _isOriginalColor;
    /// <summary>
    /// 当前帧颜色是否被改变
    /// </summary>
    protected bool _isColorChanged;
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
    /// 是否设置了持续跟随本体移动
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
    /// <summary>
    /// 玩家角色
    /// </summary>
    protected CharacterBase _player;

    public EnemyBulletBase()
    {
        _components = new List<STGObjectComponent>();
        _attachmentsList = new List<IAttachment>();
    }

    public override void Init()
    {
        base.Init();
        _componentsCount = 0;
        _detectCollision = true;
        _isGrazed = false;
        _grazeCoolDown = 0;
        _resistEliminateFlag = 0;
        // 颜色相关
        _curColor = new Color(1, 1, 1);
        _isOriginalColor = true;
        _curAlpha = 1;
        // 依附物体
        _attachmentsCount = 0;
        _isFollowingMasterContinuously = false;
        _player = PlayerInterface.GetInstance().GetCharacter();
    }

    /// <summary>
    /// 根据id设置子弹的类型
    /// </summary>
    /// <param name="id"></param>
    public virtual void SetStyleById(string id)
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetAlpha(float alpha)
    {
        if ( _curAlpha != alpha )
        {
            _curAlpha = alpha;
            _isColorChanged = true;
            _isOriginalColor = false;
        }
    }

    public float alpha
    {
        set { SetAlpha(value); }
        get { return _curAlpha; }
    }

    /// <summary>
    /// 设置rgb颜色
    /// <para>取值为0~255</para>
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    public virtual void SetColor(float r,float g,float b)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 设置完成的rgba值
    /// </summary>
    /// <param name="r">0~255</param>
    /// <param name="g">0~255</param>
    /// <param name="b">0~255</param>
    /// <param name="a">0~1</param>
    public virtual void SetColor(float r,float g,float b,float a)
    {
        throw new System.NotImplementedException();
    }

    public virtual T AddOrGetComponent<T>()
        where T : STGObjectComponent, new()
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
        where T : STGObjectComponent
    {
        foreach (STGObjectComponent bc in _components)
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
        if (_clearFlag == 1) return false;
        if ( ((int)eliminateType & _resistEliminateFlag) != 0 )
        {
            return false;
        }
        if ( _attachableMaster != null )
        {
            IAttachable master = _attachableMaster;
            _attachableMaster = null;
            master.OnAttachmentEliminated(this);
        }
        if ( _attachmentsCount != 0 )
        {
            int attachmentsCount = _attachmentsCount;
            _attachmentsCount = 0;
            for (int i = 0; i < attachmentsCount; i++)
            {
                if (_attachmentsList[i] != null)
                {
                    _attachmentsList[i].OnMasterEliminated(eliminateType);
                }
            }
            _attachmentsList.Clear();
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
        for (int i=0;i<_attachmentsCount;i++)
        {
            if ( _attachmentsList[i] == attachment )
            {
                _attachmentsList[i] = null;
            }
        }
    }

    public void AttachTo(IAttachable master, bool eliminatedWithMaster)
    {
        if (_attachableMaster != null || master == null) return;
        _attachableMaster = master;
        master.AddAttachment(this);
        _isEliminatedWithMaster = eliminatedWithMaster;
    }

    public void SetRelativePos(float offsetX, float offsetY, float rotation, bool followMasterRotation,bool isFollowingMasterContinuously)
    {
        _relativePosToMaster = new Vector2(offsetX, offsetY);
        _relativeRotationToMaster = rotation;
        SetRotation(rotation);
        _isFollowMasterRotation = followMasterRotation;
        _isFollowingMasterContinuously = isFollowingMasterContinuously;
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
        if (!_isEliminatedWithMaster) return;
        Eliminate(eEliminateDef.CodeEliminate);
    }

    public virtual void DoStraightMove(float v,float angle)
    {
        throw new System.NotImplementedException();
    }

    public virtual void DoAcceleration(float acce,float accAngle)
    {
        throw new System.NotImplementedException();
    }

    public virtual void DoAccelerationWithLimitation(float acce,float accAngle,float maxVelocity)
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetStraightParas(float v,float angle,float acce,float accAngle)
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetPolarParas(float radius,float angle,float deltaR,float omega)
    {
        throw new System.NotImplementedException();
    }

    public virtual void SetPolarParas(float radius, float angle, float deltaR, float omega, float centerPosX, float centerPosY)
    {
        throw new System.NotImplementedException();
    }

    public virtual void MoveTo(float endX, float endY, int duration, InterpolationMode intMode)
    {
        throw new System.NotImplementedException();
    }

    public virtual void MoveTowards(float v,float angle,int duration)
    {
        throw new System.NotImplementedException();
    }

    public virtual void AddExtraSpeedParas(float v, float vAngle, float acce, float accAngle)
    {
        throw new System.NotImplementedException();
    }

    public virtual float velocity
    {
        get { return 0; }
        set { }
    }

    public virtual float vx
    {
        get { return 0; }
        set { }
    }

    public virtual float vy
    {
        get { return 0; }
        set { }
    }

    public virtual float maxVelocity
    {
        get { return 0; }
        set { }
    }

    public virtual float vAngle
    {
        get { return 0; }
        set { }
    }

    public virtual float acce
    {
        get { return 0; }
        set { }
    }

    public virtual float accAngle
    {
        get { return 0; }
        set { }
    }

    public void AddTask(Task task)
    {
        BCCustomizedTask component = GetComponent<BCCustomizedTask>();
        if ( component == null )
        {
            component = AddOrGetComponent<BCCustomizedTask>();
        }
        component.AddTask(task);
    }

    public virtual bool SetParaValue(STGObjectParaType type,float value)
    {
        throw new System.NotImplementedException();
    }

    public virtual bool GetParaValue(STGObjectParaType type,out float value)
    {
        throw new System.NotImplementedException();
    }

    public override void Clear()
    {
        _attachableMaster = null;
        if ( _attachmentsCount != 0 )
        {
            _attachmentsList.Clear();
            _attachmentsCount = 0;
        }
        for (int i=0;i<_componentsCount;i++)
        {
            _components[i].Clear();
        }
        _components.Clear();
        _componentsCount = 0;
        _attachmentsList.Clear();
        _attachmentsCount = 0;
        _player = null;
        base.Clear();
    }
}
