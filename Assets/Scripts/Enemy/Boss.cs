using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : EnemyBase
{
    protected string _bossName;
    protected BossRefData _refData;

    protected AnimationCharacter _bossAni;

    protected float _beginX, _beginY;

    protected Task _bossTask;

    protected bool _isPlayAni;
    protected int _playAniTime;
    protected int _playAniDuration;

    protected SpellCard _sc;
    protected bool _isCastingSpell;
    protected Task _scTask;
    protected int _scTimeLeft;
    protected float _scMaxHp;
    protected float _scCurHp;

    protected bool _isInvincible;
    protected int _invincibleTimeLeft;

    protected SpriteRenderer _bloodBarSp;
    protected List<GameObject> _segmentGoList;
    protected Transform _bloodBarLayerTf;
    protected List<float> _weights;
    protected float _totalWeight;
    protected float _remainingWeight;
    protected float _beginRate;
    protected float _curTotalRate;
    protected float _curRate;
    protected int _curSubPhase;

    protected bool _isShowBloodBar;

    public Boss()
        :base()
    {
        _type = EnemyType.Boss;
        _movableObj = new MovableObject();
        _segmentGoList = new List<GameObject>();
    }

    public void Init(string bossName)
    {
        _bossName = bossName;
        _refData = EnemyManager.GetInstance().GetRefDataByName(bossName);
        InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
        InterpreterManager.GetInstance().CallLuaFunction(_refData.initFuncRef, 1);
        Logger.Log("Call InitFunc of Boss " + _bossName + " Complete!");
        _bossTask = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
        _bossTask.funcRef = _refData.taskFuncRef;
        InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
        InterpreterManager.GetInstance().CallTaskCoroutine(_bossTask, 1);
        Logger.Log("Start Task Coroutine of Boss " + _bossName + " !");
    }

    public void SetAni(string aniId)
    {
        if ( _enemyGo == null )
        {
            _enemyGo = ResourceManager.GetInstance().GetPrefab("Prefab/Boss", "Boss");
        }
        _bossAni = AnimationManager.GetInstance().CreateAnimation<AnimationCharacter>(_enemyGo, "Animation",LayerId.Enemy);
        _bloodBarLayerTf = _enemyGo.transform.Find("BloodBarLayer");
        _bloodBarLayerTf.gameObject.SetActive(false);
        _isShowBloodBar = false;
        _bloodBarSp = _bloodBarLayerTf.Find("BloodBar").GetComponent<SpriteRenderer>();
        _bossAni.Play(aniId, AniActionType.Idle, Consts.DIR_NULL);
    }

    public void SetCurPhaseData(List<float> weights)
    {
        _weights = weights;
        _curSubPhase = _weights.Count - 1;
        int i,count;
        GameObject segmentGo;
        _totalWeight = 0;
        for (i=0,count=_weights.Count;i<count;i++)
        {
            _totalWeight += _weights[i];
            segmentGo = ResourceManager.GetInstance().GetPrefab("Prefab/Boss","Segment");
            segmentGo.transform.parent = _bloodBarLayerTf;
            segmentGo.transform.localScale = new Vector3(35, 35, 1);
            _segmentGoList.Add(segmentGo);
        }
        _remainingWeight = _totalWeight;
        // 设置segmentGo的位置
        float tmpWeight = 0;
        float angle;
        float r = 42;
        for (i=0;i<count;i++)
        {
            angle = 2 * Mathf.PI * tmpWeight / _totalWeight;
            // 旋转的角度
            _segmentGoList[i].transform.localRotation = Quaternion.Euler(0, 0, 360f * tmpWeight / _totalWeight);
            //Logger.Log("Segment Angle : " + 360f * tmpWeight / _totalWeight);
            angle += Mathf.PI / 2;
            // 计算位置
            _segmentGoList[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0);
            tmpWeight += _weights[i];
            //Logger.Log("Segment Position : " + _segmentGoList[i].transform.localPosition);
        }
        _curSubPhase = count;
        //Logger.Log("beginRate = " + _beginRate + "curTotalRate = " + _curTotalRate);
    }

    protected void UpdateBloodBar()
    {
        //Logger.Log("BeginRate = " + _beginRate + "   hpRate = " + (_scCurHp / _scMaxHp) * _curTotalRate);
        _curRate = _beginRate + (_scCurHp / _scMaxHp) * _curTotalRate;
        _bloodBarSp.material.SetFloat("_Rate", _curRate);
        //Logger.Log("CurRate : " + _curRate);
    }

    public override void MoveToPos(float posX, float posY, int duration, InterpolationMode mode)
    {
        base.MoveToPos(posX, posY, duration, mode);
        if (posX > _curPos.x)
        {
            PlayAni(AniActionType.Move, Consts.DIR_RIGHT, duration);
            //_bossAni.Play(AniActionType.Move, Consts.DIR_RIGHT);
        }
        else if (posX < _curPos.x)
        {
            PlayAni(AniActionType.Move, Consts.DIR_LEFT, duration);
            //_bossAni.Play(AniActionType.Move, Consts.DIR_LEFT);
        }
    }

    public override void MoveTowards(float velocity, float angle, int duration)
    {
        while (angle < 0)
        {
            angle += 360f;
        }
        while (angle >= 360)
        {
            angle -= 360f;
        }
        base.MoveTowards(velocity, angle, duration);
        if (angle > 90 && angle < 270)
        {
            PlayAni(AniActionType.Move, Consts.DIR_LEFT, duration);
        }
        else if (angle < 90 || angle > 270)
        {
            PlayAni(AniActionType.Move, Consts.DIR_RIGHT, duration);
        }
    }

    public override void Update()
    {
        DoBossTask();
        if ( _isInvincible )
        {
            UpdateInvincibleStatue();
        }
        UpdateTask();
        _movableObj.Update();
        UpdatePos();
        if ( _isShowBloodBar )
        {
            UpdateBloodBar();
        }
        CheckCollisionWithCharacter();
        if ( _isPlayAni )
        {
            CheckPlayAni();
        }
    }

    public void PlayAni(AniActionType at,int dir,int duration)
    {
        _isPlayAni = true;
        _playAniTime = 0;
        _playAniDuration = duration;
        _bossAni.Play(at, dir);
    }

    protected void DoBossTask()
    {
        // 开启BOSS的Task
        if ( _bossTask != null && !_bossTask.isStarted )
        {
            InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
            InterpreterManager.GetInstance().CallTaskCoroutine(_bossTask,1);
            return;
        }
        // 当前不在taks线程挂起状态，直接执行task
        if ( !_isCastingSpell )
        {
            if ( !_bossTask.isFinish )
            {
                _bossTask.Update();
                if ( _bossTask.isFinish )
                {
                    Logger.Log("Boss Task Complete!");
                }
            }
            return;
        }
        if ( _isCastingSpell )
        {
            CastingSpellCard();
        }
    }

    public void EnterSpellCard(SpellCard sc)
    {
        _isCastingSpell = true;
        _sc = sc;
        _sc.isFinish = false;
        // 计算血量
        _curSubPhase--;
        _remainingWeight -= _weights[_curSubPhase];
        _beginRate = _remainingWeight / _totalWeight;
        _curTotalRate = _weights[_curSubPhase] / _totalWeight;
        Logger.Log("Enter SpellCard " + sc.name + " ");
        _scTimeLeft = (int)(_sc.spellDuration * 60);
        _scMaxHp = _sc.maxHp;
        _scCurHp = _sc.maxHp;
        // 设置无敌状态
        _invincibleTimeLeft = (int)(_sc.invincibleDuration * 60);
        _isInvincible = _invincibleTimeLeft != 0;
        // 显示符卡时间
        object[] data = { _scTimeLeft };
        CommandManager.GetInstance().RunCommand(CommandConsts.NewSpellCardTime, data);
    }

    public void CastingSpellCard()
    {
        if ( _scTask != null )
        {
            // 更新符卡时间
            _scTimeLeft--;
            object[] data = { _scTimeLeft };
            CommandManager.GetInstance().RunCommand(CommandConsts.UpdateSpellCardTime,data);
            // 判断是否已经击破sc或者sc时间已经到了
            if ( _scCurHp <= 0 || _scTimeLeft <= 0 )
            {
                if ( _scCurHp <= 0 )
                {
                    Logger.Log("SpellCard + " + _sc.name + " is finish,Reason : Destroy");
                }
                else
                {
                    Logger.Log("SpellCard + " + _sc.name + " is finish,Reason : TimedOut");
                }
                InterpreterManager.GetInstance().StopTaskThread(_scTask);
                _isCastingSpell = false;
                ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(_scTask);
                _scTask = null;
                ClearTasks();
                if ( _sc.finishFuncRef != 0 )
                {
                    InterpreterManager.GetInstance().CallLuaFunction(_sc.finishFuncRef, 0);
                }
                BulletsManager.GetInstance().ClearAllEnemyBullets();
                EnemyManager.GetInstance().RawEliminateAllEnemyByCode(false);
                // 更新血条
                // 执行下一个task
            }
            else
            {
                _scTask.Update();
            }
        }
        else
        {
            _scTask = ObjectsPool.GetInstance().GetPoolClassAtPool<Task>();
            _scTask.funcRef = _sc.taskRef;
            InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
            InterpreterManager.GetInstance().CallTaskCoroutine(_scTask, 1);
            // 显示血条
            _isShowBloodBar = true;
            _bloodBarLayerTf.gameObject.SetActive(true);
            Logger.Log("Casting SpellCard : " + _sc.name + " hp = " + _scMaxHp + " lastTime = " + _scTimeLeft);
        }
    }

    protected void UpdateInvincibleStatue()
    {
        _invincibleTimeLeft--;
        if ( _invincibleTimeLeft <= 0 )
        {
            _isInvincible = false;
        }
    }

    protected void CheckPlayAni()
    {
        _playAniTime++;
        if ( _playAniTime >= _playAniDuration )
        {
            _isPlayAni = false;
            _bossAni.Play(AniActionType.Idle, Consts.DIR_NULL);
        }
    }

    protected override void UpdatePos()
    {
        _curPos = _movableObj.GetPos();
        _enemyGo.transform.localPosition = _curPos;
    }

    public override void SetCollisionParams(float collisionHW, float collisionHH)
    {
        _collisionHalfWidth = collisionHW;
        _collisionHalfHeight = collisionHH;
    }

    public override int GetCollisionParams(out float arg1, out float arg2, out float arg3, out float arg4)
    {
        arg1 = _curPos.x;
        arg2 = _curPos.y;
        arg3 = _collisionHalfWidth;
        arg4 = _collisionHalfHeight;
        return Consts.CollisionType_Rect;
    }

    public override bool CanHit()
    {
        return true;
    }

    public override void GetHit(float damage)
    {
        if ( !_isInvincible )
        {
            _scCurHp -= damage;
            if ( _scCurHp <= 0 )
            {
                _scCurHp = 0;
            }
        }
    }

    public virtual float GetSpellCardMaxHp()
    {
        return _scMaxHp;
    }

    public virtual float GetSpellCardCurHp()
    {
        return _scCurHp;
    }

    public virtual float GetSpellCardHpRate()
    {
        return _scCurHp / _scMaxHp;
    }

    public virtual float GetSpellCardTimeLeftRate()
    {
        return _scTimeLeft / _sc.spellDuration / 60;
    }

    public override void Clear()
    {
        base.Clear();
        if ( _bossTask != null )
        {
            if ( !_bossTask.isFinish )
            {
                InterpreterManager.GetInstance().StopTaskThread(_bossTask.luaState, _bossTask.funcRef);
            }
            ObjectsPool.GetInstance().RestorePoolClassToPool<Task>(_bossTask);
            _bossTask = null;
        }
    }
}
