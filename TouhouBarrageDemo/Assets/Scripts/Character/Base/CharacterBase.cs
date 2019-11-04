using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBase : IAffectedMovableObject
{
    protected double _hitRadius = 0d;
    protected GameObject _character;
    /// <summary>
    /// 本体tf
    /// </summary>
    protected Transform _trans;
    /// <summary>
    /// 副武器的显示层级
    /// </summary>
    protected Transform _subLayerTrans;
    /// <summary>
    /// 人物本体动画id
    /// </summary>
    protected string _charAniId;
    /// <summary>
    /// AnimationCharacter
    /// </summary>
    protected AnimationCharacter _aniChar;
    /// <summary>
    /// 动画本体SpriteRenderer,用来做闪烁等特效
    /// </summary>
    protected SpriteRenderer _charAniSp;
    /// <summary>
    /// 射击间隔
    /// </summary>
    protected int _shootCoolDown;
    protected int _curShootCD;

    protected int _inputDir;
    protected int _inputMoveMode;
    protected bool _inputShoot;
    protected bool _inputBomb;

    protected int _charID;
    protected int _subID;
    /// <summary>
    /// 自机左、右侧的子弹发射位置的偏移量
    /// </summary>
    protected Vector3 _leftBulletOffset, _rightBulletOffset;

    protected string _mainBulletId;
    protected BulletType _subBulletId;

    /// <summary>
    /// 是否可以移动
    /// </summary>
    protected bool _isMovable;
    protected ePlayerMoveMode _preMoveMode;
    protected ePlayerMoveMode _curMoveMode;
    protected Transform _collisionPointTf;
    protected Transform _rotatePointTf;
    protected Vector3 _collisionPointRotateVec3;

    protected int _availableSubCount;
    protected SubWeaponBase[] _subWeapons;

    /// <summary>
    /// 碰撞检测的真实半径
    /// </summary>
    protected float _collisionRadius;
    /// <summary>
    /// 擦弹半径，此处为正方形边长的一半
    /// </summary>
    protected float _grazeRadius;
    /// <summary>
    /// 当前位置
    /// </summary>
    protected Vector2 _curPos;
    /// <summary>
    /// Z轴
    /// </summary>
    protected int _orderInLayer;

    protected delegate void StateEnterFunc();
    protected delegate void StateUpdateFunc();
    protected delegate void StateExitFunc();

    /// <summary>
    /// 当前状态
    /// </summary>
    protected eCharacterState _curState;
    protected eCharacterState _nextState;

    protected StateEnterFunc _stateEnterFunc;
    protected StateUpdateFunc _stateUpdateFunc;
    protected StateExitFunc _stateExitFunc;

    protected int _appearTime, _appearDuration;
    /// <summary>
    /// 机体刷新的起始、结束Y坐标
    /// </summary>
    protected float _appearStartY, _appearEndY;
    /// <summary>
    /// 决死经过的时间
    /// </summary>
    protected int _dyingTime;
    /// <summary>
    /// 决死总时间
    /// </summary>
    protected int _dyingDuration;

    protected int _waitRebornTime;
    protected int _waitRebornDuration;

    protected bool _isInvincible;
    protected int _invincibleTime;
    protected int _invincibleDuration;

    protected BombBase _bomb;
    protected int _bombCoolDown;
    protected int _curBombCD;
    protected bool _isCastingBomb;
    protected int _bombInvincibleDuration;
    /// <summary>
    /// 松开射击键之后经过的时间
    /// </summary>
    protected int _shootTimeAfterKeyUp;
    /// <summary>
    /// 标识当前帧是否按下射击键
    /// </summary>
    protected bool _isInputShootKey;
    /// <summary>
    /// 当前是否在射击状态
    /// </summary>
    protected bool _isInShootingStatus;
    /// <summary>
    /// 当前是否允许射击
    /// </summary>
    protected bool _isShootAvailable;
    /// <summary>
    /// 禁止射击的时间
    /// </summary>
    protected int _shootUnavailableDuration;

    /// <summary>
    /// 被赋予的额外速度的x分量
    /// </summary>
    protected float _extraVelocityX;
    /// <summary>
    /// 被赋予的额外速度的y分量
    /// </summary>
    protected float _extraVelocityY;
    /// <summary>
    /// 被赋予的额外加速度x分量
    /// </summary>
    protected float _extraAcceX;
    /// <summary>
    /// 被赋予的额外加速度的y分量
    /// </summary>
    protected float _extraAcceY;

    #region 状态机相关

    #region 状态机基本函数
    protected void UpdateState()
    {
        if ( _nextState != eCharacterState.Undefined && _curState != _nextState )
        {
            OnStateExit();
            _stateUpdateFunc = null;
            _stateExitFunc = null;
            _curState = _nextState;
            OnStateEnter();
        }
        _stateUpdateFunc();
    }

    protected void OnStateEnter()
    {
        switch ( _curState )
        {
            case eCharacterState.Appear:
                _stateEnterFunc = OnStateAppearEnter;
                //Logger.Log("Player Enter State : Appear");
                break;
            case eCharacterState.Normal:
                _stateEnterFunc = OnStateNormalEnter;
                //Logger.Log("Player Enter State : Nromal");
                break;
            case eCharacterState.Dying:
                _stateEnterFunc = OnStateDyingEnter;
                //Logger.Log("Player Enter State : Dying");
                break;
            case eCharacterState.WaitReborn:
                _stateEnterFunc = OnStateWaitRebornEnter;
                //Logger.Log("Player Enter State : WaitBorn");
                break;
        }
        _stateEnterFunc();
    }

    protected void OnStateExit()
    {
        if ( _stateExitFunc != null )
        {
            _stateExitFunc();
        }
    }
    #endregion

    #region 自机从屏幕下方出现至屏幕的状态
    protected void OnStateAppearEnter()
    {
        _curPos = new Vector3(0, _appearStartY, 0);
        _appearTime = 0;
        _isInvincible = true;
        _invincibleTime = 0;
        _invincibleDuration = Consts.AppearInvincibleDuration;
        _stateUpdateFunc = StateAppearUpdate;
        _character.SetActive(true);
        _preMoveMode = _curMoveMode = Consts.MoveModeHighSpeed;
        // 显示副武器
        //UpdateSubWeaponsVisible();
        ResetSubWeapons();
    }

    protected void StateAppearUpdate()
    {
        RotateCollisionPoint();
        // 更新自机移动位置
        float curY = MathUtil.GetEaseInQuadInterpolation(_appearStartY, _appearEndY, _appearTime, _appearDuration);
        _curPos.y = curY;
        _appearTime++;
        if ( _appearTime >= _appearDuration )
        {
            _nextState = eCharacterState.Normal;
        }
        UpdateInvincibleStatus();
        UpdatePosition();
    }
    #endregion

    #region 自机普通状态
    protected virtual void OnStateNormalEnter()
    {
        _stateUpdateFunc = StateNormalUpdate;
        _stateExitFunc = OnStateNormalExit;
        UpdateCollisionData();
        // 射击相关
        _shootTimeAfterKeyUp = Consts.MaxShootDurationAfterKeyUp + 1;
        _isInputShootKey = false;
        _isShootAvailable = true;
    }

    protected virtual void StateNormalUpdate()
    {
        CheckMoveMode();
        Move();
        UpdateCD();
        Shoot();
        CastBomb();
        if ( _isCastingBomb )
        {
            _bomb.Update();
            if ( _bomb.IsFinish )
            {
                _isCastingBomb = false;
                _bomb.Clear();
            }
        }
        UpdateSubWeapons();
        UpdateCollisionData();
        if ( _isInvincible )
        {
            UpdateInvincibleStatus();
        }
        UpdatePosition();
        ResetExtraSpeedParas();
    }

    protected virtual void OnStateNormalExit()
    {
        int i;
        // 隐藏所有副武器
        for (i=0;i<4;i++)
        {
            _subWeapons[i].OnCharacterStateExit(eCharacterState.Normal);
            _subWeapons[i].SetActive(false);
        }
        _availableSubCount = -1;
        _isShootAvailable = false;
        // 信号相关
        PlayerService.GetInstance().SetSignalValue(0);
    }
    #endregion

    #region 自机决死状态
    protected virtual void OnStateDyingEnter()
    {
        SoundManager.GetInstance().Play("se_pldead00", false);
        _dyingTime = 0;
        _stateUpdateFunc = StateDyingUpdate;
        _stateExitFunc = OnStateDyingExit;
        // 自机暂时消失
        _character.SetActive(false);
        // 无敌
        SetInvincible(true, 20);
        CommandManager.GetInstance().RunCommand(CommandConsts.PlayerDying);
    }

    protected virtual void StateDyingUpdate()
    {
        RotateCollisionPoint();
        UpdateCD();
        CastBomb();
        if ( _isCastingBomb )
        {
            _nextState = eCharacterState.Normal;
            _character.SetActive(true);
            return;
        }
        _dyingTime++;
        if ( _dyingTime >= _dyingDuration )
        {
            _nextState = eCharacterState.WaitReborn;
        }
    }

    protected void OnStateDyingExit()
    {
        
    }
    #endregion

    #region 自机等待复活状态
    protected virtual void OnStateWaitRebornEnter()
    {
        _waitRebornTime = 0;
        _stateUpdateFunc = StateWaitRebornUpdate;
        // 创建死亡特效
        STGPlayerDeadEffect effect = EffectsManager.GetInstance().CreateEffectByType(EffectType.PlayerDeadEffect) as STGPlayerDeadEffect;
        effect.SetPosition(_curPos.x, _curPos.y);
        CommandManager.GetInstance().RunCommand(CommandConsts.PlayerMiss);
    }

    protected virtual void StateWaitRebornUpdate()
    {
        RotateCollisionPoint();
        _waitRebornTime++;
        if (_waitRebornTime >= _waitRebornDuration)
        {
            _nextState = eCharacterState.Appear;
        }
    }
    #endregion

    #endregion

    public CharacterBase()
    {
        _appearStartY = -280;
        _appearEndY = -200;
        _appearDuration = 60;
        _dyingDuration = 8;
        _waitRebornDuration = 60;
        _orderInLayer = 0;
    }

    public virtual void Init()
    {
        _trans = _character.transform;
        _charAniSp = _aniChar.GetAniObject().GetComponent<SpriteRenderer>();
        _leftBulletOffset = new Vector3(-8, 24);
        _rightBulletOffset = new Vector3(8, 24);
        // 设置判定点sprite
        GameObject collisionPointGo = ResourceManager.GetInstance().GetPrefab("Prefab/Character", "CollisionPointObject");
        _collisionPointTf = collisionPointGo.transform;
        _rotatePointTf = _collisionPointTf.Find("Point");
        UIManager.GetInstance().AddGoToLayer(collisionPointGo, LayerId.PlayerCollisionPoint);
        _preMoveMode = Consts.MoveModeHighSpeed;
        _curMoveMode = Consts.MoveModeHighSpeed;
        collisionPointGo.SetActive(false);
        _collisionPointRotateVec3 = new Vector3(0, 0, 3f);
        // 初始化副武器的层
        _subLayerTrans = _trans.Find("SubWeapons");
        // 初始化副武器列表
        _subWeapons = new SubWeaponBase[4];
        _inputShoot = false;
        _availableSubCount = 5;
        // 设置状态
        _curState = 0;
        _nextState = eCharacterState.Appear;
        _isCastingBomb = false;
        _isMovable = true;
    }

    public void Update()
    {
        UpdateState();
    }

    public void CheckMoveMode()
    {
        _preMoveMode = _curMoveMode;
        ePlayerMoveMode inputMoveMode = OperationController.GetInstance().GetKey(eSTGKey.KeyShift) ? ePlayerMoveMode.LowSpeed : ePlayerMoveMode.HighSpeed;
        if ( _curMoveMode != inputMoveMode)
        {
            _curMoveMode = inputMoveMode;
            if ( _curMoveMode == ePlayerMoveMode.HighSpeed )
            {
                _collisionPointTf.gameObject.SetActive(false);
            }
            else
            {
                _collisionPointTf.gameObject.SetActive(true);
            }
        }
        if ( _curMoveMode == ePlayerMoveMode.LowSpeed )
        {
            RotateCollisionPoint();
        }
    }

    /// <summary>
    /// 碰撞点的自旋
    /// </summary>
    protected void RotateCollisionPoint()
    {
        _rotatePointTf.transform.Rotate(_collisionPointRotateVec3);
    }

    /// <summary>
    /// 自己本体射击
    /// </summary>
    public void Shoot()
    {
        // 当前不能射击，直接返回
        if (!CanShoot()) return;
        if (!_isInShootingStatus) return;
        // 当前处于射击间隔，直接返回
        if (IsInShootCD()) return;
        CreateMainBullets();
        // 设置发射冷却
        _curShootCD = _shootCoolDown;
        SoundManager.GetInstance().Play("se_plst00", false);
    }

    /// <summary>
    /// 创建自机发射的子弹
    /// </summary>
    protected virtual void CreateMainBullets()
    {

    }

    public void Move()
    {
        if (!_isMovable) return;
        float speed = GetMoveSpeed();
        Vector2 pos = _curPos;
        bool isIdle = true;
        OperationController op = OperationController.GetInstance();
        if (op.IsDirKeyAvailable(eSTGKey.KeyLeft))
        {
            pos.x -= speed;
            isIdle = false;
            _aniChar.Play(AniActionType.Move, Consts.DIR_LEFT);
        }
        else if (op.IsDirKeyAvailable(eSTGKey.KeyRight))
        {
            pos.x += speed;
            isIdle = false;
            _aniChar.Play(AniActionType.Move, Consts.DIR_RIGHT);
        }
        if (op.IsDirKeyAvailable(eSTGKey.KeyDown))
        {
            pos.y -= speed;
        }
        else if (op.IsDirKeyAvailable(eSTGKey.KeyUp))
        {
            pos.y += speed;
        }
        // 检测是否越界
        if ( pos.x < Global.PlayerLBBorderPos.x )
        {
            pos.x = Global.PlayerLBBorderPos.x;
        }
        if (pos.y < Global.PlayerLBBorderPos.y)
        {
            pos.y = Global.PlayerLBBorderPos.y;
        }
        if ( pos.x > Global.PlayerRTBorderPos.x )
        {
            pos.x = Global.PlayerRTBorderPos.x;
        }
        if (pos.y > Global.PlayerRTBorderPos.y)
        {
            pos.y = Global.PlayerRTBorderPos.y;
        }
        // 计算额外运动参数
        pos.x += _extraVelocityX + _extraAcceX;
        pos.y += _extraVelocityY + _extraAcceY;
        _curPos = pos;
        if (isIdle)
        {
            _aniChar.Play(AniActionType.Idle, Consts.DIR_NULL);
        }
    }

    /// <summary>
    /// 获取移动速度
    /// </summary>
    public virtual float GetMoveSpeed()
    {
        float speed = _curMoveMode == ePlayerMoveMode.HighSpeed ? Consts.HighSpeed : Consts.SlowSpeed;
        return speed;
    }

    public void CastBomb()
    {
        if ( !_inputBomb || _curBombCD > 0 )
        {
            return;
        }
        if ( PlayerService.GetInstance().CastSpellCard() )
        {
            OnCastSpellCard();
            _bomb.Start();
            _curBombCD = _bombCoolDown;
            SetInvincible(true, _bombInvincibleDuration);
            _isCastingBomb = true;
            CommandManager.GetInstance().RunCommand(CommandConsts.PlayerCastSC);
        }
    }

    /// <summary>
    /// 自机施放符卡时调用
    /// <para>用于设置一些参数</para>
    /// </summary>
    protected virtual void OnCastSpellCard()
    {

    }

    public void SetInvincible(bool isInvincible,int duration)
    {
        _isInvincible = isInvincible;
        if ( _isInvincible )
        {
            _invincibleDuration = duration;
            _invincibleTime = 0;
        }
    }

    /// <summary>
    /// 设置玩家射击是否可用
    /// </summary>
    /// <param name="isAvailable"></param>
    /// <param name="duration"></param>
    public void SetShootAvailable(bool isAvailable,int duration=-1)
    {
        _isShootAvailable = isAvailable;
        if ( !isAvailable )
        {
            _shootUnavailableDuration = duration;
        }
    }

    protected virtual void UpdateSubWeapons()
    {

    }

    protected virtual void ResetSubWeapons()
    {

    }

    protected void UpdateSubWeaponsVisible()
    {
        int intPower = PlayerService.GetInstance().GetPower() / 100;
        if ( intPower != _availableSubCount )
        {
            SubWeaponBase subWeapon;
            int i;
            _availableSubCount = intPower;
            for (i=0;i<4;i++)
            {
                subWeapon = _subWeapons[i];
                subWeapon.SetActive(i < _availableSubCount);
            }
        }
    }

    protected void UpdateSubWeaponsShoot()
    {
        SubWeaponBase subWeapon;
        int i;
        // subWeapon.Update()
        for (i = 0; i < _availableSubCount; i++)
        {
            subWeapon = _subWeapons[i];
            subWeapon.Update();
        }
    }

    protected void UpdateInvincibleStatus()
    {
        _invincibleTime++;
        // 做闪烁特效
        if ( _invincibleTime % 12 == 0 )
        {
            Color color;
            if ( (_invincibleTime / 12) % 2 == 1 )
            {
                color = new Color(0, 0, 1);
            }
            else
            {
                color = new Color(1, 1, 1);
            }
            _charAniSp.material.SetColor("_Color", color);
        }
        if ( _invincibleTime >= _invincibleDuration )
        {
            _charAniSp.material.SetColor("_Color", new Color(1, 1, 1));
            _isInvincible = false;
        }
    }

    protected void UpdatePosition()
    {
        _trans.localPosition = new Vector3(_curPos.x,_curPos.y,-_orderInLayer);
        _collisionPointTf.localPosition = new Vector3(_curPos.x, _curPos.y, -_orderInLayer);
    }

    public void GetPosition(out float posX,out float posY)
    {
        posX = _curPos.x;
        posY = _curPos.y;
    }

    public Vector2 GetPosition()
    {
        return _curPos;
    }

    public void SetPosition(float posX,float posY)
    {
        if (_curState != eCharacterState.Appear)
        {
            _curPos = new Vector2(posX, posY);
        }
    }

    public void SetPosition(Vector2 pos)
    {
        if ( _curState != eCharacterState.Appear )
        {
            _curPos = pos;
        }
    }

    public void SetRotation(float value)
    {

    }

    public float GetRotation()
    {
        return 0;
    }

    protected void UpdateCollisionData()
    {
        Global.PlayerPos.x = _curPos.x;
        Global.PlayerPos.y = _curPos.y;
        Global.PlayerCollisionVec.x = _grazeRadius;
        Global.PlayerCollisionVec.y = _grazeRadius;
        Global.PlayerCollisionVec.z = _collisionRadius;
    }

    /// <summary>
    /// 更新信号
    /// </summary>
    protected void UpdateSignal()
    {
        if ( _curPos.y > Consts.AutoGetItemY )
        {
            float value = PlayerService.GetInstance().GetSignalValue();
            if ( value <= 100 )
            {
                value = value + 50f >= 100 ? 100 : value + 50;
                PlayerService.GetInstance().SetSignalValue(value);
            }
            else
            {
                PlayerService.GetInstance().AddToSignalValue(0.09f);
            }
        }
        else
        {
            PlayerService.GetInstance().AddToSignalValue(-0.3f);
        }
    }

    public void Clear()
    {
        // 人物动画
        AnimationManager.GetInstance().RemoveAnimation(_aniChar);
        _aniChar = null;
        _charAniSp.sprite = null;
        _charAniSp = null;
        _isCastingBomb = false;
        // B
        _bomb.Clear();
        _bomb = null;
        // 副武器
        _subLayerTrans = null;
        for (int i=0;i<_subWeapons.Length;i++)
        {
            _subWeapons[i].Clear();
        }
        _subWeapons = null;
        // 销毁
        GameObject.Destroy(_character);
        _character = null;
        // 判定点
        GameObject.Destroy(_collisionPointTf.gameObject);
        _collisionPointTf = null;
        _rotatePointTf = null;
        // 状态机
        _stateEnterFunc = null;
        _stateExitFunc = null;
        _stateUpdateFunc = null;
    }

    /// <summary>
    /// 是否可以移动
    /// </summary>
    public bool IsMovable
    {
        get { return _isMovable; }
        set { _isMovable = value; }
    }

    public bool CanShoot()
    {
        if (!STGStageManager.GetInstance().GetIsEnableToShoot()) return false;
        if (!_isShootAvailable) return false;
        return true;
    }

    /// <summary>
    /// 设置额外的直线运动的参数
    /// <para>一般是被引力场影响</para>
    /// </summary>
    /// <param name="v"></param>
    /// <param name="angle"></param>
    /// <param name="acce"></param>
    /// <param name="accAngle"></param>
    public virtual void AddExtraSpeedParas(float v,float angle,float acce,float accAngle)
    {
        _extraVelocityX += v * Mathf.Cos(angle * Mathf.Deg2Rad);
        _extraVelocityY += v * Mathf.Sin(angle * Mathf.Deg2Rad);
        _extraAcceX += acce * Mathf.Cos(accAngle * Mathf.Deg2Rad);
        _extraAcceY += acce * Mathf.Cos(accAngle * Mathf.Deg2Rad);
    }

    /// <summary>
    /// 重置额外直线运动的参数
    /// </summary>
    protected void ResetExtraSpeedParas()
    {
        _extraVelocityX = 0;
        _extraVelocityY = 0;
        _extraAcceX = 0;
        _extraAcceY = 0;
    }

    /// <summary>
    /// 更新射击状态
    /// </summary>
    /// <returns></returns>
    protected bool UpdateShootingStatus()
    {
        if (!_inputShoot)
        {
            // 刚好在上一帧松开射击键
            if (_isInputShootKey)
            {
                _shootTimeAfterKeyUp = 0;
            }
            else
            {
                _shootTimeAfterKeyUp++;
            }
            _isInputShootKey = _inputShoot;
            // 射击键松开的时间超过了松开之后的射击最大持续时间
            if (_shootTimeAfterKeyUp >= Consts.MaxShootDurationAfterKeyUp) return false;
        }
        else
        {
            _isInputShootKey = _inputShoot;
        }
        return true;
    }

    /// <summary>
    /// 当前是否在射击状态
    /// </summary>
    /// <returns></returns>
    public bool IsInShootingStatus()
    {
        return _isInShootingStatus;
    }

    protected bool IsInShootCD()
    {
        if ( _curShootCD == 0 )
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 更新射击、雷的CD
    /// </summary>
    protected void UpdateCD()
    {
        if (_curShootCD > 0)
        {
            _curShootCD--;
        }
        if ( _curBombCD > 0 )
        {
            _curBombCD--;
        }
        if ( !_isShootAvailable && _shootUnavailableDuration > 0 )
        {
            _shootUnavailableDuration--;
            if ( _shootUnavailableDuration == 0 )
            {
                _isShootAvailable = true;
            }
        }
        _isInShootingStatus = UpdateShootingStatus();
    }

    public void BeingHit()
    {
        if ( !_isInvincible )
        {
            _nextState = eCharacterState.Dying;
        }
    }

    /// <summary>
    /// 键盘输入是否射击
    /// </summary>
    public bool InputShoot
    {
        set { _inputShoot = value; }
        get { return _inputShoot; }
    }

    public bool InputBomb
    {
        set { _inputBomb = value; }
        get { return _inputBomb; }
    }

    /// <summary>
    /// 上一帧的移动模式
    /// </summary>
    public ePlayerMoveMode PreMoveMode
    {
        get { return _preMoveMode; }
    }

    /// <summary>
    /// 当前帧的移动模式
    /// </summary>
    public ePlayerMoveMode CurModeMode
    {
        get { return _curMoveMode; }
    }
}