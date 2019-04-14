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

    /// <summary>
    /// 添加改变参数的事件
    /// </summary>
    /// <param name="para">参数类型</param>
    /// <param name="changeMode">改变的模式，(增加到、减少到、变化到)</param>
    /// <param name="changeValue">改变的值</param>
    /// <param name="valueOffset">改变的值的上下偏移</param>
    /// <param name="delay">延迟</param>
    /// <param name="duration">变化的持续时间</param>
    /// <param name="intMode">变化的插值方式</param>
    /// <param name="repeatCount">重复次数</param>
    /// <param name="repeatInterval">重复的时间间隔</param>
    public void AddParaChangeEvent(BulletParaType para, ParaChangeMode changeMode,
       ParaChangeValue value,int delay,float duration, InterpolationMode intMode,int repeatCount,int repeatInterval)
    {
        BulletParasChangeData changeData = CreateChangeData(para, intMode);
        changeData.changeMode = changeMode;
        changeData.changeValue = value;
        changeData.delay = delay;
        changeData.changeTime = 0;
        changeData.changeDuration = duration;
        changeData.repeatCount = repeatCount;
        changeData.repeatInterval = repeatInterval;
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
                if (changeData.repeatIntervalTime == 0 )
                {
                    if (!changeData.isCached) CacheChangeData(changeData);
                    changeData.changeTime++;
                    float changeValue = changeData.GetInterpolationValueFunc(changeData.begin, changeData.end, changeData.changeTime, changeData.changeDuration);
                    _bullet.SetBulletPara(changeData.paraType, changeValue);
                    if (changeData.changeTime >= changeData.changeDuration)
                    {
                        changeData.repeatCount--;
                        if (changeData.repeatCount <= 0)
                        {
                            changeData.isFinish = true;
                        }
                        else
                        {
                            changeData.repeatIntervalTime = changeData.repeatInterval;
                            // 再次执行时需要重新计算一下各个数值
                            changeData.isCached = false;
                        }
                    }
                }
                if (changeData.repeatIntervalTime > 0) changeData.repeatIntervalTime--;
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
            float value = GetValueByChangeValue(changeData.changeValue);
            switch (changeData.changeMode)
            {
                case ParaChangeMode.ChangeTo:
                    changeData.end = value;
                    break;
                case ParaChangeMode.IncBy:
                    changeData.end = changeData.begin + value;
                    break;
                case ParaChangeMode.DecBy:
                    changeData.end = changeData.begin - value;
                    break;
            }
        }
    }

    /// <summary>
    /// 根据ChangeValue的值获取对应的值并返回
    /// </summary>
    /// <param name="changeValue"></param>
    /// <returns></returns>
    private float GetValueByChangeValue(ParaChangeValue changeValue)
    {
        float value = 0;
        if ( changeValue.argType == 0 )
        {
            value = changeValue.arg0;
        }
        else if ( changeValue.argType == 1 )
        {
            Vector2 playerPos = new Vector2(Global.PlayerPos.x, Global.PlayerPos.y);
            Vector2 bulletPos = _bullet.GetPosition();
            float angle = MathUtil.GetAngleBetweenXAxis(playerPos - bulletPos);
            value = angle;
        }
        else if ( changeValue.argType == 2 )
        {
            Vector2 targetPos = new Vector2(changeValue.arg0, changeValue.arg1);
            Vector2 bulletPos = _bullet.GetPosition();
            float angle = MathUtil.GetAngleBetweenXAxis(targetPos - bulletPos);
            value = angle;
        }
        if ( changeValue.offset != 0 )
        {
            value += MTRandom.GetNextFloat(-changeValue.offset, changeValue.offset);
        }
        return value;
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
    /// <summary>
    /// 改变的值的结构
    /// </summary>
    public ParaChangeValue changeValue;
    public float begin;
    public float end;
    public float changeTime;
    public float changeDuration;
    public bool isCached;
    public bool isFinish;
    public MathUtil.InterpolationFloatFunc GetInterpolationValueFunc;
    /// <summary>
    /// 重复执行次数
    /// </summary>
    public int repeatCount;
    /// <summary>
    /// 重复执行的间隔
    /// </summary>
    public int repeatInterval;
    /// <summary>
    /// 重复执行的时间间隔计数
    /// </summary>
    public int repeatIntervalTime;

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
    /// /// <summary>
    /// 速度x轴方向分量
    /// </summary>
    Vx = 2,
    /// <summary>
    /// 速度y轴方向分量
    /// </summary>
    Vy = 3,
    /// 当前速度方向
    /// </summary>
    VAngel = 4,
    /// <summary>
    /// 当前加速度
    /// </summary>
    Acce = 5,
    /// <summary>
    /// 当前加速度方向
    /// </summary>
    AccAngle = 6,
    /// <summary>
    /// 最大运动速度
    /// </summary>
    MaxVelocity = 7,
    /// <summary>
    /// 极坐标半径
    /// </summary>
    CurveRadius = 11,
    /// <summary>
    /// 圆周运动角度
    /// </summary>
    CurveAngle = 12,
    /// <summary>
    /// 圆周运动半径增量
    /// </summary>
    CurveDeltaR = 13,
    /// <summary>
    /// 圆周运动角速度增量
    /// </summary>
    CurveOmega = 14,
    CurveCenterX = 15,
    CurveCenterY = 16,
    Alpha = 21,
    ScaleX = 25,
    ScaleY = 26,
    /// <summary>
    /// 激光的长度
    /// </summary>
    LaserLength = 31,
    /// <summary>
    /// 激光的宽度
    /// </summary>
    LaserWidth = 32,
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

public struct ParaChangeValue
{
    /// <summary>
    /// 参数类型
    /// <para>0  直接取arg0的值</para>
    /// <para>1 自机</para>
    /// <para>2 指定坐标</para>
    /// </summary>
    public int argType;
    public float arg0;
    public float arg1;
    /// <summary>
    /// 参数随机偏移
    /// </summary>
    public float offset;
}
