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
    private float _curPower;

    public void Init()
    {
        _curPower = 2.95f;
        // todo 选择人物
        _character = new Reimu();
        _character.Init();
    }

    public CharacterBase GetCharacter()
    {
        return _character;
    }

    public float GetPower()
    {
        return _curPower;
    }

    public void AddPower(float value)
    {
        _curPower += value;
        if ( _curPower > 4 )
        {
            _curPower = 4;
        }
    }

    public void SetPower(float value)
    {
        _curPower = value;
    }
}
