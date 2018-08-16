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

    private int _curPower;

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
        UIManager.GetInstance().RegisterViewUpdate(this);
        _curPower = -1;
    }

    public void Execute(int cmd,object[] args)
    {

    }

    public override void Update()
    {
        UpdatePowerValue();
        UpdateGrazeValue();
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
        return LayerId.GameUI;
    }
}
