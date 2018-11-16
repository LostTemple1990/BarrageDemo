using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCParasChange : BulletComponent
{
    private EnemyBulletMovable _bullet;
    private int _listCount;
    private List<BulletParasChangeData> _changeList;

    public override void Init(EnemyBulletBase bullet)
    {
        _bullet = bullet as EnemyBulletMovable;
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

    public void AddParaChangeEvent(MovePara para, ParaChangeMode changeMode,float changeValue,int delay,
        InterpolationMode intMode,float duration)
    {
        BulletParasChangeData changeData = CreateChangeData(para, intMode);
        changeData.delay = delay;
        changeData.changeTime = 0;
        changeData.changeDuration = duration;
        switch ( changeMode )
        {
            case ParaChangeMode.ChangeTo:
                changeData.end = changeValue;
                break;
            case ParaChangeMode.IncBy:
                changeData.end = changeData.begin + changeValue;
                break;
            case ParaChangeMode.DecBy:
                changeData.end = changeData.begin - changeValue;
                break;
        }
        _changeList.Add(changeData);
        _listCount++;
    }

    private BulletParasChangeData CreateChangeData(MovePara movePara,InterpolationMode intMode)
    {
        BulletParasChangeData changeData = new BulletParasChangeData();
        changeData.para = movePara;
        switch (changeData.para)
        {
            case MovePara.Velocity:
                changeData.begin = _bullet.getVelocity();
                changeData.SetParaFunc = _bullet.SetVelocity;
                break;
            case MovePara.VAngel:
                changeData.begin = _bullet.GetVAnlge();
                changeData.SetParaFunc = _bullet.SetVAngle;
                break;
            case MovePara.Acc:
                changeData.begin = _bullet.GetAcce();
                changeData.SetParaFunc = _bullet.SetAcce;
                break;
            case MovePara.AccAngle:
                changeData.begin = _bullet.GetAccAngle();
                changeData.SetParaFunc = _bullet.SetAccAngle;
                break;
            case MovePara.CircleRadius:
                changeData.begin = _bullet.GetCirRadius();
                changeData.SetParaFunc = _bullet.SetCirRadius;
                break;
            case MovePara.CircleAngle:
                changeData.begin = _bullet.GetCirAngle();
                changeData.SetParaFunc = _bullet.SetCirAngle;
                break;
            case MovePara.CircleDeltaR:
                changeData.begin = _bullet.GetCirDeltaR();
                changeData.SetParaFunc = _bullet.SetCirDeltaR;
                break;
            case MovePara.CircleOmiga:
                changeData.begin = _bullet.GetCirOmiga();
                changeData.SetParaFunc = _bullet.SetCirOmiga;
                break;
            case MovePara.CircleCenterX:
                changeData.begin = _bullet.GetCirCenterX();
                changeData.SetParaFunc = _bullet.SetCirCenterX;
                break;
            case MovePara.CircleCenterY:
                changeData.begin = _bullet.GetCirCenterY();
                changeData.SetParaFunc = _bullet.SetCirCenterY;
                break;

        }
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
                changeData.changeTime++;
                float changeValue = changeData.GetInterpolationValueFunc(changeData.begin, changeData.end, changeData.changeTime, changeData.changeDuration);
                changeData.SetParaFunc(changeValue);
                if (changeData.changeTime >= changeData.changeDuration)
                {
                    changeData.isFinish = true;
                    changeData.GetInterpolationValueFunc = null;
                    changeData.SetParaFunc = null;
                }
            }
        }
    }
     
    public override void Clear()
    {
        BulletParasChangeData changeData;
        for (int i = 0; i < _listCount; i++)
        {
            changeData = _changeList[i];
            if ( !changeData.isFinish )
            {
                changeData.isFinish = true;
                changeData.GetInterpolationValueFunc = null;
                changeData.SetParaFunc = null;
            }
        }
        _changeList.Clear();
    }
}

public class BulletParasChangeData
{
    /// <summary>
    /// 延迟执行的帧数
    /// </summary>
    public int delay;
    public MovePara para;
    public InterpolationMode mode;
    public float begin;
    public float end;
    public float changeTime;
    public float changeDuration;
    public bool isFinish;
    public delegate void SetPara(float value);
    public MathUtil.InterpolationFloatFunc GetInterpolationValueFunc;
    public SetPara SetParaFunc;
}

/// <summary>
/// 子弹移动相关的变量对应enum
/// </summary>
public enum MovePara :byte
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
    CircleRadius = 5,
    /// <summary>
    /// 圆周运动角度
    /// </summary>
    CircleAngle = 6,
    /// <summary>
    /// 圆周运动半径增量
    /// </summary>
    CircleDeltaR = 7,
    /// <summary>
    /// 圆周运动角速度增量
    /// </summary>
    CircleOmiga = 8,
    CircleCenterX = 9,
    CircleCenterY = 10,
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
}

public enum ParaChangeMode : byte
{
    ChangeTo = 1,
    IncBy = 2,
    DecBy = 3,
}
