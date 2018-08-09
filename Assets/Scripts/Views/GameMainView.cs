using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameMainView : ViewBase,ICommand
{
    private const string BigNumStr = "BigNum";
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

    private int _curPower;

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        _powerInt = _viewTf.Find("Bg/BgRight/Power/PowerValue/PowerValueInt").GetComponent<Image>();
        _powerBit1 = _viewTf.Find("Bg/BgRight/Power/PowerValue/PowerValueB1").GetComponent<Image>();
        _powerBit2 = _viewTf.Find("Bg/BgRight/Power/PowerValue/PowerValueB2").GetComponent<Image>();
        UIManager.GetInstance().RegisterViewUpdate(this);
        _curPower = -1;
    }

    public void Execute(int cmd,object[] args)
    {

    }

    public override void Update()
    {
        UpdatePowerValue();
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
}
