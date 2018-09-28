using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : EnemyBase
{
    protected string _bossName;

    protected AnimationCharacter _bossAni;

    protected float _beginX, _beginY;

    protected bool _isPlayAni;
    protected int _playAniTime;
    protected int _playAniDuration;

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
        _segmentGoList = new List<GameObject>();
    }

    public void Init(string bossName)
    {
        _bossName = bossName;
        if ( _movableObj == null )
        {
            _movableObj = ObjectsPool.GetInstance().GetPoolClassAtPool<MovableObject>();
        }
        int initFuncRef = InterpreterManager.GetInstance().GetEnemyInitFuncRef(_bossName);
        InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
        InterpreterManager.GetInstance().CallLuaFunction(initFuncRef, 1);
        Logger.Log("Call InitFunc of Boss " + _bossName + " Complete!");
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
        float r = (256 - 4 - 1) * 0.35f * 0.5f;
        for (i=0;i<count;i++)
        {
            angle = 2 * Mathf.PI * tmpWeight / _totalWeight;
            // 旋转的角度
            _segmentGoList[i].transform.localRotation = Quaternion.Euler(0, 0, 360f * tmpWeight / _totalWeight);
            //Logger.Log("Segment Angle : " + 360f * tmpWeight / _totalWeight);
            angle += Mathf.PI / 2;
            // 计算位置
            _segmentGoList[i].transform.localPosition = new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, -1);
            tmpWeight += _weights[i];
            //Logger.Log("Segment Position : " + _segmentGoList[i].transform.localPosition);
        }
        _curSubPhase = count;
        //Logger.Log("beginRate = " + _beginRate + "curTotalRate = " + _curTotalRate);
    }

    protected void UpdateBloodBar()
    {
        //Logger.Log("BeginRate = " + _beginRate + "   hpRate = " + (_scCurHp / _scMaxHp) * _curTotalRate);
        _curRate = _beginRate + ((float)_curHp / _maxHp) * _curTotalRate;
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

    /// <summary>
    /// 重载setMaxHp
    /// <para>设置了这个值说明进入了新的符卡阶段</para>
    /// <para>因此重新计算血条位置</para>
    /// </summary>
    /// <param name="maxHp"></param>
    public override void SetMaxHp(int maxHp)
    {
        // 计算血量
        _curSubPhase--;
        _remainingWeight -= _weights[_curSubPhase];
        _beginRate = _remainingWeight / _totalWeight;
        _curTotalRate = _weights[_curSubPhase] / _totalWeight;
        base.SetMaxHp(maxHp);
    }

    /// <summary>
    /// 设置无敌时间
    /// </summary>
    /// <param name="duration"></param>
    public void SetInvincible(float duration)
    {
        _invincibleTimeLeft = (int)(duration * 60);
        _isInvincible = _invincibleTimeLeft != 0;
    }

    /// <summary>
    /// 显示BOSS血条
    /// </summary>
    /// <param name="value"></param>
    public void ShowBloodBar(bool value)
    {
        if ( _isShowBloodBar != value )
        {
            _isShowBloodBar = value;
            _bloodBarLayerTf.gameObject.SetActive(_isShowBloodBar);
        }
    }

    /// <summary>
    /// 符卡结束时，清除所有task
    /// </summary>
    public void OnSpellCardFinish()
    {
        ClearTasks();
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

    public override bool CanHit()
    {
        return true;
    }

    public override void GetHit(int damage)
    {
        if ( !_isInvincible )
        {
            _curHp -= damage;
            if ( _curHp <= 0 )
            {
                _curHp = 0;
            }
        }
    }

    public override void Clear()
    {
        // BOSS动画
        AnimationManager.GetInstance().RemoveAnimation(_bossAni);
        _bossAni = null;
        // 血条部分
        _bloodBarSp = null;
        _bloodBarLayerTf = null;
        _segmentGoList.Clear();
        _weights.Clear();
        GameObject.Destroy(_enemyGo);
        // movableObject
        ObjectsPool.GetInstance().RestorePoolClassToPool<MovableObject>(_movableObj);
        _movableObj = null;
        base.Clear();
    }
}
