using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TweenAlpha : TweenBase
{
    private delegate void ChangeAlhpaFunc(float value);

    private float _startAlpha;
    private float _endAlpha;
    private Image _img;
    private RawImage _rawImg;
    private SpriteRenderer _spRenderer;
    private Text _text;
    ChangeAlhpaFunc _changeAlhpaFunc;
    MathUtil.InterpolationFloatFunc _interpolationFunc;

    public override void SetParas(GameObject go, eUIType uiType, int startTime, int endTime, TweenFinishCallBack callBack)
    {
        base.SetParas(go, uiType, startTime, endTime, callBack);
        switch ( uiType )
        {
            case eUIType.Image:
                _changeAlhpaFunc = ChangeImgAlhpa;
                _img = _tweenGo.GetComponent<Image>();
                break;
            case eUIType.RawImage:
                _changeAlhpaFunc = ChangeRawImgAlhpa;
                _rawImg = _tweenGo.GetComponent<RawImage>();
                break;
            case eUIType.SpriteRenderer:
                _changeAlhpaFunc = ChangeSpriteRendererAlhpa;
                _spRenderer = _tweenGo.GetComponent<SpriteRenderer>();
                break;
            case eUIType.Text:
                _changeAlhpaFunc = ChangeTextAlhpa;
                _text = _tweenGo.GetComponent<Text>();
                break;
        }
    }

    public void SetParas(float startAlpha,float endAlpha,InterpolationMode mode)
    {
        _startAlpha = startAlpha;
        _endAlpha = endAlpha;
        _interpolationFunc = MathUtil.GetInterpolationFloatFunc(mode);
    }

    private void ChangeImgAlhpa(float value)
    {
        Color color = _img.color;
        color.a = value;
        _img.color = color;
    }

    private void ChangeRawImgAlhpa(float value)
    {
        Color color = _rawImg.color;
        color.a = value;
        _img.color = color;
    }

    private void ChangeSpriteRendererAlhpa(float value)
    {
        Color color = _spRenderer.color;
        color.a = value;
        _img.color = color;
    }

    private void ChangeTextAlhpa(float value)
    {
        Color color = _text.color;
        color.a = value;
        _img.color = color;
    }

    protected override void OnUpdate()
    {
        float value = _interpolationFunc(_startAlpha, _endAlpha, _curTime-_startTime, _duration);
        _changeAlhpaFunc(value);
    }

    public override void Clear()
    {
        _rawImg = null;
        _img = null;
        _spRenderer = null;
        _text = null;
        _changeAlhpaFunc = null;
        _interpolationFunc = null;
        base.Clear();
    }
}
