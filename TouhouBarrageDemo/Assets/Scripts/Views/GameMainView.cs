using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class GameMainView : ViewBase,ICommand
{
    private const string BigNumStr = "BigNum";
    /// <summary>
    /// 擦弹最大位数
    /// </summary>
    private const int GrazeBitMaxCount = 6;
    private const int TickArrSize = 150;
    private const int UpdateFpsInterval = 60;

    /// <summary>
    /// power整数位
    /// </summary>
    private Image _powerInt;
    /// <summary>
    /// power小数点后1位
    /// </summary>
    private Image _powerBit1;
    /// <summary>
    /// power小数点后2位
    /// </summary>
    private Image _powerBit2;

    private List<Image> _grazeImgList;
    /// <summary>
    /// 擦弹数目的位数
    /// </summary>
    private int _grazeBitCount;
    /// <summary>
    /// 当前擦弹数
    /// </summary>
    private int _curGraze;
    /// <summary>
    /// 当前Power
    /// </summary>
    private int _curPower;
    /// <summary>
    /// 当前残机
    /// </summary>
    private ItemWithFramentsCounter _curLifeCounter;
    private List<Image> _lifeImgList;
    /// <summary>
    /// 当前符卡
    /// </summary>
    private ItemWithFramentsCounter _curSpellCardCounter;
    private List<Image> _spellCardImgList;
    /// <summary>
    /// 显示fps的文本框
    /// </summary>
    private Text _fpsText;
    /// <summary>
    /// 当前帧的时间
    /// </summary>
    private int _curTimeTickIndex;
    /// <summary>
    /// 上一帧的时间
    /// </summary>
    private int _lastTimeTickIndex;
    /// <summary>
    /// 帧计数
    /// </summary>
    private int _updateFrameTimer;

    private long[] _frameTimeTick;

    /// <summary>
    /// boss位置提示信息的最大个数
    /// </summary>
    private const int MaxBossHintCount = 2;
    /// <summary>
    /// 对象池
    /// </summary>
    private Stack<RectTransform> _bossHintTfPool;
    /// <summary>
    /// 当前显示的boss提示信息
    /// </summary>
    private Dictionary<Boss, RectTransform> _bossHintTfDic;
    /// <summary>
    /// 底部的宽度
    /// </summary>
    private float _bottomSizeX;

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        _powerInt = _viewTf.Find("Info/Power/PowerValue/PowerValueInt").GetComponent<Image>();
        _powerBit1 = _viewTf.Find("Info/Power/PowerValue/PowerValueB1").GetComponent<Image>();
        _powerBit2 = _viewTf.Find("Info/Power/PowerValue/PowerValueB2").GetComponent<Image>();
        if ( _grazeImgList == null )
        {
            _grazeImgList = new List<Image>();
            for (int i=0;i<GrazeBitMaxCount; i++)
            {
                _grazeImgList.Add(_viewTf.Find("Info/Graze/GrazeValue/GrazeBit" + i).GetComponent<Image>());
            }
            _curGraze = -1;
            _grazeBitCount = 0;
        }
        if ( _lifeImgList == null )
        {
            _lifeImgList = new List<Image>();
            for (int i=0;i<Consts.PlayerMaxLifeCount;i++)
            {
                _lifeImgList.Add(_viewTf.Find("Info/Life/Count/Life" + i).GetComponent<Image>());
            }
        }
        if (_spellCardImgList == null)
        {
            _spellCardImgList = new List<Image>();
            for (int i = 0; i < Consts.PlayerMaxLifeCount; i++)
            {
                _spellCardImgList.Add(_viewTf.Find("Info/SpellCard/Count/SpellCard" + i).GetComponent<Image>());
            }
        }
        _curPower = -1;
        _fpsText = _viewTf.Find("FpsText").GetComponent<Text>();
        _frameTimeTick = new long[TickArrSize];
        // Boss位置提示相关
        _bossHintTfPool = new Stack<RectTransform>();
        _bossHintTfDic = new Dictionary<Boss, RectTransform>();
        for (int i=0;i<MaxBossHintCount;i++)
        {
            RectTransform hintTf = _viewTf.Find("Bg/BgBottom/PosHint" + i).GetComponent<RectTransform>();
            _bossHintTfPool.Push(hintTf);
            hintTf.anchoredPosition = new Vector2(2000, 2000);
        }
        _bottomSizeX = UIManager.GetInstance().GetUIRootSize().x + _viewTf.Find("Bg/BgBottom").GetComponent<RectTransform>().sizeDelta.x;
    }

    public override void OnShow(object data)
    {
        base.OnShow(data);
        // 重新初始化
        _curLifeCounter = new ItemWithFramentsCounter();
        _curSpellCardCounter = new ItemWithFramentsCounter();
        AddEvents();
        ResetFpsText();
        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    private void ResetFpsText()
    {
        _lastTimeTickIndex = -1;
        _curTimeTickIndex = -1;
        _updateFrameTimer = 0;
        _fpsText.text = "";
    }

    private void AddEvents()
    {
        CommandManager.GetInstance().Register(CommandConsts.ShowBossPosHint, this);
    }

    public void Execute(int cmd,object data)
    {
        if (cmd == CommandConsts.ShowBossPosHint)
            OnShowBossPosHint(data);
    }

    private void OnShowBossPosHint(object data)
    {
        List<object> datas = data as List<object>;
        Boss boss = datas[0] as Boss;
        bool isShow = (bool)datas[1];
        RectTransform hintTf;
        if (isShow)
        {
            if (!_bossHintTfDic.TryGetValue(boss, out hintTf))
            {
                if (_bossHintTfPool.Count > 0)
                {
                    hintTf = _bossHintTfPool.Pop();
                    _bossHintTfDic.Add(boss, hintTf);
                }
            }
            float posX = (float)datas[2];
            float halfWidth = Consts.GameWidth / 2;
            if (posX < -halfWidth || posX >= halfWidth)
            {
                hintTf.anchoredPosition = new Vector2(2000, 2000);
            }
            else
            {
                float factor = (posX + halfWidth) / Consts.GameWidth;
                hintTf.anchoredPosition = new Vector2(_bottomSizeX * factor, 0);
            }
        }
        else
        {
            if (_bossHintTfDic.ContainsKey(boss))
            {
                _bossHintTfDic.TryGetValue(boss, out hintTf);
                _bossHintTfPool.Push(hintTf);
                _bossHintTfDic.Remove(boss);
                hintTf.anchoredPosition = new Vector2(2000, 2000);
            }
        }
    }

    public override void Update()
    {
        UpdateFps();
        if (Global.IsPause) return;
        UpdatePowerValue();
        UpdateGrazeValue();
        UpdateLifeValue();
        UpdateSpellCardValue();
    }

    /// <summary>
    /// 更新power的显示
    /// </summary>
    private void UpdatePowerValue()
    {
        int power = PlayerInterface.GetInstance().GetPower();
        if ( power != _curPower )
        {
            _curPower = power;
            int num = _curPower / 100;
            _powerInt.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, BigNumStr + num);
            num = (_curPower % 100) / 10;
            _powerBit1.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, BigNumStr + num);
            num = _curPower % 10;
            _powerBit2.sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, BigNumStr + num);
        }
    }

    private void UpdateGrazeValue()
    {
        int graze = PlayerInterface.GetInstance().GetGraze();
        if ( graze != _curGraze )
        {
            int bit = GetBit(graze);
            // 显示、隐藏多余的位数
            if ( bit > _grazeBitCount )
            {
                for (int i=_grazeBitCount;i<bit;i++)
                {
                    _grazeImgList[i].gameObject.SetActive(true);
                }
            }
            else if (bit < _grazeBitCount)
            {
                for (int i = bit; i < _grazeBitCount; i++)
                {
                    _grazeImgList[i].gameObject.SetActive(false);
                }
            }
            // 从个位开始显示擦弹数目
            int num;
            for (int i=bit;i>0;i--)
            {
                num = graze % 10;
                _grazeImgList[i-1].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, BigNumStr + num);
                graze /= 10;
            }
        }
    }

    /// <summary>
    /// 获取一个整形的位数
    /// <para>0的时候返回1</para>
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private int GetBit(int num)
    {
        int bit = 1;
        while ( (num /= 10) != 0 )
        {
            bit++;
        }
        return bit;
    }

    private void UpdateLifeValue()
    {
        ItemWithFramentsCounter lifeCounter = PlayerInterface.GetInstance().GetLifeCounter();
        if ( lifeCounter != _curLifeCounter )
        {
            _curLifeCounter = lifeCounter;
            int i;
            for (i=0;i<_curLifeCounter.itemCount;i++)
            {
                _lifeImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "life1");
            }
            // 有残机碎片且残机没有达到最大值时，显示残机碎片
            if ( _curLifeCounter.fragmentCount > 0 && _curLifeCounter.itemCount != _curLifeCounter.maxItemCount )
            {
                _lifeImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "life" + _curLifeCounter.fragmentCount + "-" + _curLifeCounter.maxFragmentCount);
                i++;
            }
            for (;i<Consts.PlayerMaxLifeCount;i++)
            {
                _lifeImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "life0");
            }
        }
    }

    private void UpdateSpellCardValue()
    {
        ItemWithFramentsCounter spellCardCounter = PlayerInterface.GetInstance().GetSpellCardCounter();
        if (spellCardCounter != _curSpellCardCounter)
        {
            _curSpellCardCounter = spellCardCounter;
            int i;
            for (i = 0; i < _curSpellCardCounter.itemCount; i++)
            {
                _spellCardImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "sc1");
            }
            // 有符卡碎片且符卡没有达到最大值时，显示符卡碎片
            if (_curSpellCardCounter.fragmentCount > 0 && _curSpellCardCounter.itemCount != _curSpellCardCounter.maxItemCount)
            {
                _spellCardImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "sc" + _curSpellCardCounter.fragmentCount + "-" + _curSpellCardCounter.maxFragmentCount);
                i++;
            }
            for (; i < Consts.PlayerMaxSpellCardCount; i++)
            {
                _spellCardImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "sc0");
            }
        }
    }

    private void UpdateFps()
    {
        _updateFrameTimer++;
        _curTimeTickIndex = _curTimeTickIndex + 1 >= TickArrSize ? 0 : _curTimeTickIndex + 1;
        _frameTimeTick[_curTimeTickIndex] = Stopwatch.GetTimestamp();
        if (_lastTimeTickIndex == -1)
        {
            _lastTimeTickIndex = 0;
            return;
        }
        if (_frameTimeTick[_curTimeTickIndex] - _frameTimeTick[_lastTimeTickIndex] >= 20000000)
        {
            int frameCount = (_curTimeTickIndex + TickArrSize - _lastTimeTickIndex) % TickArrSize;
            double fps = frameCount * 10000000d / (_frameTimeTick[_curTimeTickIndex] - _frameTimeTick[_lastTimeTickIndex]);
            fps = Math.Round(fps, 1);
            if (_updateFrameTimer >= UpdateFpsInterval)
            {
                _fpsText.text = fps + "fps";
                _updateFrameTimer = 0;
            }
            _lastTimeTickIndex = _lastTimeTickIndex + 1 >= TickArrSize ? 0 : _lastTimeTickIndex + 1;
        }
    }

    public override void OnHide()
    {
        int i;
        // clear擦弹部分
        for (i=0;i<GrazeBitMaxCount;i++)
        {
            _grazeImgList[i].gameObject.SetActive(false);
        }
        _grazeBitCount = 0;
        _curGraze = -1;
        CommandManager.GetInstance().Remove(CommandConsts.ShowBossPosHint, this);
        foreach (var value in _bossHintTfDic.Values)
        {
            _bossHintTfPool.Push(value);
        }
        _bossHintTfDic.Clear();
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Bottom;
    }
}
