using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationController
{
    private static OperationController _instance;
    public static OperationController GetInstance()
    {
        if (_instance == null)
        {
            _instance = new OperationController();
        }
        return _instance;
    }

    private const eSTGKey DirKeys = eSTGKey.KeyUp | eSTGKey.KeyDown | eSTGKey.KeyLeft | eSTGKey.KeyRight;

    private CharacterBase _character;
    /// <summary>
    /// 按照按键顺序记录的方向键的输入
    /// </summary>
    private List<eSTGKey> _dirInput;
    /// <summary>
    /// 四个方向键的list
    /// </summary>
    private List<eSTGKey> _dirKeyList;
    /// <summary>
    /// 上一帧的输入
    /// </summary>
    private eSTGKey _lastInput;
    /// <summary>
    /// 当前帧的输入
    /// </summary>
    private eSTGKey _curInput;

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
    private KeyCode _pCtrl;
    /// <summary>
    /// 每帧玩家的按键顺序
    /// </summary>
    private List<eSTGKey> _keyList;
    /// <summary>
    /// 有效的方向键输入
    /// </summary>
    private eSTGKey _availableDirKey;


    private OperationController()
    {
        _dirInput = new List<eSTGKey>();
        _dirKeyList = new List<eSTGKey> { eSTGKey.KeyUp, eSTGKey.KeyDown, eSTGKey.KeyLeft, eSTGKey.KeyRight };
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
        _pCtrl = KeyCode.LeftControl;
    }

    public void InitCharacter()
    {
        _character = PlayerInterface.GetInstance().GetCharacter();
        if (!Global.IsInReplayMode)
        {
            _keyList = new List<eSTGKey>();
        }
        else
        {
            _keyList = ReplayManager.GetInstance().GetReplayKeyList();
        }
        _lastInput = eSTGKey.None;
        _curInput = eSTGKey.None;
    }

    public void Update()
    {
        _lastInput = _curInput;
        GetInput();
        CheckMove();
        CheckShoot();
        CheckBomb();
    }

    /// <summary>
    /// 获取玩家当前的操作按键
    /// </summary>
    /// <returns></returns>
    public List<eSTGKey> GetOperationKeyList()
    {
        return _keyList;
    }

    /// <summary>
    /// 当前帧该按键是否处于按下状态
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetKey(eSTGKey key)
    {
        return (_curInput & key) != 0;
    }

    /// <summary>
    /// 当前帧该按键是否按下
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetKeyDown(eSTGKey key)
    {
        // 上一帧已经按下这个按键了，所以直接返回
        if ((_lastInput & key) != 0)
            return false;
        // 当前帧该键按下
        if ((_curInput & key) != 0)
            return true;
        return false;
    }

    /// <summary>
    /// 当前帧该按键是否松开
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool GetKeyUp(eSTGKey key)
    {
        // 上一帧该按键不在按下状态，直接返回
        if ((_lastInput & key) == 0)
            return false;
        // 当前帧该键按下
        if ((_curInput & key) == 0)
            return true;
        return false;
    }


    private void GetInput()
    {
        _curInput = eSTGKey.None; ;
        if ( Global.IsInReplayMode )
        {
            int curFrame = STGStageManager.GetInstance().GetFrameSinceStageStart();
            _curInput = _keyList[curFrame];
        }
        else
        {
            if (Input.GetKey(_pLeft)) _curInput |= eSTGKey.KeyLeft;
            if (Input.GetKey(_pRight)) _curInput |= eSTGKey.KeyRight;
            if (Input.GetKey(_pUp)) _curInput |= eSTGKey.KeyUp;
            if (Input.GetKey(_pDown)) _curInput |= eSTGKey.KeyDown;
            if (Input.GetKey(_pShift)) _curInput |= eSTGKey.KeyShift;
            if (Input.GetKey(_pZ)) _curInput |= eSTGKey.KeyZ;
            if (Input.GetKey(_pX)) _curInput |= eSTGKey.KeyX;
            if (Input.GetKey(_pC)) _curInput |= eSTGKey.KeyC;
            if (Input.GetKey(_pCtrl)) _curInput |= eSTGKey.KeyCtrl;
            // 添加到keyList中
            _keyList.Add(_curInput);
        }
    }
    
    /// <summary>
    /// 检测移动
    /// </summary>
    private void CheckMove()
    {
        foreach (eSTGKey dirKey in _dirKeyList)
        {
            // 检测按下
            if (GetKey(dirKey))
            {
                if (_dirInput.IndexOf(dirKey) == -1)
                {
                    _dirInput.Add(dirKey);
                }
            }
            else if (_dirInput.IndexOf(dirKey) != -1)
            {
                _dirInput.Remove(dirKey);
            }
        }
        // 判定移动方向，相反方向的话根据后按的那个来进行
        _availableDirKey = eSTGKey.None;
        // 上下
        int upIndex = _dirInput.IndexOf(eSTGKey.KeyUp);
        int downIndex = _dirInput.IndexOf(eSTGKey.KeyDown);
        if ( upIndex > downIndex )
        {
            _availableDirKey |= eSTGKey.KeyUp;
        }
        else if ( upIndex < downIndex )
        {
            _availableDirKey |= eSTGKey.KeyDown;
        }
        // 左右
        int leftIndex = _dirInput.IndexOf(eSTGKey.KeyLeft);
        int rightIndex = _dirInput.IndexOf(eSTGKey.KeyRight);
        if (leftIndex > rightIndex)
        {
            _availableDirKey |= eSTGKey.KeyLeft;
        }
        else if (leftIndex < rightIndex)
        {
            _availableDirKey |= eSTGKey.KeyRight;
        }
    }

    /// <summary>
    /// 在当前帧下key对应的方向键是否为有效输入
    /// <para>用于自机移动</para>
    /// </summary>
    /// <returns></returns>
    public bool IsDirKeyAvailable(eSTGKey key)
    {
        return (_availableDirKey & key) != 0;
    }

    public bool IsKeyAvailable(eSTGKey key)
    {
        if ((key & DirKeys) != 0)
            return IsDirKeyAvailable(key);
        return (_curInput & key) != 0;
    }

    /// <summary>
    /// 检测射击
    /// </summary>
    private void CheckShoot()
    {
        if ( (_curInput & eSTGKey.KeyZ) != 0 )
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
        if ( (_curInput & eSTGKey.KeyX) != 0 )
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
