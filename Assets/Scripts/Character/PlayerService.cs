using UnityEngine;
using System.Collections;

public class PlayerService
{
    private static PlayerService _instance = new PlayerService();

    public static PlayerService GetInstance()
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

    public PlayerService()
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

    public void Init()
    {
        _curPower = Consts.PlayerInitPower;
        _graze = 0;
        // todo 选择人物
        _character = new Reimu();
        _character.Init();
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
        _lifeCounter.AddItemCount(count);
    }

    /// <summary>
    /// 增加残机碎片
    /// </summary>
    /// <param name="fragmentCount"></param>
    public void AddLifeFragmentCount(int fragmentCount)
    {
        _lifeCounter.AddFragmentCount(fragmentCount);
    }

    /// <summary>
    /// 自机Miss
    /// </summary>
    /// <returns>返回值为false说明残机为0</returns>
    public bool Miss()
    {
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

    public void Clear()
    {
        _character.Clear();
        _character = null;
    }
}
