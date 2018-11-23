using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCParasChange : BulletComponent
{
    private EnemyBulletBase _bullet;
    private int _listCount;
    private List<BulletParasChangeData> _changeList;

    public override void Init(EnemyBulletBase bullet)
    {
        _bullet = bullet;
        _changeList = new List<BulletParasChangeData>();
        _listCount = 0;
    }

    public override void Update()
    {
        if ( _bullet == null )
        {
            return;
        }
        for (int i=0;i<_listCount;i++)
        {
            UpdateChangeData(i);
        }
    }

    public void AddParaChangeEvent(BulletParaType para, ParaChangeMode changeMode,float changeValue,int delay,
        float duration, InterpolationMode intMode)
    {
        BulletParasChangeData changeData = CreateChangeData(para, intMode);
        changeData.changeMode = changeMode;
        changeData.changeValue = changeValue;
        changeData.delay = delay;
        changeData.changeTime = 0;
        changeData.changeDuration = duration;
        _changeList.Add(changeData);
        _listCount++;
    }

    private BulletParasChangeData CreateChangeData(BulletParaType paraType,InterpolationMode intMode)
    {
        BulletParasChangeData changeData = ObjectsPool.GetInstance().GetPoolClassAtPool<BulletParasChangeData>();
        changeData.paraType = paraType;
        changeData.mode = intMode;
        changeData.GetInterpolationValueFunc = MathUtil.GetInterpolationFloatFunc(changeData.mode);
        return changeData;
    }

    private void UpdateChangeData(int idx)
    {
        BulletParasChangeData changeData = _changeList[idx];
        if ( !changeData.isFinish )
        {
            if ( changeData.delay > 0 )
            {
                changeData.delay--;
            }
            else
            {
                if (!changeData.isCached) CacheChangeData(changeData);
                changeData.changeTime++;
                float changeValue = changeData.GetInterpolationValueFunc(changeData.begin, changeData.end, changeData.changeTime, changeData.changeDuration);
                _bullet.SetBulletPara(changeData.paraType, changeValue);
                if (changeData.changeTime >= changeData.changeDuration)
                {
                    changeData.isFinish = true;
                }
            }
        }
    }

    private void CacheChangeData(BulletParasChangeData changeData)
    {
        changeData.isCached = true;
        float beginValue;
        if (!_bullet.GetBulletPara(changeData.paraType, out beginValue))
        {
            changeData.isFinish = true;
            Logger.LogWarn("ParasChange Warn :ParaType is not valid for bullet");
        }
        else
        {
            changeData.begin = beginValue;
            switch (changeData.changeMode)
            {
                case ParaChangeMode.ChangeTo:
                    changeData.end = changeData.changeValue;
                    break;
                case ParaChangeMode.IncBy:
                    changeData.end = changeData.begin + changeData.changeValue;
                    break;
                case ParaChangeMode.DecBy:
                    changeData.end = changeData.begin - changeData.changeValue;
                    break;
            }
        }
    }
     
    public override void Clear()
    {
        BulletParasChangeData changeData;
        for (int i = 0; i < _listCount; i++)
        {
            changeData = _changeList[i];
            ObjectsPool.GetInstance().RestorePoolClassToPool<BulletParasChangeData>(changeData);
        }
        _changeList.Clear();
    }
}

public class BulletParasChangeData : IPoolClass
{
    /// <summary>
    /// 延迟执行的帧数
    /// </summary>
    public int delay;
    public BulletParaType paraType;
    public ParaChangeMode changeMode;
    public InterpolationMode mode;
    public float changeValue;
    public float begin;
    public float end;
    public float changeTime;
    public float changeDuration;
    public bool isCached;
    public bool isFinish;
    public MathUtil.InterpolationFloatFunc GetInterpolationValueFunc;

    public BulletParasChangeData()
    {
        isCached = false;
        isFinish = false;
    }

    public void Clear()
    {
        isCached = false;
        isFinish = false;
        GetInterpolationValueFunc = null;
    }
}

/// <summary>
/// 子弹移动相关的变量对应enum
/// </summary>
public enum BulletParaType :byte
{
    /// <summary>
    /// 当前速度
    /// </summary>
    Velocity = 1,
    /// <summary>
    /// 当前速度方向
    /// </summary>
    VAngel = 2,
    /// <summary>
    /// 当前加速度
    /// </summary>
    Acc = 3,
    /// <summary>
    /// 当前加速度方向
    /// </summary>
    AccAngle = 4,
    /// <summary>
    /// 极坐标半径
    /// </summary>
    CurveRadius = 5,
    /// <summary>
    /// 圆周运动角度
    /// </summary>
    CurveAngle = 6,
    /// <summary>
    /// 圆周运动半径增量
    /// </summary>
    CurveDeltaR = 7,
    /// <summary>
    /// 圆周运动角速度增量
    /// </summary>
    CurveOmiga = 8,
    CurveCenterX = 9,
    CurveCenterY = 10,
    Alpha = 15,
    ScaleX = 20,
    ScaleY = 21,
}

/// <summary>
/// 插值方式
/// </summary>
public enum InterpolationMode : byte
{
    None = 0,
    Linear = 1,
    EaseInQuad = 2,
    EaseOutQuad = 3,
    EaseInOutQuad = 4,
    Sin = 9,
    Cos = 10,
}

public enum ParaChangeMode : byte
{
    ChangeTo = 1,
    IncBy = 2,
    DecBy = 3,
}
