using UnityEngine;
using System.Collections;

public class ShakeEffect : IEffect
{
    private const float MaxShakeLevel = 10;

    private Camera _camera;
    private bool _isFinish;
    private Rect _originalRect;

    private int _shakeDelay;
    private int _shakeTotalTime;
    private int _shakeInterval;
    private int _shakeTime;

    private float _shakeDelta;
    private float _shakeLevel;

    private bool _isShaking;

    private float _clampMinX;
    private float _clampMaxX;
    private float _clampMinY;
    private float _clampMaxY;

    public void Init()
    {
        _isFinish = false;
        _camera = UIManager.GetInstance().GetSTGCamera();
        _originalRect = _camera.rect;
        _isShaking = false;
        float clampValue = MaxShakeLevel / Screen.height;
        _clampMinX = -clampValue;
        _clampMaxX = clampValue;
        _clampMinY = -clampValue;
        _clampMaxY = clampValue;
    }

    public void DoShake(int delay,int shakeTime,int shakeInterval,float shakeDelta,float shakeLevel)
    {
        _shakeDelay = delay;
        _shakeTotalTime = shakeTime;
        _shakeInterval = shakeInterval;
        _shakeTime = 0;
        _shakeDelta = 1;
        _shakeLevel = shakeLevel / Screen.height;
        _isShaking = true;
    }

    public void Update()
    {
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
            if (_shakeTime % _shakeInterval == 0)
            {
                Vector2 offset = Vector2.zero;
                offset.x += _shakeDelta * (Random.Range(-_shakeLevel, _shakeLevel));
                offset.y += _shakeDelta * (Random.Range(-_shakeLevel,_shakeLevel));
                Rect curRect = _camera.rect;
                curRect.x += offset.x;
                curRect.y += offset.y;
                _camera.rect = curRect;
            }
            if (_shakeTime >= _shakeTotalTime)
            {
                _isShaking = false;
            }
            _shakeTime++;
        }
    }

    public void SetToPos(float posX, float posY)
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        _camera.rect = _originalRect;
        _camera = null;
    }

    public bool IsFinish()
    {
        return _isFinish;
    }

    public void FinishEffect()
    {
        _isFinish = true;
    }
}
