using UnityEngine;
using System.Collections.Generic;

public class ShakeEffect : STGEffectBase
{
    /// <summary>
    /// 当前的ShakeEffect列表
    /// </summary>
    private List<ShakeEffect> ShakeList = new List<ShakeEffect>();
    /// <summary>
    /// 当前激活的ShakeEffect索引
    /// </summary>
    private int CurrentActiveIndex = -1;

    private Camera _camera;
    private Transform _shakeLayerTf;
    private Rect _originalRect;

    private int _shakeDelay;
    /// <summary>
    /// 抖动持续的时间
    /// </summary>
    private int _shakeDuration;
    /// <summary>
    /// 抖动间隔
    /// </summary>
    private int _shakeInterval;
    /// <summary>
    /// 抖动效果经过的时间
    /// </summary>
    private int _shakeTime;

    /// <summary>
    /// 震幅
    /// </summary>
    private float _shakeLevel;
    /// <summary>
    /// 最小震幅
    /// </summary>
    private float _minShakeLevel;
    /// <summary>
    /// 最大抖动范围
    /// </summary>
    private float _maxShakeRange;

    private bool _isShaking;
    /// <summary>
    /// 当前的偏移
    /// <para>偏移量实际上等于gamelayer的localPos</para>
    /// </summary>
    private Vector3 _curGameLayerPos;

    public ShakeEffect()
    {
        _effectType = EffectType.ShakeEffect;
    }

    public override void Init()
    {
        base.Init();
        _isFinish = false;
        _camera = UIManager.GetInstance().GetSTGCamera();
        _shakeLayerTf = UIManager.GetInstance().GetSTGLayerTf();
        _originalRect = _camera.rect;
        _isShaking = false;
        // 添加到全局shake列表
        ShakeList.Add(this);
        if (CurrentActiveIndex == -1)
            CurrentActiveIndex = ShakeList.IndexOf(this);
    }

    /// <summary>
    /// 触发屏幕抖动特效
    /// </summary>
    /// <param name="delay">抖动延迟</param>
    /// <param name="shakeTime">抖动次数</param>
    /// <param name="shakeInterval">抖动间隔</param>
    /// <param name="shakeDelta"></param>
    /// <param name="shakeLevel"></param>
    public void DoShake(int delay,int shakeDuration,int shakeInterval,float shakeLevel)
    {
        _shakeDelay = delay;
        _shakeDuration = shakeDuration;
        _shakeInterval = shakeInterval;
        _shakeTime = 0;
        _shakeLevel = shakeLevel * Global.STGAcutalScaleHeight;
        _minShakeLevel = 0;
        _maxShakeRange = 0;
        // 初始化偏移量
        _curGameLayerPos = _shakeLayerTf.localPosition;
        _isShaking = true;
    }

    /// <summary>
    /// 触发抖动屏幕特效
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="shakeDuration"></param>
    /// <param name="shakeInterval">抖动间隔</param>
    /// <param name="shakeDelta"></param>
    /// <param name="shakeLevel">震幅，即在[-shakeLevel,shakeLevel]之间</param>
    /// <param name="minShakeLevel">每次最小的抖动范围</param>
    /// <param name="maxShakeRange">抖动的最大范围限制</param>
    public void DoShake(int delay, int shakeDuration, int shakeInterval, float shakeLevel, float minShakeLevel, float maxShakeRange)
    {
        _shakeDelay = delay;
        _shakeDuration = shakeDuration;
        _shakeInterval = shakeInterval;
        _shakeTime = 0;
        _shakeLevel = shakeLevel * Global.STGAcutalScaleHeight;
        if ( minShakeLevel < shakeLevel )
        {
            _minShakeLevel = minShakeLevel * Global.STGAcutalScaleHeight;
        }
        else
        {
            _minShakeLevel = 0;
        }
        if (_maxShakeRange <= 0)
            _maxShakeRange = int.MaxValue;
        _maxShakeRange = maxShakeRange * Global.STGAcutalScaleHeight;
        // 初始化偏移量
        _curGameLayerPos = _shakeLayerTf.localPosition;
        _isShaking = true;
    }

    public override void Update()
    {
        if (CurrentActiveIndex == -1)
        {
            CurrentActiveIndex = ShakeList.IndexOf(this);
        }
        if ( _isShaking )
        {
            Shake();
        }
    }

    private void Shake()
    {
        if (_shakeDelay > 0)
        {
            _shakeDelay--;
        }
        else
        {
            _shakeTime++;
            // 只有当前ShakeEffect处于激活状态时才执行抖动
            if (ShakeList[CurrentActiveIndex] == this)
            {
                if (_shakeTime % _shakeInterval == 0)
                {
                    Vector3 newPos = Vector3.zero;
                    newPos.x = GetNextOffset(_curGameLayerPos.x, _minShakeLevel, _shakeLevel, _maxShakeRange);
                    newPos.y = GetNextOffset(_curGameLayerPos.y, _minShakeLevel, _shakeLevel, _maxShakeRange);
                    _curGameLayerPos = newPos;
                    _shakeLayerTf.localPosition = _curGameLayerPos;
                }
            }
            if (_shakeTime >= _shakeDuration)
            {
                _isShaking = false;
                _isFinish = true;
            }
        }
    }

    /// <summary>
    /// 随机获得下一个偏移量
    /// </summary>
    /// <param name="cur">当前偏移</param>
    /// <param name="minOffset">最小的偏移</param>
    /// <param name="maxOffset">最大的偏移</param>
    /// <param name="range">[-range,range]为偏移的范围</param>
    /// <returns></returns>
    private float GetNextOffset(float cur,float minOffset,float maxOffset,float range)
    {
        //没有设置range，则默认为无限制
        if ( range <= 0 )
        {
            range = 9999;
        }
        float offset = cur;
        float min, max;
        // 负、正
        int neg = 0, pos = 1;
        // 确定向左随机还是
        if ( cur + minOffset > range )
        {
            pos = 0;
        }
        if ( cur - minOffset < -range )
        {
            neg = 1;
        }
        // 往左或者往右偏移的距离都没有达到最小偏移距离
        // 故直接返回当前数值
        if ( neg > pos )
        {
            return offset;
        }
        float factor = 2 * Random.Range(neg, pos + 1) - 1;
        // 往左偏移
        if ( factor == -1 )
        {
            min = cur - maxOffset;
            if ( min < -range )
            {
                min = -range;
            }
            max = cur - minOffset;
            offset = Random.Range(min, max);
        }
        else if ( factor == 1 )
        {
            min = cur + minOffset;
            max = cur + maxOffset;
            if ( max > range )
            {
                max = range;
            }
            offset = Random.Range(min, max);
        }
        return offset;
    }

    public override void SetPosition(float posX, float posY)
    {
        throw new System.NotImplementedException();
    }

    public override void Clear()
    {
        _shakeLayerTf.localPosition = Vector3.zero;
        _camera = null;
        _shakeLayerTf = null;
        // 从ShakeList中移除
        int index = ShakeList.IndexOf(this);
        if (CurrentActiveIndex == index)
        {
            CurrentActiveIndex = -1;
        }
        ShakeList[index] = null;
        CommonUtils.RemoveNullElementsInList<ShakeEffect>(ShakeList);
    }
}
