using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameInfoView : ViewBase,ICommand
{
    struct SpriteNumData
    {
        public GameObject go;
        public Transform tf;
        public SpriteRenderer sr;
    };

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

    private GameObject _spellCardResultPanel;
    private GameObject _getBonusGo;
    private GameObject _bonusFailGo;

    private GameObject _finishTimePanel;
    private GameObject _finishTimeContainer;
    private SpriteNumData[] _finishTimeDatas;


    private GameObject _realTimePanel;
    private GameObject _realTimeContainer;
    private SpriteNumData[] _realTimeDatas;

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
        // 符卡结算相关ui
        Transform spellCardResultPanelTf = _viewTf.Find("SpellCardResultPanel");
        _spellCardResultPanel = spellCardResultPanelTf.gameObject;
        // GetBonusGo
        _getBonusGo = spellCardResultPanelTf.Find("GetBonusSprite").gameObject;
        // BonusFailGo
        _bonusFailGo = spellCardResultPanelTf.Find("BonusFailSprite").gameObject;
        // 符卡击破时间相关ui
        Transform finishTimePanelTf = spellCardResultPanelTf.Find("FinishTimePanel");
        _finishTimePanel = finishTimePanelTf.gameObject;
        Transform finishTimeContainerTf = finishTimePanelTf.Find("FinishTimeContainer");
        _finishTimeContainer = finishTimeContainerTf.gameObject;
        _finishTimeDatas = new SpriteNumData[10];
        int i;
        for (i=0;i<10;i++)
        {
            SpriteNumData data = new SpriteNumData();
            data.tf = finishTimeContainerTf.Find("Num" + i);
            data.go = data.tf.gameObject;
            data.sr = data.tf.GetComponent<SpriteRenderer>();
            _finishTimeDatas[i] = data;
        }
        //通过符卡实际时间ui
        Transform realTimePanelTf = spellCardResultPanelTf.Find("RealTimePanel");
        _realTimePanel = realTimePanelTf.gameObject;
        Transform realTimeContainerTf = realTimePanelTf.Find("RealTimeContainer");
        _realTimeContainer = realTimeContainerTf.gameObject;
        _realTimeDatas = new SpriteNumData[10];
        for (i = 0; i < 10; i++)
        {
            SpriteNumData data = new SpriteNumData();
            data.tf = realTimeContainerTf.Find("Num" + i);
            data.go = data.tf.gameObject;
            data.sr = data.tf.GetComponent<SpriteRenderer>();
            _realTimeDatas[i] = data;
        }

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
        _spellCardResultPanel.SetActive(false);
    }

    public void Execute(int cmd, object[] datas)
    {
        switch ( cmd )
        {
            case CommandConsts.NewSpellCardTime:
                NewSpellCardTime((int)datas[0]);
                break;
            case CommandConsts.UpdateSpellCardTime:
                UpdateSpellCardTime((int)datas[0]);
                break;
            case CommandConsts.RetryGame:
            case CommandConsts.RetryStage:
                Reset();
                break;
            case CommandConsts.ShowSpellCardInfo:
                ShowSpellCardInfo((string)datas[0]);
                break;
            case CommandConsts.SpellCardFinish:
                OnSpellCardFinish(datas);
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
        _scNameObject.transform.localPosition = new Vector2(192, -112);
        TweenPos2D tweenPos = TweenManager.GetInstance().Create<TweenPos2D>();
        tweenPos.SetParas(_scNameObject, 60, 60, ePlayMode.Once);
        tweenPos.SetParas(new Vector2(192,-112), new Vector2(192,224), InterpolationMode.Linear);
        tweenPos.SetIgnoreTimeScale(false);
        TweenManager.GetInstance().AddTween(tweenPos);
        _isShowSpellCardInfo = true;
    }

    /// <summary>
    /// 符卡结束回调
    /// <para>bool isSpellCard 是否为符卡(非符or符卡)</para>
    /// <para>bool isFinishSpellCard 是否成功击破符卡</para>
    /// <para>bool getBonus 是否获得符卡bonus<para>
    /// <para>int timePassed 经过的时间</para>
    /// <para>int realTimePassed 经过的时间</para>
    /// </summary>
    /// <param name="datas"></param>
    private void OnSpellCardFinish(object[] datas)
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
        // 如果是符卡，则显示符卡通过信息
        bool isSpellCard = (bool)datas[0];
        if ( isSpellCard == true )
        {
            _spellCardResultPanel.SetActive(true);
            bool isFinishSpellCard = (bool)datas[1];
            if ( isFinishSpellCard )
            {
                // 显示符卡击破时间
                _finishTimePanel.SetActive(true);
                int timePassed = (int)datas[3];
                UpdateShowTime(timePassed, _finishTimeDatas);
                // 判断是否获得bonus
                bool getBonus = (bool)datas[2];
                if ( getBonus )
                {
                    _getBonusGo.SetActive(true);
                    _bonusFailGo.SetActive(false);
                }
                else
                {
                    _getBonusGo.SetActive(false);
                    _bonusFailGo.SetActive(false);
                }
            }
            else
            {
                _finishTimePanel.SetActive(false);
                _getBonusGo.SetActive(false);
                _bonusFailGo.SetActive(true);
            }
            // 显示实际时间
            int realTimePassed = (int)datas[4];
            UpdateShowTime(realTimePassed, _realTimeDatas);
        }
        else
        {
            _spellCardResultPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 更新符卡结束后需要显示的时间的ui
    /// <para>timeInMs 需要显示的时间，单位：毫秒</para>
    /// </summary>
    /// <param name="frame"></param>
    private void UpdateShowTime(int timeInMs,SpriteNumData[] goDatas)
    {
        // 单位 0.01秒 即 10毫秒
        int finishTime = timeInMs / 10;
        // 秒数
        int num0 = finishTime / 100;
        // 小数点后两位
        int num1 = finishTime % 100;
        int i, num, tmp, baseNum, posX;
        for (tmp = num0, baseNum = 0; tmp > 0;)
        {
            tmp /= 10;
            if ( baseNum == 0 )
            {
                baseNum = 1;
            }
            else
            {
                baseNum *= 10;
            }
        }
        if (baseNum == 0) baseNum = 1;
        SpriteNumData data;
        // 整数部分
        for (tmp = num0, i = 0, posX = 0; baseNum > 0; i++)
        {
            num = tmp / baseNum;
            data = goDatas[i];
            data.go.SetActive(true);
            data.tf.localPosition = new Vector3(posX, 0, 0);
            data.tf.localScale = new Vector3(50, 50, 1);
            data.sr.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "ResultChar" + num);

            posX += 16;
            tmp -= num * baseNum;
            baseNum /= 10;
        }
        // 添加小数点
        data = goDatas[i++];
        data.go.SetActive(true);
        data.tf.localPosition = new Vector3(posX, 0, 0);
        data.tf.localScale = new Vector3(25, 25, 1);
        data.sr.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "ResultCharDot");
        posX += 8;
        // 小数部分
        for (baseNum = 10, tmp = num1; baseNum > 0; i++)
        {
            num = tmp / baseNum;
            data = goDatas[i];
            data.go.SetActive(true);
            data.tf.localPosition = new Vector3(posX, 0, 0);
            data.tf.localScale = new Vector3(25, 25, 1);
            data.sr.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "ResultChar" + num);

            posX += 8;
            tmp -= num * baseNum;
            baseNum /= 10;
        }
        for (; i < 10; i++)
        {
            data = _finishTimeDatas[i];
            data.go.SetActive(false);
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
