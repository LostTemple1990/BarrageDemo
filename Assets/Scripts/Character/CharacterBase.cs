using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBase :ICollisionObject,IGrazeObject{
    protected double _hitRadius = 0d;
    protected GameObject _character;
    protected Transform _trans;
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
    protected Vector3 _leftBulletOffset, _rightBulletOffset;

    protected BulletId _mainBulletId;
    protected BulletId _subBulletId;

    protected int _curMoveMode;
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
    protected Vector3 _curPos;

    protected delegate void StateEnterFunc();
    protected delegate void StateUpdateFunc();
    protected delegate void StateExitFunc();

    protected const int StateNormal = 1;
    protected const int StateAppear = 2;
    protected const int StateDying = 3;
    protected const int StateWaitReborn = 4;
    /// <summary>
    /// 当前状态
    /// </summary>
    protected int _curState;
    protected int _nextState;

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

    #region 状态机相关

    #region 状态机基本函数
    protected void UpdateState()
    {
        if ( _nextState != -1 && _curState != _nextState )
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
            case StateAppear:
                _stateEnterFunc = OnStateAppearEnter;
                //Logger.Log("Player Enter State : Appear");
                break;
            case StateNormal:
                _stateEnterFunc = OnStateNormalEnter;
                //Logger.Log("Player Enter State : Nromal");
                break;
            case StateDying:
                _stateEnterFunc = OnStateDyingEnter;
                //Logger.Log("Player Enter State : Dying");
                break;
            case StateWaitReborn:
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
        // 显示副武器
        UpdateSubWeaponsVisible();
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
            _nextState = StateNormal;
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
        UpdateSubWeaponsVisible();
        UpdateSubWeaponsShoot();
        UpdateCollisionData();
        if ( _isInvincible )
        {
            UpdateInvincibleStatus();
        }
        UpdatePosition();
    }

    protected virtual void OnStateNormalExit()
    {
        int i;
        // 隐藏所有副武器
        for (i=0;i<4;i++)
        {
            _subWeapons[i].SetActive(false);
        }
        _availableSubCount = -1;
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
    }

    protected virtual void StateDyingUpdate()
    {
        RotateCollisionPoint();
        UpdateCD();
        CastBomb();
        if ( _isCastingBomb )
        {
            _nextState = StateNormal;
            _character.SetActive(true);
            return;
        }
        _dyingTime++;
        if ( _dyingTime >= _dyingDuration )
        {
            _nextState = StateWaitReborn;
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
    }

    protected virtual void StateWaitRebornUpdate()
    {
        RotateCollisionPoint();
        _waitRebornTime++;
        if (_waitRebornTime >= _waitRebornDuration)
        {
            _nextState = StateAppear;
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
    }

    public virtual void Init()
    {
        _trans = _character.transform;
        _charAniSp = _aniChar.GetAniObject().GetComponent<SpriteRenderer>();
        _leftBulletOffset = new Vector3(-8, 24);
        _rightBulletOffset = new Vector3(8, 24);
        // 设置判定点sprite
        GameObject go = ResourceManager.GetInstance().GetPrefab("Prefab/Character", "CollisionPointObject");
        _collisionPointTf = go.transform;
        _rotatePointTf = _collisionPointTf.Find("Point");
        UIManager.GetInstance().AddGoToLayer(go, LayerId.PlayerCollisionPoint);
        _curMoveMode = Consts.SpeedMove;
        go.SetActive(false);
        _collisionPointRotateVec3 = new Vector3(0, 0, 3f);
        // 初始化副武器的层
        _subLayerTrans = _trans.Find("SubWeapons");
        // 初始化副武器列表
        _subWeapons = new SubWeaponBase[4];
        _inputShoot = false;
        _availableSubCount = 5;
        // 设置状态
        _curState = 0;
        _nextState = StateAppear;
        _isCastingBomb = false;
    }

    public void Update()
    {
        UpdateState();
    }

    public void CheckMoveMode()
    {
        if ( _curMoveMode != _inputMoveMode )
        {
            _curMoveMode = _inputMoveMode;
            if ( _curMoveMode == Consts.SpeedMove )
            {
                _collisionPointTf.gameObject.SetActive(false);
            }
            else
            {
                _collisionPointTf.gameObject.SetActive(true);
            }
        }
        if ( _curMoveMode == Consts.SlowMove )
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
        if (!CanShoot() || IsInShootCD() || !_inputShoot )
        {
            return;
        }
        BulletBase bullet = ObjectsPool.GetInstance().CreateBullet(_mainBulletId);
        bullet.SetToPosition(_curPos.x+_leftBulletOffset.x, _curPos.y + _leftBulletOffset.y);
        bullet = ObjectsPool.GetInstance().CreateBullet(_mainBulletId);
        bullet.SetToPosition(_curPos.x + _rightBulletOffset.x, _curPos.y + _rightBulletOffset.y);
        // 设置发射冷却
        _curShootCD = _shootCoolDown;
        SoundManager.GetInstance().Play("se_plst00", false);
    }

    public void Move()
    {
        if (!IsMovable())
        {
            return;
        }
        float speed = _inputMoveMode == Consts.SpeedMove ? Consts.HighSpeed : Consts.SlowSpeed;
        Vector3 pos = _curPos;
        bool isIdle = true;
        if ((_inputDir & Consts.DIR_LEFT) != 0)
        {
            pos.x -= speed;
            isIdle = false;
            _aniChar.Play(AniActionType.Move, Consts.DIR_LEFT);
        }
        else if ((_inputDir & Consts.DIR_RIGHT) != 0)
        {
            pos.x += speed;
            isIdle = false;
            _aniChar.Play(AniActionType.Move, Consts.DIR_RIGHT);
        }
        if ((_inputDir & Consts.DIR_DOWN) != 0)
        {
            pos.y -= speed;
        }
        else if ((_inputDir & Consts.DIR_UP) != 0)
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
        _curPos = pos;
        if (isIdle)
        {
            _aniChar.Play(AniActionType.Idle, Consts.DIR_NULL);
        }
    }

    public void CastBomb()
    {
        if ( !_inputBomb || _curBombCD > 0 )
        {
            return;
        }
        _bomb.Start();
        _curBombCD = _bombCoolDown;
        SetInvincible(true, _bombInvincibleDuration);
        _isCastingBomb = true;
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

    protected void UpdateSubWeaponsVisible()
    {
        // TODO 更新当前可用的副武器数目
        int intPower = PlayerService.GetInstance().GetPower() / 100;
        SubWeaponBase subWeapon;
        int i;
        if ( intPower != _availableSubCount )
        {
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
            subWeapon.Update(_curMoveMode);
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
        _trans.localPosition = _curPos;
        _collisionPointTf.localPosition = _curPos;
    }

    public void GetPosition(out float posX,out float posY)
    {
        posX = _curPos.x;
        posY = _curPos.y;
    }

    protected void UpdateCollisionData()
    {
        Global.PlayerPos.x = _curPos.x;
        Global.PlayerPos.y = _curPos.y;
        Global.PlayerCollisionVec.x = _grazeRadius;
        Global.PlayerCollisionVec.y = _grazeRadius;
        Global.PlayerCollisionVec.z = _collisionRadius;
    }

    public void Clear()
    {
        // 人物动画
        _aniChar.Clear();
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
    /// <returns></returns>
    protected bool IsMovable()
    {
        return true;
    }

    public bool CanShoot()
    {
        return true;
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
    }

    public void BeingHit()
    {
        if ( !_isInvincible )
        {
            _nextState = StateDying;
        }
    }

    public virtual int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        arg1 = _curPos.x;
        arg2 = _curPos.y;
        arg3 = _collisionRadius;
        arg4 = _collisionRadius;
        return Consts.CollisionType_Circle;
    }

    public virtual int GetGrazeParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        arg1 = _curPos.x;
        arg2 = _curPos.y;
        arg3 = _grazeRadius;
        arg4 = _grazeRadius;
        return Consts.GrazeType_Rect;
    }

    /// <summary>
    /// 键盘输入的高低速模式
    /// </summary>
    public int InputMoveMode
    {
        set { _inputMoveMode = value; }
    }

    /// <summary>
    /// 键盘输入方向
    /// </summary>
    public int InputDir
    {
        set { _inputDir = value; }
        get { return _inputDir; }
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
}