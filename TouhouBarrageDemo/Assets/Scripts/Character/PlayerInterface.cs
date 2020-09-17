using UnityEngine;
using System.Collections.Generic;

public class PlayerInterface
{
    private static PlayerInterface _instance = new PlayerInterface();

    public static PlayerInterface GetInstance()
    {
        return _instance;
    }

    private CharacterBase _character;
    private int _curPower;
    /// <summary>
    /// 擦弹数目
    /// </summary>
    private int _graze;
    /// <summary>
    /// 残机计数器
    /// </summary>
    private ItemWithFramentsCounter _lifeCounter;
    /// <summary>
    /// 符卡计数器
    /// </summary>
    private ItemWithFramentsCounter _spellCardCounter;
    /// <summary>
    /// 信号数值
    /// </summary>
    private float _signalValue;
    /// <summary>
    /// 信仰值
    /// </summary>
    private float _faithValue;

    public PlayerInterface()
    {
        _lifeCounter = new ItemWithFramentsCounter
        {
            maxItemCount = Consts.PlayerMaxLifeCount,
            maxFragmentCount = Consts.PlayerMaxLifeFragmentCount,
        };
        _spellCardCounter = new ItemWithFramentsCounter
        {
            maxItemCount = Consts.PlayerMaxSpellCardCount,
            maxFragmentCount = Consts.PlayerMaxSpellCardFragmentCount,
        };
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public CharacterBase CreateCharacter(int index)
    {
        _curPower = Consts.PlayerInitPower;
        _graze = 0;
        _signalValue = 0;
        if (index == 0)
        {
            _character = new Reimu();
        }
        else if (index == 1)
        {
            _character = new MarisaA();
        }
        if (_character == null)
        {
            throw new System.Exception("invalid index of character!");
        }
        _character.Init();
        return _character;
    }

    /// <summary>
    /// 根据id获取角色名称
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string GetCharacterNameByIndex(int index)
    {
        if (index == 0)
            return "ReimuA";
        else if (index == 1)
            return "MarisaA";
        throw new System.Exception("invalid index of character!");
    }

    public CharacterBase GetCharacter()
    {
        return _character;
    }

    public int GetPower()
    {
        return _curPower;
    }

    public void AddPower(int value)
    {
        if (_curPower >= Consts.PlayerMaxPower)
            return;
        _curPower += value;
        if ( _curPower > Consts.PlayerMaxPower )
        {
            _curPower = Consts.PlayerMaxPower;
        }
    }

    public void SetPower(int value)
    {
        _curPower = value;
    }

    public void SetGraze(int value)
    {
        _graze = value;
    }

    public void AddGraze(int value)
    {
        _graze += value;
        _character.PlayGrazeEffect();
        AddToSignalValue(value * 8);
    }

    /// <summary>
    /// 获取当前擦弹数
    /// </summary>
    /// <returns></returns>
    public int GetGraze()
    {
        return _graze;
    }

    #region 残机相关
    /// <summary>
    /// 设置残机数目
    /// </summary>
    /// <param name="count">残机</param>
    /// <param name="fragmentCount">残机碎片</param>
    public void SetLifeCounter(int count,int fragmentCount)
    {
        _lifeCounter.itemCount = 0;
        _lifeCounter.fragmentCount = 0;
        _lifeCounter.AddItemCount(count);
        _lifeCounter.AddFragmentCount(fragmentCount);
    }

    public ItemWithFramentsCounter GetLifeCounter()
    {
        return _lifeCounter;
    }

    /// <summary>
    /// 增加残机
    /// </summary>
    /// <param name="count"></param>
    public void AddLifeCount(int count)
    {
        int pre = _lifeCounter.itemCount;
        _lifeCounter.AddItemCount(count);
        if (pre < _lifeCounter.itemCount)
            CommandManager.GetInstance().RunCommand(CommandConsts.PlayerExtend);
    }

    /// <summary>
    /// 增加残机碎片
    /// </summary>
    /// <param name="fragmentCount"></param>
    public void AddLifeFragmentCount(int fragmentCount)
    {
        int pre = _lifeCounter.itemCount;
        _lifeCounter.AddFragmentCount(fragmentCount);
        if (pre < _lifeCounter.itemCount)
            CommandManager.GetInstance().RunCommand(CommandConsts.PlayerExtend);
    }

    /// <summary>
    /// 自机Miss
    /// <para>返回值为false说明残机为0</para>
    /// </summary>
    public bool Miss()
    {
        // power减少100
        _curPower = _curPower - 100;
        if (_curPower < 0)
            _curPower = 0;
        // B大于3时不变，小于3时设置为3
        if (_spellCardCounter.itemCount < 3)
        {
            SetSpellCardCounter(3, 0);
        }
        List<int> items;
        if (_lifeCounter.itemCount == 1)
        {
            // 剩余1残的时候掉落满P点
            items = new List<int> { (int)ItemType.PowerFull, 1 };
        }
        else
        {
            // 掉落5个小P点道具
            items = new List<int> { (int)ItemType.PowerNormal, 5 };
        }
        Vector2 pos = _character.GetPosition();
        ItemManager.GetInstance().DropItems(items, pos.x, pos.y + 130, 80, 80);
        return _lifeCounter.CostItem(1);
    }
    #endregion

    #region 符卡相关
    /// <summary>
    /// 设置符卡数目
    /// </summary>
    /// <param name="count">符卡</param>
    /// <param name="fragmentCount">符卡碎片</param>
    public void SetSpellCardCounter(int count, int fragmentCount)
    {
        _spellCardCounter.itemCount = 0;
        _spellCardCounter.fragmentCount = 0;
        _spellCardCounter.AddItemCount(count);
        _spellCardCounter.AddFragmentCount(fragmentCount);
    }

    public ItemWithFramentsCounter GetSpellCardCounter()
    {
        return _spellCardCounter;
    }

    /// <summary>
    /// 增加符卡
    /// </summary>
    /// <param name="count"></param>
    public void AddSpellCardCount(int count)
    {
        _spellCardCounter.AddItemCount(count);
    }

    /// <summary>
    /// 增加残符卡碎片
    /// </summary>
    /// <param name="fragmentCount"></param>
    public void AddSpellCardFragmentCount(int fragmentCount)
    {
        _spellCardCounter.AddFragmentCount(fragmentCount);
    }

    /// <summary>
    /// 自机使用符卡
    /// </summary>
    /// <returns>返回值为false说明无法使用符卡</returns>
    public bool CastSpellCard()
    {
        return _spellCardCounter.CostItem(1);
    }
    #endregion

    /// <summary>
    /// 设置信号值
    /// </summary>
    /// <param name="value"></param>
    public void SetSignalValue(float value)
    {
        _signalValue = Mathf.Clamp(value, 0, Consts.MaxSignalValue);
    }

    /// <summary>
    /// 增加信号值
    /// </summary>
    /// <param name="value">增加的值</param>
    public void AddToSignalValue(float value)
    {
        _signalValue += value;
        _signalValue = Mathf.Clamp(value, 0, Consts.MaxSignalValue);
    }

    /// <summary>
    /// 取得当前信号值
    /// </summary>
    /// <returns></returns>
    public float GetSignalValue()
    {
        return _signalValue;
    }

    public void Clear()
    {
        _character.Clear();
        _character = null;
    }
}
