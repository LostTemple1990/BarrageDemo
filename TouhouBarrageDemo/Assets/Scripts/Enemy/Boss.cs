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
    /// <summary>
    /// 是否显示位置提示
    /// </summary>
    protected bool _isShowPosHint;
    /// <summary>
    /// 显示魔法阵
    /// </summary>
    protected bool _isShowAura;
    private eAuraState _auraState;
    private int _auraTime;
    /// <summary>
    /// 缓存魔法阵消失的时间
    /// </summary>
    private int _auraDisappearTime;
    private Transform _auraTf;
    /// <summary>
    /// 魔法阵的显示状态
    /// </summary>
    enum eAuraState : byte
    {
        Creating,
        Normal,
        Disappearing,
    }
    /// <summary>
    /// 背后魔法阵生成动画的时间
    /// </summary>
    private const int AuraCreatingDuration = 180;
    /// <summary>
    /// 背后魔法阵消失动画的时间
    /// </summary>
    private const int AuraDisappearingDuration = 180;

    private GameObject _scLifeGo;
    private Mesh _scLifeBorderMesh;
    private Transform _scLifeBorderTf;
    private Mesh _scLifeMesh;
    private Transform _scLifeTf;

    private bool _isShowSCHpAura;
    private int _scLifeTime;
    private const int SpellCardLifeCreatingDuration = 120;
    // 符卡生命框的旋转角速度
    private const float SpellCardLifeOmega = 3;
    private const float SpellCardLifeBorderCreatingScale = 5;
    private const float SpellCardLifeCreatingScale = 0;
    private const float SpellCardLifeEndScale = 0.5f;
    /// <summary>
    /// 符卡血量的显示状态
    /// </summary>
    enum eSpellCardLifeState : byte
    {
        Creating,
        Normal,
        Disappearing,
    }

    private eSpellCardLifeState _scLifeState;

    public Boss()
        :base()
    {
        _type = eEnemyType.Boss;
        _segmentGoList = new List<GameObject>();
    }

    public void Init(string bossName)
    {
        _bossName = bossName;
        int initFuncRef = InterpreterManager.GetInstance().GetEnemyInitFuncRef(_bossName);
        InterpreterManager.GetInstance().AddPara(this, LuaParaType.LightUserData);
        InterpreterManager.GetInstance().CallLuaFunction(initFuncRef, 1);
        _checkOutOfBorder = false;
        _isShowPosHint = false;
        Logger.Log("Call InitFunc of Boss " + _bossName + " Complete!");
    }

    public void SetAni(string aniId)
    {
        if ( _enemyGo == null )
        {
            _enemyGo = ResourceManager.GetInstance().GetPrefab("Prefab/Boss", "Boss");
        }
        _enemyTf = _enemyGo.transform;
        _bossAni = AnimationManager.GetInstance().CreateAnimation<AnimationCharacter>(_enemyGo, "Animation",LayerId.Enemy);
        _bloodBarLayerTf = _enemyTf.Find("BloodBarLayer");
        _bloodBarLayerTf.gameObject.SetActive(false);
        _isShowBloodBar = false;
        _bloodBarSp = _bloodBarLayerTf.Find("BloodBar").GetComponent<SpriteRenderer>();
        // 魔法阵
        _isShowAura = false;
        _auraTf = _enemyTf.Find("MagicSquare");
        // 符卡血条
        _scLifeGo = _enemyTf.Find("SpellCardLife").gameObject;
        _scLifeBorderTf = _enemyTf.Find("SpellCardLife/Border");
        _scLifeBorderMesh = _scLifeBorderTf.GetComponent<MeshFilter>().mesh;
        _scLifeTf = _enemyTf.Find("SpellCardLife/Life");
        _scLifeMesh = _scLifeTf.GetComponent<MeshFilter>().mesh;
        PopulateMesh(_scLifeBorderMesh, 128, 16, 5);
        PopulateMesh(_scLifeMesh, 112, 16, 3);
        _isShowAura = false;
        _isShowSCHpAura = false;
        // 默认动作
        _bossAni.Play(aniId, AniActionType.Idle, Consts.DIR_NULL);
    }

    /// <summary>
    /// 设置BOSS阶段数据
    /// </summary>
    /// <param name="weights">权重</param>
    /// <param name="isMultiPhase">是否为多个阶段</param>
    public void SetCurPhaseData(List<float> weights,bool isMultiPhase)
    {
        int i,count;
        GameObject segmentGo;
        _totalWeight = 0;
        for (i=0,count= weights.Count;i<count;i++)
        {
            _totalWeight += weights[i];
            segmentGo = ResourceManager.GetInstance().GetPrefab("Prefab/Boss","Segment");
            segmentGo.transform.parent = _bloodBarLayerTf;
            segmentGo.transform.localScale = new Vector3(35, 35, 1);
            _segmentGoList.Add(segmentGo);
        }
        _remainingWeight = _totalWeight;
        weights.Reverse();
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
            tmpWeight += weights[i];
            //Logger.Log("Segment Position : " + _segmentGoList[i].transform.localPosition);
        }
        if ( isMultiPhase )
        {
            _weights = weights;
            _curSubPhase = count;
        }
        else
        {
            _weights = new List<float> { _totalWeight };
            _curSubPhase = 1;
        }
        //Logger.Log("beginRate = " + _beginRate + "curTotalRate = " + _curTotalRate);
    }

    protected void UpdateBloodBar()
    {
        //Logger.Log("BeginRate = " + _beginRate + "   hpRate = " + (_scCurHp / _scMaxHp) * _curTotalRate);
        _curRate = _beginRate + ((float)_curHp / _maxHp) * _curTotalRate;
        _bloodBarSp.material.SetFloat("_Rate", _curRate);
        //Logger.Log("CurRate : " + _curRate);
    }

    public override void MoveTo(float posX, float posY, int duration, InterpolationMode mode)
    {
        base.MoveTo(posX, posY, duration, mode);
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
        angle = MathUtil.ClampAngle(angle);
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
            UpdateInvincibleStatus();
        }
        UpdateTask();
        if ( _movableObj.IsActive() )
        {
            _movableObj.Update();
            _curPos = _movableObj.GetPos();
        }
        if ( _isShowBloodBar )
        {
            UpdateBloodBar();
        }
        CheckCollisionWithCharacter();
        if ( _isPlayAni )
        {
            CheckPlayAni();
        }
        RenderTransform();
        if (_isShowAura)
            RenderAura();
        if (_isShowSCHpAura)
            RenderSpellCardLife();
        // 添加背景扭曲效果
        //object[] datas = new object[] { _curPos.x, _curPos.y, 128f, 0.01f, new Color(0.62f, 0.22f, 0.61f, 1) };
        //CommandManager.GetInstance().RunCommand(CommandConsts.UpdateBgDistortEffectProps, datas);
    }

    public override void PlayAni(AniActionType at, int dir = Consts.DIR_NULL, int duration = int.MaxValue)
    {
        if (_bossAni.Play(at, dir))
        {
            _isPlayAni = true;
            _playAniTime = 0;
            _playAniDuration = duration;
        }
    }

    /// <summary>
    /// 重载setMaxHp
    /// <para>设置了这个值说明进入了新的符卡阶段</para>
    /// <para>因此重新计算血条位置</para>
    /// </summary>
    /// <param name="maxHp"></param>
    public override void SetMaxHp(float maxHp)
    {
        // 计算血量
        _curSubPhase--;
        _remainingWeight -= _weights[_curSubPhase];
        _beginRate = _remainingWeight / _totalWeight;
        _curTotalRate = _weights[_curSubPhase] / _totalWeight;
        // 重置抵抗消除标识
        _resistEliminateFlag = 0;
        base.SetMaxHp(maxHp);
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
    /// 显示Boss的位置提示
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowPosHint(bool isShow)
    {
        _isShowPosHint = isShow;
        if (isShow)
        {
            List<object> datas = new List<object> { this, true, _curPos.x };
            CommandManager.GetInstance().RunCommand(CommandConsts.ShowBossPosHint, datas);
        }
        else
        {
            List<object> datas = new List<object> { this, false };
            CommandManager.GetInstance().RunCommand(CommandConsts.ShowBossPosHint, datas);
        }
    }

    /// <summary>
    /// 显示魔法阵
    /// </summary>
    /// <param name="isShow">显示、隐藏</param>
    /// <param name="ani">是否有出现、消失动画</param>
    public void ShowAura(bool isShow,bool ani)
    {
        if (isShow)
        {
            if (!ani)
            {
                _auraState = eAuraState.Normal;
                _auraTf.localScale = Vector3.one;
                _auraTime = 0;
            }
            else
            {
                _auraState = eAuraState.Creating;
                _auraTime = 0;
            }
            _isShowAura = isShow;
        }
        else
        {
            if (!_isShowAura)
                return;
            if (!ani)
            {
                _isShowAura = false;
                _auraTf.localScale = Vector3.zero;
            }
            else
            {
                _auraDisappearTime = _auraTime;
                _auraTime = 0;
                _auraState = eAuraState.Disappearing;
            }
        }
    }

    private void RenderAura()
    {
        if (_auraState == eAuraState.Creating)
        {
            _auraTime++;
            float scale = (float)_auraTime / AuraCreatingDuration;
            _auraTf.localScale = new Vector3(scale, scale, 1);
            float zAngle = AuraCreatingDuration * 1f / 15 * _auraTime - 0.5f * _auraTime * _auraTime * 1f / 15;
            _auraTf.localEulerAngles = new Vector3(0, 0, zAngle);
            if (_auraTime >= AuraCreatingDuration)
            {
                _auraTime = 0;
                _auraState = eAuraState.Normal;
            }
        }
        else if (_auraState == eAuraState.Normal)
        {
            _auraTime++;
            // 后一个数字t是周期，t帧为一个周期，前一个360为角度
            // 通常顺序是 (_auraTime / t) * 360
            float angle = _auraTime * 360f / 720;
            _auraTf.localEulerAngles = new Vector3(45f * Mathf.Sin(angle * Mathf.Deg2Rad), -45f * Mathf.Sin(angle * 0.5f * Mathf.Deg2Rad), _auraTime * 0.6f);
        }
        else if (_auraState == eAuraState.Disappearing)
        {
            _auraTime++;
            float scale = 1 - (float)_auraTime / AuraCreatingDuration;
            _auraTf.localScale = new Vector3(scale, scale, 1);
            float realTime = _auraTime + _auraDisappearTime;
            float angle = realTime * 360f / 360;
            _auraTf.localEulerAngles = new Vector3(-45f * Mathf.Sin(angle * Mathf.Deg2Rad), 45f * Mathf.Sin(angle * Mathf.Deg2Rad), realTime * 0.6f);
            if (_auraTime >= AuraDisappearingDuration)
                _isShowAura = false;
        }
    }

    private const int CircleSegmentCount = 128;
    private const int DefaultTextureCount = 6;
    private const float DefaultTextureHeight = 128f;
    private const float DefaultCircumference = DefaultTextureHeight * DefaultTextureCount;
    private const float DefaultRadius = DefaultCircumference / 2 / Mathf.PI;

    private void PopulateMesh(Mesh mesh,float radius,float width,int textureIndex)
    {
        float factor = radius / DefaultRadius;
        float minR = radius - width / 2;
        float maxR = radius + width / 2;
        int index, tmpIndex;
        Vector3[] vers = new Vector3[(CircleSegmentCount+1) << 1];
        Vector2[] uvs = new Vector2[(CircleSegmentCount + 1) << 1];
        Color[] colors = new Color[(CircleSegmentCount + 1) << 1];
        float startU = textureIndex / 8f;
        float endU = (textureIndex+1) / 8f;
        index = 0;
        for (int i = 0; i <= CircleSegmentCount; i++)
        {
            float angle = 360f * i / CircleSegmentCount;
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            float v = DefaultCircumference * i / CircleSegmentCount / (DefaultTextureHeight * factor);
            vers[index] = new Vector3(minR * cos, minR * sin);
            colors[index] = new Color(1, 1, 1, 0.8f);
            uvs[index++] = new Vector2(startU, v);
            vers[index] = new Vector3(maxR * cos, maxR * sin);
            colors[index] = new Color(1, 1, 1, 0.8f);
            uvs[index++] = new Vector2(endU, v);
        }
        // 构建三角形
        int[] triArr = new int[CircleSegmentCount * 6];
        index = 0;
        for (int i = 0; i < CircleSegmentCount; i++)
        {
            tmpIndex = i << 1;
            triArr[index++] = tmpIndex;
            triArr[index++] = tmpIndex + 2;
            triArr[index++] = tmpIndex + 1;
            triArr[index++] = tmpIndex + 1;
            triArr[index++] = tmpIndex + 2;
            triArr[index++] = tmpIndex + 3;
        }
        mesh.vertices = vers;
        mesh.triangles = triArr;
        mesh.uv = uvs;
        mesh.colors = colors;
    }

    public void ShowSpellCardHpAura(bool isShow)
    {
        if (_isShowSCHpAura != isShow)
        {
            _isShowSCHpAura = isShow;
            _scLifeGo.SetActive(isShow);
            if (isShow)
            {
                _scLifeState = eSpellCardLifeState.Creating;
                _scLifeTime = 0;
                _scLifeTf.localScale = new Vector3(SpellCardLifeCreatingScale, SpellCardLifeCreatingScale, 1);
                _scLifeBorderTf.localScale = new Vector3(SpellCardLifeBorderCreatingScale, SpellCardLifeBorderCreatingScale, 1);
            }
        }
    }

    private void RenderSpellCardLife()
    {
        if (_scLifeState == eSpellCardLifeState.Creating)
        {
            _scLifeTime++;
            // 缩放
            float scale = MathUtil.GetEaseOutQuadInterpolation(SpellCardLifeCreatingScale, 1, _scLifeTime, SpellCardLifeCreatingDuration);
            _scLifeTf.localScale = new Vector3(scale, scale, 1);
            scale = MathUtil.GetEaseOutQuadInterpolation(SpellCardLifeBorderCreatingScale, 1, _scLifeTime, SpellCardLifeCreatingDuration);
            _scLifeBorderTf.localScale = new Vector3(scale, scale, 1);
            // 旋转
            _scLifeTf.localEulerAngles = new Vector3(0, 0, SpellCardLifeOmega * _scLifeTime);
            _scLifeBorderTf.localEulerAngles = new Vector3(0, 0, -SpellCardLifeOmega * _scLifeTime);
            if (_scLifeTime >= SpellCardLifeCreatingDuration)
            {
                _scLifeState = eSpellCardLifeState.Normal;
            }
        }
        else if (_scLifeState == eSpellCardLifeState.Normal)
        {
            _scLifeTime++;
            // 旋转
            _scLifeTf.localEulerAngles = new Vector3(0, 0, SpellCardLifeOmega * _scLifeTime);
            _scLifeBorderTf.localEulerAngles = new Vector3(0, 0, -SpellCardLifeOmega * _scLifeTime);
            // 根据血量缩放
            float factor = 1 - _curHp / _maxHp;
            float scale = Mathf.Lerp(1, SpellCardLifeEndScale, factor);
            _scLifeTf.localScale = new Vector3(scale, scale, 1);
            _scLifeBorderTf.localScale = new Vector3(scale, scale, 1);
        }
    }

    /// <summary>
    /// 符卡结束时，清除所有task
    /// </summary>
    public void OnSpellCardFinish()
    {
        ShowBloodBar(false);
        ShowSpellCardHpAura(false);
        ClearTasks();
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

    protected override void RenderTransform()
    {
        base.RenderTransform();
        if (_isShowPosHint)
        {
            List<object> datas = new List<object> { this, true, _curPos.x };
            CommandManager.GetInstance().RunCommand(CommandConsts.ShowBossPosHint, datas);
        }
    }

    public override void SetCollisionParams(float collisionHW, float collisionHH)
    {
        _collisionHalfWidth = collisionHW;
        _collisionHalfHeight = collisionHH;
    }

    public override bool CanHit()
    {
        if (!_isInteractive)
            return false;
        return true;
    }

    public override void TakeDamage(float damage,eEliminateDef eliminateType=eEliminateDef.PlayerBullet)
    {
        if ((_resistEliminateFlag & (int)eliminateType) != 0) return;
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
        if ( _weights != null )
        {
            _weights.Clear();
        }
        GameObject.Destroy(_enemyGo);
        // Boss位置提示
        if (_isShowPosHint)
        {
            List<object> datas = new List<object> { this, false };
            CommandManager.GetInstance().RunCommand(CommandConsts.ShowBossPosHint, datas);
        }
        _auraTf = null;
        _scLifeGo = null;
        _scLifeMesh = null;
        _scLifeBorderMesh = null;
        _scLifeBorderTf = null;
        _scLifeTf = null;
        base.Clear();
        CommandManager.GetInstance().RunCommand(CommandConsts.RemoveBgDistortEffect);
    }
}
