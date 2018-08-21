using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationController
{
    private CharacterBase _character;
    private List<int> _dirInput;
    private Dictionary<KeyCode, int> _dirDec;

    public OperationController()
    {
        _dirInput = new List<int>();
        _dirDec = new Dictionary<KeyCode, int>();
        _dirDec.Add(KeyCode.UpArrow, Consts.DIR_UP);
        _dirDec.Add(KeyCode.DownArrow, Consts.DIR_DOWN);
        _dirDec.Add(KeyCode.LeftArrow, Consts.DIR_LEFT);
        _dirDec.Add(KeyCode.RightArrow, Consts.DIR_RIGHT);
    }

    public void InitCharacter(CharacterBase character)
    {
        _character = character;
    }

    public void Update()
    {
        CheckMove();
        CheckShoot();
        CheckBomb();
    }
    
    /// <summary>
    /// 检测移动
    /// </summary>
    private void CheckMove()
    {
        foreach (KeyValuePair<KeyCode,int> kv in _dirDec)
        {
            // 检测按下
            if (Input.GetKey(kv.Key))
            {
                if (_dirInput.IndexOf(kv.Value) == -1)
                {
                    _dirInput.Add(kv.Value);
                }
            }
            else if ( _dirInput.IndexOf(kv.Value) != -1 )
            {
                _dirInput.Remove(kv.Value);
            }
        }
        // 判定移动方向，相反方向的话根据后按的那个来进行
        int dir = Consts.DIR_NULL;
        // 上下
        int upIndex = _dirInput.IndexOf(Consts.DIR_UP);
        int downIndex = _dirInput.IndexOf(Consts.DIR_DOWN);
        if ( upIndex > downIndex )
        {
            dir |= Consts.DIR_UP;
        }
        else if ( upIndex < downIndex )
        {
            dir |= Consts.DIR_DOWN;
        }
        // 左右
        int leftIndex = _dirInput.IndexOf(Consts.DIR_LEFT);
        int rightIndex = _dirInput.IndexOf(Consts.DIR_RIGHT);
        if (leftIndex > rightIndex)
        {
            dir |= Consts.DIR_LEFT;
        }
        else if (leftIndex < rightIndex)
        {
            dir |= Consts.DIR_RIGHT;
        }
        int moveMode = Input.GetKey(KeyCode.LeftShift) ? Consts.SlowMove : Consts.SpeedMove;
        _character.InputMoveMode = moveMode;
        _character.InputDir = dir;
    }

    /// <summary>
    /// 检测射击
    /// </summary>
    private void CheckShoot()
    {
        if ( Input.GetKey(KeyCode.Z) )
        {
            _character.InputShoot = true;
        }
        else
        {
            _character.InputShoot = false;
        }
    }

    private void CheckBomb()
    {
        if (Input.GetKey(KeyCode.X))
        {
            _character.InputBomb = true;
        }
        else
        {
            _character.InputBomb = false;
        }
    }

    public void Clear()
    {
        _character = null;
    }
}
