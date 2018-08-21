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

    public void Clear()
    {
        _character.Clear();
        _character = null;
    }
}
