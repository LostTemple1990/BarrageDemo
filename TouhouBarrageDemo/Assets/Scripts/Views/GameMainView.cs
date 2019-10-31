using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameMainView : ViewBase,ICommand
{
    private const string BigNumStr = "BigNum";
    /// <summary>
    /// 擦弹最大位数
    /// </summary>
    private const int GrazeBitMaxCount = 6;
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
    }

    public override void OnShow(object data)
    {
        base.OnShow(data);
        // 重新初始化
        _curLifeCounter = new ItemWithFramentsCounter();
        _curSpellCardCounter = new ItemWithFramentsCounter();
        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    public void Execute(int cmd,object data)
    {

    }

    public override void Update()
    {
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
        int power = PlayerService.GetInstance().GetPower();
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
        int graze = PlayerService.GetInstance().GetGraze();
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
        ItemWithFramentsCounter lifeCounter = PlayerService.GetInstance().GetLifeCounter();
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
                _lifeImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "life" + _curLifeCounter.fragmentCount + "/" + _curLifeCounter.maxFragmentCount);
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
        ItemWithFramentsCounter spellCardCounter = PlayerService.GetInstance().GetSpellCardCounter();
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
                _spellCardImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "sc" + _curSpellCardCounter.fragmentCount + "/" + _curSpellCardCounter.maxFragmentCount);
                i++;
            }
            for (; i < Consts.PlayerMaxSpellCardCount; i++)
            {
                _spellCardImgList[i].sprite = ResourceManager.GetInstance().GetSprite(Consts.STGMainViewAtlasName, "sc0");
            }
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
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameUI_Normal;
    }
}
