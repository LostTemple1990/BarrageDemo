using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class STGPauseView : ViewBase
{
    /// <summary>
    /// 暂停界面
    /// </summary>
    private const int StatePause = 1;
    /// <summary>
    /// 确认界面
    /// </summary>
    private const int StateConfirm = 2;
    /// <summary>
    /// 当前可选择的Item数目
    /// </summary>
    private const int SelectItemsCount = 4;

    private const int IndexReturnToGame = 0;
    private const int IndexBackToTitle = 1;
    private const int IndexSaveReplay = 2;
    private const int IndexRetry = 3;

    private const int IndexYes = 0;
    private const int IndexNo = 1;

    private List<GameObject> _imgList;
    private List<Vector3> _imgMovementDatas;
    /// <summary>
    /// 是、否选项的Panel
    /// </summary>
    private GameObject _yesNoPanel;

    private bool _isShowAniFinish;

    /// <summary>
    /// 可选择的项目list
    /// </summary>
    private List<GameObject> _selectItems;
    /// <summary>
    /// 当前选择项目的index
    /// </summary>
    private int _curSelectIndex;

    private List<GameObject> _yesNoItems;

    private int _curYesNoIndex;
    /// <summary>
    /// 当前选择的项目
    /// </summary>
    private GameObject _curSelectItem;
    /// <summary>
    /// 当前选择的是、否
    /// </summary>
    private GameObject _curYesNoItem;

    private int _state;
    /// <summary>
    /// 可用的item索引
    /// <para>按位统计，为1说明该索引可用</para>
    /// </summary>
    private int _availableIndex;
    /// <summary>
    /// 总共显示的item的个数
    /// </summary>
    private int _showItemCount;
    /// <summary>
    /// 可选择的item的索引，值i对应_selectItems[i]
    /// </summary>
    private List<int> _availableList;
    /// <summary>
    /// 可选择的item的个数
    /// </summary>
    private int _availableCount;
    /// <summary>
    /// 当前选择的item在availableList中对应的索引
    /// </summary>
    private int _curSelectAvailableIndex;
    /// <summary>
    /// 暂停的状态
    /// </summary>
    private ePauseViewState _pauseState;

    public override void Init(GameObject viewObj)
    {
        base.Init(viewObj);
        _imgList = new List<GameObject>();
        _imgMovementDatas = new List<Vector3>();
        AddOnShowAniDatas();
        _selectItems = _imgList.GetRange(1, 4);
        // 初始化是、否面板
        _yesNoPanel = _viewTf.Find("YesNoPanel").gameObject;
        _yesNoItems = new List<GameObject>();
        _yesNoItems.Add(_yesNoPanel.transform.Find("Yes").gameObject);
        _yesNoItems.Add(_yesNoPanel.transform.Find("No").gameObject);
    }

    public override void OnShow(object data)
    {
        _isShowAniFinish = false;
        _state = StatePause;
        // 隐藏yesno面板
        _yesNoPanel.SetActive(false);
        _pauseState = data == null ? 0 : (ePauseViewState)data;
        InitAvailableItems(_pauseState);
        PlayShowAni();
        UIManager.GetInstance().RegisterViewUpdate(this);
    }

    public override void OnHide()
    {
        TweenManager.GetInstance().RemoveTweenByGo(_curSelectItem);
        ResetImgColor(_curSelectItem);
        _curSelectItem = null;
        if ( _curYesNoItem != null )
        {
            TweenManager.GetInstance().RemoveTweenByGo(_curYesNoItem);
            ResetImgColor(_curYesNoItem);
            _curYesNoItem = null;
        }
        _availableList.Clear();
    }

    /// <summary>
    /// 添加打开界面时候的移动数据
    /// </summary>
    private void AddOnShowAniDatas()
    {
        // 暂停title
        _imgList.Add(_viewTf.Find("ImgPause").gameObject);
        _imgMovementDatas.Add(new Vector3(-135,200,0));
        _imgMovementDatas.Add(new Vector3(-135,30,0));
        // 返回游戏
        _imgList.Add(_viewTf.Find("ReturnToGame").gameObject);
        _imgMovementDatas.Add(new Vector3(80, 0, 0));
        _imgMovementDatas.Add(new Vector3(-120, 0, 0));
        // 返回标题画面
        _imgList.Add(_viewTf.Find("BackToTitle").gameObject);
        _imgMovementDatas.Add(new Vector3(90, -20, 0));
        _imgMovementDatas.Add(new Vector3(-110, -20, 0));
        // 保存replay
        _imgList.Add(_viewTf.Find("SaveReplay").gameObject);
        _imgMovementDatas.Add(new Vector3(100, -40, 0));
        _imgMovementDatas.Add(new Vector3(-100, -40, 0));
        // 重新开始
        _imgList.Add(_viewTf.Find("Retry").gameObject);
        _imgMovementDatas.Add(new Vector3(110, -60, 0));
        _imgMovementDatas.Add(new Vector3(-90, -60, 0));
    }

    /// <summary>
    /// 界面打开时根据状态来显示不同的item
    /// <para>0 普通暂停</para>
    /// <para>1 stage all clear之后的暂停</para>
    /// <para>2 播放录像时的暂停</para>
    /// <para>3 录像播放完毕之后的暂停</para>
    /// </summary>
    /// <param name="state"></param>
    private void InitAvailableItems(ePauseViewState state)
    {
        _availableIndex = 0;
        if (state == ePauseViewState.PauseInGame)
        {
            _availableIndex |= 1 << 0;
            _availableIndex |= 1 << 1;
            _availableIndex |= 1 << 2;
            _availableIndex |= 1 << 3;
            _showItemCount = 4;
        }
        else if (state == ePauseViewState.PauseAfterGameClear)
        {
            _availableIndex |= 1 << 1;
            _availableIndex |= 1 << 2;
            _availableIndex |= 1 << 3;
            _showItemCount = 4;
        }
        else if (state == ePauseViewState.PauseInReplay)
        {
            _availableIndex |= 1 << 0;
            _availableIndex |= 1 << 1;
            _availableIndex |= 1 << 3;
            _showItemCount = 4;
        }
        else if (state == ePauseViewState.PauseAfterReplayFinished)
        {
            _availableIndex |= 1 << 1;
            _availableIndex |= 1 << 3;
            _showItemCount = 4;
        }
        _availableList = new List<int>();
        Image img;
        RectTransform tf;
        for (int i=0;i<_showItemCount;i++)
        {
            tf = _selectItems[i].GetComponent<RectTransform>();
            img = tf.GetComponent<Image>();
            tf.anchoredPosition = new Vector2(2000, 2000);
            if ((_availableIndex & (1 << i)) != 0)
            {
                img.color = new Color(1, 1, 1, 1);
                _availableList.Add(i);
            }
            else
            {
                img.color = new Color(1, 1, 1, 0.3f);
            }
        }
        _availableCount = _availableList.Count;
    }

    /// <summary>
    /// 界面打开的时候的界面动画
    /// </summary>
    private void PlayShowAni()
    {
        int i;
        GameObject go;
        TweenPos3D tweenPos;
        TweenAlpha tweenAlpha;
        int dDelay = 60 / 5;
        int duration = dDelay;
        for (i=0;i<5;i++)
        {
            go = _imgList[i];
            // 位置
            tweenPos = TweenManager.GetInstance().Create<TweenPos3D>();
            tweenPos.SetParas(go, dDelay * i, duration, ePlayMode.Once);
            tweenPos.SetParas(_imgMovementDatas[i * 2], _imgMovementDatas[i * 2 + 1], InterpolationMode.EaseInQuad);
            if ( i == 4 )
            {
                tweenPos.SetFinishCallBack(ShowAniFinishHandler);
            }
            // 透明度
            tweenAlpha = TweenManager.GetInstance().Create<TweenAlpha>();
            tweenAlpha.SetParas(go, 0, dDelay * i, ePlayMode.Once);
            // todo 暂时使用取巧的写法
            float endAlpha = 1;
            if (i >= 1)
            {
                if ((_availableIndex & (1 << (i-1))) == 0)
                {
                    endAlpha = 0.3f;
                }
            }
            tweenAlpha.SetParas(0, endAlpha, InterpolationMode.None);
            TweenManager.GetInstance().AddTween(go, tweenPos);
            TweenManager.GetInstance().AddTween(go, tweenAlpha);
        }
    }

    private void ShowAniFinishHandler(GameObject go)
    {
        _isShowAniFinish = true;
        _state = StatePause;
        _curSelectAvailableIndex = 0;
        SetCurSelectItem(_availableList[0]);
    }

    private void SetCurSelectItem(int index)
    {
        if ( _curSelectItem != null )
        {
            // 移除当前选择的item的特效
            TweenManager.GetInstance().RemoveTweenByGo(_curSelectItem);
            ResetImgColor(_curSelectItem);
        }
        // 切换
        _curSelectIndex = index;
        _curSelectItem = _selectItems[index];
        // 添加特效
        TweenColor tween = TweenManager.GetInstance().Create<TweenColor>();
        tween.SetParas(_curSelectItem, 0, 60, ePlayMode.PingPong);
        tween.SetParas(new Color(1, 1, 1, 1), new Color(0.5f, 0, 0, 1), InterpolationMode.Linear);
        TweenManager.GetInstance().AddTween(_curSelectItem, tween);
    }

    /// <summary>
    /// index取值为0或者1，代表yes or no
    /// </summary>
    /// <param name="index"></param>
    private void SetCurYesNoItem(int index)
    {
        if (_curYesNoItem != null)
        {
            // 移除当前选择的item的特效
            TweenManager.GetInstance().RemoveTweenByGo(_curYesNoItem);
            ResetImgColor(_curYesNoItem);
        }
        // 切换
        _curYesNoIndex = index;
        _curYesNoItem = _yesNoItems[index];
        // 添加特效
        TweenColor tween = TweenManager.GetInstance().Create<TweenColor>();
        tween.SetParas(_curYesNoItem, 0, 60, ePlayMode.PingPong);
        tween.SetParas(new Color(1, 1, 1, 1), new Color(0.5f, 0, 0, 1), InterpolationMode.Linear);
        TweenManager.GetInstance().AddTween(tween);
    }

    public override void Update()
    {
        // 动画效果播放完之后才可以操作
        if ( _isShowAniFinish )
        {
            if ( Input.GetKeyDown(KeyCode.Escape) )
            {
                OnEscHandler();
            }
            else if ( Input.GetKeyDown(KeyCode.DownArrow) )
            {
                OnDirDownHandler();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) )
            {
                OnDirUpHandler();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                OnConfirmHandler();
            }
        }
    }

    /// <summary>
    /// 按下ESC键
    /// </summary>
    private void OnEscHandler()
    {
        // 若当前是子选择界面，返回上一级
        if (_state == StateConfirm)
        {
            _state = StatePause;
            TweenManager.GetInstance().RemoveTweenByGo(_curYesNoItem);
            _yesNoPanel.SetActive(false);
            SetCurSelectItem(_curSelectIndex);
        }
        // 若当前正在暂停界面，则直接返回游戏
        else if (_state == StatePause)
        {
            if (_pauseState == ePauseViewState.PauseAfterGameClear)
                return;
            if (_pauseState == ePauseViewState.PauseAfterReplayFinished)
                return;
            Hide();
            CommandManager.GetInstance().RunCommand(CommandConsts.ContinueGame);
        }
    }

    /// <summary>
    /// 按下确认键
    /// </summary>
    private void OnConfirmHandler()
    {
        if ( _state == StatePause )
        {
            if ( _curSelectIndex == IndexReturnToGame )
            {
                Hide();
                CommandManager.GetInstance().RunCommand(CommandConsts.ContinueGame);
            }
            else if ( _curSelectIndex == IndexBackToTitle )
            {
                _state = StateConfirm;
                _yesNoPanel.SetActive(true);
                TweenManager.GetInstance().RemoveTweenByGo(_curSelectItem);
                SetCurYesNoItem(IndexNo);
            }
            else if (_curSelectIndex == IndexSaveReplay)
            {
                _state = StateConfirm;
                _yesNoPanel.SetActive(true);
                TweenManager.GetInstance().RemoveTweenByGo(_curSelectItem);
                SetCurYesNoItem(IndexNo);
            }
            else if (_curSelectIndex == IndexRetry)
            {
                _state = StateConfirm;
                _yesNoPanel.SetActive(true);
                TweenManager.GetInstance().RemoveTweenByGo(_curSelectItem);
                SetCurYesNoItem(IndexNo);
            }
        }
        else if ( _state == StateConfirm )
        {
            // 返回上一级界面、即返回暂停界面
            if ( _curYesNoIndex == IndexNo )
            {
                _state = StatePause;
                TweenManager.GetInstance().RemoveTweenByGo(_curYesNoItem);
                _yesNoPanel.SetActive(false);
                SetCurSelectItem(_curSelectIndex);
            }
            else if ( _curYesNoIndex == IndexYes )
            {
                if (_curSelectIndex == IndexBackToTitle)
                {
                    OnBackToTitle();
                }
                else if (_curSelectIndex == IndexSaveReplay)
                {
                    OnSaveReplay();
                }
                else if (_curSelectIndex == IndexRetry)
                {
                    OnRetry();
                }
            }
        }
    }

    private void OnDirUpHandler()
    {
        if ( _state == StatePause )
        {
            _curSelectAvailableIndex = (_curSelectAvailableIndex - 1 + _availableCount) % _availableCount;
            SetCurSelectItem(_availableList[_curSelectAvailableIndex]);
        }
        else
        {
            SetCurYesNoItem(1 - _curYesNoIndex);
        }
    }

    private void OnDirDownHandler()
    {
        if ( _state == StatePause )
        {
            _curSelectAvailableIndex = (_curSelectAvailableIndex + 1) % _availableCount;
            SetCurSelectItem(_availableList[_curSelectAvailableIndex]);
        }
        else if ( _state == StateConfirm )
        {
            SetCurYesNoItem(1 - _curYesNoIndex);
        }
    }
    
    /// <summary>
    /// 返回标题界面
    /// </summary>
    private void OnBackToTitle()
    {
        CommandManager.GetInstance().RunCommand(CommandConsts.BackToTitle);
        Hide();
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    private void OnRetry()
    {
        CommandManager.GetInstance().RunCommand(CommandConsts.RetryGame);
        Hide();
    }

    private void OnSaveReplay()
    {
        UIManager.GetInstance().ShowView(WindowName.ReplayView, eReplayViewMode.Save);
        Hide();
    }

    /// <summary>
    /// 将img的color设置回(1,1,1,1)
    /// </summary>
    /// <param name="go"></param>
    private void ResetImgColor(GameObject go)
    {
        Image img = go.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 1);
    }

    public override LayerId GetLayerId()
    {
        return LayerId.GameInfo;
    }
}

public enum ePauseViewState : byte
{
    PauseInGame = 0,
    PauseAfterGameClear = 1,
    PauseInReplay = 2,
    PauseAfterReplayFinished = 3,
}
