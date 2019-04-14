using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationController
{
    private const int KeyLeft = 1 << 0;
    private const int KeyRight = 1 << 1;
    private const int KeyUp = 1 << 2;
    private const int KeyDown = 1 << 3;
    private const int KeyShift = 1 << 4;
    private const int KeyZ = 1 << 5;
    private const int KeyX = 1 << 6;
    private const int KeyC = 1 << 7;

    private CharacterBase _character;
    private List<int> _dirInput;
    private Dictionary<int, int> _dirDic;
    /// <summary>
    /// 当前帧的输入
    /// </summary>
    private int _curInput;

    /// <summary>
    /// 玩家设置的对应按键
    /// p --> short for player
    /// </summary>
    private KeyCode _pLeft;
    private KeyCode _pRight;
    private KeyCode _pUp;
    private KeyCode _pDown;
    private KeyCode _pShift;
    private KeyCode _pZ;
    private KeyCode _pX;
    private KeyCode _pC;


    public OperationController()
    {
        _dirInput = new List<int>();
        _dirDic = new Dictionary<int, int>();
        _dirDic.Add(KeyUp, Consts.DIR_UP);
        _dirDic.Add(KeyDown, Consts.DIR_DOWN);
        _dirDic.Add(KeyLeft, Consts.DIR_LEFT);
        _dirDic.Add(KeyRight, Consts.DIR_RIGHT);
        InitDefaultKeyCode();
    }

    private void InitDefaultKeyCode()
    {
        _pLeft = KeyCode.LeftArrow;
        _pRight = KeyCode.RightArrow;
        _pUp = KeyCode.UpArrow;
        _pDown = KeyCode.DownArrow;
        _pShift = KeyCode.LeftShift;
        _pZ = KeyCode.Z;
        _pX = KeyCode.X;
        _pC = KeyCode.C;
    }

    public void InitCharacter(CharacterBase character)
    {
        _character = character;
    }

    public void Update()
    {
        GetInput();
        CheckMove();
        CheckShoot();
        CheckBomb();
    }

    private void GetInput()
    {
        _curInput = 0;
        if ( Global.IsInReplayMode )
        {

        }
        else
        {
            if (Input.GetKey(_pLeft)) _curInput |= KeyLeft;
            if (Input.GetKey(_pRight)) _curInput |= KeyRight;
            if (Input.GetKey(_pUp)) _curInput |= KeyUp;
            if (Input.GetKey(_pDown)) _curInput |= KeyDown;
            if (Input.GetKey(_pShift)) _curInput |= KeyShift;
            if (Input.GetKey(_pZ)) _curInput |= KeyZ;
            if (Input.GetKey(_pX)) _curInput |= KeyX;
            if (Input.GetKey(_pC)) _curInput |= KeyC;
        }
    }
    
    /// <summary>
    /// 检测移动
    /// </summary>
    private void CheckMove()
    {
        foreach (KeyValuePair<int,int> kv in _dirDic)
        {
            // 检测按下
            if ( (_curInput & kv.Key) != 0 )
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
        int moveMode = (_curInput & KeyShift) != 0 ? Consts.MoveModeLowSpeed : Consts.MoveModeHighSpeed;
        _character.InputMoveMode = moveMode;
        _character.InputDir = dir;
    }

    /// <summary>
    /// 检测射击
    /// </summary>
    private void CheckShoot()
    {
        if ( (_curInput & KeyZ) != 0 )
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
        if ( (_curInput & KeyX) != 0 )
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
