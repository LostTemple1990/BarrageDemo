using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameInfoView : ViewBase,ICommand
{
    private GameObject _bgmObject;
    private Text _bgmText;
    // 符卡时间
    private bool _isShowSpellCardTime;
    private GameObject _scTimeObject;
    private Text _scTimeText;
    private string _timeFormat;
    // 播放声音的时间
    private float _soundTime;
    // 符卡名称
    private GameObject _scNameObject;
    private Text _scNameText;
    // boss名称
    private Text _bossNameText;

    private bool _isShowSpellCardInfo;

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        // bgm
        _bgmText = _viewTf.Find("BGMText").GetComponent<Text>();
        _bgmObject = _bgmText.gameObject;
        // spellCardName
        _scNameText = _viewTf.Find("SpellCardNameText").GetComponent<Text>();
        _scNameObject = _scNameText.gameObject;
        // spellcardTime
        _scTimeText = _viewTf.Find("SpellCardTimeText").GetComponent<Text>();
        _scTimeObject = _scTimeText.gameObject;
        _timeFormat = "00.00";
        Reset();
        //_bgmText = _viewTf.Find("BGMText").GetComponent<Text>();
    }

    public override void OnShow(object[] data)
    {
        CommandManager.GetInstance().Register(CommandConsts.NewSpellCardTime, this);
        CommandManager.GetInstance().Register(CommandConsts.UpdateSpellCardTime, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Register(CommandConsts.ShowSpellCardInfo, this);
        CommandManager.GetInstance().Register(CommandConsts.SpellCardFinish, this);
    }

    public override void OnHide()
    {
        CommandManager.GetInstance().Remove(CommandConsts.NewSpellCardTime, this);
        CommandManager.GetInstance().Remove(CommandConsts.UpdateSpellCardTime, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Remove(CommandConsts.ShowSpellCardInfo, this);
        CommandManager.GetInstance().Remove(CommandConsts.SpellCardFinish, this);
        _isShowSpellCardTime = false;
        _scTimeObject.SetActive(false);
        _bgmObject.SetActive(false);
    }

    public void Execute(int cmd, object[] data)
    {
        switch ( cmd )
        {
            case CommandConsts.NewSpellCardTime:
                NewSpellCardTime((int)data[0]);
                break;
            case CommandConsts.UpdateSpellCardTime:
                UpdateSpellCardTime((int)data[0]);
                break;
            case CommandConsts.RetryGame:
            case CommandConsts.RetryStage:
                Reset();
                break;
            case CommandConsts.ShowSpellCardInfo:
                ShowSpellCardInfo((string)data[0]);
                break;
            case CommandConsts.SpellCardFinish:
                OnSpellCardFinish();
                break;
        }
    }

    private void NewSpellCardTime(int frame)
    {
        if (!_isShowSpellCardTime)
        {
            _scTimeObject.SetActive(true);
            _isShowSpellCardTime = true;
        }
        _soundTime = 10;
        float time = GetSpellCardTimeByFrameLeft(frame);
        _scTimeText.text = time.ToString(_timeFormat);
    }

    private void UpdateSpellCardTime(int frame)
    {
        float time = GetSpellCardTimeByFrameLeft(frame);
        if ( time < _soundTime )
        {
            _soundTime--;
            SoundManager.GetInstance().Play("se_timeout", false);
        }
        _scTimeText.text = time.ToString(_timeFormat);
    }

    /// <summary>
    /// 根据符卡剩余的帧数计算出剩余时间，单位秒
    /// </summary>
    /// <param name="frame"></param>
    /// <returns></returns>
    private float GetSpellCardTimeByFrameLeft(int frame)
    {
        float time = Mathf.Floor(frame / 60f * 100) * 0.01f;
        if (time >= 100)
        {
            time = 99.99f;
        }
        return time;
    }

    /// <summary>
    /// 显示符卡名称
    /// </summary>
    private void ShowSpellCardInfo(string scName)
    {
        _scNameText.text = scName;
        _scNameObject.SetActive(true);
        TweenPos2D tweenPos = TweenManager.GetInstance().Create<TweenPos2D>();
        tweenPos.SetParas(_scNameObject, 60, 60, ePlayMode.Once);
        tweenPos.SetParas(new Vector2(108,-140), new Vector2(108,209), InterpolationMode.Linear);
        TweenManager.GetInstance().AddTween(tweenPos);
        _isShowSpellCardInfo = true;
    }

    private void OnSpellCardFinish()
    {
        // 隐藏符卡时间
        _isShowSpellCardTime = false;
        _scTimeObject.SetActive(false);
        if ( _isShowSpellCardInfo )
        {
            // 隐藏符卡名称
            _scNameObject.SetActive(false);
            TweenManager.GetInstance().RemoveTweenByGo(_scNameObject);
        }
    }

    private void Reset()
    {
        // SpellCardTime
        _isShowSpellCardTime = false;
        _scTimeObject.SetActive(false);
        // 符卡名称
        TweenManager.GetInstance().RemoveTweenByGo(_scNameObject);
        _scNameObject.SetActive(false);
        _isShowSpellCardInfo = false;
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameInfo;
    }
}
