using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameInfoView : ViewBase,ICommand
{
    private const int BonusTimeTextDuration = 60;
    /// <summary>
    /// Bonus Fail/Get Bouns文字的动画的持续时间
    /// </summary>
    private const int BonusResultAniDuration = 30;
    private const int ResultDuration = 180;
    /// <summary>
    /// 代表剩余符卡数量的星星图片的间隔
    /// </summary>
    private const float BossInfoSCCountIntervalX = 11;

    struct SpriteNumData
    {
        public GameObject go;
        public RectTransform tf;
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

    private bool _isShowSpellCardInfo;

    private GameObject _spellCardResultPanel;
    private GameObject _getBonusGo;
    private GameObject _bonusFailGo;

    private GameObject _finishTimePanel;
    private GameObject _finishTimeContainer;
    private SpriteNumData[] _finishTimeDatas;
    private SpriteRenderer _finishTimeText;


    private GameObject _realTimePanel;
    private GameObject _realTimeContainer;
    private SpriteNumData[] _realTimeDatas;
    private SpriteRenderer _realTimeText;

    /// <summary>
    /// 当前是否正在显示符卡结算界面
    /// </summary>
    private bool _isShowingSCResult;
    /// <summary>
    /// 是否击破符卡
    /// </summary>
    private bool _isFinishSC;
    private int _bonusTime;
    private int _resultTime;
    private int _resultState;

    /// <summary>
    /// 获取残机的提示UI
    /// </summary>
    private bool _isShowImgExtend;
    private GameObject _imgExtendGo;
    private Image _imgExtend;

    /// <summary>
    /// 当前是否显示BOSS信息
    /// </summary>
    private bool _isShowBossInfo;
    private GameObject _bossInfoGo;
    private Text _bossInfoNameText;
    private RectTransform _bossInfoContainerTf;
    private GameObject _bossInfoStarProto;
    private List<Image> _bossInfoStarList;
    /// <summary>
    /// 当前可用的star总数
    /// </summary>
    private int _bossInfoStarTotalCount;
    /// <summary>
    /// 当前剩余的符卡数量
    /// </summary>
    private int _bossInfoSCLeft;

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
        _finishTimeText = finishTimePanelTf.Find("TextFinishTime").GetComponent<SpriteRenderer>();
        Transform finishTimeContainerTf = finishTimePanelTf.Find("FinishTimeContainer");
        _finishTimeContainer = finishTimeContainerTf.gameObject;
        _finishTimeDatas = new SpriteNumData[10];
        int i;
        for (i=0;i<10;i++)
        {
            SpriteNumData data = new SpriteNumData();
            data.tf = finishTimeContainerTf.Find("Num" + i).GetComponent<RectTransform>();
            data.go = data.tf.gameObject;
            data.sr = data.tf.GetComponent<SpriteRenderer>();
            _finishTimeDatas[i] = data;
        }
        //通过符卡实际时间ui
        Transform realTimePanelTf = spellCardResultPanelTf.Find("RealTimePanel");
        _realTimePanel = realTimePanelTf.gameObject;
        _realTimeText = realTimePanelTf.Find("TextRealTime").GetComponent<SpriteRenderer>();
        Transform realTimeContainerTf = realTimePanelTf.Find("RealTimeContainer");
        _realTimeContainer = realTimeContainerTf.gameObject;
        _realTimeDatas = new SpriteNumData[10];
        for (i = 0; i < 10; i++)
        {
            SpriteNumData data = new SpriteNumData();
            data.tf = realTimeContainerTf.Find("Num" + i).GetComponent<RectTransform>();
            data.go = data.tf.gameObject;
            data.sr = data.tf.GetComponent<SpriteRenderer>();
            _realTimeDatas[i] = data;
        }
        // 获取残机UI
        _imgExtendGo = _viewTf.Find("ImgExtend").gameObject;
        _imgExtend = _imgExtendGo.GetComponent<Image>();
        _isShowImgExtend = false;
        Reset();
        // Boss信息UI
        Transform bossInfoTf = _viewTf.Find("BossInfoPanel");
        _bossInfoGo = bossInfoTf.gameObject;
        _bossInfoNameText = bossInfoTf.Find("NameText").GetComponent<Text>();
        _bossInfoContainerTf = bossInfoTf.Find("Container").GetComponent<RectTransform>();
        _bossInfoStarProto = _bossInfoContainerTf.Find("Star").gameObject;
        _bossInfoStarList = new List<Image>();
        _bossInfoStarList.Add(_bossInfoStarProto.GetComponent<Image>());
        _bossInfoStarTotalCount = 1;
        //_bgmText = _viewTf.Find("BGMText").GetComponent<Text>();
    }

    protected override void OnShow(object data)
    {
        CommandManager.GetInstance().Register(CommandConsts.NewSpellCardTime, this);
        CommandManager.GetInstance().Register(CommandConsts.UpdateSpellCardTime, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Register(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Register(CommandConsts.ShowSpellCardInfo, this);
        CommandManager.GetInstance().Register(CommandConsts.SpellCardFinish, this);
        CommandManager.GetInstance().Register(CommandConsts.BackToTitle, this);
        CommandManager.GetInstance().Register(CommandConsts.PlayerExtend, this);
        CommandManager.GetInstance().Register(CommandConsts.ShowBossInfo, this);

        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    protected override void OnHide()
    {
        CommandManager.GetInstance().Remove(CommandConsts.NewSpellCardTime, this);
        CommandManager.GetInstance().Remove(CommandConsts.UpdateSpellCardTime, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryGame, this);
        CommandManager.GetInstance().Remove(CommandConsts.RetryStage, this);
        CommandManager.GetInstance().Remove(CommandConsts.ShowSpellCardInfo, this);
        CommandManager.GetInstance().Remove(CommandConsts.SpellCardFinish, this);
        CommandManager.GetInstance().Remove(CommandConsts.BackToTitle, this);
        CommandManager.GetInstance().Remove(CommandConsts.PlayerExtend, this);
        CommandManager.GetInstance().Remove(CommandConsts.ShowBossInfo, this);
        Reset();
    }

    public void Execute(int cmd, object data)
    {
        object[] datas = data as object[];
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
            case CommandConsts.BackToTitle:
                Reset();
                break;
            case CommandConsts.PlayerExtend:
                OnPlayerExtend();
                break;
            case CommandConsts.ShowBossInfo:
                OnShowBossInfo((string)datas[0], (int)datas[1]);
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
            _isFinishSC = (bool)datas[1];
            if (_isFinishSC)
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
                    SoundManager.GetInstance().Play("se_scbonus", Consts.DefaultUISEVolume, false, false);
                }
                else
                {
                    _getBonusGo.SetActive(false);
                    _bonusFailGo.SetActive(true);
                }
            }
            else
            {
                _finishTimePanel.SetActive(false);
                _getBonusGo.SetActive(false);
                _bonusFailGo.SetActive(true);
                SoundManager.GetInstance().Play("se_scfault", Consts.DefaultUISEVolume, false, false);
            }
            // 显示实际时间
            int realTimePassed = (int)datas[4];
            UpdateShowTime(realTimePassed, _realTimeDatas);
            // 设置符卡结果面板动画参数
            _resultState = 0;
            _resultTime = 0;
            for (int i=0;i<10;i++)
            {
                _realTimeDatas[i].sr.color = new Color(1, 1, 1, 0);
            }
            _realTimeText.color = new Color(1, 1, 1, 0);
            if (_isFinishSC)
            {
                for (int i = 0; i < 10; i++)
                {
                    _finishTimeDatas[i].sr.color = new Color(1, 1, 1, 0);
                }
                _finishTimeText.color = new Color(1, 1, 1, 0);
            }
            _getBonusGo.transform.localScale = new Vector3(100, 100, 1);
            _bonusFailGo.transform.localScale = new Vector3(100, 100, 1);
            _isShowingSCResult = true;
            // 更新BossInfo
            if (_isShowBossInfo)
            {
                if (_bossInfoSCLeft > 0)
                {
                    _bossInfoSCLeft--;
                    GameObject starGo = _bossInfoStarList[_bossInfoSCLeft].gameObject;
                    TweenAlpha tween0 = TweenManager.GetInstance().Create<TweenAlpha>();
                    tween0.SetParas(starGo, 0, 30, ePlayMode.Once);
                    tween0.SetParas(0, InterpolationMode.Linear);
                    TweenManager.GetInstance().AddTween(tween0);
                    TweenScale tween1 = TweenManager.GetInstance().Create<TweenScale>();
                    tween1.SetParas(starGo, 0, 30, ePlayMode.Once);
                    tween1.SetParas(new Vector3(2, 2, 1), InterpolationMode.EaseOutQuad);
                    TweenManager.GetInstance().AddTween(tween1);
                }
                else
                {
                    _isShowBossInfo = false;
                    _bossInfoGo.SetActive(false);
                }
            }
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
            data.tf.anchoredPosition = new Vector2(posX, 0);
            data.tf.localScale = new Vector3(50, 50, 1);
            data.sr.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "ResultChar" + num);

            posX += 16;
            tmp -= num * baseNum;
            baseNum /= 10;
        }
        // 添加小数点
        data = goDatas[i++];
        data.go.SetActive(true);
        data.tf.anchoredPosition = new Vector2(posX, 0);
        //data.tf.localPosition = new Vector3(posX, 0, 0);
        data.tf.localScale = new Vector3(25, 25, 1);
        data.sr.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "ResultCharDot");
        posX += 8;
        // 小数部分
        for (baseNum = 10, tmp = num1; baseNum > 0; i++)
        {
            num = tmp / baseNum;
            data = goDatas[i];
            data.go.SetActive(true);
            data.tf.anchoredPosition = new Vector2(posX, 0);
            data.tf.localScale = new Vector3(25, 25, 1);
            data.sr.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "ResultChar" + num);

            posX += 8;
            tmp -= num * baseNum;
            baseNum /= 10;
        }
        for (; i < 10; i++)
        {
            data = goDatas[i];
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
        // 隐藏符卡结算ui
        _isShowSpellCardInfo = false;
        _isShowingSCResult = false;
        _spellCardResultPanel.SetActive(false);
        if (_isShowImgExtend)
        {
            _imgExtendGo.SetActive(false);
            TweenManager.GetInstance().RemoveTweenByGo(_imgExtendGo);
        }
        _isShowImgExtend = false;
        _bossInfoSCLeft = 0;
        if (_isShowBossInfo)
        {
            _bossInfoGo.SetActive(false);
            _isShowBossInfo = false;
        }
    }

    public override void Update()
    {
        if (Global.IsPause)
            return;
        if (_isShowingSCResult)
        {
            UpdateShowingSCResult();
        }
    }

    private void UpdateShowingSCResult()
    {
        if (_resultState == 0)
        {
            _resultTime++;
            float alpha = (float)_resultTime / BonusTimeTextDuration;
            for (int i=0;i<10;i++)
            {
                _realTimeDatas[i].sr.color = new Color(1, 1, 1, alpha);
            }
            if (_isFinishSC)
            {
                for (int i = 0; i < 10; i++)
                    _finishTimeDatas[i].sr.color = new Color(1, 1, 1, alpha);
            }
            _realTimeText.color = new Color(1, 1, 1, alpha);
            _finishTimeText.color = new Color(1, 1, 1, alpha);
            if (_resultTime >= BonusTimeTextDuration)
            {
                _resultState = 1;
                _resultTime = 0;
            }
        }
        else if (_resultState == 1)
        {
            _resultTime++;
            if (_resultTime >= 60)
            {
                _resultState = 2;
                _resultTime = 0;
            }
        }
        else if (_resultState == 2)
        {
            _resultTime++;
            // y轴缩放
            if (_resultTime <= BonusResultAniDuration)
            {
                float scaleY = 1 - (float)_resultTime / BonusResultAniDuration;
                _getBonusGo.transform.localScale = new Vector3(100, scaleY * 100, 1);
                _bonusFailGo.transform.localScale = new Vector3(100, scaleY * 100, 1);
            }
            float alpha = 1 - (float)_resultTime / BonusTimeTextDuration;
            for (int i = 0; i < 10; i++)
            {
                _realTimeDatas[i].sr.color = new Color(1, 1, 1, alpha);
            }
            if (_isFinishSC)
            {
                for (int i = 0; i < 10; i++)
                    _finishTimeDatas[i].sr.color = new Color(1, 1, 1, alpha);
            }
            _realTimeText.color = new Color(1, 1, 1, alpha);
            _finishTimeText.color = new Color(1, 1, 1, alpha);
            if (_resultTime >= 60)
            {
                _spellCardResultPanel.SetActive(false);
                _isShowingSCResult = false;
            }
        }
    }

    private void OnPlayerExtend()
    {
        if (_isShowImgExtend)
        {
            TweenManager.GetInstance().RemoveTweenByGo(_imgExtendGo);
        }
        else
        {
            _isShowImgExtend = true;
            _imgExtendGo.SetActive(true);
            _imgExtend.color = new Color(1, 1, 1, 0);
        }
        // 出现
        TweenAlpha tween = TweenManager.GetInstance().Create<TweenAlpha>();
        tween.SetParas(_imgExtendGo, 0, 10, ePlayMode.Once);
        tween.SetParas(1, InterpolationMode.Linear);
        TweenManager.GetInstance().AddTween(tween);
        // 渐隐
        tween = TweenManager.GetInstance().Create<TweenAlpha>();
        tween.SetParas(_imgExtendGo, 120, 20, ePlayMode.Once);
        tween.SetParas(0, InterpolationMode.Linear);
        tween.SetFinishCallBack(OnExtendFinish);
        TweenManager.GetInstance().AddTween(tween);
        SoundManager.GetInstance().Play("se_extend", Consts.DefaultUISEVolume, false, false);
    }

    private void OnExtendFinish(GameObject go)
    {
        if (_isShowImgExtend)
        {
            _isShowImgExtend = false;
            _imgExtendGo.SetActive(false);
        }
    }

    /// <summary>
    /// 设置UI左上方BOSS信息
    /// <para>scLeft 为0时表示最后一张符卡，不显示代表剩余数量的星星，依次类推</para>
    /// </summary>
    /// <param name="bossName"></param>
    /// <param name="scLeft"></param>
    private void OnShowBossInfo(string bossName,int scLeft)
    {
        GameObject go;
        Image img;
        TweenAlpha tween;
        if (!_isShowBossInfo)
        {
            // 整个面板设置为可见
            _bossInfoGo.SetActive(true);
            // bossName
            Color textCol = _bossInfoNameText.color;
            textCol.a = 0;
            _bossInfoNameText.color = textCol;
            tween = TweenManager.GetInstance().Create<TweenAlpha>();
            tween.SetParas(_bossInfoNameText.gameObject, 0, 30, ePlayMode.Once);
            tween.SetParas(1, InterpolationMode.Linear);
            TweenManager.GetInstance().AddTween(tween);
        }
        _bossInfoNameText.text = bossName;
        if (scLeft < 0) scLeft = 0;
        // scCount
        // 如果剩余符卡数比当前创建的星星数量多，则先创建
        if (scLeft > _bossInfoStarTotalCount)
        {
            while (_bossInfoStarTotalCount < scLeft)
            {
                go = GameObject.Instantiate(_bossInfoStarProto, _bossInfoContainerTf, false);
                go.GetComponent<RectTransform>().anchoredPosition = new Vector2(_bossInfoStarTotalCount * BossInfoSCCountIntervalX, 0);
                _bossInfoStarList.Add(go.GetComponent<Image>());
                _bossInfoStarTotalCount++;
            }
        }
        for (int i = 0; i < scLeft; i++)
        {
            img = _bossInfoStarList[i];
            go = img.gameObject;
            go.SetActive(true);
            go.transform.localScale = Vector3.one;
            img.color = new Color(1, 1, 1, 0);
            tween = TweenManager.GetInstance().Create<TweenAlpha>();
            tween.SetParas(go.gameObject, 0, 30, ePlayMode.Once);
            tween.SetParas(1, InterpolationMode.Linear);
            TweenManager.GetInstance().AddTween(tween);
        }
        for (int i = scLeft; i < _bossInfoStarTotalCount; i++)
        {
            _bossInfoStarList[i].gameObject.SetActive(false);
        }
        _bossInfoSCLeft = scLeft;
        _isShowBossInfo = true;
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameInfo;
    }
}
