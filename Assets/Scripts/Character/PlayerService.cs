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

    public void Init()
    {
        //_curPower = Consts.PlayerInitPower;
        _curPower = Consts.PlayerInitPower;
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
}
